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
#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Displays the frame rate of the application.
    /// </summary>
    public class FrameRateCounter : MonoBehaviour
    {
        const float FpsInterval = 0.5f;
        const string Display = "{0} FPS";

        private int _fpsAccumulator = 0;
        private float _fpsNextPeriod = 0;
        private int _currentFps;

        [SerializeField]
        private Text _text;

        private void Start()
        {
            _fpsNextPeriod = Time.realtimeSinceStartup + FpsInterval;
        }

        private void Update()
        {
            // measure average frames per second
            _fpsAccumulator++;
            if (Time.realtimeSinceStartup > _fpsNextPeriod)
            {
                _currentFps = (int)(_fpsAccumulator / FpsInterval);
                _fpsAccumulator = 0;
                _fpsNextPeriod += FpsInterval;
                _text.text = string.Format(Display, _currentFps);
            }
        }
    }
}
