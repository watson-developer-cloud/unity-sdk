[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.PersonalityInsights.v3/)

### Personality Insights

The IBM Watson™ [Personality Insights][personality-insights] service enables applications to derive insights from social media, enterprise data, or other digital communications. The service uses linguistic analytics to infer individuals' intrinsic personality characteristics, including Big Five, Needs, and Values, from digital communications such as email, text messages, tweets, and forum posts.

The service can automatically infer, from potentially noisy social media, portraits of individuals that reflect their personality characteristics. The service can infer consumption preferences based on the results of its analysis and, for JSON content that is timestamped, can report temporal behavior.

For information about the meaning of the models that the service uses to describe personality characteristics, see [Personality models][personality-models]. For information about the meaning of the consumption preferences, see [Consumption preferences][consumption-preferences].

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.PersonalityInsights.v3

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.PersonalityInsights.v3": "1.1.0"
}

```
### Usage
The service offers a single `profile` method that accepts up to 20 MB of input data and produces results in JSON or CSV format. The service accepts input in Arabic, English, Japanese, or Spanish and can produce output in a variety of languages.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Personality Insights Service instance
PersonalityInsightsService _personalityInsights = new PersonalityInsightsService();

// set the credentials
_personalityInsights.SetCredential("<username>", "<password>");
```

#### Profile
Extract personality characteristics based on how a person writes.
```cs
ContentListContainer contentListContainer = new ContentListContainer()
{
    ContentItems = new List<ContentItem>()
    {
        new ContentItem()
        {
            Contenttype = ContentItem.ContenttypeEnum.TEXT_PLAIN,
            Language = ContentItem.LanguageEnum.EN,
            Content = <content-to-analyze>
        }
    }
};

 var result = _personalityInsights.Profile("text/plain", "application/json", contentListContainer, rawScores: true, consumptionPreferences:true, csvHeaders:true);
```

[personality-insights]: https://www.ibm.com/watson/developercloud/personality-insights.html
[personality-models]: https://www.ibm.com/watson/developercloud/doc/personality-insights/models.html
[consumption-preferences]:https://www.ibm.com/watson/developercloud/doc/personality-insights/preferences.html
