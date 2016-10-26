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
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;

public class TestConversation : UnitTest
{
  private Conversation m_Conversation = new Conversation();
  private string m_WorkspaceID;
  private string m_Input = "Can you unlock the door?";
  private bool m_MessageInputTested = false;
  private bool m_MessageObjectTested = false;

  public override IEnumerator RunTest()
  {
    m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");

    if (Config.Instance.FindCredentials(m_Conversation.GetServiceID()) == null)
      yield break;

    if (!m_MessageInputTested)
    {
      m_Conversation.Message(OnMessageInput, m_WorkspaceID, m_Input);
      while (!m_MessageInputTested)
        yield return null;
    }

    if (!m_MessageObjectTested)
    {
      MessageRequest messageRequest = new MessageRequest();
      messageRequest.InputText = m_Input;
      m_Conversation.Message(OnMessageObject, m_WorkspaceID, messageRequest);
      while (!m_MessageObjectTested)
        yield return null;
    }

    yield break;
  }

  private void OnMessageInput(MessageResponse resp, string customData)
  {
    Test(resp != null);
    if (resp != null)
    {
      foreach (Intent mi in resp.intents)
        Log.Debug("TestConversation", "input intent: " + mi.intent + ", confidence: " + mi.confidence);
      if (resp.output != null && resp.output.text.Length > 0)
        foreach (string txt in resp.output.text)
          Debug.Log("output: " + txt);
    }

    m_MessageInputTested = true;
  }

  private void OnMessageObject(MessageResponse resp, string customData)
  {
    Test(resp != null);
    if (resp != null)
    {
      foreach (Intent mi in resp.intents)
        Log.Debug("TestConversation", "object intent: " + mi.intent + ", confidence: " + mi.confidence);
      if (resp.output != null && resp.output.text.Length > 0)
        foreach (string txt in resp.output.text)
          Debug.Log("output: " + txt);
    }

    m_MessageObjectTested = true;
  }
}
