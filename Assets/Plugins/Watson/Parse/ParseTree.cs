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
