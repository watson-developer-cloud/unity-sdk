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

### Get Authors
You can extract Authors from a URL or HTML source.
```cs
private void GetAuthors()
{
  if(!_alchemyApi.GetAuthors(OnGetAuthors, <url-or-htmlpage>))
    Log.Debug("Failed to get authors");
}

private void OnGetAuthors(AuthorsData authors, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get authors response html: {0}", data);
}
```

### Get Concepts
You can get Concepts from a URL, HTML or Text source.
```cs
private void GetRankedConcepts()
{
  if(!_alchemyApi.GetRankedConcepts(OnGetConcepts, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts");
}

private void OnGetConcepts(ConceptsData concepts, string data)
{
  Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get ranked concepts response text: {0}", data);
}
```

### Get Date
You can extract Dates from a URL, HTML or Text source.
```cs
private void GetRankedConcepts()
{
  if(!_alchemyApi.GetDates(OnGetConcepts, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts");
}

private void OnGetDates(DateData dates, string data)
{
  Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get dates response html: {0}", data);
}
```

### Get Emotions
You can get Emotions from a URL, HTML or Text source.
```cs
private void GetEmotions()
{
  if(!_alchemyApi.GetEmotions(OnGetEmotions, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get emotions");
}

private void OnGetEmotions(EmotionData emotions, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get emotions response html: {0}", data);
}
```

### Get Entities
You can extract Entities from a URL, HTML or Text source.
```cs
private void ExtractEntities()
{
  if(!_alchemyApi.ExtractEntities(OnExtractEntities, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get emotions");
}

private void OnExtractEntities(EntityData entityData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Extract entities response html: {0}", data);
}
```

### Get Feeds
You can detect RSS Feeds from a URL source.
```cs
private void GetFeeds()
{
  if(!m_AlchemyLanguage.DetectFeeds(OnDetectFeeds, <url-or-htmlpage>))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get feeds.");
}

private void OnDetectFeeds(FeedData feedData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Detect feeds response url: {0}", data);
}
```

### Get Keywords
You can extract Keywords form a URL, HTML or Text source.
```cs
private void GetKeywords()
{
  if(!m_AlchemyLanguage.ExtractKeywords(OnExtractKeywords, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get keywords by URL POST");
}

private void OnExtractKeywords(KeywordData keywordData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Extract keywords response html: {0}", data);
}
```

### Get Languages
You can extract the language of a URL, HTML or Text source.
```cs
private void GetLanguages()
{
  if(!m_AlchemyLanguage.GetLanguages(OnGetLanguages, <text-or-url>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get languages");
}

private void OnGetLanguages(LanguageData languages, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get languages response html: {0}", data);
}
```

### Get Microformats
You can get the Microformat of a URL source.
```cs
private void GetMicroformats()
{
  if(!m_AlchemyLanguage.GetMicroformats(OnGetMicroformats, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get microformats");
}

private void OnGetMicroformats(MicroformatData microformats, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get microformats response url: {0}", data);
}
```

### Get Publication Date
You can extract the publication date from a URL or HTML source.
```cs
private void GetPublicationDate()
{
  if(!m_AlchemyLanguage.GetPublicationDate(OnGetPublicationDate, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get publication dates");
}

private void OnGetPublicationDate(PubDateData pubDates, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get publication date response url: {0}", data);
}
```

### Get Relations
You can extract Relations from a URL, HTML or Text source.
```cs
private void GetRelations()
{
  if(!m_AlchemyLanguage.GetRelations(OnGetRelations, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get relations");
}

private void OnGetRelations(RelationsData relationsData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get relations response html: {0}", data);
}
```

### Get Sentiment
You can extract the Sentiment from a URL, HTML or Text source.
```cs
private void GetTextSentiment()
{
  if(!m_AlchemyLanguage.GetTextSentiment(OnGetTextSentiment, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get sentiment");
}

private void OnGetTextSentiment(SentimentData sentimentData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get text sentiment response html: {0}", data);
}
```

### Get Targeted Sentiment
You can extract a Targeted Sentiment from a URL, HTML or Text source. Targets are a pipe delimited list of target phrases.
```cs
private void GetTargetedSentiment()
{
  if(!m_AlchemyLanguage.GetTargetedSentiment(OnGetTargetedSentiment, <text-or-url-or-htmlpage>, <targets>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get targeted sentiment");
}

private void OnGetTargetedSentiment(TargetedSentimentData sentimentData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get targeted sentiment response html: {0}", data);
}
```

### Get Taxonomy
You can get the Taxonomy of entities from a URL, HTML or Text source.
```cs
private void GetRankedTaxonomy()
{
  if(!m_AlchemyLanguage.GetRankedTaxonomy(OnGetRankedTaxonomy, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get ranked taxonomy");
}

private void OnGetRankedTaxonomy(TaxonomyData taxonomyData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get ranked taxonomy response html: {0}", data);
}
```

### Get Text
You can exctract the Text from a URL or HTML source.
```cs
private void GetText()
{
  if(!m_AlchemyLanguage.GetText(OnGetText, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get text by text");
}

private void OnGetText(TextData textData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get Text HTML response: {0}", data);
}
```

### Get Raw Text
You can exctract the Raw Text from a URL or HTML source.
```cs
private void GetRawText()
{
  if(!m_AlchemyLanguage.GetRawText(OnGetText, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get raw text by text");
}

private void OnGetRawText(TextData textData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get raw text HTML response: {0}", data);
}
```

### Get Title
You can extract the Title form a URL or HTML source.
```cs
private void GetTitle()
{
  if(!m_AlchemyLanguage.GetTitle(OnGetTitle, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get title by text POST");
}

private void OnGetTitle(Title titleData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get Title Url response: {0}", data);
}
```

### Combined Call
You can combine multiple requests into one call using a Combined Data call from a URL, HTML or Text source. Allowed services in Combined Call are authors, concepts, dates, doc-emotion, entities, feeds, keywords, pub-dates, relations, doc-sentiment, taxonomy, title, page-image and image-keywords.
```cs
private void CombinedCall()
{
  if(!m_AlchemyLanguage.GetCombinedData(OnGetCombinedData, <text-or-url-or-htmlpage>, <includeSourceText>, <extractAuthors>, <extractConcepts>, <extractDates>, <extractDocEmotion>, <extractEntities>, <extractFeeds>, <extractKeywords>, <extractPubDate>, <extractRelations>, <extractDocSentiment>, <extractTaxonomy>, <extractTitle>, <extractPageImage>, <extractImageKeywords>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get combined data by text POST");
}

private void OnGetCombinedData(CombinedCallData combinedData, string data)
{
    Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get Combined Data Text response: {0}", data);
}
```

[alchemy-api]:https://www.ibm.com/watson/developercloud/alchemy-language/api/v1/
