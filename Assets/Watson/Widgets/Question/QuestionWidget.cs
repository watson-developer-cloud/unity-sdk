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
using IBM.Watson.Utilities;
using IBM.Watson.Widgets.Avatar;
using IBM.Watson.Widgets.Question.Facet;
using IBM.Watson.Data;
using UnityEngine;
using System.Collections.Generic;

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// This class manages the answers, question, and other data related to a question asked of the AvatarWidget.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class QuestionWidget : Widget
    {
        #region Private Data
        private EventManager m_EventManager = new EventManager();
        private CubeAnimationManager m_CubeAnimMgr = null;

		private AnswersAndConfidence m_AnswersAndConfidence;
		private IBM.Watson.Widgets.Question.Facet.Evidence m_Evidence;
		private Semantic m_Semantic;
		private Features m_Features;
		private Location m_Location;
		private ParseTree m_ParseTree;
		private QuestionAndAnswer m_QuestionAndAnswer;
		private List<Base> m_Facets = new List<Base>();

        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "Question";
        }
        #endregion

        #region Public Properties
        public EventManager EventManager { get { return m_EventManager; } }
        public AvatarWidget Avatar { get; set; }
        public CubeAnimationManager Cube {
            get {
                if ( m_CubeAnimMgr == null )
                {
                    m_CubeAnimMgr = GetComponentInChildren<CubeAnimationManager>();
                    m_CubeAnimMgr.avatarGameobject = Avatar.gameObject;
                }
                return m_CubeAnimMgr;
            }
        }

		public IQuestionData QuestionData { get; set; }

        #endregion

        public void OnDisplayAnswers(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Answers );
        }

        public void OnDisplayChat( object[] args )
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Chat );
        }

        public void OnDisplayParse(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Parse );
        }

        public void OnDisplayEvidence(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Evidence );
        }

        public void OnDisplayLocation(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Location );
        }

        public void OnFold(object[] args)
        {
            Cube.Fold();
        }
        public void OnUnfold(object[] args)
        {
            Cube.UnFold();
        }

		/// <summary>
		/// Register events, set facet references, add facets to a List.
		/// </summary>
        protected void Awake()
        {
            m_EventManager.RegisterEventReceiver("fold", OnFold);
            m_EventManager.RegisterEventReceiver("unfold", OnUnfold);
            m_EventManager.RegisterEventReceiver("evidence", OnDisplayEvidence);
            m_EventManager.RegisterEventReceiver("parse", OnDisplayParse);
            m_EventManager.RegisterEventReceiver("location", OnDisplayLocation);
            m_EventManager.RegisterEventReceiver("answers", OnDisplayAnswers);
            m_EventManager.RegisterEventReceiver("chat", OnDisplayChat );

			m_AnswersAndConfidence = gameObject.GetComponent<AnswersAndConfidence>();
			m_Evidence = gameObject.GetComponent<IBM.Watson.Widgets.Question.Facet.Evidence>();
			m_Semantic = gameObject.GetComponent<Semantic>();
			m_Features = gameObject.GetComponent<Features>();
			m_Location = gameObject.GetComponent<Location>();
			m_ParseTree = gameObject.GetComponent<ParseTree>();
			m_QuestionAndAnswer = gameObject.GetComponent<QuestionAndAnswer>();

			m_Facets.Add (m_AnswersAndConfidence);
			m_Facets.Add (m_Evidence);
			m_Facets.Add (m_Semantic);
			m_Facets.Add (m_Features);
			m_Facets.Add (m_Location);
			m_Facets.Add (m_ParseTree);
			m_Facets.Add (m_QuestionAndAnswer);
        }

        protected override void Start()
        {
            base.Start();
        }

		/// <summary>
		/// Sets Question, Answer and Avatar for each facet. Init is called by the Avatar Widget.
		/// </summary>
		public void Init(IQuestionData data) 
		{
			QuestionData = data;

			foreach (Base facet in m_Facets)
			{
				facet.Init();
			}
		}
    }

	public interface IQuestionData
	{
		Questions QuestionDataObject { get; }
		Answers AnswerDataObject { get; }
		ParseData ParseDataObject { get; }
		string Location { get; }
	}
}
