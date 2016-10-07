/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/


using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.SpeechToText.v1
{
	#region Models
	/// <summary>
	/// This data class holds multiple speech models.
	/// </summary>
	[fsObject]
	public class ModelSet
	{
		/// <summary>
		/// Information about each available model.
		/// </summary>
		public Model[] models { get; set; }
	}
	/// <summary>
	/// This data class holds the data for a given speech model.
	/// </summary>
	[fsObject]
	public class Model
	{
		/// <summary>
		/// The name of the speech model.
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// The language ID for this model. (e.g. en)
		/// </summary>
		public string language { get; set; }
		/// <summary>
		/// The optimal sample rate for this model.
		/// </summary>
		public long rate { get; set; }
		/// <summary>
		/// The URL for this model.
		/// </summary>
		public string url { get; set; }
		/// <summary>
		/// Information about each available model.
		/// </summary>
		public string sessions { get; set; }
		/// <summary>
		/// Describes the additional service features supported with the model. 
		/// </summary>
		public SupportedFeatures supported_features { get; set; }
		/// <summary>
		/// A description for this model.
		/// </summary>
		public string description { get; set; }
	};

	/// <summary>
	/// This data class holds all supported features.
	/// </summary>
	[fsObject]
	public class SupportedFeatures
	{
		/// <summary>
		/// Describes the additional service features supported with the model.
		/// </summary>
		public bool custom_language_model { get; set; }
	}
	#endregion

	#region Sessions and Sessionless
	/// <summary>
	/// This data class holds the Session data.
	/// </summary>
	[fsObject]
	public class Session
	{
		/// <summary>
		/// Describes the additional service features supported with the model. 
		/// </summary>
		public string session_id { get; set; }
		/// <summary>
		/// Describes the additional service features supported with the model. 
		/// </summary>
		public string new_session_uri { get; set; }
		/// <summary>
		/// URI for REST recognition requests. 
		/// </summary>
		public string recognize { get; set; }
		/// <summary>
		/// URI for REST results observers.
		/// </summary>
		public string observe_result { get; set; }
		/// <summary>
		/// URI for WebSocket recognition requests. Needed only for working with the WebSocket interface.
		/// </summary>
		public string recognizeWS { get; set; }
	}

	/// <summary>
	/// This data object contains data for the speechRecognitionEvent.
	/// </summary>
	[fsObject]
	public class SpeechRecognitionEvent
	{
		/// <summary>
		/// The results array consists of zero or more final results followed by zero or one interim result. The final results are guaranteed not to change; the interim result may be replaced by zero or more final results (followed by zero or one interim result). The service periodically sends updates to the result list, with the result_index set to the lowest index in the array that has changed.
		/// </summary>
		public SpeechRecognitionResult[] results { get; set; }
		/// <summary>
		/// An index that indicates the change point in the results array. 
		/// </summary>
		public int result_index { get; set; }
		/// <summary>
		/// An array of warning messages about invalid query parameters or JSON fields included with the request. Each warning includes a descriptive message and a list of invalid argument strings. For example, a message such as "Unknown arguments:" or "Unknown url query arguments:" followed by a list of the form "invalid_arg_1, invalid_arg_2." The request succeeds despite the warnings.
		/// </summary>
		public string[] warnings { get; set; }

		/// <exclude />
		public SpeechRecognitionEvent(SpeechRecognitionResult[] _results)
		{
			results = _results;
		}

		/// <summary>
		/// Check if our result list has atleast one valid result.
		/// </summary>
		/// <returns>Returns true if a result is found.</returns>
		public bool HasResult()
		{
			return results != null && results.Length > 0
				&& results[0].alternatives != null && results[0].alternatives.Length > 0;
		}

		/// <summary>
		/// Returns true if we have a final result.
		/// </summary>
		/// <returns></returns>
		public bool HasFinalResult()
		{
			return HasResult() && results[0].final;
		}
	}

	/// <summary>
	/// The speech recognition result.
	/// </summary>
	[fsObject]
	public class SpeechRecognitionResult
	{
		/// <summary>
		/// If true, the result for this utterance is not updated further. 
		/// </summary>
		public bool final { get; set; }
		/// <summary>
		/// Array of alternative transcripts. 
		/// </summary>
		public SpeechRecognitionAlternative[] alternatives { get; set; }
		/// <summary>
		/// Dictionary (or associative array) whose keys are the strings specified for keywords if both that parameter and keywords_threshold are specified. A keyword for which no matches are found is omitted from the array.
		/// </summary>
		public KeywordResults keywords_result { get; set; }
		/// <summary>
		/// Array of word alternative hypotheses found for words of the input audio if word_alternatives_threshold is not null.
		/// </summary>
		public WordAlternativeResults word_alternatives { get; set; }
	}

	/// <summary>
	/// The alternatives.
	/// </summary>
	[fsObject]
	public class SpeechRecognitionAlternative
	{
		/// <summary>
		/// Transcription of the audio. 
		/// </summary>
		public string transcript { get; set; }

		/// <summary>
		/// Confidence score of the transcript in the range of 0 to 1. Available only for the best alternative and only in results marked as final.
		/// </summary>
		public double confidence { get; set; }
		/// <summary>
		/// Time alignments for each word from transcript as a list of lists. Each inner list consists of three elements: the word followed by its start and end time in seconds. Example: [["hello",0.0,1.2],["world",1.2,2.5]]. Available only for the best alternative. 
		/// </summary>
		public string[] timestamps { get; set; }
		/// <summary>
		/// Confidence score for each word of the transcript as a list of lists. Each inner list consists of two elements: the word and its confidence score in the range of 0 to 1. Example: [["hello",0.95],["world",0.866]]. Available only for the best alternative and only in results marked as final.
		/// </summary>
		public string[] word_confidence { get; set; }
		/// <summary>
		/// A optional array of timestamps objects.
		/// </summary>
		public TimeStamp[] Timestamps { get; set; }
		/// <summary>
		/// A option array of word confidence values.
		/// </summary>
		public WordConfidence[] WordConfidence { get; set; }
	}

	/// <summary>
	/// The Keword Result
	/// </summary>
	[fsObject]
	public class KeywordResults
	{
		/// <summary>
		/// List of each keyword entered via the keywords parameter and, for each keyword, an array of KeywordResult objects that provides information about its occurrences in the input audio. The keys of the list are the actual keyword strings. A keyword for which no matches are spotted in the input is omitted from the array.
		/// </summary>
		public KeywordResult[] keyword { get; set; }
	}

	/// <summary>
	/// This class holds a Word Alternative Result.
	/// </summary>
	[fsObject]
	public class WordAlternativeResults
	{
		/// <summary>
		/// Specified keyword normalized to the spoken phrase that matched in the audio input. 
		/// </summary>
		public double start_time { get; set; }
		/// <summary>
		/// Specified keyword normalized to the spoken phrase that matched in the audio input. 
		/// </summary>
		public double end_time { get; set; }
		/// <summary>
		/// Specified keyword normalized to the spoken phrase that matched in the audio input. 
		/// </summary>
		public WordAlternativeResult[] alternatives { get; set; }
	}

	/// <summary>
	/// This class holds a Keyword Result.
	/// </summary>
	[fsObject]
	public class KeywordResult
	{
		/// <summary>
		/// Specified keyword normalized to the spoken phrase that matched in the audio input. 
		/// </summary>
		public string normalized_text { get; set; }
		/// <summary>
		/// Start time in seconds of the keyword match.
		/// </summary>
		public double start_time { get; set; }
		/// <summary>
		/// End time in seconds of the keyword match. 
		/// </summary>
		public double end_time { get; set; }
		/// <summary>
		/// Confidence score of the keyword match in the range of 0 to 1.
		/// </summary>
		public double confidence { get; set; }
	}

	/// <summary>
	/// This data class holds a Word Alternative Result.
	/// </summary>
	[fsObject]
	public class WordAlternativeResult
	{
		/// <summary>
		/// Confidence score of the word alternative hypothesis. 
		/// </summary>
		public double confidence { get; set; }
		/// <summary>
		/// Word alternative hypothesis for a word from the input audio.
		/// </summary>
		public string word { get; set; }
	}

	/// <summary>
	/// This data class holds the sesion data.
	/// </summary>
	[fsObject]
	public class RecognizeStatus
	{
		/// <summary>
		/// Description of the state and possible actions for the current session
		/// </summary>
		public SessionStatus session { get; set; }
	}

	/// <summary>
	/// This data class holds the description of teh state and possbile actions for the current session.
	/// </summary>
	[fsObject]
	public class SessionStatus
	{
		/// <summary>
		/// State of the session. The state must be initialized to perform a new recognition request on the session. 
		/// </summary>
		public string state { get; set; }
		/// <summary>
		/// URI for information about the model that is used with the session. 
		/// </summary>
		public string model { get; set; }
		/// <summary>
		/// URI for REST recognition requests. 
		/// </summary>
		public string recognize { get; set; }
		/// <summary>
		/// URI for REST results observers. 
		/// </summary>
		public string observe_result { get; set; }
		/// <summary>
		/// URI for WebSocket recognition requests. Needed only for working with the WebSocket interface.
		/// </summary>
		public string recognizeWS { get; set; }
	}

	/// <summary>
	/// This data class holds the confidence value for a given recognized word.
	/// </summary>
	[fsObject]
	public class WordConfidence
	{
		/// <summary>
		/// The word as a string.
		/// </summary>
		public string Word { get; set; }
		/// <summary>
		/// The confidence value for this word.
		/// </summary>
		public double Confidence { get; set; }
	};
	/// <summary>
	/// This data class holds the start and stop times for a word.
	/// </summary>
	[fsObject]
	public class TimeStamp
	{
		/// <summary>
		/// The word.
		/// </summary>
		public string Word { get; set; }
		/// <summary>
		/// The start time.
		/// </summary>
		public double Start { get; set; }
		/// <summary>
		/// The stop time.
		/// </summary>
		public double End { get; set; }
	};
	#endregion

	#region Asynchronous
	/// <summary>
	/// This data class contains information about the register callback status.
	/// </summary>
	public class RegisterStatus
	{
		/// <summary>
		/// The current status of the job: created if the callback URL was successfully white-listed as a result of the call or already created if the URL was already white-listed. 
		/// </summary>
		public string status { get; set; }
		/// <summary>
		/// The callback URL that is successfully registered
		/// </summary>
		public string url { get; set; }
	}

	/// <summary>
	/// This data class contains information about the Jobs
	/// </summary>
	public class JobsStatusList
	{
		/// <summary>
		/// The current status of the job: created if the callback URL was successfully white-listed as a result of the call or already created if the URL was already white-listed. 
		/// </summary>
		public JobStatus[] recognitions { get; set; }
	}

	/// <summary>
	/// This data class contains information about the job status.
	/// </summary>
	public class JobStatus
	{
		/// <summary>
		/// The ID of the job. 
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// The date and time in Coordinated Universal Time (UTC) at which the job was created. The value is provided in full ISO 8601 format (YYYY-MM-DDThh:mm:ss.sTZD).  
		/// </summary>
		public string created { get; set; }
		/// <summary>
		/// The date and time in Coordinated Universal Time (UTC) at which the job was last updated by the service. The value is provided in full ISO 8601 format (YYYY-MM-DDThh:mm:ss.sTZD). 
		/// </summary>
		public string updated { get; set; }
		/// <summary>
		/// The current status of the job. waiting: The service is preparing the job for processing; the service always returns this status when the job is initially created or when it is waiting for capacity to process the job. processing: The service is actively processing the job. completed: The service has finished processing the job; if the job specified a callback URL and the event recognitions.completed_with_results, the service sent the results with the callback notification; otherwise, use the GET /v1/recognitions/{id} method to retrieve the results. failed: The job failed. 
		/// </summary>
		public string status { get; set; }
		/// <summary>
		/// The user token associated with a job that was created with a callback URL and a user token.
		/// </summary>
		public string user_token { get; set; }
	}

	/// <summary>
	/// This data class contains information about a newly created recognition job.
	/// </summary>
	public class JobStatusNew
	{
		/// <summary>
		/// The date and time in Coordinated Universal Time (UTC) at which the job was created. The value is provided in full ISO 8601 format (YYYY-MM-DDThh:mm:ss.sTZD). 
		/// </summary>
		public string created { get; set; }
		/// <summary>
		/// The ID of the job.
		/// </summary>
		public string id { get; set; }
		/// <summary>
		/// The URL to use to request information about the job with the GET /v1/recognitions/{id} method.
		/// </summary>
		public string url { get; set; }
		/// <summary>
		/// The current status of the job. waiting: The service is preparing the job for processing; the service always returns this status when the job is initially created or when it is waiting for capacity to process the job. processing: The service is actively processing the job. completed: The service has finished processing the job; if the job specified a callback URL and the event recognitions.completed_with_results, the service sent the results with the callback notification; otherwise, use the GET /v1/recognitions/{id} method to retrieve the results. failed: The job failed.
		/// </summary>
		public string status { get; set; }
	}
	#endregion

	#region Custom Models
	/// <summary>
	/// This data class contains information about the speech to text model customizations.
	/// </summary>
	public class Customizations
	{
		/// <summary>
		/// Information about each available custom model. The array is empty if the user owns no custom models (if no language is specified) or owns no custom models for the specified language.
		/// </summary>
		public Customization[] customizations { get; set; }
	}

	/// <summary>
	/// This data class contains information about the Speech to Text model customizations.
	/// </summary>
	public class Customization
	{
		/// <summary>
		/// The GUID of the custom language model. 
		/// </summary>
		public string customization_id { get; set; }
		/// <summary>
		/// The date and time in Coordinated Universal Time (UTC) at which the custom language model was created. The value is provided in full ISO 8601 format (YYYY-MM-DDThh:mm:ss.sTZD).
		/// </summary>
		public string created { get; set; }
		/// <summary>
		/// The language of the custom language model. Currently, only en-US is supported. 
		/// </summary>
		public string language { get; set; }
		/// <summary>
		/// The GUID of the service credentials for the owner of the custom language model.
		/// </summary>
		public string owner { get; set; }
		/// <summary>
		/// The name of the custom language model. 
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// The description of the custom language model.
		/// </summary>
		public string description { get; set; }
		/// <summary>
		/// The name of the base language model for which the custom language model was created. = ['en-US_BroadbandModel', 'en-US_NarrowbandModel'].
		/// </summary>
		public string base_model_name { get; set; }
		/// <summary>
		/// The current status of the custom language model: pending indicates that the model was created but is waiting either for training data to be added or for the service to finish analyzing added data. ready indicates that the model contains data and is ready to be trained. training indicates that the model is currently being trained. For this beta release, the status field continues to be ready while the model is being trained; the field does not currently report a status of training. available indicates that the model is trained and ready to use. failed indicates that training of the model failed. = ['pending', 'ready', 'training', 'available', 'failed'].
		/// </summary>
		public string status { get; set; }
		/// <summary>
		/// A percentage that indicates the progress of the model's current training. A value of 100 means that the model is fully trained. For this beta release, the progress field does not reflect the current progress of the training; the field changes from 0 to 100 when training is complete. 
		/// </summary>
		public int progress { get; set; }
		/// <summary>
		/// If the request included unknown query parameters, the following message: Unexpected query parameter(s) ['parameters'] detected, where parameters is a list that includes a quoted string for each unknown parameter.
		/// </summary>
		public string warnings { get; set; }
	}

	/// <summary>
	/// This data class contains information about the language model customization identifier.
	/// </summary>
	public class CustomizationID
	{
		/// <summary>
		/// The GUID of the new custom language model.
		/// </summary>
		public string customization_id { get; set; }
	}

	/// <summary>
	/// A data object that contains data to create a new empty custom language mode3l.
	/// </summary>
	[fsObject]
	public class CustomLanguage
	{
		/// <summary>
		/// Name of the new custom language model.
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// The base model name - Currently only en-US_BroadbandModel is supported.
		/// </summary>
		public string base_model_name { get; set; }
		/// <summary>
		/// Description of the new custom voice model.
		/// </summary>
		public string description { get; set; }
	}

	/// <summary>
	/// The type of words from the custom model's words resource on which to train the model: all (the default) trains the model on all new words, regardless of whether they were extracted from corpora or were added or modified by the user. user trains the model only on new words that were added or modified by the user; the model is not trained on new words extracted from corpora.
	/// </summary>
	public class WordTypeToAdd
	{
		/// <summary>
		/// All word types.
		/// </summary>
		public const string ALL = "all";
		/// <summary>
		/// User word types.
		/// </summary>
		public const string USER = "user";
	}
	#endregion

	#region Custom Corpora
	/// <summary>
	/// This data class contains information about multiple custom corpora.
	/// </summary>
	public class Corpora
	{
		/// <summary>
		/// Information about corpora of the custom model. The array is empty if the custom model has no corpora.
		/// </summary>
		public Corpus[] corpora { get; set; }
	}

	/// <summary>
	/// This data class contains information about the custom corpus.
	/// </summary>
	public class Corpus
	{
		/// <summary>
		/// The name of the corpus.
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// The total number of words in the corpus. The value is 0 while the corpus is being processed.
		/// </summary>
		public int total_words { get; set; }
		/// <summary>
		/// The number of OOV words in the corpus. The value is 0 while the corpus is being processed.
		/// </summary>
		public int out_of_vocabulary_words { get; set; }
		/// <summary>
		/// The status of the corpus: analyzed indicates that the service has successfully analyzed the corpus; the custom model can be trained with data from the corpus. being_processed indicates that the service is still analyzing the corpus; the service cannot accept requests to add new corpora or words, or to train the custom model. undetermined indicates that the service encountered an error while processing the corpus. = ['analyzed', 'being_processed', 'undetermined']
		/// </summary>
		public string status { get; set; }
		/// <summary>
		/// If the status of the corpus is undetermined, the following message: Analysis of corpus 'name' failed. Please try adding the corpus again by setting the 'allow_overwrite' flag to 'true'.
		/// </summary>
		public string error { get; set; }
	}
	#endregion

	#region Custom Words
	/// <summary>
	/// This data class contains information about multiple custom words.
	/// </summary>
	[fsObject]
	public class WordsList
	{
		/// <summary>
		/// Information about each word in the custom model's words resource. The array is empty if the custom model has no words.
		/// </summary>
		public WordData[] words { get; set; }
	}

	/// <summary>
	/// This data class contains information about the custom word data.
	/// </summary>
	[fsObject]
	public class WordData
	{
		/// <summary>
		/// A custom word from the custom model. The spelling of the word is used to train the model. 
		/// </summary>
		public string word { get; set; }
		/// <summary>
		/// An array of pronunciations for the custom word. The array can include the sounds-like pronunciation automatically generated by the service if none is provided for the word; the service adds this pronunciation when it finishes pre-processing the word. 
		/// </summary>
		public string[] sounds_like { get; set; }
		/// <summary>
		/// The spelling of the custom word that the service uses to display the word in a transcript. The field contains an empty string if no display-as value is provided for the word, in which case the word is displayed as it is spelled.
		/// </summary>
		public string display_as { get; set; }
		/// <summary>
		/// An array of sources that describes how the word was added to the custom model's words resource. For OOV words added from a corpus, includes the name of the corpus; if the word was added by multiple corpora, the names of all corpora are listed. If the word was modified or added by the user directly, the field includes the string user. 
		/// </summary>
		public string[] source { get; set; }
		/// <summary>
		/// If the service discovered a problem with the custom word's definition that you need to correct, a message that describes the problem, for example: Numbers are not allowed in sounds-like. You must correct the error before you can train the model.
		/// </summary>
		public string error { get; set; }
	}

	/// <summary>
	/// The type of words to be listed from the custom language model's words resource: all (the default) shows all words. user shows only custom words that were added or modified by the user. corpora shows only OOV that were extracted from corpora.
	/// </summary>
	public class WordType
	{
		/// <summary>
		///	All words.
		/// </summary>
		public const string ALL = "all";
		/// <summary>
		/// User words.
		/// </summary>
		public const string USER = "user";
		/// <summary>
		/// Corpora words.
		/// </summary>
		public const string CORPORA = "corpora";
	}

	/// <summary>
	/// Words object for adding words to a customization.
	/// </summary>
	[fsObject]
	public class Words
	{
		/// <summary>
		/// The words to add to a customization.
		/// </summary>
		public Word[] words { get; set; }
	}

	/// <summary>
	/// A word to add to a customization.
	/// </summary>
	[fsObject]
	public class Word
	{
		/// <summary>
		/// The word.
		/// </summary>
		public string word { get; set; }
		/// <summary>
		/// What the word sounds like.
		/// </summary>
		public string[] sounds_like { get; set; }
		/// <summary>
		/// How the word is displayed.
		/// </summary>
		public string display_as { get; set; }
	}
	#endregion
}
