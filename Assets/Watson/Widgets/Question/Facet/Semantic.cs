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
	public class Semantic : Base
		{
		[SerializeField]
		private Text m_LATText;
		[SerializeField]
		private Text m_SemanticText;

		private string _m_LAT;
		public string m_LAT
		{
			get { return _m_LAT; }
			set
			{
				_m_LAT = value;
				UpdateLAT();
			}
		}

		private string _m_Semantic;
		public string m_Semantic
		{
			get { return _m_Semantic; }
			set
			{
				_m_Semantic = value;
				UpdateSemantic();
			}
		}

		/// <summary>
		/// Update the LAT view.
		/// </summary>
		private void UpdateLAT()
		{
			m_LATText.text = m_LAT;
		}

		/// <summary>
		/// Update the Semantic View.
		/// </summary>
		private void UpdateSemantic()
		{
			m_SemanticText.text = m_Semantic;
		}

		/// <summary>
		/// Fired when Parse Data is set. Iterates through the LAT's features and concantinates features into a string.
		/// </summary>
		override protected void OnParseData()
		{
			string semanticText = "";

			//	Find the LAT index in the Parse Words
			int LATIndex = -1;
			for(int i = 0 ; i < m_ParseData.Words.Length; i++) {
				if(m_ParseData.Words[i].Word == m_LAT) {
					LATIndex = i;
				}
			}
			
			semanticText = "";

			//	Iterate through the LAT's features and concantinate the strings together.
			if (LATIndex != -1) {
				for (int k = 0; k < m_ParseData.Words[LATIndex].Features.Length; k++) {
					semanticText += m_ParseData.Words [LATIndex].Features [k];
					if (k < m_ParseData.Words [LATIndex].Features.Length - 1) {
						semanticText += ", ";
					} else {
						semanticText += ".";
					}
				}
			}
			
			m_Semantic = semanticText;
		}

		/// <summary>
		/// Fired when Question Data is set. Sets the value of the LAT.
		/// </summary>
		override protected void OnQuestionData()
		{
			if (m_Questions.questions.Length > 0 && m_Questions.questions [0].question.lat.Length > 0) {
				m_LAT = m_Questions.questions [0].question.lat [0];
			} else {
				m_LAT = "n/a";
			}
		}
	}
}
