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
using System.Collections.Generic;
using UnityEngine.UI;
using IBM.Watson.Widgets.Question.Facet.FacetElement;

namespace IBM.Watson.Widgets.Question.Facet
{
	public class ParseTree : FacetBase
		{
		[SerializeField]
		private GameObject m_ParseTreeTextItem;

		[SerializeField]
		private RectTransform m_ParseCanvasRectTransform;

		[SerializeField]
		private List<GameObject> m_POSList = new List<GameObject>();
		private List<ParseTreeTextItem> m_WordList = new List<ParseTreeTextItem>();
		private List<Vector3> m_PositionList = new List<Vector3> ();
		
		private int _m_WordIndex = 0;
		public int m_WordIndex 
		{
			get { return _m_WordIndex; }
			set {
				if(value > m_WordList.Count - 1) {
					_m_WordIndex = 0;
				} else if(value < 0) {
					_m_WordIndex = m_WordList.Count;
				} else {
					_m_WordIndex = value;
				}
				UpdateHighlightedWord();
			}
		}

		/// <summary>
		/// Set hard coded positions.
		/// </summary>
		void Start () 
		{
			//	TODO parse tree from hiearchy
			m_PositionList.Add(new Vector3(-583f, 188f, 0f));
			m_PositionList.Add(new Vector3(-408f,	64f, 0f));
			m_PositionList.Add(new Vector3(-184f, -49f, 0f));
			m_PositionList.Add(new Vector3(27f, -168f, 0f));
			m_PositionList.Add(new Vector3(259f, -301f, 0f));
			m_PositionList.Add(new Vector3(469f, -424f, 0f));
			m_PositionList.Add(new Vector3(-638f, -31f, 0f));
			m_PositionList.Add(new Vector3(-417f, -144f, 0f));
			m_PositionList.Add(new Vector3(-144f, -282f, 0f));
			m_PositionList.Add(new Vector3(109f, -397f, 0f));
			m_PositionList.Add(new Vector3(348f, -560f, 0f));
			m_PositionList.Add(new Vector3(-643f, -268f, 0f));
			m_PositionList.Add(new Vector3(-346f, -393f, 0f));
			m_PositionList.Add(new Vector3(-115f, -514f, 0f));
			m_PositionList.Add(new Vector3(91f, -641f, 0f));
		}

		/// <summary>
		/// Set reference to QuestionWidget.
		/// </summary>
		public override void Init()
		{
			base.Init ();
		}

		/// <summary>
		/// Generate parse tree from Parse Data.
		/// </summary>
		public void GenerateParseTree()
		{
			for (int i = 0; i < m_QuestionWidget.ParseData.Words.Length; i++) {
				GameObject wordGO = Instantiate(m_ParseTreeTextItem) as GameObject;
				RectTransform wordRectTransform = wordGO.GetComponent<RectTransform>();
				wordRectTransform.SetParent(m_ParseCanvasRectTransform, false);
				if(i < m_PositionList.Count) {
					wordRectTransform.localPosition = m_PositionList[i];
				} else {
					//	TODO fix this
					wordRectTransform.localPosition = new Vector3(5000f, 5000, 5000f);
				}
				ParseTreeTextItem word = wordGO.GetComponent<ParseTreeTextItem>();
				word.m_ParseTreeWord = m_QuestionWidget.ParseData.Words[i].Word;
				word.m_POS = m_QuestionWidget.ParseData.Words[i].Pos.ToString();
				word.m_Slot = m_QuestionWidget.ParseData.Words[i].Slot;

				for(int j = 0; j < m_QuestionWidget.ParseData.Words[i].Features.Length; j++) {
					word.m_Features.Add(m_QuestionWidget.ParseData.Words[i].Features[j]);
				}

				m_WordList.Add(word);
			}

			m_WordIndex = 0;
			InvokeRepeating ("CycleWords", 2f, 2f);
		}

		/// <summary>
		/// Delete parse list and parse GameObjects.
		/// </summary>
		public void ClearParseTree()
		{
			CancelInvoke ();
			while(m_WordList.Count != 0) {
				Destroy(m_WordList[0].gameObject);
				m_WordList.Remove(m_WordList[0]);
			}
		}

		/// <summary>
		/// Highlight words, POS and Slots based on highlighted word.
		/// </summary>
		private void UpdateHighlightedWord()
		{
			for (int i = 0; i < m_WordList.Count; i++) {
				m_WordList [i].m_IsHighlighted = false;
			}

			m_WordList [m_WordIndex].m_IsHighlighted = true;

			for (int j = 0; j < m_POSList.Count; j++) {
				POSControl posControl = m_POSList[j].GetComponent<POSControl>();
				if(posControl.m_POS == m_WordList [m_WordIndex].m_POS.ToLower() || posControl.m_POS == m_WordList[m_WordIndex].m_Slot.ToLower()) {
					posControl.m_IsHighlighted = true;
				} else {
					posControl.m_IsHighlighted = false;
				}
			}

			if(m_QuestionWidget.Questions.questions [0].question.lat.Length == 0 && m_QuestionWidget.Questions.questions [0].question.focus.Length == 0) m_POSList[2].GetComponent<POSControl>().m_IsHighlighted = true;
			if (m_QuestionWidget.Questions.questions [0].question.lat.Length > 0) {
				if (m_WordList [m_WordIndex].m_ParseTreeWord.ToLower () == m_QuestionWidget.Questions.questions [0].question.lat [0].ToLower ()) {
					m_POSList [1].GetComponent<POSControl> ().m_IsHighlighted = true;
				}
			}


			if (m_QuestionWidget.Questions.questions [0].question.focus.Length > 0) {
				if (m_WordList [m_WordIndex].m_ParseTreeWord.ToLower () == m_QuestionWidget.Questions.questions [0].question.focus [0].ToLower ()) {
					m_POSList [0].GetComponent<POSControl> ().m_IsHighlighted = true;
				}
			}
		}

		/// <summary>
		/// Cycles the words.
		/// </summary>
		private void CycleWords()
		{
			m_WordIndex ++;
		}
	}
}
