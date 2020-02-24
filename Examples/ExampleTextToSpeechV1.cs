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
        private TextToSpeechService service;
        private string allisionVoice = "en-US_AllisonV3Voice";
        private string synthesizeText = "Hello, welcome to the Watson Unity SDK!";
        private string placeholderText = "Please type text here and press enter.";
        private string waitingText = "Watson Text to Speech service is synthesizing the audio!";
        private string synthesizeMimeType = "audio/wav";
        public InputField textInput;
        private bool _textEntered = false;
        private AudioClip _recording = null;
        private byte[] audioStream = null;
        #endregion

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                service.SynthesizeUsingWebsockets(textInput.text);
                textInput.text = waitingText;
            }

            while(service != null && !service.IsListening)
            {
                if (audioStream != null && audioStream.Length > 0)
                {
                    Log.Debug("ExampleTextToSpeech", "Audio stream of {0} bytes received!", audioStream.Length.ToString()); // Use audioStream and play audio
                    // _recording = WaveFile.ParseWAV("myClip", audioStream);
                    // PlayClip(_recording);
                }
                textInput.text = placeholderText;
                audioStream = null;
                StartListening(); // need to connect because service disconnect websocket after transcribing https://cloud.ibm.com/docs/services/text-to-speech?topic=text-to-speech-usingWebSocket#WSsend
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

            service = new TextToSpeechService(authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }

            Active = true;
        }

        private void OnError(string error)
        {
            Active = false;

            Log.Debug("ExampleTextToSpeech.OnError()", "Error! {0}", error);
        }

        private void StartListening()
        {
            Log.Debug("ExampleTextToSpeech", "start-listening");
            service.Voice = allisionVoice;
            service.OnError = OnError;
            service.StartListening(OnSynthesize);
        }

        public bool Active
        {
            get { return service.IsListening; }
            set
            {
                if (value && !service.IsListening)
                {
                    StartListening();
                }
                else if (!value && service.IsListening)
                {
                    Log.Debug("ExampleTextToSpeech", "stop-listening");
                    service.StopListening();
                }
            }
        }

        private void OnSynthesize(byte[] result) {
            Log.Debug("ExampleTextToSpeechV1", "Binary data received!");
            audioStream = ConcatenateByteArrays(audioStream, result);
        }

        #region Synthesize Without Websocket Connection
        private IEnumerator ExampleSynthesize()
        {
            byte[] synthesizeResponse = null;
            AudioClip clip = null;
            service.Synthesize(
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

        #region Concatenate Byte Arrays
        private byte[] ConcatenateByteArrays(byte[] a, byte[] b)
        {
            if (a == null || a.Length == 0)
            {
                return b;
            }
            else if (b == null || b.Length == 0)
            {
                return a;
            }
            else
            {
                List<byte> list1 = new List<byte>(a);
                List<byte> list2 = new List<byte>(b);
                list1.AddRange(list2);
                byte[] result = list1.ToArray();
                return result;
            }
        }
        #endregion
    }
}
