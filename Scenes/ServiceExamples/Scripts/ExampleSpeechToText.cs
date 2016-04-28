using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;

public class ExampleSpeechToText : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_AudioClip = new AudioClip(); 
	private SpeechToText m_SpeechToText = new SpeechToText();

    void Start()
    {
		m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
    }

	void HandleOnRecognize (SpeechResultList result)
	{
		if (result != null && result.Results.Length > 0)
		{
			foreach( var res in result.Results )
			{
				foreach( var alt in res.Alternatives )
				{
					string text = alt.Transcript;
					Debug.Log(string.Format( "{0} ({1}, {2:0.00})\n", text, res.Final ? "Final" : "Interim", alt.Confidence));
				}
			}
		}
	}
}
