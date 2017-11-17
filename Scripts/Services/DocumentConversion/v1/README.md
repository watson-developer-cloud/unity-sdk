# Document Conversion

The IBM Watson™ [Document conversion][document_conversion] service converts a single HTML, PDF, or Microsoft Word™ document into a normalized HTML, plain text, or a set of JSON-formatted Answer units that can be used with other Watson services. Carefully inspect output to make sure that it contains all elements and metadata required by the security standards of you or your organization.

## Usage
Convert a document into an easily ingestible format.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    DocumentConversion _documentConversion = new DocumentConversion(credentials);
}
```

### Converting Documents
Converts a document to answer units, HTML or text. This method accepts a multipart/form-data request. Upload the document as the "file" form part and the configuration as the "config" form part.

```cs
void ConvertDocument ()
{
  if (!m_DocumentConversion.ConvertDocument(OnConvertDocument, OnFail, <document-filepath>, <conversion-target>))
    Log.Debug("ExampleDocumentConversion.ConvertDocument()", "Document conversion failed!");
}

private void OnConvertDocument(ConvertedDocument documentConversionResponse, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDocumentConversion.OnConvertDocument()", "DocuemntConversion - ConvertDocument Response: {0}", customData["json"].ToString());
}

private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleDocumentConversion.OnFail()", "Error received: {0}", error.ToString());
}
```

[document_conversion]: https://www.ibm.com/watson/services/document-conversion/
