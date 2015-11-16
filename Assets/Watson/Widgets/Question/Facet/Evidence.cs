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
    public class Evidence : Base
    {
        [SerializeField]
        private GameObject m_EvidenceItemPrefab;

        [SerializeField]
        private RectTransform m_EvidenceCanvasRectTransform;

        private List<EvidenceItem> m_EvidenceItems = new List<EvidenceItem>();

		private Data.XRAY.Answers m_AnswerData = null;
		
		private void OnEnable()
		{
			EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}
		
		private void OnDisable()
		{
			EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_ANSWERS, OnAnswerData );
		}

        /// <summary>
        /// Dynamically creates up to three Evidence Items based on returned data.
        /// </summary>
		override public void Init()
        {
			base.Init ();

			if(m_AnswerData.answers[0].evidence.Length == 0) return;

			for (int i = 0; i < m_AnswerData.answers[0].evidence.Length; i++)
            {
                if (i >= 3) return;

                GameObject evidenceItemGameObject = Instantiate(m_EvidenceItemPrefab, new Vector3(0f, -i * 60f, 0f), Quaternion.identity) as GameObject;
                RectTransform evidenceItemRectTransform = evidenceItemGameObject.GetComponent<RectTransform>();
                evidenceItemRectTransform.SetParent(m_EvidenceCanvasRectTransform, false);
                EvidenceItem evidenceItem = evidenceItemGameObject.GetComponent<EvidenceItem>();
                m_EvidenceItems.Add(evidenceItem);
				evidenceItem.Answer = m_AnswerData.answers[0].answerText;
				evidenceItem.EvidenceString = m_AnswerData.answers[0].evidence[i].decoratedPassage;
            }
        }

        /// <summary>
        /// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
        /// </summary>
        override public void Clear()
        {
            while (m_EvidenceItems.Count != 0)
            {
                Destroy(m_EvidenceItems[0].gameObject);
                m_EvidenceItems.Remove(m_EvidenceItems[0]);
            }
        }

		/// <summary>
		/// Caallback for Answer data event.
		/// </summary>
		/// <param name="args">Arguments.</param>
		private void OnAnswerData( object [] args )
		{
			m_AnswerData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Answers : null;
			Init ();
		}
    }
}
