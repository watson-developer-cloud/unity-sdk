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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls EvidenceItem views. Attached to EvidenceItem prefab.
	/// </summary>
    public class EvidenceItem : MonoBehaviour
    {
        [SerializeField]
        private Text m_EvidenceText;

		[SerializeField]
		private GameObject m_BoundingBox;
		
		private float m_horizontalPadding = 15f;
		private float m_verticalPadding = 15f;
		private List<GameObject> boundingBoxes = new List<GameObject>();

        private string m_EvidenceString;
        public string EvidenceString
        {
            get { return m_EvidenceString; }
            set
            {
                m_EvidenceString = value;
                UpdateEvidence();
            }
        }

		private string m_Answer;
		public string Answer
		{ 
			get { return m_Answer; }
			set { m_Answer = value; }
		}

        /// <summary>
        /// Update the evidence view.
        /// </summary>
        private void UpdateEvidence()
        {
            //	TODO draw box around answer
            if (EvidenceString != "")
            {
                gameObject.SetActive(true);
//				string StrippedEvidence = Regex.Replace(EvidenceString, "<[^>]*>", "");
				m_EvidenceText.text = EvidenceString;
				HighlightAnswer(m_EvidenceText, Answer);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

		/// <summary>
		/// Highlights the answer in a TextField.
		/// </summary>
		/// <param name="tf">UIText component.</param>
		/// <param name="answer">Answer string.</param>
		private void HighlightAnswer(Text tf, string answer)
		{
			//	strip out tag
//			tf.text = Regex.Replace(tf.text, "<[^>]*>", "");
			tf.text = Utilities.Utility.RemoveTags(tf.text);
			TextGenerator textGen = tf.cachedTextGenerator;
			StartCoroutine(PopulateTextGen(tf, textGen, answer, 1f));
		}
		
		/// <summary>
		/// Gets the answer indexes in supplied evidenceString.
		/// </summary>
		/// <returns>The answer indexes in a List.</returns>
		/// <param name="evidenceString">Evidence string.</param>
		/// <param name="answer">Answer string.</param>
		public List<int> GetAnswerIndexes(string evidenceString, string answer)
		{
			if (string.IsNullOrEmpty(answer))
				throw new WatsonException("need string");
			
			List<int> ind = new List<int>();
			
			for (int i = 0;; i += answer.Length)
			{
				i = evidenceString.ToLower().IndexOf(answer.ToLower(), i);
				if (i == -1)
					return ind;
				ind.Add(i);
			}
		}

		/// <summary>
		/// Delayed outline because of rect size.
		/// </summary>
		/// <returns>The text gen.</returns>
		/// <param name="tf">Tf.</param>
		/// <param name="textGen">Text gen.</param>
		/// <param name="answer">Answer.</param>
		/// <param name="delayTime">Delay time.</param>
		private IEnumerator PopulateTextGen(Text tf, TextGenerator textGen, string answer, float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			//	populate textGen

			textGen.Populate(tf.text, tf.GetGenerationSettings(tf.GetComponent<RectTransform>().rect.size));
//			Log.Debug("EvidenceItem","rect size: " + tf.GetComponent<RectTransform>().rect.size);

			//	get index of all occurences of answer
			List<int> answerList = new List<int>();
			answerList = GetAnswerIndexes(tf.text, answer);
			
			if(answerList.Count == 0) yield return null;
			
			for(int i = 0; i < answerList.Count; i++)
			{
				Vector3 topLeft = textGen.verts[answerList[i] * 4].position;
				Vector3 bottomRight = textGen.verts[((answerList[i] + answer.Length - 1) * 4) + 2].position;

				
				//	create bounding box at answer location
				GameObject boundingBox = Instantiate(m_BoundingBox, 
				                                     new Vector2(topLeft.x - m_horizontalPadding, topLeft.y + m_verticalPadding), 
				                                     Quaternion.identity) as GameObject;
				
				boundingBoxes.Add(boundingBox);
				
				RectTransform boundingBoxRectTransform = boundingBox.GetComponent<RectTransform>();
				boundingBoxRectTransform.SetParent(tf.gameObject.transform, false);
				
				float rectWidth = (bottomRight.x - topLeft.x) + m_horizontalPadding * 2;
				float rectHeight = (-bottomRight.y + topLeft.y) + m_verticalPadding * 2;
			
				boundingBoxRectTransform.sizeDelta = new Vector2(rectWidth, rectHeight);
			}
		}
    }
}
