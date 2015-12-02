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


using IBM.Watson.DataTypes;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// This widget records audio from the microphone device.
    /// </summary>
    public class MicrophoneWidget : Widget
    {
        #region Public Properties
        /// <summary>
        /// Returns a list of available microphone devices.
        /// </summary>
        public string[] Microphones { get { return Microphone.devices; } }
        /// <summary>
        /// True if microphone is active, false if inactive.
        /// </summary>
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (m_Active != value)
                {
                    m_Active = value;
                    if (m_Active && !m_Disabled)
                        StartRecording();
                    else
                        StopRecording();
                }
            }
        }
        /// <summary>
        /// True if microphone is disabled, false if enabled.
        /// </summary>
        public bool Disable
        {
            get { return m_Disabled; }
            set
            {
                if (m_Disabled != value)
                {
                    m_Disabled = value;
                    if (m_Active && !m_Disabled)
                        StartRecording();
                    else
                        StopRecording();
                }
            }
        }
        /// <summary>
        /// Button handler for toggling the active state.
        /// </summary>
        public void OnToggleActive()
        {
            Active = !Active;       
        }
        #endregion

        #region Widget interface
        /// <exclude />
        protected override string GetName()
        {
            return "Microphone";
        }
        #endregion

        #region MonoBehaviour interface
        /// <exclude />
        protected override void Start()
        {
            base.Start();
            if (m_ActivateOnStart)
                Active = true;
        }
        #endregion

        #region Private Data
        private bool m_Active = false;
        private bool m_Disabled = false;

        [SerializeField]
        private bool m_ActivateOnStart = true;
        [SerializeField]
        private Input m_DisableInput = new Input("Disable", typeof(DisableMicData), "OnDisableInput");
        [SerializeField]
        private Output m_AudioOutput = new Output(typeof(AudioData));
        [SerializeField]
        private Output m_LevelOutput = new Output(typeof(LevelData));
		[SerializeField]
        private Output m_ActivateOutput = new Output(typeof(BooleanData));
        [SerializeField, Tooltip("Size of recording buffer in seconds.")]
        private int m_RecordingBufferSize = 2;
        [SerializeField]
        private int m_RecordingHZ = 22050;                  // default recording HZ
        [SerializeField, Tooltip("ID of the microphone to use.")]
        private string m_MicrophoneID = null;               // what microphone to use for recording.
        [SerializeField, Tooltip("How often to sample for level output.")]
        private float m_LevelOutputInterval = 0.05f;
		[SerializeField]
		private float m_LevelOutputModifier = 1.0f;
		[SerializeField, Tooltip("If true, microphone will playback recorded audio on stop.")]
        private bool m_PlaybackRecording = false;
        [SerializeField]
        private Text m_StatusText = null;

        private int m_RecordingRoutine = 0;                      // ID of our co-routine when recording, 0 if not recording currently.
        private AudioClip m_Recording = null;
        private List<AudioClip> m_Playback = new List<AudioClip>();

        #endregion

        #region Private Functions
        private void OnDisableInput(Data data)
        {
            Disable = ((DisableMicData)data).Boolean;
        }

        private void StartRecording()
        {
            if (m_RecordingRoutine == 0)
            {
                AudioClipUtil.StartDestroyQueue();

                m_RecordingRoutine = Runnable.Run(RecordingHandler());
                m_ActivateOutput.SendData(new BooleanData(true));

                if (m_StatusText != null)
                    m_StatusText.text = "RECORDING";
            }
        }

        private void StopRecording()
        {
            if (m_RecordingRoutine != 0)
            {
                Microphone.End(m_MicrophoneID);
                Runnable.Stop(m_RecordingRoutine);
                m_RecordingRoutine = 0;

                m_ActivateOutput.SendData(new BooleanData(false));
                if (m_StatusText != null)
                    m_StatusText.text = "STOPPED";

                if (m_PlaybackRecording && m_Playback.Count > 0)
                {
                    AudioClip combined = AudioClipUtil.Combine(m_Playback.ToArray());
                    if (combined != null)
                    {
                        AudioSource source = GetComponentInChildren<AudioSource>();
                        if (source != null)
                        {
                            // destroy any previous audio clip..
                            if ( source.clip != null )
                                AudioClipUtil.DestroyAudioClip( source.clip );

                            source.spatialBlend = 0.0f;     // 2D sound
                            source.loop = false;            // do not loop
                            source.clip = combined;         // clip
                            source.Play();
                        }
                        else
                            Log.Warning("MicrophoneWidget", "Failed to find AudioSource.");
                    }

                    foreach( var clip in m_Playback )
                        AudioClipUtil.DestroyAudioClip( clip );
                    m_Playback.Clear();
                }
            }
        }

        private IEnumerator RecordingHandler()
        {
#if UNITY_WEBPLAYER
            yield return Application.RequestUserAuthorization( UserAuthorization.Microphone );
#endif
            m_Recording = Microphone.Start(m_MicrophoneID, true, m_RecordingBufferSize, m_RecordingHZ);
            yield return null;      // let m_RecordingRoutine get set..

			if (m_Recording == null) 
			{
				Log.Error( "MicrophoneWidget", "Failed to start recording." );
				yield break;
			}

            bool bFirstBlock = true;
            int midPoint = m_Recording.samples / 2;

            bool bOutputLevelData = m_LevelOutput.IsConnected;
            bool bOutputAudio = m_AudioOutput.IsConnected || m_PlaybackRecording;

            int lastReadPos = 0;
            float[] samples = null;

            while (m_RecordingRoutine != 0 && m_Recording != null)
            {
                int writePos = Microphone.GetPosition(m_MicrophoneID);
                if (bOutputAudio)
                {
                    if ((bFirstBlock && writePos >= midPoint)
                        || (!bFirstBlock && writePos < midPoint))
                    {
                        // front block is recorded, make a RecordClip and pass it onto our callback.
                        samples = new float[midPoint];
                        m_Recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                        AudioData record = new AudioData();
                        record.MaxLevel = Mathf.Max(samples);
                        record.Clip = AudioClip.Create("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
                        record.Clip.SetData(samples, 0);

                        if (m_PlaybackRecording)
                            m_Playback.Add(record.Clip);
                        if ( m_AudioOutput.IsConnected && !m_AudioOutput.SendData(record))
                            StopRecording();        // automatically stop recording if the callback goes away.

                        bFirstBlock = !bFirstBlock;
                    }
                    else
                    {
                        // calculate the number of samples remaining until we ready for a block of audio, 
                        // and wait that amount of time it will take to record.
                        int remaining = bFirstBlock ? (midPoint - writePos) : (m_Recording.samples - writePos);
                        float timeRemaining = (float)remaining / (float)m_RecordingHZ;
                        if (bOutputLevelData && timeRemaining > m_LevelOutputInterval)
                            timeRemaining = m_LevelOutputInterval;
                        yield return new WaitForSeconds(timeRemaining);
                    }
                }
                else
                {
                    yield return new WaitForSeconds(m_LevelOutputInterval);
                }

                if (m_Recording != null && bOutputLevelData)
                {
                    float fLevel = 0.0f;
                    if (writePos < lastReadPos)
                    {
                        // write has wrapped, grab the last bit from the buffer..
                        samples = new float[m_Recording.samples - lastReadPos];
                        m_Recording.GetData(samples, lastReadPos);
                        fLevel = Mathf.Max(fLevel, Mathf.Max(samples));

                        lastReadPos = 0;
                    }

                    if (lastReadPos < writePos)
                    {
                        samples = new float[writePos - lastReadPos];
                        m_Recording.GetData(samples, lastReadPos);
                        fLevel = Mathf.Max(fLevel, Mathf.Max(samples));

                        lastReadPos = writePos;
                    }

					m_LevelOutput.SendData(new LevelData(fLevel * m_LevelOutputModifier));
                }
            }

            yield break;
        }
        #endregion
    }
}
