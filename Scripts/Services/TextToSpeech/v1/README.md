[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.TextToSpeech.v1/)

### Text to Speech

The IBM® [Text to Speech][text-to-speech] service provides an API that uses IBM's speech-synthesis capabilities to synthesize text into natural-sounding speech in a variety of languages, accents, and voices. The service supports at least one male or female voice, sometimes both, for each language. The audio is streamed back to the client with minimal delay.

The Text to Speech API consists of the following groups of related calls:

* Voices includes methods that provide information about the voices available for synthesized speech.

* Synthesis includes methods that synthesize written text to spoken audio over the HTTP protocol. The calls support plain text and SSML input.

* WebSockets includes a method that synthesizes text to audio over the WebSocket protocol. The call supports plain text and SSML input, including the `<mark>` element as well as word timing information for all strings of the input text.

* Pronunciation includes a single method that returns the pronunciation for a specified word.

* Custom models provides methods for creating custom voice models. Custom models let users create a dictionary of words and their translations for use in speech synthesis.

* Custom words provides methods that let users manage the word/translation pairs in a custom voice model.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.TextToSpeech.v1

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.TextToSpeech.v1": "1.1.0"
}

```
### Usage
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

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
// create a Text to Speech Service instance
TextToSpeechService _textToSpeech = new TextToSpeechService();

// set the credentials
_languageTranslator.SetCredential("<username>", "<password>");
```

#### Get voices
Retrieves a list of all voices available for use with the service. The information includes the voice's name, language, and gender, among other things. To see information about a specific voice, use the Get a voice method.
```cs
//  returns a list of all available standard voice models
var results = _textToSpeech.GetVoices();
```

#### Get a voice
Lists information about the specified voice. Specify a customization_id to obtain information for that custom voice model of the specified voice. To see information about all available voices, use the Get voices method.
```cs
//  returns details of a specified voice
var results = _textToSpeech.GetVoice("<voice-name>");
```

#### Synthesize audio using file
Synthesizes text to spoken audio, returning the synthesized audio stream as an array of bytes. You can use two request methods to synthesize audio:

* The HTTP GET request method passes shorter text via a query parameter. The text size is limited by the maximum length of the HTTP request line and headers (about 6 KB) or by system limits, whichever is less.

* The HTTP POST request method passes longer text in the body of the request. Text size is limited to 5 KB.

With either request method, you can provide plain text or text that is annotated with SSML.
```cs
//  returns a stream of audio data from text in wave format using the Allison voice
var results = _textToSpeech.Synthesize(_text, Voice.EN_ALLISON, AudioType.WAV);
```

<!-- #### Synthesize audio using websockets
Synthesizes text to spoken audio over a WebSocket connection. The synthesize method establishes a connection with the service. You then send the text to be synthesized to the service as a JSON text message over the connection, and the service returns the audio as a binary stream of data.

You can provide a maximum of 5 KB of either plain text or text that is annotated with SSML. You can use the SSML <mark> element to request the location of the marker in the audio stream. You can also request word timing information in the form of start and end times for all strings of the input text. Mark and word timing results are sent as text messages over the connection.
```cs
``` -->

#### Get pronunciation
Returns the phonetic pronunciation for the specified word. You can request the pronunciation for a specific format. You can also request the pronunciation for a specific voice to see the default translation for the language of that voice or for a specific custom voice model to see the translation for that voice model.
```cs
//  returns pronunciation of the specified text
var results = _textToSpeech.GetPronunciation(_pronunciation);
```

#### Create a voice model
Creates a new empty custom voice model that is owned by the requesting user.
```cs
//  create a custom voice model object
CustomVoiceModel voiceModel = new CustomVoiceModel()
{
  Name = "<voice-model-name>",
  Description = "<voice-model-description>",
  Language = "<voice-model-language>"
};

//  create the custom voice model using the voice model data
var results = _textToSpeech.SaveCustomVoiceModel(voiceModel);
```

#### Update a voice model
Updates information for the specified custom voice model. You can update the metadata such as the name and description of the voice model. You can also update the words in the model and their translations. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to update the model. If no modelID is provided, a new custom model will be created.
```cs
// create a custom voice model object with updated data
CustomVoiceModel voiceModel = new CustomVoiceModel()
{
  Name = "<voice-model-name>",
  Description = "<voice-model-description>",
  Language = "<voice-model-language>",
  Id = "<voice-model-id>"
};

//  update the custom voice model using the voice model data
var results = _textToSpeech.SaveCustomVoiceModel(voiceModel);
```

#### List voice models
Lists metadata such as the name and description for all custom voice models that you own for all languages. Specify a language to list the voice models that you own for the specified language only. To see the words in addition to the metadata for a specific voice model, use the List a voice model method. Only the owner of a custom voice model can use this method to list information about the model.
```cs
//  lists all custom voice models
var results = _textToSpeech.GetCustomVoiceModels();
```

#### List a voice model
Lists all information about the specified custom voice model. In addition to metadata such as the name and description of the voice model, the output includes the words in the model and their translations as defined in the model. To see just the metadata for a voice model, use the List voice models method. Only the owner of a custom voice model can use this method to query information about the model.
```cs
//  list details of a specified custom voice model
var results = _textToSpeech.GetCustomVoiceModel("<voice-model-id>");
```

#### Delete a voice model
Deletes the custom voice model with the specified customization_id. Only the owner of a custom voice model can use this method to delete the model.
```cs
//  deletes the specified voice model
_textToSpeech.DeleteCustomVoiceModel("<voice-model-id>");
```

#### Add words
Adds one or more words and their translations to the specified custom voice model. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add words to the model.
```cs
//  create words to update custom voice model
CustomWordTranslation ibm = new CustomWordTranslation()
{
  Word = "IBM",
  Translation = "I B M"
};

CustomWordTranslation iPhone = new CustomWordTranslation()
{
  Word = "iPhone",
  Translation = "i Phone"
};

CustomWordTranslation jpl = new CustomWordTranslation()
{
  Word = "jpl",
  Translation = "J P L"
};

//  update custom voice model
_textToSpeech.SaveWords("<voice-model-id>", ibm, iPhone, jpl);
```

<!-- #### Add a word
Adds a single word and its translation to the specified custom voice model. Adding a new translation for a word that already exists in a custom model overwrites the word's existing translation. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add a word to the model.
```cs
``` -->

#### List words
Lists all of the words and their translations for the specified custom voice model. The output shows the translations as they are defined in the model. Only the owner of a custom voice model can use this method to query information about the model's words.
```cs
//  lists all words in a custom voice model
var results = _textToSpeech.GetWords("<voice-model-id>");
```

<!-- #### List a word
Returns the translation for a single word from the specified custom model. The output shows the translation as it is defined in the model. Only the owner of a custom voice model can use this method to query information about a word from the model.
```cs
``` -->

#### Delete a word
Deletes a single word from the specified custom voice model. Only the owner of a custom voice model can use this method to delete a word from the model.
```cs
//  deletes the word "jpl" from the custom voice model
_textToSpeech.DeleteWord(_customVoiceModelID, "jpl");
```

[text-to-speech]: https://www.ibm.com/watson/developercloud/doc/text-to-speech/index.html
[using-ssml]: https://www.ibm.com/watson/developercloud/doc/text-to-speech/SSML.html
[using-sprs]: https://www.ibm.com/watson/developercloud/doc/text-to-speech/SPRs.html
[understanding-customization]: https://www.ibm.com/watson/developercloud/doc/text-to-speech/custom-intro.html
[using-customization]: https://www.ibm.com/watson/developercloud/doc/text-to-speech/custom-using.html
