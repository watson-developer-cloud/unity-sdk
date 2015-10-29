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
using System.Collections.Generic;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question
{
	public class ParseTreeTextItem : MonoBehaviour
		{
		[SerializeField]
		private Text m_ParseTreeTextField;

		private bool _IsHighlighted = false;
		public bool IsHighlighted
		{
			get { return _IsHighlighted; }
			set {
				_IsHighlighted = value;
				m_RectTransform = m_ParseTreeTextField.gameObject.GetComponent<RectTransform>();
				LeanTween.textColor(m_RectTransform, _IsHighlighted ? m_ColorLight : m_ColorDark, m_TransitionTime);
				LeanTween.scale(m_RectTransform, _IsHighlighted ? m_ScaleUpSize : m_ScaleDownSize, m_TransitionTime);
			}
		}

		private string _ParseTreeWord;
		public string ParseTreeWord
		{
			get { return _ParseTreeWord; }
			set 
			{
				_ParseTreeWord = value;
				UpdateParseTreeTextField();
			}
		}

		[SerializeField]
		private string _m_POS;
		public string m_POS {
			get { return _m_POS; }
			set {
				_m_POS = value;
			}
		}

		[SerializeField]
		private string _m_Slot;
		public string m_Slot
		{
			get { return _m_Slot; }
			set { _m_Slot = value; }
		}

		public List<string> m_Features = new List<string>();
		private RectTransform m_RectTransform;
		private Color m_ColorLight = new Color (0.8f, 0.8f, 0.8f);
		private Color m_ColorDark = new Color (0.3f, 0.3f, 0.3f);
		private Vector3 m_ScaleUpSize = new Vector3(1.25f, 1.25f, 1.25f);
		private Vector3 m_ScaleDownSize = new Vector3(1f, 1f, 1f);
		private float m_TransitionTime = 0.5f;

		/// <summary>
		/// Sets a reference of the RectTransform.
		/// </summary>
		void Start()
		{
			m_RectTransform = m_ParseTreeTextField.gameObject.GetComponent<RectTransform>();
		}

		/// <summary>
		/// Update the ParseTree text view.
		/// </summary>
		private void UpdateParseTreeTextField()
		{
			m_ParseTreeTextField.text = ParseTreeWord;
		}
	}
}