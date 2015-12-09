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
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.DataModels.XRAY;
using UnityEngine.UI;

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
		private ParseTreeTextItem[] m_WordListSorted;// = new List<ParseTreeTextItem>();
		private List<GameObject> m_ArrowList = new List<GameObject>();

        private ParseData m_ParseData = null;
        private Questions m_QuestionData = null;

		private float horizontalWordSpacing = 100f;
		private float verticalWordSpacing = 160f;

        private int m_WordIndex = 0;
        public int WordIndex
        {
            get { return m_WordIndex; }
            set
            {
				if (value > m_WordListSorted.Length - 1)
                {
                    m_WordIndex = 0;
                }
                else if (value < 0)
                {
					m_WordIndex = m_WordListSorted.Length;
                }
                else
                {
                    m_WordIndex = value;
                }

                UpdateHighlightedWord();
				PositionParseTree(WordIndex);
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

		void OnDestroy()
		{
			StopCoroutine(CycleWords());
		}	

		/// <summary>
		/// Callback for Parse data event.
		/// </summary>
        private void OnParseData( object [] args )
        {
            if (Focused )
            {
                m_ParseData = args != null && args.Length > 0 ? args[0] as DataModels.XRAY.ParseData : null;
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
                m_QuestionData = args != null && args.Length > 0 ? args[0] as DataModels.XRAY.Questions : null;
        }

        /// <summary>
        /// Generate parse tree from Parse Data.
        /// </summary>
        private void GenerateParseTree()
        {
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


			Log.Debug("ParseTreeView", "num words: " + m_ParseData.Words.Length);

			//	sort
			m_WordListSorted = new ParseTreeTextItem[m_WordList.Count];
			foreach(ParseTreeTextItem textItem in m_WordList)
			{
				m_WordListSorted[textItem.Position] = textItem;
			}

			WordIndex = 0;
			StartCoroutine(CycleWords());
        }

		/// <summary>
		/// Creates the parse word.
		/// </summary>
		/// <param name="parseWord">Parse word.</param>
		/// <param name="parentRectTransfrom">Parent rect transfrom.</param>
		/// <param name="parentWordRectTransform">Parent word rect transform.</param>
		private void CreateParseWord(ParseTree parseWord, RectTransform parentRectTransform, RectTransform parentWordRectTransform)
		{
			//	instantiate word
			GameObject wordGameObject = Instantiate(m_ParseTreeTextItemPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;

			//	set parent to parent transform
            RectTransform wordRectTransform = wordGameObject.GetComponent<RectTransform>();
			wordRectTransform.SetParent(parentRectTransform, false);

			//	set properties
			long wordPosition = parseWord.position;
			ParseTreeTextItem word = wordGameObject.GetComponent<ParseTreeTextItem>();
			word.ParseTreeWord = parseWord.text;
			word.Position = wordPosition;
			word.ParentNode = parseWord.parentNode;
			word.POS = GetPOS(wordPosition);
			word.Slot = GetSlot(wordPosition);
			word.m_Features = GetFeatures(wordPosition);
			//	add to word list at insert position
			int insertPosition = (int)word.Position < m_WordList.Count ? (int)word.Position : m_WordList.Count;
			m_WordList.Insert(insertPosition, word);

			//	Create right child
			if(parseWord.rightChildren.Length > 0)
			{
				//	create and populate right children
				GameObject rightChild = new GameObject("Right Child");
				RectTransform rightChildRectTransform = rightChild.AddComponent<RectTransform>();
				rightChild.AddComponent<CanvasRenderer>();
				rightChildRectTransform.position = new Vector3(200f, -verticalWordSpacing, 0f);
				rightChildRectTransform.SetParent(wordRectTransform, false);
				
				for(int k = parseWord.rightChildren.Length - 1; k >= 0; k--)
				{
					CreateParseWord(parseWord.rightChildren[k], rightChildRectTransform, wordRectTransform);
				}
			}

			//	create left child
			if(parseWord.leftChildren.Length > 0)
			{
				//	create and populate left children
				GameObject leftChild = new GameObject("Left Child");
				RectTransform leftChildRectTransform = leftChild.AddComponent<RectTransform>();
				leftChild.AddComponent<CanvasRenderer>();
				
				if(parseWord.rightChildren.Length > 0)
				{
					RectTransform rightChildRectTransform = wordRectTransform.FindChild("Right Child").GetComponent<RectTransform>();
					leftChildRectTransform.position = leftChild.GetComponent<RectTransform>().rect.x + leftChild.GetComponent<RectTransform>().rect.width > rightChildRectTransform.rect.x ? new Vector3(rightChildRectTransform.anchoredPosition.x - 400f, -verticalWordSpacing, 0f) : new Vector3(-200f, -verticalWordSpacing, 0f);
				}
				else
				{
					leftChildRectTransform.position = new Vector3(-200f, -verticalWordSpacing, 0f);
				}
				
				leftChildRectTransform.SetParent(wordRectTransform, false);
				
				for(int i = parseWord.leftChildren.Length - 1; i >= 0; i--)
				{
					CreateParseWord(parseWord.leftChildren[i], leftChildRectTransform, wordRectTransform);
				}
			}
			
			if(parentRectTransform != m_ParseCanvasRectTransform)
				CreateArrow(parentWordRectTransform.GetComponentInChildren<Text>().gameObject.GetComponent<RectTransform>(), wordGameObject.transform.GetComponentInChildren<Text>().gameObject.GetComponent<RectTransform>());
			StartCoroutine(PositionWord(wordRectTransform, parentRectTransform));
		}

		/// <summary>
		/// Positions the words after they are populated based on if the parent is the left or right child.
		/// </summary>
		/// <returns>The word.</returns>
		/// <param name="wordRectTransform">Word rect transform.</param>
		/// <param name="parentRectTransform">Parent rect transform.</param>
		private IEnumerator PositionWord(RectTransform wordRectTransform, RectTransform parentRectTransform)
		{
			yield return new WaitForSeconds(0.1f);

			//	Declare word x
			float wordX = 0f;

			//	If it is the first word in the gameObject do not reposition
			if(wordRectTransform.GetSiblingIndex() == 0)
				yield break;

			//	Declare the last sibling's Text RectTransform
			RectTransform lastSiblingWordRectTransform = parentRectTransform.GetChild(wordRectTransform.GetSiblingIndex() - 1).FindChild("ParseTreeText").GetComponent<RectTransform>();
			RectTransform wordTextRectTransform = wordRectTransform.FindChild("ParseTreeText").GetComponent<RectTransform>();

			//	Expand word placment for Left Children to the left and Right  children to the Right
			if(parentRectTransform.gameObject.name == "Left Child")
			{
				wordX = lastSiblingWordRectTransform.anchoredPosition.x - wordTextRectTransform.sizeDelta.x - lastSiblingWordRectTransform.sizeDelta.x/2 - horizontalWordSpacing;
			}
			else if(parentRectTransform.gameObject.name == "Right Child")
			{
				wordX = lastSiblingWordRectTransform.anchoredPosition.x + lastSiblingWordRectTransform.sizeDelta.x + lastSiblingWordRectTransform.sizeDelta.x/2 + horizontalWordSpacing;
			}

			wordRectTransform.gameObject.transform.localPosition = new Vector3(wordX, 0f, 0f);
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
			GameObject arrowGameObject = Instantiate(m_ParseTreeArrow) as GameObject;
			RectTransform arrowRectTransform = arrowGameObject.GetComponent<RectTransform>();
			ParseTreeArrow parseTreeArrowScript = arrowGameObject.GetComponent<ParseTreeArrow>();
			parseTreeArrowScript.ParentRectTransform = parentRectTransform;
			parseTreeArrowScript.ChildRectTransform = childRectTransform;

			arrowRectTransform.SetParent(parentRectTransform.parent.transform, false);
		}

        /// <summary>
        /// Highlight words, POS and Slots based on the Word Index.
        /// </summary>
        private void UpdateHighlightedWord()
        {
			for (int i = 0; i < m_WordListSorted.Length; i++)
            {
				if(m_WordListSorted[i].IsHighlighted) m_WordListSorted[i].IsHighlighted = false;
            }

			m_WordListSorted[WordIndex].IsHighlighted = true;

            for (int j = 0; j < m_POSList.Count; j++)
            {
                POSControl posControl = m_POSList[j].GetComponent<POSControl>();
				if (posControl.POS == m_WordListSorted[WordIndex].POS.ToLower() || posControl.POS == m_WordListSorted[WordIndex].Slot.ToLower())
                {
					if(!posControl.IsHighlighted) posControl.IsHighlighted = true;
                }
                else
                {
					if(posControl.IsHighlighted) posControl.IsHighlighted = false;
                }
            }

            if ( m_QuestionData != null && m_QuestionData.HasQuestion() )
            {
                if (m_QuestionData.questions[0].question.lat.Length == 0 
                    && m_QuestionData.questions[0].question.focus.Length == 0)
                {
					if(!m_POSList[2].GetComponent<POSControl>().IsHighlighted) m_POSList[2].GetComponent<POSControl>().IsHighlighted = true;
                }
                if (m_QuestionData.questions[0].question.lat.Length > 0)
                {
					if (m_WordListSorted[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.lat[0].ToLower())
                    {
						if(!m_POSList[1].GetComponent<POSControl>().IsHighlighted) m_POSList[1].GetComponent<POSControl>().IsHighlighted = true;
                    }
                }


                if (m_QuestionData.questions[0].question.focus.Length > 0)
                {
					if (m_WordListSorted[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.focus[0].ToLower())
                    {
						if(!m_POSList[0].GetComponent<POSControl>().IsHighlighted) m_POSList[0].GetComponent<POSControl>().IsHighlighted = true;
                    }
                }
            }
        }

        /// <summary>
        /// Cycles through words via WordIndex.
        /// </summary>
        private IEnumerator CycleWords()
        {
			while(true)
			{
				yield return new WaitForSeconds(2f);
		        WordIndex++;
			}
        }

		private void PositionParseTree(int index)
		{
			Vector2 relativeWordPosition = GetPositionInCanvasSpace(m_WordListSorted[index].GetComponent<RectTransform>());
			LeanTween.move(m_ParseCanvasRectTransform, m_ParseCanvasRectTransform.anchoredPosition - relativeWordPosition, 1.75f).setEase( LeanTweenType.easeOutQuad );
		}

		private Vector2 GetPositionInCanvasSpace(RectTransform rectTransform)
		{
			Vector2 resultPoint = Vector2.zero;
			RectTransform[] rectTransformArray = rectTransform.GetComponentsInParent<RectTransform>();
			
			foreach(RectTransform parentGORectTransform in rectTransformArray)
			{
				resultPoint += parentGORectTransform.anchoredPosition;
			}

			//	remove mask transform
			resultPoint -= new Vector2(-52f, -278f);

			return resultPoint;
		}
    }
}
