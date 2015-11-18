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
using IBM.Watson.Data.XRAY;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Handles all ParseTree Facet functionality.
	/// </summary>
    public class ParseTreeView : Base
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
//        private List<Vector3> m_PositionList = new List<Vector3>();

        private Data.XRAY.ParseData m_ParseData = null;
        private Data.XRAY.Questions m_QuestionData = null;

		private float horizontalWordSpacing = 20f;
		private float verticalWordSpacing = 150f;

//        private int m_WordIndex = 0;
//        public int WordIndex
//        {
//            get { return m_WordIndex; }
//            set
//            {
//                if (value > m_WordList.Count - 1)
//                {
//                    m_WordIndex = 0;
//                }
//                else if (value < 0)
//                {
//                    m_WordIndex = m_WordList.Count;
//                }
//                else
//                {
//                    m_WordIndex = value;
//                }
//
//                UpdateHighlightedWord();
//            }
//        }

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
            m_ParseData = args != null && args.Length > 0 ? args[0] as Data.XRAY.ParseData : null;
            GenerateParseTree();
        }

		/// <summary>
		/// Callback for Question data event.
		/// </summary>
		/// <param name="args">Arguments.</param>
        private void OnQuestionData( object [] args )
        {
            m_QuestionData = args != null && args.Length > 0 ? args[0] as Data.XRAY.Questions : null;
        }

        /// <summary>
        /// Generate parse tree from Parse Data.
        /// </summary>
        private void GenerateParseTree()
        {
//            CancelInvoke();
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

//            if ( m_ParseData != null )
//            {
//                for (int i = 0; i < m_ParseData.Words.Length; i++)
//                {
//                    GameObject wordGameObject = Instantiate(m_ParseTreeTextItemPrefab) as GameObject;
//                    RectTransform wordRectTransform = wordGameObject.GetComponent<RectTransform>();
//                    wordRectTransform.SetParent(m_ParseCanvasRectTransform, false);
//
//                    if (i < m_PositionList.Count)
//                    {
//                        wordRectTransform.localPosition = m_PositionList[i];
//                    }
//                    else
//                    {
//                        //	TODO fix this
//                        wordRectTransform.localPosition = new Vector3(5000f, 5000, 0f);
//                    }
//                    ParseTreeTextItem word = wordGameObject.GetComponent<ParseTreeTextItem>();
//                    word.ParseTreeWord = m_ParseData.Words[i].Word;
//                    word.POS = m_ParseData.Words[i].Pos.ToString();
//                    word.Slot = m_ParseData.Words[i].Slot;
//
//                    for (int j = 0; j < m_ParseData.Words[i].Features.Length; j++)
//                    {
//                        word.m_Features.Add(m_ParseData.Words[i].Features[j]);
//                    }
//
//                    m_WordList.Add(word);
//                }
//
//                WordIndex = 0;
//                InvokeRepeating("CycleWords", 2f, 2f);
//            }

			CreateParseWord(m_ParseData.parseTree, m_ParseCanvasRectTransform, 0);
        }

		private void CreateParseWord(ParseTree parseWord, RectTransform parentRectTransfrom, int childWordIndex)
		{
//			float wordX = Mathf.Cos(30f + 90f) * 200f + 100f * childWordIndex;// 200 is length of arrow
//			float wordY = -Mathf.Sin(30f + 90f) * 200f;
//			RectTransform parentTextItemRectTransform = parentRectTransfrom.gameObject.GetComponentInParent<RectTransform>();

			//	set text position based on parent and child index
//			Vector3 textItemPosition = parentRectTransfrom == m_ParseCanvasRectTransform ? Vector3.zero : new Vector3(0f, 0f, 0f);

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
			word.m_ChildWordIndex = childWordIndex;

//			wordRectTransform.position = new Vector3(horizontalWordSpacing * childWordIndex, 0f, 0f);

			//	add to word list
			m_WordList.Add(word);



			/*
			//	create gameobject to hold child words
			GameObject childrenGameObject = new GameObject("Children");

			//	add UI Components
			childrenGameObject.AddComponent<RectTransform>();
			childrenGameObject.AddComponent<CanvasRenderer>();

			RectTransform childrenGameObjectRectTransform = childrenGameObject.GetComponent<RectTransform>();
			//	set position
			childrenGameObjectRectTransform.position = new Vector3(0f, -verticalWordSpacing, 0f);
			//	set parent to word
			childrenGameObjectRectTransform.SetParent(wordRectTransform, false);


			//	set child node index
			int childNodeIndex = 0;

			//	iterate through left and right children and create parse words
			for(int i = 0; i < parseWord.leftChild.Length; i++)
			{
				CreateParseWord(parseWord.leftChild[i], childrenGameObjectRectTransform, childNodeIndex);
				childNodeIndex ++;
			}

			for(int k = 0; k < parseWord.rightChild.Length; k++)
			{
				CreateParseWord(parseWord.rightChild[k], childrenGameObjectRectTransform, childNodeIndex);
				childNodeIndex ++;
			}
			*/

//			if(parentRectTransfrom != m_ParseCanvasRectTransform)
//				CreateArrow(parentRectTransfrom.gameObject, wordGameObject);

			GameObject leftChild = new GameObject("Left Child");
			leftChild.AddComponent<RectTransform>();
			leftChild.AddComponent<CanvasRenderer>();
			RectTransform leftChildRectTransform = leftChild.GetComponent<RectTransform>();
			leftChildRectTransform.position = new Vector3(-200f, -verticalWordSpacing, 0f);
			leftChildRectTransform.SetParent(wordRectTransform, false);
			int leftChildWordIndex = 0;

			for(int i = 0; i < parseWord.leftChild.Length; i++)
			{
				CreateParseWord(parseWord.leftChild[i], leftChildRectTransform, leftChildWordIndex);
				leftChildWordIndex ++;
			}

			GameObject rightChild = new GameObject("Right Child");
			rightChild.AddComponent<RectTransform>();
			rightChild.AddComponent<CanvasRenderer>();
			RectTransform rightChildRectTransform = rightChild.GetComponent<RectTransform>();
			rightChildRectTransform.position = new Vector3(200f, -verticalWordSpacing, 0f);
			rightChildRectTransform.SetParent(wordRectTransform, false);
			int rightChildWordIndex = 0;
			
			for(int k = 0; k < parseWord.rightChild.Length; k++)
			{
				CreateParseWord(parseWord.rightChild[k], rightChildRectTransform, rightChildWordIndex);
				rightChildWordIndex ++;
			}


		}

		private string GetPOS(long position)
		{
			return m_ParseData.Words[position].Pos.ToString();
		}

		private string GetSlot(long position)
		{
			return m_ParseData.Words[position].Slot;
		}

		private List<string> GetFeatures(long position)
		{
			List<string> features = new List<string>();

			for (int i = 0; i < m_ParseData.Words[position].Features.Length; i++)
            {
                features.Add(m_ParseData.Words[position].Features[i]);
            }

			return features;
		}

		private void CreateArrow(GameObject parentGameObject, GameObject childGameObject)
		{
			Vector3 direction = parentGameObject.transform.position - childGameObject.transform.position;
			direction = parentGameObject.transform.InverseTransformDirection(direction);
			float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

			GameObject arrowGameObject = Instantiate(m_ParseTreeArrow, parentGameObject.transform.position, Quaternion.Euler(new Vector3(0f, 0f, angle))) as GameObject;
			RectTransform arrowRectTransform = arrowGameObject.GetComponent<RectTransform>();

//			arrowRectTransform
			arrowRectTransform.SetParent(parentGameObject.GetComponent<RectTransform>(), false);
		}

        /// <summary>
        /// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
        /// </summary>
