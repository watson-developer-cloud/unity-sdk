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
#pragma warning disable 0649

//  Uncomment to train a new classifier
//#define TRAIN_CLASSIFIER
//  Uncommnent to delete the trained classifier
//#define DELETE_TRAINED_CLASSIFIER

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;

public class ExampleVisualRecognition : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/visual-recognition/ap\"")]
    [SerializeField]
    private string _serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string _versionDate;
    [Header("IAM Authentication")]
    [Tooltip("The IAM apikey.")]
    [SerializeField]
    private string _iamApikey;
    #endregion

    private VisualRecognition _visualRecognition;

    private string _classifierID = "";
    private string _imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";
#if TRAIN_CLASSIFIER
    private string _coreMLDownloadPath = "";
#endif

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
#if TRAIN_CLASSIFIER
    private bool _getCoreMLModelTested = false;
    private bool _isClassifierReady = false;
#endif

    void Start()
    {
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
    }

    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(_iamApikey))
        {
            throw new WatsonException("Plesae provide IAM ApiKey for the service.");
        }

        Credentials credentials = null;

        //  Authenticate using iamApikey
        TokenOptions tokenOptions = new TokenOptions()
        {
            IamApiKey = _iamApikey
        };

        credentials = new Credentials(tokenOptions, _serviceUrl);

        //  Wait for tokendata
        while (!credentials.HasIamTokenData())
            yield return null;

        //  Create credential and instantiate service
        _visualRecognition = new VisualRecognition(credentials);
        _visualRecognition.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        //          Get all classifiers
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to get all classifiers");
        if (!_visualRecognition.GetClassifiersBrief(OnGetClassifiers, OnFail))
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
        if (!_visualRecognition.DetectFaces(_imageURL, OnDetectFacesGet, OnFail, "es"))
            Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

        while (!_detectFacesGetTested)
            yield return null;

        //          Detect faces post image
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to detect faces via image");
        string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
        if (!_visualRecognition.DetectFaces(OnDetectFacesPost, OnFail, faceExamplePath, "es"))
            Log.Debug("ExampleVisualRecognition.DetectFaces()", "Detect faces failed!");

        while (!_detectFacesPostTested)
            yield return null;

#if DELETE_TRAINED_CLASSIFIER
        Runnable.Run(IsClassifierReady(_classifierToDelete));
        while (!_isClassifierReady)
            yield return null;

        //  Download Core ML Model
        Log.Debug("ExampleVisualRecognition.RunTest()", "Attempting to get Core ML Model");
        if (!_visualRecognition.GetCoreMLModel(OnGetCoreMLModel, OnFail, _classifierID))
            Log.Debug("TestVisualRecognition.GetCoreMLModel()", "Failed to get core ml model!");
        while (!_getCoreMLModelTested)
            yield return null;

        //          Delete classifier by ID
        Log.Debug("ExampleVisualRecognition.Examples()", "Attempting to delete classifier");
        if (!_visualRecognition.DeleteClassifier(OnDeleteClassifier, OnFail, _classifierToDelete))
            Log.Debug("ExampleVisualRecognition.DeleteClassifier()", "Failed to delete classifier!");

        while (!_deleteClassifierTested)
            yield return null;
#endif

        Log.Debug("ExampleVisualRecognition.Examples()", "Visual Recogition tests complete");
    }

    private void OnGetClassifiers(ClassifiersBrief classifiers, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnGetClassifiers()", "VisualRecognition - GetClassifiers Response: {0}", customData["json"].ToString());

        _getClassifiersTested = true;
    }

#if DELETE_TRAINED_CLASSIFIER
    private void OnGetClassifier(ClassifierVerbose classifier, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnGetClassifier()", "VisualRecognition - GetClassifier Response: {0}", customData["json"].ToString());
        _getClassifierTested = true;
    }
#endif

#if DELETE_TRAINED_CLASSIFIER
    private void OnDeleteClassifier(bool success, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDeleteClassifier()", "{0}", success);
        _deleteClassifierTested = true;
    }
#endif

#if TRAIN_CLASSIFIER
    private void OnTrainClassifier(ClassifierVerbose classifier, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnTrainClassifier()", "{0}", customData["json"].ToString());

#if DELETE_TRAINED_CLASSIFIER
        _classifierToDelete = classifier.classifier_id;
#endif
        _classifierID = classifier.classifier_id;
        _trainClassifierTested = true;
    }
#endif

    private void OnClassifyGet(ClassifiedImages classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassifyGet()", "{0}", customData["json"].ToString());
        _classifyGetTested = true;

    }

    private void OnClassifyPost(ClassifiedImages classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassifyPost()", "{0}", customData["json"].ToString());
        _classifyPostTested = true;
    }

    private void OnDetectFacesGet(DetectedFaces multipleImages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDetectFacesGet()", "{0}", customData["json"].ToString());
        _detectFacesGetTested = true;
    }

    private void OnDetectFacesPost(DetectedFaces multipleImages, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnDetectFacesPost()", "{0}", customData["json"].ToString());
        _detectFacesPostTested = true;
    }

#if DELETE_TRAINED_CLASSIFIER
    private void OnGetCoreMLModel(byte[] resp, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnGetCoreMLModel()", "SUCCESS!");
        _getCoreMLModelTested = true;
    }
#endif

#if DELETE_TRAINED_CLASSIFIER
#region Is Classifier Ready
    //  Checking if classifier is ready before deletion due to a known bug in the Visual Recognition service where
    //  if a classifier is deleted before it is `ready` or `failed` the classifier will still exist in object storage
    //  but will be inaccessable to the user.
    private IEnumerator IsClassifierReady(string classifierId)
    {
        Log.Debug("TestVisualRecognition.IsClassifierReady()", "Checking if classifier is ready in 15 seconds...");

        yield return new WaitForSeconds(15f);

        Dictionary<string, object> customData = new Dictionary<string, object>();
        customData.Add("classifierId", classifierId);
        if (!_visualRecognition.GetClassifier(OnCheckIfClassifierIsReady, OnFailCheckingIfClassifierIsReady, classifierId))
            IsClassifierReady(classifierId);
    }

    private void OnCheckIfClassifierIsReady(ClassifierVerbose response, Dictionary<string, object> customData)
    {
        Log.Debug("TestVisualRecognition.IsClassifierReady()", "Classifier status is {0}", response.status);

        if (response.status == "ready" || response.status == "failed")
        {
            _isClassifierReady = true;
        }
        else
        {

            Runnable.Run(IsClassifierReady(response.classifier_id));
        }
    }
    private void OnFailCheckingIfClassifierIsReady(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        IsClassifierReady(_classifierToDelete);
    }
#endregion
#endif

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
    }
}
