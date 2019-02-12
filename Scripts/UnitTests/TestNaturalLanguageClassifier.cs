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
        private fsSerializer _serializer = new fsSerializer();
        //private string _token = "<authentication-token>";

        private NaturalLanguageClassifier naturalLanguageClassifier;

        private string _classifierId = "";
        private List<string> _classifierIds = new List<string>();
        private string _inputString = "Is it hot outside?";

        private bool _autoGetClassifiersTested = false;
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
        private bool _classifyCollectionTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            //  Test NaturalLanguageClassifier using loaded credentials
            NaturalLanguageClassifier autoNaturalLanguageClassifier = new NaturalLanguageClassifier();
            while (!autoNaturalLanguageClassifier.Credentials.HasIamTokenData())
                yield return null;
            autoNaturalLanguageClassifier.GetClassifiers(OnAutoGetClassifiers, OnFail);
            while (!_autoGetClassifiersTested)
                yield return null;

            VcapCredentials vcapCredentials = new VcapCredentials();
            fsData data = null;

            string result = null;
            string credentialsFilepath = "../sdk-credentials/credentials.json";

            //  Load credentials file if it exists. If it doesn't exist, don't run the tests.
            if (File.Exists(credentialsFilepath))
                result = File.ReadAllText(credentialsFilepath);
            else
                yield break;

            //  Add in a parent object because Unity does not like to deserialize root level collection types.
            result = Utility.AddTopLevelObjectToJson(result, "VCAP_SERVICES");

            //  Convert json to fsResult
            fsResult r = fsJsonParser.Parse(result, out data);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Convert fsResult to VcapCredentials
            object obj = vcapCredentials;
            r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
            if (!r.Succeeded)
                throw new WatsonException(r.FormattedMessages);

            //  Set credentials from imported credntials
            Credential credential = vcapCredentials.GetCredentialByname("natural-language-classifier-sdk")[0].Credentials;
            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey,
            };

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(tokenOptions, credential.Url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            naturalLanguageClassifier = new NaturalLanguageClassifier(credentials);

            //  Get classifiers
            if (!naturalLanguageClassifier.GetClassifiers(OnGetClassifiers, OnFail))
                Log.Debug("TestNaturalLanguageClassifier.GetClassifiers()", "Failed to get classifiers!");

            while (!_getClassifiersTested)
                yield return null;

            if (_classifierIds.Count == 0)
                Log.Debug("TestNaturalLanguageClassifier.Examples()", "There are no trained classifiers. Please train a classifier...");

            if (_classifierIds.Count > 0)
            {
                //  Get each classifier
                foreach (string classifierId in _classifierIds)
                {
                    if (!naturalLanguageClassifier.GetClassifier(OnGetClassifier, OnFail, classifierId))
                        Log.Debug("TestNaturalLanguageClassifier.GetClassifier()", "Failed to get classifier {0}!", classifierId);
                }

                while (!_getClassifierTested)
                    yield return null;
            }

            if (!_areAnyClassifiersAvailable && _classifierIds.Count > 0)
                Log.Debug("TestNaturalLanguageClassifier.Examples()", "All classifiers are training...");

            //  Train classifier
#if TRAIN_CLASSIFIER
            string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/weather_data_train.csv";
            var trainingContent = File.ReadAllText(dataPath);
            if (!naturalLanguageClassifier.TrainClassifier(OnTrainClassifier, OnFail, _classifierName + "/" + DateTime.Now.ToString(), "en", trainingContent))
                Log.Debug("TestNaturalLanguageClassifier.TrainClassifier()", "Failed to train clasifier!");

            while (!_trainClassifierTested)
                yield return null;
#endif

#if DELETE_TRAINED_CLASSIFIER
            if (!string.IsNullOrEmpty(_classifierToDelete))
                if (!naturalLanguageClassifier.DeleteClassifer(OnDeleteTrainedClassifier, OnFail, _classifierToDelete))
                    Log.Debug("TestNaturalLanguageClassifier.DeleteClassifer()", "Failed to delete clasifier {0}!", _classifierToDelete);
#endif

            //  Classify
            if (_areAnyClassifiersAvailable)
            {
                if (!naturalLanguageClassifier.Classify(OnClassify, OnFail, _classifierId, _inputString))
                    Log.Debug("TestNaturalLanguageClassifier.Classify()", "Failed to classify!");

                while (!_classifyTested)
                    yield return null;
            }

            //  Classify Collection
            ClassifyCollectionInput classifyCollectionInput = new ClassifyCollectionInput()
            {
                collection = new List<ClassifyInput>()
                {
                    new ClassifyInput()
                    {
                        text = "Is it hot outside?"
                    },
                    new ClassifyInput()
                    {
                        text = "Is it going to rain?"
                    }
                }
            };

            if (_areAnyClassifiersAvailable)
            {
                if (!naturalLanguageClassifier.ClassifyCollection(OnClassifyCollection, OnFail, _classifierId, classifyCollectionInput))
                    Log.Debug("TestNaturalLanguageClassifier.ClassifyCollection()", "Failed to classify!");

                while (!_classifyCollectionTested)
                    yield return null;
            }

            Log.Debug("TestNaturalLanguageClassifier.RunTest()", "Natural language classifier examples complete.");

            yield break;
        }

        private void OnAutoGetClassifiers(Classifiers response, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnAutoGetClassifiers()", "Natural Language Classifier - GetClassifiers  Response: {0}", customData["json"].ToString());
            Test(response.classifiers != null);
            _autoGetClassifiersTested = true;
        }

        private void OnGetClassifiers(Classifiers classifiers, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnGetClassifiers()", "Natural Language Classifier - GetClassifiers  Response: {0}", customData["json"].ToString());

            foreach (Classifier classifier in classifiers.classifiers)
                _classifierIds.Add(classifier.classifier_id);
            Test(classifiers != null);
            _getClassifiersTested = true;
        }

        private void OnClassify(ClassifyResult result, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnClassify()", "Natural Language Classifier - Classify Response: {0}", customData["json"].ToString());
            Test(result != null);
            _classifyTested = true;
        }

#if TRAIN_CLASSIFIER
        private void OnTrainClassifier(Classifier classifier, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnTrainClassifier()", "Natural Language Classifier - Train Classifier: {0}", customData["json"].ToString());
#if DELETE_TRAINED_CLASSIFIER
            _classifierToDelete = classifier.classifier_id;
#endif
            Test(classifier != null);
            _trainClassifierTested = true;
        }
#endif

        private void OnGetClassifier(Classifier classifier, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnGetClassifier()", "Natural Language Classifier - Get Classifier {0}: {1}", classifier.classifier_id, customData["json"].ToString());
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
            Log.Debug("TestNaturalLanguageClassifier.OnDeleteTrainedClassifier()", "Natural Language Classifier - Delete Trained Classifier {0} | response: {1}", _classifierToDelete, customData["json"].ToString());
            Test(success);
        }
#endif

        private void OnClassifyCollection(ClassificationCollection result, Dictionary<string, object> customData)
        {
            Log.Debug("TestNaturalLanguageClassifier.OnClassifyCollection()", "Natural Language Classifier - Classify Collection Response: {0}", customData["json"].ToString());
            Test(result != null);
            _classifyCollectionTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Error("TestNaturalLanguageClassifier.OnFail()", "Error received: {0}", error.ToString());
        }
    }
}

