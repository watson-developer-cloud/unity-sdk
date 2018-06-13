# Natural Language Understanding

With [Natural Language Understanding][natural_language_understanding] developers can analyze semantic features of text input, including - categories, concepts, emotion, entities, keywords, metadata, relations, semantic roles, and sentiment.

## Usage
Natural Language Understanding uses natural language processing to analyze semantic features of any text. Provide plain text, HTML, or a public URL, and Natural Language Understanding returns results for the features you specify. The service cleans HTML before analysis by default, which removes most advertisements and other unwanted content.

You can create [custom models][custom_models] with Watson Knowledge Studio that can be used to detect custom [entities][entities] and [relations][relations] in Natural Language Understanding.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v2;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    NaturalLanguageUnderstanding _naturalLanguageUnderstanding = new NaturalLanguageUnderstanding(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleNaturalLanguageUnderstanding.OnFail()", "Error received: {0}", error.ToString());
}
```

### Analyze
Analyze features of natural language content.
```cs
private void Analyze()
{
  if (!_naturalLanguageUnderstanding.Analyze(OnAnalyze, OnFail, <parameters>))
      Log.Debug("ExampleNaturalLanguageUnderstanding.Analyze()", "Failed to get models.");
}

private void OnAnalyze(AnalysisResults resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleNaturalLanguageUnderstanding.OnAnalyze()", "AnalysisResults: {0}", customData["json"].ToString());
}
```



### Get Models
List available custom models.
```cs
private void GetModels()
{
  if (!_naturalLanguageUnderstanding.GetModels(OnGetModels, OnFail))
      Log.Debug("ExampleNaturalLanguageUnderstanding.GetModels()", "Failed to get models.");
}

private void OnGetModels(ListModelsResults resp, Dictionary<string, object> customData)
{
    Log.Debug("ExampleNaturalLanguageUnderstanding.OnGetModels()", "ListModelsResult: {0}", customData["json"].ToString());
}
```



### Delete Model
Delete a custom model.
```cs
private void DeleteModel()
{
  if (!_naturalLanguageUnderstanding.DeleteModel(OnDeleteModel, OnFail, <model-id>))
      Log.Debug("ExampleNaturalLanguageUnderstanding.DeleteModel()", "Failed to delete model.");
}

private void OnDeleteModel(bool success, Dictionary<string, object> customData)
{
    Log.Debug("ExampleNaturalLanguageUnderstanding.OnDeleteModel()", "DeleteModelResult: {0}", success);
}
```

[natural_language_understanding]: https://console.bluemix.net/docs/services/natural-language-understanding/index.html
[custom_models]: https://console.bluemix.net/docs/services/natural-language-understanding/customizing.html
[entities]: https://console.bluemix.net/docs/services/natural-language-understanding/entity-types.html
[relations]: https://console.bluemix.net/docs/services/natural-language-understanding/relations.html
