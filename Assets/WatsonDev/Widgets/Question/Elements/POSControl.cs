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

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls highlighting of Parts Of Speech TextItem.
	/// </summary>
    public class POSControl : MonoBehaviour
    {
		[SerializeField]
		private GameObject m_TextBox;

        private RectTransform m_RectTransform;
        private Color m_ColorLight = new Color(0.8f, 0.8f, 0.8f);
        private Color m_ColorDark = new Color(0.7f, 0.7f, 0.7f);
        private Vector3 m_ScaleUpSize = new Vector3(1.25f, 1.25f, 1.25f);
        private Vector3 m_ScaleDownSize = new Vector3(1f, 1f, 1f);
        private float m_TransitionTime = 0.5f;

        private bool m_IsHighlighted = true;
        public bool IsHighlighted
        {
            get { return m_IsHighlighted; }
            set
            {
                m_IsHighlighted = value;
                LeanTween.textColor(m_RectTransform, IsHighlighted ? m_ColorLight : m_ColorDark, m_TransitionTime);
                LeanTween.scale(m_RectTransform, IsHighlighted ? m_ScaleUpSize : m_ScaleDownSize, m_TransitionTime);
				LeanTween.alpha(m_TextBox.GetComponent<RectTransform>(), IsHighlighted ? 1.0f : 0.0f, m_TransitionTime);
            }
        }

        /// <summary>
        /// Part of speech - assigned in inspector window.
        /// </summary>
		[SerializeField]
		private string m_POS;
		public string POS {
			get { return m_POS; }
			set
			{
				m_POS = value;
			}
		}

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Awake()
        {
            m_RectTransform = gameObject.GetComponent<RectTransform>();
        }
    }
}
