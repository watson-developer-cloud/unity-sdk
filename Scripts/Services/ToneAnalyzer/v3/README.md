[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.ToneAnalyzer.v3/)

### Tone Analyzer

The IBM Watson™ [Tone Analyzer Service][tone-analyzer] uses linguistic analysis to detect three types of tones from written text: emotions, social tendencies, and writing style. Emotions identified include things like anger, fear, joy, sadness, and disgust. Identified social tendencies include things from the Big Five personality traits used by some psychologists. These include openness, conscientiousness, extraversion, agreeableness, and emotional range. Identified writing styles include confident, analytical, and tentative.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.ToneAnalyzer.v3

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.ToneAnalyzer.v3": "1.1.0"
}

```
### Usage
Use [Tone Analyzer][tone-analyzer] to detect three types of tones from written text: emotions, social tendencies, and language style. Emotions identified include things like anger, cheerfulness and sadness. Identified social tendencies include things from the Big Five personality traits used by some psychologists. These include openness, conscientiousness, extraversion, agreeableness, and neuroticism. Identified language styles include things like confident, analytical, and tentative. Input email and other written media into the [Tone Analyzer][tone-analyzer] service, and use the results to determine if your writing comes across with the tone, personality traits, and writing style that you want for your intended audience.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Tone Analyzer Service instance
ToneAnalyzerService _toneAnalyzer = new ToneAnalyzerService();

// set the credentials
_toneAnalyzer.SetCredential("<username>", "<password>");
```


#### Analyze tone
Analyzes the tone of a piece of text. The message is analyzed for several tones - social, emotional, and language. For each tone, various traits are derived. For example, conscientiousness, agreeableness, and openness.
```cs
 // Analyze Tone
 var results = service.AnalyzeTone("A word is dead when it is said, some say. Emily Dickinson");

```

[tone-analyzer]: https://www.ibm.com/watson/developercloud/doc/tone-analyzer/index.html
