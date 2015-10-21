using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Semantic : QuestionComponentBase {
	[SerializeField]
	private Text m_LatText;
	[SerializeField]
	private Text m_SemanticText;

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

	new void Start()
	{
		base.Start ();
	}

	new public void Init()
	{
		base.Init ();
		if (qWidget.Questions.questions.Length > 0) {
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
		
		for(int k = 0; k < qWidget.ParseData.Words[latIndex].Features.Length; k++) {
			semanticText += qWidget.ParseData.Words[latIndex].Features[k];
			if(k < qWidget.ParseData.Words[latIndex].Features.Length - 1) {
				semanticText += ", ";
			} else {
				semanticText += ".";
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
