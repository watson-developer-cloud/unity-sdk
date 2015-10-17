/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IBM.Watson.Widgets
{

    /// <summary>
    /// This is the base class for all widgets. A Widget has any number of inputs & outputs that carry a specific type of data.
    /// </summary>
    public abstract class Widget : MonoBehaviour
    {
        #region Public Types
        /// <summary>
        /// This is the base class for any type of data that can be passed through a connection.
        /// </summary>
        public abstract class Data
        {
            public string Name { get { return GetName(); } }

            /// <summary>
            /// Should return a print friendly name for this data.
            /// </summary>
            /// <returns>Returns a user-friendly name for this type of data.</returns>
            public abstract string GetName();
        };

        public delegate void OnReceiveData(Data data);

        /// <summary>
        /// This object handles input on a widget.
        /// </summary>
        [Serializable]
        public class Input 
        {
            #region Construction
            public Input(string name, Type dataType, string receiverFunction)
            {
                InputName = name;
                DataType = dataType;
                ReceiverFunction = receiverFunction;
            }
            #endregion

            #region Public Properties
            public Widget Owner { get; set; }
            public string OwnerName { get { return Owner.WidgetName; } }
            public string InputName { get; set; }
            public string FullInputName { get { return OwnerName + "/" + InputName; } }
            public Type DataType { get; set; }
            public string DataTypeName { get { return DataType.Name; } }
            public string ReceiverFunction { get; set; }
            public OnReceiveData DataReceiver { get; set; }
            #endregion

            #region Public Functions
            /// <summary>
            /// Start this Input.
            /// </summary>
            /// <param name="owner">The owning widget.</param>
            public virtual void Start(Widget owner)
            {
                Owner = owner;

                if (!string.IsNullOrEmpty(ReceiverFunction))
                {
                    MethodInfo info = Owner.GetType().GetMethod(ReceiverFunction, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                        BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod);
                    if (info != null)
                    {
                        DataReceiver = Delegate.CreateDelegate(typeof(OnReceiveData), Owner, info) as OnReceiveData;
                        if ( DataReceiver == null )
                            Log.Error( "Widget", "CreateDelegate failed for function {0}", ReceiverFunction );
                    }
                    else
                        Log.Error( "Widget", "Failed to find receiver function {0}.", ReceiverFunction );
                }
            }

            /// <summary>
            /// Receives input and forwards that input onto the assigned delegate. Optionally, the user may inherit from Input and
            /// override the ReceiveData() function.
            /// </summary>
            /// <param name="data">The received data object.</param>
            public virtual void ReceiveData(Data data)
            {
                if (DataReceiver != null)
                    DataReceiver(data);
            }
            #endregion
        };

        /// <summary>
        /// This object handles output on a widget.
        /// </summary>
        [Serializable]
        public class Output
        {
            #region Constructor
            public Output( Type dataType)
            {
                DataType = dataType;
            }
            #endregion

            #region Public Properties
            public Widget Owner { get; set; }
            public Type DataType { get; set; }

            public Input TargetInput { get { return m_TargetInput; } set { m_TargetInput = value; m_TargetInputResolved = true; } }
            public GameObject TargetObject { get { return m_TargetObject; } set { m_TargetObject = value; m_TargetInputResolved = false; } }
            public string TargetConnection { get { return m_TargetConnection; } set { m_TargetConnection = value; m_TargetInputResolved = false; } }
            #endregion

            #region Public Functions
            /// <summary>
            /// Starts this Output.
            /// </summary>
            /// <param name="owner"></param>
            public virtual void Start(Widget owner)
            {
                Owner = owner;
            }
            /// <summary>
            /// Sends a data object to the target of this output.
            /// </summary>
            /// <param name="data">Data object to send.</param>
            public virtual bool SendData(Data data)
            {
                if (ResolveTargetInput( data ))
                {
                    try
                    {
                        m_TargetInput.ReceiveData(data);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Widget", "Exception sending data {0} to input {1} on object {2}: {3}",
                            data.Name, m_TargetInput.InputName, m_TargetObject.name, e.ToString());
                    }
                }

                return false;
            }
            #endregion

            #region Private Functions
            private bool ResolveTargetInput( Data data )
            {
                if (!m_TargetInputResolved)
                {
                    m_TargetInputResolved = true;
                    m_TargetInput = null;

                    if (m_TargetObject == null)
                        return false;

                    string inputName = m_TargetConnection;
                    string componentName = null;

                    int seperator = inputName.IndexOf('/');
                    if (seperator >= 0)
                    {
                        componentName = inputName.Substring(0, seperator);
                        inputName = inputName.Substring(seperator + 1);
                    }

                    Widget[] widgets = m_TargetObject.GetComponents<Widget>();
                    foreach (var widget in widgets)
                    {
                        if (!string.IsNullOrEmpty(componentName)
                            && widget.WidgetName != componentName)
                            continue;   // widget is the wrong type, the target is not here..

                        foreach (Input input in widget.Inputs)
                        {
                            if ( !string.IsNullOrEmpty( inputName )
                                && input.InputName != inputName)
                                continue;
                           if ( input.DataType != data.GetType() )
                                continue;

                            m_TargetInput = input;
                            return true;
                        }
                    }

                    Log.Error("Widget", "Failed to resolve target {0} for object {1}.", m_TargetConnection, m_TargetObject.name);
                    return false;
                }

                return m_TargetInput != null;
            }
            #endregion

            #region Private Data
            [SerializeField]
            private GameObject m_TargetObject = null;
            [SerializeField]
            private string m_TargetConnection = null;

            private bool m_TargetInputResolved = false;
            private Input m_TargetInput = null;
            #endregion
        };
        #endregion

        #region Private Data
        private Input[] m_Inputs = null;
        private Output[] m_Outputs = null;
        #endregion

        #region Public Properties
        public string WidgetName { get { return GetName(); } }
        public Input[] Inputs
        {
            get
            {
                if (m_Inputs == null)
                {
                    m_Inputs = GetMembersByType<Input>();
                    foreach (var input in m_Inputs)
                        input.Start(this);
                }
                return m_Inputs;
            }
        }
        public Output[] Outputs
        {
            get
            {
                if (m_Outputs == null)
                {
                    m_Outputs = GetMembersByType<Output>();
                    foreach (var output in m_Outputs)
                        output.Start(this);
                }
                return m_Outputs;
            }
        }
        #endregion

        #region Private Functions
        private T[] GetMembersByType<T>() where T : class
        {
            List<T> inputs = new List<T>();

            MemberInfo[] members = GetType().GetMembers(BindingFlags.Instance | BindingFlags.GetField
                | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MemberInfo info in members)
            {
                FieldInfo field = info as FieldInfo;
                if (field == null)
                    continue;
                if (!typeof(T).IsAssignableFrom(field.FieldType))
                    continue;
                T input = field.GetValue(this) as T;
                if (input == null)
                    throw new WatsonException("Expected a Connection type.");

                inputs.Add(input);
            }

            return inputs.ToArray();
        }
        #endregion

        #region Widget Interface
        protected abstract string GetName();
        #endregion
    }
}
