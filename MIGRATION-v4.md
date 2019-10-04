### Unity SDK V4 Migration guide

#### Authentication

##### v3.x.x

Previously different `Config` objects were used for authentication.

```cs
void Example()
{
    TokenOptions iamTokenOptions = new TokenOptions(
        Apikey: "{iam-apikey}"
    );
    service = new AssistantService("{versionDate}", iamTokenOptions);
    service.SetEndpoint("{service-endpoint}");
}
```

Or constructors accepted `TokenOptions` or `username` and `password`.

```cs
void Example()
{
    service = new AssistantService("{tokenOptions}", "{versionDate}");
    service2 = new DiscoveryService("{username}", "{password}", "{versionDate}");
}
```

##### v4.x.x

Now we use an `Authenticator` to authenticate the service. Available authentication schemes include `IamAuthenticator`, `BasicAuthenticator`, `CloudPakForDataAuthenticator` and `BearerTokenAuthenticator`

###### IamAuthenticator

```cs
void Example()
{
    IamAuthenticator authenticator = new IamAuthenticator(
        apikey: "{apikey}"
    );
    var service = new AssistantService("{versionDate}", authenticator);
    service.SetServiceUrl("{serviceUrl}");
}
```

##### BasicAuthenticator

```cs
void Example()
{
    BasicAuthenticator authenticator = new BasicAuthenticator(
        username: "{username}",
        password: "{password}"
    );
    var service = new AssistantService("{versionDate}", authenticator);
    service.SetServiceUrl("{serviceUrl}");
}
```

###### BearerTokenAuthenticator

```cs
void Example()
{
    BearerTokenAuthenticator authenticator = new BearerTokenAuthenticator(
        bearerToken: "{bearerToken}"
    );
    var service = new AssistantService("{versionDate}", authenticator);
    service.SetServiceUrl("{serviceUrl}");
}
```

###### CloudPakForDataAuthenticator

```cs
void Example()
{
    CloudPakForDataAuthenticator authenticator = new CloudPakForDataAuthenticator(
        url: "https://{cp4d_cluster_host}{:port}",
        username: "{username}",
        password: "{password}"
    );
    var service = new AssistantService("{version-date}", authenticator);
    service.SetServiceUrl("{serviceUrl}");
    var results = service.Message("{workspace-id}", "{message-request}");
}
```

Constructors that did not take an `Authenticator` were removed.

#### Supplying credentials

You can supply credentials to your service using external `ibm-credentials.env` files.

```cs
AssistantService service = new AssistantService("{version-date}");
var listWorkspacesResult = service.ListWorkspaces();
```

##### v3.x.x

Previously we would look for these files first in the system `home` directory, followed by the current project directory.

##### v4.x.x
Now in order to allow developers to have different configurations for each project we look first in the current project directory, followed by the home directory.

#### Setting the service url

##### v3.x.x

Previously we set the service url via SetEndpoint() method.

```cs
void Example()
{
    TokenOptions iamTokenOptions = new TokenOptions(
        apikey: "{iam-apikey}"
    );
    service = new AssistantService("{versionDate}", iamTokenOptions);
    service.SetEndpoint("{service-endpoint}");
}
```

##### v4.x.x

Now we set the service url via the SetServiceUrl() method.

```cs
void Example()
{
    IamAuthenticator authenticator = new IamAuthenticator(
        apikey: "{apikey}"
    );
    var service = new AssistantService("{versionDate}", authenticator);
    service.SetServiceUrl("{serviceUrl}");
}
```

#### Service changes

##### Assistant v1

