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

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all AnswersAndConfidence Facet functionality.
	/// </summary>
    public class AnswersAndConfidence : Base
    {
        [Header("UI Faces")]
        [SerializeField]
        private AnswerConfidenceBar[] m_AnswerConfidenceBars;

		/// <summary>
		/// Iterate through Answer and Confidence bars and set the Answer and ConfidenceIndex
		/// </summary>
		//	TODO dynamically create bars?
        override public void Init()
        {
            for (int i = 0; i < m_AnswerConfidenceBars.Length; i++)
            {
                m_AnswerConfidenceBars[i].Answer = m_Question.QuestionData.AnswerDataObject.answers[i].answerText;
                m_AnswerConfidenceBars[i].Confidence = m_Question.QuestionData.AnswerDataObject.answers[i].confidence;
            }
        }
    }
}
