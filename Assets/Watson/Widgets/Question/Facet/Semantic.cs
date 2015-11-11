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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

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

		private Data.XRAY.Questions m_QuestionData = null;
		private Data.XRAY.ParseData m_ParseData = null;
		
		private void OnEnable()
		{
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestionData );
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_PARSE, OnParseData );
		}
		
		private void OnDisable()
		{
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestionData );
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_PARSE, OnParseData );
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

//		override public void Init()
//        {
//			base.Init ();
//
//
//
//           
//        }

        /// <summary>
        /// Fired when Parse Data is set. Iterates through the LAT's features and concantinates features into a string.
        /// </summary>
        private string GenerateSemanticString()
        {
            string semanticText = "";

            //	Find the LAT index in the Parse Words
            int LATIndex = -1;
			for (int i = 0; i < m_ParseData.Words.Length; i++)
            {
				if (m_ParseData.Words[i].Word == LAT)
                {
                    LATIndex = i;
                }
            }

            semanticText = "";

            //	Iterate through the LAT's features and concantinate the strings together.
            if (LATIndex != -1)
            {
				for (int k = 0; k < m_ParseData.Words[LATIndex].Features.Length; k++)
                {
					semanticText += m_ParseData.Words[LATIndex].Features[k];
					if (k < m_ParseData.Words[LATIndex].Features.Length - 1)
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

		private void OnQuestionData( object [] args )
		{
			m_QuestionData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Questions : null;
			InitQuestion ();
		}

		private void OnParseData( object [] args )
		{
			m_ParseData = args != null && args.Length > 0 ? args[0] as Data.XRAY.ParseData : null;
			InitParse ();
		}

		private void InitQuestion()
		{
			if (m_QuestionData.questions[0].question.lat.Length > 0)
			{
				LAT = m_QuestionData.questions[0].question.lat[0];
			}
			else
			{
				LAT = "n/a";
			}
		}

		private void InitParse()
		{
			SemanticString = GenerateSemanticString();
		}
    }
}
