# Retrieve and Rank

The IBM Watson™ [Retrieve and Rank][retrieve_and_rank] service combines two information retrieval components in a single service: the power of Apache Solr and a sophisticated machine learning capability. This combination provides users with more relevant results by automatically reranking them by using these machine learning algorithms.

## Usage
Ingest documents and query the document corpus.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.RetrieveAndRank.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    RetrieveAndRank _retrieveAndRank = new RetrieveAndRank(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleRetrieveAndRank.OnFail()", "Error received: {0}", error.ToString());
}
```

###  Getting clusters
Retrieves the list of Solr clusters for the service instance.

```cs
void GetClusters()
{
  if (!_retrieveAndRank.GetClusters(OnGetClusters, OnFail))
    Log.Debug("ExampleRetrieveAndRank.GetClusters()", "Failed to get clusters!");
}

private void OnGetClusters(SolrClusterListResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetClusters()", "GetClusters results: {0}", customData["json"].ToString());
}
```

###  Creating a cluster
Provisions a Solr cluster asynchronously. When the operation is successful, the status of the cluster is set to NOT_AVAILABLE. The status must be READY before you can use the cluster.

```cs
void CreateCluster()
{
  if (!_retrieveAndRank.CreateCluster(OnCreateCluster, OnFail, <cluster-name>, <cluster-size>))
    Log.Debug("ExampleRetrieveAndRank.CreateCluster()", "Failed to create cluster!");
}

private void OnCreateCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnCreateClusterMethod()", "CreateCluster results: {0}", customData["json"].ToString());
}
```

###  Deleting a cluster
Stops and deletes a Solr Cluster asynchronously.

```cs
void DeleteCluster()
{
  if (!_retrieveAndRank.DeleteCluster(OnDeleteCluster, OnFail, <cluster-id>))
    Log.Debug("ExampleRetrieveAndRank.DeleteCluster()", "Failed to delete cluster!");
}

private void OnDeleteCluster(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnDeleteCluster()", "DeleteCluster results: {0}", success);
}

```

###  Get a cluster
Returns status and other information about a cluster.

```cs
void GetCluster()
{
  if (!_retrieveAndRank.GetCluster(OnGetCluster, OnFail, <cluster-id>))
    Log.Debug("ExampleRetrieveAndRank.GetCluster()", "Failed to get cluster!");
}

private void OnGetCluster(SolrClusterResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetCluster()", "GetCluster results: {0}", customData["json"].ToString());
}
```

###  Listing cluster configs
Retrieves all configurations for a cluster.

```cs
void GetClusterConfigs()
{
  if (!_retrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, OnFail, <cluster-id>))
  Log.Debug("ExampleRetrieveAndRank.GetClusterConfigs()", "Failed to get cluster configs!");
}

private void OnGetClusterConfigs(SolrConfigList resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetClusterConfigs()", "GetClusterConfigs results: {0}", customData["json"].ToString());
}
```

###  Deleting a cluster config
Deletes the configuration for a cluster. Before you delete the configuration, delete any collections that point to it.

```cs
void DeleteClusterConfig()
{
  if (!_retrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, OnFail, <cluster-id>, <cluster-config>))
    Log.Debug("ExampleRetrieveAndRank.DeleteClusterConfig()", "Failed to delete cluster config {0}", <cluster-config>);
}

private void OnDeleteClusterConfig(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnDeleteClusterConfig()", "DeleteClusterConfig results: {0}", success);
}
```

###  Getting a cluster config
Retrieves the configuration for a cluster by its name.

```cs
void GetClusterConfig()
{
  if (!_retrieveAndRank.GetClusterConfig(OnGetClusterConfig, OnFail, <cluster-id>, <cluster-config-name>))
    Log.Debug("ExampleRetrieveAndRank.GetClusterConfig()", "Failed to get cluster config {0}!", <cluster-config-name>);
}

private void OnGetClusterConfig(byte[] respData, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetClusterConfig()", "GetClusterConfig results: {0}", customData["json"].ToString());
}
```

### Save cluster config
Saves the cluster config
```cs
void SaveClusterConfig()
{
  if(!_retrieveAndRank.SaveConfig(OnSaveConfig, OnFail, <response-data>, <file-path>, data))
    Log.Debug("ExampleRetrieveAndRank.SaveConfig()", "Failed to save cluster config!");
}

private void OnSaveConfig(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnSaveConfig()", "SaveClusterConfig results: {0}", customData["json"].ToString());
}
```

###  Uploading a cluster config
Uploads a zip file containing the configuration files for your Solr collection. The zip file must include schema.xml, solrconfig.xml, and other files you need for your configuration. Configuration files on the zip file's path are not uploaded. The request fails if a configuration with the same name exists. To update an existing config, use the [Solr configuration API](https://cwiki.apache.org/confluence/display/solr/Config+API).

```cs
void UploadClusterConfig()
{
  if (!_retrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, OnFail, <cluster-id>, <cluster-config-name>, <cluster-config-path>))
    Log.Debug("ExampleRetrieveAndRank.UploadClusterConfig()", "Failed to upload cluster config {0}!", <cluster-config-path>);
}

private void OnUploadClusterConfig(UploadResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnUploadClusterConfig()", "UploadClusterConfig results: {0}", customData["json"].ToString());
}
```

###  List collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void ListCollections()
{
  if (!_retrieveAndRank.ForwardCollectionRequest(OnGetCollections, OnFail, <cluster-id>, CollectionsAction.LIST))
    Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to get collections!");
}

private void OnGetCollections(CollectionsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetCollections()", "ListCollections results: {0}", customData["json"].ToString());
}
```

###  Create Collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void CreateCollection()
{
  if (!_retrieveAndRank.ForwardCollectionRequest(OnCreateCollection, OnFail, <cluster-id>, CollectionsAction.CREATE, <collection-name>, <cluster-config-name>))
    Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to create collections!");
}

