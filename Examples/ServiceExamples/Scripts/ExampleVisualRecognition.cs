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
//  Uncommnent to delete the trained classifier
#define DELETE_TRAINED_CLASSIFIER

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

public class ExampleVisualRecognition : MonoBehaviour
{
    private string _apikey = null;
    private string _url = null;

    private VisualRecognition _visualRecognition;
    private string _visualRecognitionVersionDate = "2016-05-20";

    private string _classifierID = "";
    private string _imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";
    //private string _imageTextURL = "http://i.stack.imgur.com/ZS6nH.png";

#if DELETE_TRAINED_CLASSIFIER
    private string _classifierToDelete;
#endif

    private bool _getClassifiersTested = false;
#if TRAIN_CLASSIFIER
    private bool _trainClassifierTested = false;
    private bool _getClassifierTested = false;
#endif
#if DELETE_TRAINED_CLASSIFIER
    private bool _deleteClassifierTested = false;
#endif
    private bool _classifyGetTested = false;
    private bool _classifyPostTested = false;
    private bool _detectFacesGetTested = false;
    private bool _detectFacesPostTested = false;

    void Start()
    {
        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_apikey, _url);

        _visualRecognition = new VisualRecognition(credentials);
        _visualRecognition.VersionDate = _visualRecognitionVersionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //          Get all classifiers
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to get all classifiers");
        if (!_visualRecognition.GetClassifiers(OnGetClassifiers, OnFail))
            Log.Debug("ExampleVisualRecognition.GetClassifiers()", "Failed to get all classifiers!");

        while (!_getClassifiersTested)
            yield return null;

#if TRAIN_CLASSIFIER
        //          Train classifier
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to train classifier");
        string positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
        string negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
        Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
        positiveExamples.Add("giraffe", positiveExamplesPath);
        if (!_visualRecognition.TrainClassifier(OnTrainClassifier, OnFail, "unity-test-classifier-example", positiveExamples, negativeExamplesPath))
            Log.Debug("ExampleVisualRecognition.TrainClassifier()", "Failed to train classifier!");

        while (!_trainClassifierTested)
            yield return null;

        //          Find classifier by ID
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to find classifier by ID");
        if (!_visualRecognition.GetClassifier(OnGetClassifier, OnFail, _classifierID))
            Log.Debug("ExampleVisualRecognition.GetClassifier()", "Failed to get classifier!");

        while (!_getClassifierTested)
            yield return null;
#endif

        //          Classify get
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to get classify via URL");
        if (!_visualRecognition.Classify(_imageURL, OnClassifyGet, OnFail))
            Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");

        while (!_classifyGetTested)
            yield return null;

        //          Classify post image
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to classify via image on file system");
        string imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
        string[] owners = { "IBM", "me" };
        string[] classifierIDs = { "default", _classifierID };
        if (!_visualRecognition.Classify(OnClassifyPost, OnFail, imagesPath, owners, classifierIDs, 0.5f))
            Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");

        while (!_classifyPostTested)
            yield return null;

        //          Detect faces get
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to detect faces via URL");
        if (!_visualRecognition.DetectFaces(_imageURL, OnDetectFacesGet, OnFail))
            Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

        while (!_detectFacesGetTested)
            yield return null;

        //          Detect faces post image
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to detect faces via image");
        string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
        if (!_visualRecognition.DetectFaces(OnDetectFacesPost, OnFail, faceExamplePath))
            Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

        while (!_detectFacesPostTested)
            yield return null;
        
#if DELETE_TRAINED_CLASSIFIER
        #region Delay
        Runnable.Run(Delay(_delayTime));
        while (_isWaitingForDelay)
            yield return null;
        #endregion

        //          Delete classifier by ID
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to delete classifier");
        if (!_visualRecognition.DeleteClassifier(OnDeleteClassifier, OnFail, _classifierToDelete))
            Log.Debug("ExampleVisualRecognition.DeleteClassifier()", "Failed to delete classifier!");

        while (!_deleteClassifierTested)
            yield return null;
#endif

        Log.Debug("ExampleVisualRecognition.Examples()", "Visual Recogition tests complete");
    }

    private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnGetClassifiers()", "VisualRecognition - GetClassifiers Response: {0}", customData["json"].ToString());

        _getClassifiersTested = true;
    }

    private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnGetClassifier()", "VisualRecognition - GetClassifier Response: {0}", customData["json"].ToString());
        _getClassifierTested = true;
    }

#if DELETE_TRAINED_CLASSIFIER
    private void OnDeleteClassifier(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDeleteClassifier()", "{0}", success);
        _deleteClassifierTested = true;
    }
#endif

#if TRAIN_CLASSIFIER
    private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnTrainClassifier()", "{0}", customData["json"].ToString());

#if DELETE_TRAINED_CLASSIFIER
        _classifierToDelete = classifier.classifier_id;
#endif
        _classifierID = classifier.classifier_id;
        _trainClassifierTested = true;
    }
#endif

    private void OnClassifyGet(ClassifyTopLevelMultiple classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassifyGet()", "{0}", customData["json"].ToString());
        _classifyGetTested = true;

    }

    private void OnClassifyPost(ClassifyTopLevelMultiple classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassifyPost()", "{0}", customData["json"].ToString());
        _classifyPostTested = true;
    }

    private void OnDetectFacesGet(FacesTopLevelMultiple multipleImages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDetectFacesGet()", "{0}", customData["json"].ToString());
        _detectFacesGetTested = true;
    }

    private void OnDetectFacesPost(FacesTopLevelMultiple multipleImages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDetectFacesPost()", "{0}", customData["json"].ToString());
        _detectFacesPostTested = true;
    }

    #region Delay
    //  Introducing a delay because of a known issue with Visual Recognition where newly created classifiers 
    //  will disappear without being deleted if a delete is attempted less than ~10 seconds after creation.
    private float _delayTime = 15f;
    private bool _isWaitingForDelay = false;

    private IEnumerator Delay(float delayTime)
    {
        _isWaitingForDelay = true;
        Log.Debug("ExampleVisualRecognition.Delay()", "Delaying for {0} seconds....", delayTime);
        yield return new WaitForSeconds(delayTime);
        _isWaitingForDelay = false;
    }
    #endregion

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
    }
}
