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
    /// This is the base class for all widgets. A Widget has any number of inputs and outputs that carry a specific type of data.
    /// </summary>
    public abstract class Widget : MonoBehaviour
    {
        #region Public Types
        /// <summary>
        /// This is the base class for any type of data that can be passed through a connection.
        /// </summary>
        public abstract class Data
        {
            /// <summary>
            /// Name of this data type.
            /// </summary>
            public string Name { get { return GetName(); } }

            /// <summary>
            /// Should return a print friendly name for this data.
            /// </summary>
            /// <returns>Returns a user-friendly name for this type of data.</returns>
            public abstract string GetName();
        };

        /// <summary>
        /// The callback object used by Widget for receiving data from an input.
        /// </summary>
        /// <param name="data"></param>
        public delegate void OnReceiveData(Data data);

        /// <summary>
        /// This object handles input on a widget.
        /// </summary>
        [Serializable]
        public class Input
        {
            #region Construction
            /// <summary>
            /// Constructs an input object for a Widget.
            /// </summary>
            /// <param name="name">The name of the input.</param>
            /// <param name="dataType">The type of data the input takes.</param>
            /// <param name="receiverFunction">The name of the function to invoke with the input. The input function must match
            /// the OnReceiveData callback.</param>
            public Input(string name, Type dataType, string receiverFunction)
            {
                InputName = name;
                DataType = dataType;
                ReceiverFunction = receiverFunction;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// A reference to the widget that contains this input, this is initialized when the Widget starts.
            /// </summary>
            public Widget Owner { get; set; }
            /// <summary>
            /// The name of the owning widget.
            /// </summary>
            public string OwnerName { get { return Owner.WidgetName; } }
            /// <summary>
            /// The name of this input.
            /// </summary>
            public string InputName { get; set; }
            /// <summary>
            /// The fully qualified name of this input.
            /// </summary>
            public string FullInputName { get { return OwnerName + "/" + InputName; } }
            /// <summary>
            /// The type of data this input accepts.
            /// </summary>
            public Type DataType { get; set; }
            /// <summary>
            /// The name of the data type.
            /// </summary>
            public string DataTypeName { get { return DataType.Name; } }
            /// <summary>
            /// The name of the receiver function.
            /// </summary>
            public string ReceiverFunction { get; set; }
            /// <summary>
            /// The delegate to the receiver function, this is set when Start() is called on this input.
            /// </summary>
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
                        if (DataReceiver == null)
                            Log.Error("Widget", "CreateDelegate failed for function {0}", ReceiverFunction);
                    }
                    else
                        Log.Error("Widget", "Failed to find receiver function {0}.", ReceiverFunction);
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
            /// <summary>
            /// The constructor for an widget output object.
            /// </summary>
            /// <param name="dataType">The type of data this widget outputs.</param>
            public Output(Type dataType)
            {
                DataType = dataType;
            }
            #endregion

            #region Public Properties
            /// <summary>
            /// Returns true if this output is connected to a input.
            /// </summary>
            public bool IsConnected { get { return ResolveTargetInput(); } }
            /// <summary>
            /// Returns a reference to the Widget owner, this is set when the Widget initializes.
            /// </summary>
            public Widget Owner { get; set; }
            /// <summary>
            /// The type of data this output sends.
            /// </summary>
            public Type DataType { get; set; }
            /// <summary>
            /// This returns a reference to the target input object.
            /// </summary>
            public Input TargetInput { get { return m_TargetInput; } set { m_TargetInput = value; m_TargetInputResolved = true; } }
            /// <summary>
            /// This returns a reference to the target object.
            /// </summary>
            public GameObject TargetObject { get { return m_TargetObject; } set { m_TargetObject = value; m_TargetInputResolved = false; } }
            /// <summary>
            /// The name of the target connection on the target object.
            /// </summary>
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
                if (ResolveTargetInput())
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
            private bool ResolveTargetInput()
            {
                if (!m_TargetInputResolved)
                {
                    m_TargetInputResolved = true;
                    m_TargetInput = null;

                    // if we have no target object, then default to our own game object..
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
                            if (!string.IsNullOrEmpty(inputName)
                                && input.InputName != inputName)
                                continue;
                            if (input.DataType != this.DataType)
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
        private bool m_Initialized = false;
        private Input[] m_Inputs = null;
        private Output[] m_Outputs = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the name of this widget.
        /// </summary>
        public string WidgetName { get { return GetName(); } }
        /// <summary>
        /// This returns an array of all inputs on this widget.
        /// </summary>
        public Input[] Inputs { get { if (! m_Initialized ) InitializeIO(); return m_Inputs; } }
        /// <summary>
        /// This returns an array of all outputs on this widget.
        /// </summary>
        public Output[] Outputs { get { if (! m_Initialized ) InitializeIO(); return m_Outputs; } }
        #endregion

        #region Private Functions
        private void InitializeIO()
        {
            if (! m_Initialized )
            {
                m_Outputs = GetMembersByType<Output>();
                foreach (var output in m_Outputs)
                    output.Start(this);
                m_Inputs = GetMembersByType<Input>();
                foreach (var input in m_Inputs)
                    input.Start(this);
                m_Initialized = true;
            }
        }

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
        /// <exclude />
        protected virtual void Start()
        {
            InitializeIO();
        }
        #endregion

        #region Widget Interface
        /// <summary>
        /// Implemented to provide a friendly name for a widget object.
        /// </summary>
        /// <returns>A string name for this widget.</returns>
        protected abstract string GetName();
        #endregion
    }
}
