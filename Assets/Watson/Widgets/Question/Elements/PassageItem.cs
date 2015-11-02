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
using UnityEngine.UI;
using System.Collections;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls PassageItem view.
	/// </summary>
	public class PassageItem : MonoBehaviour {
		private float m_MaxYPos = 480f;
		private float m_MinYPos = -250f;
		private float m_TabPosX = -160f;
		private float m_TabPosY;

		[SerializeField]
		private Text m_PassageText;

		[SerializeField]
		private TabItem m_TabItem;

		private string m_PassageString;
		public string PassageString
		{
			get { return m_PassageString; }
			set
			{
				m_PassageString = value;
				UpdatePassage();
			}
		}

		private double m_Confidence = 0f;
		public double Confidence
		{
			get { return m_Confidence; }
			set
			{
				m_Confidence = value;
				UpdateConfidence();
			}
		}

		private double m_MaxConfidence = 1f;
		public double MaxConfidence
		{
			get { return m_MaxConfidence; }
			set
			{
				m_MaxConfidence = value;
//				UpdateConfidence();
			}
		}
		
		private double m_MinConfidence = 0f;
		public double MinConfidence
		{
			get { return m_MinConfidence; }
			set
			{
				m_MinConfidence = value;
//				UpdateConfidence();
			}
		}

		/// <summary>
		/// Update the passage view.
		/// </summary>
		private void UpdatePassage()
		{
			//	TODO format passage
			m_PassageText.text = PassageString;
		}

		/// <summary>
		/// Update the confidence tab view. Move Y position of TabItem according to normalized confidence.
		/// </summary>
		private void UpdateConfidence()
		{
			m_TabItem.Confidence = Confidence;

			double NormalizedConfidence = Confidence - MinConfidence;
			double ConfidenceRange = MaxConfidence - MinConfidence;
			float ConfidencePercentage = (float)NormalizedConfidence/(float)ConfidenceRange;

//			m_TabPosY = ((m_MaxYPos - m_MinYPos) * ConfidencePercentage) + m_MinYPos;
			m_TabPosY = ((m_MaxYPos - m_MinYPos) * ConfidencePercentage) + m_MinYPos;
			RectTransform TabItemRectTransform = m_TabItem.GetComponent<RectTransform>();
			TabItemRectTransform.anchoredPosition = new Vector2(m_TabPosX, m_TabPosY);
		}
	}
}