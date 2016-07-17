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
	/// <summary>
	/// The returned value.
	/// </summary>
    [fsObject]
    public class Value
    {
		/// <summary>
		/// The value.
		/// </summary>
        public string value { get; set; }
    };

	/// <summary>
	/// The metadata of the answer.
	/// </summary>
    [fsObject]
    public class MetaDataMap
    {
		/// <summary>
		/// The original file.
		/// </summary>
        public string originalFile { get; set; }
		/// <summary>
		/// The title.
		/// </summary>
        public string title { get; set; }
		/// <summary>
		/// The corpus name.
		/// </summary>
        public string corpusName { get; set; }
		/// <summary>
		/// The file name.
		/// </summary>
        public string fileName { get; set; }
		/// <summary>
		/// The document number.
		/// </summary>
        public string DOCNO { get; set; }
		/// <summary>
		/// The corpus and document number.
		/// </summary>
        public string CorpusPlusDocno { get; set; }
    };

	/// <summary>
	/// The evidence of the answer.
	/// </summary>
    [fsObject]
    public class Evidence
    {
		/// <summary>
		/// The value.
		/// </summary>
        public string value { get; set; }
		/// <summary>
		/// The text.
		/// </summary>
        public string text { get; set; }
		/// <summary>
		/// The identifier.
		/// </summary>
        public string id { get; set; }
		/// <summary>
		/// The title.
		/// </summary>
        public string title { get; set; }
		/// <summary>
		/// The document.
		/// </summary>
        public string document { get; set; }
		/// <summary>
		/// The copyright.
		/// </summary>
        public string copyright { get; set; }
		/// <summary>
		/// The terms of use.
		/// </summary>
        public string termsOfUse { get; set; }
		/// <summary>
		/// The metadata map.
		/// </summary>
        public MetaDataMap metadataMap { get; set; }
    };

	/// <summary>
	/// Synonym of an answer.
	/// </summary>
    [fsObject]
    public class Synonym
    {
		/// <summary>
		/// Is the synonym chosen?
		/// </summary>
        public bool isChosen { get; set; }
		/// <summary>
		/// The synonym value.
		/// </summary>
        public string value { get; set; }
		/// <summary>
		/// The synonym weight.
		/// </summary>
        public double weight { get; set; }
    };

	/// <summary>
	/// Synonym set.
	/// </summary>
    [fsObject]
    public class SynSet
    {
		/// <summary>
		/// The synset name.
		/// </summary>
        public string name { get; set; }
		/// <summary>
		/// The synonyms.
		/// </summary>
        public Synonym[] synSet { get; set; }
    };

	/// <summary>
	/// Synonym list.
	/// </summary>
    [fsObject]
    public class SynonymList
    {
		/// <summary>
		/// The part of speech.
		/// </summary>
        public string partOfSpeech { get; set; }
		/// <summary>
		/// The valuse.
		/// </summary>
        public string value { get; set; }
		/// <summary>
		/// The lemma.
		/// </summary>
        public string lemma { get; set; }
		/// <summary>
		/// The synsets.
		/// </summary>
        public SynSet[] synSet { get; set; }
    };

	/// <summary>
	/// The evidence request.
	/// </summary>
    [fsObject]
    public class EvidenceRequest
    {
		/// <summary>
		/// The items.
		/// </summary>
        public long items { get; set; }
		/// <summary>
		/// The profile.
		/// </summary>
        public string profile { get; set; }
    };

	/// <summary>
	/// The answer.
	/// </summary>
    [fsObject]
    public class Answer
    {
		/// <summary>
		/// The answer identifier.
		/// </summary>
        public long id { get; set; }
		/// <summary>
		/// The answer text.
		/// </summary>
        public string text { get; set; }
		/// <summary>
		/// The pipeline.
		/// </summary>
        public string pipeline { get; set; }
		/// <summary>
		/// The formatted text.
		/// </summary>
        public string formattedText { get; set; }
		/// <summary>
		/// The confidence.
		/// </summary>
        public double confidence { get; set; }
		/// <summary>
		/// The evidence.
		/// </summary>
        public Evidence[] evidence { get; set; }
		/// <summary>
		/// The entity types.
		/// </summary>
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
		/// Answer cell.
		/// </summary>
        [fsObject]
        public class Cell
        {
			/// <summary>
			/// The cell value.
			/// </summary>
            public string Value { get; set; }
			/// <summary>
			/// The column span.
			/// </summary>
            public int ColSpan { get; set; }
			/// <summary>
			/// Is this cell highlighted?
			/// </summary>
            public bool Highlighted { get; set; }
        };

		/// <summary>
		/// Answer row.
		/// </summary>
        [fsObject]
        public class Row
        {
			/// <summary>
			/// The row columns.
			/// </summary>
            public Cell[] columns { get; set; }
        };

		/// <summary>
		/// Answer table.
		/// </summary>
        [fsObject]
        public class Table
        {
			/// <summary>
			/// The table rows.
			/// </summary>
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

	/// <summary>
	/// The word slots.
	/// </summary>
    [fsObject]
    public class Slots
    {
		/// <summary>
		/// The pred.
		/// </summary>
        public string pred { get; set; }
		/// <summary>
		/// The subj.
		/// </summary>
        public string subj { get; set; }
		/// <summary>
		/// The objprep.
		/// </summary>
        public string objprep { get; set; }
		/// <summary>
		/// The psubj.
		/// </summary>
        public string psubj { get; set; }
    };

	/// <summary>
	/// The word.
	/// </summary>
    [fsObject]
    public class Word
    {
		/// <summary>
		/// The slots.
		/// </summary>
        public Slots compSlotParseNodes { get; set; }
		/// <summary>
		/// The slot name.
		/// </summary>
        public string slotname { get; set; }
		/// <summary>
		/// The word text.
		/// </summary>
        public string wordtext { get; set; }
		/// <summary>
		/// The slot name options.
		/// </summary>
        public string slotnameoptions { get; set; }
		/// <summary>
		/// The word sense.
		/// </summary>
        public string wordsense { get; set; }
		/// <summary>
		/// The numeric sense.
		/// </summary>
        public string numericsense { get; set; }
		/// <summary>
		/// The seqno.
		/// </summary>
        public string seqno { get; set; }
		/// <summary>
		/// The word begin.
		/// </summary>
        public string wordbegin { get; set; }
		/// <summary>
		/// The frame begin.
		/// </summary>
        public string framebegin { get; set; }
		/// <summary>
		/// The frame end.
		/// </summary>
        public string frameend { get; set; }
		/// <summary>
		/// The word end.
		/// </summary>
        public string wordend { get; set; }
		/// <summary>
		/// The features.
		/// </summary>
        public string features { get; set; }
		/// <summary>
		/// The lMods.
		/// </summary>
        public Word[] lmods { get; set; }
		/// <summary>
		/// The rMods.
		/// </summary>
        public Word[] rmods { get; set; }
    };

	/// <summary>
	/// The parse tree.
	/// </summary>
    [fsObject]
    public class ParseTree : Word
    {
		/// <summary>
		/// The parse score.
		/// </summary>
        public string parseScore { get; set; }
    };

	/// <summary>
	/// The question.
	/// </summary>
    [fsObject]
    public class Question
    {
		/// <summary>
		/// The qClassList.
		/// </summary>
        public Value[] qclasslist { get; set; }
		/// <summary>
		/// The focus list.
		/// </summary>
        public Value[] focuslist { get; set; }
		/// <summary>
		/// The lat list.
		/// </summary>
        public Value[] latlist { get; set; }
		/// <summary>
		/// The evidence list.
		/// </summary>
        public Evidence[] evidencelist { get; set; }
		/// <summary>
		/// The synonyms list.
		/// </summary>
        public SynonymList[] synonymList { get; set; }
		/// <summary>
		/// The disambiguated entities.
		/// </summary>
        public string[] disambiguatedEntities { get; set; }
		/// <summary>
		/// The xsgtopparses.
		/// </summary>
        public ParseTree[] xsgtopparses { get; set; }
		/// <summary>
		/// The cas XML.
		/// </summary>
        public string casXml { get; set; }
		/// <summary>
		/// The pipeline id.
		/// </summary>
        public string pipelineid { get; set; }
		/// <summary>
		/// The formatted answer.
		/// </summary>
        public bool formattedAnswer { get; set; }
		/// <summary>
		/// The selected processing components.
		/// </summary>
        public string selectedProcessingComponents { get; set; }
		/// <summary>
		/// The category.
		/// </summary>
        public string category { get; set; }
		/// <summary>
		/// The items.
		/// </summary>
        public long items { get; set; }
		/// <summary>
		/// The status.
		/// </summary>
        public string status { get; set; }
		/// <summary>
		/// The identifier.
		/// </summary>
        public string id { get; set; }
		/// <summary>
		/// The question text.
		/// </summary>
        public string questionText { get; set; }
		/// <summary>
		/// The evidence request.
		/// </summary>
        public EvidenceRequest evidenceRequest { get; set; }
		/// <summary>
		/// The answers.
		/// </summary>
        public Answer[] answers { get; set; }
		/// <summary>
		/// The error notifications.
		/// </summary>
        public string[] errorNotifications { get; set; }
		/// <summary>
		/// The passthru.
		/// </summary>
        public string passthru { get; set; }
		/// <summary>
		/// The question identifier.
		/// </summary>
        public string questionId { get; set; }      // local cache ID
    };

	/// <summary>
	/// The question class.
	/// </summary>
    [fsObject]
    public class QuestionClass
    {
		/// <summary>
		/// Out of domain.
		/// </summary>
        public string out_of_domain { get; set; }
		/// <summary>
		/// The question.
		/// </summary>
        public string question { get; set; }
		/// <summary>
		/// The domain.
		/// </summary>
        public string domain { get; set; }
    };
    /// <summary>
    /// The response object for QA.AskQuestion().
    /// </summary>
    [fsObject]
    public class Response
    {
		/// <summary>
		/// The questions.
		/// </summary>
        public Question question { get; set; }
		/// <summary>
		/// The question classes.
		/// </summary>
        public QuestionClass[] questionClasses { get; set; }
    };

    /// <summary>
    /// A list of responses.
    /// </summary>
    [fsObject]
    public class ResponseList
    {
		/// <summary>
		/// The responses.
		/// </summary>
        public Response[] responses { get; set; }
    };
}
