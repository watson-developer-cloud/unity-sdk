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
*
*/

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
