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

using UnityEngine;
using System.Collections;
using System;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine.UI;
#pragma warning disable 0414
namespace IBM.Watson.DeveloperCloud.Widgets
{
  /// <summary>
  /// This widget gets video from the WebCam. Note: there are issues with WebCamTexture. Do not
  /// start the WebCamTexture at too low resolution!
  /// </summary>
  public class WebCamWidget : Widget
  {
    #region Inputs
    [SerializeField]
    private Input m_DisableInput = new Input("Disable", typeof(DisableWebCamData), "OnDisableInput");
    #endregion

    #region Outputs
    [SerializeField]
    private Output m_WebCamTextureOutput = new Output(typeof(WebCamTextureData));
    [SerializeField]
    private Output m_ActivateOutput = new Output(typeof(BooleanData));
    #endregion

    #region Private Data
    private bool m_Active = false;
    private bool m_Disabled = false;
    private bool m_Failure = false;
    private DateTime m_LastFailure = DateTime.Now;

    [SerializeField]
    private bool m_ActivateOnStart = true;
    [SerializeField]
    private int m_RequestedWidth = 640;
    [SerializeField]
    private int m_RequestedHeight = 480;
    [SerializeField]
    private int m_RequestedFPS = 60;
    [SerializeField]
    private Text m_StatusText = null;

    private int m_RecordingRoutine = 0;                      // ID of our co-routine when recording, 0 if not recording currently.
    private WebCamTexture m_WebCamTexture;
    private int m_WebCamIndex = 0;
    #endregion

    #region Public Properties
    /// <summary>
    /// True if WebCam is active, false if inactive.
    /// </summary>
    public bool Active
    {
      get { return m_Active; }
      set
      {
        if (m_Active != value)
        {
          m_Active = value;
          if (m_Active && !m_Disabled)
            StartRecording();
          else
            StopRecording();
        }
      }
    }
    /// <summary>
    /// True if WebCamera is disabled, false if enabled.
    /// </summary>
    public bool Disable
    {
      get { return m_Disabled; }
      set
      {
        if (m_Disabled != value)
        {
          m_Disabled = value;
          if (m_Active && !m_Disabled)
            StartRecording();
          else
            StopRecording();
        }
      }
    }
    /// <summary>
    /// This is set to true when the WebCam fails, the update will continue to try to start
    /// the microphone so long as it's active.
    /// </summary>
    public bool Failure
    {
      get { return m_Failure; }
      private set
      {
        if (m_Failure != value)
        {
          m_Failure = value;
          if (m_Failure)
            m_LastFailure = DateTime.Now;
        }
      }
    }
    /// <summary>
    /// Returns all available WebCameras.
    /// </summary>
    public WebCamDevice[] Devices
    {
      get { return WebCamTexture.devices; }
    }
    /// <summary>
    /// The curent WebCamTexture
    /// </summary>
    public WebCamTexture WebCamTexture
    {
      get { return m_WebCamTexture; }
    }
    /// <summary>
    /// The requested width of the WebCamTexture.
    /// </summary>
    public int RequestedWidth
    {
      get { return m_RequestedWidth; }
      set { m_RequestedWidth = value; }
    }
    /// <summary>
    /// The requested height of the WebCamTexture.
    /// </summary>
    public int RequestedHeight
    {
      get { return m_RequestedHeight; }
      set { m_RequestedHeight = value; }
    }

    /// <summary>
    /// The requested frame rate of the WebCamTexture.
    /// </summary>
    public int RequestedFPS
    {
      get { return m_RequestedFPS; }
      set { m_RequestedFPS = value; }
    }
    #endregion

    #region Public Funtions
    /// <summary>
    /// Activates the WebCam.
    /// </summary>
    public void ActivateWebCam()
    {
      Active = true;
    }

    /// <summary>
    /// Deactivates the WebCam.
    /// </summary>
    public void DeactivateWebCam()
    {
      Active = false;
    }

