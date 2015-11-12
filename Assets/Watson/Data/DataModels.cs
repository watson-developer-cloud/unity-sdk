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
* @author Richard Lyle (rolyle@us.ibm.com)
*/


using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Data
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

    #region XRAY Models
    namespace XRAY
    {
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        public class QuestionText
        {
            /// <summary>
            /// A string array of focus elements in this question.
            /// </summary>
            public string[] focus { get; set; }
            /// <summary>
            /// A string array of the lat for the question.
            /// </summary>
            public string[] lat { get; set; }
            /// <summary>
            /// The question transcript.
            /// </summary>
            public string questionText { get; set; }
            /// <summary>
            /// The question transcript with tagged elements.
            /// </summary>
            public string taggedText { get; set; }

            public QuestionText()
            { }

            public QuestionText( QA.Question q )
            {
                if ( q.focuslist != null )
                {
                    List<string> focusList = new List<string>();
                    foreach( var f in q.focuslist )
                        focusList.Add( f.value );
                    focus = focusList.ToArray();
                }
                else
                    focus = new string[0];

                if ( q.latlist != null )
                {
                    List<string> latList = new List<string>();
                    foreach( var l in q.latlist )
                        latList.Add( l.value );
                    lat = latList.ToArray();
                }
                else
                    lat = new string[0];

                questionText = q.questionText;
                taggedText = questionText;

                if ( focus != null )
                {
                    foreach( var f in focus )
                        taggedText = taggedText.Replace( f, "<Focus>" + f + "</Focus>" );
                }
                if ( lat != null )
                {
                    foreach( var l in lat )
                        taggedText = taggedText.Replace( l, "<Lat>" + l + "</Lat>" );
                }
            }
 
        };
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        public class Question
        {
            /// <summary>
            /// A question ID.
            /// </summary>
            public string _id { get; set; }
            /// <summary>
            /// The top confidence of all the answers to this question.
            /// </summary>
            public double topConfidence { get; set; }
            /// <summary>
            /// The creation date for the question.
            /// </summary>
            public string questionId { get; set; }
            public QuestionText question { get; set; }

            /// <summary>
            /// THe default constructor.
            /// </summary>
            public Question()
            { }

            /// <summary>
            /// Construct from a QA.Question object.
            /// </summary>
            /// <param name="question"></param>
            public Question( QA.Question q )
            {
                _id = q.id;
                if ( q.answers != null )
                {
                    foreach( var answer in q.answers )
                        topConfidence = Math.Max( topConfidence, answer.confidence );
                }

                questionId = q.questionId;
                question = new QuestionText( q );
            }
        };
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        public class Questions
        {
            /// <summary>
            /// Array of questions returned by GetQuestions().
            /// </summary>
            public Question[] questions { get; set; }

            public bool HasQuestion()
            {
                return questions != null && questions.Length > 0;
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Questions()
            { }

            /// <summary>
            /// Construct this object from a QA.ResponseList object.
            /// </summary>
            /// <param name="response"></param>
            public Questions( QA.ResponseList response )
            {
                if (response != null && response.responses != null )
                {
                    List<Question> questionList = new List<Question>();
                    foreach( var resp in response.responses )
                    {
                        if ( resp.question != null )
                            questionList.Add( new Question( resp.question ) );
                    }
                    questions = questionList.ToArray();
                }
                else
                    questions = null;
            }

            public Questions( QA.Response response )
            {
                if ( response != null && response.question != null )
                    questions = new Question[] { new Question( response.question ) };
            }

            public Questions( QA.Question q )
            {
                if ( q != null )
                    questions = new Question[] { new Question( q ) };
            }
        };

        /// <summary>
        /// The position of a word in the parse tree data.
        /// </summary>
        public enum WordPosition
        {
            INVALID = -1,
            NOUN,
            PRONOUN,
            ADJECTIVE,
            DETERMINIER,
            VERB,
            ADVERB,
            PREPOSITION,
            CONJUNCTION,
            INTERJECTION
        };
        /// <summary>
        /// This data class holds a single word of the ParseData.
        /// </summary>
        public class ParseWord
        {
            public string Word { get; set; }
            public WordPosition Pos { get; set; }
            public string Slot { get; set; }
            public string[] Features { get; set; }

            public string PosName
            {
                set
                {
                    WordPosition pos = WordPosition.INVALID;
                    if (!sm_WordPositions.TryGetValue(value, out pos))
                        Log.Error("XRAY", "Failed to find position type for {0}, Word: {1}", value, Word);
                    Pos = pos;
                }
            }

            private static Dictionary<string, WordPosition> sm_WordPositions = new Dictionary<string, WordPosition>() {
                { "noun", WordPosition.NOUN },
                { "pronoun", WordPosition.PRONOUN },        // ?
                { "adj", WordPosition.ADJECTIVE },
                { "det", WordPosition.DETERMINIER },
                { "verb", WordPosition.VERB },
                { "adverb", WordPosition.ADVERB },          // ?
                { "prep", WordPosition.PREPOSITION },
                { "conj", WordPosition.CONJUNCTION },       // ?
                { "inter", WordPosition.INTERJECTION },     // ?
            };
        };
        /// <summary>
        /// This data class is returned by the GetParseData() function.
        /// </summary>
        public class ParseData
        {
            public string Id { get; set; }
            public string Rev { get; set; }
            public long TransactionId { get; set; }
            public ParseWord[] Words { get; set; }
            public string[] Heirarchy { get; set; }
            public string[] Flags { get; set; }

            public ParseData()
            { }

            /// <summary>
            /// Marshal a QA.Question object into this ParseData object.
            /// </summary>
            /// <param name="question"></param>
            public ParseData( QA.Question question )
            {
                Id = question.id;

                if ( question.xsgtopparses != null && question.xsgtopparses.Length > 0 )
                {
                    Queue<QA.Word> tree = new Queue<QA.Word>();
                    tree.Enqueue( question.xsgtopparses[0] );

                    List<ParseWord> words = new List<ParseWord>();
                    while( tree.Count > 0 )
                    {
                        QA.Word word = tree.Dequeue();
                        if (word.lmods != null )
                        {
                            foreach( var child in word.lmods )
                                tree.Enqueue( child );
                        }
                        if ( word.rmods != null )
                        {
                            foreach( var child in word.rmods )
                                tree.Enqueue( child );
                        }

                        ParseWord parseWord = new ParseWord();
                        parseWord.Slot = word.slotname;
                        parseWord.Word = word.wordtext;
                        parseWord.Features = word.features.Split( ' ' );
                        parseWord.PosName = parseWord.Features[0];

                        int index = int.Parse( word.seqno ) - 1;
                        while(words.Count <= index )
                            words.Add( null );
                        words[ index ] = parseWord;
                    }

                    Words = words.ToArray();
                }
                else
                {
                    Log.Warning( "ParseData", "XSG parse tree not available." );

                    List<ParseWord> words = new List<ParseWord>();
                    foreach( var word in question.questionText.Split( ' ' ) )
                    {
                        ParseWord parseWord = new ParseWord();
                        parseWord.Slot = string.Empty;
                        parseWord.Word = word;
                        parseWord.Features = new string[0];

                        // look for the part of speech in the synonymList..
                        if ( question.synonymList != null )
                        {
                            foreach( var syn in question.synonymList )
                            {
                                if ( syn.value == word )
                                    parseWord.PosName = syn.partOfSpeech;
                            }
                        }
                        words.Add( parseWord );
                    }

                    Words = words.ToArray();
                }
            }

            public bool ParseJson(IDictionary json)
            {
                try
                {
                    Id = (string)json["_id"];
                    Rev = (string)json["_rev"];
                    TransactionId = (long)json["transactionId"];

                    IDictionary iparse = (IDictionary)json["parse"];
                    List<string> heirarchy = new List<string>();
                    IList iheirarchy = (IList)iparse["hierarchy"];
                    foreach (var h in iheirarchy)
                        heirarchy.Add((string)h);
                    Heirarchy = heirarchy.ToArray();

                    List<string> flags = new List<string>();
                    IList iflags = (IList)iparse["flags"];
                    foreach (var f in iflags)
                        flags.Add((string)f);
                    Flags = flags.ToArray();

                    List<ParseWord> words = new List<ParseWord>();

                    IList iWords = (IList)iparse["words"];
                    for (int i = 0; i < iWords.Count; ++i)
                    {
                        ParseWord word = new ParseWord();
                        word.Word = (string)iWords[i];

                        IList iPos = (IList)iparse["pos"];
                        if (iPos.Count != iWords.Count)
                            throw new WatsonException("ipos.Count != iwords.Count");
                        word.PosName = (string)((IDictionary)iPos[i])["value"];

                        IList iSlots = (IList)iparse["slot"];
                        if (iSlots.Count != iWords.Count)
                            throw new WatsonException("islots.Count != iwords.Count");
                        word.Slot = (string)((IDictionary)iSlots[i])["value"];

                        IList iFeatures = (IList)iparse["features"];
                        if (iFeatures.Count != iWords.Count)
                            throw new WatsonException("ifeatures.Count != iwords.Count");

                        List<string> features = new List<string>();
                        IList iWordFeatures = (IList)((IDictionary)iFeatures[i])["value"];
                        foreach (var k in iWordFeatures)
                            features.Add((string)k);
                        word.Features = features.ToArray();

                        words.Add(word);
                    }

                    Words = words.ToArray();
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("XRAY", "Exception during parse: {0}", e.ToString());
                }

                return false;
            }
        };

        public class Evidence
        {
            public string title { get; set; }
            public string passage { get; set; }
            public string decoratedPassage { get; set; }
            public string corpus { get; set; }

            public Evidence()
            { }
            public Evidence( QA.Evidence e, string answer = null )
            {
                title = e.title;
                passage = e.text;
                decoratedPassage = passage;
                if ( answer != null )
                    decoratedPassage = decoratedPassage.Replace( answer, "<answer>" + answer + "</answer>" );

                if ( e.metadataMap != null )
                    corpus = e.metadataMap.corpusName;
            }
        };
        public class Variant
        {
            public string text { get; set; }
            public string relationship { get; set; }
        };
        public class Feature
        {
            public string featureId { get; set; }
            public string label { get; set; }
            public string displayLabel { get; set; }
            public double unweightedScore { get; set; }
            public double weightedScore { get; set; }
        };
        public class Answer
        {
            public string answerText { get; set; }
            public double confidence { get; set; }
            public bool correctAnswer { get; set; }
            public Evidence[] evidence { get; set; }
            public Variant[] variants { get; set; }
            public Feature[] features { get; set; }

            public Answer()
            { }
            public Answer( QA.Answer a )
            {
                answerText = a.text;
                confidence = a.confidence;

                if ( a.evidence != null )
                {
                    evidence = new Evidence[ a.evidence.Length ];
                    for(int i=0;i<evidence.Length;++i)
                        evidence[i] = new Evidence( a.evidence[i], answerText );
                }
            }
        };
        public class Answers
        {
            public string _id { get; set; }
            public string _rev { get; set; }
            public long transactionId { get; set; }
            public double featureScoreMin { get; set; }
            public double featureScoreMax { get; set; }
            public double featureScoreRange { get; set; }
            public Answer[] answers { get; set; }

            public bool HasAnswer()
            {
                return answers != null && answers.Length > 0;
            }

            public Answers()
            { }
            public Answers( QA.Question q )
            {
                _id = q.id;
                if ( q.answers != null )
                {
                    Answer bestAnswer = null;

                    answers = new Answer[ q.answers.Length ];
                    for(int i=0;i<answers.Length;++i)
                    {
                        QA.Answer a = q.answers[i];

                        // WEA answers have their evidence in the evidenceList of the question, if we have no
                        // evidence in the answer, then copy the evidence over into the answer.
                        if ( a.evidence == null && q.evidencelist != null && a.text.Contains( "-" ) )
                        {
                            // extract the evidence ID from the answer text in a WEA
                            // "text": "142B100455C66F896BBE4FD60C849E08 - PM #8214942 v3C NWS GWF 2 Sculptor and Rankin Completions Sand Control Selection : 5. Sand Analysis : 5.3 PSD Analysis",
                            string evidenceId = a.text.Substring( 0, a.text.IndexOf( '-' ) ).Trim();
                            // strip the ID from the answer text..
                            a.text = a.text.Substring( a.text.IndexOf( '-' ) + 1 );

                            List<QA.Evidence> evidenceList = new List<QA.Evidence>();
                            foreach( var e in q.evidencelist )
                            {
                                if ( e.id.EndsWith( evidenceId ) )
                                    evidenceList.Add( e );
                            }

                            a.evidence = evidenceList.ToArray();
                        }

                        answers[i] = new Answer( a );

                        if ( bestAnswer == null || bestAnswer.confidence < answers[i].confidence )
                            bestAnswer = answers[i];
                    }

                    // mark the most correct answer..
                    if ( bestAnswer != null )
                        bestAnswer.correctAnswer = true;
                }
            }
        };
    }
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

        public bool HasResult()
        {
            return Results != null && Results.Length > 0
                && Results[0].Alternatives != null && Results[0].Alternatives.Length > 0;
        }

        public bool HasFinalResult()
        {
            return HasResult() && Results[0].Final;
        }
    };
    #endregion

    #region QA Models
    namespace QA
    {
        public class Value
        {
            public string value { get; set; }
        };

        public class MetaDataMap
        {
            public string originalFile { get; set; }
            public string title { get; set; }
            public string corpusName { get; set; }
            public string fileName { get; set; }
            public string DOCNO { get; set; }
            public string CorpusPlusDocno { get; set; }
        };

        public class Evidence
        {
            public string value { get; set; }
            public string text { get; set; }
            public string id { get; set; }
            public string title { get; set; }
            public string document { get; set; }
            public string copyright { get; set; }
            public string termsOfUse { get; set; }
            public MetaDataMap metadataMap { get; set; }
        };

        public class Synonym
        {
            public bool isChosen { get; set; }
            public string value { get; set; }
            public double weight { get; set; }
        };

        public class SynSet
        {
            public string name { get; set; }
            public Synonym[] synSet { get; set; }
        };

        public class SynonymList
        {
            public string partOfSpeech { get; set; }
            public string value { get; set; }
            public string lemma { get; set; }
            public SynSet[] synSet { get; set; }
        };

        public class EvidenceRequest
        {
            public long items { get; set; }
            public string profile { get; set; }
        };

        public class Answer
        {
            public long id { get; set; }
            public string text { get; set; }
            public string pipeline { get; set; }
            public double confidence { get; set; }
            public Evidence [] evidence { get; set; }
            public string[] entityTypes { get; set; }
        };
        public class Slots
        {
            public string pred { get; set; }
            public string subj { get; set; }
            public string objprep { get; set; }
            public string psubj { get; set; }
        };
        public class Word
        {
            public Slots compSlotParseNodes { get; set; }
            public string slotname { get; set; }
            public string wordtext { get; set; }
            public string slotnameoptions { get; set; }
            public string wordsense { get; set; }
            public string numericsense { get; set; }
            public string seqno { get; set; }
            public string wordbegin { get; set; }
            public string framebegin { get; set; }
            public string frameend { get; set; }
            public string wordend { get; set; }
            public string features { get; set; }
            public Word[] lmods { get; set; }
            public Word[] rmods { get; set; }
        };
        public class ParseTree : Word
        {
            public string parseScore { get; set; }
        };

        public class Question
        {
            public Value[] qclasslist { get; set; }
            public Value[] focuslist { get; set; }
            public Value[] latlist { get; set; }
            public Evidence[] evidencelist { get; set; }
            public SynonymList[] synonymList { get; set; }
            public string[] disambiguatedEntities { get; set; }
            public ParseTree[] xsgtopparses { get; set; }
            public string casXml { get; set; }
            public string pipelineid { get; set; }
            public bool formattedAnswer { get; set; }
            public string selectedProcessingComponents { get; set; }
            public string category { get; set; }
            public long items { get; set; }
            public string status { get; set; }
            public string id { get; set; }
            public string questionText { get; set; }
            public EvidenceRequest evidenceRequest { get; set; }
            public Answer[] answers { get; set; }
            public string[] errorNotifications { get; set; }
            public string passthru { get; set; }

            public string questionId { get; set; }      // local cache ID
        };
        public class QuestionClass
        {
            public string out_of_domain { get; set; }
            public string question { get; set; }
            public string domain { get; set; }
        };
        /// <summary>
        /// The response object for QA.AskQuestion().
        /// </summary>
        public class Response
        {
            public Question question { get; set; }
            public QuestionClass[] questionClasses { get; set; }
        };

        /// <summary>
        /// A list of responses.
        /// </summary>
        public class ResponseList
        {
            public Response[] responses { get; set; }
        };
    }
    #endregion
}
