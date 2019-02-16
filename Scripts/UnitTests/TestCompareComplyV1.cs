/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.CompareComply.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestCompareComplyV1 : UnitTest
    {
        private fsSerializer _serializer = new fsSerializer();
        private string versionDate = "2018-11-15";
        private CompareComply compareComply;

        private bool autoListFeedbackTested = true;
        private bool convertToHtmlTested = false;
        private bool classifyElementsTested = false;
        private bool extractTablesTested = false;
        private bool compareDocumentsTested = false;
        private bool addFeedbackTested = false;
        private bool listFeedbackTested = false;
        private bool getFeedbackTested = false;
        private bool deleteFeedbackTested = false;
        private bool createBatchTested = false;
        private bool listBatchesTested = false;
        private bool getBatchTestsed = false;
        private bool updateBatchTested = false;

        private string feedbackId;
        private string batchId;

        private string contractAFilepath;
        private string contractBFilepath;
        private string tableFilepath;

        public override IEnumerator RunTest()
        {
            LogSystem.InstallDefaultReactors();

            contractAFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_A.pdf";
            contractBFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_B.pdf";
            tableFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/TestTable.pdf";

            string objectStorageCredentialsInputFilepath = "../sdk-credentials/cloud-object-storage-credentials-input.json";
            string objectStorageCredentialsOutputFilepath = "../sdk-credentials/cloud-object-storage-credentials-output.json";

            //  Test CompareComply using loaded credentials
            CompareComply autoCompareComply = new CompareComply();
            while (!autoCompareComply.Credentials.HasIamTokenData())
                yield return null;
            autoCompareComply.VersionDate = versionDate;
            autoCompareComply.ListFeedback(OnAutoListFeedback, OnFail);
            while (!autoListFeedbackTested)
                yield return null;

            #region Get Credentials
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
            Credential credential = vcapCredentials.GetCredentialByname("compare-comply-sdk")[0].Credentials;
            _url = credential.Url.ToString();

            //  Create credential and instantiate service
            TokenOptions tokenOptions = new TokenOptions()
            {
                IamApiKey = credential.IamApikey
            };
            #endregion

            Credentials credentials = new Credentials(tokenOptions, credential.Url);

            //  Wait for tokendata
            while (!credentials.HasIamTokenData())
                yield return null;

            compareComply = new CompareComply(credentials);
            compareComply.VersionDate = versionDate;

            byte[] contractA = File.ReadAllBytes(contractAFilepath);
            byte[] contractB = File.ReadAllBytes(contractBFilepath);
            byte[] table = File.ReadAllBytes(tableFilepath);

            byte[] objectStorageCredentialsInputData = File.ReadAllBytes(objectStorageCredentialsInputFilepath);
            byte[] objectStorageCredentialsOutputData = File.ReadAllBytes(objectStorageCredentialsOutputFilepath);

            compareComply.ConvertToHtml(OnConvertToHtml, OnFail, contractA, fileContentType: "application/pdf");
            while (!convertToHtmlTested)
            {
                yield return null;
            }

            compareComply.ClassifyElements(OnClassifyElements, OnFail, contractA);
            while (!classifyElementsTested)
            {
                yield return null;
            }

            compareComply.ExtractTables(OnExtractTables, OnFail, table);
            while (!extractTablesTested)
            {
                yield return null;
            }

            compareComply.CompareDocuments(OnCompareDocuments, OnFail, contractA, contractB, file1ContentType: "application/pdf", file2ContentType: "application/pdf");
            while (!compareDocumentsTested)
            {
                yield return null;
            }

            DateTime before = new DateTime(2018, 11, 15);
            DateTime after = new DateTime(2018, 11, 14);
            compareComply.ListFeedback(
                successCallback: OnListFeedback,
                failCallback: OnFail,
                feedbackType: "element_classification",
                before: before,
                after: after,
                documentTitle: "unity-test-feedback-doc",
                modelId: "contracts",
                modelVersion: "2.0.0",
                categoryRemoved: "Responsibilities",
                categoryAdded: "Amendments",
                categoryNotChanged: "Audits",
                typeRemoved: "End User:Exclusion",
                typeAdded: "Disclaimer:Buyer",
                typeNotChanged: "Obligation:IBM",
                pageLimit: 1
                );
            while (!listFeedbackTested)
            {
                yield return null;
            }


            #region Feedback Data
            FeedbackInput feedbackData = new FeedbackInput()
            {
                UserId = "user_id_123x",
                Comment = "Test feedback comment",
                FeedbackData = new FeedbackDataInput()
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
                                Label = "Responsibilities",
                                ProvenanceIds = new List<string>(){ }
                            },
                            new Category()
                            {
                                Label = "Amendments",
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
                                Label = "Responsibilities",
                            },
                            new Category()
                            {
                                Label = "Audits"
                            }
                        }
                    }
                }
            };
            #endregion

            compareComply.AddFeedback(
                successCallback: OnAddFeedback,
                failCallback: OnFail,
                feedbackData: feedbackData
                );
            while (!addFeedbackTested)
            {
                yield return null;
            }

            //  temporary fix for a bug requiring `x-watson-metadata` header
            Dictionary<string, object> customData = new Dictionary<string, object>();
            Dictionary<string, string> customHeaders = new Dictionary<string, string>();
            customHeaders.Add("x-watson-metadata", "customer_id=sdk-test-customer-id");
            customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);

            compareComply.GetFeedback(
                successCallback: OnGetFeedback,
                failCallback: OnFail,
                feedbackId: feedbackId,
                modelId: "contracts",
                customData: customData
                );
            while (!getFeedbackTested)
            {
                yield return null;
            }

            compareComply.DeleteFeedback(
                successCallback: OnDeleteFeedback,
                failCallback: OnFail,
                feedbackId: feedbackId,
                modelId: "contracts"
                );
            while (!deleteFeedbackTested)
            {
                yield return null;
            }

            compareComply.ListBatches(
                successCallback: OnListBatches,
                failCallback: OnFail
                );
            while (!listBatchesTested)
            {
                yield return null;
            }

            compareComply.CreateBatch(
                successCallback: OnCreateBatch,
                failCallback: OnFail,
                function: "html_conversion",
                inputCredentialsFile: objectStorageCredentialsInputData,
                inputBucketLocation: "us-south",
                inputBucketName: "compare-comply-integration-test-bucket-input",
                outputCredentialsFile: objectStorageCredentialsOutputData,
                outputBucketLocation: "us-south",
                outputBucketName: "compare-comply-integration-test-bucket-output"
                );
            while (!createBatchTested)
            {
                yield return null;
            }

            compareComply.GetBatch(
                successCallback: OnGetBatch,
                failCallback: OnFail,
                batchId: batchId
                );
            while(!getBatchTestsed)
            {
                yield return null;
            }

            compareComply.UpdateBatch(
                successCallback: OnUpdateBatch,
                failCallback: OnFail,
                batchId: batchId,
                action: "rescan",
                modelId: "contracts"
                );
            while(!updateBatchTested)
            {
                yield return null;
            }

            Log.Debug("TestCompareComplyV1.RunTests()", "Compare and Comply integration tests complete!");
        }

        private void OnAutoListFeedback(FeedbackList response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnAutoListFeedback()", "ListFeedback Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Feedback != null);
            autoListFeedbackTested = true;
        }

        private void OnConvertToHtml(HTMLReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnConvertToHtml()", "ConvertToHtml Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Title == "Microsoft Word - contract_A.doc");
            convertToHtmlTested = true;
        }

        private void OnClassifyElements(ClassifyReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnClassifyElements()", "ClassifyElements Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Document.Title == "Microsoft Word - contract_A.doc");
            classifyElementsTested = true;
        }
        private void OnExtractTables(TableReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnExtractTables()", "ExtractTables Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Document.Title == "Untitled spreadsheet");
            extractTablesTested = true;
        }

        private void OnCompareDocuments(CompareReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnCompareDocuments()", "CompareDocuments Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Documents[0].Html.Contains("HairPopMetal4Evah"));
            compareDocumentsTested = true;
        }

        private void OnListFeedback(FeedbackList response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnListFeedback()", "ListFeedback Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response.Feedback != null);
            listFeedbackTested = true;
        }

        private void OnAddFeedback(FeedbackReturn response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnAddFeedback()", "AddFeedback Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.FeedbackId));
            feedbackId = response.FeedbackId;
            addFeedbackTested = true;
        }

        private void OnGetFeedback(GetFeedback response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.FeedbackId));
            getFeedbackTested = true;
        }

        private void OnDeleteFeedback(FeedbackDeleted response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.Message));
            deleteFeedbackTested = true;
        }

        private void OnListBatches(Batches response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnListBatches()", "ListBatches Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(response._Batches != null);
            listBatchesTested = true;
        }

        private void OnCreateBatch(BatchStatus response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnCreateBatch()", "OnCreateBatch Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.BatchId));
            batchId = response.BatchId;
            createBatchTested = true;
        }

        private void OnGetBatch(BatchStatus response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnGetBatch()", "OnGetBatch Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.BatchId));
            getBatchTestsed = true;
        }

        private void OnUpdateBatch(BatchStatus response, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnUpdateBatch()", "OnUpdateBatch Response: {0}", customData["json"].ToString());
            Test(response != null);
            Test(!string.IsNullOrEmpty(response.BatchId));
            updateBatchTested = true;
        }

        private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
        {
            Log.Debug("TestCompareComplyV1.OnFail()", "Failed: {0}", error.ErrorMessage);
        }
    }
}
