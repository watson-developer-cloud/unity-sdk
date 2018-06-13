# Text to Speech

The IBM® [Text to Speech][text-to-speech] service provides an API that uses IBM's speech-synthesis capabilities to synthesize text into natural-sounding speech in a variety of languages, accents, and voices. The service supports at least one male or female voice, sometimes both, for each language. The audio is streamed back to the client with minimal delay.

The Text to Speech API consists of the following groups of related calls:

* Voices includes methods that provide information about the voices available for synthesized speech.

* Synthesis includes methods that synthesize written text to spoken audio over the HTTP protocol. The calls support plain text and SSML input.

* WebSockets includes a method that synthesizes text to audio over the WebSocket protocol. The call supports plain text and SSML input, including the `<mark>` element as well as word timing information for all strings of the input text.

* Pronunciation includes a single method that returns the pronunciation for a specified word.

* Custom models provides methods for creating custom voice models. Custom models let users create a dictionary of words and their translations for use in speech synthesis.

* Custom words provides methods that let users manage the word/translation pairs in a custom voice model.

## Usage
The following usage information pertains to many of the calls:

* Many calls refer to the Speech Synthesis Markup Language (SSML), an XML-based markup language that provides annotations of text for speech-synthesis applications; for example, many methods accept or produce translations that use an SSML-based phoneme format. For more information about support for SSML, see [Using SSML][using-ssml] and [Using SPRs][using-sprs].

* The pronunciation and customization calls accept or return a Globally Unique Identifier (GUID); for example, customization IDs and service credentials are GUIDs. GUIDs are hexadecimal strings that have the format xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.

* Many customization calls accept or return sounds-like or phonetic translations for words. A phonetic translation is based on the SSML format for representing the phonetic string of a word. Phonetic translations can occur in one of two formats: the standard International Phonetic Alphabet (IPA) representation, for example

```xml

<phoneme alphabet=\"ipa\" ph=\"təmˈɑto\"></phoneme>

```
or the proprietary IBM Symbolic Phonetic Representation (SPR), for example

```xml

<phoneme alphabet=\"ibm\" ph=\"1gAstroEntxrYFXs\"></phoneme>

```

For more information about customization and about sounds-like and phonetic translations, see [Understanding customization][understanding-customization] and [Using customization][using-customization].

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    TextToSpeech _textToSpeech = new TextToSpeech(credentials);
}
```

### Fail handler
These examples use a common fail handler.
```cs
private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
{
    Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
}
```




### Get voices
Retrieves a list of all voices available for use with the service. The information includes the voice's name, language, and gender, among other things. To see information about a specific voice, use the Get a voice method.
```cs
private void GetVoices()
{
  if(!_textToSpeech.GetVoices(OnGetVoices, OnFail))
    Log.Debug("ExampleTextToSpeech.GetVoices()", "Failed to get voices!");
}

private void OnGetVoices(Voices voices, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetVoices()", "Text to Speech - Get voices response: {0}", customData["json"].ToString());
}
```






### Get a voice
Lists information about the specified voice. Specify a customization_id to obtain information for that custom voice model of the specified voice. To see information about all available voices, use the Get voices method.
```cs
private void GetVoice()
{
  if(!_textToSpeech.GetVoice(OnGetVoice, OnFail, <voicetype>))
    Log.Debug("ExampleTextToSpeech.GetVoice()", "Failed to get voice!");
}

private void OnGetVoice(Voice voice, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetVoice()", "Text to Speech - Get voice response: {0}", customData["json"].ToString());
}
```






### Synthesize audio using file
Synthesizes text to spoken audio, returning the synthesized audio stream as an array of bytes. You can use two request methods to synthesize audio:

* The HTTP GET request method passes shorter text via a query parameter. The text size is limited by the maximum length of the HTTP request line and headers (about 6 KB) or by system limits, whichever is less.

* The HTTP POST request method passes longer text in the body of the request. Text size is limited to 5 KB.

With either request method, you can provide plain text or text that is annotated with SSML.

```cs
private void Synthesize()
{
  _textToSpeech.Voice = <voice-type>;
  if(!_textToSpeech.ToSpeech(OnSynthesize, OnFail, <text-to-synthesize>, <use-post>))
    Log.Debug("ExampleTextToSpeech.ToSpeech()", "Failed to synthesize!");
}

