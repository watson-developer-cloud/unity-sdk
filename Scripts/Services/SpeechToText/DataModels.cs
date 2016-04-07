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
    /// This data class holds the data for a given speech model.
    /// </summary>
    [fsObject]
    public class SpeechModel
    {
        /// <summary>
        /// The name of the speech model.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The optimal sample rate for this model.
        /// </summary>
        public long Rate { get; set; }
        /// <summary>
        /// The language ID for this model. (e.g. en)
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// A description for this model.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The URL for this model.
        /// </summary>
        public string URL { get; set; }
    };
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
}
