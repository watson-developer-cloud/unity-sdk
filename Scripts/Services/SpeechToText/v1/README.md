[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-green.svg?style=flat)](https://www.nuget.org/packages/IBM.WatsonDeveloperCloud.SpeechToText.v1/)

### Speech to Text

The IBM® [Speech to Text][speech-to-text] service provides an API that enables you to add IBM's speech recognition capabilities to your applications. The service transcribes speech from various languages and audio formats to text with low latency. For most languages, the service supports two sampling rates, broadband and narrowband.

### Installation
#### Nuget
```

PM > Install-Package IBM.WatsonDeveloperCloud.SpeechToText.v1

```
#### Project.json
```JSON

"dependencies": {
   "IBM.WatsonDeveloperCloud.SpeechToText.v1": "1.1.0"
}

```
### Usage
The Speech to Text API consists of the following groups of related calls:

* Models includes calls that return information about the models (languages and sampling rates) available for transcription.

* WebSockets includes a single call that establishes a persistent connection with the service over the WebSocket protocol.

* Sessionless includes HTTP calls that provide a simple means of transcribing audio without the overhead of establishing and maintaining a session.

* Sessions provides a collection of HTTP calls that provide a mechanism for a client to maintain a long, multi-turn exchange, or session, with the service or to establish multiple parallel conversations with a particular instance of the service.

* Asynchronous provides a non-blocking HTTP interface for transcribing audio. You can register a callback URL to be notified of job status and, optionally, results, or you can poll the service to learn job status and retrieve results manually.

* Custom models provides an HTTP interface for creating custom language models. The interface lets you expand the vocabulary of a base language model with domain-specific terminology.

* Custom corpora provides an HTTP interface for managing the corpora associated with a custom language model. You add a corpus to a custom model to extract words from the corpus into the model's vocabulary.

* Custom words provides an HTTP interface for managing individual words in a custom language model. You can add, list, and delete words from a custom model.

#### Instantiating and authenticating the service
Before you can send requests to the service it must be instantiated and credentials must be set.
```cs
//  create a Speech to Text Service instance
SpeechToTextService _speechToText = new SpeechToTextService();

//  set the credentials
_speechToText.SetCredential("<username>", "<password>");
```

#### Get models
Retrieves a list of all models available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
//  get a list of available speech models
var results = _speechToText.GetModels();
```

#### Get a model
Retrieves information about a single specified model that is available for use with the service. The information includes the name of the model and its minimum sampling rate in Hertz, among other things.
```cs
//  get details of a particular speech model
var results = _speechToText.GetModel("<model-name>");
```

#### Recognize audio
###### Streaming mode

For requests to transcribe live audio as it becomes available or to transcribe multiple audio files with multipart requests, you must set the Transfer-Encoding header to chunked to use streaming mode. In streaming mode, the server closes the connection (status code 408) if the service receives no data chunk for 30 seconds and the service has no audio to transcribe for 30 seconds. The server also closes the connection (status code 400) if no speech is detected for inactivity_timeout seconds of audio (not processing time); use the inactivity_timeout parameter to change the default of 30 seconds.

###### Non-multipart requests
For non-multipart requests, you specify all parameters of the request as a collection of request headers and query parameters, and you provide the audio as the body of the request. This is the recommended means of submitting a recognition request. Use the following parameters:

* Required: Content-Type and body
* Optional: Transfer-Encoding, model, customization_id, continuous, inactivity_timeout, keywords, keywords_threshold, max_alternatives, word_alternatives_threshold, word_confidence, timestamps, profanity_filter, smart_formatting, and speaker_labels

###### Multipart requests

For multipart requests, you specify a few parameters of the request as request headers and query parameters, but you specify most parameters as multipart form data in the form of JSON metadata, in which only part_content_type is required. You then specify the audio files for the request as subsequent parts of the form data. Use this approach with browsers that do not support JavaScript or when the parameters of the request are greater than the 8 KB limit imposed by most HTTP servers and proxies. Use the following parameters:

* Required: Content-Type, metadata, and upload
* Optional: Transfer-Encoding, model, and customization_id

An example of the multipart metadata for a pair of FLAC files follows. This first part of the request is sent as JSON; the remaining parts are the audio files for the request.

```
metadata="{\"part_content_type\":\"audio/flac\",\"data_parts_count\":2,\"continuous\":true,\"inactivity_timeout\"=-1}"
```

Note about the Try It Out feature: The Try it out! button is not supported for use with the the POST /v1/recognize method. For examples of calls to the method, see the [Speech to Text API reference][speech-to-text].

```cs
//  open and read an audio file
using (FileStream fs = File.OpenRead("<path-to-audio-file>"))
{
  //  get a transcript of the audio file.
  var results = _speechToText.Recognize(fs);
}


```

#### Create a session
Creates a session and locks recognition requests to that engine. You can use the session for multiple recognition requests so that each request is processed with the same engine. The session expires after 30 seconds of inactivity. Use the Get status method to prevent the session from expiring.
```cs
//  create a speech to text session
var results = _speechToText.CreateSession("<model-name>");
```

#### Get session status
Checks whether a specified session can accept another recognition request. Concurrent recognition tasks during the same session are not allowed. The returned state must be initialized to indicate that you can send another recognition request. You can also use this method to prevent the session from expiring after 30 seconds of inactivity. The request must pass the cookie that was returned by the Create a session method.
```cs
//  get a session's status
var recognizeStatus = _speechToText.GetSessionStatus("<session-id>");
```

#### Observe session result
Requests results for a recognition task within a specified session. You can submit this method multiple times for the same recognition task. To see interim results, set the interim_results parameter to true. The request must pass the cookie that was returned by the Create a session method.
```cs
//  set up observe
var taskObserveResult = Task.Factory.StartNew<List<SpeechRecognitionEvent>>(() =>
{
    return service.ObserveResult(<session-id>);
});

//  get results
taskObserveResult.ContinueWith((antecedent) =>
{
    var results = antecedent.Result;
});

//  recognize session audio
var taskRecognizeWithSession = Task.Factory.StartNew(() =>
{
    service.RecognizeWithSession(<session-id>, <audio-stream>.GetMediaTypeFromFile(), <metadata>, <audio-stream>, "chunked", <model-name>);
});
```

#### Recognize session audio
Sends audio and returns transcription results for a session-based recognition request. By default, returns only the final transcription results for the request. To see interim results, set the parameter interim_results to true in a call to the Observe result method.
```cs
//  recognize session audio
var taskRecognizeWithSession = Task.Factory.StartNew(() =>
{
    service.RecognizeWithSession(<session-id>, <audio-stream>.GetMediaTypeFromFile(), <metadata>, <audio-stream>, "chunked", <model-name>);
});
```

#### Recognize multipart session audio
Sends audio and returns transcription results for a session-based recognition request submitted as multipart form data. By default, returns only the final transcription results for the request.
```cs
using (FileStream fs = File.OpenRead("<path-to-audio-file>"))
{
    var speechEvent = _speechToText.RecognizeWithSession(<session-id>, fs.GetMediaTypeFromFile(), fs);
}
```

#### Delete a session
Deletes an existing session and its engine. The request must pass the cookie that was returned by the Create a session method. You cannot send requests to a session after it is deleted. By default, a session expires after 30 seconds of inactivity if you do not delete it first.
```cs
//  deletes a speech to text session
_speechToText.DeleteSession("<session-id>");
```

#### Create a custom model
Creates a new custom language model for a specified base language model. The custom language model can be used only with the base language model for which it is created. The new model is owned by the individual whose service credentials are used to create it.
```cs
var result = _speechToText.CreateCustomModel(<custom-model-name>, <base-model>, <custom-model-description>);
```

#### List custom models
Lists information about all custom language models that are owned by the calling user. Use the language query parameter to see all custom models for the specified language; omit the parameter to see all custom models for all languages.
```cs
var result = _speechToText.ListCustomModels();
```

#### List a custom model
Lists information about a custom language model. Only the owner of a custom model can use this method to query information about the model.
```cs
var result = _speechToText.ListCustomModel("<customization-id>");
```

#### Train a custom model
Initiates the training of a custom language model with new corpora, words, or both. After adding training data to the custom model with the corpora or words methods, use this method to begin the actual training of the model on the new data. You can specify whether the custom model is to be trained with all words from its words resources or only with words that were added or modified by the user. Only the owner of a custom model can use this method to train the model.
```cs
var result = _speechToText.TrainCustomModel(_createdCustomizationID);
```

#### Reset a custom model
Resets a custom language model by removing all corpora and words from the model. Resetting a custom model initializes the model to its state when it was first created. Metadata such as the name and language of the model are preserved. Only the owner of a custom model can use this method to reset the model.
```cs
var result = _speechToText.ResetCustomModel("<customization-id>");
```

#### Upgrade a custom model
Upgrades a custom language model to the latest release level of the Speech to Text service. The method bases the upgrade on the latest trained data stored for the custom model. If the corpora or words for the model have changed since the model was last trained, you must use the Train a custom model method to train the model on the new data. Only the owner of a custom model can use this method to upgrade the model.

Note: This method is not currently implemented. It will be added for a future release of the API.
```cs
var result = _speechToText.UpgradeCustomModel("<customization-id>");
```

#### Delete a custom model
Deletes an existing custom language model. The custom model cannot be deleted if another request, such as adding a corpus to the model, is currently being processed. Only the owner of a custom model can use this method to delete the model.
```cs
var result = _speechToText.DeleteCustomModel("<customization-id>");
```

#### Add a corpus
Adds a single corpus text file of new training data to the custom language model. Use multiple requests to submit multiple corpus text files. Only the owner of a custom model can use this method to add a corpus to the model. Note that adding a corpus does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
using (FileStream fs = File.OpenRead("<path-to-corpus-file>"))
{
    object result = _speechToText.AddCorpus("<customization_id>", "<corpus-name>", "<allow-overwrite>", fs);
}
```

#### List corpora
Lists information about all corpora that have been added to the specified custom language model. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of each corpus. Only the owner of a custom model can use this method to list the model's corpora.
```cs
var result = _speechToText.ListCorpora("<customization-id>");
```

#### List a corpus
Lists information about a single specified corpus. The information includes the total number of words and out-of-vocabulary (OOV) words, name, and status of the corpus. Only the owner of a custom model can use this method to list information about a corpus from the model.
```cs
var result = _speechToText.GetCorpus("<customization-id>", "<corpus-name>");
```

#### Delete a corpus
Deletes an existing corpus from a custom language model. The service removes any out-of-vocabulary (OOV) words associated with the corpus from the custom model's words resource unless they were also added by another corpus or they have been modified in some way with the Add custom words or Add a custom word method. Removing a corpus does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a corpus from the model.
```cs
var result = _speechToText.DeleteCorpus("<customization-id>", "<corpus-name>");
```

#### Add custom words
Adds one or more custom words to a custom language model. The service populates the words resource for a custom model with out-of-vocabulary (OOV) words found in each corpus added to the model. You can use this method to add additional words or to modify existing words in the words resource. Only the owner of a custom model can use this method to add or modify custom words associated with the model. Adding or modifying custom words does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
object result = _speechToText.AddCustomWords("<customization-id>",
                                new Words()
                                {
                                    WordsProperty = new List<Word>()
                                    {
                                        new Word()
                                        {
                                           DisplayAs = "Watson",
                                           SoundsLike = new List<string>()
                                           {
                                               "wat son"
                                           },
                                           WordProperty = "watson"
                                        },
                                        new Word()
                                        {
                                           DisplayAs = "C#",
                                           SoundsLike = new List<string>()
                                           {
                                               "si sharp"
                                           },
                                           WordProperty = "csharp"
                                        },
                                         new Word()
                                        {
                                           DisplayAs = "SDK",
                                           SoundsLike = new List<string>()
                                           {
                                               "S.D.K."
                                           },
                                           WordProperty = "sdk"
                                        }
                                    }
                                });
```

#### Add a custom word
Adds a custom word to a custom language model. The service populates the words resource for a custom model with out-of-vocabulary (OOV) words found in each corpus added to the model. You can use this method to add additional words or to modify existing words in the words resource. Only the owner of a custom model can use this method to add or modify a custom word for the model. Adding or modifying a custom word does not affect the custom model until you train the model for the new data by using the Train a custom model method.
```cs
object result = _speechToText.AddCustomWord("<customization-id>",
                                  "<word-name>",
                                  new WordDefinition()
                                  {
                                      DisplayAs = "Social",
                                      SoundsLike = new List<string>()
                                             {
                                                 "so cial"
                                             }
                                  });
```

#### List custom words
Lists information about all custom words from a custom language model. You can list all words from the custom model's words resource, only custom words that were added or modified by the user, or only OOV words that were extracted from corpora. Only the owner of a custom model can use this method to query the words from the model.
```cs
var result = _speechToText.ListCustomWords("<customization-id>");
```

#### List a custom word
Lists information about a custom word from a custom language model. Only the owner of a custom model can use this method to query a word from the model.
```cs
var result = _speechToText.ListCustomWord("<customization-id>", "<word-name>");
```

#### Delete a custom word
Deletes a custom word from a custom language model. You can remove any word that you added to the custom model's words resource via any means. However, if the word also exists in the service's base vocabulary, the service removes only the custom pronunciation for the word; the word remains in the base vocabulary.

Removing a custom word does not affect the custom model until you train the model with the Train a custom model method. Only the owner of a custom model can use this method to delete a word from the model.
```cs
object result = _speechToText.DeleteCustomWord("<customization-id>", "<word-name>");
```

[speech-to-text]: https://www.ibm.com/watson/developercloud/doc/speech-to-text/index.html
