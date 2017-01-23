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
using UnityEngine.UI;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// Displays the frame rate of the application.
  /// </summary>
  public class FrameRateCounter : MonoBehaviour
  {
    const float FPS_INTERVAL = 0.5f;
    const string DISPLAY = "{0} FPS";

    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;

    [SerializeField]
    private Text m_Text;

    private void Start()
    {
      m_FpsNextPeriod = Time.realtimeSinceStartup + FPS_INTERVAL;
    }

    private void Update()
    {
      // measure average frames per second
      m_FpsAccumulator++;
      if (Time.realtimeSinceStartup > m_FpsNextPeriod)
      {
        m_CurrentFps = (int)(m_FpsAccumulator / FPS_INTERVAL);
        m_FpsAccumulator = 0;
        m_FpsNextPeriod += FPS_INTERVAL;
        m_Text.text = string.Format(DISPLAY, m_CurrentFps);
      }
    }
  }
}
