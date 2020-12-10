## Unity SDK V5 Migration guide

### Service changes

#### All Services
* `VersionDate` was renamed to `Version`

#### Assistant v1

* `includeCount` is now a parameter of the `ListWorkspaces()` method
* `includeCount` is now a parameter of the `ListIntents()` method
* `includeCount` is now a parameter of the `ListExamples()` method
* `includeCount` is now a parameter of the `ListCounterexamples()` method
* `includeCount` is now a parameter of the `ListEntities()` method
* `includeCount` is now a parameter of the `ListValues()` method
* `includeCount` is now a parameter of the `ListSynonyms()` method
* `includeCount` is now a parameter of the `ListDialogNodes()` method
* `context` type was changed from `Dictionary<string, object>` to `DialogNodeContext` in the `CreateDialogNode()` method
* `newContext` type was changed from `Dictionary<string, object>` to `DialogNodeContext` in the `UpdateDialogNode()` method
* `BulkClassify()` method was addded

##### Models Added

`BulkClassifyOutput`, 
`BulkClassifyResponse`, 
`BulkClassifyUtterance`, 
`DialogNodeContext`, 
`DialogNodeOutputConnectToAgentTransferInfo`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypeConnectToAgent`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypeImage`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypeOption`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypePause`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypeSearchSkill`, 
`DialogNodeOutputGenericDialogNodeOutputResponseTypeText`, 
`RuntimeResponseGenericRuntimeResponseTypeConnectToAgent`, 
`RuntimeResponseGenericRuntimeResponseTypeImage`, 
`RuntimeResponseGenericRuntimeResponseTypeOption`, 
`RuntimeResponseGenericRuntimeResponseTypePause`, 
`RuntimeResponseGenericRuntimeResponseTypeSuggestion`, 
`RuntimeResponseGenericRuntimeResponseTypeText`

##### Models Removed

`DialogSuggestionOutput`, 
`DialogSuggestionResponseGeneric`, 
`SystemResponse`

##### Model Properties Changed

`Context`
* `System` property type changed from `SystemResponse` to `Dictionary<string, object>`

`DialogNode`
* `context` property type changed from `Dictionary<string, object>` to `DialogNodeContext`

`DialogNodeOutput`
* Added `Integrations` property with getter and setter

`DialogNodeOutputGeneric`, `RuntimeResponseGeneric`
* Added `AgentAvailable`, `AgentUnavailable`, and `TransferInfo` properties

`DialogSuggestion`
* `output` property type changed from `DialogSuggestionOutput` to `Dictionary<string, object>`

#### Assistant v2

* `BulkClassify()` method was addded

##### Models Added

`BulkClassifyOutput`, 
`BulkClassifyResponse`, 
`BulkClassifyUtterance`,
`DialogNodeOutputConnectToAgentTransferInfo`, 
`RuntimeResponseGenericRuntimeResponseTypeConnectToAgent`, 
`RuntimeResponseGenericRuntimeResponseTypeImage`, 
`RuntimeResponseGenericRuntimeResponseTypeOption`, 
`RuntimeResponseGenericRuntimeResponseTypePause`, 
`RuntimeResponseGenericRuntimeResponseTypeSearch`, 
`RuntimeResponseGenericRuntimeResponseTypeSuggestion`, 
`RuntimeResponseGenericRuntimeResponseTypeText`

##### Models Removed

`MessageContextSkills`

##### Model Properties Changed

`MessageContext`, `MessageContextStateless`
* `Skills` property type changed from `MessageContextSkills` to `Dictionary<string, MessageContextSkill>`

`MessageContextSkill`
* `System` property type changed from `Dictionary<string, object>` to `MessageContextSkillSystem`

`RuntimeResponseGeneric`
* Added `AgentAvailable`, `AgentUnavailable`, and `TransferInfo` properties

#### Compare Comply v1

* `before` and `after` parameters were removed from `ListFeedback` method

##### Model Properties Changed

`Category`, `TypeLabel`
* Added `ModificationEnumValue` class
* Added `Modification` property

`OriginalLabelsOut`, `UpdatedLabelsOut`
* Removed `ModificationEnumValue` class
* Removed `Modification` property

#### Discovery v1

##### Models Removed

`NluEnrichmentCategories`

##### Model Properties Changed

`NluEnrichmentFeatures`
* Changed `Categories` property type from `NluEnrichmentCategories` to `Dictionary<string, object>`

#### Discovery v2

* `AnalyzeDocument()` method was addded

##### Models Added

`AnalyzedDocument`,
`AnalyzedResult`,
`QueryResponsePassage`

##### Models Removed

`QueryNoticesResult`

##### Model Properties Changed

`QueryGroupByAggregation`
* Inherits `QueryAggregation`

`QueryResponse`
* Added `Passages` property

#### Language Translator v3

No changes

#### Natural Language Classifier v1

No changes

#### Natural Language Understanding v1

##### Models Added

`FeaturesResultsMetadata`

##### Models Removed

`AnalysisResultsMetadata`,
`MetadataOptions`

##### Model Properties Changed

`AnalysisResults`
* Changed `Metadata` property type from `AnalysisResultsMetadata` to `FeaturesResultsMetadata`

`Features`
* Changed `Metadata` property type from `MetadataOptions` to `object`

#### Personality Insights

* Added deprecation notice
* Changed `content` parameter type from `Content` to `System.IO.MemoryStream` in `Profile()` method
* Changed `content` parameter type from `Content` to `System.IO.MemoryStream` in `ProfileAsCsv()` method

##### Models Removed

`Content`,
`ContentItem`

#### Speech To Text v1

* Changed `audio` parameter type from `byte[]` to `System.IO.MemoryStream` in `Recognize()` method

* Changed `audio` parameter type from `byte[]` to `System.IO.MemoryStream` in `CreateJob()` method

* Changed `grammarFile` parameter type from `string` to `System.IO.MemoryStream` in `AddGrammar()` method

* Changed `audioResource` parameter type from `byte[]` to `System.IO.MemoryStream` in `AddAudio()` method

#### Text To Speech v1

* Renamed `CreateVoiceModel()` method to `CreateCustomModel()`

* Renamed `ListVoiceModels()` method to `ListCustomModels()`

* Renamed `UpdateVoiceModel()` method to `UpdateCustomModel()`

* Renamed `GetVoiceModel()` method to `GetCustomModel()`

* Renamed `DeleteVoiceModel()` method to `GetCustomModel()`

##### Models Added

`CustomModel`, 
`CustomModels`

##### Models Removed

`VoiceModel`, 
`VoiceModels`

##### Model Properties Changed

`Voice`
* Change return type of `Customization` from `VoiceModel` to `CustomModel`

#### Tone Analyzer v3

* Changed `toneInput` parameter type from `ToneInput` to `System.IO.MemoryStream` in `Tone()` method

#### Visual Recognition v3

* Added deprecation notice

#### Visual Recognition v4

* Added deprecation notice
* Changed `startTime` and `endTime` parameter types from `string` to `DateTime` in `GetTrainingUsage()` method

##### Models Added

`CollectionTrainingStatus`,
`ObjectDetailLocation`

##### Model Properties Changed

`Collection`
* `TrainingStatus` property is no longer virtual
* Changed `TrainingStatus` property type from `TraningStatus` to `CollectionTrainingStatus`

`ObjectDetail`
* Changed `Location` property type from `Location` to `ObjectDetailLocation`
