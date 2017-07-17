[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.Conversation.v1/)

### Conversation

With the IBM Watson™ [Conversation][conversation] service, you can create an application that understands natural-language input and uses machine learning to respond to customers in a way that simulates a conversation between humans.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.Conversation.v1

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.Conversation.v1": "1.1.0"
}

```
### Usage
You complete these steps to implement your application:

* Configure a workspace. With the easy-to-use graphical environment, you set up the dialog flow and training data for your application.

* Develop your application. You code your application to connect to the Conversation workspace through API calls. You then integrate your app with other systems that you need, including back-end systems and third-party services such as chat services or social media.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Language Translator Service instance
ConversationService _conversation = new ConversationService();

// set the credentials
_conversation.SetCredential(<username>, <password>);
```

#### List workspaces
List existing workspaces for the service instance.
```cs
var result = _conversation.ListWorkspaces();
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

[conversation]:https://www.ibm.com/watson/developercloud/doc/conversation/index.html
