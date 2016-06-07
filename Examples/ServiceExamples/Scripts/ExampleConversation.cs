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
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;

public class ExampleConversation : MonoBehaviour
{
	private Conversation m_Conversation = new Conversation();
	private string m_WorkspaceID = "car_demo_1";
	private string m_Input = "Can you unlock the door?";

	void Start () {
		Debug.Log("User: " + m_Input);
		m_Conversation.Message(m_WorkspaceID, m_Input, OnMessage);
	}

	void OnMessage (DataModels.MessageResponse resp)
	{
		foreach(DataModels.MessageIntent mi in resp.intents)
			Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);
		
		Debug.Log("response: " + resp.output.text);
	}
}
