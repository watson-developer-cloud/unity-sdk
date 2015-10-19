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

	//	panel 1
	public Text tf_answer0;
	public Text tf_answer1;
	public Text tf_answer2;
	public Text tf_answer3;
	public Text tf_answer4;
	public Text tf_answer5;
	public Text tf_answer6;
	public Text tf_answer7;
	public Text tf_answer8;
	public Text tf_answer9;
	public Text tf_confidence0;
	public Text tf_confidence1;
	public Text tf_confidence2;
	public Text tf_confidence3;
	public Text tf_confidence4;
	public Text tf_confidence5;
	public Text tf_confidence6;
	public Text tf_confidence7;
	public Text tf_confidence8;
	public Text tf_confidence9;

	//	panel 2
	public Text tf_question;
	public Text tf_answer;
	public Text tf_topConfidence;

	private void populateCube(ITM.Answers answers, ITM.Questions questions, ITM.ParseData parse)
	{
//		for(int i = 0; i < answers.answers.Length; i++) {
//
//		}

		//	panel 1
		tf_answer0.text = answers.answers [0].answerText;
		tf_answer1.text = answers.answers [1].answerText;
		tf_answer2.text = answers.answers [2].answerText;
		tf_answer3.text = answers.answers [3].answerText;
		tf_answer4.text = answers.answers [4].answerText;
		tf_answer5.text = answers.answers [5].answerText;
		tf_answer6.text = answers.answers [6].answerText;
		tf_answer7.text = answers.answers [7].answerText;
		tf_answer8.text = answers.answers [8].answerText;
		tf_answer9.text = answers.answers [9].answerText;

		tf_confidence0.text = answers.answers [0].confidence.ToString();
		tf_confidence1.text = answers.answers [1].confidence.ToString();
		tf_confidence2.text = answers.answers [2].confidence.ToString();
		tf_confidence3.text = answers.answers [3].confidence.ToString();
		tf_confidence4.text = answers.answers [4].confidence.ToString();
		tf_confidence5.text = answers.answers [5].confidence.ToString();
		tf_confidence6.text = answers.answers [6].confidence.ToString();
		tf_confidence7.text = answers.answers [7].confidence.ToString();
		tf_confidence8.text = answers.answers [8].confidence.ToString();
		tf_confidence9.text = answers.answers [9].confidence.ToString();

		//	panel 2
//		tf_question.text = questions.questions
	}
}
