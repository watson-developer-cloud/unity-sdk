/**
* Copyright 2018 IBM Corp. All Rights Reserved.
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
using FullSerializer.Internal;
using System;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    /// <summary>
    /// Object that describes a long query.
    /// </summary>
    [fsObject(Converter = typeof(QueryLargeConverter))]
    public class QueryLarge
    {
        /// <summary>
        /// A cacheable query that excludes documents that don't mention the query content. Filter searches are better
        /// for metadata-type searches and for assessing the concepts in the data set.
        /// </summary>
        [fsProperty("filter")]
        public string Filter { get; set; }
        /// <summary>
        /// A query search returns all documents in your data set with full enrichments and full text, but with the most
        /// relevant documents listed first. Use a query search when you want to find the most relevant search results.
        /// You cannot use **natural_language_query** and **query** at the same time.
        /// </summary>
        [fsProperty("query")]
        public string Query { get; set; }
        /// <summary>
        /// A natural language query that returns relevant documents by utilizing training data and natural language
        /// understanding. You cannot use **natural_language_query** and **query** at the same time.
        /// </summary>
        [fsProperty("natural_language_query")]
        public string NaturalLanguageQuery { get; set; }
        /// <summary>
        /// A passages query that returns the most relevant passages from the results.
        /// </summary>
        [fsProperty("passages")]
        public bool? Passages { get; set; }
        /// <summary>
        /// An aggregation search that returns an exact answer by combining query search with filters. Useful for
        /// applications to build lists, tables, and time series. For a full list of possible aggregations, see the
        /// Query reference.
        /// </summary>
        [fsProperty("aggregation")]
        public string Aggregation { get; set; }
        /// <summary>
        /// Number of results to return.
        /// </summary>
        [fsProperty("count")]
        public long? Count { get; set; }
        /// <summary>
        /// A comma-separated list of the portion of the document hierarchy to return.
        /// </summary>
        [fsProperty("return")]
        public string ReturnFields { get; set; }
        /// <summary>
        /// The number of query results to skip at the beginning. For example, if the total number of results that are
        /// returned is 10 and the offset is 8, it returns the last two results.
        /// </summary>
        [fsProperty("offset")]
        public long? Offset { get; set; }
        /// <summary>
        /// A comma-separated list of fields in the document to sort on. You can optionally specify a sort direction by
        /// prefixing the field with `-` for descending or `+` for ascending. Ascending is the default sort direction if
        /// no prefix is specified. This parameter cannot be used in the same query as the **bias** parameter.
        /// </summary>
        [fsProperty("sort")]
        public string Sort { get; set; }
        /// <summary>
        /// When true, a highlight field is returned for each result which contains the fields which match the query
        /// with `<em></em>` tags around the matching query terms.
        /// </summary>
        [fsProperty("highlight")]
        public bool? Highlight { get; set; }
        /// <summary>
        /// A comma-separated list of fields that passages are drawn from. If this parameter not specified, then all
        /// top-level fields are included.
        /// </summary>
        [fsProperty("passages.fields")]
        public string PassagesFields { get; set; }
        /// <summary>
        /// The maximum number of passages to return. The search returns fewer passages if the requested total is not
        /// found. The default is `10`. The maximum is `100`.
        /// </summary>
        [fsProperty("passages.count")]
        public long? PassagesCount { get; set; }
        /// <summary>
        /// The approximate number of characters that any one passage will have.
        /// </summary>
        [fsProperty("passages.characters")]
        public long? PassagesCharacters { get; set; }
        /// <summary>
        /// When `true` and used with a Watson Discovery News collection, duplicate results (based on the contents of
        /// the **title** field) are removed. Duplicate comparison is limited to the current query only; **offset** is
        /// not considered. This parameter is currently Beta functionality.
        /// </summary>
        [fsProperty("deduplicate")]
        public bool? Deduplicate { get; set; }
        /// <summary>
        /// When specified, duplicate results based on the field specified are removed from the returned results.
        /// Duplicate comparison is limited to the current query only, **offset** is not considered. This parameter is
        /// currently Beta functionality.
        /// </summary>
        [fsProperty("deduplicate.field")]
        public string DeduplicateField { get; set; }
        /// <summary>
        /// A comma-separated list of collection IDs to be queried against. Required when querying multiple collections,
        /// invalid when performing a single collection query.
        /// </summary>
        [fsProperty("collection_ids")]
        public string CollectionIds { get; set; }
        /// <summary>
        /// When `true`, results are returned based on their similarity to the document IDs specified in the
        /// **similar.document_ids** parameter.
        /// </summary>
        [fsProperty("similar")]
        public bool? Similar { get; set; }
        /// <summary>
        /// A comma-separated list of document IDs to find similar documents.
        ///
        /// **Tip:** Include the **natural_language_query** parameter to expand the scope of the document similarity
        /// search with the natural language query. Other query parameters, such as **filter** and **query**, are
        /// subsequently applied and reduce the scope.
        /// </summary>
        [fsProperty("similar.document_ids")]
        public string SimilarDocumentIds { get; set; }
        /// <summary>
        /// A comma-separated list of field names that are used as a basis for comparison to identify similar documents.
        /// If not specified, the entire document is used for comparison.
        /// </summary>
        [fsProperty("similar.fields")]
        public string SimilarFields { get; set; }
        /// <summary>
        /// Field which the returned results will be biased against. The specified field must be either a **date** or
        /// **number** format. When a **date** type field is specified returned results are biased towards field values
        /// closer to the current date. When a **number** type field is specified, returned results are biased towards
        /// higher field values. This parameter cannot be used in the same query as the **sort** parameter.
        /// </summary>
        [fsProperty("bias")]
        public string Bias { get; set; }
    }

    #region Query Large Converter
    public class QueryLargeConverter : fsConverter
    {
        private fsSerializer _serializer = new fsSerializer();

        public override bool CanProcess(Type type)
        {
            return type == typeof(QueryLarge);
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            if (data.IsString == false)
            {
                return fsResult.Fail("Type converter requires a string");
            }
            instance = fsTypeCache.GetType(data.AsString);
            if (instance == null)
            {
                return fsResult.Fail("Unable to find type " + data.AsString);
            }
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            QueryLarge queryLarge = (QueryLarge)instance;
            serialized = null;

            Dictionary<string, fsData> serialization = new Dictionary<string, fsData>();

            fsData tempData = null;

            if(queryLarge.Filter != null)
            {
                _serializer.TrySerialize(queryLarge.Filter, out tempData);
                serialization.Add("filter", tempData);
            }

            if (queryLarge.Query != null)
            {
                _serializer.TrySerialize(queryLarge.Query, out tempData);
                serialization.Add("query", tempData);
            }

            if (queryLarge.NaturalLanguageQuery != null)
            {
                _serializer.TrySerialize(queryLarge.NaturalLanguageQuery, out tempData);
                serialization.Add("natural_language_query", tempData);
            }

            if (queryLarge.Passages != null)
            {
                _serializer.TrySerialize(queryLarge.Passages, out tempData);
                serialization.Add("passages", tempData);
            }

            if (queryLarge.Aggregation != null)
            {
                _serializer.TrySerialize(queryLarge.Aggregation, out tempData);
                serialization.Add("aggregation", tempData);
            }

            if (queryLarge.Count != null)
            {
                _serializer.TrySerialize(queryLarge.Count, out tempData);
                serialization.Add("count", tempData);
            }

            if (queryLarge.ReturnFields != null)
            {
                _serializer.TrySerialize(queryLarge.ReturnFields, out tempData);
                serialization.Add("return", tempData);
            }

            if (queryLarge.Offset != null)
            {
                _serializer.TrySerialize(queryLarge.Offset, out tempData);
                serialization.Add("offset", tempData);
            }

            if (queryLarge.Sort != null)
            {
                _serializer.TrySerialize(queryLarge.Sort, out tempData);
                serialization.Add("sort", tempData);
            }

            if (queryLarge.Highlight != null)
            {
                _serializer.TrySerialize(queryLarge.Highlight, out tempData);
                serialization.Add("highlight", tempData);
            }

            if (queryLarge.PassagesFields != null)
            {
                _serializer.TrySerialize(queryLarge.PassagesFields, out tempData);
                serialization.Add("passages.fields", tempData);
            }

            if (queryLarge.PassagesCount != null)
            {
                _serializer.TrySerialize(queryLarge.PassagesCount, out tempData);
                serialization.Add("passages.count", tempData);
            }

            if (queryLarge.PassagesCharacters != null)
            {
                _serializer.TrySerialize(queryLarge.PassagesCharacters, out tempData);
                serialization.Add("passages.characters", tempData);
            }

            if (queryLarge.Deduplicate != null)
            {
                _serializer.TrySerialize(queryLarge.Deduplicate, out tempData);
                serialization.Add("deduplicate", tempData);
            }

            if (queryLarge.DeduplicateField != null)
            {
                _serializer.TrySerialize(queryLarge.DeduplicateField, out tempData);
                serialization.Add("deduplicate.field", tempData);
            }

            if (queryLarge.CollectionIds != null)
            {
                _serializer.TrySerialize(queryLarge.CollectionIds, out tempData);
                serialization.Add("collection_ids", tempData);
            }

            if (queryLarge.Similar != null)
            {
                _serializer.TrySerialize(queryLarge.Similar, out tempData);
                serialization.Add("similar", tempData);
            }

            if (queryLarge.SimilarDocumentIds != null)
            {
                _serializer.TrySerialize(queryLarge.SimilarDocumentIds, out tempData);
                serialization.Add("similar.document_ids", tempData);
            }

            if (queryLarge.SimilarFields != null)
            {
                _serializer.TrySerialize(queryLarge.SimilarFields, out tempData);
                serialization.Add("similar.fields", tempData);
            }

            if (queryLarge.Bias != null)
            {
                _serializer.TrySerialize(queryLarge.Bias, out tempData);
                serialization.Add("bias", tempData);
            }

            serialized = new fsData(serialization);

            return fsResult.Success;
        }
    }
    #endregion
}
