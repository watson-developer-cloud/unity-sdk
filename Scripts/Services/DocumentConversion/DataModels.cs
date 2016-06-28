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

using UnityEngine;
using System.Collections;

namespace IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1
{
    public class DataModels : MonoBehaviour
    {
        /// <summary>
        /// Document Conversion response.
        /// </summary>
        public class ConversionResponse
        {
            /// <summary>
            /// The source document ID.
            /// </summary>
            public string source_document_id { get; set; }
            /// <summary>
            /// The timestamp.
            /// </summary>
            public string timestamp { get; set; }
            /// <summary>
            /// The detected media type.
            /// </summary>
            public string media_type_detected { get; set; }
            /// <summary>
            /// Array of document metadata.
            /// </summary>
            public Metadata[] metadata { get; set; }
            /// <summary>
            /// Answer Units.
            /// </summary>
            public AnswerUnit[] answer_units { get; set; }
            /// <summary>
            /// Warnings.
            /// </summary>
            public Warning[] warnings { get; set; }
        }

        /// <summary>
        /// Metadata for the doucment conversion.
        /// </summary>
        public class Metadata
        {
            /// <summary>
            /// Metadata name.
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// Metadata content.
            /// </summary>
            public string content { get; set; }
        }

        /// <summary>
        /// The units of the broken down document.
        /// </summary>
        public class AnswerUnit
        {
            /// <summary>
            /// The AnswerUnit ID.
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// The AnswerUnit type.
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// The AnswerUnit parent ID.
            /// </summary>
            public string parent_id { get; set; }
            /// <summary>
            /// The AnswerUnit title.
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// The AnswerUnit direction.
            /// </summary>
            public string direction { get; set; }
            /// <summary>
            /// The AnswerUnit content.
            /// </summary>
            public Content[] content { get; set; }
        }

        public class Content
        {
            /// <summary>
            /// The content media type.
            /// </summary>
            public string media_type { get; set; }
            /// <summary>
            /// The content text.
            /// </summary>
            public string text { get; set; }
        }

        /// <summary>
        /// The error response.
        /// </summary>
        public class Warning
        {
            /// <summary>
            /// The warning code.
            /// </summary>
            public string code { get; set; }
            /// <summary>
            /// The warning error text.
            /// </summary>
            public string error { get; set; }
        }

        /// <summary>
        /// Targets for document conversion.
        /// </summary>
        public class ConversionTargets
        {
            public const string ANSWER_UNITS = "answer_units";
            public const string NORMALIZED_HTML = "normalized_html";
            public const string NORMALIZED_TEXT = "normalized_text";
        }

        /// <summary>
        /// The Document Conversion service version.
        /// </summary>
        public class Version
        {
            public const string DOCUMENT_CONVERSION = "2015-12-15";
        }
    }
}
