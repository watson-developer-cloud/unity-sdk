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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

namespace IBM.Watson.Camera
{

    /// <summary>
    /// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.
    /// </summary>
    public class WatsonCamera : MonoBehaviour
    {

        #region Private Variables
        private bool m_isAnimationPaused = false;
        private Vector3 m_TargetCameraLocation;
        private Vector3 m_CameraInitialLocation;
        [SerializeField]
        private float m_PanSpeed = 0.07f;
        [SerializeField]
        private float m_ZoomSpeed = 20.0f;
        [SerializeField]
        private float m_SpeedForCameraAnimation = 2f;

        //TODO: Add boundary limits
        //private Vector3 targetCenter = Vector3.zero;    //Center of Avatar
        //private Vector2 boundaryLimit = Vector2.zero;
        #endregion

        #region OnEnable / OnDisable to register some events

        void OnEnable()
        {
            m_CameraInitialLocation = transform.localPosition;
            m_TargetCameraLocation = m_CameraInitialLocation;
            // TouchEventManager.Instance.RegisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
            // EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
        }

        void OnDisable()
        {
            //TouchEventManager.Instance.UnregisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
            //EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
        }

        #endregion

        #region OnUpdate - All Update animations on camera

        void Update()
        {
            CameraPositionOnUpdate();
        }

        #endregion

        #region Touch Drag Actions
        public void DragTwoFinger(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
            {
                TouchScript.Gestures.ScreenTransformGesture transformGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

                Log.Status("WatsonCamera", "twoFingerTransformHandler: {0} , DeltaScale: {1}, PanSpeed: {2}, ZoomSpeed:{3}",
                transformGesture.DeltaPosition,
                transformGesture.DeltaScale,
                m_PanSpeed,
                m_ZoomSpeed);

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



        void CameraPositionOnUpdate()
        {
            //For Zooming and Panning
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_TargetCameraLocation, Time.deltaTime * m_SpeedForCameraAnimation);
        }

        #endregion

        #region Camera Events Received from Outside - Set default position / Move Left - Right - Up - Down / Zoom-in-out
        public void ResetCameraPosition(System.Object[] args)
        {
            //Log.Status("WatsonCamera", "Reset Camera Position");
            m_TargetCameraLocation = m_CameraInitialLocation;
        }

        public void MoveUp(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.up;
        }

        public void MoveDown(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.down;
        }

        public void MoveLeft(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.left;
        }

        public void MoveRight(System.Object[] args)
        {
            m_TargetCameraLocation += Vector3.right;
        }

        public void ZoomIn(System.Object[] args)
        {
            m_TargetCameraLocation += transform.forward * m_ZoomSpeed;
        }

        public void ZoomOut(System.Object[] args)
        {
            m_TargetCameraLocation += transform.forward * m_ZoomSpeed * -1.0f;
        }

        public void ShowVirtualKeyboard(System.Object[] args)
        {
            try
            {
                System.Diagnostics.Process.Start("TabTip.exe");
            }
            catch (System.Exception e)
            {
                Log.Error("WatsonCamera", "ShowVirtualKeyboard has exception: {0}", e.Message);
            }

        }
        #endregion

        #region Application Related Actions - Methods to call 

        public void AnimationSpeedUp()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_UP);
        }

        public void AnimationSpeedDown()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_DOWN);
        }

        public void AnimationSpeedDefault()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_DEFAULT);
        }

        public void AnimationPause()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_PAUSE);
        }

        public void AnimationResume()
        {
            EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_RESUME);
        }

        public void AnimationPauseResume()
        {
            m_isAnimationPaused = !m_isAnimationPaused;

            if (m_isAnimationPaused)
                AnimationPause();
            else
                AnimationResume();
        }

        public void ApplicationQuit(System.Object[] args = null)
        {
            Application.Quit();
        }

        #endregion
    }

}
