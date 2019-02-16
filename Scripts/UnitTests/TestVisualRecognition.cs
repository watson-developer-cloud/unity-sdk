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
#define TRAIN_DELETE_CLASSIFIER
//  Uncomment to test RC
#define TEST_RC

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using FullSerializer;
using System.IO;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using System;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
#if TEST_RC
    public class TestVisualRecognition : UnitTest
    {
        private fsSerializer _serializer = new fsSerializer();

        private VisualRecognition _visualRecognition;
        private string _visualRecognitionVersionDate = "2018-03-19";

        private string _classifierID = "";
        private string _imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";

#if TRAIN_DELETE_CLASSIFIER
        private string _classifierToDelete;
#endif

        private bool _getClassifiersTested = false;
#if TRAIN_DELETE_CLASSIFIER
        private bool _trainClassifierTested = false;
        private bool _getClassifierTested = false;
        private bool _getCoreMLModelTested = false;
#endif
#if TRAIN_DELETE_CLASSIFIER
        private bool _deleteClassifierTested = false;
        private bool _isClassifierReady = false;
#endif
        private bool _classifyGetTested = false;
        private bool _classifyPostTested = false;
        private bool _detectFacesGetTested = false;
        private bool _detectFacesPostTested = false;
        private bool _autoGetClassifiersTested = false;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            //  Test VisualRecognition using loaded credentials
            VisualRecognition autoVisualRecognition = new VisualRecognition();
            autoVisualRecognition.VersionDate = _visualRecognitionVersionDate;
            while (!autoVisualRecognition.Credentials.HasIamTokenData())
                yield return null;
            autoVisualRecognition.GetClassifiersBrief(OnAutoGetClassifiers, OnFail);
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
            Credential credential = vcapCredentials.GetCredentialByname("visual-recognition-sdk")[0].Credentials;

            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey,
            };

            Credentials credentials = new Credentials(tokenOptions, credential.Url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            _visualRecognition = new VisualRecognition(credentials);
            _visualRecognition.VersionDate = _visualRecognitionVersionDate;

            //          Get all classifiers
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to get all classifiers");
            if (!_visualRecognition.GetClassifiersBrief(OnGetClassifiers, OnFail))
                Log.Debug("TestVisualRecognition.GetClassifiers()", "Failed to get all classifiers!");

            while (!_getClassifiersTested)
                yield return null;

#if TRAIN_DELETE_CLASSIFIER
            _isClassifierReady = false;
            //          Train classifier
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to train classifier");
            string positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
            string negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
            Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
            positiveExamples.Add("giraffe", positiveExamplesPath);
            if (!_visualRecognition.TrainClassifier(OnTrainClassifier, OnFail, "unity-test-classifier-ok-to-delete", positiveExamples, negativeExamplesPath))
                Log.Debug("TestVisualRecognition.TrainClassifier()", "Failed to train classifier!");

            while (!_trainClassifierTested)
                yield return null;

            //          Find classifier by ID
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to find classifier by ID");
            if (!_visualRecognition.GetClassifier(OnGetClassifier, OnFail, _classifierID))
                Log.Debug("TestVisualRecognition.GetClassifier()", "Failed to get classifier!");

            while (!_getClassifierTested)
                yield return null;
#endif

            //  Classify get
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to get classify via URL");
            if (!_visualRecognition.Classify(_imageURL, OnClassifyGet, OnFail))
                Log.Debug("TestVisualRecognition.Classify()", "Classify image failed!");

            while (!_classifyGetTested)
                yield return null;

            //  Classify post image
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to classify via image on file system");
            string imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
            string[] owners = { "IBM", "me" };
            string[] classifierIDs = { "default", _classifierID };
            if (!_visualRecognition.Classify(OnClassifyPost, OnFail, imagesPath, owners, classifierIDs, 0.5f))
                Log.Debug("TestVisualRecognition.Classify()", "Classify image failed!");

            while (!_classifyPostTested)
                yield return null;

            //  Detect faces get
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to detect faces via URL");
            if (!_visualRecognition.DetectFaces(_imageURL, OnDetectFacesGet, OnFail, "es"))
                Log.Debug("TestVisualRecognition.DetectFaces()", "Detect faces failed!");

            while (!_detectFacesGetTested)
                yield return null;

            //  Detect faces post image
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to detect faces via image");
            string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
            if (!_visualRecognition.DetectFaces(OnDetectFacesPost, OnFail, faceExamplePath, "es"))
                Log.Debug("TestVisualRecognition.DetectFaces()", "Detect faces failed!");

            while (!_detectFacesPostTested)
                yield return null;

#if TRAIN_DELETE_CLASSIFIER
            Runnable.Run(IsClassifierReady(_classifierToDelete));
            while (!_isClassifierReady)
                yield return null;

            //  Download Core ML Model
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to get Core ML Model");
            if (!_visualRecognition.GetCoreMLModel(OnGetCoreMLModel, OnFail, _classifierID))
                Log.Debug("TestVisualRecognition.GetCoreMLModel()", "Failed to get core ml model!");
            while (!_getCoreMLModelTested)
                yield return null;

            //  Delete classifier by ID
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to delete classifier");
            if (!_visualRecognition.DeleteClassifier(OnDeleteClassifier, OnFail, _classifierToDelete))
                Log.Debug("TestVisualRecognition.DeleteClassifier()", "Failed to delete classifier!");

            while (!_deleteClassifierTested)
                yield return null;
#endif

            Log.Debug("TestVisualRecognition.RunTest()", "Visual Recogition tests complete");
            yield break;
        }

        private void OnAutoGetClassifiers(ClassifiersBrief response, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnAutoGetClassifiers()", "VisualRecognition - GetClassifiers Response: {0}", customData["json"].ToString());
            Test(response.classifiers != null);
            _autoGetClassifiersTested = true;
        }

        private void OnGetClassifiers(ClassifiersBrief classifiers, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnGetClassifiers()", "VisualRecognition - GetClassifiers Response: {0}", customData["json"].ToString());
            Test(classifiers != null);
            _getClassifiersTested = true;
        }

