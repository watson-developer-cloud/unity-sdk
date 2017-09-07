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





### Create an environment
Creates an environment for the service instance. Note: You can create only one environment per service instance. Attempting to create another environment for the same service instance results in an error. See the [Discovery service home page][discovery-sizing] for additional information about sizing and pricing. To create a free trial environment, specify the value of size as 0 (zero).
```cs
private void CreateEnvironment()
{
  if (!_discovery.AddEnvironment(OnAddEnvironment, <environment-name>, <environment-description>, <environment-size>))
    Log.Debug("ExampleDiscoveryV1", "Failed to add environment");
}

private void OnAddEnvironment(Environment resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - AddEnvironment Response: {0}", data);
}
```






### List environments
List existing environments for the service instance.
```cs
private void GetEnvironments()
{
  if (!_discovery.GetEnvironments(OnGetEnvironments))
    Log.Debug("ExampleDiscoveryV1", "Failed to get environments");
}

private void OnGetEnvironments(GetEnvironmentsResponse resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - GetEnvironments Response: {0}", data);
}
```






### List environment details
Gets detailed information about the specified environment.
```cs
private void GetEnvironment()
{
  if (!_discovery.GetEnvironment(OnGetEnvironment, <environment-id>))
    Log.Debug("ExampleDiscoveryV1", "Failed to get environment");
}

private void OnGetEnvironment(Environment resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - GetEnvironment Response: {0}", data);
}
```







### Delete an environment
Deletes an existing environment.
```cs
private void DeleteEnvironment()
{
  if (!_discovery.DeleteEnvironment(OnDeleteEnvironment, <environment-id>))
    Log.Debug("ExampleDiscoveryV1", "Failed to delete environment");
}

private void OnDeleteEnvironment(bool success, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - DeleteEnvironment Response: deleted:{0}", success);
}
```






### Add an configuration
Adds a configuration to the service instance.
```cs
private void AddConfiguration()
{
  if (!_discovery.AddConfiguration(OnAddConfiguration, <environment-id>, <configuration-json-path>))
    Log.Debug("ExampleDiscoveryV1", "Failed to add configuration");
}

private void OnAddConfiguration(Configuration resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - AddConfiguration Response: {0}", data);
}
```






### List configurations
Lists existing configurations for the service instance.
```cs
private void GetConfigurations()
{
  if (!_discovery.GetConfigurations(OnGetConfigurations, <environment-id>))
    Log.Debug("ExampleDiscoveryV1", "Failed to get configurations");
}

private void OnGetConfigurations(GetConfigurationsResponse resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - GetConfigurations Response: {0}", data);
}
```






### List configuration details
Get information about the specified configuration.
```cs
private void GetConfiguration()
{
  if (!_discovery.GetConfiguration(OnGetConfiguration, <environment-id>, <configuration-id>))
    Log.Debug("ExampleDiscoveryV1", "Failed to get configuration");
}

private void OnGetConfiguration(Configuration resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - GetConfiguration Response: {0}", data);
}
```






### Preview a configuration
Runs a sample document through the default or your configuration and returns diagnostic information designed to help you understand how the document was processed. The document is not added to the index.
```cs
private void PreviewConfiguration()
{
  if (!_discovery.PreviewConfiguration(OnPreviewConfiguration, <environment-id>, <configuration-id>, null, <filepath-to-ingest>, <metadata>))
    Log.Debug("ExampleDiscoveryV1", "Failed to preview configuration");
}

private void OnPreviewConfiguration(TestDocument resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Preview configuration Response: {0}", data);
}
```






### Delete a configuration
Deletes an existing configuration from the service instance.

The delete operation is performed unconditionally. A delete request succeeds even if the configuration is referenced by a collection or document ingestion. However, documents that have already been submitted for processing continue to use the deleted configuration; documents are always processed with a snapshot of the configuration as it existed at the time the document was submitted.
```cs
private void DeleteConfiguration()
{
  if (!_discovery.DeleteConfiguration(OnDeleteConfiguration, <environment-id>, <configuration-id>))
    Log.Debug("ExampleDiscoveryV1", "Failed to delete configuration");
}

private void OnDeleteConfiguration(bool success, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - DeleteConfiguration Response: deleted:{0}", success);
}
```






