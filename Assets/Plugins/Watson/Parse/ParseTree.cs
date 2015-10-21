using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;

public class ParseTree : QuestionComponentBase {
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
		//	create word item from resources, each word item holds its own props
		for (int i = 0; i < qWidget.ParseData.Words.Length; i++) {
			GameObject wordGO = Instantiate(Resources.Load("ParseTreeTextItem", typeof(GameObject))) as GameObject;
			ParseTreeTextItem word = wordGO.GetComponent<ParseTreeTextItem>();
			word.m_slot = qWidget.ParseData.Words[i].Slot;

			for(int j = 0; j < qWidget.ParseData.Words[i].Features.Length; j++) {
				word.m_Features.Add(qWidget.ParseData.Words[i].Features[j]);
			}

			m_WordList.Add(word);
		}
	}

	private void onAdd()
	{
		Debug.Log ("word added: " + m_WordList.Count);
	}
}
