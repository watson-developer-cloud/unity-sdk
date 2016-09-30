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
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleSpeechToText : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_AudioClip = new AudioClip(); 
	private SpeechToText m_SpeechToText = new SpeechToText();

	private string m_SessionIDToDelete = "7ccb52d188f695e380bc0edf19040cb3";
	//482bb56bea58429c26d0d8a2f0290e42

	void Start()
    {
		//m_SpeechToText.Recognize(m_AudioClip, HandleOnRecognize);
		LogSystem.InstallDefaultReactors();

		//	test GetModels and GetModel
		//TestGetModels();

		//	test CreateSession
		//TestCreateSession("en-US_BroadbandModel");

		//	test DeleteSesion
		TestDeleteSession(m_SessionIDToDelete);
    }

	private void TestGetModels()
	{
		Log.Debug("ExampleSpeechToText", "Attempting to get models");
		m_SpeechToText.GetModels(HandleGetModels);
	}

	private void TestGetModel(string modelID)
	{
		Log.Debug("ExampleSpeechToText", "Attempting to get model {0}", modelID);
		m_SpeechToText.GetModel(HandleGetModel, modelID);
	}

	private void TestCreateSession(string model)
	{
		Log.Debug("ExampleSpeechToText", "Attempting to create session with model {0}", model);
		m_SpeechToText.CreateSession(HandleCreateSession, model);
	}

	private void TestDeleteSession(string sessionID)
	{
		Log.Debug("ExampleSpeechToText", "Attempting to delete session session with model {0}", sessionID);
		m_SpeechToText.DeleteSession(HandleDeleteSession, sessionID);
	}

	private void HandleGetModels(Model[] models)
	{
		if (models != null)
		{
			if (models != null)
			{
				if (models.Length == 0)
				{
					Log.Warning("ExampleSpeedchToText", "There are no custom models!");
				}
				else
				{
					foreach (Model model in models)
					{
						Log.Debug("ExampleSpeechToText", "Model: {0}", model.name);
					}

					TestGetModel((models[Random.Range(0, models.Length - 1)] as Model).name);
				}
			}
		}
		else
		{
			Log.Warning("ExampleSpeechToText", "Failed to get models!");
		}
	}

	private void HandleGetModel(Model model)
	{
		if(model != null)
		{
			Log.Debug("ExampleSpeechToText", "Model - name: {0} | description: {1} | language:{2} | rate: {3} | sessions: {4} | url: {5} | customLanguageModel: {6}", 
				model.name, model.description, model.language, model.rate, model.sessions, model.url, model.supported_features.custom_language_model);
		}
		else
		{
			Log.Warning("ExampleSpeechToText", "Failed to get model!");
		}
	}

	private void HandleOnRecognize (SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0)
		{
			foreach( var res in result.results )
			{
				foreach( var alt in res.alternatives )
				{
					string text = alt.transcript;
					Debug.Log(string.Format( "{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
				}
			}
		}
	}

	private void HandleCreateSession(Session session, string customData)
	{
		if(session != null)
		{
			if (!string.IsNullOrEmpty(customData))
				Log.Debug("ExampleSpeechToText", "custom data: {0}", customData);

			Log.Debug("ExampleSpeechToText", "Session - sessionID: {0} | new_session_url: {1} | observeResult: {2} | recognize: {3} | recognizeWS: {4}", 
				session.session_id, session.new_session_uri, session.observe_result, session.recognize, session.recognizeWS);
		}
		else
		{
			Log.Debug("ExampleSpeechToText", "Failed to create session!");
		}
	}

	private void HandleDeleteSession(bool success, string data)
	{
		if (success)
		{
			Log.Debug("ExampleSpeechToText", "Deleted session!");
		}
		else
		{
			Log.Debug("ExampleSpeechToText", "Failed to delete session!");
		}
	}
}
