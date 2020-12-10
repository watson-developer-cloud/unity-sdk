/**
* (C) Copyright IBM Corp. 2019, 2020.
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

using IBM.Watson.ToneAnalyzer.V3;
using IBM.Watson.ToneAnalyzer.V3.Model;
using IBM.Cloud.SDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Text;
using System.IO;

namespace IBM.Watson.Examples
{
    public class ExampleToneAnalyzerV3 : MonoBehaviour
    {
        #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
        [Space(10)]
        [Tooltip("The IAM apikey.")]
        [SerializeField]
        private string iamApikey;
        [Tooltip("The service URL (optional). This defaults to \"https://api.us-south.tone-analyzer.watson.cloud.ibm.com\"")]
        [SerializeField]
        private string serviceUrl;
        [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
        [SerializeField]
        private string versionDate;
        #endregion

        private ToneAnalyzerService service;
        private string stringToTestTone = "This service enables people to discover and understand, and revise the impact of tone in their content. It uses linguistic analysis to detect and interpret emotional, social, and language cues found in text.";

        private bool toneTested = false;
        private bool toneChatTested = false;

        private void Start()
        {
            LogSystem.InstallDefaultReactors();
            Runnable.Run(CreateService());
        }

        private IEnumerator CreateService()
        {
            if (string.IsNullOrEmpty(iamApikey))
            {
                throw new IBMException("Plesae provide IAM ApiKey for the service.");
            }

            //  Create credential and instantiate service
            IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

            //  Wait for tokendata
            while (!authenticator.CanAuthenticate())
                yield return null;

            service = new ToneAnalyzerService(versionDate, authenticator);
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                service.SetServiceUrl(serviceUrl);
            }

            Runnable.Run(Examples());
        }

        private IEnumerator Examples()
        {
            byte[] bytes = Encoding.ASCII.GetBytes(stringToTestTone);
            MemoryStream toneInput = new MemoryStream(bytes);

            List<string> tones = new List<string>()
            {
                "emotion",
                "language",
                "social"
            };
            service.Tone(callback: OnTone, toneInput: toneInput, sentences: true, tones: tones, contentLanguage: "en", acceptLanguage: "en", contentType: "application/json");

            while (!toneTested)
            {
                yield return null;
            }


            List<Utterance> utterances = new List<Utterance>()
            {
                new Utterance()
                {
                    Text = stringToTestTone,
                    User = "User 1"
                }
            };
            service.ToneChat(callback: OnToneChat, utterances: utterances, contentLanguage: "en", acceptLanguage: "en");

            while (!toneChatTested)
            {
                yield return null;
            }

            Log.Debug("ExampleToneAnalyzerV3.Examples()", "Examples complete!");
        }

        private void OnTone(DetailedResponse<ToneAnalysis> response, IBMError error)
        {
            if (error != null)
            {
                Log.Debug("ExampleToneAnalyzerV3.OnTone()", "Error: {0}: {1}", error.StatusCode, error.ErrorMessage);
            }
            else
            {
                Log.Debug("ExampleToneAnalyzerV3.OnTone()", "{0}", response.Response);
            }

            toneTested = true;
        }

        private void OnToneChat(DetailedResponse<UtteranceAnalyses> response, IBMError error)
        {
            if (error != null)
            {
                Log.Debug("ExampleToneAnalyzerV3.OnToneChat()", "Error: {0}: {1}", error.StatusCode, error.ErrorMessage);
            }
            else
            {
                Log.Debug("ExampleToneAnalyzerV3.OnToneChat()", "{0}", response.Response);
            }

            toneChatTested = true;
        }
    }
}
