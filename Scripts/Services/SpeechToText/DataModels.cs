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
	/// <summary>
	/// This data class holds the actual transcript for the text generated from speech audio data.
	/// </summary>
	[fsObject]
	public class SpeechAlt
	{
		/// <summary>
		/// The transcript of what was understood.
		/// </summary>
		public string Transcript { get; set; }
		/// <summary>
		/// The confidence in this transcript of the audio data.
		/// </summary>
		public double Confidence { get; set; }
		/// <summary>
		/// A optional array of timestamps objects.
		/// </summary>
		public TimeStamp[] Timestamps { get; set; }
		/// <summary>
		/// A option array of word confidence values.
		/// </summary>
		public WordConfidence[] WordConfidence { get; set; }
	};
	/// <summary>
	/// A Result object that is returned by the Recognize() method.
	/// </summary>
	[fsObject]
	public class SpeechResult
	{
		/// <summary>
		/// If true, then this is the final result and no more results will be sent for the given audio data.
		/// </summary>
		public bool Final { get; set; }
		/// <summary>
		/// A array of alternatives speech to text results, this is controlled by the MaxAlternatives property.
		/// </summary>
		public SpeechAlt[] Alternatives { get; set; }
	};
	/// <summary>
	/// This data class holds a list of Result objects returned by the Recognize() method.
	/// </summary>
	[fsObject]
	public class SpeechResultList
	{
		/// <summary>
		/// The array of Result objects.
		/// </summary>
		public SpeechResult[] Results { get; set; }

		/// <exclude />
		public SpeechResultList(SpeechResult[] results)
		{
			Results = results;
		}

		/// <summary>
		/// Check if our result list has atleast one valid result.
		/// </summary>
		/// <returns>Returns true if a result is found.</returns>
		public bool HasResult()
		{
			return Results != null && Results.Length > 0
				&& Results[0].Alternatives != null && Results[0].Alternatives.Length > 0;
		}

		/// <summary>
		/// Returns true if we have a final result.
		/// </summary>
		/// <returns></returns>
		public bool HasFinalResult()
		{
			return HasResult() && Results[0].Final;
		}
	};

















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

	#region Sessions
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

	#endregion

	#region Sessionless
	#endregion

	#region Asynchronous
	#endregion

	#region Custom Models
	#endregion

	#region Custom Corpora
	#endregion

	#region Custom Words
	#endregion
}
