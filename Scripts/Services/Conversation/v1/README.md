# Conversation

**Conversation V1 is deprecated and will be removed in the next major release of the SDK. Use Assistant V1 or Assistant V2.**

With the IBM Watson™ [Conversation][conversation] service, you can create an application that understands natural-language input and uses machine learning to respond to customers in a way that simulates a conversation between humans.

## Usage
You complete these steps to implement your application:

* Configure a workspace. With the easy-to-use graphical environment, you set up the dialog flow and training data for your application.

* Develop your application. You code your application to connect to the Conversation workspace through API calls. You then integrate your app with other systems that you need, including back-end systems and third-party services such as chat services or social media.

### Message
Send a message to the Conversation instance
```cs
//  Send a simple message using a string
private void Message()
{
  if (!_conversation.Message(OnMessage, OnFail, <workspace-id>, <input-string>))
    Log.Debug("ExampleConversation.Message()", "Failed to message!");
}

private void OnMessage(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleConversation.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());
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

  if (!_conversation.Message(OnMessage, OnFail, <workspace-id>, messageRequest))
    Log.Debug("ExampleConversation.Message()", "Failed to message!");
}

private void OnMessage(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleConversation.OnMessage()", "Conversation: Message Response: {0}", customData["json"].ToString());
}
```
```cs
//  Send a message perserving conversation context
Dictionary<string, object> _context; // context to persist
//  Initiate a conversation
private void Message0()
{
  if (!_conversation.Message(OnMessage, OnFail, <workspace-id>, <input-string0>))
    Log.Debug("ExampleConversation.Message()", "Failed to message!");
}

private void OnMessage0(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleConversation.OnMessage0()", "Conversation: Message Response: {0}", customData["json"].ToString());

  //  Set context for next round of messaging
  object _tempContext = null;
  (resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

  if (_tempContext != null)
      _context = _tempContext as Dictionary<string, object>;
  else
      Log.Debug("ExampleConversation.OnMessage0()", "Failed to get context");
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

  if (!_conversation.Message(OnMessage, OnFail, <workspace-id>, messageRequest))
    Log.Debug("ExampleConversation.Message1()", "Failed to message!");
}

private void OnMessage1(object resp, Dictionary<string, object> customData)
{
  Log.Debug("ExampleConversation.OnMessage1()", "Conversation: Message Response: {0}", customData["json"].ToString());
}
```

[conversation]: https://console.bluemix.net/docs/services/conversation/index.html
