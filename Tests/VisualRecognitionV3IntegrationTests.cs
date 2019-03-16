/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.VisualRecognition.V3;
using IBM.Watson.VisualRecognition.V3.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class VisualRecognitionV3IntegrationTests
    {
        private VisualRecognitionService service;
        private string versionDate = "2019-02-13";
        private Dictionary<string, object> customData;
        private Dictionary<string, string> customHeaders = new Dictionary<string, string>();
        private string giraffePositiveExamplesFilepath;
        private string turtlePositiveExamplesFilepath;
        private string negativeExamplesFilepath;
        private string giraffeImageFilepath;
        private string turtleImageFilepath;
        private string turtleImageContentType = "image/jpeg";
        private string obamaImageFilepath;
        private string obamaImageContentType = "image/jpeg";
        private string imageMetadataFilepath;
        private string classifierName = "unity-sdk-classifier-safe-to-delete";
        private string classifierId;
        private bool isClassifierReady = false;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            customHeaders.Add("X-Watson-Test", "1");

            giraffePositiveExamplesFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/giraffe_positive_examples.zip";
            turtlePositiveExamplesFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/turtle_positive_examples.zip";
            negativeExamplesFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/negative_examples.zip";
            giraffeImageFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/giraffe_to_classify.jpg";
            turtleImageFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/turtle_to_classify.jpg";
            obamaImageFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/obama.jpg";
            imageMetadataFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/imageMetadata.json";
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new VisualRecognitionService(versionDate);
            }

            while (!service.Credentials.HasIamTokenData())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            customData = new Dictionary<string, object>();
            customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);
        }

        #region Classify
        [UnityTest, Order(0)]
        public IEnumerator TestClassify()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to Classify...");
            ClassifiedImages classifyResponse = null;
            using (FileStream fs = File.OpenRead(turtleImageFilepath))
            {
                service.Classify(
                    callback: (DetailedResponse<ClassifiedImages> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Classify result: {0}", customResponseData["json"].ToString());
                        classifyResponse = response.Result;
                        Assert.IsNotNull(classifyResponse);
                        Assert.IsNotNull(classifyResponse.Images);
                        Assert.IsTrue(classifyResponse.Images.Count > 0);
                        Assert.IsNotNull(classifyResponse.Images[0].Classifiers);
                        Assert.IsTrue(classifyResponse.Images[0].Classifiers.Count > 0);
                        Assert.IsNull(error);
                    },
                    imagesFile: fs,
                    imagesFileContentType: turtleImageContentType,
                    customData: customData
                );

                while (classifyResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region DetectFaces
        [UnityTest, Order(1)]
        public IEnumerator TestDetectFaces()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to DetectFaces...");
            DetectedFaces detectFacesResponse = null;
            using (FileStream fs = File.OpenRead(obamaImageFilepath))
            {
                service.DetectFaces(
                    callback: (DetailedResponse<DetectedFaces> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "DetectFaces result: {0}", customResponseData["json"].ToString());
                        detectFacesResponse = response.Result;
                        Assert.IsNotNull(detectFacesResponse);
                        Assert.IsNotNull(detectFacesResponse.Images);
                        Assert.IsTrue(detectFacesResponse.Images.Count > 0);
                        Assert.IsNotNull(detectFacesResponse.Images[0].Faces);
                        Assert.IsTrue(detectFacesResponse.Images[0].Faces.Count > 0);
                        Assert.IsNull(error);
                    },
                    imagesFile: fs,
                    imagesFileContentType: obamaImageContentType,
                    customData: customData
                );

                while (detectFacesResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region CreateClassifier
        [UnityTest, Order(2)]
        public IEnumerator TestCreateClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to CreateClassifier...");
            Classifier createClassifierResponse = null;
            using (FileStream fs0 = File.OpenRead(giraffePositiveExamplesFilepath))
            {
                using (FileStream fs1 = File.OpenRead(negativeExamplesFilepath))
                {
                    Dictionary<string, FileStream> positiveExamples = new Dictionary<string, FileStream>();
                    positiveExamples.Add("giraffe_positive_examples", fs0);
                    service.CreateClassifier(
                        callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                        {
                            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "CreateClassifier result: {0}", customResponseData["json"].ToString());
                            createClassifierResponse = response.Result;
                            classifierId = createClassifierResponse.ClassifierId;
                            Assert.IsNotNull(createClassifierResponse);
                            Assert.IsNotNull(classifierId);
                            Assert.IsTrue(createClassifierResponse.Name == classifierName);
                            Assert.IsNotNull(createClassifierResponse.Classes);
                            Assert.IsTrue(createClassifierResponse.Classes.Count > 0);
                            Assert.IsTrue(createClassifierResponse.Classes[0].ClassName == "giraffe");
                            Assert.IsNull(error);
                        },
                        name: classifierName,
                        positiveExamples: positiveExamples,
                        negativeExamples: fs1,
                        customData: customData
                    );

                    while (createClassifierResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetClassifier
        [UnityTest, Order(3)]
        public IEnumerator TestGetClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to GetClassifier...");
            Classifier getClassifierResponse = null;
            service.GetClassifier(
                callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "GetClassifier result: {0}", customResponseData["json"].ToString());
                    getClassifierResponse = response.Result;
                    Assert.IsNotNull(getClassifierResponse);
                    Assert.IsTrue(getClassifierResponse.ClassifierId == classifierId);
                    Assert.IsNull(error);
                },
                classifierId: classifierId,
                customData: customData
            );

            while (getClassifierResponse == null)
                yield return null;
        }
        #endregion

        #region ListClassifiers
        [UnityTest, Order(4)]
        public IEnumerator TestListClassifiers()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to ListClassifiers...");
            Classifiers listClassifiersResponse = null;
            service.ListClassifiers(
                callback: (DetailedResponse<Classifiers> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "ListClassifiers result: {0}", customResponseData["json"].ToString());
                    listClassifiersResponse = response.Result;
                    Assert.IsNotNull(listClassifiersResponse);
                    Assert.IsNotNull(listClassifiersResponse._Classifiers);
                    Assert.IsTrue(listClassifiersResponse._Classifiers.Count > 0);
                    Assert.IsNull(error);
                },
                verbose: true,
                customData: customData
            );

            while (listClassifiersResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForClassifier
        [UnityTest, Order(5)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to WaitForClassifier...");
            isClassifierReady = false;
            Runnable.Run(CheckClassifierStatus());

            while (!isClassifierReady)
                yield return null;
        }
        #endregion

        #region UpdateClassifier
        [UnityTest, Order(6)]
        public IEnumerator TestUpdateClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to UpdateClassifier...");
            using (FileStream fs = File.OpenRead(turtlePositiveExamplesFilepath))
            {
                Classifier updateClassifierResponse = null;
                Dictionary<string, FileStream> positiveExamples = new Dictionary<string, FileStream>();
                positiveExamples.Add("turtles_positive_examples", fs);
                service.UpdateClassifier(
                    callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "UpdateClassifier result: {0}", customResponseData["json"].ToString());
                        updateClassifierResponse = response.Result;
                        Assert.IsNotNull(updateClassifierResponse);
                        Assert.IsNotNull(updateClassifierResponse.Classes);
                        Assert.IsTrue(updateClassifierResponse.Classes.Count > 0);
                        Assert.IsNull(error);
                    },
                    classifierId: classifierId,
                    positiveExamples: positiveExamples,
                    customData: customData
                );

                while (updateClassifierResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region WaitForClassifier
        [UnityTest, Order(7)]
        [Timeout(int.MaxValue)]
        public IEnumerator WaitForClassifier2()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to WaitForClassifier2...");
            isClassifierReady = false;
            Runnable.Run(CheckClassifierStatus());

            while (!isClassifierReady)
                yield return null;
        }
        #endregion

        #region GetCoreMlModel
        [UnityTest, Order(8)]
        public IEnumerator TestGetCoreMlModel()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to GetCoreMlModel...");
            byte[] getCoreMlModelResponse = null;
            service.GetCoreMlModel(
                callback: (DetailedResponse<byte[]> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    getCoreMlModelResponse = response.Result;
                    Assert.IsNotNull(getCoreMlModelResponse);
                    Assert.IsNull(error);

                    //  Save file
                    using (FileStream fs = File.Create(Application.dataPath + "/myModel.mlmodel"))
                    {
                        fs.Write(getCoreMlModelResponse, 0, getCoreMlModelResponse.Length);
                        fs.Close();
                    }
                },
                classifierId: classifierId,
                customData: customData
            );

            while (getCoreMlModelResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteClassifier
        [UnityTest, Order(98)]
        public IEnumerator TestDeleteClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to DeleteClassifier...");
            bool isComplete = false;
            service.DeleteClassifier(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "DeleteClassifier result: {0}", customResponseData["json"].ToString());
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                classifierId: classifierId,
                customData: customData
            );

            while (!isComplete)
                yield return null;
        }
        #endregion

        #region DeleteUserData
        [UnityTest, Order(99)]
        public IEnumerator TestDeleteUserData()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to DeleteUserData...");
            object deleteUserDataResponse = null;
            service.DeleteUserData(
                callback: (DetailedResponse<object> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "DeleteUserData result: {0}", customResponseData["json"].ToString());
                    deleteUserDataResponse = response.Result;
                    Assert.IsNotNull(deleteUserDataResponse);
                    Assert.IsNull(error);
                },
                customerId: "customerId",
                customData: customData
            );

            while (deleteUserDataResponse == null)
                yield return null;
        }
        #endregion

        #region CheckClassifierStatus
        private IEnumerator CheckClassifierStatus()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to CheckClassifierStatus in 15 seconds...");
            yield return new WaitForSeconds(15f);

            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to GetClassifier...");
            Classifier getClassifierResponse = null;
            try
            {
                service.GetClassifier(
                    callback: (DetailedResponse<Classifier> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        getClassifierResponse = response.Result;
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "CheckClassifierStatus: {0}", getClassifierResponse.Status);
                        if (getClassifierResponse.Status == Classifier.StatusValue.READY || getClassifierResponse.Status == Classifier.StatusValue.FAILED)
                        {
                            isClassifierReady = true;
                        }
                        else
                        {
                            Runnable.Run(CheckClassifierStatus());
                        }
                    },
                    classifierId: classifierId,
                    customData: customData
                );
            }
            catch
            {
                Runnable.Run(CheckClassifierStatus());
            }

            while (getClassifierResponse == null)
                yield return null;
        }
        #endregion
    }
}
