using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WatsonTestTextToSpeech : MonoBehaviour {

	public Text textForTextToSpeech;
	public WatsonTextToSpeech.VoiceType voiceForTextToSpeech = WatsonTextToSpeech.VoiceType.en_US_Michael;
	public WatsonTextToSpeech.AudioFormatType audioFormat = WatsonTextToSpeech.AudioFormatType.wav;
	public bool isTestingAllOptions = false;


	public void TestTextToSpeech(){
		WSTextToSpeech.StartSpeaking(textForTextToSpeech.text, voice: voiceForTextToSpeech ,audioFormat: this.audioFormat);
	}

	void Update () {
		if (isTestingAllOptions) {
			isTestingAllOptions = false;
			testTextToSpeechAllOptions();
		}
	}

	private void testTextToSpeechAllOptions(){
		for (int i = 0; i < (int)WatsonTextToSpeech.VoiceType.NaN; i++) {
			for (int j = 0; j < (int)WatsonTextToSpeech.AudioFormatType.NaN; j++) {
				WatsonTextToSpeech.VoiceType selectedVoice = (WatsonTextToSpeech.VoiceType) i;
				WatsonTextToSpeech.AudioFormatType selectedAudioFormat = (WatsonTextToSpeech.AudioFormatType) j;
				WSTextToSpeech.StartSpeaking(textForTextToSpeech.text, voice: selectedVoice, audioFormat: selectedAudioFormat);
			}
		}

	}
}
