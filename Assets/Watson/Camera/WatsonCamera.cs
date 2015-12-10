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
using IBM.Watson.Widgets.Question;

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
		private Quaternion m_TargetCameraRotation;

        private Vector3 m_CameraInitialLocation;
		private Quaternion m_CameraInitialRotation;
        [SerializeField]
        private float m_PanSpeed = 0.07f;
        [SerializeField]
        private float m_ZoomSpeed = 20.0f;
        [SerializeField]
        private float m_SpeedForCameraAnimation = 2f;

		private Vector3 m_TargetCameraLocationBeforeIdle;
		private Quaternion m_TargetCameraRotationBeforeIdle;
		private int m_AnimationCameraIdle = -1;
		private float m_LastTimeActionCaptured = 0.0f;
		private float m_TimeForWaitingToBecomeIdle = 6000.0f;	//10min as idle just test purpose for now! TODO: change it!
		private bool m_IsIdle = false;

        //TODO: Add boundary limits
        //private Vector3 targetCenter = Vector3.zero;    //Center of Avatar
        //private Vector2 boundaryLimit = Vector2.zero;
        #endregion

        #region OnEnable / OnDisable to register some events

        void OnEnable()
        {
            m_CameraInitialLocation = transform.localPosition;
			m_CameraInitialRotation = transform.rotation;

            m_TargetCameraLocation = m_CameraInitialLocation;
			m_TargetCameraRotation = m_CameraInitialRotation;

			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_KEYBOARD_ANYKEY_DOWN, ChangeOnScene);			//any keyboard press make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_TOUCH_PRESSED_FULLSCREEN, ChangeOnScene);		//any touch press make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_TOUCH_RELEASED_FULLSCREEN, ChangeOnScene);		//any touch release make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, ChangeOnScene);		//and avatar state change (voice recognition / or failure make it awake)
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, QuestionStateChanged);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, MakeIdle);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_VIRTUAL_KEYBOARD_TOGGLE, ShowVirtualKeyboard);
            // TouchEventManager.Instance.RegisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
            // EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
        }

        void OnDisable()
        {
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_KEYBOARD_ANYKEY_DOWN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_TOUCH_PRESSED_FULLSCREEN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_TOUCH_RELEASED_FULLSCREEN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, QuestionStateChanged);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, MakeIdle);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_VIRTUAL_KEYBOARD_TOGGLE, ShowVirtualKeyboard);
            //TouchEventManager.Instance.UnregisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
            //EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
        }

        #endregion

        #region OnUpdate - All Update animations on camera

		void Start(){
			ChangeOnScene ();
		}

        void Update()
        {
			CheckIdleState();
            CameraPositionOnUpdate();
        }


		void QuestionStateChanged(System.Object[] args = null){
			if (args != null && args.Length == 1 && args [0] is int) {
				//( args[0] as Ques
				CubeAnimationManager.CubeAnimationState cubeAnimationState = (CubeAnimationManager.CubeAnimationState)args [0];
				if (cubeAnimationState == CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE || cubeAnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED) {
					m_LastTimeActionCaptured = float.MaxValue;	//there won't be any idle animation if cube has focused
				}
				else{
					m_LastTimeActionCaptured = Time.realtimeSinceStartup;
				}
			} else {
				m_LastTimeActionCaptured = Time.realtimeSinceStartup;
			}

		}

		void ChangeOnScene(System.Object[] args = null){
			m_LastTimeActionCaptured = Time.realtimeSinceStartup;
			if(m_IsIdle)
				EventManager.Instance.SendEvent(Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, false);
		}

		void CheckIdleState(){
			if (m_LastTimeActionCaptured > 0 && m_LastTimeActionCaptured + m_TimeForWaitingToBecomeIdle < Time.realtimeSinceStartup) {
				//if it is Idle then changing state now
				if(!m_IsIdle)
					EventManager.Instance.SendEvent(Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, true);
			} 
		}

		private float minDistance = 60.0f;
		private float maxDistance = 160.0f;
		private float timeForOneFLoop = 60.0f;
		private float timeForEachTick = 3.0f;
		private float height = 10.0f;

		void MakeIdle(System.Object[] args){
			if (args != null && args.Length == 1 && args[0] is bool)
			{
				m_IsIdle = (bool) args[0];
				EventManager.Instance.SendEvent(Constants.Event.ON_APPLICATION_BECAME_IDLE, m_IsIdle);

				if(m_IsIdle){
					//start animation looop
					m_TargetCameraLocationBeforeIdle = m_TargetCameraLocation;
					m_TargetCameraRotationBeforeIdle = m_TargetCameraRotation;
					int numberOfLoop = 0;

					Vector3 targetLocation = GetTargetPositionByLoopCount(0);
					LTBezierPath path = new LTBezierPath(new Vector3[]{
						transform.localPosition,
						new Vector3(transform.localPosition.x , transform.localPosition.y, transform.localPosition.z),
						new Vector3(targetLocation.x , targetLocation.y, targetLocation.z),
						targetLocation
					});
				
					m_AnimationCameraIdle = LeanTween.value(this.gameObject, 0.0f, 1.0f, timeForEachTick).setLoopClamp().setOnUpdate((float f) =>{

						//Follow the path
						m_TargetCameraLocation = Vector3.Lerp(m_TargetCameraLocation, path.point(f) , Time.deltaTime * m_SpeedForCameraAnimation);
						Vector3 relativePos = Vector3.zero - transform.position;
						m_TargetCameraRotation = Quaternion.LookRotation(relativePos);

					}).setOnComplete( ()=>{
						numberOfLoop = (numberOfLoop + 1) % (int) timeForOneFLoop;

						targetLocation = GetTargetPositionByLoopCount(numberOfLoop);
						path = new LTBezierPath(new Vector3[]{
							m_TargetCameraLocation,
							new Vector3(m_TargetCameraLocation.x , m_TargetCameraLocation.y, m_TargetCameraLocation.z),
							new Vector3(targetLocation.x , targetLocation.y, targetLocation.z),
							targetLocation
						});

						//m_TargetCameraLocation = Vector3.Lerp(m_TargetCameraLocation, path.point(0.0f) , Time.deltaTime * m_SpeedForCameraAnimation);
						//Vector3 relativePos = Vector3.zero - transform.position;
						//m_TargetCameraRotation = Quaternion.LookRotation(relativePos);

						/*
						//Vector3 currentTargetLocation = m_TargetCameraLocation;
						float angleRadian = ((timeForEachTick / timeForOneFLoop) * numberOfLoop * Mathf.PI) - (Mathf.PI * 0.5f);
						float randomDistanceFromCenter = Random.Range(minDistance, maxDistance);
						m_TargetCameraLocation = Vector3.Lerp(m_TargetCameraLocation, new Vector3(Mathf.Cos(angleRadian) * randomDistanceFromCenter, height, Mathf.Sin(angleRadian) * randomDistanceFromCenter), Time.deltaTime * m_SpeedForCameraAnimation);
*/
						//m_TargetCameraLocation += Vector3.one * 0.1f;



					}).setOnCompleteOnRepeat(true).id;

				}
				else{
					m_TargetCameraLocation = m_CameraInitialLocation;
					m_TargetCameraRotation = m_CameraInitialRotation;
					//cancel animation
					if(LeanTween.descr(m_AnimationCameraIdle) != null){
						LeanTween.cancel(m_AnimationCameraIdle);
						m_AnimationCameraIdle = -1;
					}
				}
			}
			else
			{
				MakeIdle(new object[]{!m_IsIdle});
				//Log.Warning("WatsonCamera", "MakeIdle has invalid argument");
			}
		}

		public Vector3 GetTargetPositionByLoopCount(int numberOfLoop){
			float angleRadian = ((timeForEachTick / timeForOneFLoop) * numberOfLoop * Mathf.PI) - (Mathf.PI * 0.5f);
			float randomDistanceFromCenter = Random.Range(minDistance, maxDistance);
			return new Vector3(Mathf.Cos(angleRadian) * randomDistanceFromCenter, height, Mathf.Sin(angleRadian) * randomDistanceFromCenter);

		}

        #endregion

        #region Touch Drag Actions
        public void DragTwoFinger(System.Object[] args)
        {
			ChangeOnScene ();

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
			transform.rotation = Quaternion.Lerp(transform.localRotation, m_TargetCameraRotation, Time.deltaTime * m_SpeedForCameraAnimation );
			//TODO: check if position / rotation is hit the target location and rotation and stop making calculation.
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
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            try
            {
                System.Diagnostics.Process.Start("TabTip.exe");
            }
            catch (System.Exception e)
            {
                Log.Error("WatsonCamera", "ShowVirtualKeyboard has exception: {0}", e.Message);
            }
			#elif UNITY_IOS || UNITY_ANDROID
				TouchScreenKeyboard.Open("",TouchScreenKeyboardType.Default, false, false, false,false,"");
			#endif
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
