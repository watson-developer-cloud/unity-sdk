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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using System.Collections;
using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using IBM.Watson.Data;
using UnityEngine;

namespace IBM.Watson.UnitTests
{
    public class TestDialog : UnitTest
    {
        Dialog m_Dialog = new Dialog();
        bool m_GetDialogsTested = false;
        bool m_UploadTested = false;
        bool m_ConverseTested = false;
        string m_DialogID = null;
        int m_ClientID = 0;
        int m_ConversationID = 0;

        public override IEnumerator RunTest()
        {
            m_Dialog.GetDialogs( OnGetDialogs );
            while(! m_GetDialogsTested )
                yield return null;

            if (! m_UploadTested )
            {
                m_Dialog.UploadDialog( "xray", OnDialogUploaded, Application.streamingAssetsPath + "/pizza_sample.xml" );
                while(! m_UploadTested )
                    yield return null;
            }

            m_Dialog.Converse( m_DialogID, "Hello", OnConverse );
            while( !m_ConverseTested )
                yield return null;

            m_ConverseTested = false;
            m_Dialog.Converse( m_DialogID, "What do you have?", OnConverse, 
                m_ConversationID, m_ClientID );
            while( !m_ConverseTested )
                yield return null;

            yield break;
        }

        private void OnConverse( ConverseResponse resp )
        {
            Test( resp != null );
            if ( resp != null )
            {
                m_ClientID = resp.client_id;
                m_ConversationID = resp.conversation_id;

                foreach( var r in resp.response )
                    Log.Debug( "TestDialog", "Response: {0}", r );
            }
            m_ConverseTested = true;
        }

        private void OnDialogUploaded( string id )
        {
            Test( id != null );
            if ( id != null )
            {
                Log.Debug( "TestDialog", "Dialog ID: {0}", id );
                m_DialogID = id;
            }
            m_UploadTested = true;
        }

        private void OnGetDialogs( Dialogs dialogs )
        {
            Test( dialogs != null );
            if (dialogs != null && dialogs.dialogs != null )
            {
                foreach( var d in dialogs.dialogs )
                {
                    Log.Debug( "TestDialog", "Name: {0}, ID: {1}", d.name, d.dialog_id );
                    if ( d.name == "xray" )
                    {
                        m_UploadTested = true;
                        m_DialogID = d.dialog_id;
                    }
                }
            }
            m_GetDialogsTested = true;         
        }
    }
}

