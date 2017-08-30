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

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System;
using System.IO;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestTextToSpeech : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        TextToSpeech _textToSpeech;
        string _testString = "<speak version=\"1.0\"><say-as interpret-as=\"letters\">I'm sorry</say-as>. <prosody pitch=\"150Hz\">This is Text to Speech!</prosody><express-as type=\"GoodNews\">I'm sorry. This is Text to Speech!</express-as></speak>";

        string _createdCustomizationId;
        CustomVoiceUpdate _customVoiceUpdate;
        string _customizationName = "unity-example-customization";
        string _customizationLanguage = "en-US";
        string _customizationDescription = "A text to speech voice customization created within Unity.";
        string _testWord = "Watson";

        private bool _synthesizeTested = false;
        private bool _getVoicesTested = false;
        private bool _getVoiceTested = false;
        private bool _getPronuciationTested = false;
        private bool _getCustomizationsTested = false;
        private bool _createCustomizationTested = false;
        private bool _deleteCustomizationTested = false;
        private bool _getCustomizationTested = false;
        private bool _updateCustomizationTested = false;
        private bool _getCustomizationWordsTested = false;
        private bool _addCustomizationWordsTested = false;
        private bool _deleteCustomizationWordTested = false;
        private bool _getCustomizationWordTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            try
            {
                VcapCredentials vcapCredentials = new VcapCredentials();
                fsData data = null;

                //  Get credentials from a credential file defined in environmental variables in the VCAP_SERVICES format. 
                //  See https://www.ibm.com/watson/developercloud/doc/common/getting-started-variables.html.
                var environmentalVariable = Environment.GetEnvironmentVariable("VCAP_SERVICES");
                var fileContent = File.ReadAllText(environmentalVariable);

                //  Add in a parent object because Unity does not like to deserialize root level collection types.
                fileContent = Utility.AddTopLevelObjectToJson(fileContent, "VCAP_SERVICES");

                //  Convert json to fsResult
                fsResult r = fsJsonParser.Parse(fileContent, out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Convert fsResult to VcapCredentials
                object obj = vcapCredentials;
                r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                //  Set credentials from imported credntials
                Credential credential = vcapCredentials.VCAP_SERVICES["text_to_speech"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestTextToSpeech", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};
            
            _textToSpeech = new TextToSpeech(credentials);

            //  Synthesize
            Log.Debug("ExampleTextToSpeech", "Attempting synthesize.");
            _textToSpeech.Voice = VoiceType.en_US_Allison;
            _textToSpeech.ToSpeech(_testString, HandleToSpeechCallback, true);
            while (!_synthesizeTested)
                yield return null;

            //	Get Voices
            Log.Debug("ExampleTextToSpeech", "Attempting to get voices.");
            _textToSpeech.GetVoices(OnGetVoices);
            while (!_getVoicesTested)
                yield return null;

            //	Get Voice
            Log.Debug("ExampleTextToSpeech", "Attempting to get voice {0}.", VoiceType.en_US_Allison);
            _textToSpeech.GetVoice(OnGetVoice, VoiceType.en_US_Allison);
            while (!_getVoiceTested)
                yield return null;

            //	Get Pronunciation
            Log.Debug("ExampleTextToSpeech", "Attempting to get pronunciation of {0}", _testWord);
            _textToSpeech.GetPronunciation(OnGetPronunciation, _testWord, VoiceType.en_US_Allison);
            while (!_getPronuciationTested)
                yield return null;

            //  Get Customizations
            Log.Debug("ExampleTextToSpeech", "Attempting to get a list of customizations");
            _textToSpeech.GetCustomizations(OnGetCustomizations);
            while (!_getCustomizationsTested)
                yield return null;

            //  Create Customization
            Log.Debug("ExampleTextToSpeech", "Attempting to create a customization");
            _textToSpeech.CreateCustomization(OnCreateCustomization, _customizationName, _customizationLanguage, _customizationDescription);
            while (!_createCustomizationTested)
                yield return null;

            //  Get Customization
            Log.Debug("ExampleTextToSpeech", "Attempting to get a customization");
            if (!_textToSpeech.GetCustomization(OnGetCustomization, _createdCustomizationId))
                Log.Debug("ExampleTextToSpeech", "Failed to get custom voice model!");
            while (!_getCustomizationTested)
                yield return null;

            //  Update Customization
            Log.Debug("ExampleTextToSpeech", "Attempting to update a customization");
            Word[] wordsToUpdateCustomization =
            {
            new Word()
            {
                word = "hello",
                translation = "hullo"
            },
            new Word()
            {
                word = "goodbye",
                translation = "gbye"
            },
            new Word()
            {
                word = "hi",
                translation = "ohioooo"
            }
        };

            _customVoiceUpdate = new CustomVoiceUpdate()
            {
                words = wordsToUpdateCustomization,
                description = "My updated description",
                name = "My updated name"
            };

            if (!_textToSpeech.UpdateCustomization(OnUpdateCustomization, _createdCustomizationId, _customVoiceUpdate))
                Log.Debug("ExampleTextToSpeech", "Failed to update customization!");
            while (!_updateCustomizationTested)
                yield return null;

            //  Get Customization Words
            Log.Debug("ExampleTextToSpeech", "Attempting to get a customization's words");
            string customIdentifierToGetWords = "1476ea80-5355-4911-ac99-ba39162a2d34";
            if (!_textToSpeech.GetCustomizationWords(OnGetCustomizationWords, customIdentifierToGetWords))
                Log.Debug("ExampleTextToSpeech", "Failed to get {0} words!", customIdentifierToGetWords);
            while (!_getCustomizationWordsTested)
                yield return null;

            //  Add Customization Words
            Log.Debug("ExampleTextToSpeech", "Attempting to add words to a customization");
            Word[] wordArrayToAddToCustomization =
            {
            new Word()
            {
                word = "bananna",
                translation = "arange"
            },
            new Word()
            {
                word = "orange",
                translation = "gbye"
            },
            new Word()
            {
                word = "tomato",
                translation = "tomahto"
            }
        };

            Words wordsToAddToCustomization = new Words()
            {
                words = wordArrayToAddToCustomization
            };

            if (!_textToSpeech.AddCustomizationWords(OnAddCustomizationWords, _createdCustomizationId, wordsToAddToCustomization))
                Log.Debug("ExampleTextToSpeech", "Failed to add words to {0}!", _createdCustomizationId);
            while (!_addCustomizationWordsTested)
                yield return null;

            //  Get Customization Word
            Log.Debug("ExampleTextToSpeech", "Attempting to get the translation of a custom voice model's word.");
            string customIdentifierWord = wordsToUpdateCustomization[0].word;
            if (!_textToSpeech.GetCustomizationWord(OnGetCustomizationWord, _createdCustomizationId, customIdentifierWord))
                Log.Debug("ExampleTextToSpeech", "Failed to get the translation of {0} from {1}!", customIdentifierWord, _createdCustomizationId);
            while (!_getCustomizationWordTested)
                yield return null;

            //  Delete Customization Word
            Log.Debug("ExampleTextToSpeech", "Attempting to delete customization word from custom voice model.");
            string wordToDelete = "goodbye";
            if (!_textToSpeech.DeleteCustomizationWord(OnDeleteCustomizationWord, _createdCustomizationId, wordToDelete))
                Log.Debug("ExampleTextToSpeech", "Failed to delete {0} from {1}!", wordToDelete, _createdCustomizationId);
            while (!_deleteCustomizationWordTested)
                yield return null;

            //  Delete Customization
            Log.Debug("ExampleTextToSpeech", "Attempting to delete a customization");
            if (!_textToSpeech.DeleteCustomization(OnDeleteCustomization, _createdCustomizationId))
                Log.Debug("ExampleTextToSpeech", "Failed to delete custom voice model!");
            while (!_deleteCustomizationTested)
                yield return null;

            Log.Debug("ExampleTextToSpeech", "Text to Speech examples complete.");

            yield break;
        }

        void HandleToSpeechCallback(AudioClip clip, string customData)
        {
            PlayClip(clip);
        }

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

                _synthesizeTested = true;
            }
        }

        private void OnGetVoices(Voices voices, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get voices response: {0}", customData);
            Test(voices != null);
            _getVoicesTested = true;
        }

        private void OnGetVoice(Voice voice, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get voice  response: {0}", customData);
            Test(voice != null);
            _getVoiceTested = true;
        }

        private void OnGetPronunciation(Pronunciation pronunciation, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get pronunciation response: {0}", customData);
            Test(pronunciation != null);
            _getPronuciationTested = true;
        }

        private void OnGetCustomizations(Customizations customizations, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get customizations response: {0}", customData);
            Test(customizations != null);
            _getCustomizationsTested = true;
        }

        private void OnCreateCustomization(CustomizationID customizationID, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Create customization response: {0}", customData);
            _createdCustomizationId = customizationID.customization_id;
            Test(customizationID != null);
            _createCustomizationTested = true;
        }

        private void OnDeleteCustomization(bool success, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Delete customization response: {0}", customData);
            _createdCustomizationId = null;
            Test(success);
            _deleteCustomizationTested = true;
        }

        private void OnGetCustomization(Customization customization, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get customization response: {0}", customData);
            Test(customization != null);
            _getCustomizationTested = true;
        }

        private void OnUpdateCustomization(bool success, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Update customization response: {0}", customData);
            Test(success);
            _updateCustomizationTested = true;
        }

        private void OnGetCustomizationWords(Words words, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get customization words response: {0}", customData);
            Test(words != null);
            _getCustomizationWordsTested = true;
        }

        private void OnAddCustomizationWords(bool success, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Add customization words response: {0}", customData);
            Test(success);
            _addCustomizationWordsTested = true;
        }

        private void OnDeleteCustomizationWord(bool success, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Delete customization word response: {0}", customData);
            Test(success);
            _deleteCustomizationWordTested = true;
        }

        private void OnGetCustomizationWord(Translation translation, string customData)
        {
            Log.Debug("ExampleTextToSpeech", "Text to Speech - Get customization word response: {0}", customData);
            Test(translation != null);
            _getCustomizationWordTested = true;
        }
    }
}