* `includeCount` is no longer a parameter of the `ListWorkspaces()` method
* `includeCount` is no longer a parameter of the `ListIntents()` method
* `includeCount` is no longer a parameter of the `ListExamples()` method
* `includeCount` is no longer a parameter of the `ListCounterexamples()` method
* `includeCount` is no longer a parameter of the `ListEntities()` method
* `includeCount` is no longer a parameter of the `ListValues()` method
* `includeCount` is no longer a parameter of the `ListSynonyms()` method
* `includeCount` is no longer a parameter of the `ListDialogNodes()` method
* `valueType` was renamed to `type` in the `CreateValue()` method
* `newValueType` was renamed to `newType` in the `UpdateValue()` method
* `nodeType` was renamed to `type` in the `CreateDialogNode()` method
* `nodeType` was renamed to `type` in the `CreateDialogNode()` method
* `newNodeType` was renamed to `newType` in the `UpdateDialogNode()` method
* `ValueTypeEnumValue` was renamed to `TypeEnumValue` in the `CreateValue` model
* `ValueType` was renamed to `Type` in the `CreateValue` model
* `NodeTypeEnumValue` was renamed to `TypeEnumValue` in the `DialogNode` model
* `NodeType` was renamed to `Type` in the `DialogNode` model
* `ActionTypeEnumValue` was renamed to `TypeEnumValue` in the `DialogNodeAction` model
* `ActionType` was renamed to `Type` in the `DialogNodeAction` model
* `SEARCH_SKILL` constant was added to the `DialogNodeOutputGeneric` model
* `QueryTypeEnumValue` constants were added to the `DialogNodeOutputGeneric` model
* `QueryType` property was added to the `DialogNodeOutputGeneric` model
* `Query` property was added to the `DialogNodeOutputGeneric` model
* `Filter` property was added to the `DialogNodeOutputGeneric` model
* `DiscoveryVersion` property was added to the `DialogNodeOutputGeneric` model
* `Output` property type was converted from `Dictionary<string, object>` to `DialogSuggestionOutput` in the `DialogSuggestion` model
* `LogMessage` model no longer has `additionalProperties`
* `DialogRuntimeResponseGeneric` was renamed to `RuntimeResponseGeneric`
* `RuntimeEntity` model no longer has `additionalProperties`
* `RuntimeIntent` model no longer has `additionalProperties`
* `ValueTypeEnumValue` was renamed to `TypeEnumValue` in the `Value` model
* `ValueType` was renamed to `Type` in the `Value` model

##### Assistant v2

* `ActionTypeEnumValue` was renamed to `TypeEnumValue` in the `DialogNodeAction` model
* `ActionType` was renamed to `Type` in the `DialogNodeAction` model
* `DialogRuntimeResponseGeneric` was renamed to `RuntimeResponseGeneric`

##### Compare Comply v1

* `ConvertToHtml()` method does not require a `filename` parameter

##### Discovery v1

* `returnFields` was renamed to `_return` in the `Query()` method
* `loggingOptOut` was renamed to `xWatsonLoggingOptOut` in the `Query()` method
* `spellingSuggestions` was added to the `Query()` method
* `collectionIds` is no longer a parameter of the `Query()` method
* `returnFields` was renamed to `_return` in the `QueryNotices()` method
* `loggingOptOut` was renamed to `xWatsonLoggingOptOut` in the `FederatedQuery()` method
* `collectionIds` is now required in the `FederatedQuery()` method
* `collectionIds` changed position in the `FederatedQuery()` method
* `returnFields` was renamed to `_return` in the `FederatedQuery()` method
* `returnFields` was renamed to `_return` in the `FederatedQueryNotices()` method
* `EnrichmentName` was renamed to `_Enrichment` in the `Enrichment` model
* `FieldTypeEnumValue` was renamed to `TypeEnumValue` in the `Field` model
* `FieldType` was renamed to `Type` in the `Field` model
* `FieldName` was renamed to `_Field` in the `Field` model
* `TestConfigurationInEnvironment()` method was removed
* `QueryEntities()` method was removed
* `QueryRelations()` method was removed

##### Language Translator v3

* `defaultModels` was renamed to `_default` in the `ListModels()` method
* `TranslationOutput` was renamed to `_Translation` in the `Translation` model

##### Natural Language Classifier v1

* `metadata` was renamed to `trainingMetadata` in the `CreateClassifier()` method

##### Speech to Text v1

* `strict` is no longer a parameter of the `TrainAcousticModel()` method
* `FinalResults` was renamed to `Final` in the `SpeakerLabelsResult` model
* `FinalResults` was renamed to `Final` in the `SpeechRecognitionResult` model

##### Visual Recognition v3

* `DetectFaces()` method was removed
* `ClassName` was renamed to `_Class` in the `ClassResult` model
* `ClassName` was renamed to `_Class` in the `ModelClass` model

##### Visual Recognition v4

* New service!