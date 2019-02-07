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

namespace IBM.Cloud.SDK.Utilities
{
    /// <summary>
    /// Helper class for automatically destroying objects after a static amount of time has elapsed.
    /// </summary>
    public class TimedDestroy : MonoBehaviour
    {
        [SerializeField, Tooltip("How many seconds until this component destroy's it's parent object.")]
        private float _destroyTime = 5.0f;
        private float _elapsedTime = 0.0f;
        private bool _timeReachedToDestroy = false;
        [SerializeField]
        private bool _alphaFade = true;
        [SerializeField]
        private bool _alphaFadeOnAwake = false;
        [SerializeField]
        private float _fadeTime = 1.0f;
        [SerializeField]
        private float _fadeTimeOnAwake = 1.0f;
        [SerializeField]
        private Graphic _alphaTarget = null;
        private bool _fading = false;
        private float _fadeStart = 0.0f;
        private Color _initialColor = Color.white;
        private float _fadeAwakeRatio = 0.0f;

        private void Start()
        {
            _elapsedTime = 0.0f;

            if (_alphaFade && _alphaTarget != null)
            {
                _initialColor = _alphaTarget.color;

                if (_alphaFadeOnAwake)
                {
                    _alphaTarget.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0.0f);
                }
            }
        }

        private void Update()
        {

            if (_alphaFadeOnAwake)
            {
                _fadeAwakeRatio += (Time.deltaTime / _fadeTimeOnAwake);
                _alphaTarget.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, Mathf.Clamp01(_fadeAwakeRatio));
                if (_fadeAwakeRatio > 1.0f)
                    _alphaFadeOnAwake = false;
            }

            if (!_timeReachedToDestroy)
            {
                _elapsedTime += Time.deltaTime;
                if (_elapsedTime > _destroyTime)
                {
                    _timeReachedToDestroy = true;
                    OnTimeExpired();
                }
            }

            if (_fading)
            {
                float fElapsed = Time.time - _fadeStart;
                if (fElapsed < _fadeTime && _alphaTarget != null)
                {
                    Color c = _alphaTarget.color;
                    c.a = 1.0f - fElapsed / _fadeTime;
                    _alphaTarget.color = c;
                }
                else
                    Destroy(gameObject);
            }
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void ResetTimer()
        {
            _elapsedTime = 0.0f;
            _fading = false;
            _timeReachedToDestroy = false;

            if (_alphaFade && _alphaTarget != null)
            {
                _alphaTarget.color = _initialColor;

            }
        }

        private void OnTimeExpired()
        {
            if (_alphaFade && _alphaTarget != null)
            {
                _fading = true;
                _fadeStart = Time.time;
            }
            else
                Destroy(gameObject);
        }
    }
}
