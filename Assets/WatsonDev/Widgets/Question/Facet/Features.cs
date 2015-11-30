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
* @author Taj Santiago (asantiago@us.ibm.com)
*/

using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data;


namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// Handles all Features Facet functionality. 
    /// </summary>
    public class Features : Facet
    {
        [SerializeField]
        private GameObject m_FeatureItemPrefab;
        [SerializeField]
        private RectTransform m_FeaturesCanvasRectTransform;
        [SerializeField]
        private int m_MaxFeatures = 8;

        private List<FeatureItem> m_FeatureItems = new List<FeatureItem>();
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

                while (m_FeatureItems.Count > 0)
                {
                    Destroy(m_FeatureItems[0].gameObject);
                    m_FeatureItems.RemoveAt(0);
                }

                if (m_AnswerData != null && m_AnswerData.HasAnswer()
                    && m_AnswerData.answers[0].features != null)
                {
                    for (int i = 0; i < m_AnswerData.answers[0].features.Length; i++)
                    {
                        if (i >= m_MaxFeatures)
                            break;

                        GameObject featureItemGameObject = Instantiate(m_FeatureItemPrefab, new Vector3(95f, -i * 50f - 150f, 0f), Quaternion.identity) as GameObject;
                        RectTransform featureItemRectTransform = featureItemGameObject.GetComponent<RectTransform>();
                        featureItemRectTransform.SetParent(m_FeaturesCanvasRectTransform, false);
                        FeatureItem featureItem = featureItemGameObject.GetComponent<FeatureItem>();
                        featureItem.FeatureString = m_AnswerData.answers[0].features[i].displayLabel;
                        featureItem.FeatureIndex = m_AnswerData.answers[0].features[i].weightedScore;

                        m_FeatureItems.Add(featureItem);
                    }
                }
            }
        }
    }
}
