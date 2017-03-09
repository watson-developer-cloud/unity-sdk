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
using System.Collections.Generic;
using System.Collections;
using IBM.Watson.DeveloperCloud.DataTypes;
using System.Reflection;

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

        //Debug.Log("**********User: Hello!");
        //    MessageWithOnlyInput("Hello!");

        GetRawOutput("Hello");
    }
    
    private void GetRawOutput(string input)
    {
        m_Conversation.Message(OnGetRawOutput, m_WorkspaceID, input);
    }

    private void OnGetRawOutput(object resp, string customData)
    {
        if (!string.IsNullOrEmpty(customData))
            Debug.Log(customData);
        else
            Debug.Log("No raw data was received.");

        if (resp != null)
        {
            Dictionary<string, object> respDict = resp as Dictionary<string, object>;
            object intents;
            respDict.TryGetValue("intents", out intents);

            foreach(var intentObj in (intents as List<object>))
            {
                Dictionary<string, object> intentDict = intentObj as Dictionary<string, object>;

                object intentString;
                intentDict.TryGetValue("intent", out intentString);

                object confidenceString;
                intentDict.TryGetValue("confidence", out confidenceString);

                Log.Debug("ExampleConversation", "intent: {0} | confidence {1}", intentString.ToString(), confidenceString.ToString());
            }
        }
    }

  //private void MessageWithOnlyInput(string input)
  //{
  //  if (string.IsNullOrEmpty(input))
  //    throw new ArgumentNullException("input");

  //  m_Conversation.Message(OnMessageWithOnlyInput, m_WorkspaceID, input);
  //}


  //private void OnMessageWithOnlyInput(object resp, string customData)
  //{
  //  if (resp != null)
  //  {
  //    foreach (Intent mi in resp.intents)
  //      Debug.Log("Message Only intent: " + mi.intent + ", confidence: " + mi.confidence);

  //    if (resp.output != null && resp.output.text.Length > 0)
  //      foreach (string txt in resp.output.text)
  //        Debug.Log("Message Only output: " + txt);

  //    if (resp.context != null)
  //    {
  //      if (!string.IsNullOrEmpty(resp.context.conversation_id))
  //        Log.Debug("ExampleConversation", "Conversation ID: {0}", resp.context.conversation_id);
  //      else
  //        Log.Debug("ExampleConversation", "Conversation ID is null.");

  //      if (resp.context.system != null)
  //      {
  //        Log.Debug("ExampleConversation", "dialog_request_counter: {0}", resp.context.system.dialog_request_counter);
  //        Log.Debug("ExampleConversation", "dialog_turn_counter: {0}", resp.context.system.dialog_turn_counter);

  //        if (resp.context.system.dialog_stack != null)
  //        {
  //          foreach (Dictionary<string, string> dialogNode in resp.context.system.dialog_stack)
  //            foreach(KeyValuePair<string, string> node in dialogNode)
  //              Log.Debug("ExampleConversation", "dialogNode: {0}", node.Value);
  //        }
  //        else
  //        {
  //          Log.Debug("ExampleConversation", "dialog stack is null");
  //        }

  //      }
  //      else
  //      {
  //        Log.Debug("ExampleConversation", "system is null.");
  //      }

  //    }
  //    else
  //    {
  //      Log.Debug("ExampleConversation", "Context is null");
  //    }

  //    string questionStr = questionArray[UnityEngine.Random.Range(0, questionArray.Length - 1)];
  //    Debug.Log(string.Format("**********User: {0}", questionStr));

  //    MessageRequest messageRequest = new MessageRequest();
  //    messageRequest.InputText = questionStr;
  //    messageRequest.alternate_intents = m_UseAlternateIntents;
  //    messageRequest.ContextData = resp.context;

  //    MessageWithFullMessageRequest(messageRequest);
  //  }
  //  else
  //  {
  //    Debug.Log("Message Only: Failed to invoke Message();");
  //  }
  //}

  //private void MessageWithFullMessageRequest(MessageRequest messageRequest)
  //{
  //  if (messageRequest == null)
  //    throw new ArgumentNullException("messageRequest");
  //  m_Conversation.Message(OnMessageWithFullRequest, m_WorkspaceID, messageRequest);
  //}

  //private void OnMessageWithFullRequest(MessageResponse resp, string customData)
  //{
  //  if (resp != null)
  //  {
  //    foreach (Intent mi in resp.intents)
  //      Debug.Log("Full Request intent: " + mi.intent + ", confidence: " + mi.confidence);

  //    if (resp.output != null && resp.output.text.Length > 0)
  //      foreach (string txt in resp.output.text)
  //        Debug.Log("Full Request output: " + txt);

  //    string questionStr = questionArray[UnityEngine.Random.Range(0, questionArray.Length - 1)];
  //    Debug.Log(string.Format("**********User: {0}", questionStr));

  //    MessageRequest messageRequest = new MessageRequest();
  //    messageRequest.InputText = questionStr;
  //    messageRequest.alternate_intents = m_UseAlternateIntents;
  //    messageRequest.ContextData = resp.context;
  //  }
  //  else
  //  {
  //    Debug.Log("Full Request: Failed to invoke Message();");
  //  }
  //}

}
