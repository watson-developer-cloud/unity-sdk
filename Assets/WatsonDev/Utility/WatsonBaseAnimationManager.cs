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

namespace IBM.Watson.Utilities
{
	/// <summary>
	/// Watson animation manager.
	/// Base class of all animation managers. 
	/// It handles all centeralized way to stop / pause / resume / speed up-down animations
	/// </summary>
	public class WatsonBaseAnimationManager : MonoBehaviour {

		private static float sm_speedModifier = 1.0f;
		private static float sm_speedModifierConstant = 1.1f;

		protected virtual void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_STOP, OnAnimationStop);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_PAUSE, OnAnimationPause);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_RESUME, OnAnimationResume);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_UP, OnAnimationSpeedUp);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DOWN, OnAnimationSpeedDown);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DEFAULT, OnAnimationSpeedDefault);

		}

		protected virtual void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_STOP, OnAnimationStop);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_PAUSE, OnAnimationPause);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_RESUME, OnAnimationResume);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_UP, OnAnimationSpeedUp);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DOWN, OnAnimationSpeedDown);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DEFAULT, OnAnimationSpeedDefault);
		}

		private void OnAnimationStop(System.Object[] args){
			OnAnimationStop ();
		}

		private void OnAnimationPause(System.Object[] args){
			OnAnimationPause ();
		}

		private void OnAnimationResume(System.Object[] args){
			OnAnimationResume ();
		}

		private void OnAnimationSpeedUp(System.Object[] args){
			sm_speedModifier *= sm_speedModifierConstant;
			OnAnimationSpeedChange (sm_speedModifier);
		}
		
		private void OnAnimationSpeedDown(System.Object[] args){
			sm_speedModifier *= (1.0f / sm_speedModifierConstant);
			OnAnimationSpeedChange (sm_speedModifier);
		}

		private void OnAnimationSpeedDefault(System.Object[] args){
			sm_speedModifier = 1.0f;
			OnAnimationSpeedChange (sm_speedModifier);
		}

		protected virtual void OnAnimationStop(){ }
		protected virtual void OnAnimationPause(){ }
		protected virtual void OnAnimationResume(){ }
		protected virtual void OnAnimationSpeedChange(float speedModifier){ }

	}
}