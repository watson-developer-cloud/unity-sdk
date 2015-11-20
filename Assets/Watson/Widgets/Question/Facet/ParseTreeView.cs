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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Data.XRAY;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all ParseTree Facet functionality.
	/// </summary>
    public class ParseTreeView : Facet
    {
        [SerializeField]
        private GameObject m_ParseTreeTextItemPrefab;

        [SerializeField]
        private RectTransform m_ParseCanvasRectTransform;

		[SerializeField]
		private GameObject m_ParseTreeArrow;

        [SerializeField]
        private List<GameObject> m_POSList = new List<GameObject>();
        private List<ParseTreeTextItem> m_WordList = new List<ParseTreeTextItem>();
		private List<GameObject> m_ArrowList = new List<GameObject>();

        private ParseData m_ParseData = null;
        private Questions m_QuestionData = null;

		private float horizontalWordSpacing = 10f;
		private float verticalWordSpacing = 160f;

        private int m_WordIndex = 0;
        public int WordIndex
        {
            get { return m_WordIndex; }
            set
            {
                if (value > m_WordList.Count - 1)
                {
                    m_WordIndex = 0;
                }
                else if (value < 0)
                {
                    m_WordIndex = m_WordList.Count;
                }
                else
                {
                    m_WordIndex = value;
                }

                UpdateHighlightedWord();
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION_PARSE, OnParseData );
            EventManager.Instance.RegisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestionData );
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION_PARSE, OnParseData );
            EventManager.Instance.UnregisterEventReceiver( Constants.Event.ON_QUESTION, OnQuestionData );
        }

		/// <summary>
		/// Callback for Parse data event.
		/// </summary>
        private void OnParseData( object [] args )
        {
            if (Focused )
            {
                m_ParseData = args != null && args.Length > 0 ? args[0] as Data.XRAY.ParseData : null;
                GenerateParseTree();
            }
        }

		/// <summary>
		/// Callback for Question data event.
		/// </summary>
		/// <param name="args">Arguments.</param>
        private void OnQuestionData( object [] args )
        {
            if (Focused )
                m_QuestionData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Questions : null;
        }

        /// <summary>
        /// Generate parse tree from Parse Data.
        /// </summary>
        private void GenerateParseTree()
        {
            CancelInvoke();
            while (m_WordList.Count > 0)
            {
                Destroy(m_WordList[0].gameObject);
                m_WordList.RemoveAt( 0 );
            }

			while (m_ArrowList.Count > 0)
			{
				Destroy(m_ArrowList[0].gameObject);
				m_ArrowList.RemoveAt( 0 );
			}

			CreateParseWord(m_ParseData.parseTree, m_ParseCanvasRectTransform, m_ParseCanvasRectTransform);

			WordIndex = 0;
			InvokeRepeating("CycleWords", 2f, 2f);
        }

		/// <summary>
		/// Creates the parse word.
		/// </summary>
		/// <param name="parseWord">Parse word.</param>
		/// <param name="parentRectTransfrom">Parent rect transfrom.</param>
		/// <param name="parentWordRectTransform">Parent word rect transform.</param>
		private void CreateParseWord(ParseTree parseWord, RectTransform parentRectTransfrom, RectTransform parentWordRectTransform)
		{
			//	instantiate word
			int siblingCount = parentRectTransfrom.childCount;
			RectTransform lastSibling = siblingCount > 0 ? parentRectTransfrom.GetChild(siblingCount - 1).gameObject.GetComponent<RectTransform>() : null;

			float wordX = siblingCount > 0 ? lastSibling.sizeDelta.x + lastSibling.rect.width + horizontalWordSpacing : 0f;
			GameObject wordGameObject = Instantiate(m_ParseTreeTextItemPrefab, new Vector3(wordX, 0f, 0f), Quaternion.identity) as GameObject;

			//	set parent to parent transform
            RectTransform wordRectTransform = wordGameObject.GetComponent<RectTransform>();
			wordRectTransform.SetParent(parentRectTransfrom, false);

			//	set properties
			long wordPosition = parseWord.position;
			ParseTreeTextItem word = wordGameObject.GetComponent<ParseTreeTextItem>();
			word.ParseTreeWord = parseWord.text;
			word.Position = wordPosition;
			word.POS = GetPOS(wordPosition);
			word.Slot = GetSlot(wordPosition);
			word.m_Features = GetFeatures(wordPosition);

			//	add to word list
			m_WordList.Add(word);

			if(parseWord.leftChildren.Length > 0)
			{
				//	create and populate left children
				GameObject leftChild = new GameObject("Left Child");
				leftChild.AddComponent<RectTransform>();
				leftChild.AddComponent<CanvasRenderer>();
				RectTransform leftChildRectTransform = leftChild.GetComponent<RectTransform>();
				leftChildRectTransform.position = new Vector3(-200f, -verticalWordSpacing, 0f);
				leftChildRectTransform.SetParent(wordRectTransform, false);

				for(int i = 0; i < parseWord.leftChildren.Length; i++)
				{
					CreateParseWord(parseWord.leftChildren[i], leftChildRectTransform, wordRectTransform);
				}
			}

			if(parseWord.rightChildren.Length > 0)
			{
				//	create and populate right children
				GameObject rightChild = new GameObject("Right Child");
				rightChild.AddComponent<RectTransform>();
				rightChild.AddComponent<CanvasRenderer>();
				RectTransform rightChildRectTransform = rightChild.GetComponent<RectTransform>();
				rightChildRectTransform.position = new Vector3(200f, -verticalWordSpacing, 0f);
				rightChildRectTransform.SetParent(wordRectTransform, false);
				
				for(int k = 0; k < parseWord.rightChildren.Length; k++)
				{
					CreateParseWord(parseWord.rightChildren[k], rightChildRectTransform, wordRectTransform);
				}
			}

			if(parentRectTransfrom != m_ParseCanvasRectTransform)
				CreateArrow(parentWordRectTransform, wordRectTransform);
				//StartCoroutine(CreateArrow(parentWordRectTransform, wordRectTransform));
		}

		/// <summary>
		/// Gets the PO.
		/// </summary>
		/// <returns>The PO.</returns>
		/// <param name="position">Position.</param>
		private string GetPOS(long position)
		{
			return m_ParseData.Words[position].Pos.ToString();
		}

		/// <summary>
		/// Gets the slot.
		/// </summary>
		/// <returns>The slot.</returns>
		/// <param name="position">Position.</param>
		private string GetSlot(long position)
		{
			return m_ParseData.Words[position].Slot;
		}

		/// <summary>
		/// Gets the features.
		/// </summary>
		/// <returns>The features.</returns>
		/// <param name="position">Position.</param>
		private List<string> GetFeatures(long position)
		{
			List<string> features = new List<string>();

			for (int i = 0; i < m_ParseData.Words[position].Features.Length; i++)
            {
                features.Add(m_ParseData.Words[position].Features[i]);
            }

			return features;
		}

		/// <summary>
		/// Creates the arrow.
		/// </summary>
		/// <param name="parentRectTransform">Parent rect transform.</param>
		/// <param name="childRectTransform">Child rect transform.</param>
		private void CreateArrow(RectTransform parentRectTransform, RectTransform childRectTransform)
		{
			//	find two points
			Vector3 parentPoint = parentRectTransform.position;
			Vector3 childPoint = childRectTransform.position + (new Vector3(0f, childRectTransform.rect.height/2, 0f) * 0.01f);

			Vector3 direction = parentPoint - childPoint;
			direction = parentRectTransform.TransformDirection(direction);
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg - 90f;

			GameObject arrowGameObject = Instantiate(m_ParseTreeArrow, parentPoint, Quaternion.Euler(new Vector3(0f, 0f, angle))) as GameObject;
			RectTransform arrowRectTransform = arrowGameObject.GetComponent<RectTransform>();
			float dist = Vector3.Distance(parentPoint, childPoint);
			Vector2 tempSizedelta = arrowRectTransform.sizeDelta;
			tempSizedelta.x = dist * 160f;
			arrowRectTransform.sizeDelta = tempSizedelta;
			arrowRectTransform.SetParent(parentRectTransform, false);
		}

        /// <summary>
        /// Highlight words, POS and Slots based on the Word Index.
        /// </summary>
        private void UpdateHighlightedWord()
        {
            for (int i = 0; i < m_WordList.Count; i++)
            {
                m_WordList[i].IsHighlighted = false;
            }

            m_WordList[WordIndex].IsHighlighted = true;

            for (int j = 0; j < m_POSList.Count; j++)
            {
                POSControl posControl = m_POSList[j].GetComponent<POSControl>();
                if (posControl.POS == m_WordList[WordIndex].POS.ToLower() || posControl.POS == m_WordList[WordIndex].Slot.ToLower())
                {
                    posControl.IsHighlighted = true;
                }
                else
                {
                    posControl.IsHighlighted = false;
                }
            }

            if ( m_QuestionData != null && m_QuestionData.HasQuestion() )
            {
                if (m_QuestionData.questions[0].question.lat.Length == 0 
                    && m_QuestionData.questions[0].question.focus.Length == 0)
                {
                    m_POSList[2].GetComponent<POSControl>().IsHighlighted = true;
                }
                if (m_QuestionData.questions[0].question.lat.Length > 0)
                {
                    if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.lat[0].ToLower())
                    {
                        m_POSList[1].GetComponent<POSControl>().IsHighlighted = true;
                    }
                }


                if (m_QuestionData.questions[0].question.focus.Length > 0)
                {
                    if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.focus[0].ToLower())
                    {
                        m_POSList[0].GetComponent<POSControl>().IsHighlighted = true;
                    }
                }
            }
        }

        /// <summary>
        /// Cycles through words via WordIndex.
        /// </summary>
        private void CycleWords()
        {
            WordIndex++;
        }
    }
}