#if TRAIN_DELETE_CLASSIFIER
        private void OnGetClassifier(ClassifierVerbose classifier, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnGetClassifier()", "VisualRecognition - GetClassifier Response: {0}", customData["json"].ToString());
            Test(classifier != null);
            _getClassifierTested = true;
        }

        private void OnDeleteClassifier(bool success, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnDeleteClassifier()", "VisualRecognition - DeleteClassifier Response: {0}", success);
            Test(success);
            _deleteClassifierTested = true;
        }

        private void OnTrainClassifier(ClassifierVerbose classifier, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnTrainClassifier()", "VisualRecognition - TrainClassifier Response: {0}", customData["json"].ToString());

            _classifierToDelete = classifier.classifier_id;

            _classifierID = classifier.classifier_id;
            Test(classifier != null);
            _trainClassifierTested = true;
        }
#endif

        private void OnClassifyGet(ClassifiedImages classify, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnClassifyGet()", "VisualRecognition - ClassifyGet Response: {0}", customData["json"].ToString());
            Test(classify != null);
            _classifyGetTested = true;

        }

        private void OnClassifyPost(ClassifiedImages classify, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnClassifyPost()", "VisualRecognition - ClassifyPost Response: {0}", customData["json"].ToString());
            Test(classify != null);
            _classifyPostTested = true;
        }

        private void OnDetectFacesGet(DetectedFaces multipleImages, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnDetectFacesGet()", "VisualRecognition - DetectFacesGet Response: {0}", customData["json"].ToString());
            Test(multipleImages != null);
            Test(multipleImages.images[0].faces[0].gender.GenderLabel == "macho");
            _detectFacesGetTested = true;
        }

        private void OnDetectFacesPost(DetectedFaces multipleImages, Dictionary<string, object> customData)
        {
            Log.Debug("TestVisualRecognition.OnDetectFacesPost()", "VisualRecognition - DetectFacesPost Response: {0}", customData["json"].ToString());
            Test(multipleImages != null);
            Test(multipleImages.images[0].faces[0].gender.GenderLabel == "macho");
            _detectFacesPostTested = true;
        }

#if TRAIN_DELETE_CLASSIFIER
        private void OnGetCoreMLModel(byte[] resp, Dictionary<string, object> customData)
        {
            Test(resp != null);
            _getCoreMLModelTested = true;
        }

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
            Log.Error("TestVisualRecognition.OnFail()", "Error received: {0}", error.ToString());
        }
    }
#endif
}
