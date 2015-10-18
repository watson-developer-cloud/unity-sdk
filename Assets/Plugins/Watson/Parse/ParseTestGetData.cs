using UnityEngine;
using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

public class ParseTestGetData : MonoBehaviour {
	ITM m_ITM = new ITM();
	bool m_GetPipelineTested = false;
	
	void Start ()
	{
		StartCoroutine (getPipeline());
	}

	private IEnumerator getPipeline() 
	{
		m_ITM.GetPipeline( "thunderstone", true, OnGetPipeline );
		while (! m_GetPipelineTested)
			yield return null;

		GetParse ();
	}
	
	private void OnGetPipeline( ITM.Pipeline pipeline )
	{
		Debug.Log("testing pipeline: " + pipeline);
		m_GetPipelineTested = true;
	}

	private void GetParse()
	{
		m_ITM.GetParseData( -1773927182, OnParseDataReceived);
	}

	private void OnParseDataReceived(ITM.ParseData data)
	{
		Debug.Log ("parse data words: " + data.Words.Length);

		for (int i = 0; i < data.Words.Length; i++) {
			Debug.Log("word " + i + ": " + data.Words[i].Word);
		}
	}
}
