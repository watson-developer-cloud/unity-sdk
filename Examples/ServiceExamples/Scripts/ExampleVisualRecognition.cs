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

public class ExampleVisualRecognition : MonoBehaviour
{
    private string _apikey = null;
    private string _url = null;

    private VisualRecognition _visualRecognition;
    private string _visualRecognitionVersionDate = "2016-05-20";

    private string _classifierID = "swiftsdkunittestcarstrucks_128487308";
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
    //private bool _recognizeTextGetTested = false;
    //private bool _recognizeTextPostTested = false;

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
        Log.Debug("ExampleVisualRecognition", "Attempting to get all classifiers");
        if (!_visualRecognition.GetClassifiers(OnGetClassifiers))
            Log.Debug("ExampleVisualRecognition", "Failed to get all classifiers!");

        while (!_getClassifiersTested)
            yield return null;

#if TRAIN_CLASSIFIER
        //          Train classifier
        Log.Debug("ExampleVisualRecognition", "Attempting to train classifier");
        string positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
        string negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
        Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
        positiveExamples.Add("giraffe", positiveExamplesPath);
        if (!_visualRecognition.TrainClassifier(OnTrainClassifier, "unity-test-classifier-example", positiveExamples, negativeExamplesPath))
            Log.Debug("ExampleVisualRecognition", "Failed to train classifier!");

        while (!_trainClassifierTested)
            yield return null;

        //          Find classifier by ID
        Log.Debug("ExampleVisualRecognition", "Attempting to find classifier by ID");
        if (!_visualRecognition.GetClassifier(OnGetClassifier, _classifierID))
            Log.Debug("ExampleVisualRecognition", "Failed to get classifier!");

        while (!_getClassifierTested)
            yield return null;
#endif

#if DELETE_TRAINED_CLASSIFIER
        //          Delete classifier by ID
        Log.Debug("ExampleVisualRecognition", "Attempting to delete classifier");
        if (!_visualRecognition.DeleteClassifier(OnDeleteClassifier, _classifierToDelete))
            Log.Debug("ExampleVisualRecognition", "Failed to delete classifier!");
#endif

        while (!_deleteClassifierTested)
            yield return null;

        //          Classify get
        Log.Debug("ExampleVisualRecognition", "Attempting to get classify via URL");
        if (!_visualRecognition.Classify(OnClassifyGet, _imageURL))
            Log.Debug("ExampleVisualRecognition", "Classify image failed!");

        while (!_classifyGetTested)
            yield return null;

        //          Classify post image
        Log.Debug("ExampleVisualRecognition", "Attempting to classify via image on file system");
        string imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
        string[] owners = { "IBM", "me" };
        string[] classifierIDs = { "default", _classifierID };
        if (!_visualRecognition.Classify(imagesPath, OnClassifyPost, owners, classifierIDs, 0.5f))
            Log.Debug("ExampleVisualRecognition", "Classify image failed!");

        while (!_classifyPostTested)
            yield return null;

        //          Detect faces get
        Log.Debug("ExampleVisualRecognition", "Attempting to detect faces via URL");
        if (!_visualRecognition.DetectFaces(OnDetectFacesGet, _imageURL))
            Log.Debug("ExampleVisualRecogntiion", "Detect faces failed!");

        while (!_detectFacesGetTested)
            yield return null;

        //          Detect faces post image
        Log.Debug("ExampleVisualRecognition", "Attempting to detect faces via image");
        string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
        if (!_visualRecognition.DetectFaces(faceExamplePath, OnDetectFacesPost))
            Log.Debug("ExampleVisualRecognition", "Detect faces failed!");

        while (!_detectFacesPostTested)
            yield return null;

        ////          Recognize text get
        //Log.Debug("ExampleVisualRecognition", "Attempting to recognizeText via URL");
        //if (!_visualRecognition.RecognizeText(OnRecognizeTextGet, _imageTextURL))
        //    Log.Debug("ExampleVisualRecognition", "Recognize text failed!");

        //while (!_recognizeTextGetTested)
        //    yield return null;

        ////          Recognize text post image
        //Log.Debug("ExampleVisualRecognition", "Attempting to recognizeText via image");
        //string textExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/from_platos_apology.png";
        //if (!_visualRecognition.RecognizeText(textExamplePath, OnRecognizeTextPost))
        //    Log.Debug("ExampleVisualRecognition", "Recognize text failed!");

        //while (!_recognizeTextPostTested)
        //    yield return null;

        Log.Debug("ExampleVisualRecognition", "Visual Recogition tests complete");
    }

    private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - GetClassifiers Response: {0}", data);

        _getClassifiersTested = true;
    }

    private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - GetClassifier Response: {0}", data);
        _getClassifierTested = true;
    }

#if DELETE_TRAINED_CLASSIFIER
    private void OnDeleteClassifier(bool success, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - DeleteClassifier Response: {0}", success);
        _deleteClassifierTested = true;
    }
#endif

#if TRAIN_CLASSIFIER
    private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - TrainClassifier Response: {0}", data);

#if DELETE_TRAINED_CLASSIFIER
        _classifierToDelete = classifier.classifier_id;
#endif
        _trainClassifierTested = true;
    }
#endif

    private void OnClassifyGet(ClassifyTopLevelMultiple classify, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - ClassifyGet Response: {0}", data);
        _classifyGetTested = true;

    }

    private void OnClassifyPost(ClassifyTopLevelMultiple classify, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - ClassifyPost Response: {0}", data);
        _classifyPostTested = true;
    }

    private void OnDetectFacesGet(FacesTopLevelMultiple multipleImages, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - DetectFacesGet Response: {0}", data);
        _detectFacesGetTested = true;
    }

    private void OnDetectFacesPost(FacesTopLevelMultiple multipleImages, string data)
    {
        Log.Debug("ExampleVisualRecognition", "VisualRecognition - DetectFacesPost Response: {0}", data);
        _detectFacesPostTested = true;
    }

    //private void OnRecognizeTextGet(TextRecogTopLevelMultiple multipleImages, string data)
    //{
    //    Log.Debug("ExampleVisualRecognition", "VisualRecognition - RecognizeTextGet Response: {0}", data);
    //    _recognizeTextGetTested = true;
    //}

    //private void OnRecognizeTextPost(TextRecogTopLevelMultiple multipleImages, string data)
    //{
    //    Log.Debug("ExampleVisualRecognition", "VisualRecognition - RecognizeTextPost Response: {0}", data);
    //    _recognizeTextPostTested = true;
    //}
}
