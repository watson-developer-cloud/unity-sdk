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

        private IQuestionData m_QuestionData = null;

        private void Start()
        {
            QuestionWidget question = GetComponentInParent<QuestionWidget>();
            if (question != null)
            {
                m_QuestionData = question.QuestionData;
                m_QuestionData.OnAnswer += OnAnswer;
                m_QuestionData.OnQuestion += OnQuestion;
            }
        }

        private void OnDisable()
        {
            if (m_QuestionData != null)
            {
                m_QuestionData.OnAnswer -= OnAnswer;
                m_QuestionData.OnQuestion -= OnQuestion;
            }
        }

        private void OnQuestion(string add)
        {
            AddChat(add, m_QuestionPrefab.gameObject);
        }

        private void OnAnswer(string add)
        {
            AddChat(add, m_AnswerPrefab.gameObject);
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