private void OnSynthesize(AudioClip clip, Dictionary<string, object> customData)
{
  PlayClip(clip);
}

private void PlayClip(AudioClip clip)
{
  if (Application.isPlaying && clip != null)
  {
    GameObject audioObject = new GameObject("AudioObject");
    AudioSource source = audioObject.AddComponent<AudioSource>();
    source.spatialBlend = 0.0f;
    source.loop = false;
    source.clip = clip;
    source.Play();

    Destroy(audioObject, clip.length);
  }
}
```






<!-- ### Synthesize audio using websockets
Synthesizes text to spoken audio over a WebSocket connection. The synthesize method establishes a connection with the service. You then send the text to be synthesized to the service as a JSON text message over the connection, and the service returns the audio as a binary stream of data.

You can provide a maximum of 5 KB of either plain text or text that is annotated with SSML. You can use the SSML <mark> element to request the location of the marker in the audio stream. You can also request word timing information in the form of start and end times for all strings of the input text. Mark and word timing results are sent as text messages over the connection.
```cs
``` -->






### Get pronunciation
Returns the phonetic pronunciation for the specified word. You can request the pronunciation for a specific format. You can also request the pronunciation for a specific voice to see the default translation for the language of that voice or for a specific custom voice model to see the translation for that voice model.
```cs
private void GetPronunciation()
{
  if(!_textToSpeech.GetPronunciation(OnGetPronunciation, OnFail, <word>, <voicetype>))
    Log.Debug("ExampleTextToSpeech.GetPronunciation()", "Failed to get pronunication!");
}

private void OnGetPronunciation(Pronunciation pronunciation, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetPronunciation()", "Text to Speech - Get pronunciation response: {0}", customData["json"].ToString());
}
```






### Create a voice model
Creates a new empty custom voice model that is owned by the requesting user.
```cs
private void CreateCustomization()
{
  if(!_textToSpeech.CreateCustomization(OnCreateCustomization, OnFail, <customization-name>, <customization-language>, <customization-description>))
    Log.Debug("ExampleTextToSpeech.CreateCustomization()", "Failed to create customization!");
}

private void OnCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnCreateCustomization()", "Text to Speech - Create customization response: {0}", customData["json"].ToString());
}
```






### Update a voice model
Updates information for the specified custom voice model. You can update the metadata such as the name and description of the voice model. You can also update the words in the model and their translations. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to update the model. If no modelID is provided, a new custom model will be created.
```cs
private void UpdateCustomization()
{
  CustomVoiceUpdate _customVoiceUpdate = new CustomVoiceUpdate()
  {
    words = <customization-words>,
    description = <customization-description>,
    name = <customization-name>
  }

  if(!_textToSpeech.UpdateCustomization(OnUpdateCustomization, OnFail, <customization-id>, _customVoiceUpdate))
    Log.Debug("ExampleTextToSpeech.UpdateCustomization()", "Failed to update customization!");
}

private void OnUpdateCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnUpdateCustomization()", "Text to Speech - Update customization response: {0}", success);
}
```






### List voice models
Lists metadata such as the name and description for all custom voice models that you own for all languages. Specify a language to list the voice models that you own for the specified language only. To see the words in addition to the metadata for a specific voice model, use the List a voice model method. Only the owner of a custom voice model can use this method to list information about the model.
```cs
private void GetCustomizations()
{
  if(!_textToSpeech.GetCustomizations(OnGetCustomizations, OnFail))
    Log.Debug("ExampleTextToSpeech.GetCustomizations()", "Failed to get customizations!");
}

