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
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleConversation : MonoBehaviour
{
	private Conversation m_Conversation = new Conversation();
    private string m_WorkspaceID;
	private string m_Input = "Can you unlock the door?";

	void Start () {
        LogSystem.InstallDefaultReactors();
        m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
        Debug.Log("User: " + m_Input);

        //  Message with input only
        //m_Conversation.Message(OnMessage, m_WorkspaceID, m_Input);

        //  Message by creating message request
        //MessageRequest messageRequest = new MessageRequest();
        //messageRequest.inputText = m_Input;
        //m_Conversation.Message(OnMessage, m_WorkspaceID, messageRequest);

        //  Message by passing input, alternate intents and conversationID
        m_Conversation.Message(OnMessage, m_WorkspaceID, m_Input, false, null);
	}

	void OnMessage (MessageResponse resp, string customData)
	{
        if(resp != null)
        {
    		foreach(Intent mi in resp.intents)
    			Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);
    		
            if(resp.output != null && resp.output.text.Length > 0)
                foreach(string txt in resp.output.text)
    		        Debug.Log("output: " + txt);
        }
        else
        {
            Debug.Log("Failed to invoke Message();");
        }
	}
}
