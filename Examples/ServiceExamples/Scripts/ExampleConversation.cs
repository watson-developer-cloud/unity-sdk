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
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;

public class ExampleConversation : MonoBehaviour
{
  private Conversation m_Conversation = new Conversation();
  private string m_WorkspaceID;
  private bool m_UseAlternateIntents = true;
  private string[] questionArray = { "can you turn up the AC", "can you turn on the wipers", "can you turn off the wipers", "can you turn down the ac", "can you unlock the door" };

  void Start()
  {
    LogSystem.InstallDefaultReactors();
    m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");

    Debug.Log("**********User: Hello!");
    MessageWithOnlyInput("Hello!");
  }

  private void MessageWithOnlyInput(string input)
  {
    if (string.IsNullOrEmpty(input))
      throw new ArgumentNullException("input");

    m_Conversation.Message(OnMessageWithOnlyInput, m_WorkspaceID, input);
  }


  private void OnMessageWithOnlyInput(MessageResponse resp, string customData)
  {
    if (resp != null)
    {
      foreach (Intent mi in resp.intents)
        Debug.Log("intent: " + mi.intent + ", confidence: " + mi.confidence);

      if (resp.output != null && resp.output.text.Length > 0)
        foreach (string txt in resp.output.text)
          Debug.Log("output: " + txt);

      string questionStr = questionArray[UnityEngine.Random.Range(0, questionArray.Length - 1)];
      Debug.Log(string.Format("**********User: {0}", questionStr));

      MessageRequest messageRequest = new MessageRequest();
      messageRequest.InputText = questionStr;
      messageRequest.alternate_intents = m_UseAlternateIntents;
      messageRequest.ContextData = resp.context;

      MessageWithFullMessageRequest(messageRequest);
    }
    else
    {
      Debug.Log("Failed to invoke Message();");
    }
  }

  private void MessageWithFullMessageRequest(MessageRequest messageRequest)
  {
    if (messageRequest == null)
      throw new ArgumentNullException("messageRequest");
    m_Conversation.Message(OnMessageWithOnlyInput, m_WorkspaceID, messageRequest);
  }
}
