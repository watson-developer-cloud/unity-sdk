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
* @author Dogukan Erenel (derenel@us.ibm.com)
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
    [RequireComponent(typeof(MicrophoneWidget))]
    [RequireComponent(typeof(SpeechToTextWidget))]
    [RequireComponent(typeof(TextToSpeechWidget))]
    public class AvatarWidget : Widget
    {
        #region Public Types
        public enum AvatarState {
            LISTENING,
            THINKING,
            ANSWERING
        };
        #endregion

        #region Private Data
        private ITM m_ITM = new ITM();                      // ITM service is used to get question & answer details
        private NLC m_NLC = new NLC();                      // natural language classifier
        private AvatarState m_State = AvatarState.LISTENING;
        private GameObject m_FocusQuestion = null;

        [SerializeField]
        private TextToSpeech.VoiceType m_VoiceType = TextToSpeech.VoiceType.en_US_Michael;
        [SerializeField]
        private string m_ClassifierId = "5E00F7x2-nlc-540";     // default to XRAY classifier
        [SerializeField]
        private float m_SoundVisualizerModifier = 20.0f;
        [SerializeField]
        private string m_Pipeline = "thunderstone";
        [SerializeField]
        private Input m_RecognizeInput = new Input( "Recognize", typeof(SpeechToTextData), "OnRecognize" );
        [SerializeField]
        private Input m_levelInput = new Input("Level", typeof(FloatData), "OnLevelInput");
        [SerializeField]
        private Output m_TextOutput = new Output( typeof(TextData) );
        [SerializeField, Tooltip("Recognized speech is put into this Text UI field.") ]
        private Text m_RecognizeText = null;    
        [SerializeField]
        private Text m_QuestionText = null;
        [SerializeField, Tooltip("The results of the NLC is placed in this text field.") ]
        private Text m_ClassifyText = null;
        [SerializeField]
        private Text m_AnswerText = null;
        [SerializeField]
        private GameObject m_QuestionPrefab = null;
        #endregion

        #region Public Properties
        public ITM ITM { get { return m_ITM; } }
        public NLC NLC { get { return m_NLC; } }
        public AvatarState State { get { return m_State; }
            private set {
                m_State = value;
                if ( m_State == AvatarState.LISTENING )
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Idle );
                else if ( m_State == AvatarState.THINKING )
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Urgent );
                else
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Interested );
            }
        }
        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "Avatar";
        }
        #endregion

        #region Pebble Manager for Visualization
        private PebbleManager m_pebbleManager;

        /// <summary>
        /// Gets the pebble manager. Sound Visualization on the avatar. 
        /// </summary>
        /// <value>The pebble manager.</value>
        public PebbleManager pebbleManager
        {
            get
            {
                if ( m_pebbleManager == null )
                    m_pebbleManager = GetComponentInChildren<PebbleManager>();
                return m_pebbleManager;
            }
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            MoodManager.Instance.currentMood = MoodType.Idle;
            BehaviorManager.Instance.currentBehavior = BehaviorType.Idle;
        }
        protected override void Start()
        {
            base.Start();
            m_ITM.Login( OnItmLogin );
            m_ITM.GetPipeline( m_Pipeline, true );
        }
        private void OnItmLogin(bool success)
        {
            if (! success )
                Log.Error( "AvtarWidget", "Failed to login to ITM." );
        }
        #endregion

        #region Level Input
        private void OnLevelInput(Data data)
        {
            FloatData levelInput = (FloatData)data;
            if (pebbleManager != null)
                pebbleManager.SetAudioData(levelInput.Float * m_SoundVisualizerModifier);
        }
        #endregion

        #region Audio Input
        private void OnRecognize( Data data)
	    {
            SpeechToText.ResultList result = ((SpeechToTextData)data).Results;
            if ( State == AvatarState.LISTENING )
            {
	            if (result != null && result.Results.Length > 0 
                    && result.Results[0].Final
                    && result.Results[0].Alternatives.Length > 0 )
	            {
                    string text = result.Results[0].Alternatives[0].Transcript;
                    Log.Debug( "AvatarWidget", "OnRecognize: {0}", text );

	                if ( m_RecognizeText != null )
	                    m_RecognizeText.text = "R: " + text;

                    State = AvatarState.THINKING;
                    m_ClassifyResult = null;
                    m_QuestionResult = null;

                    if (! m_NLC.Classify( m_ClassifierId, text, OnSpeechClassified ) )
                        Log.Error( "AvatarWidget", "Failed to send {0} to NLC.", text );
                    if (! m_ITM.AskQuestion( text, OnAskQuestion ) )
                        Log.Error( "AvatarWidget", "Failed to send {0} to ITM.", text );
	            }
            }
	    }

        private NLC.ClassifyResult m_ClassifyResult = null;
        private void OnSpeechClassified( NLC.ClassifyResult classify )
        {
            m_ClassifyResult = classify;      
            if ( m_ClassifyResult != null && m_QuestionResult != null )
                OnClassifiedAction();
        }

        private ITM.Questions m_QuestionResult = null;
        private void OnAskQuestion( ITM.Questions questions )
        {
            m_QuestionResult = questions;
            if ( m_ClassifyResult != null && m_QuestionResult != null )
                OnClassifiedAction();
        }

        private delegate void OnAction();

        private void OnClassifiedAction()
        {
            Log.Debug( "Avatar", "TopClass: {0}", m_ClassifyResult.top_class );
            if ( m_ClassifyText != null )
                m_ClassifyText.text = "C: " + m_ClassifyResult.top_class;

            Dictionary<string,OnAction> classifyActions = new Dictionary<string, OnAction>()
            {
                {"parse", OnDisplayParse },
                {"question", OnNewQuestion },
                {"evidence", OnDisplayEvidence },
                {"features", OnDisplayFeatures },
                {"location", OnDisplayLocation },
                {"answers", OnDisplayAnswers }, 
                {"unfold", OnUnfold },
                {"fold", OnFold },
            };

            OnAction action = null;
            if ( classifyActions.TryGetValue( m_ClassifyResult.top_class, out action ) )
                action();
            else
                State = AvatarState.LISTENING;
        }

        private void OnNewQuestion()
        {
            bool bGettingAnswers = false;
            if ( m_QuestionResult != null && m_QuestionResult.questions != null )
            {
                ITM.Question topQuestion = m_QuestionResult.questions.Length > 0 ? m_QuestionResult.questions[0] : null;
                if ( topQuestion != null )
                {
                    if ( m_QuestionText != null )
                        m_QuestionText.text = "Q: " + topQuestion.question.questionText;
                    bGettingAnswers = m_ITM.GetAnswers( topQuestion.transactionId, OnAnswerQuestion );
                }
            }

            if (! bGettingAnswers )
            {
                m_TextOutput.SendData( new TextData( "Does not compute. beep." ) );
                State = AvatarState.LISTENING;
            }
        }

        private void OnAnswerQuestion( ITM.Answers answers )
        {
            if ( answers != null && answers.answers.Length > 0 )
            {
                foreach( var a in answers.answers )
                    Log.Debug( "AvatarWidget", "A: {0} ({1})", a.answerText, a.confidence );

                string answer = answers.answers[0].answerText;
                if ( m_AnswerText != null )
                    m_AnswerText.text = "A: " + answer;
                m_TextOutput.SendData( new TextData( answer ) );

                if ( m_QuestionPrefab != null )
                {
                    if ( m_FocusQuestion != null )
                        Destroy( m_FocusQuestion );

                    m_FocusQuestion = GameObject.Instantiate( m_QuestionPrefab );
                    m_FocusQuestion.transform.SetParent( transform, false );

                    QuestionWidget question = m_FocusQuestion.GetComponentInChildren<QuestionWidget>();
                    if ( question != null )
                    {
                        question.Avatar = this;
                        question.Questions = m_QuestionResult;
                        question.Answers = answers;
                    }
                    else
                        Log.Error( "AvatarWidget", "Failed to find QuestionWidget in question prefab." );
                }
            }
            State = AvatarState.LISTENING;
        }

        private void OnDisplayAnswers()
        {
            State = AvatarState.LISTENING;
        }

        private void OnDisplayParse()
        {
            State = AvatarState.LISTENING;
        }

        private void OnDisplayEvidence()
        {
            State = AvatarState.LISTENING;
        }

        private void OnDisplayFeatures()
        {
            State = AvatarState.LISTENING;
        }

        private void OnDisplayLocation()
        {
            State = AvatarState.LISTENING;
        }

        private void OnFold()
        {
            State = AvatarState.LISTENING;
        }
        private void OnUnfold()
        {
            State = AvatarState.LISTENING;
        }

        #endregion



    }
}
