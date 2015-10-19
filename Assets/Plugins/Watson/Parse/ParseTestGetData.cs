using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

public class ParseTestGetData : MonoBehaviour {
	private ITM m_ITM = new ITM();
	private bool m_GetPipelineTested = false;
	private bool m_GetQuestionsTested = false;
	public GameObject parseCubeWidget;
	
	void Start ()
	{
		StartCoroutine (getPipeline());
	}


	private IEnumerator getPipeline() 
	{
		m_ITM.GetPipeline( "thunderstone", true, OnGetPipeline );
		while (! m_GetPipelineTested)
			yield return null;

		GetQuestions ();
		GetAnswers ();
		GetParse ();
	}

	
	private void OnGetPipeline( ITM.Pipeline pipeline )
	{
		Debug.Log("testing pipeline: " + pipeline);
		m_GetPipelineTested = true;
	}


	private void GetQuestions()
	{
		m_ITM.GetQuestions( OnGetQuestions );
	}


	private void GetParse()
	{
		m_ITM.GetParseData( -1773927182, OnParseDataReceived);
	}


	private void GetAnswers()
	{
		m_ITM.GetAnswers (-1773927182, OnGetAnswers);
	}


	private void OnParseDataReceived(ITM.ParseData data)
	{
		Debug.Log ("parse data: " + data);

		for (int i = 0; i < data.Words.Length; i++) {
			Debug.Log("word " + i + ": " + data.Words[i].Word);
		}
	}


	private void OnGetQuestions( ITM.Questions questions )
	{
		Debug.Log ("questions: " + questions);
		if ( questions != null )
		{
			for(int i=0;i<questions.questions.Length;++i)
				Log.Status( "TestITM", "Question: {0}", questions.questions[i] );
		}
		m_GetQuestionsTested = true;
	}


	private void OnGetAnswers(ITM.Answers answers)
	{
		Debug.Log ("answers: " + answers);
	}

	public Text answer0;
	public Text answer1;
	public Text answer2;
	public Text answer3;
	public Text answer4;
	public Text answer5;
	public Text answer6;
	public Text answer7;
	public Text answer8;
	public Text answer9;
	public Text confidence0;
	public Text confidence1;
	public Text confidence2;
	public Text confidence3;
	public Text confidence4;
	public Text confidence5;
	public Text confidence6;
	public Text confidence7;
	public Text confidence8;
	public Text confidence9;

	private void populate0(ITM.Answers answers)
	{
//		for(int i = 0; i < answers.answers.Length; i++) {
//
//		}

		answer0.text = answers.answers [0].answerText;
		answer1.text = answers.answers [1].answerText;
		answer2.text = answers.answers [2].answerText;
		answer3.text = answers.answers [3].answerText;
		answer4.text = answers.answers [4].answerText;
		answer5.text = answers.answers [5].answerText;
		answer6.text = answers.answers [6].answerText;
		answer7.text = answers.answers [7].answerText;
		answer8.text = answers.answers [8].answerText;
		answer9.text = answers.answers [9].answerText;

		confidence0.text = answers.answers [0].confidence;
		confidence1.text = answers.answers [1].confidence;
		confidence2.text = answers.answers [2].confidence;
		confidence3.text = answers.answers [3].confidence;
		confidence4.text = answers.answers [4].confidence;
		confidence5.text = answers.answers [5].confidence;
		confidence6.text = answers.answers [6].confidence;
		confidence7.text = answers.answers [7].confidence;
		confidence8.text = answers.answers [8].confidence;
		confidence9.text = answers.answers [9].confidence;

	}
}
