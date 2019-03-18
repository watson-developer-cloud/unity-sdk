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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.CompareComply.V1;
using IBM.Watson.CompareComply.V1.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace IBM.Watson.Tests
{
    public class CompareComplyV1IntegrationTests
    {
        private CompareComplyService service;
        private string versionDate = "2019-02-13";
        private Dictionary<string, object> customData;
        private Dictionary<string, string> customHeaders = new Dictionary<string, string>();
        private string contractAFilepath;
        private string contractBFilepath;
        private string tableFilepath;
        private string createdFeedbackId;
        private string objectStorageCredentialsInputFilepath;
        private string objectStorageCredentialsOutputFilepath;
        private string createdBatchId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();
            customHeaders.Add("X-Watson-Test", "1");

            contractAFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/contract_A.pdf";
            contractBFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/contract_B.pdf";
            tableFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/TestTable.pdf";

            objectStorageCredentialsInputFilepath = "../sdk-credentials/cloud-object-storage-credentials-input.json";
            objectStorageCredentialsOutputFilepath = "../sdk-credentials/cloud-object-storage-credentials-output.json";
        }

        [UnitySetUp]
        public IEnumerator UnityTestSetup()
        {
            if (service == null)
            {
                service = new CompareComplyService(versionDate);
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

        #region ConvertToHtml
        [UnityTest, Order(0)]
        public IEnumerator TestConvertToHtml()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ConvertToHtml...");
            HTMLReturn convertToHtmlResponse = null;
            using (FileStream fs = File.OpenRead(contractAFilepath))
            {
                service.ConvertToHtml(
                    callback: (DetailedResponse<HTMLReturn> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("CompareComplyServiceV1IntegrationTests", "ConvertToHtml result: {0}", customResponseData["json"].ToString());
                        convertToHtmlResponse = response.Result;
                        Assert.IsNotNull(convertToHtmlResponse);
                        Assert.IsNotNull(convertToHtmlResponse.Html);
                        Assert.IsNull(error);
                    },
                    file: fs,
                    modelId: "contracts",
                    fileContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath)),
                    customData: customData
                );

                while (convertToHtmlResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region ClassifyElements
        [UnityTest, Order(1)]
        public IEnumerator TestClassifyElements()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ClassifyElements...");
            ClassifyReturn classifyElementsResponse = null;
            using (FileStream fs = File.OpenRead(contractAFilepath))
            {
                service.ClassifyElements(
                    callback: (DetailedResponse<ClassifyReturn> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("CompareComplyServiceV1IntegrationTests", "ClassifyElements result: {0}", customResponseData["json"].ToString());
                        classifyElementsResponse = response.Result;
                        Assert.IsNotNull(classifyElementsResponse);
                        Assert.IsNotNull(classifyElementsResponse.Elements);
                        Assert.IsNull(error);
                    },
                    file: fs,
                    modelId: "contracts",
                    fileContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath)),
                    customData: customData
                );

                while (classifyElementsResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region ExtractTables
        [UnityTest, Order(2)]
        public IEnumerator TestExtractTables()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ExtractTables...");
            TableReturn extractTablesResponse = null;
            using (FileStream fs = File.OpenRead(tableFilepath))
            {
                service.ExtractTables(
                    callback: (DetailedResponse<TableReturn> response, IBMError error, Dictionary<string, object> customResponseData) =>
                    {
                        Log.Debug("CompareComplyServiceV1IntegrationTests", "ExtractTables result: {0}", customResponseData["json"].ToString());
                        extractTablesResponse = response.Result;
                        Assert.IsNotNull(extractTablesResponse);
                        Assert.IsNotNull(extractTablesResponse.Tables);
                        Assert.IsNull(error);
                    },
                    file: fs,
                    modelId: "tables",
                    fileContentType: Utility.GetMimeType(Path.GetExtension(tableFilepath)),
                    customData: customData
                );

                while (extractTablesResponse == null)
                    yield return null;
            }
        }
        #endregion

        #region CompareDocuments
        [UnityTest, Order(3)]
        public IEnumerator TestCompareDocuments()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to CompareDocuments...");
            CompareReturn compareDocumentsResponse = null;
            using (FileStream fs0 = File.OpenRead(contractAFilepath))
            {
                using (FileStream fs1 = File.OpenRead(contractBFilepath))
                {
                    service.CompareDocuments(
                        callback: (DetailedResponse<CompareReturn> response, IBMError error, Dictionary<string, object> customResponseData) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "CompareDocuments result: {0}", customResponseData["json"].ToString());
                            compareDocumentsResponse = response.Result;
                            Assert.IsNotNull(compareDocumentsResponse);
                            Assert.IsNotNull(compareDocumentsResponse.Documents);
                            Assert.IsNull(error);
                        },
                        file1: fs0,
                        file2: fs1,
                        file1Label: "Contract A",
                        file2Label: "Contract B",
                        modelId: "contracts",
                        file1ContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath)),
                        file2ContentType: Utility.GetMimeType(Path.GetExtension(contractBFilepath)),
                        customData: customData
                    );

                    while (compareDocumentsResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region AddFeedback
        [UnityTest, Order(4)]
        public IEnumerator TestAddFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to AddFeedback...");
            FeedbackReturn addFeedbackResponse = null;

            #region feedbackData
            var feedbackData = new FeedbackDataInput()
            {
                FeedbackType = "element_classification",
                Document = new ShortDoc()
                {
                    Hash = "",
                    Title = "doc title"
                },
                ModelId = "contracts",
                ModelVersion = "11.00",
                Location = new Location()
                {
                    Begin = 241,
                    End = 237
                },
                Text = "1. IBM will provide a Senior Managing Consultant / expert resource, for up to 80 hours, to assist Florida Power & Light (FPL) with the creation of an IT infrastructure unit cost model for existing infrastructure.",
                OriginalLabels = new OriginalLabelsIn()
                {
                    Types = new List<TypeLabel>()
                        {
                            new TypeLabel()
                            {
                                Label = new Label()
                                {
                                    Nature = "Obligation",
                                    Party= "IBM"
                                },
                                ProvenanceIds = new List<string>()
                                {
                                    "85f5981a-ba91-44f5-9efa-0bd22e64b7bc",
                                    "ce0480a1-5ef1-4c3e-9861-3743b5610795"
                                }
                            },
                            new TypeLabel()
                            {
                                Label = new Label()
                                {
                                    Nature = "End User",
                                    Party= "Exclusion"
                                },
                                ProvenanceIds = new List<string>()
                                {
                                    "85f5981a-ba91-44f5-9efa-0bd22e64b7bc",
                                    "ce0480a1-5ef1-4c3e-9861-3743b5610795"
                                }
                            }
                        },
                    Categories = new List<Category>()
                        {
                            new Category()
                            {
                                Label = Category.LabelValue.RESPONSIBILITIES,
                                ProvenanceIds = new List<string>(){ }
                            },
                            new Category()
                            {
                                Label = Category.LabelValue.AMENDMENTS,
                                ProvenanceIds = new List<string>(){ }
                            }
                        }
                },
                UpdatedLabels = new UpdatedLabelsIn()
                {
                    Types = new List<TypeLabel>()
                        {
                            new TypeLabel()
                            {
                                Label = new Label()
                                {
                                    Nature = "Obligation",
                                    Party = "IBM"
                                }
                            },
                            new TypeLabel()
                            {
                                Label = new Label()
                                {
                                    Nature = "Disclaimer",
                                    Party = "buyer"
                                }
                            }
                        },
                    Categories = new List<Category>()
                        {
                            new Category()
                            {
                                Label = Category.LabelValue.RESPONSIBILITIES,
                            },
                            new Category()
                            {
                                Label = Category.LabelValue.AUDITS
                            }
                        }
                }
            };
            #endregion

            service.AddFeedback(
                callback: (DetailedResponse<FeedbackReturn> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "AddFeedback result: {0}", customResponseData["json"].ToString());
                    addFeedbackResponse = response.Result;
                    createdFeedbackId = addFeedbackResponse.FeedbackId;
                    Assert.IsNotNull(addFeedbackResponse);
                    Assert.IsNotNull(createdFeedbackId);
                    Assert.IsNull(error);
                },
                feedbackData: feedbackData,
                userId: "user_id_123x",
                comment: "Test feedback comment",
                customData: customData
            );

            while (addFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region GetFeedback
        [UnityTest, Order(5)]
        public IEnumerator TestGetFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to GetFeedback...");
            GetFeedback getFeedbackResponse = null;
            service.GetFeedback(
                callback: (DetailedResponse<GetFeedback> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "GetFeedback result: {0}", customResponseData["json"].ToString());
                    getFeedbackResponse = response.Result;
                    Assert.IsNotNull(getFeedbackResponse);
                    Assert.IsTrue(getFeedbackResponse.FeedbackId == createdFeedbackId);
                    Assert.IsNull(error);
                },
                feedbackId: createdFeedbackId,
                modelId: "contracts",
                customData: customData
            );

            while (getFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region ListFeedback
        [UnityTest, Order(6)]
        public IEnumerator TestListFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ListFeedback...");
            FeedbackList listFeedbackResponse = null;
            service.ListFeedback(
                callback: (DetailedResponse<FeedbackList> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "ListFeedback result: {0}", customResponseData["json"].ToString());
                    listFeedbackResponse = response.Result;
                    Assert.IsNotNull(listFeedbackResponse);
                    Assert.IsNotNull(listFeedbackResponse.Feedback);
                    Assert.IsTrue(listFeedbackResponse.Feedback.Count > 0);
                    Assert.IsNull(error);
                },
                feedbackType: "element_classification",
                includeTotal: true,
                customData: customData
            );

            while (listFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region CreateBatch
        [UnityTest, Order(7)]
        public IEnumerator TestCreateBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to CreateBatch...");
            BatchStatus createBatchResponse = null;
            using (FileStream fsInput = File.OpenRead(objectStorageCredentialsInputFilepath))
            {
                using (FileStream fsOutput = File.OpenRead(objectStorageCredentialsOutputFilepath))
                {
                    service.CreateBatch(
                        callback: (DetailedResponse<BatchStatus> response, IBMError error, Dictionary<string, object> customResponseData) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "CreateBatch result: {0}", customResponseData["json"].ToString());
                            createBatchResponse = response.Result;
                            createdBatchId = createBatchResponse.BatchId;
                            Assert.IsNotNull(createBatchResponse);
                            Assert.IsNotNull(createdBatchId);
                            Assert.IsNull(error);
                        },
                        function: "html_conversion",
                        inputCredentialsFile: fsInput,
                        inputBucketLocation: "us-south",
                        inputBucketName: "compare-comply-integration-test-bucket-input",
                        outputCredentialsFile: fsOutput,
                        outputBucketLocation: "us-south",
                        outputBucketName: "compare-comply-integration-test-bucket-output",
                        modelId: "contracts",
                        customData: customData
                    );
                }
            }
            while (createBatchResponse == null)
                yield return null;
        }
        #endregion

        #region GetBatch
        [UnityTest, Order(8)]
        public IEnumerator TestGetBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to GetBatch...");
            BatchStatus getBatchResponse = null;
            service.GetBatch(
                callback: (DetailedResponse<BatchStatus> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "GetBatch result: {0}", customResponseData["json"].ToString());
                    getBatchResponse = response.Result;
                    Assert.IsNotNull(getBatchResponse);
                    Assert.IsTrue(getBatchResponse.BatchId == createdBatchId);
                    Assert.IsNull(error);
                },
                batchId: createdBatchId,
                customData: customData
            );

            while (getBatchResponse == null)
                yield return null;
        }
        #endregion

        #region ListBatches
        [UnityTest, Order(9)]
        public IEnumerator TestListBatches()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ListBatches...");
            Batches listBatchesResponse = null;
            service.ListBatches(
                callback: (DetailedResponse<Batches> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "ListBatches result: {0}", customResponseData["json"].ToString());
                    listBatchesResponse = response.Result;
                    Assert.IsNotNull(listBatchesResponse);
                    Assert.IsNotNull(listBatchesResponse._Batches);
                    Assert.IsTrue(listBatchesResponse._Batches.Count > 0);
                    Assert.IsNull(error);
                },
                customData: customData
            );

            while (listBatchesResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateBatch
        [UnityTest, Order(10)]
        public IEnumerator TestUpdateBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to UpdateBatch...");
            BatchStatus updateBatchResponse = null;
            service.UpdateBatch(
                callback: (DetailedResponse<BatchStatus> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "UpdateBatch result: {0}", customResponseData["json"].ToString());
                    updateBatchResponse = response.Result;
                    Assert.IsNotNull(updateBatchResponse);
                    Assert.IsNull(error);
                },
                batchId: createdBatchId,
                action: "rescan",
                modelId: "contracts",
                customData: customData
            );

            while (updateBatchResponse == null)
                yield return null;
        }
        #endregion

        #region DeleteFeedback
        [UnityTest, Order(99)]
        public IEnumerator TestDeleteFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to DeleteFeedback...");
            FeedbackDeleted deleteFeedbackResponse = null;
            service.DeleteFeedback(
                callback: (DetailedResponse<FeedbackDeleted> response, IBMError error, Dictionary<string, object> customResponseData) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "DeleteFeedback result: {0}", customResponseData["json"].ToString());
                    deleteFeedbackResponse = response.Result;
                    Assert.IsNotNull(deleteFeedbackResponse);
                    Assert.IsTrue(deleteFeedbackResponse.Status == 200);
                    Assert.IsTrue(deleteFeedbackResponse.Message.Contains(createdFeedbackId));
                    Assert.IsNull(error);
                },
                feedbackId: createdFeedbackId,
                modelId: "contracts",
                customData: customData
            );

            while (deleteFeedbackResponse == null)
                yield return null;
        }
        #endregion
    }
}
