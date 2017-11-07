﻿/**
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
using IBM.Watson.DeveloperCloud.Connection;
using FullSerializer;
using System;
using System.IO;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestVisualRecognition : UnitTest
    {
        private string _apikey;
        private fsSerializer _serializer = new fsSerializer();

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
        //private bool _recognizeTextGetTested = false;
        //private bool _recognizeTextPostTested = false;

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
                Credential credential = vcapCredentials.VCAP_SERVICES["visual_recognition"][TestCredentialIndex].Credentials;
                _apikey = credential.Apikey.ToString();
                _url = credential.Url.ToString();
            }
            catch
            {
                Log.Debug("TestVisualRecognition.RunTest()", "Failed to get credentials from VCAP_SERVICES file. Please configure credentials to run this test. For more information, see: https://github.com/watson-developer-cloud/unity-sdk/#authentication");
            }

            //  Create credential and instantiate service
            Credentials credentials = new Credentials(_apikey, _url);

            _visualRecognition = new VisualRecognition(credentials);
            _visualRecognition.VersionDate = _visualRecognitionVersionDate;

            //          Get all classifiers
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to get all classifiers");
            if (!_visualRecognition.GetClassifiers(OnGetClassifiers))
                Log.Debug("TestVisualRecognition.GetClassifiers()", "Failed to get all classifiers!");

            while (!_getClassifiersTested)
                yield return null;

#if TRAIN_CLASSIFIER
            //          Train classifier
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to train classifier");
            string positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
            string negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
            Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
            positiveExamples.Add("giraffe", positiveExamplesPath);
            if (!_visualRecognition.TrainClassifier(OnTrainClassifier, "unity-test-classifier-example", positiveExamples, negativeExamplesPath))
                Log.Debug("TestVisualRecognition.TrainClassifier()", "Failed to train classifier!");

            while (!_trainClassifierTested)
                yield return null;

            //          Find classifier by ID
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to find classifier by ID");
            if (!_visualRecognition.GetClassifier(OnGetClassifier, _classifierID))
                Log.Debug("TestVisualRecognition.GetClassifier()", "Failed to get classifier!");

            while (!_getClassifierTested)
                yield return null;
#endif

            //          Classify get
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to get classify via URL");
            if (!_visualRecognition.Classify(OnClassifyGet, _imageURL))
                Log.Debug("TestVisualRecognition.Classify()", "Classify image failed!");

            while (!_classifyGetTested)
                yield return null;

            //          Classify post image
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to classify via image on file system");
            string imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
            string[] owners = { "IBM", "me" };
            string[] classifierIDs = { "default", _classifierID };
            if (!_visualRecognition.Classify(imagesPath, OnClassifyPost, owners, classifierIDs, 0.5f))
                Log.Debug("TestVisualRecognition.Classify()", "Classify image failed!");

            while (!_classifyPostTested)
                yield return null;

            //          Detect faces get
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to detect faces via URL");
            if (!_visualRecognition.DetectFaces(OnDetectFacesGet, _imageURL))
                Log.Debug("TestVisualRecognition.DetectFaces()", "Detect faces failed!");

            while (!_detectFacesGetTested)
                yield return null;

            //          Detect faces post image
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to detect faces via image");
            string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
            if (!_visualRecognition.DetectFaces(faceExamplePath, OnDetectFacesPost))
                Log.Debug("TestVisualRecognition.DetectFaces()", "Detect faces failed!");

            while (!_detectFacesPostTested)
                yield return null;

            ////          Recognize text get
            //Log.Debug("TestVisualRecognition.RunTest()", "Attempting to recognizeText via URL");
            //if (!_visualRecognition.RecognizeText(OnRecognizeTextGet, _imageTextURL))
            //    Log.Debug("TestVisualRecognition.RecognizeText()", "Recognize text failed!");

            //while (!_recognizeTextGetTested)
            //    yield return null;

            ////          Recognize text post image
            //Log.Debug("TestVisualRecognition.RunTest()", "Attempting to recognizeText via image");
            //string textExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/from_platos_apology.png";
            //if (!_visualRecognition.RecognizeText(textExamplePath, OnRecognizeTextPost))
            //    Log.Debug("TestVisualRecognition.RecognizeText()", "Recognize text failed!");

            //while (!_recognizeTextPostTested)
            //    yield return null;

#if DELETE_TRAINED_CLASSIFIER
            #region Delay
            Runnable.Run(Delay(_delayTime));
            while (_isWaitingForDelay)
                yield return null;
            #endregion

            //          Delete classifier by ID
            Log.Debug("TestVisualRecognition.RunTest()", "Attempting to delete classifier");
            if (!_visualRecognition.DeleteClassifier(OnDeleteClassifier, _classifierToDelete))
                Log.Debug("TestVisualRecognition.DeleteClassifier()", "Failed to delete classifier!");

            while (!_deleteClassifierTested)
                yield return null;
#endif

            Log.Debug("TestVisualRecognition.RunTest()", "Visual Recogition tests complete");
            yield break;
        }

        private void OnGetClassifiers(RESTConnector.ParsedResponse<GetClassifiersTopLevelBrief> resp)
        {
            Log.Debug("TestVisualRecognition.OnGetClassifiers()", "VisualRecognition - GetClassifiers Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getClassifiersTested = true;
        }

        private void OnGetClassifier(RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> resp)
        {
            Log.Debug("TestVisualRecognition.OnGetClassifier()", "VisualRecognition - GetClassifier Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _getClassifierTested = true;
        }

#if DELETE_TRAINED_CLASSIFIER
        private void OnDeleteClassifier(RESTConnector.ParsedResponse<object> resp)
        {
            Log.Debug("TestVisualRecognition.OnDeleteClassifier()", "VisualRecognition - DeleteClassifier Response: {0}", resp.Success);
            Test(resp.Success);
            _deleteClassifierTested = true;
        }
#endif

#if TRAIN_CLASSIFIER
        private void OnTrainClassifier(RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> resp)
        {
            Log.Debug("TestVisualRecognition.OnTrainClassifier()", "VisualRecognition - TrainClassifier Response: {0}", resp.JSON);

#if DELETE_TRAINED_CLASSIFIER
            _classifierToDelete = resp.DataObject.classifier_id;
#endif
            _classifierID = resp.DataObject.classifier_id;
            Test(resp.DataObject != null);
            _trainClassifierTested = true;
        }
#endif

        private void OnClassifyGet(RESTConnector.ParsedResponse<ClassifyTopLevelMultiple> resp)
        {
            Log.Debug("TestVisualRecognition.OnClassifyGet()", "VisualRecognition - ClassifyGet Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _classifyGetTested = true;

        }

        private void OnClassifyPost(RESTConnector.ParsedResponse<ClassifyTopLevelMultiple> resp)
        {
            Log.Debug("TestVisualRecognition.OnClassifyPost()", "VisualRecognition - ClassifyPost Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _classifyPostTested = true;
        }

        private void OnDetectFacesGet(RESTConnector.ParsedResponse<FacesTopLevelMultiple> resp)
        {
            Log.Debug("TestVisualRecognition.OnDetectFacesGet()", "VisualRecognition - DetectFacesGet Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _detectFacesGetTested = true;
        }

        private void OnDetectFacesPost(RESTConnector.ParsedResponse<FacesTopLevelMultiple> resp)
        {
            Log.Debug("TestVisualRecognition.OnDetectFacesPost()", "VisualRecognition - DetectFacesPost Response: {0}", resp.JSON);
            Test(resp.DataObject != null);
            _detectFacesPostTested = true;
        }

        //private void OnRecognizeTextGet(RESTConnector.ParsedResponse<TextRecogTopLevelMultiple> resp)
        //{
        //    Log.Debug("TestVisualRecognition.OnRecognizeTextGet()", "VisualRecognition - RecognizeTextGet Response: {0}", resp.JSON);
        //    _recognizeTextGetTested = true;
        //}

        //private void OnRecognizeTextPost(RESTConnector.ParsedResponse<TextRecogTopLevelMultiple> resp)
        //{
        //    Log.Debug("TestVisualRecognition.OnRecognizeTextPost()", "VisualRecognition - RecognizeTextPost Response: {0}", resp.JSON);
        //    _recognizeTextPostTested = true;
        //}

        #region Delay
        //  Introducing a delay because of a known issue with Visual Recognition where newly created classifiers 
        //  will disappear without being deleted if a delete is attempted less than ~10 seconds after creation.
        private float _delayTime = 15f;
        private bool _isWaitingForDelay = false;

        private IEnumerator Delay(float delayTime)
        {
            _isWaitingForDelay = true;
            Log.Debug("TestVisualRecognition.Delay()", "Delaying for {0} seconds....", delayTime);
            yield return new WaitForSeconds(delayTime);
            _isWaitingForDelay = false;
        }
        #endregion
    }
}
