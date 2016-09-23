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

namespace IBM.Watson.DeveloperCloud.Services.Conversation.v1
{
    /// <summary>
    /// The mesage response.
    /// </summary>
    #region MessageResponse
    [fsObject]
    public class MessageResponse
    {
        /// <summary>
        /// The input text.
        /// </summary>
        public InputData input { get; set; }
        /// <summary>
        /// The context objext.
        /// </summary>
        public Context context { get; set; }
        /// <summary>
        /// Terms from the request that are identified as entities.
        /// </summary>
        public EntityResponse[] entities { get; set; }
        /// <summary>
        /// Terms from the request that are identified as intents.
        /// </summary>
        public Intent[] intents { get; set; }
        /// <summary>
        /// Output logs.
        /// </summary>
        public OutputData output { get; set; }
    }

    /// <summary>
    /// The entity object.
    /// </summary>
    [fsObject]
    public class EntityResponse
    {
        /// <summary>
        /// The entity name.
        /// </summary>
        public string entity { get; set; }
        /// <summary>
        /// The entity location.
        /// </summary>
        public int[] location { get; set; }
        /// <summary>
        /// The entity value.
        /// </summary>
        public string value { get; set; }
    }

    /// <summary>
    /// The resultant intent.
    /// </summary>
    [fsObject]
    public class Intent
    {
        /// <summary>
        /// The intent.
        /// </summary>
        public string intent { get; set; }
        /// <summary>
        /// The confidence.
        /// </summary>
        public float confidence { get; set; }
    }

    /// <summary>
    /// The Output data.
    /// </summary>
    [fsObject]
    public class OutputData
    {
        /// <summary>
        /// Log messages.
        /// </summary>
        public LogMessageResponse[] log_messages { get; set; }
        /// <summary>
        /// Output text.
        /// </summary>
        public string[] text { get; set; }
        /// <summary>
        /// The nodes that were visited.
        /// </summary>
        public string[] nodes_visited { get; set; }
    }

    /// <summary>
    /// The log message object.
    /// </summary>
    [fsObject]
    public class LogMessageResponse
    {
        /// <summary>
        /// The log level.
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// The log message.
        /// </summary>
        public string msg { get; set; }
    }
    #endregion

    #region Message Request
    /// <summary>
    /// The user's input, with optional intents, entities, and other properties from the response.
    /// </summary>
    [fsObject]
    public class MessageRequest
    {
		///// <summary>
		///// Default constructor.
		///// </summary>
		//public MessageRequest()
		//{
		//	input = new InputData();
		//	context = new Context();
		//}

        /// <summary>
        /// The input text. 
        /// </summary>
        public InputData input { get; set; }
        /// <summary>
        /// Whether to return more than one intent. Set to true to return all matching intents.
        /// </summary>
        public bool alternate_intents { get; set; }
        /// <summary>
        /// State information about the conversation.
        /// </summary>
        public Context context { get; set; }

        /// <summary>
        /// Creates the input object and sets the InputText.
        /// </summary>
        public string InputText
        {
            get {
				if (input == null)
				{
					input = new InputData();
					return "";
				}
				else
				{
					return input.text;
				}
			}
            set
            {
                if (input == null)
                    input = new InputData();

                input.text = value;
            }
        }

		/// <summary>
		/// Gets and sets the input value and creates the InputData object if it hasn't been created.
		/// </summary>
		public InputData InputData
		{
			get { return input != null ? input : input = new InputData(); }
			set
			{
				if (input == null)
					input = new InputData();

				input = value;
			}
		}

        /// <summary>
        /// Creates the Context object and sets the conversation_id.
        /// </summary>
        public string conversationID
        {
            get { return context != null ? context.conversation_id : null;  }
            set
            {
                if (context == null)
                    context = new Context();

                context.conversation_id = value;
            }
        }

		/// <summary>
		/// Gets and sets the context value and creates the Context object if it hasn't been created.
		/// </summary>
		public Context ContextData
		{
			get { return context != null ? context : context = new Context(); }
			set
			{
				if (context == null)
					context = new Context();

				context = value;
			}
		}
    }
    #endregion

    #region common
    /// <summary>
    /// The input text.
    /// </summary>
    [fsObject]
    public class InputData
    {
        /// <summary>
        /// The user's input.
        /// </summary>
        public string text { get; set; }
    }

    /// <summary>
    /// Information about the conversation.
    /// </summary>
    [fsObject]
    public class Context
    {
		///// <summary>
		///// Default constructor.
		///// </summary>
		//public Context()
		//{
		//	system = new SystemResponse();
		//}
        /// <summary>
        /// The unique identifier of the conversation. 
        /// </summary>
        public string conversation_id { get; set; }
        /// <summary>
        /// Information about the dialog
        /// </summary>
        public SystemResponse system { get; set; }

		/// <summary>
		/// Creates the SystemResponse object and sets it.
		/// </summary>
		public SystemResponse SystemResponse
		{
			get { return system != null ? system : system = new SystemResponse(); }
			set
			{
				if (system == null)
					system = new SystemResponse();

				system = value;
			}
		}
	}

    /// <summary>
    /// Dialog information.
    /// </summary>
    [fsObject]
    public class SystemResponse
    {
        /// <summary>
        /// An array of dialog node IDs that are in focus in the conversation.
        /// </summary>
        public string[] dialog_stack { get; set; }
        /// <summary>
        /// The number of cycles of user input and response in this conversation.
        /// </summary>
        public int dialog_turn_counter { get; set; }
        /// <summary>
        /// The number of inputs in this conversation. This counter might be higher than the dialog_turn_counter counter when multiple inputs are needed before a response can be returned.
        /// </summary>
        public int dialog_request_counter { get; set; }
    }
    #endregion

    #region Version
    /// <summary>
    /// The conversation service version.
    /// </summary>
    public class Version
    {
		/// <summary>
		/// The version.
		/// </summary>
        public const string VERSION = "2016-07-11";
    }
    #endregion
}
