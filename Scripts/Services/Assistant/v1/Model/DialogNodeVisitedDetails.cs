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

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// DialogNodeVisitedDetails.
    /// </summary>
    [fsObject]
    public class DialogNodeVisitedDetails
    {
        /// <summary>
        /// A dialog node that was triggered during processing of the input message.
        /// </summary>
        /// <value>A dialog node that was triggered during processing of the input message.</value>
        [fsProperty("dialog_node")]
        public string DialogNode { get; set; }
        /// <summary>
        /// The title of the dialog node.
        /// </summary>
        /// <value>The title of the dialog node.</value>
        [fsProperty("title")]
        public string Title { get; set; }
    }

}
