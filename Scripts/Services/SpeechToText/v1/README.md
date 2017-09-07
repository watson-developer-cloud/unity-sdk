# Speech to Text

The IBM® [Speech to Text][speech-to-text] service provides an API that enables you to add IBM's speech recognition capabilities to your applications. The service transcribes speech from various languages and audio formats to text with low latency. For most languages, the service supports two sampling rates, broadband and narrowband.

## Usage
The Speech to Text API consists of the following groups of related calls:

* Models includes calls that return information about the models (languages and sampling rates) available for transcription.

* WebSockets includes a single call that establishes a persistent connection with the service over the WebSocket protocol.

* Sessionless includes HTTP calls that provide a simple means of transcribing audio without the overhead of establishing and maintaining a session.

* Sessions provides a collection of HTTP calls that provide a mechanism for a client to maintain a long, multi-turn exchange, or session, with the service or to establish multiple parallel conversations with a particular instance of the service.

* Asynchronous provides a non-blocking HTTP interface for transcribing audio. You can register a callback URL to be notified of job status and, optionally, results, or you can poll the service to learn job status and retrieve results manually.

* Custom models provides an HTTP interface for creating custom language models. The interface lets you expand the vocabulary of a base language model with domain-specific terminology.

* Custom corpora provides an HTTP interface for managing the corpora associated with a custom language model. You add a corpus to a custom model to extract words from the corpus into the model's vocabulary.

* Custom words provides an HTTP interface for managing individual words in a custom language model. You can add, list, and delete words from a custom model.

### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Utilities;

void Start()
{
    Credentials credentials = new Credentials(<username>, <password>, <url>);
    SpeechToText _speechToText = new SpeechToText(credentials);
}
```

### Get models
Retrieves a list of all models available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
private void GetModels()
{
  if(!_speechToText.GetModels(HandleGetModels))
    Log.Debug("ExampleSpeechToText", "Failed to get models");
}

private void HandleGetModels(ModelSet result, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get models response: {0}", customData);
}
```




### Get a model
Retrieves information about a single specified model that is available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
private void GetModel()
{
  if(!_speechToText.GetModel(HandleGetModel, <model-name>))
    Log.Debug("ExampleSpeechToText", "Failed to get model");
}

private void HandleGetModel(Model result, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get model response: {0}", customData);
}
```

### Recognize audio
#### Accessing the device microphone and sending data to the Speech to Text instance
You can access the microphone of a device using Unity's Microphone class.

```cs
_speechToText.StartListening(OnRecognize);
_recording = Microphone.Start(<device-name>, <loop>, <length-seconds>, <frequency>);
```

AudioData can be created using the resulting AudioClip
```cs
int midPoint = _recording.samples / 2;
samples = new float[midPoint];
_recording.GetData(samples, 0);

AudioData record = new AudioData();
record.MaxLevel = Mathf.Max(samples);
record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
record.Clip.SetData(samples, 0);
```

The AudioData can be sent to the Speech to Text service and handled by the OnRecognize callback
```cs
 _speechToText.OnListen(record);
```

```cs
private void OnRecognize(SpeechRecognitionEvent result)
{
    //  do something
}
```

Please see `ExampleStreaming` scene for an example.

#### Streaming mode

For requests to transcribe live audio as it becomes available or to transcribe multiple audio files with multipart requests, you must set the Transfer-Encoding header to chunked to use streaming mode. In streaming mode, the server closes the connection (status code 408) if the service receives no data chunk for 30 seconds and the service has no audio to transcribe for 30 seconds. The server also closes the connection (status code 400) if no speech is detected for inactivity_timeout seconds of audio (not processing time); use the inactivity_timeout parameter to change the default of 30 seconds. An example of streaming from the Unity microphone is provided in the Examples directory.

#### Non-multipart requests
For non-multipart requests, you specify all parameters of the request as a collection of request headers and query parameters, and you provide the audio as the body of the request. This is the recommended means of submitting a recognition request. Use the following parameters:

* Required: Content-Type and body
* Optional: Transfer-Encoding, model, customization_id, continuous, inactivity_timeout, keywords, keywords_threshold, max_alternatives, word_alternatives_threshold, word_confidence, timestamps, profanity_filter, smart_formatting, and speaker_labels

#### Multipart requests

For multipart requests, you specify a few parameters of the request as request headers and query parameters, but you specify most parameters as multipart form data in the form of JSON metadata, in which only part_content_type is required. You then specify the audio files for the request as subsequent parts of the form data. Use this approach with browsers that do not support JavaScript or when the parameters of the request are greater than the 8 KB limit imposed by most HTTP servers and proxies. Use the following parameters:

* Required: Content-Type, metadata, and upload
* Optional: Transfer-Encoding, model, and customization_id

An example of the multipart metadata for a pair of FLAC files follows. This first part of the request is sent as JSON; the remaining parts are the audio files for the request.

```
metadata="{\"part_content_type\":\"audio/flac\",\"data_parts_count\":2,\"continuous\":true,\"inactivity_timeout\"=-1}"
```

```cs
private void Recognize()
{
  //  create AudioClip with clip bytearray data
  _audioClip = WaveFile.ParseWAV(<clip-name>, <clip-data>);

  if(!_speechToText.Recognize(HandleRecognize))
    Log.Debug("ExampleSpeechToText", "Failed to recognize!");
}

