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
  /// Hot corners to handle Three Tap on corners and middle points of the current device screen. 
  /// </summary>
  public class HotCorner : MonoBehaviour
  {
    #region Private Members
    [SerializeField]
    private float m_NormalizedThresholdWidth = 0.1f;
    [SerializeField]
    private float m_NormalizedThresholdHeight = 0.1f;
    #endregion

    #region TapThreeTimes Event Handles

    void OnEnable()
    {
      EventManager.Instance.RegisterEventReceiver("OnTripleTap", TapThreeTimes);
    }

    void OnDisable()
    {
      EventManager.Instance.UnregisterEventReceiver("OnTripleTap", TapThreeTimes);
    }

    void TapThreeTimes(System.Object[] args = null)
    {
      if (args != null && args.Length > 0 && args[0] is TouchScript.Gestures.TapGesture)
      {
        //Got three tap gesture, now checking the corners
        TouchScript.Gestures.TapGesture tapGesture = args[0] as TouchScript.Gestures.TapGesture;

        if (tapGesture.NormalizedScreenPosition.x < m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y < m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapBottomLeft");
        }
        else if (tapGesture.NormalizedScreenPosition.x < m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y > 1.0f - m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapTopLeft");
        }
        else if (tapGesture.NormalizedScreenPosition.x > 1.0f - m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y < m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapBottomRight");
        }
        else if (tapGesture.NormalizedScreenPosition.x > 1.0f - m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y > 1.0f - m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapTopRight");
        }
        else if (tapGesture.NormalizedScreenPosition.x < m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y > 0.5f - (m_NormalizedThresholdHeight / 2.0f) && tapGesture.NormalizedScreenPosition.y < 0.5f + (m_NormalizedThresholdHeight / 2.0f))
        {
          EventManager.Instance.SendEvent("OnTripleTapMiddleLeft");
        }
        else if (tapGesture.NormalizedScreenPosition.x > 1.0f - m_NormalizedThresholdWidth && tapGesture.NormalizedScreenPosition.y > 0.5f - (m_NormalizedThresholdHeight / 2.0f) && tapGesture.NormalizedScreenPosition.y < 0.5f + (m_NormalizedThresholdHeight / 2.0f))
        {
          EventManager.Instance.SendEvent("OnTripleTapMiddleRight");
        }
        else if (tapGesture.NormalizedScreenPosition.x > 0.5f - (m_NormalizedThresholdWidth / 2.0f) && tapGesture.NormalizedScreenPosition.x < 0.5f + (m_NormalizedThresholdWidth / 2.0f) && tapGesture.NormalizedScreenPosition.y < m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapMiddleBottom");
        }
        else if (tapGesture.NormalizedScreenPosition.x > 0.5f - (m_NormalizedThresholdWidth / 2.0f) && tapGesture.NormalizedScreenPosition.x < 0.5f + (m_NormalizedThresholdWidth / 2.0f) && tapGesture.NormalizedScreenPosition.y > 1.0f - m_NormalizedThresholdHeight)
        {
          EventManager.Instance.SendEvent("OnTripleTapMiddleTop");
        }
        else
        {
          //	do nothing
        }

      }
      else
      {
        Log.Warning("WatsonCamera", "TapThreeTimes has invalid arguments.");
      }
    }

    #endregion
  }

}
