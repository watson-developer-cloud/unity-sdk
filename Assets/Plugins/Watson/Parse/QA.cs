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
		Debug.Log (qWidget.Questions);
		m_Question = qWidget.Questions.questions[0].question.questionText;
		m_Answer = qWidget.Answers.answers [0].answerText;
		m_Confidence = qWidget.Answers.answers [0].confidence;
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
		float confidence = (float)m_Confidence * 100;
		m_ConfidenceText.text = confidence.ToString ("f1");
	}
}