private void HandleRecognize(SpeechRecognitionEvent result)
{
  if (result != null && result.results.Length > 0)
  {
    foreach (var res in result.results)
    {
      foreach (var alt in res.alternatives)
      {
        string text = alt.transcript;
        Log.Debug("ExampleSpeechToText", string.Format("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));

        if (res.final)
          _recognizeTested = true;
      }
    }
  }
}
```




### List custom models
Lists information about all custom language models that are owned by the calling user. Use the language query parameter to see all custom models for the specified language; omit the parameter to see all custom models for all languages.
```cs
private void GetCustomizations()
{
  if(!_speechToText.GetCustomizations(HandleGetCustomizations))
    Log.Debug("ExampleSpeechToText", "Failed to get customizations");
}

private void HandleGetCustomizations(Customizations customizations, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get customizations response: {0}", customData);
}
```




### List a custom model
Lists information about a custom language model. Only the owner of a custom model can use this method to query information about the model.
```cs
private void GetCustomization()
{
  if(!_speechToText.GetCustomization(HandleGetCustomization, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to get customization");
}

private void HandleGetCustomization(Customization customization, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get customization response: {0}", customData);
}
```




### Create a custom model
Creates a new custom language model for a specified base language model. The custom language model can be used only with the base language model for which it is created. The new model is owned by the individual whose service credentials are used to create it.
```cs
private void CreateModel()
{
  if(!_speechToText.CreateCustomization(HandleCreateCustomization, <customization-name>, <base-model-name>, <customization-description>))
    Log.Debug("ExampleSpeechToText", "Failed to create custom model");
}

private void HandleCreateCustomization(CustomizationID customizationID, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get model response: {0}", customData);
}
```




### Train a custom model
Initiates the training of a custom language model with new corpora, words, or both. After adding training data to the custom model with the corpora or words methods, use this method to begin the actual training of the model on the new data. You can specify whether the custom model is to be trained with all words from its words resources or only with words that were added or modified by the user. Only the owner of a custom model can use this method to train the model.
```cs
private void TrainModel()
{
  if(!_speechToText.TrainCustomization(HandleTrainCustomization, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to train custom model");
}

private void HandleTrainCustomization(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Train model response: {0}", success);
}
```





### Reset a custom model
Resets a custom language model by removing all corpora and words from the model. Resetting a custom model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved. Only the owner of a custom model can use this method to reset the model.
```cs
private void ResetModel()
{
  if(!_speechToText.ResetCustomization(HandleResetCustomization, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to train custom model");
}

private void HandleResetCustomization(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Reset model response: {0}", success);
}
```




<!-- ### Upgrade a custom model
Upgrades a custom language model to the latest release level of the Speech to Text service. The method bases the upgrade on the latest trained data stored for the custom model. If the corpora or words for the model have changed since the model was last trained, you must use the  train method to train the model on the new data. You must use credentials for the instance of the service that owns a model to upgrade it. **Note**: This method is not currently implemented. It will be added for a future release of the API.
```cs
private void UpgradeModel()
{
  if(!_speechToText.UpgradeCustomization(HandleUpgradeCustomizationh, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to train custom model");
}

private void HandleUpgradeCustomizationh(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Upgrade model response: {0}", success);
}
``` -->




### Delete a custom model
Deletes an existing custom language model. The custom model cannot be deleted if another request, such as adding a corpus to the model, is currently being processed. Only the owner of a custom model can use this method to delete the model.
```cs
private void DeleteModel()
{
  if(!_speechToText.DeleteCustomization(HandleDeleteCustomization, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to delete custom model");
}

private void HandleDeleteCustomization(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Delete model response: {0}", success);
}
```





### Add a corpus
Adds a single corpus text file of new training data to the custom language model. Use multiple requests to submit multiple corpus text files. Only the owner of a custom model can use this method to add a corpus to the model. Note that adding a corpus does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
private void AddCustomCorpus()
{
  if(!_speechToText.AddCustomCorpus(HandleAddCustomCorpus, <customization-id>, <corpus-name>, <allow-overwrite>, <corpus-file-path>))
    Log.Debug("ExampleSpeechToText", "Failed to delete custom model");
}

private void HandleAddCustomCorpus(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom corpus response: {0}", success);
}
```





### List corpora
Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
```cs
private void GetCorpora()
{
  if(!_speechToText.GetCustomCorpora(HandleGetCustomCorpora, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to get custom corpora");
}

private void HandleGetCustomCorpora(Corpora corpora, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom corpora response: {0}", customdData);
}
```





### List a corpus
Lists information about a single specified corpus. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of the corpus. Only the owner of a custom model can use this method to list information about a corpus from the model.
```cs
private void GetCorpus()
{
  if(!_speechToText.GetCustomCorpus(HandleGetCustomCorpus, <customization-id>, <corpus-name>))
    Log.Debug("ExampleSpeechToText", "Failed to get custom corpus");
}

private void HandleGetCustomCorpus(Corpus corpus, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom corpus response: {0}", customdData);
}
```





### Delete a corpus
Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV) words associated with the corpus from the custom model's words resource unless they were also added by another corpus or they have been modified in some way with the Add custom words or Add a custom word method. Removing a corpus does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a corpus from the model.
```cs
private void DeleteCorpus()
{
  if(!_speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, <customization-id>, <corpus-name>))
    Log.Debug("ExampleSpeechToText", "Failed to delete custom corpus");
}

private void HandleDeleteCustomCorpus(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Delete custom corpus response: {0}", success);
}
```





### Add custom words
Adds one or more custom words to a custom language model. The service populates the words resource for a custom model with out-of-vocabulary (OOV) words found in each corpus added to the model. You can use this method to add additional words or to modify existing words in the words resource. Only the owner of a custom model can use this method to add or modify custom words associated with the model. Adding or modifying custom words does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
//  Add custom words using words object
var words = new Words()
{
    WordsProperty = new List<Word>()
    {
        new Word()
        {
           DisplayAs = <word-string-0>,
           SoundsLike = new List<string>()
           {
               <soundslike-string-0>
           },
           WordProperty = <wordproperty-string-0>
        },
        new Word()
        {
          DisplayAs = <word-string-1>,
          SoundsLike = new List<string>()
          {
              <soundslike-string-1>
          },
          WordProperty = <wordproperty-string-1>
        },
         new Word()
        {
          DisplayAs = <word-string-2>,
          SoundsLike = new List<string>()
          {
              <soundslike-string-2>
          },
          WordProperty = <wordproperty-string-2>
        }
    }
});

private void AddCustomWordsUsingObject()
{
  if(!_speechToText.AddCustomWords(HandleAddCustomWords, <customization-id>, words))
    Log.Debug("ExampleSpeechToText", "Failed to add custom words");
}

//  Add custom words using words file
private void AddCustomWordsUsingFile()
{
  if(!_speechToText.AddCustomWords(HandleAddCustomWords, <customization-id>, <words-filepath>))
    Log.Debug("ExampleSpeechToText", "Failed to add custom words");
}

private void HandleAddCustomCorpus(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Add custom words response: {0}", success);
}
```




### List custom words
Lists information about all custom words from a custom language model. You can list all words from the custom model's words resource, only custom words that were added or modified by the user, or only OOV words that were extracted from corpora. Only the owner of a custom model can use this method to query the words from the model.
```cs
private void GetCustomWords()
{
  if(!_speechToText.GetCustomWords(HandleGetCustomWords, <customization-id>))
    Log.Debug("ExampleSpeechToText", "Failed to get custom words");
}

private void HandleGetCustomWords(WordsList wordList, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom words response: {0}", customData);
}
```





### List a custom word
Lists information about a custom word from a custom language model. Only the owner of a custom model can use this method to query a word from the model.
```cs
private void GetCustomWord()
{
  if(!_speechToText.GetCustomWord(HandleGetCustomWord, <customization-id>, <word>))
    Log.Debug("ExampleSpeechToText", "Failed to get custom word");
}

private void HandleGetCustomWord(WordData word, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Get custom word response: {0}", customData);
}
```





### Delete a custom word
Deletes a custom word from a custom language model. You can remove any word that you added to the custom model's words resource via any means. However, if the word also exists in the service's base vocabulary, the service removes only the custom pronunciation for the word; the word remains in the base vocabulary.

Removing a custom word does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a word from the model.
```cs
private void DeleteCustomWord()
{
  if(!_speechToText.DeleteCustomWord(HandleDeleteCustomWord, <customization-id>, <word>))
    Log.Debug("ExampleSpeechToText", "Failed to delete custom word");
}

private void HandleDeleteCustomWord(bool success, string customData)
{
  Log.Debug("ExampleSpeechToText", "Speech to Text - Delete custom word response: {0}", success);
}
```

[speech-to-text]: https://console.bluemix.net/docs/services/speech-to-text/index.html
