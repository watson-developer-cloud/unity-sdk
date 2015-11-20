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
* @author Taj Santiago
*/

using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data;

namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// Handles all Evidence Facet functionality. 
    /// </summary>
    public class Evidence : Facet
    {
        [SerializeField]
        private GameObject m_EvidenceItemPrefab;
        [SerializeField]
        private RectTransform m_EvidenceCanvasRectTransform;
        [SerializeField]
        private int m_MaxEvidence = 3;

        private List<EvidenceItem> m_EvidenceItems = new List<EvidenceItem>();
        private Data.XRAY.Answers m_AnswerData = null;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData);
        }

        private void OnAnswerData(object[] args)
        {
            if ( Focused )
            {
                m_AnswerData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Answers : null;

                while (m_EvidenceItems.Count != 0)
                {
                    Destroy(m_EvidenceItems[0].gameObject);
                    m_EvidenceItems.RemoveAt(0);
                }

                if (m_AnswerData != null && m_AnswerData.HasAnswer()
                    && m_AnswerData.answers[0].evidence != null)
                {
                    Data.XRAY.Answer answer = m_AnswerData.answers[0];

                    for (int i = 0; i < answer.evidence.Length; i++)
                    {
                        if (i >= m_MaxEvidence)
                            break;

                        GameObject evidenceItemGameObject = Instantiate(m_EvidenceItemPrefab, new Vector3(0f, -i * 60f, 0f), Quaternion.identity) as GameObject;
                        RectTransform evidenceItemRectTransform = evidenceItemGameObject.GetComponent<RectTransform>();
                        evidenceItemRectTransform.SetParent(m_EvidenceCanvasRectTransform, false);
                        EvidenceItem evidenceItem = evidenceItemGameObject.GetComponent<EvidenceItem>();
                        evidenceItem.Answer = m_AnswerData.answers[0].answerText;
                        evidenceItem.EvidenceString = m_AnswerData.answers[0].evidence[i].decoratedPassage;

                        m_EvidenceItems.Add(evidenceItem);
                    }
                }
            }
        }
    }
}
