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
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.DeveloperCloud.Widgets;
using System.Collections;

namespace IBM.Watson.DeveloperCloud.Debug
{
  /// <summary>
  /// This class handles the display and updating of the debug console. This class must be attached onto a prefab object, since
  /// it requires data to be setup to function. This class is a singleton instance, but doesn't use the Singleton class since 
  /// it needs hold to data.
  /// </summary>
  public class DebugConsole : MonoBehaviour
  {
    #region Construction
    /// <summary>
    /// Public constructor.
    /// </summary>
    public DebugConsole()
    {
      Instance = this;
    }
    #endregion

    #region Public Types
    /// <summary>
    /// This callback is invoked by the DebugConsole to get the string to display for a DebugInfo component in the console.
    /// </summary>
    /// <returns></returns>
    public delegate string GetDebugInfo();
    #endregion

    #region Private Data
    private class DebugInfo
    {
      public string m_Label;
      public GetDebugInfo m_Callback;
      public GameObject m_InfoObject;
      public Text m_TextOutput;
    }
    private List<DebugInfo> m_DebugInfos = new List<DebugInfo>();

    [SerializeField]
    private bool m_ActiveOutput = false;
    [SerializeField]
    private bool m_ActiveInput = false;
    [SerializeField, Tooltip("The root of the debug console, this is what is hidden or displayed to show the console when active.")]
    private GameObject m_RootOutput = null;
    [SerializeField]
    private GameObject m_RootInput = null;
    [SerializeField]
    private LayoutGroup m_MessageLayout = null;
    [SerializeField]
    private Text m_MessagePrefab = null;
    [SerializeField]
    private InputField m_CommandInput = null;
    [SerializeField]
    private LayoutGroup m_DebugInfoLayout = null;
    [SerializeField]
    private Text m_DebugInfoPrefab = null;

    private MicrophoneWidget m_MicWidget = null;
    #endregion

    #region Public Properties
    /// <summary>
    /// The current instance of the DebugConsole.
    /// </summary>
    public static DebugConsole Instance { get; private set; }
    /// <summary>
    /// Returns true if this debug console output is being displayed.
    /// </summary>
    public bool ActiveOutput
    {
      get { return m_ActiveOutput; }
      set
      {
        if (m_ActiveOutput != value)
        {
          m_ActiveOutput = value;
          m_RootOutput.SetActive(m_ActiveOutput);
        }
      }
    }

    /// <summary>
    /// Returns true if this debug input field is being displayed.
    /// </summary>
    public bool ActiveInput
    {
      get { return m_ActiveInput; }
      set
      {
        if (m_ActiveInput != value)
        {
          m_ActiveInput = value;
          m_RootInput.SetActive(m_ActiveInput);
        }
      }
    }
    #endregion

    /// <summary>
    /// Register a debug info 
    /// </summary>
    /// <param name="label">The label to display next to the string returned by the callback.</param>
    /// <param name="callback">A callback function to invoke that should return a string to display in the debug console.</param>
    public void RegisterDebugInfo(string label, GetDebugInfo callback)
    {
      if (string.IsNullOrEmpty(label))
        throw new ArgumentNullException("label");
      if (callback == null)
        throw new ArgumentNullException("callback");

      DebugInfo info = new DebugInfo();
      info.m_Label = label;
      info.m_Callback = callback;
      info.m_InfoObject = Instantiate(m_DebugInfoPrefab.gameObject) as GameObject;
      info.m_TextOutput = info.m_InfoObject.GetComponentInChildren<Text>();
      m_DebugInfos.Add(info);

      info.m_InfoObject.transform.SetParent(m_DebugInfoLayout.transform, false);
    }

    /// <summary>
    /// Unregister a debug info hook.
    /// </summary>
    /// <param name="label">The label to unregister.</param>
    /// <param name="callback">The callback to unregister. If null, then the callback will be matched by label only.</param>
    /// <returns>Returns true if the callback was unregistered.</returns>
    public bool UnregisterDebugInfo(string label, GetDebugInfo callback = null)
    {
      if (string.IsNullOrEmpty(label))
        throw new ArgumentNullException("label");
      for (int i = 0; i < m_DebugInfos.Count; ++i)
      {
        if (m_DebugInfos[i].m_Label == label)
        {
          if (callback != null && callback != m_DebugInfos[i].m_Callback)
            continue;

          Destroy(m_DebugInfos[i].m_InfoObject);
          m_DebugInfos.RemoveAt(i);
          return true;
        }
      }

      return false;
    }

