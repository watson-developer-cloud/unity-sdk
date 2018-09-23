# Assistant V1

The IBM Watson™ [Assistant][assistant] service combines machine learning, natural language understanding, and integrated dialog tools to create conversation flows between your apps and your users.

## Usage
You complete these steps to implement your application:

* Configure a workspace. With the easy-to-use graphical environment, you set up the dialog flow and training data for your application.

* Develop your application. You code your application to connect to the Assistant workspace through API calls. You then integrate your app with other systems that you need, including back-end systems and third-party services such as chat services or social media.

### Message
Send a message to the Assistant instance

- Send a message using a MessageRequest object
```cs
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


- Send a message perserving conversation context
```cs
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


- Send a message perserving conversation context - Extract code from [ExampleAssistant.cs](https://github.com/watson-developer-cloud/unity-sdk/blob/develop/Examples/ServiceExamples/Scripts/ExampleAssistant.cs)
```cs

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

private void OnMessage(object response, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleAssistant.OnMessage()", "Response: {0}", customData["json"].ToString());

        //  Convert resp to fsdata
        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsdata to MessageResponse
        MessageResponse messageResponse = new MessageResponse();
        object obj = messageResponse;
        r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Set context for next round of messaging
        object _tempContext = null;
        (response as Dictionary<string, object>).TryGetValue("context", out _tempContext);

        if (_tempContext != null)
            _context = _tempContext as Dictionary<string, object>;
        else
            Log.Debug("ExampleAssistant.OnMessage()", "Failed to get context");

        //  Get intent
        object tempIntentsObj = null;
        (response as Dictionary<string, object>).TryGetValue("intents", out tempIntentsObj);
        object tempIntentObj = (tempIntentsObj as List<object>)[0];
        object tempIntent = null;
        (tempIntentObj as Dictionary<string, object>).TryGetValue("intent", out tempIntent);
        string intent = tempIntent.ToString();

        Log.Debug("ExampleAssistant.OnMessage()", "intent: {0}", intent);

        _messageTested = true;
}

```

[assistant]: https://console.bluemix.net/docs/services/assistant/index.html
