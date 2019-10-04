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
using IBM.Cloud.SDK.Authentication;
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
        private string giraffePositiveExamplesFilepath;
        private string turtlePositiveExamplesFilepath;
        private string negativeExamplesFilepath;
        private string giraffeImageFilepath;
        private string turtleImageFilepath;
        private string turtleImageContentType = "image/jpeg";
        private string obamaImageFilepath;
        private string imageMetadataFilepath;
        private string classifierName = "unity-sdk-classifier-safe-to-delete";
        private string classifierId;
        private bool isClassifierReady = false;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();

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

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region CreateClassifier
        //[UnityTest, Order(0)]
        public IEnumerator TestCreateClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to CreateClassifier...");
            Classifier createClassifierResponse = null;
            using (FileStream fs0 = File.OpenRead(giraffePositiveExamplesFilepath))
            {
                using (FileStream fs1 = File.OpenRead(negativeExamplesFilepath))
                {
                    using (MemoryStream ms0 = new MemoryStream())
                    {
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            fs0.CopyTo(ms0);
                            fs1.CopyTo(ms1);
                            Dictionary<string, MemoryStream> positiveExamples = new Dictionary<string, MemoryStream>();
                            positiveExamples.Add("giraffe", ms0);
                            service.CreateClassifier(
                                callback: (DetailedResponse<Classifier> response, IBMError error) =>
                                {
                                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "CreateClassifier result: {0}", response.Response);
                                    createClassifierResponse = response.Result;
                                    classifierId = createClassifierResponse.ClassifierId;
                                    Assert.IsNotNull(createClassifierResponse);
                                    Assert.IsNotNull(classifierId);
                                    Assert.IsTrue(createClassifierResponse.Name == classifierName);
                                    Assert.IsNotNull(createClassifierResponse.Classes);
                                    Assert.IsTrue(createClassifierResponse.Classes.Count > 0);
                                    Assert.IsTrue(createClassifierResponse.Classes[0]._Class == "giraffe");
                                    Assert.IsNull(error);
                                },
                                name: classifierName,
                                positiveExamples: positiveExamples,
                                negativeExamples: ms1,
                                negativeExamplesFilename: Path.GetFileName(negativeExamplesFilepath)
                            );

                            while (createClassifierResponse == null)
                                yield return null;
                        }
                    }
                }
            }
        }
        #endregion

        #region Classify
        [UnityTest, Order(1)]
        public IEnumerator TestClassify()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to Classify...");
            ClassifiedImages classifyResponse = null;
            using (FileStream fs = File.OpenRead(turtleImageFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.Classify(
                    callback: (DetailedResponse<ClassifiedImages> response, IBMError error) =>
                    {
                        Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Classify result: {0}", response.Response);
                        classifyResponse = response.Result;
                        Assert.IsNotNull(classifyResponse);
                        Assert.IsNotNull(classifyResponse.Images);
                        Assert.IsTrue(classifyResponse.Images.Count > 0);
                        Assert.IsNotNull(classifyResponse.Images[0].Classifiers);
                        Assert.IsTrue(classifyResponse.Images[0].Classifiers.Count > 0);
                        Assert.IsNull(error);
                    },
                    imagesFile: ms,
                    imagesFileContentType: turtleImageContentType,
                    imagesFilename: Path.GetFileName(turtleImageFilepath)
                );

                    while (classifyResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region GetClassifier
        //[UnityTest, Order(3)]
        public IEnumerator TestGetClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to GetClassifier...");
            Classifier getClassifierResponse = null;
            service.GetClassifier(
                callback: (DetailedResponse<Classifier> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "GetClassifier result: {0}", response.Response);
                    getClassifierResponse = response.Result;
                    Assert.IsNotNull(getClassifierResponse);
                    Assert.IsTrue(getClassifierResponse.ClassifierId == classifierId);
                    Assert.IsNull(error);
                },
                classifierId: classifierId
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
                callback: (DetailedResponse<Classifiers> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "ListClassifiers result: {0}", response.Response);
                    listClassifiersResponse = response.Result;
                    Assert.IsNotNull(listClassifiersResponse);
                    Assert.IsNotNull(listClassifiersResponse._Classifiers);
                    Assert.IsTrue(listClassifiersResponse._Classifiers.Count > 0);
                    Assert.IsNull(error);
                },
                verbose: true
            );

            while (listClassifiersResponse == null)
                yield return null;
        }
        #endregion

        #region WaitForClassifier
        //[UnityTest, Order(5)]
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
        //[UnityTest, Order(6)]
        public IEnumerator TestUpdateClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to UpdateClassifier...");
            using (FileStream fs = File.OpenRead(turtlePositiveExamplesFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    Classifier updateClassifierResponse = null;
                    Dictionary<string, MemoryStream> positiveExamples = new Dictionary<string, MemoryStream>();
                    positiveExamples.Add("turtles_positive_examples", ms);
                    service.UpdateClassifier(
                        callback: (DetailedResponse<Classifier> response, IBMError error) =>
                        {
                            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "UpdateClassifier result: {0}", response.Response);
                            updateClassifierResponse = response.Result;
                            Assert.IsNotNull(updateClassifierResponse);
                            Assert.IsNotNull(updateClassifierResponse.Classes);
                            Assert.IsTrue(updateClassifierResponse.Classes.Count > 0);
                            Assert.IsNull(error);
                        },
                        classifierId: classifierId,
                        positiveExamples: positiveExamples
                    );

                    while (updateClassifierResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region WaitForClassifier
        //[UnityTest, Order(7)]
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
        //[UnityTest, Order(8)]
        public IEnumerator TestGetCoreMlModel()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to GetCoreMlModel...");
            byte[] getCoreMlModelResponse = null;
            service.GetCoreMlModel(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
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
                classifierId: classifierId
            );

            while (getCoreMlModelResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteClassifier
        //[UnityTest, Order(98)]
        public IEnumerator TestDeleteClassifier()
        {
            Log.Debug("VisualRecognitionServiceV3IntegrationTests", "Attempting to DeleteClassifier...");
            bool isComplete = false;
            service.DeleteClassifier(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "DeleteClassifier result: {0}", response.Response);
                    Assert.IsTrue(response.StatusCode == 200);
                    Assert.IsNull(error);
                    isComplete = true;
                },
                classifierId: classifierId
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
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV3IntegrationTests", "DeleteUserData result: {0}", response.Response);
                    deleteUserDataResponse = response.Result;
                    Assert.IsNotNull(deleteUserDataResponse);
                    Assert.IsNull(error);
                },
                customerId: "customerId"
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
                    callback: (DetailedResponse<Classifier> response, IBMError error) =>
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
                    classifierId: classifierId
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