    /// <summary>
    /// Switches WebCamDevice to the next device.
    /// </summary>
    public void SwitchWebCam()
    {
      WebCamDevice[] devices = Devices;

      if (devices.Length == 0)
        throw new WatsonException(string.Format("There are no WebCam devices!"));
      if (devices.Length == 1)
      {
        Log.Warning("WebCamWidget", "There is only one WebCam device!");
        return;
      }

      m_WebCamTexture.Stop();
      int requestedIndex;
      if (m_WebCamIndex == devices.Length - 1)
        requestedIndex = 0;
      else
        requestedIndex = m_WebCamIndex + 1;
      m_WebCamTexture.deviceName = devices[requestedIndex].name;
      m_WebCamIndex = requestedIndex;
      Log.Status("WebCamWidget", "Switched to WebCam {0}, name: {1}, isFontFacing: {2}.", m_WebCamIndex, devices[m_WebCamIndex].name, devices[m_WebCamIndex].isFrontFacing);
      m_WebCamTexture.Play();
    }

    /// <summary>
    /// Switches the WebCam device based on index.
    /// </summary>
    /// <param name="index">The WebCam index.</param>
    public void SwitchWebCam(int index)
    {
      WebCamDevice[] devices = Devices;

      if (index < devices.Length)
      {
        throw new WatsonException(string.Format("Requested WebCam index {0} does not exist! There are {1} available WebCams.", index, devices.Length));
      }

      m_WebCamTexture.Stop();
      m_WebCamTexture.deviceName = devices[index].name;
      m_WebCamIndex = index;
      Log.Status("WebCamWidget", "Switched to WebCam {0}, name: {1}, isFontFacing: {2}.", m_WebCamIndex, devices[m_WebCamIndex].name, devices[m_WebCamIndex].isFrontFacing);
      m_WebCamTexture.Play();
    }

    /// <summary>
    /// Toggles between Active and Inactive states.
    /// </summary>
    public void ToggleActive()
    {
      Active = !Active;
    }
    #endregion

    #region EventHandlers
    protected override void Start()
    {
      base.Start();
      LogSystem.InstallDefaultReactors();
      Log.Debug("WebCamWidget", "WebCamWidget.Start();");

      if (m_ActivateOnStart)
        Active = true;
    }

    private void OnDisableInput(Data data)
    {
      Disable = ((DisableWebCamData)data).Boolean;
    }
    #endregion

    #region Widget Interface
    protected override string GetName()
    {
      return "WebCam";
    }
    #endregion

    #region Recording Functions
    private void StartRecording()
    {
      Log.Debug("WebCamWidget", "StartRecording(); m_RecordingRoutine: {0}", m_RecordingRoutine);
      if (m_RecordingRoutine == 0)
      {
        //UnityObjectUtil.StartDestroyQueue();

        m_RecordingRoutine = Runnable.Run(RecordingHandler());
        m_ActivateOutput.SendData(new BooleanData(true));

        if (m_StatusText != null)
          m_StatusText.text = "WEB CAMERA ON";
      }
    }

    private void StopRecording()
    {
      Log.Debug("WebCamWidget", "StopRecording();");
      if (m_RecordingRoutine != 0)
      {
        Runnable.Stop(m_RecordingRoutine);
        m_RecordingRoutine = 0;

        m_ActivateOutput.SendData(new BooleanData(false));

        m_WebCamTexture.Stop();

        if (m_StatusText != null)
          m_StatusText.text = "WEB CAMERA OFF";
      }
    }

    private IEnumerator RecordingHandler()
    {
      Failure = false;

#if !UNITY_EDITOR
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif

      m_WebCamTexture = new WebCamTexture(m_RequestedWidth, m_RequestedHeight, m_RequestedFPS);
      yield return null;

      if (m_WebCamTexture == null)
      {
        Failure = true;
        StopRecording();
        yield break;
      }

      WebCamTextureData camData = new WebCamTextureData(m_WebCamTexture, m_RequestedWidth, m_RequestedHeight, m_RequestedFPS);
      m_WebCamTextureOutput.SendData(camData);
    }
    #endregion
  }
}
