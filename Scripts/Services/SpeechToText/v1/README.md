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

### Get models
Retrieves a list of all models available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
private void GetModels()
{
  if(!_speechToText.GetModels(HandleGetModels, OnFail))
    Log.Debug("ExampleSpeechToText.GetModels()", "Failed to get models");
}

private void HandleGetModels(ModelSet result, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetModels()", "Speech to Text - Get models response: {0}", customData["json"].ToString());
}
```




### Get a model
Retrieves information about a single specified model that is available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
private void GetModel()
{
  if(!_speechToText.GetModel(HandleGetModel, OnFail, <model-name>))
    Log.Debug("ExampleSpeechToText.GetModel()", "Failed to get model");
}

private void HandleGetModel(Model result, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetModel()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
}
```

### Recognize audio
#### Accessing the device microphone and sending data to the Speech to Text instance
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

  if(!_speechToText.Recognize(HandleRecognize, OnFail))
    Log.Debug("ExampleSpeechToText.Recognize()", "Failed to recognize!");
}

private void HandleRecognize(SpeechRecognitionEvent result, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleRecognize()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
}
```




### List custom models
Lists information about all custom language models that are owned by the calling user. Use the language query parameter to see all custom models for the specified language; omit the parameter to see all custom models for all languages.
```cs
private void GetCustomizations()
{
  if(!_speechToText.GetCustomizations(HandleGetCustomizations, OnFail))
    Log.Debug("ExampleSpeechToText.GetCustomizations()", "Failed to get customizations");
}

