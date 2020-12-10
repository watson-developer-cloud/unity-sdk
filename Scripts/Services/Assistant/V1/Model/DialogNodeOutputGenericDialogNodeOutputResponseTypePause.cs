/**
* (C) Copyright IBM Corp. 2020.
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

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// An object that describes a response with response type `pause`.
    /// </summary>
    public class DialogNodeOutputGenericDialogNodeOutputResponseTypePause : DialogNodeOutputGeneric
    {
        /// <summary>
        /// The type of response returned by the dialog node. The specified response type must be supported by the
        /// client application or channel.
        /// </summary>
        public class ResponseTypeValue
        {
            /// <summary>
            /// Constant PAUSE for pause
            /// </summary>
            public const string PAUSE = "pause";
            
        }

        /// <summary>
        /// How long to pause, in milliseconds. The valid values are from 0 to 10000.
        /// </summary>
        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public new long? Time
        {
            get { return base.Time; }
            set { base.Time = value; }
        }
        /// <summary>
        /// Whether to send a "user is typing" event during the pause. Ignored if the channel does not support this
        /// event.
        /// </summary>
        [JsonProperty("typing", NullValueHandling = NullValueHandling.Ignore)]
        public new bool? Typing
        {
            get { return base.Typing; }
            set { base.Typing = value; }
        }
    }
}
