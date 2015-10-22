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
using IBM.Watson.Utilities;
using UnityEngine.UI;

public class ParseTree : QuestionComponentBase {
	[SerializeField]
	private RectTransform m_ParseCanvasRectTransform;

	private ObservedList<ParseTreeTextItem> m_WordList = new ObservedList<ParseTreeTextItem>();

	new void Start () 
	{
		base.Start ();
		m_WordList.Added += onAdd;
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
			ParseTreeTextItem word = wordGO.GetComponent<ParseTreeTextItem>();
			word.m_ParseTreeWord = qWidget.ParseData.Words[i].Word;
			word.m_pos = qWidget.ParseData.Words[i].Pos.ToString();
			word.m_slot = qWidget.ParseData.Words[i].Slot;

			for(int j = 0; j < qWidget.ParseData.Words[i].Features.Length; j++) {
				word.m_Features.Add(qWidget.ParseData.Words[i].Features[j]);
			}

			m_WordList.Add(word);
		}
	}

	private void onAdd()
	{
		Debug.Log ("word added: " + m_WordList[m_WordList.Count - 1].m_pos);
	}
}
