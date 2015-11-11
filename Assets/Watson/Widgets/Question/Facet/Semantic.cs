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
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all Semantic Facet functionality.
	/// </summary>
    public class Semantic : Base
    {
        [SerializeField]
        private Text m_LATText;
        [SerializeField]
        private Text m_SemanticText;

        private string m_LAT;
        public string LAT
        {
            get { return m_LAT; }
            set
            {
                m_LAT = value;
                UpdateLAT();
            }
        }

        private string m_SemanticString;
        public string SemanticString
        {
            get { return m_SemanticString; }
            set
            {
                m_SemanticString = value;
                UpdateSemantic();
            }
        }

        /// <summary>
        /// Update the LAT view.
        /// </summary>
        private void UpdateLAT()
        {
            m_LATText.text = LAT;
        }

        /// <summary>
        /// Update the Semantic View.
        /// </summary>
        private void UpdateSemantic()
        {
            m_SemanticText.text = SemanticString;
        }

		override public void Init()
        {
			base.Init ();

            if (Question.QuestionData.QuestionDataObject.questions.Length > 0 && Question.QuestionData.QuestionDataObject.questions[0].question.lat.Length > 0)
            {
                LAT = Question.QuestionData.QuestionDataObject.questions[0].question.lat[0];
            }
            else
            {
                LAT = "n/a";
            }

            SemanticString = GenerateSemanticString();
        }

        /// <summary>
        /// Fired when Parse Data is set. Iterates through the LAT's features and concantinates features into a string.
        /// </summary>
        private string GenerateSemanticString()
        {
            string semanticText = "";

            //	Find the LAT index in the Parse Words
            int LATIndex = -1;
            for (int i = 0; i < Question.QuestionData.ParseDataObject.Words.Length; i++)
            {
                if (Question.QuestionData.ParseDataObject.Words[i].Word == LAT)
                {
                    LATIndex = i;
                }
            }

            semanticText = "";

            //	Iterate through the LAT's features and concantinate the strings together.
            if (LATIndex != -1)
            {
                for (int k = 0; k < Question.QuestionData.ParseDataObject.Words[LATIndex].Features.Length; k++)
                {
                    semanticText += Question.QuestionData.ParseDataObject.Words[LATIndex].Features[k];
                    if (k < Question.QuestionData.ParseDataObject.Words[LATIndex].Features.Length - 1)
                    {
                        semanticText += ", ";
                    }
                    else
                    {
                        semanticText += ".";
                    }
                }
            }

            return semanticText;
        }
    }
}
