# Compare Comply
IBM Watson™ [Compare and Comply]() analyzes governing documents to provide details about critical aspects of the documents.

## Usage
About
IBM Watson™ Compare and Comply is a collection of advanced APIs that enable better and faster document understanding. The APIs are pre-trained to handle document conversion, table understanding, Natural Language Processing, and comparison for contracts. JSON output adds real power to end-user applications, for a wide variety of use cases. The machine learning feedback interface is available to collect your feedback which is then incorporated into regular improvements to the core NLP model; the more you use it, the better the system performs.

Compare and Comply is designed to provide:

- Natural language understanding of contracts and invoices
- The ability to convert programmatic or scanned PDF documents, Microsoft Word files, image files, and text files to annotated JSON or to HTML
- Identification of legal entities and categories that align with subject matter expertise
- The ability to extract table information from an input document
- The ability to compare two contracts
- Compare and Comply brings together a functionally rich set of integrated, automated Watson APIs to input a file to identify sections, lists (numbered and bulleted), footnotes, and tables, converting these items into a structured HTML format. Furthermore, classification of this structured format is annotated and output as JSON with labeled elements, types, categories, and other information.

### HTML Conversion
Uploads a file. The response includes an HTML version of the document.
```cs
private void ConvertHtml()
{
  compareComply.ConvertToHtml(OnConvertToHtml, OnFail, <file-data>, fileContentType: "application/pdf");
}

private void OnConvertToHtml(HTMLReturn response, Dictionary<string, object> customData)
{
  Log.Debug("ExampleCompareComplyV1.OnConvertToHtml()", "ConvertToHtml Response: {0}", customData["json"].ToString());
}
```






### Classify Elements
Uploads a file. The response includes an analysis of the document’s structural and semantic elements.
```cs
private void ClassifyElements()
{
  compareComply.ClassifyElements(OnClassifyElements, OnFail, <file-data>);
}

private void OnClassifyElements(ClassifyReturn response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnClassifyElements()", "ClassifyElements Response: {0}", customData["json"].ToString());
}
```






### Extract Tables
Uploads a file. The response includes an analysis of the document’s tables.
```cs
private void ExtractTables()
{
  compareComply.ExtractTables(OnExtractTables, OnFail, <file-data>);
}

private void OnExtractTables(TableReturn response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnExtractTables()", "ExtractTables Response: {0}", customData["json"].ToString());
}
```







### Compare Documents
Uploads two PDF or JSON files. The response includes JSON comparing the two documents. Uploaded files must be in the same file format.
```cs
private void CompareDocuments()
{
  compareComply.CompareDocuments(OnCompareDocuments, OnFail, <file-1-data>, <file-2-data>, file1ContentType: <file-1-content-type>, file2ContentType: <file-2-content-type>);

}

private void OnCompareDocuments(CompareReturn response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnCompareDocuments()", "CompareDocuments Response: {0}", customData["json"].ToString());
}
```






### List Feedback
Gets the list of batch-processing jobs submitted by users.
```cs
private void ListFeedback()
{
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
}

private void OnListFeedback(FeedbackList response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnListFeedback()", "ListFeedback Response: {0}", customData["json"].ToString());
    listFeedbackTested = true;
}
```






### Add Feedback
Adds feedback in the form of labels from a subject-matter expert (SME) to a governing document.
Important: Feedback is not immediately incorporated into the training model, nor is it guaranteed to be incorporated at a later date. Instead, submitted feedback is used to suggest future updates to the training model.
```cs
private void AddFeedback()
{
  compareComply.AddFeedback(
      successCallback: OnAddFeedback,
      failCallback: OnFail,
      feedbackData: <feedback-data>
      );
}

private void OnAddFeedback(FeedbackReturn response, Dictionary<string, object> customData)
{
  Log.Debug("ExampleCompareComplyV1.OnAddFeedback()", "AddFeedback Response: {0}", customData["json"].ToString());
}
```






### Get Feedback
List a specified feedback entry
```cs
private void GetFeedback()
{
  //  temporary fix for a bug requiring `x-watson-metadata` header
  Dictionary<string, object> customData = new Dictionary<string, object>();
  Dictionary<string, string> customHeaders = new Dictionary<string, string>();
  customHeaders.Add("x-watson-metadata", "customer_id=sdk-test-customer-id");
  customData.Add(Constants.String.CUSTOM_REQUEST_HEADERS, customHeaders);

  compareComply.GetFeedback(
      successCallback: OnGetFeedback,
      failCallback: OnFail,
      feedbackId: <feedback-id>
      customData: customData
      );
}

private void OnGetFeedback(GetFeedback response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
}
```






### Delete Feedback
Deletes a specified feedback entry
```cs
private void DeleteFeedback()
{
  compareComply.DeleteFeedback(
      successCallback: OnDeleteFeedback,
      failCallback: OnFail,
      feedbackId: <feedback-id>
      );
}

private void OnDeleteFeedback(FeedbackDeleted response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnGetFeedback()", "GetFeedback Response: {0}", customData["json"].ToString());
}
```






### List Batches
Gets the list of submitted batch-processing jobs
```cs
private void ListBatches()
{
  compareComply.ListBatches(
      successCallback: OnListBatches,
      failCallback: OnFail
      );
}

private void OnListBatches(Batches response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnListBatches()", "ListBatches Response: {0}", customData["json"].ToString());
}
```






### Create Batch Request
Run Compare and Comply methods over a collection of input documents.
Important: Batch processing requires the use of the IBM Cloud Object Storage service. The use of IBM Cloud Object Storage with Compare and Comply is discussed at Using batch processing.
```cs
private void CreateBatch()
{
  compareComply.CreateBatch(
      successCallback: OnCreateBatch,
      failCallback: OnFail,
      function: <function>,
      inputCredentialsFile: <credential-file-data>,
      inputBucketLocation: <bucket-location>,
      inputBucketName: <bucket-name>,
      outputCredentialsFile: <credential-file-data>,
      outputBucketLocation: <bucket-location>,
      outputBucketName: <bucket-name>
      );
}

private void OnCreateBatch(BatchStatus response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnCreateBatch()", "OnCreateBatch Response: {0}", customData["json"].ToString());
}
```






### Get Batch
Gets information about a specific batch-processing request
```cs
private void GetBatch()
{
  compareComply.GetBatch(
      successCallback: OnGetBatch,
      failCallback: OnFail,
      batchId: <batch-id>
      );
}

private void OnGetBatch(BatchStatus response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnGetBatch()", "OnGetBatch Response: {0}", customData["json"].ToString());
}
```






### Update Batch
Updates a pending or active batch-processing request. You can rescan the input bucket to check for new documents or cancel a request.
```cs
private void UpdateBatch()
{
  compareComply.UpdateBatch(
      successCallback: OnUpdateBatch,
      failCallback: OnFail,
      batchId: <batch-id>,
      action: <action>
      );
}

private void OnUpdateBatch(BatchStatus response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleCompareComplyV1.OnUpdateBatch()", "OnUpdateBatch Response: {0}", customData["json"].ToString());
}
```

[compare-comply]: https://cloud.ibm.com/docs/services/compare-comply/index.html