private void OnGetCustomizations(Customizations customizations, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetCustomizations()", "Text to Speech - Get customizations response: {0}", customData["json"].ToString());
}
```






### List a voice model
Lists all information about the specified custom voice model. In addition to metadata such as the name and description of the voice model, the output includes the words in the model and their translations as defined in the model. To see just the metadata for a voice model, use the List voice models method. Only the owner of a custom voice model can use this method to query information about the model.
```cs
private void GetCustomization()
{
  if(!_textToSpeech.GetCustomization(OnGetCustomization, OnFail))
    Log.Debug("ExampleTextToSpeech.GetCustomization()", "Failed to get customization!");
}

private void OnGetCustomization(Customization customization, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetCustomization()", "Text to Speech - Get customization response: {0}", customData["json"].ToString());
}
```






### Delete a voice model
Deletes the custom voice model with the specified customization_id. Only the owner of a custom voice model can use this method to delete the model.
```cs
private void DeleteCustomization()
{
  if(!_textToSpeech.DeleteCustomization(OnDeleteCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleTextToSpeech.DeleteCustomization()", "Failed to delete customization!");
}

private void OnDeleteCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnDeleteCustomization()", "Text to Speech - Get customization response: {0}", success);
}
```






### Add words
Adds one or more words and their translations to the specified custom voice model. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add words to the model.
```cs
Word[] _wordArrayToAddToCustomization =
{
  new Word()
  {
    word = "bananna",
    translation = "arange"
  },
  new Word()
  {
    word = "orange",
    translation = "gbye"
  },
  new Word()
  {
    word = "tomato",
    translation = "tomahto"
  }
};

Words wordsToAddToCustomization = new Words()
{
    words = wordArrayToAddToCustomization
};

if (!_textToSpeech.AddCustomizationWords(OnAddCustomizationWords, OnFail, <customization-id>, _wordsToAddToCustomization))
    Log.Debug("ExampleTextToSpeech.AddCustomizationWords()", "Failed to add words customization!");
```

<!-- ### Add a word
Adds a single word and its translation to the specified custom voice model. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add a word to the model.
```cs
``` -->






### List words
Lists all of the words and their translations for the specified custom voice model. The output shows the translations as they are defined in the model. Only the owner of a custom voice model can use this method to query information about the model's words.
```cs
private void GetCustomizationWords()
{
  if(!_textToSpeech.GetCustomizationWords(OnGetCustomizationWords, OnFail, <customization-id>))
    Log.Debug("ExampleTextToSpeech.GetCustomizationWords()", "Failed to get customization words!");
}

private void OnGetCustomizationWords(Words words, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnGetCustomizationWords()", "Text to Speech - Get customization words response: {0}", customData["json"].ToString());
}
```

<!-- ### List a word
Returns the translation for a single word from the specified custom model. The output shows the translation as it is defined in the model. Only the owner of a custom voice model can use this method to query information about a word from the model.
```cs
``` -->






### Delete a word
Deletes a single word from the specified custom voice model. Only the owner of a custom voice model can use this method to delete a word from the model.
```cs
private void DeleteCustomizationWord()
{
  if(!_textToSpeech.DeleteCustomizationWords(OnDeleteCustomizationWords, OnFail, <customization-id>, <customization-word>))
    Log.Debug("ExampleTextToSpeech.DeleteCustomizationWord()", "Failed to get delete word!");
}

private void OnDeleteCustomizationWords(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleTextToSpeech.OnDeleteCustomizationWords()", "Text to Speech - Delete customization word response: {0}", success);
}
```

[text-to-speech]: https://console.bluemix.net/docs/services/text-to-speech/index.html
[using-ssml]: https://console.bluemix.net/docs/services/text-to-speech/SSML.html
[using-sprs]: https://console.bluemix.net/docs/services/text-to-speech/SPRs.html
[understanding-customization]: https://console.bluemix.net/docs/services/text-to-speech/custom-intro.html
[using-customization]: https://console.bluemix.net/docs/services/text-to-speech/custom-using.html
