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
                EnableEvents(value);
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



        /// <summary>
        /// Method called on Tapping on Question Widget 
        /// </summary>
        /// <param name="tapGesture">Tap Gesture with all touch information</param>
        /// <param name="hitTransform">Hit Tranform of tap</param>
		public void OnTapInside(object [] args)
        {
			if (args != null && args.Length == 2 && args [0] is TouchScript.Gestures.TapGesture && args [1] is Transform) 
			{
				Log.Status("Question Widget", "OnTapInside");
				//TouchScript.Gestures.TapGesture tapGesture = args [0] as TouchScript.Gestures.TapGesture; 
				Transform hitTransform = args [1] as Transform;

				//Touch on side
				switch (CubeAnimationManager.Instance.AnimationState) {
				case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
					break;
				case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
					Cube.UnFold ();
					break;
				case CubeAnimationManager.CubeAnimationState.UNFOLDING:
					FocusOnSide (hitTransform);
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
					FocusOnSide (hitTransform);
					break;
				case CubeAnimationManager.CubeAnimationState.FOLDING:
					Cube.UnFold ();
					break;
				case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
					FocusOnSide (hitTransform);
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
					FocusOnSide (hitTransform);
					break;
				case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
					break;
				default:
					break;
				}
			} else 
			{
				Log.Warning("Question Widget", "OnTapInside has invalid arguments!");
			}
           
        }

        private void FocusOnSide(Transform hitTransform)
        {
            int touchedSide = 0;
            int.TryParse(hitTransform.name.Substring(1, 1), out touchedSide);
            Cube.FocusOnSide((CubeAnimationManager.CubeSideType)touchedSide);
        }

        /// <summary>
        /// Method called on Tapping outside of the Question Widget 
        /// </summary>
        /// <param name="tapGesture">Tap Gesture with all touch information</param>
        /// <param name="hitTransform">Hit Tranform of tap</param>
		public void OnTapOutside(object [] args)
        {
			if (args != null && args.Length == 2 && args [0] is TouchScript.Gestures.TapGesture && args [1] is Transform) 
			{
				Log.Status("Question Widget", "OnTapOutside");
				//TouchScript.Gestures.TapGesture tapGesture = args [0] as TouchScript.Gestures.TapGesture; 
				//Transform hitTransform = args [1] as Transform;

				//Touch out-side
				switch (CubeAnimationManager.Instance.AnimationState)
				{
				case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
					break;
				case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
					break;
				case CubeAnimationManager.CubeAnimationState.UNFOLDING:
					Cube.Fold();
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
					Cube.Fold();
					break;
				case CubeAnimationManager.CubeAnimationState.FOLDING:
					break;
				case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
					Cube.UnFocus();
					break;
				case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
					Cube.UnFocus();
					break;
				case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
					break;
				default:
					break;
				}
			}
			else
			{
				Log.Warning("Question Widget", "OnTapOutside has invalid arguments!");
			}
            
        }

		public void DragOneFinger(object [] args)
        {
			if (args != null && args.Length == 1 && args [0] is TouchScript.Gestures.ScreenTransformGesture ) 
			{
				TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

				if (Cube != null)
				{
					Cube.DragOneFinger(OneFingerManipulationGesture);
				}
			}
            
        }

        public void OnDisplayAnswers(object [] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.ANSWERS);
        }

        public void OnDisplayChat(object [] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.CHAT);
        }

        public void OnDisplayParse(object [] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.PARSE);
        }

        public void OnDisplayEvidence(object [] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.EVIDENCE);
        }

        public void OnDisplayLocation(object [] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.LOCATION);
        }

        public void OnFold(object [] args)
        {
            Cube.Fold();
        }
        public void OnUnfold(object [] args)
        {
            Cube.UnFold();
        }

        public void OnRotateOrPause(object [] args)
        {
            Cube.RotateOrPause();
        }

        public void OnFocus(object [] args)
        {
            if ( args != null && args.Length > 0 && args[0] is CubeAnimationManager.CubeSideType )
                Cube.FocusOnSide( (CubeAnimationManager.CubeSideType)args[0] );
        }
        public void OnFocusNext(object [] args)
        {
            Cube.FocusOnNextSide();
        }
        public void OnUnFocus(object [] args)
        {
            Cube.UnFocus();
        }
        public void OnLeaveTheSceneAndDestroy(object [] args = null )
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
            EnableEvents(false);

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

        private void EnableEvents(bool enable)
        {
            EventWidget eventWidget = GetComponentInChildren<EventWidget>();
            if ( eventWidget != null )
                eventWidget.enabled = enable;

			TouchWidget touchWidget = GetComponentInChildren<TouchWidget>();
			if ( touchWidget != null )
				touchWidget.enabled = enable;

			KeyboardWidget keyboardWidget = GetComponentInChildren<KeyboardWidget>();
			if ( keyboardWidget != null )
				keyboardWidget.enabled = enable;
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
