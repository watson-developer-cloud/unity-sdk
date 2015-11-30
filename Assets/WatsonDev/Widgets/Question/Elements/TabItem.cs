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
using UnityEngine.UI;
using System.Collections;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls TabItem view.
	/// </summary>
	public class TabItem : MonoBehaviour {
		/// <summary>
		/// The m_ confidence text.
		/// </summary>
		[SerializeField]
		private Text m_ConfidenceText;


		private double m_Confidence;
		public double Confidence
		{
			get { return m_Confidence; }
			set
			{
				m_Confidence = value;
				UpdateConfidence();
			}
		}

		/// <summary>
		/// Update the confidence view.
		/// </summary>
		private void UpdateConfidence()
		{
			float confidence = (float)Confidence * 100;
			m_ConfidenceText.text = confidence.ToString("f1");
		}
	}
}