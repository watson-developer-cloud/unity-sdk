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
#pragma warning disable 0649

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

public class ExampleCompareComplyV1 : MonoBehaviour
{
    #region PLEASE SET THESE VARIABLES IN THE INSPECTOR
    [Space(10)]
    [Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/compare-comply/api\"")]
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

    private CompareComply compareComply;
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

        //  Create credential and instantiate service
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

        compareComply = new CompareComply(credentials);
        compareComply.VersionDate = _versionDate;

        Runnable.Run(Examples());
    }

    private IEnumerator Examples()
    {
        contractAFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_A.pdf";
        contractBFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/contract_B.pdf";
        tableFilepath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/compare-comply/TestTable.pdf";

        string objectStorageCredentialsInputFilepath = "../sdk-credentials/cloud-object-storage-credentials-input.json";
        string objectStorageCredentialsOutputFilepath = "../sdk-credentials/cloud-object-storage-credentials-output.json";

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
        while (!getBatchTestsed)
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
        while (!updateBatchTested)
        {
            yield return null;
        }

        Log.Debug("ExampleCompareComplyV1.RunTests()", "Compare and Comply examples complete!");
    }

    private void OnConvertToHtml(HTMLReturn response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnConvertToHtml()", "ConvertToHtml Response: {0}", customData["json"].ToString());
        convertToHtmlTested = true;
    }

    private void OnClassifyElements(ClassifyReturn response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnClassifyElements()", "ClassifyElements Response: {0}", customData["json"].ToString());
        classifyElementsTested = true;
    }
    private void OnExtractTables(TableReturn response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnExtractTables()", "ExtractTables Response: {0}", customData["json"].ToString());
        extractTablesTested = true;
    }

    private void OnCompareDocuments(CompareReturn response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnCompareDocuments()", "CompareDocuments Response: {0}", customData["json"].ToString());
        compareDocumentsTested = true;
    }

    private void OnListFeedback(FeedbackList response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnListFeedback()", "ListFeedback Response: {0}", customData["json"].ToString());
        listFeedbackTested = true;
    }

    private void OnAddFeedback(FeedbackReturn response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnAddFeedback()", "AddFeedback Response: {0}", customData["json"].ToString());
        feedbackId = response.FeedbackId;
        addFeedbackTested = true;
    }

    private void OnGetFeedback(GetFeedback response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
        getFeedbackTested = true;
    }

    private void OnDeleteFeedback(FeedbackDeleted response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
        deleteFeedbackTested = true;
    }

    private void OnListBatches(Batches response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnListBatches()", "ListBatches Response: {0}", customData["json"].ToString());
        listBatchesTested = true;
    }

    private void OnCreateBatch(BatchStatus response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnCreateBatch()", "OnCreateBatch Response: {0}", customData["json"].ToString());
        batchId = response.BatchId;
        createBatchTested = true;
    }

    private void OnGetBatch(BatchStatus response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnGetBatch()", "OnGetBatch Response: {0}", customData["json"].ToString());
        getBatchTestsed = true;
    }

    private void OnUpdateBatch(BatchStatus response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnUpdateBatch()", "OnUpdateBatch Response: {0}", customData["json"].ToString());
        updateBatchTested = true;
    }

    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleCompareComplyV1.OnFail()", "Failed: {0}", error.ErrorMessage);
    }
}
