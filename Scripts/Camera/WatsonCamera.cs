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
using UnityStandardAssets.ImageEffects;

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
        //private Vector3 m_TargetCameraLocation;
		//private Quaternion m_TargetCameraRotation;

        private Vector3 m_CameraInitialLocation;
		private Quaternion m_CameraInitialRotation;
        [SerializeField]
        private float m_PanSpeed = 0.07f;
        [SerializeField]
        private float m_ZoomSpeed = 20.0f;
        [SerializeField]
        private float m_SpeedForCameraAnimation = 2f;

        private Antialiasing m_AntiAliasing;
        private DepthOfField m_DepthOfField;
        private bool m_DisableTwoFinger = false;

        #endregion

        #region Public Variable

        public static WatsonCamera Instance
        {
            get
            {
                return mp_Instance;
            }
        }

        public CameraTarget CurrentCameraTarget
        {
            get{
                if (m_TargetCamera == null)
                {
                    InitializeCameraTargetList();
                }

                return m_TargetCamera;
            }
            set{
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

        public CameraTarget DefaultCameraTarget{
            get
            {
                if (m_ListCameraTarget == null || m_ListCameraTarget.Count == 0)
                    InitializeCameraTargetList();
                
                return m_ListCameraTarget[0];
            }
        }

        #endregion

        #region Event Registration

        void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CAMERA_SET_ANTIALIASING, OnCameraSetAntiAliasing);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CAMERA_SET_DEPTHOFFIELD, OnCameraSetDepthOfField);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CAMERA_SET_TWOFINGERDRAG, OnCameraSetTwoFingerDrag);
        }

        void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CAMERA_SET_ANTIALIASING, OnCameraSetAntiAliasing);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CAMERA_SET_DEPTHOFFIELD, OnCameraSetDepthOfField);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CAMERA_SET_TWOFINGERDRAG, OnCameraSetTwoFingerDrag);
        }

        #endregion

        #region Start / Update

        void Awake(){
            mp_Instance = this;
            m_AntiAliasing = this.GetComponent<Antialiasing>();
            m_DepthOfField = this.GetComponent<DepthOfField>();
        }

		void Start(){
			m_CameraInitialLocation = transform.localPosition;
			m_CameraInitialRotation = transform.rotation;
		}

		void Update()
		{
			CameraPositionOnUpdate ();
		}

		void CameraPositionOnUpdate()
		{
			//For Zooming and Panning
            if (CurrentCameraTarget != null)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, CurrentCameraTarget.TargetPosition, Time.deltaTime * m_SpeedForCameraAnimation);
                transform.rotation = Quaternion.Lerp(transform.localRotation, CurrentCameraTarget.TargetRotation, Time.deltaTime * m_SpeedForCameraAnimation);
            }
		}

        void InitializeCameraTargetList()
        {
            if (m_ListCameraTarget == null)
                m_ListCameraTarget = new List<CameraTarget>();

            m_ListCameraTarget.Clear();

            CameraTarget defaultCameraTarget = this.gameObject.AddComponent<CameraTarget>();
            defaultCameraTarget.TargetPosition = m_CameraInitialLocation;
            defaultCameraTarget.TargetRotation = m_CameraInitialRotation;
            m_ListCameraTarget.Add(defaultCameraTarget);

            m_TargetCamera = m_ListCameraTarget[0];

        }

		#endregion

        #region Touch Drag Actions

		/// <summary>
		/// Event handler to pan and zoom with two-finger dragging
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void DragTwoFinger(System.Object[] args)
        {
            if (m_DisableTwoFinger)
                return;
            
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
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

        public void OnCameraSetAntiAliasing(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is bool)
            {
                bool valueSet = (bool)args[0];

                if (m_AntiAliasing != null)
                {
                    m_AntiAliasing.enabled = valueSet;
                }
            }
        }

        public void OnCameraSetDepthOfField(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is bool)
            {
                bool valueSet = (bool)args[0];

                if (m_DepthOfField != null)
                {
                    m_DepthOfField.enabled = valueSet;
                }
            }
        }

        public void OnCameraSetTwoFingerDrag(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is bool)
            {
                m_DisableTwoFinger = !(bool)args[0];
            }
        }


		/// <summary>
		/// Event handler reseting the camera position.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ResetCameraPosition(System.Object[] args)
        {
            //Log.Status("WatsonCamera", "Reset Camera Position");
            DefaultCameraTarget.TargetPosition = m_CameraInitialLocation;
            DefaultCameraTarget.TargetRotation = m_CameraInitialRotation;
        }

		/// <summary>
		/// Event handler moving the camera up.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveUp(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += this.transform.up;
        }

		/// <summary>
		/// Event handler moving the camera down.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveDown(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += this.transform.up * -1.0f;
        }

		/// <summary>
		/// Event handler moving the camera left.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveLeft(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += this.transform.right * -1.0f;;
        }

		/// <summary>
		/// Event handler moving the camera right.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveRight(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += this.transform.right;
        }

		/// <summary>
		/// Event handler zooming-in the camera.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ZoomIn(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += transform.forward * m_ZoomSpeed;
        }

		/// <summary>
		/// Event handler zooming-out the camera.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ZoomOut(System.Object[] args)
        {
            DefaultCameraTarget.TargetPosition += transform.forward * m_ZoomSpeed * -1.0f;
        }

        #endregion

    }

}
