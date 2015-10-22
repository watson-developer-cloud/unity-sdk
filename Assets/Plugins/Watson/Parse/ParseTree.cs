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

public class ParseTree : QuestionComponentBase {
	[SerializeField]
	private RectTransform m_ParseCanvasRectTransform;

	[SerializeField]
	private List<GameObject> m_POSList = new List<GameObject>();
	private List<ParseTreeTextItem> m_WordList = new List<ParseTreeTextItem>();
	private List<Vector3> positionList = new List<Vector3> ();
	
	private int _wordIndex = 0;
	public int wordIndex 
	{
		get { return _wordIndex; }
		set {
			if(value > m_WordList.Count - 1) {
				_wordIndex = 0;
			} else if(value < 0) {
				_wordIndex = m_WordList.Count;
			} else {
				_wordIndex = value;
			}
			UpdateHighlightedWord();
		}
	}

	new void Start () 
	{
		base.Start ();
		positionList.Add(new Vector3(-583f, 188f, 0f));
		positionList.Add(new Vector3(-408f,	64f, 0f));
		positionList.Add(new Vector3(-184f, -49f, 0f));
		positionList.Add(new Vector3(27f, -168f, 0f));
		positionList.Add(new Vector3(259f, -301f, 0f));
		positionList.Add(new Vector3(469f, -424f, 0f));
		positionList.Add(new Vector3(-638f, -31f, 0f));
		positionList.Add(new Vector3(-417f, -144f, 0f));
		positionList.Add(new Vector3(-144f, -282f, 0f));
		positionList.Add(new Vector3(109f, -397f, 0f));
		positionList.Add(new Vector3(348f, -560f, 0f));
		positionList.Add(new Vector3(-643f, -268f, 0f));
		positionList.Add(new Vector3(-346f, -393f, 0f));
		positionList.Add(new Vector3(-115f, -514f, 0f));
		positionList.Add(new Vector3(91f, -641f, 0f));

//		m_WordList.Added += OnAdd;
//		m_WordList.Removed += OnRemove;
	}

	new public void Init()
	{
		base.Init ();
	}

	public void GenerateParseTree()
	{
		for (int i = 0; i < qWidget.ParseData.Words.Length; i++) {
			GameObject wordGO = Instantiate(Resources.Load("ParseTreeTextItem", typeof(GameObject))) as GameObject;
			RectTransform wordRectTransform = wordGO.GetComponent<RectTransform>();
			wordRectTransform.SetParent(m_ParseCanvasRectTransform, false);
			wordRectTransform.localPosition = positionList[i];
			ParseTreeTextItem word = wordGO.GetComponent<ParseTreeTextItem>();
			word.m_ParseTreeWord = qWidget.ParseData.Words[i].Word;
			word.m_pos = qWidget.ParseData.Words[i].Pos.ToString();
			word.m_slot = qWidget.ParseData.Words[i].Slot;

			for(int j = 0; j < qWidget.ParseData.Words[i].Features.Length; j++) {
				word.m_Features.Add(qWidget.ParseData.Words[i].Features[j]);
			}

			m_WordList.Add(word);
		}

		wordIndex = 0;
		InvokeRepeating ("CycleWords", 2f, 2f);
	}

	public void ClearParseTree()
	{
		CancelInvoke ();
		while(m_WordList.Count != 0) {
			Destroy(m_WordList[0].gameObject);
			m_WordList.Remove(m_WordList[0]);
		}
	}

//	private void OnAdd()
//	{
//		Debug.Log ("word added: " + m_WordList[m_WordList.Count - 1].m_pos);
//	}
//
//	private void OnRemove()
//	{
//		Debug.Log ("word removed: " + m_WordList.Count);
//	}

	private void UpdateHighlightedWord()
	{
		for (int i = 0; i < m_WordList.Count; i++) {
			m_WordList [i].isHighlighted = false;
		}

		m_WordList [wordIndex].isHighlighted = true;

		for (int j = 0; j < m_POSList.Count; j++) {
			POSControl posControl = m_POSList[j].GetComponent<POSControl>();
			//Debug.Log("posControl.m_POS: " + posControl.m_POS + ", WordPOS: " + m_WordList [wordIndex].m_pos.ToLower());
			if(posControl.m_POS == m_WordList [wordIndex].m_pos.ToLower()) {
				posControl.isHighlighted = true;
			} else {
				posControl.isHighlighted = false;
			}
		}

		if(qWidget.Questions.questions [0].question.lat.Length == 0 && qWidget.Questions.questions [0].question.focus.Length == 0) m_POSList[2].GetComponent<POSControl>().isHighlighted = true;
		if (qWidget.Questions.questions [0].question.lat.Length > 0) {
			if (m_WordList [wordIndex].m_ParseTreeWord.ToLower () == qWidget.Questions.questions [0].question.lat [0].ToLower ()) {
				m_POSList [1].GetComponent<POSControl> ().isHighlighted = true;
			}
		}


		if (qWidget.Questions.questions [0].question.focus.Length > 0) {
			if (m_WordList [wordIndex].m_ParseTreeWord.ToLower () == qWidget.Questions.questions [0].question.focus [0].ToLower ()) {
				m_POSList [0].GetComponent<POSControl> ().isHighlighted = true;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.C)) {
			wordIndex --;
		}

		if (Input.GetKeyDown (KeyCode.V)) {
			wordIndex ++;
		}
	}

	private void CycleWords()
	{
		wordIndex ++;
	}
}
