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
		private RectTransform m_BarProgress;

		private string _Answer;
		public string Answer
		{
			get { return _Answer; }
			set 
			{
				_Answer = value;
				UpdateAnswer();
			}
		}

		private double _Confidence;
		public double Confidence 
		{
			get { return _Confidence; }
			set {
				_Confidence = value;
				UpdateConfidence();
			}
		}

		/// <summary>
		/// Update the answer view.
		/// </summary>
		private void UpdateAnswer()
		{
			m_AnswerText.text = Answer;
		}

		/// <summary>
		/// Update the confidence view.
		/// </summary>
		private void UpdateConfidence()
		{
			float confidence = (float)Confidence * 100;
			m_ConfidenceText.text = confidence.ToString ("f1");
			m_BarProgress.localScale = new Vector3((float)Confidence, 1f, 1f);
		}
	}
}