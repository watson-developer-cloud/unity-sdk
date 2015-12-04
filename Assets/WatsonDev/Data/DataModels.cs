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



using FullSerializer;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBM.Watson.DataModels
{
    #region XRAY Models
    namespace XRAY
    {
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        [fsObject]
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

            public QuestionText(QA.Question q)
            {
                if (q.focuslist != null)
                {
                    List<string> focusList = new List<string>();
                    foreach (var f in q.focuslist)
                        focusList.Add(f.value);
                    focus = focusList.ToArray();
                }
                else
                    focus = new string[0];

                if (q.latlist != null)
                {
                    List<string> latList = new List<string>();
                    foreach (var l in q.latlist)
                        latList.Add(l.value);
                    lat = latList.ToArray();
                }
                else
                    lat = new string[0];

                questionText = q.questionText;
                taggedText = questionText;

                if (focus != null)
                {
                    foreach (var f in focus)
                        taggedText = taggedText.Replace(f, "<Focus>" + f + "</Focus>");
                }
                if (lat != null)
                {
                    foreach (var l in lat)
                        taggedText = taggedText.Replace(l, "<Lat>" + l + "</Lat>");
                }
            }

        };
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        [fsObject]
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
            public Question(QA.Question q)
            {
                _id = q.id;
                if (q.answers != null)
                {
                    foreach (var answer in q.answers)
                        topConfidence = Math.Max(topConfidence, answer.confidence);
                }

                questionId = q.questionId;
                question = new QuestionText(q);
            }
        };
        /// <summary>
        /// Data class for GetQuestions() method.
        /// </summary>
        [fsObject]
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
            public Questions(QA.ResponseList response)
            {
                if (response != null && response.responses != null)
                {
                    List<Question> questionList = new List<Question>();
                    foreach (var resp in response.responses)
                    {
                        if (resp.question != null)
                            questionList.Add(new Question(resp.question));
                    }
                    questions = questionList.ToArray();
                }
                else
                    questions = null;
            }

            public Questions(QA.Response response)
            {
                if (response != null && response.question != null)
                    questions = new Question[] { new Question(response.question) };
            }

            public Questions(QA.Question q)
            {
                if (q != null)
                    questions = new Question[] { new Question(q) };
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
            INTERJECTION,
            SPECIAL,
            SUBINF
        };
        /// <summary>
        /// This data class holds a single word of the ParseData.
        /// </summary>
        [fsObject]
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
                        Log.Error("XRAY", "Failed to find PosName: {0}, Word: {1}", value, Word);
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
                { "adv", WordPosition.ADVERB },             // ?
                { "prep", WordPosition.PREPOSITION },
                { "conj", WordPosition.CONJUNCTION },       // ?
                { "inter", WordPosition.INTERJECTION },     // ?
                { "incomplete", WordPosition.INVALID },
                { "special", WordPosition.SPECIAL },
                { "subinf", WordPosition.SUBINF },
            };
        };

        [fsObject]
        public class ParseTree
        {
            public long position { get; set; }
            public string text { get; set; }
            public int parentNode { get; set; }
            public ParseTree[] rightChildren { get; set; }
            public ParseTree[] leftChildren { get; set; }
        };

        [fsObject]
        public class Value
        {
            public string text { get; set; }
            public string value { get; set; }
        };
        [fsObject]
        public class ArrayValue
        {
            public string text { get; set; }
            public string[] value { get; set; }
        };
        [fsObject]
        public class Parse
        {
            public string[] flags { get; set; }
            public string[] words { get; set; }
            public Value[] pos { get; set; }
            public Value[] slot { get; set; }
            public ArrayValue[] features { get; set; }
        };

        public class ParseDataProcessor : fsObjectProcessor
        {
            public override bool CanProcess(Type type)
            {
                return typeof(ParseData).IsAssignableFrom(type);
            }

            public override void OnAfterDeserialize(Type storageType, object instance)
            {
                ParseData parseData = instance as ParseData;
                if (parseData == null)
                    throw new WatsonException("Unexpected type.");

                base.OnAfterDeserialize(storageType, instance);

                List<ParseWord> words = new List<ParseWord>();

                if (parseData.parse != null
                    && parseData.parse.words != null)
                {
                    for (int i = 0; i < parseData.parse.words.Length; ++i)
                    {
                        ParseWord word = new ParseWord();
                        word.Word = parseData.parse.words[i];
                        word.PosName = parseData.parse.pos[i].value;
                        word.Slot = parseData.parse.slot[i].value;
                        word.Features = parseData.parse.features[i].value;
                        words.Add(word);
                    }
                }

                parseData.Words = words.ToArray();
            }
        };

        /// <summary>
        /// This data class is returned by the GetParseData() function.
        /// </summary>
        [fsObject(Processor = typeof(ParseDataProcessor))]
        public class ParseData
        {
            public ParseWord[] Words { get; set; }
            public Parse parse { get; set; }
            public ParseTree parseTree { get; set; }
        };

        [fsObject]
        public class Evidence
        {
            public string title { get; set; }
            public string passage { get; set; }
            public string decoratedPassage { get; set; }
            public string corpus { get; set; }

            public Evidence()
            { }
            public Evidence(QA.Evidence e, string answer = null)
            {
                title = e.title;
                passage = e.text;
                decoratedPassage = passage;
                if (answer != null)
                    decoratedPassage = decoratedPassage.Replace(answer, "<answer>" + answer + "</answer>");

                if (e.metadataMap != null)
                    corpus = e.metadataMap.corpusName;
            }
        };
        [fsObject]
        public class Variant
        {
            public string text { get; set; }
            public string relationship { get; set; }
        };
        [fsObject]
        public class Feature
        {
            public string featureId { get; set; }
            public string label { get; set; }
            public string displayLabel { get; set; }
            public double unweightedScore { get; set; }
            public double weightedScore { get; set; }
        };

        [fsObject]
        public class Cell
        {
            public string Value { get; set; }
            public int ColSpan { get; set; }            // how many colums does this cell span, by default just 1..
            public bool Highlighted { get; set; }
        };
        [fsObject]
        public class Row
        {
            public Cell[] columns { get; set; }
        };

        [fsObject]
        public class Table
        {
            public Row[] rows { get; set; }
        };

        [fsObject]
        public class Answer
        {
            public string answerText { get; set; }
            public double confidence { get; set; }
            public bool correctAnswer { get; set; }
            public Evidence[] evidence { get; set; }
            public Variant[] variants { get; set; }
            public Feature[] features { get; set; }
            public Table[] tables { get; set; }

            public Answer()
            { }
            public Answer(QA.Answer a)
            {
                answerText = a.text;
                confidence = a.confidence;
                tables = a.ExtractTables(answerText);

                if (a.evidence != null)
                {
                    evidence = new Evidence[a.evidence.Length];
                    for (int i = 0; i < evidence.Length; ++i)
                        evidence[i] = new Evidence(a.evidence[i], answerText);
                }
            }
        };
        [fsObject]
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
            public Answers(QA.Question q)
            {
                _id = q.id;
                if (q.answers != null)
                {
                    Answer bestAnswer = null;

                    answers = new Answer[q.answers.Length];
                    for (int i = 0; i < answers.Length; ++i)
                    {
                        QA.Answer a = q.answers[i];

                        // WEA answers have their evidence in the evidenceList of the question, if we have no
                        // evidence in the answer, then copy the evidence over into the answer.
                        if (a.evidence == null && q.evidencelist != null && a.text.Contains("-"))
                        {
                            // extract the evidence ID from the answer text in a WEA
                            // "text": "142B100455C66F896BBE4FD60C849E08 - PM #8214942 v3C NWS GWF 2 Sculptor and Rankin Completions Sand Control Selection : 5. Sand Analysis : 5.3 PSD Analysis",
                            string evidenceId = a.text.Substring(0, a.text.IndexOf('-')).Trim();

                            StringBuilder weaAnswer = new StringBuilder();

                            List<QA.Evidence> evidenceList = new List<QA.Evidence>();
                            foreach (var e in q.evidencelist)
                            {
                                if (e.id.EndsWith(evidenceId))
                                {
                                    evidenceList.Add(e);

                                    weaAnswer.Append("<b><size=27>" + e.title + "</size></b>\n\n");
                                    weaAnswer.Append(e.text + "\n\n");
                                }
                            }

                            a.text = weaAnswer.ToString();
                            a.evidence = evidenceList.ToArray();
                        }

                        answers[i] = new Answer(a);

                        if (bestAnswer == null || bestAnswer.confidence < answers[i].confidence)
                            bestAnswer = answers[i];
                    }

                    // mark the most correct answer..
                    if (bestAnswer != null)
                        bestAnswer.correctAnswer = true;
                }
            }
        };

        [fsObject]
        public class AskResponse
        {
            public Questions questions { get; set; }
            public Answers answers { get; set; }
            public ParseData parseData { get; set; }
        };
    }
    #endregion

    #region QA Models
    namespace QA
    {
        [fsObject]
        public class Value
        {
            public string value { get; set; }
        };

        [fsObject]
        public class MetaDataMap
        {
            public string originalFile { get; set; }
            public string title { get; set; }
            public string corpusName { get; set; }
            public string fileName { get; set; }
            public string DOCNO { get; set; }
            public string CorpusPlusDocno { get; set; }
        };

        [fsObject]
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

        [fsObject]
        public class Synonym
        {
            public bool isChosen { get; set; }
            public string value { get; set; }
            public double weight { get; set; }
        };

        [fsObject]
        public class SynSet
        {
            public string name { get; set; }
            public Synonym[] synSet { get; set; }
        };

        [fsObject]
        public class SynonymList
        {
            public string partOfSpeech { get; set; }
            public string value { get; set; }
            public string lemma { get; set; }
            public SynSet[] synSet { get; set; }
        };

        [fsObject]
        public class EvidenceRequest
        {
            public long items { get; set; }
            public string profile { get; set; }
        };

        [fsObject]
        public class Answer
        {
            public long id { get; set; }
            public string text { get; set; }
            public string pipeline { get; set; }
            public string formattedText { get; set; }
            public double confidence { get; set; }
            public Evidence[] evidence { get; set; }
            public string[] entityTypes { get; set; }

            private static string CleanInnerText(string text)
            {
                text = text.Replace("&nbsp;", " ");
                text = text.Replace("\\n", "\n");
                text = text.Replace("\\r", "\r");
                text = text.Replace("\\t", "\t");
                return text.Trim(new char[] { '\n', '\r', '\t', ' ' });
            }

            /// <summary>
            /// Helper function to extract all tables from the formatted answer
            /// </summary>
            /// <returns>An array of all tables found.</returns>
            public XRAY.Table[] ExtractTables(string answer)
            {
                if (!string.IsNullOrEmpty(formattedText))
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(formattedText);

                    if (doc.DocumentNode == null)
                        return null;
                    var table_nodes = doc.DocumentNode.SelectNodes("//table");
                    if (table_nodes == null)
                        return null;

                    List<XRAY.Table> tables = new List<XRAY.Table>();
                    foreach (var table in table_nodes)
                    {
                        var row_nodes = table.SelectNodes("*/tr");
                        if (row_nodes == null)
                            continue;

                        List<XRAY.Row> rows = new List<XRAY.Row>();
                        foreach (var row in row_nodes)
                        {
                            var cell_nodes = row.SelectNodes("*/th|td");
                            if (cell_nodes == null)
                                continue;

                            List<XRAY.Cell> cells = new List<XRAY.Cell>();
                            foreach (var cell in cell_nodes)
                            {
                                string text = CleanInnerText(cell.InnerText);

                                int colspan = 1;
                                if (cell.Attributes.Contains("colspan"))
                                    colspan = int.Parse(cell.Attributes["colspan"].Value);
                                bool bHighlighted = false;
                                if (text == answer)
                                    bHighlighted = true;

                                cells.Add(new XRAY.Cell() { Value = text, ColSpan = colspan, Highlighted = bHighlighted });
                                for (int i = 1; i < colspan; ++i)
                                    cells.Add(null);      // add empty cells for the spans
                            }

                            rows.Add(new XRAY.Row() { columns = cells.ToArray() });
                        }

                        tables.Add(new XRAY.Table() { rows = rows.ToArray() });
                    }

                    return tables.ToArray();
                }

                return null;
            }
        };
        [fsObject]
        public class Slots
        {
            public string pred { get; set; }
            public string subj { get; set; }
            public string objprep { get; set; }
            public string psubj { get; set; }
        };
        [fsObject]
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
        [fsObject]
        public class ParseTree : Word
        {
            public string parseScore { get; set; }
        };

        [fsObject]
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
        [fsObject]
        public class QuestionClass
        {
            public string out_of_domain { get; set; }
            public string question { get; set; }
            public string domain { get; set; }
        };
        /// <summary>
        /// The response object for QA.AskQuestion().
        /// </summary>
        [fsObject]
        public class Response
        {
            public Question question { get; set; }
            public QuestionClass[] questionClasses { get; set; }
        };

        /// <summary>
        /// A list of responses.
        /// </summary>
        [fsObject]
        public class ResponseList
        {
            public Response[] responses { get; set; }
        };
    }

    #endregion
}
