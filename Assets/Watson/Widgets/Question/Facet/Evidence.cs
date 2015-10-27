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
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets.Question.Facet.FacetElement;

namespace IBM.Watson.Widgets.Question.Facet
{
	public class Evidence : Base
	{
		[SerializeField]
		private GameObject m_EvidenceItemPrefab;

		[SerializeField]
		private RectTransform m_EvidenceCanvasRectTransform;

		private List<EvidenceItem> m_EvidenceItems = new List<EvidenceItem>();

		/// <summary>
		/// Fired when Answer Data is set. Sets the evidence value in each evidence item. Limited to 3 evidence items.
		/// </summary>
		override protected void OnAnswerData()
		{
			for(int i = 0; i < Answers.answers[0].evidence.Length; i++)
			{
				if(i >= 3) return;

				GameObject evidenceItemGameObject = Instantiate(m_EvidenceItemPrefab, new Vector3(0f, -i*60f, 0f), Quaternion.identity) as GameObject;
				RectTransform evidenceItemRectTransform = evidenceItemGameObject.GetComponent<RectTransform>();
				evidenceItemRectTransform.SetParent(m_EvidenceCanvasRectTransform, false);
				EvidenceItem evidenceItem = evidenceItemGameObject.GetComponent<EvidenceItem>();
				m_EvidenceItems.Add(evidenceItem);
				evidenceItem.EvidenceString = Answers.answers[0].evidence[i].decoratedPassage;
			}
		}

		/// <summary>
		/// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
		/// </summary>
		override public void Clear()
		{
			while(m_EvidenceItems.Count != 0) {
				Destroy(m_EvidenceItems[0].gameObject);
				m_EvidenceItems.Remove(m_EvidenceItems[0]);
			}
		}
	}
}
