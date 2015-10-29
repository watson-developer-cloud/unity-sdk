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

namespace IBM.Watson.Widgets.Question
{
    public class ParseTree : Base
    {
        [SerializeField]
        private GameObject m_ParseTreeTextItemPrefab;

        [SerializeField]
        private RectTransform m_ParseCanvasRectTransform;

        [SerializeField]
        private List<GameObject> m_POSList = new List<GameObject>();
        private List<ParseTreeTextItem> m_WordList = new List<ParseTreeTextItem>();
        private List<Vector3> m_PositionList = new List<Vector3>();

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

        /// <summary>
        /// Set hard coded positions.
        /// </summary>
        override protected void Start()
        {
            base.Start();
            //	TODO parse tree from hiearchy
            m_PositionList.Add(new Vector3(-583f, 188f, 0f));
            m_PositionList.Add(new Vector3(-408f, 64f, 0f));
            m_PositionList.Add(new Vector3(-184f, -49f, 0f));
            m_PositionList.Add(new Vector3(27f, -168f, 0f));
            m_PositionList.Add(new Vector3(259f, -301f, 0f));
            m_PositionList.Add(new Vector3(469f, -424f, 0f));
            m_PositionList.Add(new Vector3(-638f, -31f, 0f));
            m_PositionList.Add(new Vector3(-417f, -144f, 0f));
            m_PositionList.Add(new Vector3(-144f, -282f, 0f));
            m_PositionList.Add(new Vector3(109f, -397f, 0f));
            m_PositionList.Add(new Vector3(348f, -560f, 0f));
            m_PositionList.Add(new Vector3(-643f, -268f, 0f));
            m_PositionList.Add(new Vector3(-346f, -393f, 0f));
            m_PositionList.Add(new Vector3(-115f, -514f, 0f));
            m_PositionList.Add(new Vector3(91f, -641f, 0f));
        }

        /// <summary>
        /// Generates Parse tree on Initialization of data.
        /// </summary>
        override public void Init()
        {
            GenerateParseTree();
        }

        /// <summary>
        /// Generate parse tree from Parse Data.
        /// </summary>
        private void GenerateParseTree()
        {
            // TODO: Need to destroy any previous tree!

            for (int i = 0; i < m_Question.QuestionData.ParseDataObject.Words.Length; i++)
            {
                GameObject wordGameObject = Instantiate(m_ParseTreeTextItemPrefab) as GameObject;
                RectTransform wordRectTransform = wordGameObject.GetComponent<RectTransform>();
                wordRectTransform.SetParent(m_ParseCanvasRectTransform, false);
                if (i < m_PositionList.Count)
                {
                    wordRectTransform.localPosition = m_PositionList[i];
                }
                else
                {
                    //	TODO fix this
                    wordRectTransform.localPosition = new Vector3(5000f, 5000, 5000f);
                }
                ParseTreeTextItem word = wordGameObject.GetComponent<ParseTreeTextItem>();
                word.ParseTreeWord = m_Question.QuestionData.ParseDataObject.Words[i].Word;
                word.POS = m_Question.QuestionData.ParseDataObject.Words[i].Pos.ToString();
                word.Slot = m_Question.QuestionData.ParseDataObject.Words[i].Slot;

                for (int j = 0; j < m_Question.QuestionData.ParseDataObject.Words[i].Features.Length; j++)
                {
                    word.m_Features.Add(m_Question.QuestionData.ParseDataObject.Words[i].Features[j]);
                }

                m_WordList.Add(word);
            }

            WordIndex = 0;
            InvokeRepeating("CycleWords", 2f, 2f);
        }

        /// <summary>
        /// Clears dynamically generated Facet Elements when a question is answered. Called from Question Widget.
        /// </summary>
        override protected void Clear()
        {
            CancelInvoke();
            while (m_WordList.Count != 0)
            {
                Destroy(m_WordList[0].gameObject);
                m_WordList.Remove(m_WordList[0]);
            }
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

            if (m_Question.QuestionData.QuestionDataObject.questions[0].question.lat.Length == 0 && m_Question.QuestionData.QuestionDataObject.questions[0].question.focus.Length == 0) m_POSList[2].GetComponent<POSControl>().IsHighlighted = true;
            if (m_Question.QuestionData.QuestionDataObject.questions[0].question.lat.Length > 0)
            {
                if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_Question.QuestionData.QuestionDataObject.questions[0].question.lat[0].ToLower())
                {
                    m_POSList[1].GetComponent<POSControl>().IsHighlighted = true;
                }
            }


            if (m_Question.QuestionData.QuestionDataObject.questions[0].question.focus.Length > 0)
            {
                if (m_WordList[WordIndex].ParseTreeWord.ToLower() == m_Question.QuestionData.QuestionDataObject.questions[0].question.focus[0].ToLower())
                {
                    m_POSList[0].GetComponent<POSControl>().IsHighlighted = true;
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
