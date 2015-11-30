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

namespace IBM.Watson.Utilities
{
    /// <summary>
    /// Helper class for automatically destroying objects after a static amount of time has elapsed.
    /// </summary>
    public class TimedDestroy : MonoBehaviour
    {
        [SerializeField, Tooltip("How many seconds until this component destroy's it's parent object.")]
        private float m_DestroyTime = 5.0f;
        [SerializeField]
        private bool m_AlphaFade = true;
        [SerializeField]
        private float m_FadeTime = 1.0f;
        [SerializeField]
        private Graphic m_AlphaTarget = null;
        private bool m_Fading = false;
        private float m_FadeStart = 0.0f;

        private void Start()
        {
            Invoke("OnTimeExpired", m_DestroyTime);
        }

        private void Update()
        {
            if ( m_Fading )
            {
                float fElapsed = Time.time - m_FadeStart;
                if ( fElapsed < m_FadeTime && m_AlphaTarget != null )
                {
                    Color c = m_AlphaTarget.color;
                    c.a = 1.0f - fElapsed / m_FadeTime;
                    m_AlphaTarget.color = c;
                }
                else
                    Destroy( gameObject );            
            }
        }

        private void OnTimeExpired()
        {
            if (m_AlphaFade && m_AlphaTarget != null)
            {
                m_Fading = true;
                m_FadeStart = Time.time;
            }
            else
                Destroy(gameObject);
        }

    }
}
