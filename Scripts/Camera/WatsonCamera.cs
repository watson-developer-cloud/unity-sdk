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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

namespace IBM.Watson.DeveloperCloud.Camera
{

  /// <summary>
  /// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.
  /// </summary>
  public class WatsonCamera : MonoBehaviour
  {

    #region Private Variables
    private static WatsonCamera mp_Instance;
    private List<CameraTarget> m_ListCameraTarget = new List<CameraTarget>();
    private CameraTarget m_TargetCamera = null;

    protected Vector3 m_CameraInitialLocation;
    protected Quaternion m_CameraInitialRotation;
    [SerializeField]
    protected float m_PanSpeed = 0.07f;
    [SerializeField]
    protected float m_ZoomSpeed = 20.0f;
    [SerializeField]
    protected float m_SpeedForCameraAnimation = 2f;

    private float m_CommandMovementModifier = 10.0f;

    [SerializeField]
    private MonoBehaviour m_AntiAliasing;
    [SerializeField]
    private MonoBehaviour m_DepthOfField;
    protected bool m_DisableInteractivity = false;

    #endregion

    #region Public Variable

    /// <summary>
    /// Static instance of WatsonCamera.
    /// </summary>
    public static WatsonCamera Instance
    {
      get
      {
        return mp_Instance;
      }
    }

    /// <summary>
    /// The camera's current target.
    /// </summary>
    public CameraTarget CurrentCameraTarget
    {
      get
      {
        if (m_TargetCamera == null)
        {
          InitializeCameraTargetList();
        }

        return m_TargetCamera;
      }
      set
      {
        if (value != null)
        {
          m_TargetCamera = value;

          if (!m_ListCameraTarget.Contains(value))
          {
            m_ListCameraTarget.Add(value);
          }
        }
        else
        {   //Delete current camera and clear from the list

          if (m_ListCameraTarget.Contains(m_TargetCamera))
          {
            m_ListCameraTarget.Remove(m_TargetCamera);
          }

          if (m_ListCameraTarget.Count > 0)
          {
            m_TargetCamera = m_ListCameraTarget[m_ListCameraTarget.Count - 1];
          }
          else
          {
            InitializeCameraTargetList();
          }
        }
      }
    }

    /// <summary>
    /// The camera's default target.
    /// </summary>
    public CameraTarget DefaultCameraTarget
    {
      get
      {
        if (m_ListCameraTarget == null || m_ListCameraTarget.Count == 0)
          InitializeCameraTargetList();

        return m_ListCameraTarget[0];
      }
    }

    #endregion

    #region Event Registration

    protected virtual void OnEnable()
    {
      EventManager.Instance.RegisterEventReceiver("OnCameraReset", ResetCameraPosition);
      EventManager.Instance.RegisterEventReceiver("OnCameraSetAntiAliasing", OnCameraSetAntiAliasing);
      EventManager.Instance.RegisterEventReceiver("OnCameraSetDepthOfField", OnCameraSetDepthOfField);
      EventManager.Instance.RegisterEventReceiver("OnCameraSetInteractivity", OnCameraSetTwoFingerDrag);
    }

    protected virtual void OnDisable()
    {
      EventManager.Instance.UnregisterEventReceiver("OnCameraReset", ResetCameraPosition);
      EventManager.Instance.UnregisterEventReceiver("OnCameraSetAntiAliasing", OnCameraSetAntiAliasing);
      EventManager.Instance.UnregisterEventReceiver("OnCameraSetDepthOfField", OnCameraSetDepthOfField);
      EventManager.Instance.UnregisterEventReceiver("OnCameraSetInteractivity", OnCameraSetTwoFingerDrag);
    }

    #endregion

    #region Start / Update

    protected virtual void Awake()
    {
      mp_Instance = this;
    }

    protected virtual void Start()
    {
      m_CameraInitialLocation = transform.localPosition;
      m_CameraInitialRotation = transform.rotation;
    }

    protected virtual void Update()
    {
      CameraPositionOnUpdate();
    }

    protected virtual void CameraPositionOnUpdate()
    {
      //For Zooming and Panning
      if (CurrentCameraTarget != null)
      {
        transform.localPosition = Vector3.Lerp(transform.localPosition, CurrentCameraTarget.TargetPosition, Time.deltaTime * m_SpeedForCameraAnimation);
        transform.rotation = Quaternion.Slerp(transform.localRotation, CurrentCameraTarget.TargetRotation, Time.deltaTime * m_SpeedForCameraAnimation);
      }
    }