### Create an collection
Creates a new collection for storing documents.
```cs
private void AddCollection()
{
  if (!_discovery.AddCollection(OnAddCollection, <environment-id>, <collection-name>, <collection-description>, <configuration-id>))
    Log.Debug("ExampleDiscovery", "Failed to add collection");
}

private void OnAddCollection(CollectionRef resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Add collection Response: {0}", data);
}
```






### List collections
Display a list of existing collections.
```cs
private void GetCollections()
{
  if (!_discovery.GetCollections(OnGetCollections, <environment-id>))
    Log.Debug("ExampleDiscovery", "Failed to get collections");
}

private void OnGetCollections(GetCollectionsResponse resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Get colletions Response: {0}", data);
}
```






### List collection details
Show detailed information about an existing collection.
```cs
private void GetCollection()
{
  if (!_discovery.GetCollection(OnGetCollection, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery", "Failed to get collection");
}

private void OnGetCollection(Collection resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Get colletion Response: {0}", data);
}
```






### List fields
Gets a list of the unique fields, and each field's type, that are stored in a collection's index.
```cs
private void GetFields()
{
  if (!_discovery.GetFields(OnGetFields, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery", "Failed to get fields");
}

private void OnGetFields(GetFieldsResponse resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Get fields Response: {0}", data);
}
```






### Delete an collection
Deletes an existing collection.
```cs
private void DeleteCollection()
{
  if (!_discovery.DeleteCollection(OnDeleteCollection, <environment-id>, <collection-id>))
    Log.Debug("ExampleDiscovery", "Failed to add collection");
}

private void OnDeleteCollection(bool success, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Delete collection Response: deleted:{0}", success);
}
```







### Add a document
Add a document to your collection.
```cs
private void AddDocument()
{
  if (!_discovery.AddDocument(OnAddDocument, <environment-id>, <collection-id>, <filepath-to-ingest>, <configuration-id>, null))
    Log.Debug("ExampleDiscovery", "Failed to add document");
}

private void OnAddDocument(DocumentAccepted resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Add document Response: {0}", data);
}
```







### Update a document
Update or partially update a document to create or replace an existing document.
```cs
private void UpdateDocument()
{
  if (!_discovery.UpdateDocument(OnUpdateDocument, <environment-id>, <collection-id>, <document-id>, <filepath-to-ingest>, <configuration-id>, null))
    Log.Debug("ExampleDiscovery", "Failed to update document");
}

private void OnUpdateDocument(DocumentAccepted resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Update document Response: {0}", data);
}
```







### List document details
Display status information about a submitted document.
```cs
private void GetDocument()
{
  if (!_discovery.GetDocument(OnGetDocument, <environment-id>, <collection-id>, <document-id>))
    Log.Debug("ExampleDiscovery", "Failed to get document");
}

private void OnGetDocument(DocumentStatus resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Get document Response: {0}", data);
}
```






### Delete a document
Delete a document from a collection.
```cs
private void DeleteDocument()
{
  if (!_discovery.DeleteDocument(OnDeleteDocument, <environment-id>, <collection-id>, <document-id>))
    Log.Debug("ExampleDiscovery", "Failed to delete document");
}

private void OnDeleteDocument(bool success, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Delete document Response: deleted:{0}", success);
}
```






### Queries
Query the documents in your collection.

Once your content is uploaded and enriched by the Discovery service, you can build queries to search your content. For a deep dive into queries, see [Building Queries and Delivering Content][discovery-query].
```cs
private void Query()
{
  if (!_discovery.Query(OnQuery, <environment-id>, <collection-id>, null, <query>, null, 10, null, 0))
    Log.Debug("ExampleDiscovery", "Failed to query");
}

private void OnQuery(QueryResponse resp, string data)
{
  Log.Debug("ExampleDiscoveryV1", "Discovery - Query Response: {0}", data);
}
```

[discovery]: https://www.ibm.com/watson/developercloud/discovery.html
[discovery-sizing]: https://www.ibm.com/watson/developercloud/discovery.html
[discovery-query]: https://console.bluemix.net/docs/services/discovery/using.html
