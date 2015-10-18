using UnityEngine;
using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;

public class ParseTestGetData : MonoBehaviour {
	ITM m_ITM = new ITM();
	bool m_GetPipelineTested = false;
	bool m_ParseTested = false;
	
	void Start ()
	{
		StartCoroutine (GetParseData());
	}

	private IEnumerator GetParseData() 
	{
		m_ITM.GetPipeline( "thunderstone", true, OnGetPipeline );
		while(! m_GetPipelineTested )
			yield return null;
		
		m_ITM.GetParseData( -1773927182, OnGetParseData );
		while(! m_ParseTested )
			yield return null;
		
		yield break;
	}
	
	private void OnGetPipeline( ITM.Pipeline pipeline )
	{
		Debug.Log("testing pipeline: " + pipeline);
		m_GetPipelineTested = true;
	}

	private void OnGetParseData( ITM.ParseData parse)
	{
		Debug.Log ("testing parse: " + parse);
		m_ParseTested = true;
	}
}
