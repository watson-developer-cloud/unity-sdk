### Alchemy API

The [AlchemyLanguage API][alchemy-api] uses natural language processing technology and machine learning algorithms to extract semantic meta-data from content, such as information on people, places, companies, topics, facts, relationships, authors, and languages.

### Usage
#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
Credentials credentials = new Credentials(_apikey, _url);
AlchemyAPI _alchemyApi = new AlchemyAPI(credentials);
```

#### Get Authors
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

#### Get Concepts
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

#### Get Date
You can extract Dates from a URL, HTML or Text source.
```cs
private void GetRankedConcepts()
{
  if(!_alchemyApi.GetDates(OnGetConcepts, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get concepts");
}

private void OnGetDatesHtml(DateData dates, string data)
{
  Log.Debug("ExampleAlchemyLanguage", "Alchemy Language - Get dates response html: {0}", data);
}
```

#### Get Emotions
You can get Emotions from a URL, HTML or Text source.
```cs
private void GetEmotions()
{
  if(!_alchemyApi.GetEmotions(OnGetEmotions, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get emotions");
}
```
#### Get Entities
You can extract Entities from a URL, HTML or Text source.
```cs
private void ExtractEntities()
{
  if(!_alchemyApi.ExtractEntities(OnExtractEntities, <text-or-url-or-htmlpage>))
    Log.Debug("ExampleAlchemyLanguage", "Failed to get emotions");
}
```
#### Get Feeds
You can detect RSS Feeds from a URL source.
```cs
private void GetFeeds()
{
  if(!m_AlchemyLanguage.DetectFeeds(OnDetectFeeds, <url-or-htmlpage>))
	    Log.Debug("ExampleAlchemyLanguage", "Failed to get feeds.");
}
```
#### Get Keywords
You can extract Keywords form a URL, HTML or Text source.
```cs
private void GetKeywords()
{
  if(!m_AlchemyLanguage.ExtractKeywords(OnExtractKeywords, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get keywords by URL POST");
}
```
#### Get Languages
You can extract the language of a URL, HTML or Text source.
```cs
private void GetLanguages()
{
  if(!m_AlchemyLanguage.GetLanguages(OnGetLanguages, <text-or-url>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get languages");
}
```
#### Get Microformats
You can get the Microformat of a URL source.
```cs
private void GetMicroformats()
{
  if(!m_AlchemyLanguage.GetMicroformats(OnGetMicroformats, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get microformats");
}
```
#### Get Publication Date
You can extract the publication date from a URL or HTML source.
```cs
private void GetPublicationDate()
{
  if(!m_AlchemyLanguage.GetPublicationDate(OnGetPublicationDate, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get publication dates");
}
```
#### Get Relations
You can extract Relations from a URL, HTML or Text source.
```cs
private void GetRelations()
{
  if(!m_AlchemyLanguage.GetRelations(OnGetRelations, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get relations");
}
```
#### Get Sentiment
You can extract the Sentiment from a URL, HTML or Text source.
```cs
private void GetTextSentiment()
{
  if(!m_AlchemyLanguage.GetTextSentiment(OnGetTextSentiment, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get sentiment");
}
```
#### Get Targeted Sentiment
You can extract a Targeted Sentiment from a URL, HTML or Text source. Targets are a pipe delimited list of target phrases.
```cs
private void GetTargetedSentiment()
{
  if(!m_AlchemyLanguage.GetTargetedSentiment(OnGetTargetedSentiment, <text-or-url-or-htmlpage>, <targets>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get targeted sentiment");
}
```
#### Get Taxonomy
You can get the Taxonomy of entities from a URL, HTML or Text source.
```cs
private void GetRankedTaxonomy()
{
  if(!m_AlchemyLanguage.GetRankedTaxonomy(OnGetRankedTaxonomy, <text-or-url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get ranked taxonomy");
}
```
#### Get Text
You can exctract the Text from a URL or HTML source.
```cs
private void GetText()
{
  if(!m_AlchemyLanguage.GetText(OnGetText, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get text by text");
}
```
#### Get Raw Text
You can exctract the Raw Text from a URL or HTML source.
```cs
private void GetRawText()
{
  if(!m_AlchemyLanguage.GetRawText(OnGetText, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get raw text by text");
}
```
#### Get Title
You can extract the Title form a URL or HTML source.
```cs
private void GetTitle()
{
  if(!m_AlchemyLanguage.GetTitle(OnGetTitle, <url-or-htmlpage>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get title by text POST");
}
```
#### Combined Call
You can combine multiple requests into one call using a Combined Data call from a URL, HTML or Text source. Allowed services in Combined Call are authors, concepts, dates, doc-emotion, entities, feeds, keywords, pub-dates, relations, doc-sentiment, taxonomy, title, page-image and image-keywords.
```cs
private void CombinedCall()
{
  if(!m_AlchemyLanguage.GetCombinedData(OnGetCombinedData, <text-or-url-or-htmlpage>, <includeSourceText>, <extractAuthors>, <extractConcepts>, <extractDates>, <extractDocEmotion>, <extractEntities>, <extractFeeds>, <extractKeywords>, <extractPubDate>, <extractRelations>, <extractDocSentiment>, <extractTaxonomy>, <extractTitle>, <extractPageImage>, <extractImageKeywords>))
      Log.Debug("ExampleAlchemyLanguage", "Failed to get combined data by text POST");
}
```

























#### Create workspace
Create a new workspace.
```cs
CreateWorkspace workspace = new CreateWorkspace()
{
    Name = <workspace-name>,
    Description = <workspace-description>,
    Language = <workspace-language>
};

var result = _conversation.CreateWorkspace(workspace);
```

#### Delete workspace
Delete an existing workspace.
```cs
var result = _conversation.DeleteWorkspace(<workspace-id>);
```

#### Get workspace details
Get detailed information about a specific workspace.
```cs
var result = _conversation.GetWorkspace(<workspace-id>);
```

#### Update workspace details
Update an existing workspace.
```cs
UpdateWorkspace updatedWorkspace = new UpdateWorkspace()
{
    Name = <updated-workspace-name>,
    Description = <updated-workspace-description>,
    Language = <updated-workspace-language>
};

var result = _conversation.UpdateWorkspace(<workspace-id>, updatedWorkspace);
```

#### Message
Get a response to a user's input.
```cs
//  create message request
MessageRequest messageRequest = new MessageRequest()
{
  Input = new InputData()
  {
    Text = <input-string>
  }
};

//  send a message to the conversation instance
var result = _conversation.Message(<workspace-id>, messageRequest);
```

#### List Counterexamples
List the counterexamples for a workspace. Counterexamples are examples that have been marked as irrelevant input.
```cs
var result = _conversation.ListCounterexamples(<workspaceId>);
```

#### Create Counterexamples
Add a new counterexample to a workspace. Counterexamples are examples that have been marked as irrelevant input.
```cs
CreateExample example = new CreateExample()
{
    Text = <counterExample>
};

var result = _conversation.CreateCounterexample(<workspaceId>, example);
```

#### Delete Counterexample
Delete a counterexample from a workspace. Counterexamples are examples that have been marked as irrelevant input.
```cs
var result = _conversation.DeleteCounterexample(<workspaceId>, <counterExample>);
```

#### Get Counterexample
Get information about a counterexample. Counterexamples are examples that have been marked as irrelevant input.
```cs
var result = _conversation.GetCounterexample(<workspaceId>, <counterExample>);
```

#### Update Counterexample
Update the text of a counterexample. Counterexamples are examples that have been marked as irrelevant input.
```cs
UpdateExample updatedExample = new UpdateExample()
{
    Text = <updatedCounterExample>
};

var result = _conversation.UpdateCounterexample(<workspaceId>, <counterExample>, updatedExample);
```

#### List Entities
List the entities for a workspace.
```cs
var result = _conversation.ListEntities(<workspaceId>);
```

#### Create Entity
Create a new entity.
```cs
CreateEntity entity = new CreateEntity()
{
    Entity = <entity>,
    Description = <entity-description>
};

var result = _conversation.CreateEntity(<workspaceId>, entity);
```

#### Delete Entity
Delete an entity from a workspace.
```cs
var result = _conversation.DeleteEntity(<workspaceId>, <entity>);
```

#### Get Entity
Get information about an entity, optionally including all entity content.
```cs
var result = _conversation.GetEntity(<workspaceId>, <entity>);
```

#### Update Entity
Update an existing entity with new or modified data. You must provide JSON data defining the content of the updated entity.

Any elements included in the new JSON will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are included in the new JSON.) For example, if you update the values for an entity, the previously existing values are discarded and replaced with the new values specified in the JSON input.
```cs
UpdateEntity updatedEntity = new UpdateEntity()
{
    Entity = updatedEntity,
    Description = updatedEntityDescription
};

var result = _conversation.UpdateEntity(<workspaceId>, <entity>, updatedEntity);
```

#### List Entity Values
List the values for an entity.
```cs
var result = _conversation.ListValues(<workspaceId>, <entity>);
```

#### Add Entity Value
Add a new value to an entity.
```cs
CreateValue value = new CreateValue()
{
    Value = <value>
};

var result = _conversation.CreateValue(<workspaceId>, <entity>, value);
```

#### Delete Entity Value
Delete a value from an entity.
```cs
var result = _conversation.DeleteValue(<workspaceId>, <entity>, <value>);
```

#### Get Entity Value
Get information about an entity value.
```cs
var result = _conversation.GetValue(<workspaceId>, <entity>, <value>);
```

#### Update Entity Value
Update an existing entity value with new or modified data. You must provide JSON data defining the content of the updated entity value.

Any elements included in the new JSON will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are included in the new JSON.) For example, if you update the synonyms for an entity value, the previously existing synonyms are discarded and replaced with the new synonyms specified in the JSON input.
```cs
UpdateValue updatedValue = new UpdateValue()
{
    Value = <updatedValue>
};

var result = _conversation.UpdateValue(<workspaceId>, <entity>, <value>, updatedValue);
```

#### List Synonyms
List the synonyms for an entity value.
```cs
var result = _conversation.ListSynonyms(<workspaceId>, <entity>, <value>);
```

#### Add Synonym
Add a new synonym to an entity value.
```cs
CreateSynonym synonym = new CreateSynonym()
{
    Synonym = <synonym>
};

var result = _conversation.CreateSynonym(<workspaceId>, <entity>, <value>, synonym);
```

#### Delete Synonym
Delete a synonym from an entity value.
```cs
var result = _conversation.DeleteSynonym(<workspaceId>, <entity>, <value>, <synonym>);
```

#### Get Synonym
Get information about a synonym of an entity value.
```cs
var result = _conversation.GetSynonym(<workspaceId>, <entity>, <value>, <synonym>);
```

#### Update Synonym
Update an existing entity value synonym with new text.
```cs
UpdateSynonym updatedSynonym = new UpdateSynonym()
{
    Synonym = <synonym>
};

var result = _conversation.UpdateSynonym(<workspaceId>, <entity>, <value>, <synonym>, updatedSynonym);
```

#### List Intents
List the intents for a workspace.
```cs
var result = _conversation.ListIntents(<workspaceId>);
```

#### Create Intent
Create a new intent.
```cs
CreateIntent intent = new CreateIntent()
{
    Intent = <intent>,
    Description = <intent-description>
};

var result = _conversation.CreateIntent(<workspaceId>, intent);
```

#### Delete Intent
Delete an intent from a workspace.
```cs
var result = _conversation.DeleteIntent(<workspaceId>, <intent>);
```

#### Get Intent
Get information about an intent, optionally including all intent content.
```cs
var result = _conversation.GetIntent(<workspaceId>, <intent>);
```

#### Update Intent
Update an existing intent with new or modified data. You must provide JSON data defining the content of the updated intent.

Any elements included in the new JSON will completely replace the equivalent existing elements, including all subelements. (Previously existing subelements are not retained unless they are included in the new JSON.) For example, if you update the user input examples for an intent, the previously existing examples are discarded and replaced with the new examples specified in the JSON input.
```cs
UpdateIntent intent = new UpdateIntent()
{
    Intent = <intent>,
    Description = <intent-description>
};

var result = _conversation.UpdateIntent(<workspaceId>, <intent>, intent);
```

#### List Examples
List the user input examples for an intent.
```cs
var result = _conversation.ListExamples(<workspaceId>, <intent>);
```

#### Create Example
Add a new user input example to an intent.
```cs
CreateExample example = new CreateExample()
{
    Text = <example>
};

var result = _conversation.CreateExample(<workspaceId>, <intent>, example);
```

#### Delete Example
Delete a user input example from an intent.
```cs
var result = _conversation.DeleteExample(<workspaceId>, <intent>, <example>);
```

#### Get Example
Get information about a user input example.
```cs
var result = _conversation.GetExample(<workspaceId>, <intent>, <example>);
```

#### Update Example
Update the text of a user input example.
```cs
UpdateExample updatedExample = new UpdateExample()
{
    Text = <example>
};

var result = _conversation.UpdateExample(<workspaceId>, <intent>, <example>, updatedExample);
```

#### List Log Events
List the events from the log of a workspace.
```cs
var result = _conversation.ListLogs(<workspaceId>);
```

[alchemy-api]:https://www.ibm.com/watson/developercloud/alchemy-language/api/v1/
