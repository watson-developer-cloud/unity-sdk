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
	public class AnswerConfidenceBar : MonoBehaviour 
	{
		[SerializeField]
		private Text m_AnswerText;
		[SerializeField]
		private Text m_ConfidenceText;
		[SerializeField]
		private RectTransform m_barProgress;

		private string _m_Answer;
		public string m_Answer
		{
			get { return _m_Answer; }
			set 
			{
				_m_Answer = value;
				UpdateAnswer();
			}
		}

		private double _m_Confidence;
		public double m_Confidence 
		{
			get { return _m_Confidence; }
			set {
				_m_Confidence = value;
				UpdateConfidence();
			}
		}

		private void UpdateAnswer()
		{
			m_AnswerText.text = m_Answer;
		}

		private void UpdateConfidence()
		{
			float confidence = (float)m_Confidence * 100;
			m_ConfidenceText.text = confidence.ToString ("f1");
			m_barProgress.localScale = new Vector3((float)m_Confidence, 1f, 1f);
		}
	}
}