    protected virtual void InitializeCameraTargetList()
    {
      if (m_ListCameraTarget == null)
        m_ListCameraTarget = new List<CameraTarget>();

      for (int i = 0; m_ListCameraTarget != null && i < m_ListCameraTarget.Count; i++)
      {
        Destroy(m_ListCameraTarget[i]);
      }

      m_ListCameraTarget.Clear();

      CameraTarget defaultCameraTarget = this.gameObject.GetComponent<CameraTarget>();
      if (defaultCameraTarget == null)
        defaultCameraTarget = this.gameObject.AddComponent<CameraTarget>();

      defaultCameraTarget.TargetPosition = m_CameraInitialLocation;
      if (defaultCameraTarget.TargetObject == null)
      {
        defaultCameraTarget.TargetRotation = m_CameraInitialRotation;
      }
      else
      {
        defaultCameraTarget.TargetObject = defaultCameraTarget.TargetObject;
      }

      m_ListCameraTarget.Add(defaultCameraTarget);

      m_TargetCamera = m_ListCameraTarget[0];
    }

    #endregion

    #region Touch Drag Actions

    /// <summary>
    /// Event handler to pan and zoom with two-finger dragging
    /// </summary>
    /// <param name="args">Arguments.</param>
    public virtual void DragTwoFinger(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      if (args != null && args.Length > 0 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
      {
        TouchScript.Gestures.ScreenTransformGesture transformGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

        //Pannning with 2-finger
        DefaultCameraTarget.TargetPosition += (transformGesture.DeltaPosition * m_PanSpeed * -1.0f);
        //Zooming with 2-finger
        DefaultCameraTarget.TargetPosition += transform.forward * (transformGesture.DeltaScale - 1.0f) * m_ZoomSpeed;
      }
      else
      {
        Log.Warning("WatsonCamera", "TwoFinger drag has invalid argument");
      }
    }

    #endregion

    #region Camera Events Received from Outside - Set default position / Move Left - Right - Up - Down / Zoom-in-out
    /// <summary>
    /// Event Handler for setting Antialiasing event
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void OnCameraSetAntiAliasing(System.Object[] args)
    {
      if (args != null && args.Length > 0 && args[0] is bool)
      {
        bool valueSet = (bool)args[0];

        if (m_AntiAliasing != null)
        {
          m_AntiAliasing.enabled = valueSet;
        }
      }
    }

    /// <summary>
    /// Event Handler for setting Depth of Field event
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void OnCameraSetDepthOfField(System.Object[] args)
    {
      if (args != null && args.Length > 0 && args[0] is bool)
      {
        bool valueSet = (bool)args[0];

        if (m_DepthOfField != null)
        {
          m_DepthOfField.enabled = valueSet;
        }
      }
    }

    /// <summary>
    /// Event Handler for Two Finger Drag
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void OnCameraSetTwoFingerDrag(System.Object[] args)
    {
      if (args != null && args.Length > 0 && args[0] is bool)
      {
        m_DisableInteractivity = !(bool)args[0];
      }
    }


    /// <summary>
    /// Event handler reseting the camera. Deleting all camera target and set the initial as default. 
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void ResetCamera(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      InitializeCameraTargetList();
    }

    /// <summary>
    /// Event handler reseting the camera position.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void ResetCameraPosition(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;
      //Log.Status("WatsonCamera", "Reset Camera Position");
      CurrentCameraTarget.TargetPosition = m_CameraInitialLocation;
      CurrentCameraTarget.TargetRotation = m_CameraInitialRotation;
    }


    /// <summary>
    /// Event handler moving the camera up.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void MoveUp(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += this.transform.up * m_CommandMovementModifier;
    }

    /// <summary>
    /// Event handler moving the camera down.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void MoveDown(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += this.transform.up * -m_CommandMovementModifier;
    }

    /// <summary>
    /// Event handler moving the camera left.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void MoveLeft(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += this.transform.right * -m_CommandMovementModifier;
    }

    /// <summary>
    /// Event handler moving the camera right.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void MoveRight(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += this.transform.right * m_CommandMovementModifier;
    }

    /// <summary>
    /// Event handler zooming-in the camera.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void ZoomIn(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += transform.forward * m_ZoomSpeed;
    }

    /// <summary>
    /// Event handler zooming-out the camera.
    /// </summary>
    /// <param name="args">Arguments.</param>
    protected virtual void ZoomOut(System.Object[] args)
    {
      if (m_DisableInteractivity)
        return;

      DefaultCameraTarget.TargetPosition += transform.forward * m_ZoomSpeed * -1.0f;
    }

    #endregion
  }
}
