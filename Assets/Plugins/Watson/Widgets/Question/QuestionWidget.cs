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


using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Avatar;
using IBM.Watson.Services.v1;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Avatar of Watson 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class QuestionWidget : Widget
    {
        #region Private Data
        private EventManager m_EventManager = new EventManager();
        private CubeAnimationManager m_CubeAnimMgr = null;

		private AnswersAndConfidence answersAndConfidence;
		private Evidence evidence;
		private Semantic semantic;
		private Features features;
		private Chat chat;
		private Location location;
		private ParseTree parseTree;
		private QA qa;

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
        public ITM.Questions Questions { get; set; }
        public ITM.Answers Answers { get; set; }
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
        #endregion

        public void OnDisplayAnswers(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Answers );
        }

        public void OnDisplayParse(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Parse );
        }

        public void OnDisplayEvidence(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Evidence );
        }

        public void OnDisplayFeatures(object[] args)
        {
            Cube.FocusOnSide( CubeAnimationManager.CubeSideType.Chat );
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

        protected void Awake()
        {
            m_EventManager.RegisterEventReceiver("fold", OnFold);
            m_EventManager.RegisterEventReceiver("unfold", OnUnfold);
            m_EventManager.RegisterEventReceiver("evidence", OnDisplayEvidence);
            m_EventManager.RegisterEventReceiver("parse", OnDisplayParse);
            m_EventManager.RegisterEventReceiver("features", OnDisplayFeatures);
            m_EventManager.RegisterEventReceiver("location", OnDisplayLocation);
            m_EventManager.RegisterEventReceiver("answers", OnDisplayAnswers);

			answersAndConfidence = gameObject.GetComponent<AnswersAndConfidence>();
			evidence = gameObject.GetComponent<Evidence>();
			semantic = gameObject.GetComponent<Semantic>();
			features = gameObject.GetComponent<Features>();
			chat = gameObject.GetComponent<Chat>();
			location = gameObject.GetComponent<Location>();
			parseTree = gameObject.GetComponent<ParseTree>();
			qa = gameObject.GetComponent<QA>();
        }

        protected override void Start()
        {
            base.Start();

			if (Questions == null) {
				Log.Error("QuestionWidget", "There is no Questions object!");
				return;
			}

            if (!Avatar.ITM.GetParseData(Questions.questions[0].transactionId, OnParseData))
                Log.Error("QuestionWidget", "Failed to request ParseData.");

            // give the cube animation manager the game object


        }

		public void Init()
		{
			Debug.Log ("QuestionWIdget.init();");
			answersAndConfidence.Init ();
			evidence.Init ();
			semantic.Init ();
			features.Init ();
			chat.Init ();
			location.Init ();
			parseTree.Init ();
			qa.Init ();
		}

        private void OnParseData(ITM.ParseData parse)
        {

        }
    }
}