    private void Start()
    {
      if (m_RootInput == null
          || m_RootOutput == null
          || m_MessageLayout == null
          || m_MessagePrefab == null
          || m_CommandInput == null
          || m_DebugInfoLayout == null
          || m_DebugInfoPrefab == null)
      {
        Log.Error("DebugConsole", "DebugConsole is missing references to it's parts, disabling debug console.");
        gameObject.SetActive(false);
      }
      else
      {
        m_RootInput.SetActive(m_ActiveInput);
        m_RootOutput.SetActive(m_ActiveOutput);
        m_MicWidget = FindObjectOfType<MicrophoneWidget>();
      }
    }

    private void OnEnable()
    {
      EventManager.Instance.RegisterEventReceiver("OnDebugMessage", OnDebugMessage);

      EventManager.Instance.RegisterEventReceiver("OnDebugToggle", OnToggleActive);
      EventManager.Instance.RegisterEventReceiver("OnKeyboardBackquote", OnToggleActive);

      EventManager.Instance.RegisterEventReceiver("OnDebugBeginCommand", OnBeginEdit);
      EventManager.Instance.RegisterEventReceiver("OnKeyboardReturn", OnBeginEdit);
    }

    private void OnDisable()
    {
      EventManager.Instance.UnregisterEventReceiver("OnDebugMessage", OnDebugMessage);

      EventManager.Instance.UnregisterEventReceiver("OnDebugToggle", OnToggleActive);
      EventManager.Instance.UnregisterEventReceiver("OnKeyboardBackquote", OnToggleActive);

      EventManager.Instance.UnregisterEventReceiver("OnDebugBeginCommand", OnBeginEdit);
      EventManager.Instance.UnregisterEventReceiver("OnKeyboardReturn", OnBeginEdit);
    }

    private void Update()
    {
      if (ActiveOutput)
      {
        for (int i = 0; i < m_DebugInfos.Count; ++i)
        {
          DebugInfo info = m_DebugInfos[i];
          if (info.m_Callback == null || info.m_TextOutput == null)
          {
            if (info.m_InfoObject != null)
              Destroy(info.m_InfoObject);
            m_DebugInfos.RemoveAt(i--);
            continue;
          }
          info.m_TextOutput.text = string.Format("{0}: {1}", info.m_Label, info.m_Callback());
        }
      }
    }

    #region Event Handlers

    private void OnDebugMessage(object[] args)
    {
      if (args != null && args.Length > 0)
      {
        if (args[0] is string)
        {
          GameObject messageObject = Instantiate(m_MessagePrefab.gameObject) as GameObject;
          messageObject.GetComponent<Text>().text = Utility.RemoveTags((string)args[0]);
          messageObject.transform.SetParent(m_MessageLayout.transform, false);
        }
      }
    }

    private void OnToggleActive(object[] args)
    {
      ActiveOutput = !ActiveOutput;
      EventManager.Instance.SendEvent("OnDebugToggleFinish");
    }

    private void OnBeginEdit(object[] args)
    {
      if (m_CommandInput != null)
      {
        ActiveInput = true;
        m_CommandInput.gameObject.SetActive(true);
        m_CommandInput.ActivateInputField();

        // turn off all key press events..
        KeyEventManager.Instance.Active = false;

        //	turn off mic
        if (m_MicWidget != null)
          m_MicWidget.Active = false;

        //	timer to turn mic back on
        StartCoroutine(ActivateMicAfterTime(10f));
      }
    }

    /// <summary>
    /// Event handler for input end.
    /// </summary>
    public void OnEndEdit()
    {
      if (m_CommandInput != null)
      {
        EventManager.Instance.SendEvent("OnDebugCommand", m_CommandInput.text);
        m_CommandInput.text = string.Empty;
        m_CommandInput.gameObject.SetActive(false);   // hide the input     
        ActiveInput = false;

        // restore the key manager state
        KeyEventManager.Instance.Active = true;

        //	turn on mic
        if (m_MicWidget != null)
          m_MicWidget.Active = true;
      }
    }

    private IEnumerator ActivateMicAfterTime(float time)
    {
      yield return new WaitForSeconds(time);
      if (m_MicWidget != null)
        m_MicWidget.Active = true;
    }
    #endregion
  }
}
