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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Assistant.V1.Model
{
    /// <summary>
    /// DialogNodeOutputGenericDialogNodeOutputResponseTypeChannelTransfer.
    /// </summary>
    public class DialogNodeOutputGenericDialogNodeOutputResponseTypeChannelTransfer : DialogNodeOutputGeneric
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
        /// The message to display to the user when initiating a channel transfer.
        /// </summary>
        [JsonProperty("message_to_user", NullValueHandling = NullValueHandling.Ignore)]
        public new string MessageToUser
        {
            get { return base.MessageToUser; }
            set { base.MessageToUser = value; }
        }
        /// <summary>
        /// Information used by an integration to transfer the conversation to a different channel.
        /// </summary>
        [JsonProperty("transfer_info", NullValueHandling = NullValueHandling.Ignore)]
        public new ChannelTransferInfo TransferInfo { get; protected set; }
        /// <summary>
        /// An array of objects specifying channels for which the response is intended.
        /// </summary>
        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        public new List<ResponseGenericChannel> Channels
        {
            get { return base.Channels; }
            set { base.Channels = value; }
        }
    }
}
