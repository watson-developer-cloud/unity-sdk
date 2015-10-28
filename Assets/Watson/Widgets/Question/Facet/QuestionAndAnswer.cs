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

		private string _QuestionString;
		public string QuestionString
		{
			get { return _QuestionString; }
			set 
			{
				_QuestionString = value;
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
			m_AnswerText.text = Answer;
		}

		/// <summary>
		/// Update question view.
		/// </summary>
		private void UpdateQuestion()
		{
			m_QuestionText.text = QuestionString;
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
		override protected void OnQuestion(string data)
		{
			base.OnQuestion (data);

			QuestionString = Questions.questions [0].question.questionText;
		}

		/// <summary>
		/// Fired when Answer Data is set. Sets the value of the Answer.
		/// </summary>
		override protected void OnAnswer(string data)
		{
			base.OnAnswer (data);

			Answer = Answers.answers [0].answerText;
			m_Confidence = Answers.answers [0].confidence;
		}
	}
}
