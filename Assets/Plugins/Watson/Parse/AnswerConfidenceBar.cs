using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerConfidenceBar : MonoBehaviour {
	[SerializeField]
	private Text m_AnswerText;
	[SerializeField]
	private Text m_ConfidenceText;
	[SerializeField]
	private RectTransform m_barMask;

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
		float confidence = (float)m_Confidence;
		m_ConfidenceText.text = m_Confidence.ToString ("f1");
		m_barMask.localScale = new Vector3(confidence, 1f, 1f);
	}

//	void Start()
//	{
//		m_Answer = "here is my answer";
//		m_Confidence = 0.420549848561f;
//	}
}
