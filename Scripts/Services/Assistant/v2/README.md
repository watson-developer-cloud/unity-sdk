# Assistant V2  

**Watson Assistant v2 API is released in beta. For details, see the ["Introducing Watson Assistant"](https://www.ibm.com/blogs/watson/2018/03/the-future-of-watson-conversation-watson-assistant/) blog post.**

The IBM Watson™ [Assistant][assistant] service combines machine learning, natural language understanding, and integrated dialog tools to create conversation flows between your apps and your users.

## Usage
You complete these steps to implement your application:

* Create an assistant.

* Add skills to your assistant. Choose the appropriate skill set for each assistant that you want to build.

    Note: Currently, you can add one conversational skill to the assistant.

* Configure the skill. For a conversational skill, use the intuitive graphical tool to define the training data and dialog for the conversation between your assistant and your customers.

    The training data consists of the following artifacts:

    * Intents: Goals that you anticipate your users will have when they interact with the service. Define one intent for each goal that can be identified in a user's input. For example, you might define an intent named store_hours that answers questions about store hours. For each intent, you add sample utterances that reflect the input customers might use to ask for the information they need, such as, What time do you open?

        Or use prebuilt content catalogs provided by IBM to get started with data that addresses common customer goals.

    * Entities: An entity represents a term or object that provides context for an intent. For example, an entity might be a city name that helps your dialog to distinguish which store the user wants to know store hours for.

        As you add training data, a natural language classifier is automatically added to the skill, and is trained to understand the types of requests that you have indicated the service should listen for and respond to.

    * Dialog: Use the dialog tool to build a dialog flow that incorporates your intents and entities. The dialog flow is represented graphically in the tool as a tree. You can add a branch to process each of the intents that you want the service to handle. You can then add branch nodes that handle the many possible permutations of a request based on other factors, such as the entities found in the user input or information that is passed to the service from an external service.

* Integrate your assistant. Create a channel integration to deploy the configured assistant directly to a social media or messaging channel.

#### Create session
Create a new session. A session is used to send user input to a skill and receive responses. It also maintains the state of the conversation.
```cs
private void Example()
{
    _assistant.CreateSession(OnCreateSession, OnFail, assistantId);
}

private void OnCreateSession(SessionResponse response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAssistantV2.OnMessage()", "Assistant: Create Session Response: {0}", customData["json"].ToString());
}
```

#### Delete Session
Deletes a session explicitly before it times out.
```cs
private void Example()
{
    _assistant.DeleteSession(OnDeleteSession, OnFail, assistantId, sessionId);
}

private void OnDeleteSession(object response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAssistantV2.OnMessage()", "Assistant: Delete Session Response: {0}", customData["json"].ToString());
}
```

#### Message
Send user input to an assistant and receive a response. There is no rate limit for this operation.
```cs
private void Example()
{
    MessageRequest messageRequest = new MessageRequest()
    {
        Input = new MessageInput()
        {
            Text = "conversation text"
        }
    };

    _assistant.Message(OnMessage, OnFail, assistantId, sessionId, messageRequest);
}

private void OnMessage(MessageResponse response, Dictionary<string, object> customData)
{
    Log.Debug("ExampleAssistantV2.OnMessage()", "Assistant: Message Response: {0}", customData["json"].ToString());    
}
```


[assistant]: https://console.bluemix.net/docs/services/assistant/index.html
