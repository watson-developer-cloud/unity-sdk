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

//  Uncomment to train a new classifier
#define TRAIN_CLASSIFIER
//  Uncomment to delete the newley trained classifier
#define DELETE_TRAINED_CLASSIFIER

using System.Collections;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.IO;
using System;
using FullSerializer;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Connection;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestNaturalLanguageClassifier : UnitTest
    {
        private string _username = null;
        private string _password = null;
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private NaturalLanguageClassifier naturalLanguageClassifier;

        private string _classifierId = "";
        private List<string> _classifierIds = new List<string>();
        private string _inputString = "Is it hot outside?";

        private bool _areAnyClassifiersAvailable = false;
        private bool _getClassifiersTested = false;
        private bool _getClassifierTested = false;
#if TRAIN_CLASSIFIER
        private string _classifierName = "testClassifier";
        private bool _trainClassifierTested = false;
#endif
#if DELETE_TRAINED_CLASSIFIER
        private string _classifierToDelete;
#endif
        private bool _classifyTested = false;

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
                Credential credential = vcapCredentials.VCAP_SERVICES["natural_language_classifier"][TestCredentialIndex].Credentials;
                _username = credential.Username.ToString();
                _password = credential.Password.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestNaturalLanguageClassifier.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_username, _password, _url);

            //  Or authenticate using token
            //Credentials credentials = new Credentials(_url)
            //{
            //    AuthenticationToken = _token
            //};

            naturalLanguageClassifier = new NaturalLanguageClassifier(credentials);

            //  Get classifiers
            if (!naturalLanguageClassifier.GetClassifiers(OnGetClassifiers, OnFail))
                Log.Debug("ExampleNaturalLanguageClassifier.GetClassifiers()", "Failed to get classifiers!");

            while (!_getClassifiersTested)
                yield return null;

            if (_classifierIds.Count == 0)
                Log.Debug("ExampleNaturalLanguageClassifier.Examples()", "There are no trained classifiers. Please train a classifier...");

            if (_classifierIds.Count > 0)
            {
                //  Get each classifier
                foreach (string classifierId in _classifierIds)
                {
                    if (!naturalLanguageClassifier.GetClassifier(OnGetClassifier, OnFail, classifierId))
                        Log.Debug("ExampleNaturalLanguageClassifier.GetClassifier()", "Failed to get classifier {0}!", classifierId);
                }

                while (!_getClassifierTested)
                    yield return null;
            }

            if (!_areAnyClassifiersAvailable && _classifierIds.Count > 0)
                Log.Debug("ExampleNaturalLanguageClassifier.Examples()", "All classifiers are training...");

            //  Train classifier
#if TRAIN_CLASSIFIER
            string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/weather_data_train.csv";
            var trainingContent = File.ReadAllText(dataPath);
            if (!naturalLanguageClassifier.TrainClassifier(OnTrainClassifier, OnFail, _classifierName + "/" + DateTime.Now.ToString(), "en", trainingContent))
                Log.Debug("ExampleNaturalLanguageClassifier.TrainClassifier()", "Failed to train clasifier!");

            while (!_trainClassifierTested)
                yield return null;
#endif

#if DELETE_TRAINED_CLASSIFIER
            if (!string.IsNullOrEmpty(_classifierToDelete))
                if (!naturalLanguageClassifier.DeleteClassifer(OnDeleteTrainedClassifier, OnFail, _classifierToDelete))
                    Log.Debug("ExampleNaturalLanguageClassifier.DeleteClassifer()", "Failed to delete clasifier {0}!", _classifierToDelete);
#endif

            //  Classify
            if (_areAnyClassifiersAvailable)
            {
                if (!naturalLanguageClassifier.Classify(OnClassify, OnFail, _classifierId, _inputString))
                    Log.Debug("ExampleNaturalLanguageClassifier.Classify()", "Failed to classify!");

                while (!_classifyTested)
                    yield return null;
            }

            Log.Debug("TestNaturalLanguageClassifier.RunTest()", "Natural language classifier examples complete.");

            yield break;
        }

        private void OnGetClassifiers(Classifiers classifiers, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleNaturalLanguageClassifier.OnGetClassifiers()", "Natural Language Classifier - GetClassifiers  Response: {0}", customData["json"].ToString());

            foreach (Classifier classifier in classifiers.classifiers)
                _classifierIds.Add(classifier.classifier_id);
            Test(classifiers != null);
            _getClassifiersTested = true;
        }

        private void OnClassify(ClassifyResult result, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleNaturalLanguageClassifier.OnClassify()", "Natural Language Classifier - Classify Response: {0}", customData["json"].ToString());
            Test(result != null);
            _classifyTested = true;
        }

#if TRAIN_CLASSIFIER
        private void OnTrainClassifier(Classifier classifier, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleNaturalLanguageClassifier.OnTrainClassifier()", "Natural Language Classifier - Train Classifier: {0}", customData["json"].ToString());
#if DELETE_TRAINED_CLASSIFIER
            _classifierToDelete = classifier.classifier_id;
#endif
            Test(classifier != null);
            _trainClassifierTested = true;
        }
#endif

        private void OnGetClassifier(Classifier classifier, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleNaturalLanguageClassifier.OnGetClassifier()", "Natural Language Classifier - Get Classifier {0}: {1}", classifier.classifier_id, customData["json"].ToString());
            Test(classifier != null);

            //  Get any classifier that is available
            if (!string.IsNullOrEmpty(classifier.status) && classifier.status.ToLower() == "available")
            {
                _areAnyClassifiersAvailable = true;
                _classifierId = classifier.classifier_id;
            }

            if (classifier.classifier_id == _classifierIds[_classifierIds.Count - 1])
                _getClassifierTested = true;
        }

#if DELETE_TRAINED_CLASSIFIER
        private void OnDeleteTrainedClassifier(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("ExampleNaturalLanguageClassifier.OnDeleteTrainedClassifier()", "Natural Language Classifier - Delete Trained Classifier {0} | response: {1}", _classifierToDelete, customData["json"].ToString());
            Test(success);
        }
#endif

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("ExampleAlchemyLanguage.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}

