/**
* (C) Copyright IBM Corp. 2021.
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
    /// ResponseGenericChannel.
    /// </summary>
    public class ResponseGenericChannel
    {
        /// <summary>
        /// A channel for which the response is intended.
        /// </summary>
        public class ChannelValue
        {
            /// <summary>
            /// Constant CHAT for chat
            /// </summary>
            public const string CHAT = "chat";
            /// <summary>
            /// Constant FACEBOOK for facebook
            /// </summary>
            public const string FACEBOOK = "facebook";
            /// <summary>
            /// Constant INTERCOM for intercom
            /// </summary>
            public const string INTERCOM = "intercom";
            /// <summary>
            /// Constant SLACK for slack
            /// </summary>
            public const string SLACK = "slack";
            /// <summary>
            /// Constant TEXT_MESSAGING for text_messaging
            /// </summary>
            public const string TEXT_MESSAGING = "text_messaging";
            /// <summary>
            /// Constant VOICE_TELEPHONY for voice_telephony
            /// </summary>
            public const string VOICE_TELEPHONY = "voice_telephony";
            /// <summary>
            /// Constant WHATSAPP for whatsapp
            /// </summary>
            public const string WHATSAPP = "whatsapp";
            
        }

        /// <summary>
        /// A channel for which the response is intended.
        /// Constants for possible values can be found using ResponseGenericChannel.ChannelValue
        /// </summary>
        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public string Channel { get; set; }
    }
}
