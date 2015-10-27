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

namespace IBM.Watson.Widgets.Question.Facet
{
	public class QuestionAndAnswer : Base
		{
		[SerializeField]
		private Text m_QuestionText;
		[SerializeField]
		private Text m_AnswerText;
		[SerializeField]
		private Text m_ConfidenceText;


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

		private string _m_Question;
		public string m_Question
		{
			get { return _m_Question; }
			set 
			{
				_m_Question = value;
				UpdateQuestion();
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

		/// <summary>
		/// Update answer view.
		/// </summary>
		private void UpdateAnswer()
		{
			m_AnswerText.text = m_Answer;
		}

		/// <summary>
		/// Update question view.
		/// </summary>
		private void UpdateQuestion()
		{
			m_QuestionText.text = m_Question;
		}

		/// <summary>
		/// Update confidence view.
		/// </summary>
		private void UpdateConfidence()
		{
			float confidence = (float)m_Confidence * 100;
			m_ConfidenceText.text = confidence.ToString ("f1");
		}

		/// <summary>
		/// Fired when Question Data is set. Sets the value of the Question.
		/// </summary>
		override protected void OnQuestionData()
		{
			m_Question = m_Questions.questions[0].question.questionText;
		}

		/// <summary>
		/// Fired when Answer Data is set. Sets the value of the Answer.
		/// </summary>
		override protected void OnAnswerData()
		{
			m_Answer = m_Answers.answers [0].answerText;
			m_Confidence = m_Answers.answers [0].confidence;
		}
	}
}