private void HandleGetCustomizations(Customizations customizations, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomizations()", "Speech to Text - Get customizations response: {0}", customData["json"].ToString());
}
```




### List a custom model
Lists information about a custom language model. Only the owner of a custom model can use this method to query information about the model.
```cs
private void GetCustomization()
{
  if(!_speechToText.GetCustomization(HandleGetCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.GetCustomization()", "Failed to get customization");
}

private void HandleGetCustomization(Customization customization, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomization()", "Speech to Text - Get customization response: {0}", customData["json"].ToString());
}
```




### Create a custom model
Creates a new custom language model for a specified base language model. The custom language model can be used only with the base language model for which it is created. The new model is owned by the individual whose service credentials are used to create it.
```cs
private void CreateModel()
{
  if(!_speechToText.CreateCustomization(HandleCreateCustomization, OnFail, <customization-name>, <base-model-name>, <customization-description>))
    Log.Debug("ExampleSpeechToText.CreateCustomization()", "Failed to create custom model");
}

private void HandleCreateCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleCreateCustomization()", "Speech to Text - Get model response: {0}", customData["json"].ToString());
}
```




### Train a custom model
Initiates the training of a custom language model with new corpora, words, or both. After adding training data to the custom model with the corpora or words methods, use this method to begin the actual training of the model on the new data. You can specify whether the custom model is to be trained with all words from its words resources or only with words that were added or modified by the user. Only the owner of a custom model can use this method to train the model.
```cs
private void TrainModel()
{
  if(!_speechToText.TrainCustomization(HandleTrainCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.TrainCustomization()", "Failed to train custom model");
}

private void HandleTrainCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleTrainCustomization()", "Speech to Text - Train model response: {0}", success);
}
```





### Reset a custom model
Resets a custom language model by removing all corpora and words from the model. Resetting a custom model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved. Only the owner of a custom model can use this method to reset the model.
```cs
private void ResetModel()
{
  if(!_speechToText.ResetCustomization(HandleResetCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.ResetCustomization()", "Failed to train custom model");
}

private void HandleResetCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleResetCustomization()", "Speech to Text - Reset model response: {0}", success);
}
```




<!-- ### Upgrade a custom model
Upgrades a custom language model to the latest release level of the Speech to Text service. The method bases the upgrade on the latest trained data stored for the custom model. If the corpora or words for the model have changed since the model was last trained, you must use the  train method to train the model on the new data. You must use credentials for the instance of the service that owns a model to upgrade it. **Note**: This method is not currently implemented. It will be added for a future release of the API.
```cs
private void UpgradeModel()
{
  if(!_speechToText.UpgradeCustomization(HandleUpgradeCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.UpgradeCustomization()", "Failed to train custom model");
}

private void HandleUpgradeCustomizationh(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleUpgradeCustomizationh()", "Speech to Text - Upgrade model response: {0}", success);
}
``` -->




### Delete a custom model
Deletes an existing custom language model. The custom model cannot be deleted if another request, such as adding a corpus to the model, is currently being processed. Only the owner of a custom model can use this method to delete the model.
```cs
private void DeleteModel()
{
  if(!_speechToText.DeleteCustomization(HandleDeleteCustomization, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.DeleteCustomization()", "Failed to delete custom model");
}

private void HandleDeleteCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleDeleteCustomization()", "Speech to Text - Delete model response: {0}", success);
}
```





### Add a corpus
Adds a single corpus text file of new training data to the custom language model. Use multiple requests to submit multiple corpus text files. Only the owner of a custom model can use this method to add a corpus to the model. Note that adding a corpus does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
private void AddCustomCorpus()
{
  if(!_speechToText.AddCustomCorpus(HandleAddCustomCorpus, OnFail, <customization-id>, <corpus-name>, <allow-overwrite>, <corpus-file-path>))
    Log.Debug("ExampleSpeechToText.AddCustomCorpus()", "Failed to delete custom model");
}

private void HandleAddCustomCorpus(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleAddCustomCorpus()", "Speech to Text - Add custom corpus response: {0}", success);
}
```





### List corpora
Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
```cs
private void GetCorpora()
{
  if(!_speechToText.GetCustomCorpora(HandleGetCustomCorpora, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.GetCustomCorpora()", "Failed to get custom corpora");
}

private void HandleGetCustomCorpora(Corpora corpora, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomCorpora()", "Speech to Text - Get custom corpora response: {0}", customdData);
}
```





### List a corpus
Lists information about a single specified corpus. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of the corpus. Only the owner of a custom model can use this method to list information about a corpus from the model.
```cs
private void GetCorpus()
{
  if(!_speechToText.GetCustomCorpus(HandleGetCustomCorpus, OnFail, <customization-id>, <corpus-name>))
    Log.Debug("ExampleSpeechToText.GetCustomCorpus()", "Failed to get custom corpus");
}

private void HandleGetCustomCorpus(Corpus corpus, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomCorpus()", "Speech to Text - Get custom corpus response: {0}", customdData);
}
```





### Delete a corpus
Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV) words associated with the corpus from the custom model's words resource unless they were also added by another corpus or they have been modified in some way with the Add custom words or Add a custom word method. Removing a corpus does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a corpus from the model.
```cs
private void DeleteCorpus()
{
  if(!_speechToText.DeleteCustomCorpus(HandleDeleteCustomCorpus, OnFail, <customization-id>, <corpus-name>))
    Log.Debug("ExampleSpeechToText.DeleteCustomCorpus()", "Failed to delete custom corpus");
}

private void HandleDeleteCustomCorpus(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleDeleteCustomCorpus()", "Speech to Text - Delete custom corpus response: {0}", success);
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
  if(!_speechToText.AddCustomWords(HandleAddCustomWords, OnFail, <customization-id>, words))
    Log.Debug("ExampleSpeechToText.AddCustomWords()", "Failed to add custom words");
}

//  Add custom words using words file
private void AddCustomWordsUsingFile()
{
  if(!_speechToText.AddCustomWords(HandleAddCustomWords, OnFail, <customization-id>, <words-filepath>))
    Log.Debug("ExampleSpeechToText.AddCustomWords()", "Failed to add custom words");
}

private void HandleAddCustomCorpus(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleAddCustomCorpus()", "Speech to Text - Add custom words response: {0}", success);
}
```




### List custom words
Lists information about all custom words from a custom language model. You can list all words from the custom model's words resource, only custom words that were added or modified by the user, or only OOV words that were extracted from corpora. Only the owner of a custom model can use this method to query the words from the model.
```cs
private void GetCustomWords()
{
  if(!_speechToText.GetCustomWords(HandleGetCustomWords, OnFail, <customization-id>))
    Log.Debug("ExampleSpeechToText.GetCustomWordsMethod()", "Failed to get custom words");
}

private void HandleGetCustomWords(WordsList wordList, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomWords()", "Speech to Text - Get custom words response: {0}", customData["json"].ToString());
}
```





### List a custom word
Lists information about a custom word from a custom language model. Only the owner of a custom model can use this method to query a word from the model.
```cs
private void GetCustomWord()
{
  if(!_speechToText.GetCustomWord(HandleGetCustomWord, OnFail, <customization-id>, <word>))
    Log.Debug("ExampleSpeechToText.GetCustomWord()", "Failed to get custom word");
}

private void HandleGetCustomWord(WordData word, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomWord()", "Speech to Text - Get custom word response: {0}", customData["json"].ToString());
}
```





### Delete a custom word
Deletes a custom word from a custom language model. You can remove any word that you added to the custom model's words resource via any means. However, if the word also exists in the service's base vocabulary, the service removes only the custom pronunciation for the word; the word remains in the base vocabulary.

Removing a custom word does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a word from the model.
```cs
private void DeleteCustomWord()
{
  if(!_speechToText.DeleteCustomWord(HandleDeleteCustomWord, OnFail, <customization-id>, <word>))
    Log.Debug("ExampleSpeechToText.DeleteCustomWord()", "Failed to delete custom word");
}

private void HandleDeleteCustomWord(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleDeleteCustomWord()", "Speech to Text - Delete custom word response: {0}", success);
}
```



### List custom acoustic models
Lists information about all custom acoustic models that are owned by an instance of the service. Use the `language` parameter to see all custom acoustic models for the specified language; omit the parameter to see all custom acoustic models for all languages. You must use credentials for the instance of the service that owns a model to list information about it.
```cs
private void ListCustomAcousticModels()
{
  if(!_speechToText.GetCustomAcousticModels(HandleGetCustomAcousticModels, OnFail))
    Log.Debug("ExampleSpeechToText.GetCustomAcousticModels()", "Failed to list custom acoustic models");
}

private void HandleListCustomAcousticModels(AcousticCustomizations acousticCustomizations, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleListCustomAcousticModels()", "acousticCustomizations: {0}", customData["json"].ToString());
}
```




### Create custom acoustic model
Creates a new custom acoustic model for a specified base model. The custom acoustic model can be used only with the base model for which it is created. The model is owned by the instance of the service whose credentials are used to create it.
```cs
private void CreateAcousticCustomization()
{
  if(!_speechToText.CreateAcousticCustomization(HandleCreateAcousticCustomization, OnFail, "<createdAcousticModelName>"))
    Log.Debug("ExampleSpeechToText.CreateAcousticCustomization()", "Failed to create acoustic customization");
}

private void HandleCreateAcousticCustomization(CustomizationID customizationID, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleCreateAcousticCustomization()", "customizationId: {0}", customData["json"].ToString());
}

```





### Delete custom acoustic model
Deletes an existing custom acoustic model. The custom model cannot be deleted if another request, such as adding an audio resource to the model, is currently being processed. You must use credentials for the instance of the service that owns a model to delete it.
```cs
private void DeleteAcousticCustomization()
{
  if(!_speechToText.DeleteAcousticCustomization(HandleDeleteAcousticCustomization, OnFail, "<createdAcousticModelId>"))
    Log.Debug("ExampleSpeechToText.DeleteAcousticCustomization()", "Failed to delete acoustic customization");
}

private void HandleDeleteAcousticCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleDeleteAcousticCustomization()", "deleted acoustic customization: {0}", success);
}
```





### Get details about a custom acoustic model
Lists information about a specified custom acoustic model. You must use credentials for the instance of the service that owns a model to list information about it.
```cs
private void GetCustomAcousticModel()
{
  if(!_speechToText.GetCustomAcousticModel(HandleGetCustomAcousticModel, OnFail, "<createdAcousticModelId>"))
    Log.Debug("ExampleSpeechToText.GetCustomAcousticModel()", "Failed to get custom acoustic model");
}

private void HandleGetCustomAcousticModel(AcousticCustomization acousticCustomization, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticModel()", "acousticCustomization: {0}", customData["json"].ToString());
}
```





### Train a custom acoustic model
Initiates the training of a custom acoustic model with new or changed audio resources. After adding or deleting audio resources for a custom acoustic model, use this method to begin the actual training of the model on the latest audio data. The custom acoustic model does not reflect its changed data until you train it. You must use credentials for the instance of the service that owns a model to train it.

The training method is asynchronous. It can take on the order of minutes or hours to complete depending on the total amount of audio data on which the model is being trained and the current load on the service. Typically, training takes approximately twice the length of the total audio contained in the custom model. The method returns an HTTP 200 response code to indicate that the training process has begun.

You can monitor the status of the training by using the `GET /v1/acoustic_customizations/{customization_id}` method to poll the model's status. Use a loop to check the status once a minute. The method returns an `AcousticCustomization` object that includes `status` and `progress` fields. A status of `available` indicates that the custom model is trained and ready to use. The service cannot accept subsequent training requests, or requests to add new audio resources, until the existing request completes.

You can use the optional `custom_language_model_id` query parameter to specify the GUID of a separately created custom language model that is to be used during training. Specify a custom language model if you have verbatim transcriptions of the audio files that you have added to the custom model or you have either corpora (text files) or a list of words that are relevant to the contents of the audio files. For information about creating a separate custom language model, see [Creating a custom language model][creating-a-custom-language-model].

Training can fail to start for the following reasons:
* The service is currently handling another request for the custom model, such as another training request or a request to add audio resources to the model.
* The custom model contains less than 10 minutes or more than 50 hours of audio data.
* One or more of the custom model's audio resources is invalid.
```cs
private void TrainAcousticCustomization()
{
  if(!_speechToText.TrainAcousticCustomization(HandleTrainAcousticCustomization, OnFail, "<createdAcousticModelId>", "<customLanguageModelId>", "<forceTrain>"))
    Log.Debug("ExampleSpeechToText.TrainAcousticCustomization()", "Failed to train acoustic customization");
}

private void HandleTrainAcousticCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleTrainAcousticCustomization()", "train customization success: {0}", success);
}
```





### Reset a custom acoustic model
Resets a custom acoustic model by removing all audio resources from the model. Resetting a custom acoustic model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved, but the model's audio resources are removed and must be re-created. You must use credentials for the instance of the service that owns a model to reset it.
```cs
private void ResetAcousticCustomization()
{
  if(!_speechToText.ResetAcousticCustomization(HandleResetAcousticCustomization, OnFail, "<createdAcousticModelId>"))
    Log.Debug("ExampleSpeechToText.ResetAcousticCustomization()", "Failed to reset acoustic customizations");
}

private void HandleResetAcousticCustomization(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleResetAcousticCustomization()", "reset customization success: {0}", success);
}
```





### List information about a custom acoustic model's audio resources
Lists information about all audio resources from a custom acoustic model. The information includes the name of the resource and information about its audio data, such as its duration. It also includes the status of the audio resource, which is important for checking the service's analysis of the resource in response to a request to add it to the custom acoustic model. You must use credentials for the instance of the service that owns a model to list its audio resources.
```cs
private void GetCustomAcousticResources()
{
  if(!_speechToText.GetCustomAcousticResources(HandleGetCustomAcousticResources, OnFail, "<createdAcousticModelId>"))
    Log.Debug("ExampleSpeechToText.GetCustomAcousticResources()", "Failed to get custom acoustic resources");
}

private void HandleGetCustomAcousticResources(AudioResources audioResources, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResources()", "audioResources: {0}", customData["json"].ToString());
}
```





### Delete an audio resource from a custom acoustic model
Deletes an existing audio resource from a custom acoustic model. Deleting an archive-type audio resource removes the entire archive of files; the current interface does not allow deletion of individual files from an archive resource. Removing an audio resource does not affect the custom model until you train the model on its updated data by using the `POST /v1/acoustic_customizations/{customization_id}/train` method. You must use credentials for the instance of the service that owns a model to delete its audio resources.
```cs
private void DeleteAcousticResource()
{
  if(!_speechToText.DeleteAcousticResource(HandleDeleteAcousticResource, OnFail, "<createdAcousticModelId>", "<acousticResourceName>"))
    Log.Debug("ExampleSpeechToText.DeleteAcousticResource()", "Failed to delete acoustic resource");
}

private void HandleDeleteAcousticResource(bool success, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleDeleteAcousticResource()", "deleted acoustic resource: {0}", success);
}
```





### Get information about an audio resource associated with a custom acoustic model
Lists information about an audio resource from a custom acoustic model. The method returns an `AudioListing` object whose fields depend on the type of audio resource you specify with the method's `audio_name` parameter:
For an audio-type resource, the object's fields match those of an `AudioResource` object: `duration`, `name`, `details`, and `status`.

For an archive-type resource, the object includes a `container` field whose fields match those of an `AudioResource` object. It also includes an `audio` field, which contains an array of `AudioResource` objects that provides information about the audio files that are contained in the archive.

The information includes the status of the specified audio resource, which is important for checking the service's analysis of the resource in response to a request to add it to the custom model. You must use credentials for the instance of the service that owns a model to list its audio resources.
```cs
private void GetCustomAcousticResource()
{
  if(!_speechToText.GetCustomAcousticResource(HandleGetCustomAcousticResource, OnFail, "<createdAcousticModelId>", "<acousticResourceName>"))
    Log.Debug("ExampleSpeechToText.GetCustomAcousticResource()", "Failed to get custom acoustic resource");
}

private void HandleGetCustomAcousticResource(AudioListing audioListing, Dictionary<string, object> customData)
{
  Log.Debug("ExampleSpeechToText.HandleGetCustomAcousticResource()", "audioListing: {0}", customData["json"].ToString());
}
```





### Add an audio resource to a custom acoustic model
Adds an audio resource to a custom acoustic model. Add audio content that reflects the acoustic characteristics of the audio that you plan to transcribe. You must use credentials for the instance of the service that owns a model to add an audio resource to it. Adding audio data does not affect the custom acoustic model until you train the model for the new data by using the `POST /v1/acoustic_customizations/{customization_id}/train` method.

You can add individual audio files or an archive file that contains multiple audio files. Adding multiple audio files via a single archive file is significantly more efficient than adding each file individually.
You can add an individual audio file in any format that the service supports for speech recognition. Use the `Content-Type` header to specify the format of the audio file.

You can add an archive file (**.zip** or **.tar.gz** file) that contains audio files in any format that the service supports for speech recognition. All audio files added with the same archive file must have the same audio format. Use the `Content-Type` header to specify the archive type, `application/zip` or `application/gzip`. Use the `Contained-Content-Type` header to specify the format of the contained audio files; the default format is `audio/wav`.

You can use this method to add any number of audio resources to a custom model by calling the method once for each audio or archive file. But the addition of one audio resource must be fully complete before you can add another. You must add a minimum of 10 minutes and a maximum of 50 hours of audio that includes speech, not just silence, to a custom acoustic model before you can train it. No audio resource, audio- or archive-type, can be larger than 100 MB.

The method is asynchronous. It can take several seconds to complete depending on the duration of the audio and, in the case of an archive file, the total number of audio files being processed. The service returns a 201 response code if the audio is valid. It then asynchronously analyzes the contents of the audio file or files and automatically extracts information about the audio such as its length, sampling rate, and encoding. You cannot submit requests to add additional audio resources to a custom acoustic model, or to train the model, until the service's analysis of all audio files for the current request completes.

To determine the status of the service's analysis of the audio, use the `GET /v1/acoustic_customizations/{customization_id}/audio/{audio_name}` method to poll the status of the audio. The method accepts the GUID of the custom model and the name of the audio resource, and it returns the status of the resource. Use a loop to check the status of the audio every few seconds until it becomes `ok`.

**Note:** The sampling rate of an audio file must match the sampling rate of the base model for the custom model: for broadband models, at least 16 kHz; for narrowband models, at least 8 kHz. If the sampling rate of the audio is higher than the minimum required rate, the service down-samples the audio to the appropriate rate. If the sampling rate of the audio is lower than the minimum required rate, the service labels the audio file as `invalid`.
```cs
private void AddAcousticResource()
{
  string mimeType = Utility.GetMimeType(Path.GetExtension("<acousticResourceUrl>"));
  if(!_speechToText.AddAcousticResource(HandleAddAcousticResource, OnFail, "<acousticModelId>", "<acousticResourceName>", mimeType, mimeType, true, "<acousticResourceData>")
    Log.Debug("ExampleSpeechToText.AddAcousticResourceMethod()", "Failed to add acoustic resource");
}

private void HandleAddAcousticResource(string data)
{
  Log.Debug("ExampleSpeechToText.HandleAddAcousticResource()", "added acoustic resource: {0}", customData["json"].ToString());
}
```





[speech-to-text]: https://console.bluemix.net/docs/services/speech-to-text/index.html
[creating-a-custom-language-model]: https://console.bluemix.net/docs/services/speech-to-text/language-create.html