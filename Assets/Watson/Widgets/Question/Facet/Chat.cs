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

namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// This class handles displaying the avatar chat.
    /// </summary>
    public class Chat : MonoBehaviour
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
        private ScrollRect m_ScrollRect = null;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestion );
            EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswer );
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestion );
            EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswer );
        }

        private void OnQuestion(object [] args)
        {
            if ( args != null && args.Length > 0 )
            {
                Data.XRAY.Questions questions = args[0] as Data.XRAY.Questions;
                if ( questions != null && questions.HasQuestion() )
                    AddChat( questions.questions[0].question.questionText, m_QuestionPrefab.gameObject );
            }
        }

        private void OnAnswer( object [] args)
        {
            if ( args != null && args.Length > 0 )
            {
                Data.XRAY.Answers answers = args[0] as Data.XRAY.Answers;
                if ( answers != null && answers.HasAnswer() )
                    AddChat( answers.answers[0].answerText, m_AnswerPrefab.gameObject );
            }
        }

        private void AddChat(string add, GameObject prefab)
        {
            if (m_ChatLayout == null)
                throw new WatsonException("m_ChatLayout is null.");
            if (prefab == null)
                throw new ArgumentNullException("prefab");

            GameObject textObject = Instantiate(prefab) as GameObject;
            textObject.GetComponent<Text>().text = add;
            textObject.transform.SetParent(m_ChatLayout.transform, false);

            // remove old children..
            while (m_ChatLayout.transform.childCount > m_HistoryCount)
                DestroyImmediate(m_ChatLayout.transform.GetChild(0).gameObject);

            Invoke("ScrollToEnd", 0.5f);
        }

        private void ScrollToEnd()
        {
            if (m_ScrollRect != null)
                m_ScrollRect.verticalNormalizedPosition = 0.0f;
        }
    }

};
