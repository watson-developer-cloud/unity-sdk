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
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.DeepQA.v1
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

        /// <summary>
        /// Helper function to extract all tables from the formatted answer
        /// </summary>
        /// <returns>An array of all tables found.</returns>
        public Table[] ExtractTables(string answer)
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

                List<Table> tables = new List<Table>();
                foreach (var table in table_nodes)
                {
                    var row_nodes = table.SelectNodes("*/tr");
                    if (row_nodes == null)
                        continue;

                    List<Row> rows = new List<Row>();
                    foreach (var row in row_nodes)
                    {
                        var cell_nodes = row.SelectNodes("*/th|td");
                        if (cell_nodes == null)
                            continue;

                        List<Cell> cells = new List<Cell>();
                        foreach (var cell in cell_nodes)
                        {
                            string text = CleanInnerText(cell.InnerText);

                            int colspan = 1;
                            if (cell.Attributes.Contains("colspan"))
                                colspan = int.Parse(cell.Attributes["colspan"].Value);
                            bool bHighlighted = false;
                            if (text == answer)
                                bHighlighted = true;

                            cells.Add(new Cell() { Value = text, ColSpan = colspan, Highlighted = bHighlighted });
                            for (int i = 1; i < colspan; ++i)
                                cells.Add(null);      // add empty cells for the spans
                        }

                        rows.Add(new Row() { columns = cells.ToArray() });
                    }

                    tables.Add(new Table() { rows = rows.ToArray() });
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
