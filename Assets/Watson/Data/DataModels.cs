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

namespace IBM.Watson.DataModels
{
    #region Dialog Models
    /// <summary>
    /// This data class is contained by Dialogs, it represents a single dialog available.
    /// </summary>
    public class DialogEntry
    {
        /// <summary>
        /// The dialog ID.
        /// </summary>
        public string dialog_id { get; set; }
        /// <summary>
        /// The user supplied name for the dialog.
        /// </summary>
        public string name { get; set; }
    };
    /// <summary>
    /// The object returned by GetDialogs().
    /// </summary>
    public class Dialogs
    {
        /// <summary>
        /// The array of Dialog's available.
        /// </summary>
        public DialogEntry[] dialogs { get; set; }
    };
    /// <summary>
    /// This data class holds the response to a call to Converse().
    /// </summary>
    public class ConverseResponse
    {
        /// <summary>
        /// An array of response strings.
        /// </summary>
        public string[] response { get; set; }
        /// <summary>
        /// The text input passed into Converse().
        /// </summary>
        public string input { get; set; }
        /// <summary>
        /// The conversation ID to use in future calls to Converse().
        /// </summary>
        public int conversation_id { get; set; }
        /// <summary>
        /// The confidence in this response.
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// The client ID of the user.
        /// </summary>
        public int client_id { get; set; }
    };
    #endregion

    #region Translation Models
    /// <summary>
    /// Language data class.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// String that contains the country code.
        /// </summary>
        public string language { get; set; }        // country code of the language 
                                                    /// <summary>
                                                    /// The language name.
                                                    /// </summary>
        public string name { get; set; }        // name of the language                                    
    }
    /// <summary>
    /// Languages data class.
    /// </summary>
    public class Languages
    {
        /// <summary>
        /// Array of language objects.
        /// </summary>
        public Language[] languages { get; set; }
    }
    /// <summary>
    /// Translation data class.
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// Translation text.
        /// </summary>
        public string translation { get; set; }
    };
    /// <summary>
    /// Translate data class returned by the TranslateCallback.
    /// </summary>
    public class Translations
    {
        public long word_count { get; set; }
        public long character_count { get; set; }
        public Translation[] translations { get; set; }
    }
    /// <summary>
    /// Language model data class.
    /// </summary>
    public class TranslationModel
    {
        /// <summary>
        /// The language model ID.
        /// </summary>
        public string model_id { get; set; }
        /// <summary>
        /// The name of the language model.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The source language ID.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// The target language ID.
        /// </summary>
        public string target { get; set; }
        /// <summary>
        /// The model of which this model was based.
        /// </summary>
        public string base_model_id { get; set; }
        /// <summary>
        /// The domain of the language model.
        /// </summary>
        public string domain { get; set; }
        /// <summary>
        /// Is this model customizable?
        /// </summary>
        public bool customizable { get; set; }
        /// <summary>
        /// Is this model default.
        /// </summary>
        public bool @default { get; set; }
        /// <summary>
        /// Who is the owner of this model.
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// What is the status of this model.
        /// </summary>
        public string status { get; set; }
    }
    /// <summary>
    /// Models data class.
    /// </summary>
    public class TranslationModels
    {
        public TranslationModel[] models { get; set; }
    }
    #endregion

    #region NLC Models
    /// <summary>
    /// This data class holds the data for a given classifier returned by GetClassifier().
    /// </summary>
    public class Classifier
    {
        /// <summary>
        /// The name of the classifier.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The language ID of the classifier (e.g. en)
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The URL for the classifier.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The classifier ID.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// When was this classifier created.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Whats the current status of this classifier.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// A description of the classifier status.
        /// </summary>
        public string status_description { get; set; }
    };
    /// <summary>
    /// This data class wraps an array of Classifiers.
    /// </summary>
    public class Classifiers
    {
        public Classifier[] classifiers { get; set; }
    };
    /// <summary>
    /// A class returned by the ClassifyResult object.
    /// </summary>
    public class Class
    {
        /// <summary>
        /// The confidence in this class.
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// The name of the class.
        /// </summary>
        public string class_name { get; set; }
    };
    /// <summary>
    /// This result object is returned by the Classify() method.
    /// </summary>
    public class ClassifyResult
    {
        /// <summary>
        /// The ID of the classifier used.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// The URL of the classifier.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The input text into the classifier.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The top class found for the text.
        /// </summary>
        public string top_class { get; set; }
        /// <summary>
        /// A array of all classifications for the input text.
        /// </summary>
        public Class[] classes { get; set; }

        /// <summary>
        /// Helper function to return the top confidence value of all the returned classes.
        /// </summary>
        public double topConfidence
        {
            get
            {
                double fTop = 0.0;
                if (classes != null)
                {
                    foreach (var c in classes)
                        fTop = Math.Max(c.confidence, fTop);
                }
                return fTop;
            }
        }
    };
    #endregion

    #region SpeechToText Models
    /// <summary>
    /// This data class holds the data for a given speech model.
    /// </summary>
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
    #endregion
}
