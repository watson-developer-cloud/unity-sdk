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
	public class Semantic : MonoBehaviour
		{
		[SerializeField]
		private Text m_LatText;
		[SerializeField]
		private Text m_SemanticText;

		private QuestionWidget qWidget;

		private string _m_lat;
		public string m_lat
		{
			get { return _m_lat; }
			set
			{
				_m_lat = value;
				UpdateLat();
			}
		}

		private string _m_semantic;
		public string m_semantic
		{
			get { return _m_semantic; }
			set
			{
				_m_semantic = value;
				UpdateSemantic();
			}
		}

		new public void Init()
		{
			qWidget = gameObject.GetComponent<QuestionWidget>();
			if (qWidget.Questions.questions.Length > 0 && qWidget.Questions.questions [0].question.lat.Length > 0) {
				m_lat = qWidget.Questions.questions [0].question.lat [0];
			} else {
				m_lat = "no lat";
			}
		}

		public void OnUpdateSemantic()
		{
			string semanticText = "";
			
			int latIndex = -1;
			for(int i = 0 ; i < qWidget.ParseData.Words.Length; i++) {
				if(qWidget.ParseData.Words[i].Word == m_lat) {
					latIndex = i;
				}
			}

			if (latIndex == -1) {
				semanticText = "";
			} else {
				for (int k = 0; k < qWidget.ParseData.Words[latIndex].Features.Length; k++) {
					semanticText += qWidget.ParseData.Words [latIndex].Features [k];
					if (k < qWidget.ParseData.Words [latIndex].Features.Length - 1) {
						semanticText += ", ";
					} else {
						semanticText += ".";
					}
				}
			}
			
			m_semantic = semanticText;
		}

		private void UpdateLat()
		{
			m_LatText.text = m_lat;
		}

		private void UpdateSemantic()
		{
			m_SemanticText.text = m_semantic;
		}
	}
}
