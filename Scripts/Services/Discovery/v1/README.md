# Discovery
The IBM Watson™ [Discovery][discovery] service makes it possible to rapidly build cognitive, cloud-based exploration applications that unlock actionable insights hidden in unstructured data - including your own proprietary data, as well as public and third-party data.

## Usage
The IBM Watson™ [Discovery][discovery] Service uses data analysis combined with cognitive intuition in order to take your unstructured data and enrich it so that you can query it to return the information that you need from it.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.Discovery.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    Discovery _discovery = new Discovery(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleDiscovery.OnFail()", "Error received: {0}", error.ToString());
}
```



### Create an environment
Creates an environment for the service instance. Note: You can create only one environment per service instance. Attempting to create another environment for the same service instance results in an error. See the [Discovery service home page][discovery-sizing] for additional information about sizing and pricing. To create a free trial environment, specify the value of size as 0 (zero).
```cs
private void CreateEnvironment()
{
  if (!_discovery.AddEnvironment(OnAddEnvironment, OnFail, <environment-name>, <environment-description>, <environment-size>))
    Log.Debug("ExampleDiscovery.CreateEnvironment()", "Failed to add environment");
}

private void OnAddEnvironment(Environment resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnAddEnvironment()", "Discovery - AddEnvironment Response: {0}", customData["json"].ToString());
}
```






### List environments
List existing environments for the service instance.
```cs
private void GetEnvironments()
{
  if (!_discovery.GetEnvironments(OnGetEnvironments, OnFail))
    Log.Debug("ExampleDiscovery.GetEnvironments()", "Failed to get environments");
}

private void OnGetEnvironments(GetEnvironmentsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetEnvironments()", "Discovery - GetEnvironments Response: {0}", customData["json"].ToString());
}
```






### List environment details
Gets detailed information about the specified environment.
```cs
private void GetEnvironment()
{
  if (!_discovery.GetEnvironment(OnGetEnvironment, OnFail, <environment-id>))
    Log.Debug("ExampleDiscovery.GetEnvironment()", "Failed to get environment");
}

private void OnGetEnvironment(Environment resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetEnvironment()", "Discovery - GetEnvironment Response: {0}", customData["json"].ToString());
}
```







### Delete an environment
Deletes an existing environment.
```cs
private void DeleteEnvironment()
{
  if (!_discovery.DeleteEnvironment(OnDeleteEnvironment, OnFail, <environment-id>))
    Log.Debug("ExampleDiscovery.DeleteEnvironment()", "Failed to delete environment");
}

private void OnDeleteEnvironment(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnDeleteEnvironment()", "Discovery - DeleteEnvironment Response: deleted:{0}", success);
}
```






### Add an configuration
Adds a configuration to the service instance.
```cs
private void AddConfiguration()
{
  if (!_discovery.AddConfiguration(OnAddConfiguration, OnFail, <environment-id>, <configuration-json-path>))
    Log.Debug("ExampleDiscovery.AddConfiguration()", "Failed to add configuration");
}

private void OnAddConfiguration(Configuration resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnAddConfiguration()", "Discovery - AddConfiguration Response: {0}", customData["json"].ToString());
}
```






### List configurations
Lists existing configurations for the service instance.
```cs
private void GetConfigurations()
{
  if (!_discovery.GetConfigurations(OnGetConfigurations, OnFail, <environment-id>))
    Log.Debug("ExampleDiscovery.GetConfigurations()", "Failed to get configurations");
}

private void OnGetConfigurations(GetConfigurationsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetConfigurations()", "Discovery - GetConfigurations Response: {0}", customData["json"].ToString());
}
```






### List configuration details
Get information about the specified configuration.
```cs
private void GetConfiguration()
{
  if (!_discovery.GetConfiguration(OnGetConfiguration, OnFail, <environment-id>, <configuration-id>))
    Log.Debug("ExampleDiscovery.GetConfiguration()", "Failed to get configuration");
}

private void OnGetConfiguration(Configuration resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetConfiguration()", "Discovery - GetConfiguration Response: {0}", customData["json"].ToString());
}
```






### Preview a configuration
Runs a sample document through the default or your configuration and returns diagnostic information designed to help you understand how the document was processed. The document is not added to the index.
```cs
private void PreviewConfiguration()
{
  if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, OnFail, <environment-id>, <configuration-id>, null, <filepath-to-ingest>, <metadata>))
    Log.Debug("ExampleDiscovery.PreviewConfiguration()", "Failed to preview configuration");
}

private void OnPreviewConfiguration(TestDocument resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnPreviewConfiguration()", "Discovery - Preview configuration Response: {0}", customData["json"].ToString());
}
```






### Delete a configuration
Deletes an existing configuration from the service instance.

The delete operation is performed unconditionally. A delete request succeeds even if the configuration is referenced by a collection or document ingestion. However, documents that have already been submitted for processing continue to use the deleted configuration; documents are always processed with a snapshot of the configuration as it existed at the time the document was submitted.
```cs
private void DeleteConfiguration()
{
  if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, OnFail, <environment-id>, <configuration-id>))
    Log.Debug("ExampleDiscovery.DeleteConfiguration()", "Failed to delete configuration");
}

