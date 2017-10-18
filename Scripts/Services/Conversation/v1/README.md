# Conversation

With the IBM Watson™ [Conversation][conversation] service, you can create an application that understands natural-language input and uses machine learning to respond to customers in a way that simulates a conversation between humans.

## Usage
You complete these steps to implement your application:

* Configure a workspace. With the easy-to-use graphical environment, you set up the dialog flow and training data for your application.

* Develop your application. You code your application to connect to the Conversation workspace through API calls. You then integrate your app with other systems that you need, including back-end systems and third-party services such as chat services or social media.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    Conversation _conversation = new Conversation(credentials);
}
```

### Message
Send a message to the Conversation instance
```cs
//  Send a simple message using a string
private void Message()
{
  if (!_conversation.Message(OnMessage, <workspace-id>, <input-string>))
    Log.Debug("ExampleConversation", "Failed to message!");
}

private void OnMessage(object resp, string data)
{
  Log.Debug("ExampleConversation", "Conversation: Message Response: {0}", data);
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

  if (!_conversation.Message(OnMessage, <workspace-id>, messageRequest))
    Log.Debug("ExampleConversation", "Failed to message!");
}

private void OnMessage(object resp, string data)
{
  Log.Debug("ExampleConversation", "Conversation: Message Response: {0}", data);
}
```
```cs
//  Send a message perserving conversation context
Dictionary<string, object> _context; // context to persist
//  Initiate a conversation
private void Message0()
{
  if (!_conversation.Message(OnMessage, <workspace-id>, <input-string0>))
    Log.Debug("ExampleConversation", "Failed to message!");
}

private void OnMessage0(object resp, string data)
{
  Log.Debug("ExampleConversation", "Conversation: Message Response: {0}", data);

  //  Set context for next round of messaging
  object _tempContext = null;
  (resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

  if (_tempContext != null)
      _context = _tempContext as Dictionary<string, object>;
  else
      Log.Debug("ExampleConversation", "Failed to get context");
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

  if (!_conversation.Message(OnMessage, <workspace-id>, messageRequest))
    Log.Debug("ExampleConversation", "Failed to message!");
}

private void OnMessage1(object resp, string data)
{
  Log.Debug("ExampleConversation", "Conversation: Message Response: {0}", data);
}
```

[conversation]: https://console.bluemix.net/docs/services/conversation/index.html
