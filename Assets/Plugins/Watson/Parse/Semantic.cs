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

	void Start()
	{
		base.Start ();

		m_lat = qWidget.Questions.questions [0].question.lat[0];
		m_semantic = qWidget.
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
