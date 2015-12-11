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

	public class IdleCamera : CameraTarget {

		/*
		private Vector3 m_TargetCameraLocationBeforeIdle;
		private Quaternion m_TargetCameraRotationBeforeIdle;
		private int m_AnimationCameraIdle = -1;
		private float m_LastTimeActionCaptured = 0.0f;
		private float m_TimeForWaitingToBecomeIdle = 6000.0f;	//10min as idle just test purpose for now! TODO: change it!
		private bool m_IsIdle = false;


		//TODO: Add boundary limits
		//private Vector3 targetCenter = Vector3.zero;    //Center of Avatar
		//private Vector2 boundaryLimit = Vector2.zero;

		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_KEYBOARD_ANYKEY_DOWN, ChangeOnScene);			//any keyboard press make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_TOUCH_PRESSED_FULLSCREEN, ChangeOnScene);		//any touch press make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_TOUCH_RELEASED_FULLSCREEN, ChangeOnScene);		//any touch release make it awake
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, ChangeOnScene);		//and avatar state change (voice recognition / or failure make it awake)
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, QuestionStateChanged);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, MakeIdle);
		}

		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_KEYBOARD_ANYKEY_DOWN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_TOUCH_PRESSED_FULLSCREEN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_TOUCH_RELEASED_FULLSCREEN, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, ChangeOnScene);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, QuestionStateChanged);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_APPLICATION_TO_BECOME_IDLE, MakeIdle);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_VIRTUAL_KEYBOARD_TOGGLE, ShowVirtualKeyboard);
		}
		
		void Start(){
			ChangeOnScene ();
		}
		
		void Update()
		{
			CheckIdleState();
			CameraPositionOnUpdate();
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
						

						//Vector3 currentTargetLocation = m_TargetCameraLocation;
//						float angleRadian = ((timeForEachTick / timeForOneFLoop) * numberOfLoop * Mathf.PI) - (Mathf.PI * 0.5f);
//						float randomDistanceFromCenter = Random.Range(minDistance, maxDistance);
//						m_TargetCameraLocation = Vector3.Lerp(m_TargetCameraLocation, new Vector3(Mathf.Cos(angleRadian) * randomDistanceFromCenter, height, Mathf.Sin(angleRadian) * randomDistanceFromCenter), Time.deltaTime * m_SpeedForCameraAnimation);

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
		*/
	}



}
