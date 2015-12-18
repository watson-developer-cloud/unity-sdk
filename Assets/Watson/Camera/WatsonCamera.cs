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
        private Vector3 m_TargetCameraLocation;
		private Quaternion m_TargetCameraRotation;

        private Vector3 m_CameraInitialLocation;
		private Quaternion m_CameraInitialRotation;
        [SerializeField]
        private float m_PanSpeed = 0.07f;
        [SerializeField]
        private float m_ZoomSpeed = 20.0f;
        [SerializeField]
        private float m_SpeedForCameraAnimation = 2f;

        #endregion

        #region Start / Update

		void Start(){
			m_CameraInitialLocation = transform.localPosition;
			m_CameraInitialRotation = transform.rotation;
			
			m_TargetCameraLocation = m_CameraInitialLocation;
			m_TargetCameraRotation = m_CameraInitialRotation;
		}

		void Update()
		{
			CameraPositionOnUpdate ();
		}

		void CameraPositionOnUpdate()
		{
			//For Zooming and Panning
			transform.localPosition = Vector3.Lerp(transform.localPosition, m_TargetCameraLocation, Time.deltaTime * m_SpeedForCameraAnimation);
			transform.rotation = Quaternion.Lerp(transform.localRotation, m_TargetCameraRotation, Time.deltaTime * m_SpeedForCameraAnimation );
		}

		#endregion

        #region Touch Drag Actions

		/// <summary>
		/// Event handler to pan and zoom with two-finger dragging
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void DragTwoFinger(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
            {
                TouchScript.Gestures.ScreenTransformGesture transformGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

                //Pannning with 2-finger
                m_TargetCameraLocation += (transformGesture.DeltaPosition * m_PanSpeed * -1.0f);
                //Zooming with 2-finger
                m_TargetCameraLocation += transform.forward * (transformGesture.DeltaScale - 1.0f) * m_ZoomSpeed;
            }
            else
            {
                Log.Warning("WatsonCamera", "TwoFinger drag has invalid argument");
            }
        }

        #endregion

        #region Camera Events Received from Outside - Set default position / Move Left - Right - Up - Down / Zoom-in-out

		/// <summary>
		/// Event handler reseting the camera position.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ResetCameraPosition(System.Object[] args)
        {
            //Log.Status("WatsonCamera", "Reset Camera Position");
            m_TargetCameraLocation = m_CameraInitialLocation;
			m_TargetCameraRotation = m_CameraInitialRotation;
        }

		/// <summary>
		/// Event handler moving the camera up.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveUp(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.up;
        }

		/// <summary>
		/// Event handler moving the camera down.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveDown(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.down;
        }

		/// <summary>
		/// Event handler moving the camera left.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveLeft(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.left;
        }

		/// <summary>
		/// Event handler moving the camera right.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void MoveRight(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.right;
        }

		/// <summary>
		/// Event handler zooming-in the camera.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ZoomIn(System.Object[] args)
        {
            m_TargetCameraLocation += transform.forward * m_ZoomSpeed;
        }

		/// <summary>
		/// Event handler zooming-out the camera.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void ZoomOut(System.Object[] args)
        {
            m_TargetCameraLocation += transform.forward * m_ZoomSpeed * -1.0f;
        }

        #endregion

    }

}
