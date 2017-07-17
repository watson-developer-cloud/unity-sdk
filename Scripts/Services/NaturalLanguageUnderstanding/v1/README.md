[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1/)

### Natural Language Understanding
With [Natural Language Understanding][natural_language_understanding] developers can analyze semantic features of text input, including - categories, concepts, emotion, entities, keywords, metadata, relations, semantic roles, and sentiment.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1": "1.1.0"
}

```
### Usage
Natural Language Understanding uses natural language processing to analyze semantic features of any text. Provide plain text, HTML, or a public URL, and Natural Language Understanding returns results for the features you specify. The service cleans HTML before analysis by default, which removes most advertisements and other unwanted content.

You can create [custom models][custom_models] with Watson Knowledge Studio that can be used to detect custom [entities][entities] and [relations][relations] in Natural Language Understanding.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Natural Language Understanding Service instance
NaturalLanguageUnderstandingService _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService();

// set the credentials
_naturalLanguageUnderstandingService.SetCredential("<username>", "<password>");
```

#### Analyze
Analyze features of natural language content.
```cs
Parameters parameters = new Parameters()
{
    Text = <string-to-analyze>,
    Features = new Features()
    {
        Keywords = new KeywordsOptions()
        {
            Limit = 8,
            Sentiment = true,
            Emotion = true
        }
    }
};

var result = _naturalLanguageUnderstandingService.Analyze(parameters);
```

#### List Models
List available [custom models][custom_models].
```cs
var result = _naturalLanguageUnderstandingService.GetModels();
```

[natural_language_understanding]: https://www.ibm.com/watson/developercloud/doc/natural-language-understanding/index.html
[custom_models]: https://www.ibm.com/watson/developercloud/doc/natural-language-understanding/customizing.html
[entities]: https://www.ibm.com/watson/developercloud/natural-language-understanding/api/v1/#entities
[relations]: https://www.ibm.com/watson/developercloud/natural-language-understanding/api/v1/#relations
