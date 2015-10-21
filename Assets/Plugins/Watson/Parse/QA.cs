using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QA : QuestionComponentBase {
	[SerializeField]
	private Text m_QuestionText;
	[SerializeField]
	private Text m_AnswerText;
	[SerializeField]
	private Text m_ConfidenceText;

	private string _m_Answer;
	public string m_Answer
	{
		get { return _m_Answer; }
		set 
		{
			_m_Answer = value;
			UpdateAnswer();
		}
	}

	private string _m_Question;
	public string m_Question
	{
		get { return _m_Question; }
		set 
		{
			_m_Question = value;
			UpdateQuestion();
		}
	}

	private double _m_Confidence;
	public double m_Confidence 
	{
		get { return _m_Confidence; }
		set {
			_m_Confidence = value;
			UpdateConfidence();
		}
	}

	void Start()
	{
		base.Start ();
	}

	private void UpdateAnswer()
	{
		m_AnswerText.text = m_Answer;
	}

	private void UpdateQuestion()
	{
		m_QuestionText.text = m_Question;
	}

	private void UpdateConfidence()
	{
		float confidence = (float)m_Confidence;
		m_ConfidenceText.text = m_Confidence.ToString ("f1");
	}
}
