using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;

public class ExampleTextToSpeech : MonoBehaviour
{
	TextToSpeech m_TextToSpeech = new TextToSpeech();
	string m_TestString = "Hello! This is Text to Speech!";

	void Start ()
	{
		m_TextToSpeech.Voice = VoiceType.en_GB_Kate;
		m_TextToSpeech.ToSpeech(m_TestString, HandleToSpeechCallback);
	}

	void HandleToSpeechCallback (AudioClip clip)
	{
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			GameObject.Destroy(audioObject, clip.length);
		}
	}
}
