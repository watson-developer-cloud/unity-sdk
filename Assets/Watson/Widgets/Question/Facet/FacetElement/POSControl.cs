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
* @author Taj Santiago
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question.Facet.FacetElement
{
	public class POSControl : MonoBehaviour
	{
		private RectTransform m_rectTransform;
		private Color m_colorLight = new Color (0.8f, 0.8f, 0.8f);
		private Color m_colorDark = new Color (0.3f, 0.3f, 0.3f);
		private Vector3 m_scaleUpSize = new Vector3(1.25f, 1.25f, 1.25f);
		private Vector3 m_scaleDownSize = new Vector3(1f, 1f, 1f);
		private float m_transitionTime = 0.5f;

		private bool _m_isHighlighted = true;
		public bool m_isHighlighted
		{
			get { return _m_isHighlighted; }
			set {
				_m_isHighlighted = value;
				LeanTween.textColor(m_rectTransform, m_isHighlighted ? m_colorLight : m_colorDark, m_transitionTime);
				LeanTween.scale(m_rectTransform, m_isHighlighted ? m_scaleUpSize : m_scaleDownSize, m_transitionTime);
			}
		}

		public string m_POS;

		void Start()
		{
			m_rectTransform = gameObject.GetComponent<RectTransform> ();
		}
	}
}
