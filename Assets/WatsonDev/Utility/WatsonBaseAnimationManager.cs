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

		/// <exclude />
		protected virtual void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_STOP, OnAnimationStop);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_PAUSE, OnAnimationPause);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_RESUME, OnAnimationResume);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_UP, OnAnimationSpeedUp);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DOWN, OnAnimationSpeedDown);
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DEFAULT, OnAnimationSpeedDefault);

		}

		/// <exclude />
		protected virtual void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_STOP, OnAnimationStop);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_PAUSE, OnAnimationPause);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_RESUME, OnAnimationResume);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_UP, OnAnimationSpeedUp);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DOWN, OnAnimationSpeedDown);
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_ANIMATION_SPEED_DEFAULT, OnAnimationSpeedDefault);
		}

		/// <summary>
		/// Event handler on animation stop
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationStop(System.Object[] args){
			OnAnimationStop ();
		}

		/// <summary>
		/// Event handler on animation pause
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationPause(System.Object[] args){
			OnAnimationPause ();
		}

		/// <summary>
		/// Event handler on animation resume
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationResume(System.Object[] args){
			OnAnimationResume ();
		}

		/// <summary>
		/// Event handler on animation speed up
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationSpeedUp(System.Object[] args){
			sm_speedModifier *= sm_speedModifierConstant;
			OnAnimationSpeedChange (sm_speedModifier);
		}

		/// <summary>
		/// Event handler on animation speed down
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationSpeedDown(System.Object[] args){
			sm_speedModifier *= (1.0f / sm_speedModifierConstant);
			OnAnimationSpeedChange (sm_speedModifier);
		}

		/// <summary>
		/// Event handler on animation speed set default
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnimationSpeedDefault(System.Object[] args){
			sm_speedModifier = 1.0f;
			OnAnimationSpeedChange (sm_speedModifier);
		}

		/// <exclude />
		protected virtual void OnAnimationStop(){ }
		/// <exclude />
		protected virtual void OnAnimationPause(){ }
		/// <exclude />
		protected virtual void OnAnimationResume(){ }
		/// <exclude />
		protected virtual void OnAnimationSpeedChange(float speedModifier){ }

	}
}