using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerConfidenceBar : MonoBehaviour {
	[SerializeField]
	private Text m_AnswerText;
	[SerializeField]
	private Text m_ConfidenceText;
	[SerializeField]
	private RectTransform m_barProgress;

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

	private double _m_Confidence;
	public double m_Confidence 
	{
		get { return _m_Confidence; }
		set {
			_m_Confidence = value;
			UpdateConfidence();
		}
	}

	private void UpdateAnswer()
	{
		m_AnswerText.text = m_Answer;
	}

	private void UpdateConfidence()
	{
		float confidence = (float)m_Confidence * 100;
		m_ConfidenceText.text = confidence.ToString ("f1");
		m_barProgress.localScale = new Vector3((float)m_Confidence, 1f, 1f);
	}

	void Start()
	{
		m_Answer = "Here is my answer";
		m_Confidence = 0.125851;
	}
}