private void OnCreateCollection(CollectionsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnCreateCollection()", "ListCollections results: {0}", customData["json"].ToString());
}
```

###  Delete Collection request
An example of a method that forwards to the [Solr Collections API](https://cwiki.apache.org/confluence/display/solr/Collections+API). This Retrieve and Rank resource improves error handling and resiliency of the Solr Collections API.

```cs
void DeleteCollection()
{
  if (!_retrieveAndRank.ForwardCollectionRequest(OnGetCollections, OnFail, <cluster-id>, CollectionsAction.DELETE, <collection-name>))
    Log.Debug("ExampleRetrieveAndRank.ForwardCollectionRequest()", "Failed to delete collections!");
}

private void OnGetCollections(CollectionsResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetCollections()", "DeleteCollection results: {0}", customData["json"].ToString());
}
```

###  Index documents
Adds content to a Solr index so you can search it.

An example of a method that forwards to Solr. For more information about indexing, see [Indexing and Basic Data Operations](https://cwiki.apache.org/confluence/display/solr/Indexing+and+Basic+Data+Operations) in the Apache Solr Reference.

You must commit your documents to the index to search for them. For more information about when to commit, see [UpdateHandlers in SolrConfig](https://cwiki.apache.org/confluence/display/solr/UpdateHandlers+in+SolrConfig) in the Solr Reference.

```cs
void IndexDocuments()
{
  if (!_retrieveAndRank.IndexDocuments(OnIndexDocuments, OnFail, <index-data-path>, <cluster-id>, <collection-name>))
    Log.Debug("ExampleRetrieveAndRank.IndexDocuments()", "Failed to index documents!");
}

private void OnIndexDocuments(IndexResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnIndexDocuments()", "IndexDocuments results: {0}", customData["json"].ToString());
}
```

###  Standard Search and Ranked Search
Forwards to the Solr [standard query parser](https://cwiki.apache.org/confluence/display/solr/The+Standard+Query+Parser).

```cs
void Search()
{
  //  Standard search
  string[] fl = { "title", "id", "body", "author", "bibliography" };
  if (!_retrieveAndRank.Search(OnSearch, OnFail, <cluster-id>, <collection-name>, <query>, fl))
    Log.Debug("ExampleRetrieveAndRank.Search()", "Failed to search!");

  //  Ranked search
  string[] fl = { "title", "id", "body", "author", "bibliography" };
  if (!_retrieveAndRank.Search(OnSearch, OnFail, <cluster-id>, <collection-name>, <query>, fl, true, <ranker-id>))
    Log.Debug("ExampleRetrieveAndRank.Search()", "Failed to search!");
}

private void OnSearch(SearchResponse resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnSearch()", "Search results: {0}", customData["json"].ToString());
}
```

###  Getting rankers
Retrieves the list of rankers for the service instance.

```cs
void GetRankers()
{
  if (!_retrieveAndRank.GetRankers(OnGetRankers, OnFail))
    Log.Debug("ExampleRetrieveAndRank.GetRankers()", "Failed to get rankers!");
}

private void OnGetRankers(ListRankersPayload resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetRankers()", "GetRankers results: {0}", customData["json"].ToString());
}
```

###  Creating a ranker
Sends data to create and train a ranker and returns information about the new ranker.

When the operation is successful, the status of the ranker is set to Training. The status must be Available before you can use the ranker.

```cs
void CreateRanker()
{
  if (!_retrieveAndRank.CreateRanker(OnCreateRanker, OnFail, <ranker-training-path>, <ranker-name>))
    Log.Debug("ExampleRetrieveAndRank.CreateRanker()", "Failed to create ranker!");
}

private void OnCreateRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnCreateRanker()", "CreateRanker results: {0}", customData["json"].ToString());
}
```

###  Rank
Returns the top answer and a list of ranked answers with their ranked scores and confidence values. Use the [Get information about a ranker](http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#get_status) method to retrieve the status.

Use this method to return answers when you train the ranker with custom features. However, in most cases, you can use the [Search and rank](http://www.ibm.com/watson/developercloud/retrieve-and-rank/api/v1/#query_ranker) method.

```cs
void Rank()
{
  if (!_retrieveAndRank.Rank(OnRank, OnFail, <ranker-id>, <ranker-data-path>))
    Log.Debug("ExampleRetrieveAndRank.Rank()", "Failed to rank!");
}

private void OnRank(RankerOutputPayload resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnRank()", "Rank results: {0}", customData["json"].ToString());
}
```

###  Deleting rankers
Deletes a ranker.

```cs
void DeleteRanker()
{
  if (!_retrieveAndRank.DeleteRanker(OnDeleteRanker, OnFail, <ranker-to-delete>))
    Log.Debug("ExampleRetrieveAndRank.DeleteRanker()", "Failed to delete ranker {0}!", <rankerToDelete>);
}

private void OnDeleteRanker(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnDeleteRanker()", "DeleteRanker results: {0}", success);
}
```

###  Getting ranker info
Returns status and other information about a ranker.

```cs
void GetRanker()
{
  if (!_retrieveAndRank.GetRanker(OnGetRanker, OnFail, <ranker-id>))
      Log.Debug("ExampleRetrieveAndRank.GetRanker()", "Failed to get ranker!");
}

private void OnGetRanker(RankerStatusPayload resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleRetrieveAndRank.OnGetRanker()", "GetRanker results: {0}", customData["json"].ToString());
}
```
[retrieve-and-rank-service]: https://www.ibm.com/watson/services/retrieve-and-rank/
[retrieve-and-rank-documentation]: https://console.bluemix.net/docs/services/retrieve-and-rank/index.html
