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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using IBM.Watson.Logging;
using IBM.Watson.Data;
using IBM.Watson.Widgets.Question;
using UnityEngine;
using System.Collections.Generic;
using IBM.Watson.Utilities;

namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// This class manages the answers, question, and other data related to a question asked of the AvatarWidget.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class QuestionWidget : Widget
    {
        #region Private Data
        private CubeAnimationManager m_CubeAnimMgr = null;

        private AnswersAndConfidence m_AnswersAndConfidence;
        private Question.Evidence m_Evidence;
        private Semantic m_Semantic;
        private Features m_Features;
        private Location m_Location;
        private ParseTree m_ParseTree;
        private QuestionAndAnswer m_QuestionAndAnswer;
		private Passages m_Passages;
        private List<Base> m_Facets = new List<Base>();

        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "Question";
        }
        #endregion

        #region Public Properties

        private bool m_Focused = false;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IBM.Watson.Widgets.Question.QuestionWidget"/> is focused.
        /// </summary>
        /// <value><c>true</c> if focused; otherwise, <c>false</c>.</value>
        public bool Focused
        {
            get
            {
                return m_Focused;
            }
            set
            {
                m_Focused = value;

                //if (value)
                //{
                //    KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CUBE_TO_FOLD, Constants.KeyCodes.MODIFIER_KEY, OnFold);
                //    KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CUBE_TO_FOCUS, Constants.KeyCodes.MODIFIER_KEY, OnFocusNext);
                //    KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CUBE_TO_ROTATE_OR_PAUSE, Constants.KeyCodes.MODIFIER_KEY, OnRotateOrPause);
                //    KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CUBE_TO_UNFOCUS, Constants.KeyCodes.MODIFIER_KEY, OnUnFocus);
                //    KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CUBE_TO_UNFOLD, Constants.KeyCodes.MODIFIER_KEY, OnUnfold);
                //}
                //else
                //{
                //    KeyEventManager.Instance.UnregisterKeyEvent(Constants.KeyCodes.CUBE_TO_FOLD, Constants.KeyCodes.MODIFIER_KEY, OnFold);
                //    KeyEventManager.Instance.UnregisterKeyEvent(Constants.KeyCodes.CUBE_TO_FOCUS, Constants.KeyCodes.MODIFIER_KEY, OnFocusNext);
                //    KeyEventManager.Instance.UnregisterKeyEvent(Constants.KeyCodes.CUBE_TO_ROTATE_OR_PAUSE, Constants.KeyCodes.MODIFIER_KEY, OnRotateOrPause);
                //    KeyEventManager.Instance.UnregisterKeyEvent(Constants.KeyCodes.CUBE_TO_UNFOCUS, Constants.KeyCodes.MODIFIER_KEY, OnUnFocus);
                //    KeyEventManager.Instance.UnregisterKeyEvent(Constants.KeyCodes.CUBE_TO_UNFOLD, Constants.KeyCodes.MODIFIER_KEY, OnUnfold);
                //}
            }
        }

        public CubeAnimationManager Cube
        {
            get
            {
                if (m_CubeAnimMgr == null)
                    m_CubeAnimMgr = GetComponentInChildren<CubeAnimationManager>();

                if (m_CubeAnimMgr == null)
                    Log.Error("QuestionWidget", "CubeAnimationManager is not found under Question widget");

                return m_CubeAnimMgr;
            }
        }

        public IQuestionData QuestionData { get; set; }

        #endregion

        #region Cube Actions
        public void OnDisplayAnswers(ClassifyResult result)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.ANSWERS);
        }

        public void OnDisplayChat(ClassifyResult result)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.CHAT);
        }

        public void OnDisplayParse(ClassifyResult result)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.PARSE);
        }

        public void OnDisplayEvidence(ClassifyResult result)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.EVIDENCE);
        }

        public void OnDisplayLocation(ClassifyResult result)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.LOCATION);
        }

        public void OnFold(ClassifyResult result)
        {
            Cube.Fold();
        }
        public void OnUnfold(ClassifyResult result)
        {
            Cube.UnFold();
        }
        public void OnRotateOrPause()
        {
            Cube.RotateOrPause();
        }

        public void OnFocus(CubeAnimationManager.CubeSideType sideType)
        {
            Cube.FocusOnSide(sideType);
        }
        public void OnFocusNext()
        {
            Cube.FocusOnNextSide();
        }
        public void OnUnFocus()
        {
            Cube.UnFocus();
        }
        public void OnLeaveTheSceneAndDestroy()
        {
            Cube.LeaveTheSceneAndDestroy();
        }
        #endregion

        /// <summary>
        /// Register events, set facet references, add facets to a List.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_AnswersAndConfidence = gameObject.GetComponent<AnswersAndConfidence>();
            m_Evidence = gameObject.GetComponent<Question.Evidence>();
            m_Semantic = gameObject.GetComponent<Semantic>();
            m_Features = gameObject.GetComponent<Features>();
            m_Location = gameObject.GetComponent<Location>();
            m_ParseTree = gameObject.GetComponent<ParseTree>();
            m_QuestionAndAnswer = gameObject.GetComponent<QuestionAndAnswer>();
			m_Passages = gameObject.GetComponent<Passages>();

            m_Facets.Add(m_AnswersAndConfidence);
            m_Facets.Add(m_Evidence);
            m_Facets.Add(m_Semantic);
            m_Facets.Add(m_Features);
            m_Facets.Add(m_Location);
            m_Facets.Add(m_ParseTree);
            m_Facets.Add(m_QuestionAndAnswer);
			m_Facets.Add(m_Passages);
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Sets Question, Answer and Avatar for each facet. This is called by the Avatar Widget.
        /// </summary>
        public void UpdateFacets()
        {
            foreach (Base facet in m_Facets)
                facet.Init();
        }
    }

    public delegate void OnMessage(string msg);

    public interface IQuestionData
    {
        Questions QuestionDataObject { get; }
        Answers AnswerDataObject { get; }
        ParseData ParseDataObject { get; }
        string Location { get; }
        OnMessage OnQuestionEvent { get; set; }
        OnMessage OnAnswerEvent { get; set; }
    }
}
