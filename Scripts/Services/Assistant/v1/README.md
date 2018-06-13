# Assistant

The IBM Watson™ [Assistant][assistant] service combines machine learning, natural language understanding, and integrated dialog tools to create conversation flows between your apps and your users.

## Usage
You complete these steps to implement your application:

* Configure a workspace. With the easy-to-use graphical environment, you set up the dialog flow and training data for your application.

* Develop your application. You code your application to connect to the Assistant workspace through API calls. You then integrate your app with other systems that you need, including back-end systems and third-party services such as chat services or social media.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    Assistant _assistant = new Assistant(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleAssistant.OnFail()", "Error received: {0}", error.ToString());
}
```

### Message
Send a message to the Assistant instance
```cs
//  Send a simple message using a string
private void Message()
{
  if (!_assistant.Message(OnMessage, OnFail, <workspace-id>, <input-string>))
    Log.Debug("ExampleAssistant.Message()", "Failed to message!");
}

private void OnMessage(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAssistant.OnMessage()", "Assistant: Message Response: {0}", customData["json"].ToString());
}
```
```cs
//  Send a message using a MessageRequest object
private void Message()
{
  MessageRequest messageRequest = new MessageRequest()
  {
    input = new Dictionary<string, object>()
    {
        { "text", <input-string> }
    }
  };

  if (!_assistant.Message(OnMessage, OnFail, <workspace-id>, messageRequest))
    Log.Debug("ExampleAssistant.Message()", "Failed to message!");
}

private void OnMessage(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAssistant.OnMessage()", "Assistant: Message Response: {0}", customData["json"].ToString());
}
```
```cs
//  Send a message perserving conversation context
Dictionary<string, object> _context; // context to persist

//  Initiate a conversation
private void Message0()
{
  if (!_assistant.Message(OnMessage, OnFail, <workspace-id>, <input-string0>))
    Log.Debug("ExampleAssistant.Message()", "Failed to message!");
}

private void OnMessage0(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAssistant.OnMessage0()", "Assistant: Message Response: {0}", customData["json"].ToString());

  //  Set context for next round of messaging
  object _tempContext = null;
  (resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

  if (_tempContext != null)
      _context = _tempContext as Dictionary<string, object>;
  else
      Log.Debug("ExampleAssistant.OnMessage0()", "Failed to get context");
}

private void Message1()
{
  MessageRequest messageRequest = new MessageRequest()
  {
    input = new Dictionary<string, object>()
    {
        { "text", <input-string1> }
    },
    context = _context
  };

  if (!_assistant.Message(OnMessage, OnFail, <workspace-id>, messageRequest))
    Log.Debug("ExampleAssistant.Message1()", "Failed to message!");
}

private void OnMessage1(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleAssistant.OnMessage1()", "Assistant: Message Response: {0}", customData["json"].ToString());
}
```

[assistant]: https://console.bluemix.net/docs/services/assistant/index.html
