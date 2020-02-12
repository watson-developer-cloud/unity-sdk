/**
* Copyright 2020 IBM Corp. All Rights Reserved.
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

using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK;


namespace IBM.Watson.Examples
{
    public class ExampleTextToSpeechV1 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
        [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/text-to-speech/api\"")]
        [SerializeField]
        private string serviceUrl;
        private TextToSpeechService _service;
        private string allisionVoice = "en-US_AllisonV3Voice";
        private string synthesizeText = "Hello, welcome to the Watson Unity SDK!";
        private string synthesizeMimeType = "audio/wav";
        public Text textInput;
        private int _recordingRoutine = 0;
        private bool _textEntered = false;
        private AudioClip _recording = null;
        #endregion

        #region PlayClip
        private void PlayClip(AudioClip clip)
        {
            if (Application.isPlaying && clip != null)
            {
                GameObject audioObject = new GameObject("AudioObject");
                AudioSource source = audioObject.AddComponent<AudioSource>();
                source.spatialBlend = 0.0f;
                source.loop = false;
                source.clip = clip;
                source.Play();

                GameObject.Destroy(audioObject, clip.length);
            }
        }
        #endregion

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
            textInput = GetComponent<Text>();
        }

        void Update()
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // has backspace/delete been pressed?
                {
                    if (textInput.text.Length != 0)
                    {
                        textInput.text = textInput.text.Substring(0, textInput.text.Length - 1);
                    }
                }
                else if ((c == '\n') || (c == '\r')) // enter/return
                {
                    print("User entered the text: " + textInput.text);
                    _service.OnListen(textInput.text);
                    textInput.text = "";
                }
                else
                {
                    textInput.text += c;
                }
            }
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
            }

            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            while (!authenticator.CanAuthenticate())
            {
                yield return null;
            }

            _service = new TextToSpeechService(authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                _service.SetServiceUrl(serviceUrl);
            }

            Active = true;
            StartListening();
            // Runnable.Run(ExampleSynthesize());
        }

        private void OnError(string error)
        {
            Active = false;

            Log.Debug("ExampleTextToSpeech.OnError()", "Error! {0}", error);
        }

        public bool Active
        {
            get { return _service.IsListening; }
            set
            {
                if (value && !_service.IsListening)
                {
                    Log.Debug("start-", "listening");
                    _service.Voice = allisionVoice;
                    _service.OnError = OnError;
                    _service.StartListening(OnSynthesize);
                }
                else if (!value && _service.IsListening)
                {
                    Log.Debug("stop", "listening");
                    _service.StopListening();
                }
            }
        }

        private void StartListening()
        {
            if (_recordingRoutine == 0)
            {
                UnityObjectUtil.StartDestroyQueue();
                // _recordingRoutine = Runnable.Run(SynthesizeHandler());
            }
        }

        // private IEnumerator SynthesizeHandler()
        // {
        //     yield return null;      // let _recordingRoutine get set..

        //     Log.Debug("ExampleTextToSpeechV1", "Text entered: {0}, {1}", synthesizeText, _textEntered);
        //     while (_recordingRoutine != 0)
        //     {
        //         Log.Debug("ExampleTextToSpeechV1", "Text entered: {0}", synthesizeText);
        //         if (_textEntered)
        //         {
        //             _service.OnListen(synthesizeText);
        //             _textEntered = false;
        //             textInput.text = "";
        //         }
        //     }
        //     yield break;
        // }

        private void OnSynthesize(byte[] result) {
            Log.Debug("ExampleTextToSpeechV1", "Synthesize done!");
            _recording = WaveFile.ParseWAV("myClip", result);
            PlayClip(_recording);
        }

        #region Synthesize
        private IEnumerator ExampleSynthesize()
        {
            byte[] synthesizeResponse = null;
            AudioClip clip = null;
            _service.Synthesize(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
                {
                    synthesizeResponse = response.Result;
                    Log.Debug("ExampleTextToSpeechV1", "Synthesize done!");
                    clip = WaveFile.ParseWAV("myClip", synthesizeResponse);
                    PlayClip(clip);
                },
                text: synthesizeText,
                voice: allisionVoice,
                accept: synthesizeMimeType
            );

            while (synthesizeResponse == null)
                yield return null;

            yield return new WaitForSeconds(clip.length);
        }
        #endregion
    }
}
