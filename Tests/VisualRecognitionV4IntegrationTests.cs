/**
* (C) Copyright IBM Corp. 2018, 2020.
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
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Model;
using IBM.Watson.VisualRecognition.V4;
using IBM.Watson.VisualRecognition.V4.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class VisualRecognitionV4IntegrationTests
    {
        private VisualRecognitionService service;
        private string versionDate = "2019-02-13";
        private string negativeExamplesFilepath;
        private string giraffeImageFilepath;
        private string turtleImageFilepath;
        private string imageMetadataFilepath;
        private string giraffeCollectionId = "a06f7036-0529-49ee-bdf6-82ddec276923";
        private string giraffeClassname = "giraffe";
        private string collectionId;
        private string imageId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();

            negativeExamplesFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/negative_examples.zip";
            giraffeImageFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/giraffe_to_classify.jpg";
            turtleImageFilepath = Application.dataPath + "/Watson/Tests/TestData/VisualRecognitionV3/turtle_to_classify.jpg";
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

        #region Analyze
        [UnityTest, Order(1)]
        public IEnumerator TestAnalyze()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Analyze...");
            AnalyzeResponse analyzeResponse = null;
            List<FileWithMetadata> imagesFile = new List<FileWithMetadata>();
            using (FileStream fs1 = File.OpenRead(giraffeImageFilepath), fs2 = File.OpenRead(turtleImageFilepath))
            {
                using (MemoryStream ms1 = new MemoryStream(), ms2 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    fs2.CopyTo(ms2);
                    FileWithMetadata fileWithMetadata = new FileWithMetadata()
                    {
                        Data = ms1,
                        ContentType = "image/jpeg",
                        Filename = Path.GetFileName(giraffeImageFilepath)
                    };
                    imagesFile.Add(fileWithMetadata);

                    FileWithMetadata fileWithMetadata2 = new FileWithMetadata()
                    {
                        Data = ms2,
                        ContentType = "image/jpeg",
                        Filename = Path.GetFileName(turtleImageFilepath)
                    };
                    imagesFile.Add(fileWithMetadata2);

                    service.Analyze(
                        callback: (DetailedResponse<AnalyzeResponse> response, IBMError error) =>
                        {
                            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Analyze result: {0}", response.Response);
                            analyzeResponse = response.Result;
                            Assert.IsNotNull(analyzeResponse);
                            Assert.IsNotNull(analyzeResponse.Images);
                            Assert.IsTrue(analyzeResponse.Images.Count > 0);
                            Assert.IsNotNull(analyzeResponse.Images[0].Objects.Collections);
                            Assert.IsTrue(analyzeResponse.Images[0].Objects.Collections.Count > 0);
                            Assert.IsNull(error);
                        },
                        collectionIds: new List<string>() { giraffeCollectionId },
                        features: new List<string>() { "objects" },
                        imagesFile: imagesFile
                    );

                    while (analyzeResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region Collections
        [UnityTest, Order(2)]
        public IEnumerator TestListCollections()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to List Collections...");
            CollectionsList collectionsList = null;
            service.ListCollections(
                callback: (DetailedResponse<CollectionsList> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "List Collections result: {0}", response.Response);
                    collectionsList = response.Result;
                    Assert.IsNotNull(collectionsList.Collections);
                    Assert.IsTrue(collectionsList.Collections.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (collectionsList == null)
                yield return null;
        }

        [UnityTest, Order(3)]
        public IEnumerator TestGetCollection()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Get Collection...");
            Collection collection = null;
            service.GetCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "GetCollection result: {0}", response.Response);
                    collection = response.Result;
                    Assert.IsNotNull(collection.CollectionId);
                    Assert.IsTrue(collection.CollectionId == giraffeCollectionId);
                    Assert.IsNull(error);
                },
                collectionId: giraffeCollectionId
            );

            while (collection == null)
                yield return null;
        }

        [UnityTest, Order(4)]
        public IEnumerator TestCreateCollection()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Create Collection...");
            Collection collection = null;
            service.CreateCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "CreateCollection result: {0}", response.Response);
                    collection = response.Result;
                    Assert.IsNotNull(collection.CollectionId);
                    Assert.IsNull(error);
                    collectionId = collection.CollectionId;
                }
            );

            while (collection == null)
                yield return null;
        }

        [UnityTest, Order(5)]
        public IEnumerator TestUpdateCollection()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Update Collection...");
            Collection collection = null;
            string updatedTestCollectionName = "newTestCollection";
            byte[] updatedTestCollectionNameUtf8Bytes = System.Text.Encoding.UTF8.GetBytes(updatedTestCollectionName);
            service.UpdateCollection(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "UpdateCollection result: {0}", response.Response);
                    collection = response.Result;
                    Assert.IsNotNull(collection.CollectionId);
                    Assert.IsTrue(collection.CollectionId == collectionId);
                    Assert.IsNull(error);
                },
                collectionId: collectionId,
                name: System.Text.Encoding.UTF8.GetString(updatedTestCollectionNameUtf8Bytes)
            );

            while (collection == null)
                yield return null;
        }

        [UnityTest, Order(99)]
        public IEnumerator TestDeleteCollection()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Delete Collection...");
            bool isComplete = false;
            service.DeleteCollection(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "DeleteCollection result: {0}", response.Response);
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(response.StatusCode == 200);
                    isComplete = true;
                },
                collectionId: collectionId
            );

            while (isComplete == true)
                yield return null;
        }
        #endregion

        #region Images
        [UnityTest, Order(6)]
        public IEnumerator TestAddImage()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Add Images...");
            ImageDetailsList imageDetailsList = null;
            List<FileWithMetadata> imagesFile = new List<FileWithMetadata>();
            using (FileStream fs1 = File.OpenRead(giraffeImageFilepath), fs2 = File.OpenRead(turtleImageFilepath))
            {
                using (MemoryStream ms1 = new MemoryStream(), ms2 = new MemoryStream())
                {
                    fs1.CopyTo(ms1);
                    fs2.CopyTo(ms2);
                    FileWithMetadata fileWithMetadata = new FileWithMetadata()
                    {
                        Data = ms1,
                        ContentType = "image/jpeg",
                        Filename = Path.GetFileName(giraffeImageFilepath)
                    };
                    imagesFile.Add(fileWithMetadata);

                    FileWithMetadata fileWithMetadata2 = new FileWithMetadata()
                    {
                        Data = ms2,
                        ContentType = "image/jpeg",
                        Filename = Path.GetFileName(turtleImageFilepath)
                    };
                    imagesFile.Add(fileWithMetadata2);

                    service.AddImages(
                        callback: (DetailedResponse<ImageDetailsList> response, IBMError error) =>
                        {
                            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "AddImages result: {0}", response.Response);
                            imageDetailsList = response.Result;
                            Assert.IsNotNull(imageDetailsList.Images);
                            Assert.IsTrue(imageDetailsList.Images.Count > 0);
                            Assert.IsNull(error);
                            imageId = imageDetailsList.Images[0].ImageId;
                        },
                        collectionId: collectionId,
                        imagesFile: imagesFile
                    );

                    while (imageDetailsList == null)
                        yield return null;
                }
            }
        }

        [UnityTest, Order(7)]
        public IEnumerator TestListImages()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to List Images...");
            ImageSummaryList imagesList = null;
            service.ListImages(
                callback: (DetailedResponse<ImageSummaryList> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "ListImages result: {0}", response.Response);
                    imagesList = response.Result;
                    Assert.IsNotNull(imagesList.Images);
                    Assert.IsTrue(imagesList.Images.Count > 0);
                    Assert.IsNull(error);
                },
                collectionId: collectionId
            );

            while (imagesList == null)
                yield return null;
        }

        [UnityTest, Order(8)]
        public IEnumerator TestGetImage()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Get Image...");
            ImageDetails imageDetails = null;
            service.GetImageDetails(
                callback: (DetailedResponse<ImageDetails> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "GetImage result: {0}", response.Response);
                    imageDetails = response.Result;
                    Assert.IsNotNull(imageDetails);
                    Assert.IsTrue(imageDetails.ImageId == imageId);
                    Assert.IsNull(error);
                },
                collectionId: collectionId,
                imageId: imageId
            );

            while (imageDetails == null)
                yield return null;
        }

        [UnityTest, Order(9)]
        public IEnumerator TestGetJpegImage()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Get Image...");
            bool isComplete = false;
            service.GetJpegImage(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "GetImage result: {0}", response.Response);
                    Assert.IsNotNull(response.Result);
                    isComplete = true;
                },
                collectionId: collectionId,
                imageId: imageId
            );

            while (isComplete == true)
                yield return null;
        }

        [UnityTest, Order(98)]
        public IEnumerator TestDeleteImage()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Delete Image...");
            bool isComplete = false;
            service.DeleteImage(
                callback: (DetailedResponse<object> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "DeleteImage result: {0}", response.Response);
                    Assert.IsNotNull(response.Result);
                    Assert.IsTrue(response.StatusCode == 200);
                    isComplete = true;
                },
                collectionId: collectionId,
                imageId: imageId
            );

            while (isComplete == true)
                yield return null;
        }
        #endregion

        #region Training Data
        [UnityTest, Order(10)]
        public IEnumerator TestAddImageTrainingData()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Add Image Training Data...");
            TrainingDataObjects trainingDataObjects = null;
            var objectName = giraffeClassname;
            List<TrainingDataObject> objects = new List<TrainingDataObject>()
            {
                new TrainingDataObject()
                {
                    _Object = objectName,
                    Location = new Location()
                    {
                        Left = 27,
                        Top = 64,
                        Width = 75,
                        Height = 78
                    }

                }
            };
            service.AddImageTrainingData(
                callback: (DetailedResponse<TrainingDataObjects> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "AddImageTrainingData result: {0}", response.Response);
                    trainingDataObjects = response.Result;
                    Assert.IsNotNull(trainingDataObjects);
                    Assert.IsTrue(trainingDataObjects.Objects.Count > 0);
                    Assert.IsNull(error);
                },
                collectionId: collectionId,
                imageId: imageId,
                objects: objects
            );

            while (trainingDataObjects == null)
                yield return null;
        }

        [UnityTest, Order(11)]
        public IEnumerator TestTrain()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Train...");
            Collection collection = null;
            service.Train(
                callback: (DetailedResponse<Collection> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Train result: {0}", response.Response);
                    collection = response.Result;
                    Assert.IsNotNull(collection);
                    Assert.IsTrue(collection.TrainingStatus.Objects.InProgress == true);
                    Assert.IsNull(error);
                },
                collectionId: collectionId
            );

            while (collection == null)
                yield return null;
        }

        [UnityTest, Order(12)]
        public IEnumerator TestGetTrainingUsage()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Train...");
            var startTime = "2019-11-18";
            var endTime = "2020-11-20";
            var dateStartTime = DateTime.Parse(startTime);
            var dateEndTime = DateTime.Parse(endTime);
            TrainingEvents trainingEvents = null;
            service.GetTrainingUsage(
                callback: (DetailedResponse<TrainingEvents> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "GetTrainingUsage result: {0}", response.Response);
                    trainingEvents = response.Result;
                    Assert.IsNotNull(trainingEvents);
                    Assert.IsNotNull(trainingEvents.Events);
                    Assert.IsTrue(trainingEvents.Events.Count > 0);
                    Assert.IsNull(error);
                },
                startTime: dateStartTime,
                endTime: dateEndTime
            );

            while (trainingEvents == null)
                yield return null;
        }

        [UnityTest, Order(13)]
        public IEnumerator TestListObjectMetadata()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to List ObjectMetadata...");
            ObjectMetadataList objectList = null;
            service.ListObjectMetadata(
                callback: (DetailedResponse<ObjectMetadataList> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "ListObjectMetadata result: {0}", response.Response);
                    objectList = response.Result;
                    Assert.IsNotNull(objectList.Objects);
                    Assert.IsTrue(objectList.Objects.Count > 0);
                    Assert.IsNull(error);
                },
                collectionId: giraffeCollectionId
            );

            while (objectList == null)
                yield return null;
        }

        [UnityTest, Order(13)]
        public IEnumerator TestGetObjectMetadata()
        {
            Log.Debug("VisualRecognitionServiceV4IntegrationTests", "Attempting to Get ObjectMetadata...");
            ObjectMetadata objectMetadata = null;
            service.GetObjectMetadata(
                callback: (DetailedResponse<ObjectMetadata> response, IBMError error) =>
                {
                    Log.Debug("VisualRecognitionServiceV4IntegrationTests", "GetObjectMetadata result: {0}", response.Response);
                    objectMetadata = response.Result;
                    Assert.IsNotNull(objectMetadata._Object);
                    Assert.IsTrue(objectMetadata.Count > 0);
                    Assert.IsNull(error);
                },
                collectionId: giraffeCollectionId,
                _object: "giraffe"
            );

            while (objectMetadata == null)
                yield return null;
        }
        #endregion
    }
}
