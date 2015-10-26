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
	public class Features : FacetBase
	{
		[Header("Features")]
		[SerializeField]
		private FeatureItem[] m_FeatureItems;

		public override void Init()
		{
			base.Init ();

			//	TODO instantiate FeatureItems from resources
			for (int i = 0; i < m_FeatureItems.Length; i++) {
				m_FeatureItems[i].m_Feature = m_QuestionWidget.Answers.answers[0].features[i].displayLabel;
				m_FeatureItems[i].m_FeatureIndex = m_QuestionWidget.Answers.answers[0].features[i].weightedScore;
			}
		}
	}
}
