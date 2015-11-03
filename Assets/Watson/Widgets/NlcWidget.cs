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

using IBM.Watson.Services.v1;
using IBM.Watson.Logging;
using UnityEngine;
using UnityEngine.UI;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Natural Language Classifier Widget.
    /// </summary>
	public class NlcWidget : Widget
	{
	    #region Private Data
	    private NLC m_NLC = new NLC();

        [SerializeField]
        private Input m_ClassifyInput = new Input( "Classify", typeof(TextData), "OnClasify" );
        [SerializeField]
        private Output m_TopClassOutput = new Output( typeof(TextData) );
        [SerializeField]
        private Output m_ClassifyOutput = new Output( typeof(ClassifyResultData) );
        [SerializeField]
        private string m_ClassifierId = "5E00F7x2-nlc-540";     // default to XRAY classifier
        [SerializeField, Tooltip( "If true, then the top class is sent as a event.") ]
        private bool m_SendAsEvent = false;
        [SerializeField]
        private Text m_TopClassText = null;
        #endregion

        #region MonoBehaviour interface
        /// <exclude />
        protected override void Start()
	    {
            base.Start();
	        Logger.InstallDefaultReactors();
	    }
        #endregion

        #region Widget interface
        /// <exclude />
        protected override string GetName()
        {
            return "NlcWidget";
        }
        #endregion

        private void OnClasify( Data data )
        {
            Log.Debug( "NlcWidget", "OnClasify: {0}", ((TextData)data).Text );
            if (! m_NLC.Classify( m_ClassifierId, ((TextData)data).Text, OnClassified ) )
                Log.Error( "NlcWidget", "Failed to request classify." );
        }

	    private void OnClassified(ClassifyResult result)
	    {
            if ( m_ClassifyOutput.IsConnected )
                m_ClassifyOutput.SendData( new ClassifyResultData( result ) );

            if ( result != null )
            {
                if ( m_TopClassOutput.IsConnected && !string.IsNullOrEmpty( result.top_class ) )
                    m_TopClassOutput.SendData( new TextData( result.top_class ) );
                if ( m_TopClassText != null )
                    m_TopClassText.text = result.top_class; 
                if ( m_SendAsEvent && !string.IsNullOrEmpty( result.top_class) )
                    EventManager.Instance.SendEvent( result.top_class );
            }
	    }

	}
}
