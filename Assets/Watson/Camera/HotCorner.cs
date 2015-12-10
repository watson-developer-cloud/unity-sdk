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

	public class HotCorner : MonoBehaviour {

		private float m_thresholdCornerWidth = 0.1f;
		private float m_thresholdCornerHeight = 0.1f;


		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (Constants.Event.ON_TAP_THREETIMES, TapThreeTimes);
		}

		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (Constants.Event.ON_TAP_THREETIMES, TapThreeTimes);
		}

		void TapThreeTimes(System.Object[] args = null){
			if (args != null && args.Length == 1 && args [0] is TouchScript.Gestures.TapGesture) {
				//Got three tap gesture, now checking the corners
				TouchScript.Gestures.TapGesture tapGesture = args [0] as TouchScript.Gestures.TapGesture;

				if(tapGesture.NormalizedScreenPosition.x < m_thresholdCornerWidth && tapGesture.NormalizedScreenPosition.y < m_thresholdCornerHeight){
					//bottom left
					TapOnBottomLeft();
				}
				else if(tapGesture.NormalizedScreenPosition.x < m_thresholdCornerWidth && tapGesture.NormalizedScreenPosition.y > 1.0f - m_thresholdCornerHeight){
					//top left
					TapOnTopLeft();
				}
				else if(tapGesture.NormalizedScreenPosition.x > 1.0f - m_thresholdCornerWidth && tapGesture.NormalizedScreenPosition.y < m_thresholdCornerHeight){
					//bottom right
					TapOnBottomRight();
				}
				else if(tapGesture.NormalizedScreenPosition.x > 1.0f - m_thresholdCornerWidth && tapGesture.NormalizedScreenPosition.y > 1.0f - m_thresholdCornerHeight){
					//top right
					TapOnTopRight();
				}
				else{
					//do nothing
				}

			} else {
				Log.Warning("WatsonCamera", "TapThreeTimes has invalid arguments.");
			}
		}

		void TapOnBottomLeft(){
			EventManager.Instance.SendEvent(Constants.Event.ON_TAP_THREETIMES_BOTTOM_LEFT);
			EventManager.Instance.SendEvent(Constants.Event.ON_DEBUG_TOGGLE);
		}

		void TapOnBottomRight(){
			EventManager.Instance.SendEvent(Constants.Event.ON_TAP_THREETIMES_BOTTOM_RIGHT);
			EventManager.Instance.SendEvent(Constants.Event.ON_VIRTUAL_KEYBOARD_TOGGLE);
		}

		void TapOnTopLeft(){
			EventManager.Instance.SendEvent(Constants.Event.ON_TAP_THREETIMES_TOP_LEFT);
		}

		void TapOnTopRight(){
			EventManager.Instance.SendEvent(Constants.Event.ON_TAP_THREETIMES_TOP_RIGHT);
			EventManager.Instance.SendEvent(Constants.Event.ON_APPLICATION_TO_BECOME_IDLE);
		}
	}

}
