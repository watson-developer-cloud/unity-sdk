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
//#define TRAIN_CLASSIFIER
//  Uncomment to delete the newley trained classifier
//#define DELETE_TRAINED_CLASSIFIER

using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections.Generic;
using System.Collections;

public class ExampleNaturalLanguageClassifier : MonoBehaviour
{
    private string _username = null;
    private string _password = null;
    private string _url = null;

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

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        naturalLanguageClassifier = new NaturalLanguageClassifier(credentials);

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //  Get classifiers
        if (!naturalLanguageClassifier.GetClassifiers(OnGetClassifiers))
            Log.Debug("ExampleNaturalLanguageClassifier", "Failed to get classifiers!");

        while (!_getClassifiersTested)
            yield return null;

        if (_classifierIds.Count == 0)
            Log.Debug("ExampleNaturalLanguageClassifier", "There are no trained classifiers. Please train a classifier...");

        if (_classifierIds.Count > 0)
        {
            //  Get each classifier
            foreach (string classifierId in _classifierIds)
            {
                if (!naturalLanguageClassifier.GetClassifier(classifierId, OnGetClassifier))
                    Log.Debug("ExampleNaturalLanguageClassifier", "Failed to get classifier {0}!", classifierId);
            }

            while (!_getClassifierTested)
                yield return null;
        }

        if (!_areAnyClassifiersAvailable && _classifierIds.Count > 0)
            Log.Debug("ExampleNaturalLanguageClassifier", "All classifiers are training...");

        //  Train classifier
#if TRAIN_CLASSIFIER
        string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/weather_data_train.csv";
        var trainingContent = File.ReadAllText(dataPath);
        if (!naturalLanguageClassifier.TrainClassifier(_classifierName + "/" + DateTime.Now.ToString(), "en", trainingContent, OnTrainClassifier))
            Log.Debug("ExampleNaturalLanguageClassifier", "Failed to train clasifier!");

        while (!_trainClassifierTested)
            yield return null;
#endif

#if DELETE_TRAINED_CLASSIFIER
        if (!string.IsNullOrEmpty(_classifierToDelete))
            if (!naturalLanguageClassifier.DeleteClassifer(_classifierToDelete, OnDeleteTrainedClassifier))
                Log.Debug("ExampleNaturalLanguageClassifier", "Failed to delete clasifier {0}!", _classifierToDelete);
#endif

        //  Classify
        if (_areAnyClassifiersAvailable)
        {
            if (!naturalLanguageClassifier.Classify(_classifierId, _inputString, OnClassify))
                Log.Debug("ExampleNaturalLanguageClassifier", "Failed to classify!");

            while (!_classifyTested)
                yield return null;
        }

        Log.Debug("ExampleNaturalLanguageClassifier", "Natural language classifier examples complete.");
    }

    private void OnGetClassifiers(Classifiers classifiers, string data)
    {
        Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - GetClassifiers  Response: {0}", data);

        foreach (Classifier classifier in classifiers.classifiers)
            _classifierIds.Add(classifier.classifier_id);

        _getClassifiersTested = true;
    }

    private void OnClassify(ClassifyResult result, string data)
    {
        Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Classify Response: {0}", data);
        _classifyTested = true;
    }

#if TRAIN_CLASSIFIER
    private void OnTrainClassifier(Classifier classifier, string data)
    {
        Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Train Classifier: {0}", data);
#if DELETE_TRAINED_CLASSIFIER
        _classifierToDelete = classifier.classifier_id;
#endif
        _trainClassifierTested = true;
    }
#endif

    private void OnGetClassifier(Classifier classifier, string data)
    {
        Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Get Classifier {0}: {1}", classifier.classifier_id, data);

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
    private void OnDeleteTrainedClassifier(bool success, string data)
    {
        Log.Debug("ExampleNaturalLanguageClassifier", "Natural Language Classifier - Delete Trained Classifier {0} | success: {1} {2}", _classifierToDelete, success, data);
    }
#endif
}
