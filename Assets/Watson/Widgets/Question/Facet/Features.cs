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
	public class Features : Base
	{
		[Header("Features")]
		[SerializeField]
		private FeatureItem[] m_FeatureItems;

		/// <summary>
		/// Fired when Answer Data is set. Sets the value of the Feature Item and index.
		/// </summary>
		override protected void OnAnswerData()
		{
			//	TODO Dynamically create feature items
			for (int i = 0; i < m_FeatureItems.Length; i++) {
				m_FeatureItems[i].m_Feature = m_Answers.answers[0].features[i].displayLabel;
				m_FeatureItems[i].m_FeatureIndex = m_Answers.answers[0].features[i].weightedScore;
			}
		}
	}
}
