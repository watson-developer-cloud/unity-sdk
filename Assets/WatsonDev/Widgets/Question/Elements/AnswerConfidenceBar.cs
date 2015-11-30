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

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls AnswerConfidenceBar views. Attached to two types of AnswerConfidenceBar prefabs.
	/// </summary>
    public class AnswerConfidenceBar : MonoBehaviour
    {
        [SerializeField]
        private Text m_AnswerText;
        [SerializeField]
        private Text m_ConfidenceText;
        [SerializeField]
        private RectTransform m_BarProgress;

        private string m_Answer;
        public string Answer
        {
            get { return m_Answer; }
            set
            {
                m_Answer = value;
                UpdateAnswer();
            }
        }


        private double m_Confidence;
        public double Confidence
        {
            get { return m_Confidence; }
            set
            {
                m_Confidence = value;
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
            m_ConfidenceText.text = confidence.ToString("f1");
            m_BarProgress.localScale = new Vector3((float)Confidence, 1f, 1f);
        }
    }
}