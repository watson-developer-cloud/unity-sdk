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
* @author Taj Santiago (asantiago@us.ibm.com)
*/

using UnityEngine;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.DataModels;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all AnswersAndConfidence Facet functionality.
	/// </summary>
    public class AnswersAndConfidence : Facet
    {
        [Header("UI Faces")]
        [SerializeField]
        private AnswerConfidenceBar[] m_AnswerConfidenceBars;

		private DataModels.XRAY.Answers m_AnswerData = null;

		private void OnEnable()
		{
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}
		
		private void OnDisable()
		{
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}

		private void OnAnswerData( object [] args )
		{
            if ( Focused )
            {
			    m_AnswerData = args != null && args.Length > 0 ? args[0] as DataModels.XRAY.Answers : null;
                for (int i = 0; i < m_AnswerConfidenceBars.Length; i++)
                {
				    if ( i < m_AnswerData.answers.Length )
                    {
					    m_AnswerConfidenceBars[i].Answer = m_AnswerData.answers[i].answerText;
					    m_AnswerConfidenceBars[i].Confidence = m_AnswerData.answers[i].confidence;
                    }
                    else
                    {
                        m_AnswerConfidenceBars[i].Answer = string.Empty;
                        m_AnswerConfidenceBars[i].Confidence = 0.0f;
                    }
                }
            }
		}
    }
}
