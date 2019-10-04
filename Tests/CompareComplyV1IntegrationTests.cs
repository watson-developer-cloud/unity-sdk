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
using IBM.Cloud.SDK.Authentication;
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
        private string contractAFilepath;
        private string contractBFilepath;
        private string tablePdfFilepath;
        private string tablePngFilepath;
        private string createdFeedbackId;
        private string objectStorageCredentialsInputFilepath;
        private string objectStorageCredentialsOutputFilepath;
        private string createdBatchId;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LogSystem.InstallDefaultReactors();

            contractAFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/contract_A.pdf";
            contractBFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/contract_B.pdf";
            tablePdfFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/TestTable.pdf";
            tablePngFilepath = Application.dataPath + "/Watson/Tests/TestData/CompareComplyV1/TableTestV3.png";

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

            while (!service.Authenticator.CanAuthenticate())
                yield return null;
        }

        [SetUp]
        public void TestSetup()
        {
            service.WithHeader("X-Watson-Test", "1");
        }

        #region ConvertToHtml
        [UnityTest, Order(0)]
        public IEnumerator TestConvertToHtml()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ConvertToHtml...");
            HTMLReturn convertToHtmlResponse = null;
            using (FileStream fs = File.OpenRead(contractAFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.ConvertToHtml(
                        callback: (DetailedResponse<HTMLReturn> response, IBMError error) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "ConvertToHtml result: {0}", response.Response);
                            convertToHtmlResponse = response.Result;
                            Assert.IsNotNull(convertToHtmlResponse);
                            Assert.IsNotNull(convertToHtmlResponse.Html);
                            Assert.IsNull(error);
                        },
                        file: ms,
                        model: "contracts",
                        fileContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath))
                    );

                    while (convertToHtmlResponse == null)
                        yield return null;
                }
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
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.ClassifyElements(
                        callback: (DetailedResponse<ClassifyReturn> response, IBMError error) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "ClassifyElements result: {0}", response.Response);
                            classifyElementsResponse = response.Result;
                            Assert.IsNotNull(classifyElementsResponse);
                            //Assert.IsNotNull(classifyElementsResponse.ContractType);
                            Assert.IsNotNull(classifyElementsResponse.Elements);
                            Assert.IsNull(error);
                        },
                        file: ms,
                        model: "contracts",
                        fileContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath))
                    );

                    while (classifyElementsResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region ExtractTablesPdf
        [UnityTest, Order(2)]
        public IEnumerator TestExtractPdfTables()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ExtractTables...");
            TableReturn extractTablesResponse = null;
            using (FileStream fs = File.OpenRead(tablePdfFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.ExtractTables(
                        callback: (DetailedResponse<TableReturn> response, IBMError error) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "ExtractTables result: {0}", response.Response);
                            extractTablesResponse = response.Result;
                            Assert.IsNotNull(extractTablesResponse);
                            Assert.IsNotNull(extractTablesResponse.Tables);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderIds);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderTexts);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderTextsNormalized);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderIds);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderTexts);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderTextsNormalized);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].KeyValuePairs);
                            Assert.IsNull(error);
                        },
                        file: ms,
                        model: "tables",
                        fileContentType: Utility.GetMimeType(Path.GetExtension(tablePdfFilepath))
                    );

                    while (extractTablesResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region ExtractTablesPng
        [UnityTest, Order(3)]
        public IEnumerator TestExtractPngTables()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ExtractTables...");
            TableReturn extractTablesResponse = null;
            using (FileStream fs = File.OpenRead(tablePngFilepath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    service.ExtractTables(
                        callback: (DetailedResponse<TableReturn> response, IBMError error) =>
                        {
                            Log.Debug("CompareComplyServiceV1IntegrationTests", "ExtractTables from png file result: {0}", response.Response);
                            extractTablesResponse = response.Result;
                            Assert.IsNotNull(extractTablesResponse);
                            Assert.IsNotNull(extractTablesResponse.Tables);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderIds);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderTexts);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].RowHeaderTextsNormalized);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderIds);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderTexts);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].BodyCells[0].ColumnHeaderTextsNormalized);
                            Assert.IsNotNull(extractTablesResponse.Tables[0].KeyValuePairs);
                            Assert.IsNull(error);
                        },
                        file: ms,
                        model: "tables",
                        fileContentType: Utility.GetMimeType(Path.GetExtension(tablePngFilepath))
                    );

                    while (extractTablesResponse == null)
                        yield return null;
                }
            }
        }
        #endregion

        #region CompareDocuments
        [UnityTest, Order(4)]
        public IEnumerator TestCompareDocuments()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to CompareDocuments...");
            CompareReturn compareDocumentsResponse = null;
            using (FileStream fs0 = File.OpenRead(contractAFilepath))
            {
                using (FileStream fs1 = File.OpenRead(contractBFilepath))
                {
                    using (MemoryStream ms0 = new MemoryStream())
                    {
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            fs0.CopyTo(ms0);
                            fs1.CopyTo(ms1);
                            service.CompareDocuments(
                            callback: (DetailedResponse<CompareReturn> response, IBMError error) =>
                            {
                                Log.Debug("CompareComplyServiceV1IntegrationTests", "CompareDocuments result: {0}", response.Response);
                                compareDocumentsResponse = response.Result;
                                Assert.IsNotNull(compareDocumentsResponse);
                                Assert.IsNotNull(compareDocumentsResponse.Documents);
                                Assert.IsNull(error);
                            },
                            file1: ms0,
                            file2: ms1,
                            file1Label: "Contract A",
                            file2Label: "Contract B",
                            model: "contracts",
                            file1ContentType: Utility.GetMimeType(Path.GetExtension(contractAFilepath)),
                            file2ContentType: Utility.GetMimeType(Path.GetExtension(contractBFilepath))
                        );

                            while (compareDocumentsResponse == null)
                                yield return null;
                        }
                    }
                }
            }
        }
        #endregion

        #region AddFeedback
        [UnityTest, Order(5)]
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
                callback: (DetailedResponse<FeedbackReturn> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "AddFeedback result: {0}", response.Response);
                    addFeedbackResponse = response.Result;
                    createdFeedbackId = addFeedbackResponse.FeedbackId;
                    Assert.IsNotNull(addFeedbackResponse);
                    Assert.IsNotNull(createdFeedbackId);
                    Assert.IsNull(error);
                },
                feedbackData: feedbackData,
                userId: "user_id_123x",
                comment: "Test feedback comment"
            );

            while (addFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region GetFeedback
        [UnityTest, Order(6)]
        public IEnumerator TestGetFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to GetFeedback...");
            GetFeedback getFeedbackResponse = null;
            service.GetFeedback(
                callback: (DetailedResponse<GetFeedback> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "GetFeedback result: {0}", response.Response);
                    getFeedbackResponse = response.Result;
                    Assert.IsNotNull(getFeedbackResponse);
                    Assert.IsTrue(getFeedbackResponse.FeedbackId == createdFeedbackId);
                    Assert.IsNull(error);
                },
                feedbackId: createdFeedbackId,
                model: "contracts"
            );

            while (getFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region ListFeedback
        [UnityTest, Order(7)]
        public IEnumerator TestListFeedback()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ListFeedback...");
            FeedbackList listFeedbackResponse = null;
            service.ListFeedback(
                callback: (DetailedResponse<FeedbackList> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "ListFeedback result: {0}", response.Response);
                    listFeedbackResponse = response.Result;
                    Assert.IsNotNull(listFeedbackResponse);
                    Assert.IsNotNull(listFeedbackResponse.Feedback);
                    Assert.IsTrue(listFeedbackResponse.Feedback.Count > 0);
                    Assert.IsNull(error);
                },
                feedbackType: "element_classification",
                includeTotal: true
            );

            while (listFeedbackResponse == null)
                yield return null;
        }
        #endregion

        #region CreateBatch
        [UnityTest, Order(8)]
        public IEnumerator TestCreateBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to CreateBatch...");
            BatchStatus createBatchResponse = null;
            using (FileStream fsInput = File.OpenRead(objectStorageCredentialsInputFilepath))
            {
                using (FileStream fsOutput = File.OpenRead(objectStorageCredentialsOutputFilepath))
                {
                    using (MemoryStream msInput = new MemoryStream())
                    {
                        using (MemoryStream msOutput = new MemoryStream())
                        {
                            fsInput.CopyTo(msInput);
                            fsOutput.CopyTo(msOutput);
                            service.CreateBatch(
                            callback: (DetailedResponse<BatchStatus> response, IBMError error) =>
                            {
                                Log.Debug("CompareComplyServiceV1IntegrationTests", "CreateBatch result: {0}", response.Response);
                                createBatchResponse = response.Result;
                                createdBatchId = createBatchResponse.BatchId;
                                Assert.IsNotNull(createBatchResponse);
                                Assert.IsNotNull(createdBatchId);
                                Assert.IsNull(error);
                            },
                            function: "html_conversion",
                            inputCredentialsFile: msInput,
                            inputBucketLocation: "us-south",
                            inputBucketName: "compare-comply-integration-test-bucket-input",
                            outputCredentialsFile: msOutput,
                            outputBucketLocation: "us-south",
                            outputBucketName: "compare-comply-integration-test-bucket-output",
                            model: "contracts"
                        );
                        }
                    }


                }
            }
            while (createBatchResponse == null)
                yield return null;
        }
        #endregion

        #region GetBatch
        [UnityTest, Order(9)]
        public IEnumerator TestGetBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to GetBatch...");
            BatchStatus getBatchResponse = null;
            service.GetBatch(
                callback: (DetailedResponse<BatchStatus> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "GetBatch result: {0}", response.Response);
                    getBatchResponse = response.Result;
                    Assert.IsNotNull(getBatchResponse);
                    Assert.IsTrue(getBatchResponse.BatchId == createdBatchId);
                    Assert.IsNull(error);
                },
                batchId: createdBatchId
            );

            while (getBatchResponse == null)
                yield return null;
        }
        #endregion

        #region ListBatches
        [UnityTest, Order(10)]
        public IEnumerator TestListBatches()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to ListBatches...");
            Batches listBatchesResponse = null;
            service.ListBatches(
                callback: (DetailedResponse<Batches> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "ListBatches result: {0}", response.Response);
                    listBatchesResponse = response.Result;
                    Assert.IsNotNull(listBatchesResponse);
                    Assert.IsNotNull(listBatchesResponse._Batches);
                    Assert.IsTrue(listBatchesResponse._Batches.Count > 0);
                    Assert.IsNull(error);
                }
            );

            while (listBatchesResponse == null)
                yield return null;
        }
        #endregion

        #region UpdateBatch
        [UnityTest, Order(11)]
        public IEnumerator TestUpdateBatch()
        {
            Log.Debug("CompareComplyServiceV1IntegrationTests", "Attempting to UpdateBatch...");
            BatchStatus updateBatchResponse = null;
            service.UpdateBatch(
                callback: (DetailedResponse<BatchStatus> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "UpdateBatch result: {0}", response.Response);
                    updateBatchResponse = response.Result;
                    Assert.IsNotNull(updateBatchResponse);
                    Assert.IsNull(error);
                },
                batchId: createdBatchId,
                action: "rescan",
                model: "contracts"
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
                callback: (DetailedResponse<FeedbackDeleted> response, IBMError error) =>
                {
                    Log.Debug("CompareComplyServiceV1IntegrationTests", "DeleteFeedback result: {0}", response.Response);
                    deleteFeedbackResponse = response.Result;
                    Assert.IsNotNull(deleteFeedbackResponse);
                    Assert.IsTrue(deleteFeedbackResponse.Status == 200);
                    Assert.IsTrue(deleteFeedbackResponse.Message.Contains(createdFeedbackId));
                    Assert.IsNull(error);
                },
                feedbackId: createdFeedbackId,
                model: "contracts"
            );

            while (deleteFeedbackResponse == null)
                yield return null;
        }
        #endregion
    }
}
