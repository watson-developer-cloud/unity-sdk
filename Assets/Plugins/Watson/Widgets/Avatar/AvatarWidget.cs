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


using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Avatar;
using IBM.Watson.Services.v1;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#pragma warning disable 414

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
        public enum AvatarState
        {
            CONNECTING,
            LISTENING,
            THINKING,
            ANSWERING
        };
        #endregion

        #region Private Data
        private ITM m_ITM = new ITM();                      // ITM service is used to get question & answer details
        private NLC m_NLC = new NLC();                      // natural language classifier
        private AvatarState m_State = AvatarState.CONNECTING;
        private NLC.ClassifyResult m_ClassifyResult = null;

        private bool m_Sleeping = true;
        private SpeechToText.ResultList m_SpeechResult = null;
        private string m_SpeechText = null;
        private ITM.Questions m_QuestionResult = null;
        private GameObject m_FocusQuestion = null;
                
        [SerializeField]
        private TextToSpeech.VoiceType m_VoiceType = TextToSpeech.VoiceType.en_US_Michael;
        [SerializeField]
        private string m_ClassifierId = "5E00F7x2-nlc-540";     // default to XRAY classifier
        [SerializeField]
        private float m_SoundVisualizerModifier = 20.0f;
        [SerializeField,Tooltip("What is the minimum word confidence needed to send onto the NLC?")]
        private double m_MinWordConfidence = 0.9; 
        [SerializeField,Tooltip("Recognized speech below this confidence is just ignored.")]
        private double m_IgnoreWordConfidence = 0.5;
        [SerializeField]
        private string m_Hello = "Hello";
        [SerializeField]
        private string m_Goodbye = "Goodbye";
        [SerializeField]
        private string m_RecognizeFailure = "I'm sorry, but I didn't understand your question.";
        [SerializeField]
        private string m_Pipeline = "thunderstone";
        [SerializeField]
        private Input m_RecognizeInput = new Input("Recognize", typeof(SpeechToTextData), "OnRecognize");
        [SerializeField]
        private Input m_levelInput = new Input("Level", typeof(FloatData), "OnLevelInput");
        [SerializeField]
        private Output m_TextOutput = new Output(typeof(TextData));
        [SerializeField, Tooltip("Recognized speech is put into this Text UI field.")]
        private Text m_RecognizeText = null;
        [SerializeField]
        private Text m_QuestionText = null;
        [SerializeField, Tooltip("The results of the NLC is placed in this text field.")]
        private Text m_ClassifyText = null;
        [SerializeField]
        private Text m_AnswerText = null;
        [SerializeField]
        private Text m_StateText = null;
        [SerializeField]
        private GameObject m_QuestionPrefab = null;
        #endregion

        #region Public Types
        public delegate void OnQuestion( string question );
        public delegate void OnAnswer( string answer );
        #endregion

        #region Public Properties
        public OnQuestion QuestionEvent { get; set; }
        public OnAnswer AnswerEvent { get; set; }
        public ITM ITM { get { return m_ITM; } }
        public NLC NLC { get { return m_NLC; } }
        public AvatarState State
        {
            get { return m_State; }
            private set
            {
                m_State = value;
                Log.Debug( "AvatarWidget", "State {0}", m_State.ToString() );

                if ( m_Sleeping )
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Idle );
                else if (m_State == AvatarState.LISTENING)
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Shy);
                else if (m_State == AvatarState.THINKING)
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Interested);
                else
                    EventManager.Instance.SendEvent(EventManager.onMoodChange, MoodType.Upset);

                if ( m_StateText != null )
                    m_StateText.text = m_Sleeping ? "SLEEPING" : m_State.ToString();
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
                if (m_pebbleManager == null)
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

            // login to ITM, then select the pipeline
            m_ITM.Login(OnItmLogin);
            // TEMP: spin up a cube right off the bat
            //m_FocusQuestion = GameObject.Instantiate(m_QuestionPrefab);
            //m_FocusQuestion.GetComponentInChildren<QuestionWidget>().Avatar = this;
        }
        private void OnItmLogin(bool success)
        {
            if (success)
                m_ITM.GetPipeline(m_Pipeline, true, OnPipeline );
            else
                Log.Error("AvtarWidget", "Failed to login to ITM.");
        }
        private void OnPipeline( ITM.Pipeline pipeline )
        {
            if ( pipeline != null )
                State = AvatarState.LISTENING;
            else
                Log.Equals( "AvatarWidget", "Failed to select pipeline." );

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
        private void OnRecognize(Data data)
        {
            SpeechToText.ResultList result = ((SpeechToTextData)data).Results;
            if (State == AvatarState.LISTENING )
            {
                if (result != null && result.Results.Length > 0
                    && result.Results[0].Final
                    && result.Results[0].Alternatives.Length > 0 )
                {
                    string text = result.Results[0].Alternatives[0].Transcript;
                    double textConfidence = result.Results[0].Alternatives[0].Confidence;

                    if (m_RecognizeText != null)
                    {
                        m_RecognizeText.text = string.Format( "R: {0} ({1})", text, textConfidence );
                        m_RecognizeText.color = textConfidence > m_MinWordConfidence ? Color.white : Color.red;
                    }

                    Log.Debug( "AvatarWidget", "OnRecognize: {0} ({1})", text, textConfidence );

                    if ( textConfidence > m_MinWordConfidence )
                    {
                        State = AvatarState.THINKING;
                        m_SpeechResult = result;
                        m_SpeechText = text;

                        if (m_FocusQuestion != null )
                            m_FocusQuestion.GetComponent<QuestionWidget>().OnFold(null);

                        if (!m_NLC.Classify(m_ClassifierId, m_SpeechText, OnSpeechClassified))
                            Log.Error("AvatarWidget", "Failed to send {0} to NLC.", m_SpeechText);
                    }
                    else
                    {
                        State = AvatarState.LISTENING;
                        if ( textConfidence > m_IgnoreWordConfidence )
                            m_TextOutput.SendData( new TextData(m_RecognizeFailure) );
                    }
                }
            }
        }

        private void OnSpeechClassified(NLC.ClassifyResult classify)
        {
            m_ClassifyResult = classify;

            Log.Debug("Avatar", "TopClass: {0}", m_ClassifyResult.top_class);
            if (m_ClassifyText != null)
                m_ClassifyText.text = "C: " + m_ClassifyResult.top_class;

            if ( m_Sleeping )
            {
                if ( m_ClassifyResult.top_class == "wakeup" )
                {
                    m_Sleeping = false;
                    m_TextOutput.SendData( new TextData( m_Hello ) );

                    if (m_FocusQuestion == null )
                    {
                        m_FocusQuestion = GameObject.Instantiate(m_QuestionPrefab);
                        m_FocusQuestion.GetComponentInChildren<QuestionWidget>().Avatar = this;
                    }
                    else
                        m_FocusQuestion.SetActive( true );
                }
                State = AvatarState.LISTENING;
            }
            else
            {
                if ( m_ClassifyResult.top_class == "sleep" )
                {
                    m_TextOutput.SendData( new TextData( m_Goodbye ) );
                    if ( m_FocusQuestion != null )
                        m_FocusQuestion.SetActive( false );

                    m_Sleeping = true;
                    State = AvatarState.LISTENING;
                }
                else if ( m_ClassifyResult.top_class == "question" || m_ClassifyResult.top_class == "watson-thunder" )
                {
                    if (!m_ITM.AskQuestion(m_SpeechResult.Results[0].Alternatives[0].Transcript, OnAskQuestion))
                        Log.Error("AvatarWidget", "Failed to send question to ITM." );
                }
                else
                {
                    State = AvatarState.LISTENING;
                    // send event to question then..
                    if (m_FocusQuestion != null)
                        m_FocusQuestion.GetComponent<QuestionWidget>().EventManager.SendEvent(m_ClassifyResult.top_class);
                }
            }
        }

		private void OnAskQuestion(ITM.Questions questions)
        {
            m_QuestionResult = questions;

            bool bGettingAnswers = false;
            if (m_QuestionResult != null && m_QuestionResult.questions != null)
            {
                ITM.Question topQuestion = m_QuestionResult.questions.Length > 0 ? m_QuestionResult.questions[0] : null;
                if (topQuestion != null)
                {
                    if (m_QuestionText != null)
                        m_QuestionText.text = "Q: " + topQuestion.question.questionText;
                    if ( QuestionEvent != null )
                        QuestionEvent( topQuestion.question.questionText );

                    bGettingAnswers = m_ITM.GetAnswers(topQuestion.transactionId, OnAnswerQuestion);
				}
            }

            if (!bGettingAnswers)
            {
                m_TextOutput.SendData(new TextData("Does not compute. beep."));
                State = AvatarState.LISTENING;
            }
        }

        private void OnAnswerQuestion(ITM.Answers answers)
        {
            if (answers != null && answers.answers.Length > 0)
            {
                foreach (var a in answers.answers)
                    Log.Debug("AvatarWidget", "A: {0} ({1})", a.answerText, a.confidence);

                string answer = answers.answers[0].answerText;
                if (m_AnswerText != null)
                    m_AnswerText.text = "A: " + answer;
                if ( AnswerEvent != null )
                    AnswerEvent( answer );
                m_TextOutput.SendData(new TextData(answer));

                if (m_QuestionPrefab != null)
                {
                    if (m_FocusQuestion == null)
                        m_FocusQuestion = GameObject.Instantiate(m_QuestionPrefab);

                    //m_FocusQuestion.transform.SetParent(transform, false);

                    QuestionWidget question = m_FocusQuestion.GetComponentInChildren<QuestionWidget>();
                    if (question != null)
                    {
                        question.Avatar = this;
                        question.Questions = m_QuestionResult;
                        question.Answers = answers;
						question.Init();

                        // show the answer panel
                        //question.EventManager.SendEvent( "answers" );

						if (!ITM.GetParseData(question.Questions.questions[0].transactionId, question.OnParseData));
							Log.Error("QuestionWidget", "Failed to request ParseData.");
                    }
                    else
                        Log.Error("AvatarWidget", "Failed to find QuestionWidget in question prefab.");
                }
            }
            State = AvatarState.LISTENING;
        }

        #endregion
    }
}
