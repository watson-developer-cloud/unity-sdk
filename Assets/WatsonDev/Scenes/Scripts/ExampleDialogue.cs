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
using IBM.Watson.Logging;
using IBM.Watson.DataModels;
using IBM.Watson.DataTypes;

public class ExampleDialogue : Widget {
	#region implemented abstract members of Widget

	protected override string GetName ()
	{
		return "Dialog";
	}

	#endregion

	const string DIALOG_NAME = "ut_20151029_5";
	
	Dialog m_Dialog = new Dialog();
	bool m_GetDialogsTested = false;
	string m_DialogID = null;
	int m_ClientID = 0;
	int m_ConversationID = 0;
	bool isDialogAvailable = false;

	[SerializeField]
	private Input m_SpeechInput = new Input( "SpeechInput", typeof(SpeechToTextData), "OnSpeechInput" );

	void OnEnable()
	{
		m_Dialog.GetDialogs( OnGetDialogs );
//		m_Dialog.UploadDialog( DIALOG_NAME, OnDialogUploaded, Application.dataPath + "/../Docs/pizza_sample.xml" );
	}

	void OnDisable()
	{
//		m_Dialog.DeleteDialog( m_DialogID, OnDialogDeleted );
	}

	public void Converse(string dialog)
	{
		if (! string.IsNullOrEmpty( m_DialogID ) )
		{
			m_Dialog.Converse( m_DialogID, dialog, OnConverse );
		}
	}

	private void OnDialogDeleted( bool success )
	{
		Log.Debug("Example_Dialogue", "Deleted");
	}
	
	private void OnConverse( ConverseResponse resp )
	{
		if ( resp != null )
		{
			m_ClientID = resp.client_id;
			m_ConversationID = resp.conversation_id;
			
			foreach( var r in resp.response )
				Log.Debug( "TestDialog", "Response: {0}", r );
		}
	}
	
	private void OnDialogUploaded( string id )
	{
		if (! string.IsNullOrEmpty( id ) )
		{
			Log.Debug( "TestDialog", "Dialog ID: {0}", id );
			m_DialogID = id;
		}
	}
	
	private void OnGetDialogs( Dialogs dialogs )
	{
		if (dialogs != null && dialogs.dialogs != null )
		{
			foreach( var d in dialogs.dialogs )
			{
				Log.Debug( "TestDialog", "Name: {0}, ID: {1}", d.name, d.dialog_id );
				if ( d.name == DIALOG_NAME )
				{
					isDialogAvailable = true;
					m_DialogID = d.dialog_id;
				}
			}
		}

		if(!isDialogAvailable) m_Dialog.UploadDialog( DIALOG_NAME, OnDialogUploaded, Application.dataPath + "/../Assets/Watson/Editor/TestData/pizza_sample.xml" );
	}
}