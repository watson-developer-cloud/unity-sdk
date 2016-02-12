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

namespace IBM.Watson.DeveloperCloud.Services.Dialog.v1
{
    /// <summary>
    /// This data class is contained by Dialogs, it represents a single dialog available.
    /// </summary>
    [fsObject]
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
    [fsObject]
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
    [fsObject]
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
}
