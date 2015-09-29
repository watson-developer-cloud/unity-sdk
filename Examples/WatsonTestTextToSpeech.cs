/**
 * Copyright 2015 IBM Corp. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/
 
using UnityEngine;
using UnityEngine.UI;

public class WatsonTestTextToSpeech : MonoBehaviour {

	public Text textForTextToSpeech;
	public WatsonTextToSpeech.VoiceType voiceForTextToSpeech = WatsonTextToSpeech.VoiceType.en_US_Michael;
	public WatsonTextToSpeech.AudioFormatType audioFormat = WatsonTextToSpeech.AudioFormatType.wav;
	public bool isTestingAllOptions = false;


	public void TestTextToSpeech(){
		WSTextToSpeech.StartSpeaking(textForTextToSpeech.text, voice: voiceForTextToSpeech );
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
