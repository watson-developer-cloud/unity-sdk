/**
* (C) Copyright IBM Corp. 2020, 2021.
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// RuntimeResponseGenericRuntimeResponseTypeSuggestion.
    /// </summary>
    public class RuntimeResponseGenericRuntimeResponseTypeSuggestion : RuntimeResponseGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        [JsonProperty("response_type", NullValueHandling = NullValueHandling.Ignore)]
        public new string ResponseType
        {
            get { return base.ResponseType; }
            set { base.ResponseType = value; }
        }
        /// <summary>
        /// The title or introductory text to show before the response.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public new string Title
        {
            get { return base.Title; }
            set { base.Title = value; }
        }
        /// <summary>
        /// An array of objects describing the possible matching dialog nodes from which the user can choose.
        /// </summary>
        [JsonProperty("suggestions", NullValueHandling = NullValueHandling.Ignore)]
        public new List<DialogSuggestion> Suggestions
        {
            get { return base.Suggestions; }
            set { base.Suggestions = value; }
        }
        /// <summary>
        /// An array of objects specifying channels for which the response is intended. If **channels** is present, the
        /// response is intended for a built-in integration and should not be handled by an API client.
        /// </summary>
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        public new List<ResponseGenericChannel> Channels
        {
            get { return base.Channels; }
            set { base.Channels = value; }
        }
    }
}
