[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.Discovery.v1/)

### Discovery
The IBM Watson™ [Discovery][discovery] service makes it possible to rapidly build cognitive, cloud-based exploration applications that unlock actionable insights hidden in unstructured data - including your own proprietary data, as well as public and third-party data.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.Discovery.v1

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.Discovery.v1": "1.1.0"
}

```
### Usage
The IBM Watson™ [Discovery][discovery] Service uses data analysis combined with cognitive intuition in order to take your unstructured data and enrich it so that you can query it to return the information that you need from it.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Discovery Service instance
DiscoveryService _discovery = new DiscoveryService();

// set the credentials
_discovery.SetCredential("<username>", "<password>");
```

#### Create an environment
Creates an environment for the service instance. Note: You can create only one environment per service instance. Attempting to create another environment for the same service instance results in an error. See the [Discovery service home page][discovery-sizing] for additional information about sizing and pricing. To create a free trial environment, specify the value of size as 0 (zero).
```cs
CreateEnvironmentRequest createEnvironmentRequest = new CreateEnvironmentRequest()
{
    Name = <environment-name>,
    Description = <environment-description>,
    Size = <environment-size>
};

var result = _discovery.CreateEnvironment(createEnvironmentRequest);
```

#### List environments
List existing environments for the service instance.
```cs
var result = _discovery.ListEnvironments();
```

#### List environment details
Gets detailed information about the specified environment.
```cs
var result = _discovery.GetEnvironment(<environmentId>);
```

#### Update an environment
Updates an existing environment.
```cs
UpdateEnvironmentRequest updateEnvironmentRequest = new UpdateEnvironmentRequest()
{
    Name = <updated-environment-name>,
    Description = <updated-environment-description>
};

var result = _discovery.UpdateEnvironment(updateEnvironmentRequest);
```

#### Delete an environment
Deletes an existing environment.
```cs
var result = _discovery.DeleteEnvironment(<environmentId>);
```

#### Add an configuration
Adds a configuration to the service instance.
```cs
Configuration configuration = new Configuration()
{
    Name = <configuration-name>,
    Description = <configuration-description>,
    [...]
};

var result = _discovery.CreateConfiguration(<environmentId>, configuration);
```

#### List configurations
Lists existing configurations for the service instance.
```cs
var result = _discovery.ListConfigurations(<environmentId>);
```

##### List configuration details
Get information about the specified configuration.
```cs
var result = _discovery.GetConfiguration(<environmentId>, <configurationId>);
```

##### Update a configuration
Replaces an existing configuration. This operation completely replaces the previous configuration. Important: Do not attempt to replace the default configuration.

The new configuration can contain one or more of the configuration_id, updated, or created elements, but the elements are ignored and do not generate errors to enable pasting in another existing configuration. You can also provide a new configuration with none of the three elements.

Documents are processed with a snapshot of the configuration that was in place at the time the document was submitted for ingestion. This means documents that were already submitted are not processed with the new configuration.
```cs
Configuration configuration = new Configuration()
{
    Name = <configuration-name>,
    Description = <configuration-description>,
    [...]
};

var result = _discovery.UpdateConfiguration(<environmentId>, <configurationId>, configuration);
```

##### Delete a configuration
Deletes an existing configuration from the service instance.

The delete operation is performed unconditionally. A delete request succeeds even if the configuration is referenced by a collection or document ingestion. However, documents that have already been submitted for processing continue to use the deleted configuration; documents are always processed with a snapshot of the configuration as it existed at the time the document was submitted.
```cs
var result = _discovery.DeleteConfiguration(<environmentId>, <configurationId>);
```

<!-- ##### Test your configuration on a document
Run a sample document against your configuration or the default configuration and return diagnostic information designed to help you understand how the document was processed. The document is not added to a collection.
```cs
``` -->

##### Create an collection
Creates a new collection for storing documents.
```cs
CreateCollectionRequest createCollectionRequest = new CreateCollectionRequest()
{
    Language = <collectionLanguage>,
    Name = <collectionName>,
    Description = <collectionDescription>,
    ConfigurationId = <configurationId>
};

var result = _discovery.CreateCollection(<environmentId>, createCollectionRequest);
```

##### List collections
Display a list of existing collections.
```cs
var result = _discovery.ListCollections(<environmentId>);
```

##### List collection details
Show detailed information about an existing collection.
```cs
```

##### Update an collection
Creates a new collection for storing documents.
```cs
var result = _discovery.GetCollection(<environmentId>, <collectionId>);
```

##### List fields
Gets a list of the unique fields, and each field's type, that are stored in a collection's index.
```cs
var result = _discovery.ListCollectionFields(<environmentId>, <collectionId>);
```

##### Delete an collection
Deletes an existing collection.
```cs
var result = _discovery.DeleteCollection(<environmentId>, <collectionId>);
```

##### Add a document
Add a document to your collection.
```cs
using (FileStream fs = File.OpenRead(<filepath>))
{
    var result = _discovery.AddDocument(<environmentId>, <collectionId>, <configurationId>, fs as Stream);
}
```

##### Update a document
Update or partially update a document to create or replace an existing document.
```cs
using (FileStream fs = File.OpenRead(<filepath>))
{
    var result = _discovery.UpdateDocument(<environmentId>, <collectionId>, <documentId>, <configurationId>, fs as Stream);
}
```

##### List document details
Display status information about a submitted document.
```cs
var result = _discovery.GetDocumentStatus(<environmentId, <collectionId>, <documentId>);
```

##### Delete a document
Delete a document from a collection.
```cs
var result = _discovery.DeleteDocument(<environmentId>, <collectionId>, <documentId>);
```

##### Queries
Query the documents in your collection.

Once your content is uploaded and enriched by the Discovery service, you can build queries to search your content. For a deep dive into queries, see [Building Queries and Delivering Content][discovery-query].
```cs
var result = _discovery.Query(<environmentId>, <collectionId>, <query>);
```

[discovery]: https://www.ibm.com/watson/developercloud/discovery.html
[discovery-sizing]: https://www.ibm.com/watson/developercloud/discovery.html#pricing-block
[discovery-query]: https://www.ibm.com/watson/developercloud/doc/discovery/using.html
