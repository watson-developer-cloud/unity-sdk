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


using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// This class handles displaying the avatar chat.
    /// </summary>
    public class AvatarChat : MonoBehaviour
    {
        [SerializeField]
        private VerticalLayoutGroup m_ChatLayout = null;
        [SerializeField]
        private Text m_QuestionPrefab = null;
        [SerializeField]
        private Text m_AnswerPrefab = null;
        [SerializeField]
        private int m_HistoryCount = 50;
        [SerializeField]
        private Scrollbar m_ScrollBar = null;

        private AvatarWidget m_Avatar = null;
        private ObservedList<string> m_InputLogs = null;
        private ObservedList<string> m_OutputLogs = null;

        private void Start()
        {
            QuestionWidget question = GetComponentInParent<QuestionWidget>();
            if ( question != null )
                m_Avatar = question.Avatar;
            else
                m_Avatar = GetComponentInParent<AvatarWidget>();

            if ( m_Avatar != null ) 
            {
                m_Avatar.QuestionEvent += OnQuestion;
                m_Avatar.AnswerEvent += OnAnswer;
            }
            else
                Log.Warning( "AvatarChat", "Unable to find AvatarWidget." );
        }

        private void OnDestroy()
        {
            if ( m_Avatar != null )
            {
                m_Avatar.QuestionEvent -= OnQuestion;
                m_Avatar.AnswerEvent -= OnAnswer;
            }
        }

        private void OnQuestion( string add )
        {
            AddChat( add, m_QuestionPrefab.gameObject );
        }

        private void OnAnswer( string add )
        {
            AddChat( add, m_AnswerPrefab.gameObject );
        }

        private void AddChat( string add, GameObject prefab )
        {
            if ( m_ChatLayout == null )
                throw new WatsonException( "m_ChatLayout is null." );
            if ( prefab == null )
                throw new ArgumentNullException( "prefab" );

            GameObject textObject = Instantiate( prefab ) as GameObject;
            textObject.GetComponent<Text>().text = add;
            textObject.transform.SetParent( m_ChatLayout.transform, false );

            // remove old children..
            while( m_ChatLayout.transform.childCount > m_HistoryCount )
                DestroyImmediate( m_ChatLayout.transform.GetChild(0).gameObject );

            if ( m_ScrollBar != null )
                m_ScrollBar.value = 1.0f;
        }
    }

};
