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
*/

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Widgets
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
      #region Private Data
      [NonSerialized]
      private List<Output> m_Connections = new List<Output>();
      #endregion

      #region Construction
      /// <summary>
      /// Constructs an input object for a Widget.
      /// </summary>
      /// <param name="name">The name of the input.</param>
      /// <param name="dataType">The type of data the input takes.</param>
      /// <param name="receiverFunction">The name of the function to invoke with the input. The input function must match
      /// <param name="allowMany">If true, then allow more than one output to connect to this input.</param>
      /// the OnReceiveData callback.</param>
      public Input(string name, Type dataType, string receiverFunction, bool allowMany = true)
      {
        InputName = name;
        DataType = dataType;
        ReceiverFunction = receiverFunction;
        AllowMany = allowMany;
      }
      #endregion

      #region Object Interface
      /// <exclude />
      public override string ToString()
      {
        return (Owner != null ? Owner.name : "null") + "." + InputName + " (" + DataType.Name + ")";
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
      public string InputName { get; private set; }
      /// <summary>
      /// The fully qualified name of this input.
      /// </summary>
      public string FullInputName { get { return OwnerName + "/" + InputName; } }
      /// <summary>
      /// The type of data this input accepts.
      /// </summary>
      public Type DataType { get; private set; }
      /// <summary>
      /// If true, then more than one output may connect to this input.
      /// </summary>
      public bool AllowMany { get; private set; }
      /// <summary>
      /// The array of outputs connected to this input.
      /// </summary>
      public Output[] Connections { get { return m_Connections.ToArray(); } }
      /// <summary>
      /// The name of the data type.
      /// </summary>
      public string DataTypeName { get { return DataType.Name; } }
      /// <summary>
      /// The name of the receiver function.
      /// </summary>
      public string ReceiverFunction { get; private set; }
      /// <summary>
      /// The delegate to the receiver function, this is set when Start() is called on this input.
      /// </summary>
      public OnReceiveData DataReceiver { get; private set; }
      #endregion

      #region Public Functions
      /// <summary>
      /// Add output to input.
      /// </summary>
      /// <param name="output"></param>
      /// <returns></returns>
      public bool AddOutput(Output output)
      {
        if (!AllowMany && m_Connections.Count > 0)
          return false;
        if (m_Connections.Contains(output))
          return false;
        m_Connections.Add(output);
        return true;
      }
      /// <summary>
      /// Remove the output.
      /// </summary>
      /// <param name="output"></param>
      /// <returns></returns>
      public bool RemoveOutput(Output output)
      {
        return m_Connections.Remove(output);
      }

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
            Log.Error("Widget", "Failed to find receiver function {0} in object {1}.", ReceiverFunction, Owner.gameObject.name);
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
      /// <summary>
      /// The connection between widgets.
      /// </summary>
      #region Public Types
      [Serializable]
      public class Connection
      {
        #region Private Data
        [NonSerialized]
        private Output m_Owner = null;
        [SerializeField]
        private GameObject m_TargetObject = null;
        [SerializeField]
        private string m_TargetConnection = null;
        [NonSerialized]
        private bool m_TargetInputResolved = false;
        [NonSerialized]
        private Input m_TargetInput = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// This returns a reference to the target object.
        /// </summary>
        public GameObject TargetObject { get { return m_TargetObject; } set { m_TargetObject = value; m_TargetInputResolved = false; } }
        /// <summary>
        /// The name of the target connection on the target object.
        /// </summary>
        public string TargetConnection { get { return m_TargetConnection; } set { m_TargetConnection = value; m_TargetInputResolved = false; } }
        /// <summary>
        /// This returns a reference to the target input object.
        /// </summary>
        public Input TargetInput
        {
          get { return m_TargetInput; }
          set
          {
            if (m_TargetInput != null)
              m_TargetInput.RemoveOutput(m_Owner);

            if (value != null && value.AddOutput(m_Owner))
            {
              m_TargetInput = value;
              m_TargetObject = m_TargetInput.Owner.gameObject;
              m_TargetConnection = m_TargetInput.FullInputName;
            }
            else
            {
              m_TargetObject = null;
              m_TargetConnection = string.Empty;
            }
            m_TargetInputResolved = true;
          }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Resolve the target input.
        /// </summary>
        /// <returns></returns>
        public bool ResolveTargetInput()
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
                if (!string.IsNullOrEmpty(inputName)
                    && input.InputName != inputName)
                  continue;
                if (input.DataType != m_Owner.DataType)
                  continue;
                if (!m_TargetInput.AddOutput(m_Owner))
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

        /// <summary>
        /// Start ouput.
        /// </summary>
        /// <param name="owner"></param>
        public void Start(Output owner)
        {
          m_Owner = owner;
        }
        #endregion
      };
      #endregion

      #region Constructor
      /// <summary>
      /// The constructor for an widget output object.
      /// </summary>
      /// <param name="dataType">The type of data this widget outputs.</param>
      /// <param name="allowMany">If true, then this output will connect to more than one input.</param>
      public Output(Type dataType, bool allowMany = false)
      {
        DataType = dataType;
        AllowMany = allowMany;
      }
      #endregion

      #region Object Interface
      /// <exclude />
      public override string ToString()
      {
        return (Owner != null ? Owner.name : "null") + " (" + DataType.Name + ")";
      }
      #endregion

      #region Public Properties
      /// <summary>
      /// Returns true if this output is connected to a input.
      /// </summary>
      public bool IsConnected
      {
        get
        {
          foreach (var c in m_Connections)
            if (c.ResolveTargetInput())
              return true;
          return false;
        }
      }

      /// <summary>
      /// Connections between widgets.
      /// </summary>
      public Connection[] Connections { get { return m_Connections.ToArray(); } }
      /// <summary>
      /// Returns a reference to the Widget owner, this is set when the Widget initializes.
      /// </summary>
      public Widget Owner { get; set; }
      /// <summary>
      /// The type of data this output sends.
      /// </summary>
      public Type DataType { get; set; }
      /// <summary>
      /// If true, allows more than one input to be connected to this output.
      /// </summary>
      public bool AllowMany { get; private set; }
      #endregion

      #region Public Functions
      /// <summary>
      /// Starts this Output.
      /// </summary>
      /// <param name="owner">The Widget owner of this Output.</param>
      public virtual void Start(Widget owner)
      {
        Owner = owner;
        foreach (var c in m_Connections)
          c.Start(this);
      }
      /// <summary>
      /// Sends a data object to the target of this output.
      /// </summary>
      /// <param name="data">Data object to send.</param>
      /// <returns>Returns true if the data was sent to another widget.</returns>
      public virtual bool SendData(Data data)
      {
        bool sent = false;
        foreach (var c in m_Connections)
        {
          if (c.ResolveTargetInput())
          {
            try
            {
              c.TargetInput.ReceiveData(data);
              sent = true;
            }
            catch (Exception e)
            {
              Log.Error("Widget", "Exception sending data {0} to input {1} on object {2}: {3}",
                  data.Name, c.TargetInput.InputName, c.TargetObject.name, e.ToString());
            }
          }
        }
        return sent;
      }

      /// <summary>
      /// Add a connection to this output, returns false if the connection can't be made.
      /// </summary>
      /// <param name="input">A reference to the connection to establish.</param>
      /// <returns>Returns true on success.</returns>
      public bool AddConnection(Input input)
      {
        if (!AllowMany && m_Connections.Count > 0)
          return false;       // already connected.
        if (input.DataType != DataType)
          return false;       // wrong data type

        Connection c = new Connection();
        c.Start(this);
        c.TargetInput = input;
        m_Connections.Add(c);

        return true;
      }

      /// <summary>
      /// Add a connect to a given object and optional target input. 
      /// </summary>
      /// <param name="targetObject">The object to target.</param>
      /// <param name="targetConnection">A optional argument of the target input on the object.</param>
      /// <returns>Returns true if a Connection object was added.</returns>
      public bool AddConnection(GameObject targetObject, string targetConnection = null)
      {
        if (!AllowMany && m_Connections.Count > 0)
          return false;       // already connected.

        Connection c = new Connection();
        c.Start(this);
        c.TargetObject = targetObject;
        if (targetConnection != null)
          c.TargetConnection = targetConnection;
        if (!c.ResolveTargetInput())
          return false;       // couldn't resolve a input 
        m_Connections.Add(c);

        return true;
      }

      /// <summary>
      /// Remove the connection between widgets.
      /// </summary>
      /// <param name="c"></param>
      /// <returns></returns>
      public bool RemoveConnection(Connection c)
      {
        return m_Connections.Remove(c);
      }
      #endregion

      #region Private Data
      [SerializeField]
      List<Connection> m_Connections = new List<Connection>();
      #endregion
    };
    #endregion

    #region Private Data
    [SerializeField]
    private bool m_AutoConnect = true;
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
    public Input[] Inputs { get { if (!m_Initialized) InitializeIO(); return m_Inputs; } }
    /// <summary>
    /// This returns an array of all outputs on this widget.
    /// </summary>
    public Output[] Outputs { get { if (!m_Initialized) InitializeIO(); return m_Outputs; } }
    #endregion

    #region Public Functions
    /// <summary>
    /// Call this function to go ahead and resolve auto-connections to other widgets. Normally,
    /// we would try to auto connect when the Awake() is called, this can be called to resolve
    /// the auto connections ahead of time.
    /// </summary>
    public void ResolveConnections()
    {
      InitializeIO();
      InitializeConnections();
    }
    #endregion

    #region Private Functions
    private void InitializeIO()
    {
      m_Outputs = GetMembersByType<Output>();
      foreach (var output in m_Outputs)
        output.Start(this);
      m_Inputs = GetMembersByType<Input>();
      foreach (var input in m_Inputs)
        input.Start(this);
      m_Initialized = true;
    }

    private void InitializeConnections()
    {
      // we only auto-connect when running in the editor. Doing this run-time might be very dangerous.
      if (m_AutoConnect)
      {
        // this boolean is serialized, so we only ever do this once. Set this at the start
        // so we don't end up in a circular loop of widgets.
        m_AutoConnect = false;

        Widget[] widgets = FindObjectsOfType<Widget>();
        foreach (var widget in widgets)
        {
          if (widget == null || widget == this)
            continue;       // we never connect to ourselves

          if (widget.Outputs != null)
          {
            foreach (var output in widget.Outputs)
            {
              if (m_Inputs != null)
              {
                foreach (var input in m_Inputs)
                {
                  if (input.DataType == output.DataType)
                  {
                    if (output.AddConnection(input))
                      Log.Status("Widget", "Auto-Connecting {0} -> {1}", output.ToString(), input.ToString());
                  }
                }
              }
            }
          }

        }
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
      InitializeConnections();
    }

    /// <exclude />
    protected virtual void Awake()
    {
      InitializeIO();
    }
    #endregion

    #region Widget Interface
    /// <summary>
    /// Implemented to provide a friendly name for a widget object.
    /// </summary>
    /// <returns>A string containing the name for this widget.</returns>
    protected abstract string GetName();
    #endregion
  }
}