private void OnDeleteConfiguration(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnDeleteConfiguration()", "Discovery - DeleteConfiguration Response: deleted:{0}", success);
}
```






### Create an collection
Creates a new collection for storing documents.
```cs
private void AddCollection()
{
  if (!_discovery.AddCollection(OnAddCollection, OnFail, <environment-id>, <collection-name>, <collection-description>, <configuration-id>))
    Log.Debug("ExampleDiscovery.AddCollection()", "Failed to add collection");
}

private void OnAddCollection(CollectionRef resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnAddCollection()", "Discovery - Add collection Response: {0}", customData["json"].ToString());
}
```






### List collections
Display a list of existing collections.
```cs
private void GetCollections()
{
  if (!_discovery.GetCollections(OnGetCollections, OnFail, <environment-id>))
    Log.Debug("ExampleDiscovery.GetCollections()", "Failed to get collections");
}

private void OnGetCollections(GetCollectionsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetCollections()", "Discovery - Get colletions Response: {0}", customData["json"].ToString());
}
```






### List collection details
Show detailed information about an existing collection.
```cs
private void GetCollection()
{
  if (!_discovery.GetCollection(OnGetCollection, OnFail, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery.GetCollection()", "Failed to get collection");
}

private void OnGetCollection(Collection resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetCollection()", "Discovery - Get colletion Response: {0}", customData["json"].ToString());
}
```






### List fields
Gets a list of the unique fields, and each field's type, that are stored in a collection's index.
```cs
private void GetFields()
{
  if (!_discovery.GetFields(OnGetFields, OnFail, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery.GetFields()", "Failed to get fields");
}

private void OnGetFields(GetFieldsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetFields()", "Discovery - Get fields Response: {0}", customData["json"].ToString());
}
```






### Delete an collection
Deletes an existing collection.
```cs
private void DeleteCollection()
{
  if (!_discovery.DeleteCollection(OnDeleteCollection, OnFail, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery.DeleteCollection()", "Failed to add collection");
}

private void OnDeleteCollection(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnDeleteCollection()", "Discovery - Delete collection Response: deleted:{0}", success);
}
```







### Add a document
Add a document to your collection.
```cs
private void AddDocument()
{
  if (!_discovery.AddDocument(OnAddDocument, OnFail, <environment-id>, <collection-id>, <filepath-to-ingest>, <configuration-id>, null))
    Log.Debug("ExampleDiscovery.AddDocument()", "Failed to add document");
}

private void OnAddDocument(DocumentAccepted resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnAddDocument()", "Discovery - Add document Response: {0}", customData["json"].ToString());
}
```







### Update a document
Update or partially update a document to create or replace an existing document.
```cs
private void UpdateDocument()
{
  if (!_discovery.UpdateDocument(OnUpdateDocument, OnFail, <environment-id>, <collection-id>, <document-id>, <filepath-to-ingest>, <configuration-id>, null))
    Log.Debug("ExampleDiscovery.UpdateDocument()", "Failed to update document");
}

private void OnUpdateDocument(DocumentAccepted resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnUpdateDocument()", "Discovery - Update document Response: {0}", customData["json"].ToString());
}
```







### List document details
Display status information about a submitted document.
```cs
private void GetDocument()
{
  if (!_discovery.GetDocument(OnGetDocument, OnFail, <environment-id>, <collection-id>, <document-id>))
    Log.Debug("ExampleDiscovery.GetDocument()", "Failed to get document");
}

private void OnGetDocument(DocumentStatus resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnGetDocument()", "Discovery - Get document Response: {0}", customData["json"].ToString());
}
```






### Delete a document
Delete a document from a collection.
```cs
private void DeleteDocument()
{
  if (!_discovery.DeleteDocument(OnDeleteDocument, OnFail, <environment-id>, <collection-id>, <document-id>))
    Log.Debug("ExampleDiscovery.DeleteDocument()", "Failed to delete document");
}

private void OnDeleteDocument(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnDeleteDocument()", "Discovery - Delete document Response: deleted:{0}", success);
}
```






### Queries
Query the documents in your collection.

Once your content is uploaded and enriched by the Discovery service, you can build queries to search your content. For a deep dive into queries, see [Building Queries and Delivering Content][discovery-query].
```cs
private void Query()
{
  if (!_discovery.Query(OnQuery, OnFail, <environment-id>, <collection-id>, null, <query>, null, 10, null, 0))
    Log.Debug("ExampleDiscovery.Query()", "Failed to query");
}

private void OnQuery(QueryResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleDiscovery.OnQuery()", "Discovery - Query Response: {0}", customData["json"].ToString());
}
```

[discovery]: https://www.ibm.com/watson/developercloud/discovery.html
[discovery-sizing]: https://www.ibm.com/watson/developercloud/discovery.html
[discovery-query]: https://console.bluemix.net/docs/services/discovery/using.html
