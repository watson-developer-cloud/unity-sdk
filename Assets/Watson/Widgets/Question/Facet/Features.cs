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
	public class Features : MonoBehaviour
	{
		private List<FeatureItem> FeatureItems = new List<FeatureItem>();
		private ObservedList<string> m_FeatureData = new ObservedList<string>();
		private QuestionWidget qWidget;

		[Header("Features")]
		[SerializeField]
		private FeatureItem[] m_FeatureItems;

		public void Init()
		{
			qWidget = gameObject.GetComponent<QuestionWidget>();

			//	TODO instantiate FeatureItems from resources
			for (int i = 0; i < m_FeatureItems.Length; i++) {
				m_FeatureItems[i].m_feature = qWidget.Answers.answers[0].features[i].displayLabel;
				m_FeatureItems[i].m_featureIndex = qWidget.Answers.answers[0].features[i].weightedScore;
			}
			m_FeatureData.Added += onAdd;
		}
		
		private void onAdd()
		{
			Debug.Log ("feature added");
			FeatureItem featureItem = new FeatureItem ();
			FeatureItems.Add (featureItem);
			featureItem.m_feature = m_FeatureData [m_FeatureData.Count];
		}
	}
}
