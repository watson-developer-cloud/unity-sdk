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
using IBM.Watson.DataModels;
using IBM.Watson.Utilities;
using IBM.Watson.Services.v1;
using IBM.Watson.Widgets;

public class Example_Dialogue : MonoBehaviour {
	private Dialog m_Dialog = new Dialog();
	private string m_DialogId = null;
	private int m_DialogClientId = 0;
	private int m_DialogConversationId = 0;
	private ClassifyResult m_ClassifyResult = null;

	private ClassifyResult GetClassifyResult( object [] args )
	{
		if ( args != null && args.Length > 0 )
			return args[0] as ClassifyResult;
		return null;
	}

	public void OnDialog(object [] args)
	{
		ClassifyResult result = GetClassifyResult( args );
		if ( result == null )
			throw new WatsonException( "ClassifyResult expected." );

		m_ClassifyResult = result;
		Debug.Log("m_ClassifyResult: " + m_ClassifyResult.classifier_id + ", " + m_ClassifyResult.text + ", " + m_ClassifyResult.top_class);
		
		if (!string.IsNullOrEmpty(m_DialogId))
		{
			if (m_Dialog.Converse(m_DialogId, result.text, OnDialogResponse, m_DialogConversationId, m_DialogClientId))
			{
				Debug.Log("conversed");
			}
		}
	}

	private void OnDialogResponse(ConverseResponse resp)
	{
		if (resp != null)
		{
			m_DialogClientId = resp.client_id;
			m_DialogConversationId = resp.conversation_id;
			
			if (resp.response != null)
			{
				foreach (var t in resp.response)
				{
//					if (!string.IsNullOrEmpty(t))
//						m_TextOutput.SendData(new TextToSpeechData(t));
				}
			}
		}
	}
}