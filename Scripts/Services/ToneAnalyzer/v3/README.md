# Tone Analyzer

The IBM Watson™ [Tone Analyzer Service][tone-analyzer] uses linguistic analysis to detect three types of tones from written text: emotions, social tendencies, and writing style. Emotions identified include things like anger, fear, joy, sadness, and disgust. Identified social tendencies include things from the Big Five personality traits used by some psychologists. These include openness, conscientiousness, extraversion, agreeableness, and emotional range. Identified writing styles include confident, analytical, and tentative.

## Usage
Use [Tone Analyzer][tone-analyzer] to detect three types of tones from written text: emotions, social tendencies, and language style. Emotions identified include things like anger, cheerfulness and sadness. Identified social tendencies include things from the Big Five personality traits used by some psychologists. These include openness, conscientiousness, extraversion, agreeableness, and neuroticism. Identified language styles include things like confident, analytical, and tentative. Input email and other written media into the [Tone Analyzer][tone-analyzer] service, and use the results to determine if your writing comes across with the tone, personality traits, and writing style that you want for your intended audience.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    ToneAnalyzer _toneAnalyzer = new ToneAnalyzer(credentials);
}
```


### Analyze tone
Analyzes the tone of a piece of text. The message is analyzed for several tones - social, emotional, and language. For each tone, various traits are derived. For example, conscientiousness, agreeableness, and openness.
```cs
private void AnalyzeTone()
{
    if (!_toneAnalyzer.GetToneAnalyze(OnGetToneAnalyze, _stringToTestTone))
        Log.Debug("ExampleToneAnalyzer", "Failed to analyze!");
}

private void OnGetToneAnalyze(ToneAnalyzerResponse resp, string data)
{
    Log.Debug("ExampleToneAnalyzer", "Tone Analyzer - Analyze Response: {0}", data);
}
```

[tone-analyzer]: https://console.bluemix.net/docs/services/tone-analyzer/index.html