//        override public void Clear()
//        {
//        }

        /// <summary>
        /// Highlight words, POS and Slots based on the Word Index.
        /// </summary>
//        private void UpdateHighlightedWord()
//        {
//            for (int i = 0; i < m_WordList.Count; i++)
//            {
//                m_WordList[i].IsHighlighted = false;
//            }
//
//            m_WordList[WordIndex].IsHighlighted = true;
//
//            for (int j = 0; j < m_POSList.Count; j++)
//            {
//                POSControl posControl = m_POSList[j].GetComponent<POSControl>();
//                if (posControl.POS == m_WordList[WordIndex].POS.ToLower() || posControl.POS == m_WordList[WordIndex].Slot.ToLower())
//                {
//                    posControl.IsHighlighted = true;
//                }
//                else
//                {
//                    posControl.IsHighlighted = false;
//                }
//            }
//
//            if ( m_QuestionData != null && m_QuestionData.HasQuestion() )
//            {
//                if (m_QuestionData.questions[0].question.lat.Length == 0 
//                    && m_QuestionData.questions[0].question.focus.Length == 0)
//                {
//                    m_POSList[2].GetComponent<POSControl>().IsHighlighted = true;
//                }
//                if (m_QuestionData.questions[0].question.lat.Length > 0)
//                {
//                    if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.lat[0].ToLower())
//                    {
//                        m_POSList[1].GetComponent<POSControl>().IsHighlighted = true;
//                    }
//                }
//
//
//                if (m_QuestionData.questions[0].question.focus.Length > 0)
//                {
//                    if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_QuestionData.questions[0].question.focus[0].ToLower())
//                    {
//                        m_POSList[0].GetComponent<POSControl>().IsHighlighted = true;
//                    }
//                }
//            }
//        }

        /// <summary>
        /// Cycles through words via WordIndex.
        /// </summary>
//        private void CycleWords()
//        {
//            WordIndex++;
//        }
    }
}
