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
using IBM.Watson.Avatar;
using IBM.Watson.Data;
using IBM.Watson.Services.v1;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace IBM.Watson.Widgets.Avatar
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
        /// <summary>
        /// State of the Avatar in terms of its behavior.
        /// </summary>
        public enum AvatarState
        {
            /// <summary>
            /// Connecting - initial state
            /// </summary>
            CONNECTING, 
            /// <summary>
            /// Connected - Listening continuously to understand the input
            /// </summary>
            LISTENING, 
            /// <summary>
            /// Connected - After some input it is the time for thinking the understand the input
            /// </summary>
            THINKING,
            /// <summary>
            /// Connected - After semantically understanding, Watson is responding. 
            /// If Watson didn't understand the input, then it goes to listening mode while giving some response about not understood input.
            /// </summary>
            ANSWERING
        };
        
        /// <summary>
        /// Avatar Mood which effects various things in the application like Animation speed, coloring etc.
        /// </summary>
        public enum MoodType
        {
            /// <summary>
			/// Connecting / Disconnected - Waiting to be waken-up ( initial state )
			/// </summary>
            SLEEPING = 0,          
            /// <summary>
            /// Connected - After wake up - waits a mood change
            /// </summary>
            IDLE,
            /// <summary>
            /// Connected - After wake up - set interested mood
            /// </summary>
            INTERESTED,
            /// <summary>
            /// Connected - After wake up - set urgent mood
            /// </summary>
            URGENT,
            /// <summary>
            /// Connected - After wake up - set upset mood
            /// </summary>
            UPSET,
            /// <summary>
            /// Connected - After wake up - set shy mood
            /// </summary>
            SHY                 
        }

        #endregion

        #region Private Data
        private ITM m_ITM = new ITM();                      // ITM service
        private NLC m_NLC = new NLC();                      // NLC service
        private Dialog m_Dialog = new Dialog();             // Dialog service

        private AvatarState m_State = AvatarState.CONNECTING;
        private NLC.ClassifyResult m_ClassifyResult = null;
        
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
        [SerializeField]
        private string m_DialogName = "xray";
        private string m_DialogId = null;    
        private int m_DialogClientId = 0;
        private int m_DialogConversationId = 0;
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
				EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, this, value);
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
        void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD, OnChangeMood);
        }
        void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD, OnChangeMood);
        }

        protected override void Start()
        {
            base.Start();

			Mood = MoodType.SLEEPING;
			State = AvatarState.CONNECTING;

            // login to ITM, then select the pipeline
            m_ITM.Login(OnItmLogin);
            // Find our dialog ID
            if (! string.IsNullOrEmpty( m_DialogName ) )
                m_Dialog.GetDialogs( OnFindDialog );
        }

        private void OnFindDialog( Dialog.Dialogs dialogs )
        {
            if ( dialogs != null )
            {
                foreach( var dialog in dialogs.dialogs )
                {
                    if ( dialog.name == m_DialogName )
                        m_DialogId = dialog.dialog_id;
                }
            }

            if (string.IsNullOrEmpty( m_DialogId ) )
                Log.Error( "AvatarWidget", "Failed to find dialog ID for {0}", m_DialogName );
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

            if ( Mood == MoodType.SLEEPING )
            {
                if ( m_ClassifyResult.top_class == "wakeup" )
                {
                    Mood = MoodType.IDLE;
                    if (m_FocusQuestion == null )
                    {
                        m_FocusQuestion = GameObject.Instantiate(m_QuestionPrefab);
                        m_FocusQuestion.GetComponentInChildren<QuestionWidget>().Avatar = this;
                    }
                    else
                        m_FocusQuestion.SetActive( true );

                    // start a conversation with the dialog..
                    if (! string.IsNullOrEmpty( m_DialogId ) )
                        m_Dialog.Converse( m_DialogId, m_SpeechText, OnDialog, 0, m_DialogClientId );                      
                    else
                        m_TextOutput.SendData( new TextData( m_Hello ) );
                }
                State = AvatarState.LISTENING;
            }
            else
            {
                if ( m_ClassifyResult.top_class == "dialog" )
                {
                    if (! string.IsNullOrEmpty( m_DialogId ) )
                    {
                        m_Dialog.Converse( m_DialogId, m_SpeechText, OnDialog, 
                            m_DialogConversationId, m_DialogClientId );
                    }

                    State = AvatarState.LISTENING;
                }
                else if ( m_ClassifyResult.top_class == "sleep" )
                {
                    m_TextOutput.SendData( new TextData( m_Goodbye ) );
                    if ( m_FocusQuestion != null )
                        m_FocusQuestion.SetActive( false );

                    Mood = MoodType.SLEEPING;
                    State = AvatarState.LISTENING;
                    m_DialogConversationId = 0;
                    m_DialogClientId = 0;
                }
                else if ( m_ClassifyResult.top_class == "question" || m_ClassifyResult.top_class == "watson-thunder" )
                {

                    if (m_FocusQuestion != null)
                        m_FocusQuestion.GetComponent<QuestionWidget>().OnFold(null);

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

        private void OnDialog( Dialog.Response resp )
        {
            if ( resp != null )
            {
                m_DialogClientId = resp.client_id;
                m_DialogConversationId = resp.conversation_id;

                if ( resp.response != null )
                {
                    foreach( var t in resp.response )
                    {
                        if (! string.IsNullOrEmpty( t ) )
                            m_TextOutput.SendData( new TextData( t ) );
                    }
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
						question.ClearFacets();
                        question.Questions = m_QuestionResult;
                        question.Answers = answers;
						question.Init();

                        // show the answer panel
                        //question.EventManager.SendEvent( "answers" );

						if (!ITM.GetParseData(question.Questions.questions[0].transactionId, question.OnParseData))
							Log.Error("QuestionWidget", "Failed to request ParseData.");
                    }
                    else
                        Log.Error("AvatarWidget", "Failed to find QuestionWidget in question prefab.");
                }
            }
            State = AvatarState.LISTENING;
        }

        #endregion

        #region Avatar Mood / Behavior  - Adaptive Computing
        
        public Color BehaviourColor
        {
            get
            {
                Color color = Color.white;
                switch (m_State)
                {
                    case AvatarState.CONNECTING: color = new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f); break;     //Idle color 
                    case AvatarState.LISTENING: color = new Color(0 / 255.0f, 166 / 255.0f, 160 / 255.0f); break;
                    case AvatarState.THINKING: color = new Color(238 / 255.0f, 62 / 255.0f, 150 / 255.0f); break;
                    case AvatarState.ANSWERING: color = new Color(140 / 255.0f, 198 / 255.0f, 63 / 255.0f); break;
                    default: Log.Error("AvatarWidget", "AvatarState is not defined for color!"); color = Color.white; break;
                }
                return color;
            }
        }

        private MoodType m_currentMood = MoodType.SLEEPING;
        public MoodType Mood
        {
            get
            {
                return m_currentMood;
            }
            set
            {
                m_currentMood = value;
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, (int)value);
            }
        }

        MoodType[] m_moodTypeList = null;
        public MoodType[] MoodTypeList
        {
            get
            {
                if (m_moodTypeList == null)
                {
                    m_moodTypeList = new MoodType[] { MoodType.IDLE, MoodType.INTERESTED, MoodType.URGENT, MoodType.UPSET, MoodType.SHY };
                }
                return m_moodTypeList;
            }
        }

        public Color MoodColor
        {
            get
            {
                Color color = Color.white;
                switch (Mood)
                {
					case MoodType.SLEEPING: color = new Color(255 / 255.0f, 255 / 255.0f, 255 / 255.0f); break;
					case MoodType.IDLE: color = new Color(241 / 255.0f, 241 / 255.0f, 242 / 255.0f); break;
                    case MoodType.INTERESTED: color = new Color(131 / 255.0f, 209 / 255.0f, 245 / 255.0f); break;
                    case MoodType.URGENT: color = new Color(221 / 255.0f, 115 / 255.0f, 28 / 255.0f); break;
                    case MoodType.UPSET: color = new Color(217 / 255.0f, 24 / 255.0f, 45 / 255.0f); break;
                    case MoodType.SHY: color = new Color(243 / 255.0f, 137 / 255.0f, 175 / 255.0f); break;
                    default: Log.Error("AvatarWidget", "MoodType is not defined for color!"); color = Color.white; break;
                }
                return color;
            }
        }

        public float MoodSpeedModifier
        {
            get
            {
                float value = 1.0f;
                switch (Mood)
                {
					case MoodType.SLEEPING: value = 0.0f; break;
					case MoodType.IDLE: value = 1.0f; break;
                    case MoodType.INTERESTED: value = 1.1f; break;
                    case MoodType.URGENT: value = 2.0f; break;
                    case MoodType.UPSET: value = 1.5f; break;
                    case MoodType.SHY: value = 0.9f; break;
					default: Log.Error("AvatarWidget", "MoodType : {0} is not defined for speed modifier!", Mood); value = 1.0f; break;
                }
                return value;
            }
        }

        public float MoodTimeModifier
        {
            get
            {
                float value = MoodSpeedModifier;
                if (value != 0.0f)
                    value = 1.0f / value;

                return value;
            }
        }

		public float BehaviorSpeedModifier
		{
			get
			{
				float value = 1.0f;
				switch (State)
				{
				case AvatarState.CONNECTING: value = 0.0f; break;
				case AvatarState.ANSWERING: value = 1.0f; break;
				case AvatarState.LISTENING: value = 1.0f; break;
				case AvatarState.THINKING: value = 1.0f; break;
				default: Log.Error("AvatarWidget", "State : {0} is not defined for speed modifier!", State); value = 1.0f; break;
				}
				return value;
			}
		}

		public float BehaviorTimeModifier
		{
			get
			{
				float value = BehaviorSpeedModifier;
				if (value != 0.0f)
					value = 1.0f / value;

				return value;
			}
		}

        void OnChangeMood(System.Object[] args)
        {
            if (args.Length == 1)
            {
                Mood = (MoodType)args[0];
            }
        }
        #endregion
    }

    }
