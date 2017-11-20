# Alchemy API

The [AlchemyLanguage API][alchemy-api] uses natural language processing technology and machine learning algorithms to extract semantic meta-data from content, such as information on people, places, companies, topics, facts, relationships, authors, and languages.

## Usage
### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.AlchemyAPI.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<apikey>, <url>);
    AlchemyAPI _alchemyApi = new AlchemyAPI(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleAlchemyLanguage.OnFail()", "Error received: {0}", error.ToString());
}
```

### Get Authors
You can extract Authors from a URL or HTML source.
```cs
private void GetAuthors()
{
  if(!_alchemyApi.GetAuthors(OnGetAuthors, OnFail, <url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage.GetAuthors()", "Failed to get authors");
}

private void OnGetAuthors(AuthorsData authors, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetAuthors()", "Alchemy Language - Get authors response html: {0}", customData["json"].ToString());
}
```

### Get Concepts
You can get Concepts from a URL, HTML or Text source.
```cs
private void GetRankedConcepts()
{
  if(!_alchemyApi.GetRankedConcepts(OnGetConcepts, OnFail, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage.GetRankedConcepts()", "Failed to get concepts");
}

private void OnGetConcepts(ConceptsData concepts, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAlchemyLanguage.OnGetConcepts()", "Alchemy Language - Get ranked concepts response text: {0}", customData["json"].ToString());
}
```

### Get Date
You can extract Dates from a URL, HTML or Text source.
```cs
private void GetRankedConcepts()
{
  if(!_alchemyApi.GetDates(OnGetConcepts, OnFail, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage.GetRankedConcepts()", "Failed to get concepts");
}

private void OnGetDates(DateData dates, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAlchemyLanguage.OnGetDates()", "Alchemy Language - Get dates response html: {0}", customData["json"].ToString());
}
```

### Get Emotions
You can get Emotions from a URL, HTML or Text source.
```cs
private void GetEmotions()
{
  if(!_alchemyApi.GetEmotions(OnGetEmotions, OnFail, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage.GetEmotions()", "Failed to get emotions");
}

private void OnGetEmotions(EmotionData emotions, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetEmotions()", "Alchemy Language - Get emotions response html: {0}", customData["json"].ToString());
}
```

### Get Entities
You can extract Entities from a URL, HTML or Text source.
```cs
private void ExtractEntities()
{
  if(!_alchemyApi.ExtractEntities(OnExtractEntities, OnFail, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage.ExtractEntities()", "Failed to get emotions");
}

private void OnExtractEntities(EntityData entityData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnExtractEntities()", "Alchemy Language - Extract entities response html: {0}", customData["json"].ToString());
}
```

### Get Feeds
You can detect RSS Feeds from a URL source.
```cs
private void GetFeeds()
{
  if(!m_AlchemyLanguage.DetectFeeds(OnDetectFeeds, OnFail, <url-or-htmlpage>))
	    Log.Debug("ExampleAlchemyLanguage.GetFeeds()", "Failed to get feeds.");
}

private void OnDetectFeeds(FeedData feedData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnDetectFeeds()", "Alchemy Language - Detect feeds response url: {0}", customData["json"].ToString());
}
```

### Get Keywords
You can extract Keywords form a URL, HTML or Text source.
```cs
private void GetKeywords()
{
  if(!m_AlchemyLanguage.ExtractKeywords(OnExtractKeywords, OnFail, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetKeywords()", "Failed to get keywords by URL POST");
}

private void OnExtractKeywords(KeywordData keywordData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnExtractKeywords()", "Alchemy Language - Extract keywords response html: {0}", customData["json"].ToString());
}
```

### Get Languages
You can extract the language of a URL, HTML or Text source.
```cs
private void GetLanguages()
{
  if(!m_AlchemyLanguage.GetLanguages(OnGetLanguages, OnFail, <text-or-url>))
      Log.Debug("ExampleAlchemyLanguage.GetLanguages()", "Failed to get languages");
}

private void OnGetLanguages(LanguageData languages, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetLanguages()", "Alchemy Language - Get languages response html: {0}", customData["json"].ToString());
}
```

### Get Microformats
You can get the Microformat of a URL source.
```cs
private void GetMicroformats()
{
  if(!m_AlchemyLanguage.GetMicroformats(OnGetMicroformats, OnFail, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetMicroformats()", "Failed to get microformats");
}

private void OnGetMicroformats(MicroformatData microformats, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetMicroformats()", "Alchemy Language - Get microformats response url: {0}", customData["json"].ToString());
}
```

### Get Publication Date
You can extract the publication date from a URL or HTML source.
```cs
private void GetPublicationDate()
{
  if(!m_AlchemyLanguage.GetPublicationDate(OnGetPublicationDate, OnFail, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetPublicationDate()", "Failed to get publication dates");
}

private void OnGetPublicationDate(PubDateData pubDates, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetPublicationDate()", "Alchemy Language - Get publication date response url: {0}", customData["json"].ToString());
}
```

### Get Relations
You can extract Relations from a URL, HTML or Text source.
```cs
private void GetRelations()
{
  if(!m_AlchemyLanguage.GetRelations(OnGetRelations, OnFail, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetRelations()", "Failed to get relations");
}

private void OnGetRelations(RelationsData relationsData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetRelations()", "Alchemy Language - Get relations response html: {0}", customData["json"].ToString());
}
```

### Get Sentiment
You can extract the Sentiment from a URL, HTML or Text source.
```cs
private void GetTextSentiment()
{
  if(!m_AlchemyLanguage.GetTextSentiment(OnGetTextSentiment, OnFail, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetTextSentiment()", "Failed to get sentiment");
}

private void OnGetTextSentiment(SentimentData sentimentData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetTextSentiment()", "Alchemy Language - Get text sentiment response html: {0}", customData["json"].ToString());
}
```

### Get Targeted Sentiment
You can extract a Targeted Sentiment from a URL, HTML or Text source. Targets are a pipe delimited list of target phrases.
```cs
private void GetTargetedSentiment()
{
  if(!m_AlchemyLanguage.GetTargetedSentiment(OnGetTargetedSentiment, OnFail, <text-or-url-or-htmlpage>, <targets>))
      Log.Debug("ExampleAlchemyLanguage.GetTargetedSentiment()", "Failed to get targeted sentiment");
}

private void OnGetTargetedSentiment(TargetedSentimentData sentimentData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetTargetedSentiment()", "Alchemy Language - Get targeted sentiment response html: {0}", customData["json"].ToString());
}
```

### Get Taxonomy
You can get the Taxonomy of entities from a URL, HTML or Text source.
```cs
private void GetRankedTaxonomy()
{
  if(!m_AlchemyLanguage.GetRankedTaxonomy(OnGetRankedTaxonomy, OnFail, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetRankedTaxonomy()", "Failed to get ranked taxonomy");
}

private void OnGetRankedTaxonomy(TaxonomyData taxonomyData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetRankedTaxonomy()", "Alchemy Language - Get ranked taxonomy response html: {0}", customData["json"].ToString());
}
```

### Get Text
You can exctract the Text from a URL or HTML source.
```cs
private void GetText()
{
  if(!m_AlchemyLanguage.GetText(OnGetText, OnFail, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetText()", "Failed to get text by text");
}

private void OnGetText(TextData textData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetText()", "Alchemy Language - Get Text HTML response: {0}", customData["json"].ToString());
}
```

### Get Raw Text
You can exctract the Raw Text from a URL or HTML source.
```cs
private void GetRawText()
{
  if(!m_AlchemyLanguage.GetRawText(OnGetText, OnFail, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetRawText()", "Failed to get raw text by text");
}

private void OnGetRawText(TextData textData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetRawText()", "Alchemy Language - Get raw text HTML response: {0}", customData["json"].ToString());
}
```

### Get Title
You can extract the Title form a URL or HTML source.
```cs
private void GetTitle()
{
  if(!m_AlchemyLanguage.GetTitle(OnGetTitle, OnFail, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage.GetTitle()", "Failed to get title by text POST");
}

private void OnGetTitle(Title titleData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetTitle()", "Alchemy Language - Get Title Url response: {0}", customData["json"].ToString());
}
```

### Combined Call
You can combine multiple requests into one call using a Combined Data call from a URL, HTML or Text source. Allowed services in Combined Call are authors, concepts, dates, doc-emotion, entities, feeds, keywords, pub-dates, relations, doc-sentiment, taxonomy, title, page-image and image-keywords.
```cs
private void CombinedCall()
{
  if(!m_AlchemyLanguage.GetCombinedData(OnGetCombinedData, OnFail, <text-or-url-or-htmlpage>, <includeSourceText>, <extractAuthors>, <extractConcepts>, <extractDates>, <extractDocEmotion>, <extractEntities>, <extractFeeds>, <extractKeywords>, <extractPubDate>, <extractRelations>, <extractDocSentiment>, <extractTaxonomy>, <extractTitle>, <extractPageImage>, <extractImageKeywords>))
      Log.Debug("ExampleAlchemyLanguage.CombinedCall()", "Failed to get combined data by text POST");
}

private void OnGetCombinedData(CombinedCallData combinedData, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAlchemyLanguage.OnGetCombinedData()", "Alchemy Language - Get Combined Data Text response: {0}", customData["json"].ToString());
}
```

[alchemy-api]:https://www.ibm.com/watson/developercloud/alchemy-language/api/v1/
