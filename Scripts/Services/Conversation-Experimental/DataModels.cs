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

namespace IBM.Watson.DeveloperCloud.Services.ConversationExperimental.v1
{
	#region Workspaces
	/// <summary>
	/// Workspaces.
	/// </summary>
	[fsObject]
	public class Workspaces
	{
		/// <summary>
		/// Gets or sets the workspaces.
		/// </summary>
		/// <value>The workspaces.</value>
		public Workspace[] workspaces { get; set; }
	}

	/// <summary>
	/// This data class is contained by Workspaces, it represents a single workspace.
	/// </summary>
	[fsObject]
	public class Workspace
	{
		/// <summary>
		/// Gets or sets the workspace identifier.
		/// </summary>
		/// <value>The ID of the workspace.</value>
		public string workspace_id { get; set; }
		/// <summary>
		/// Gets or sets the date created
		/// </summary>
		/// <value>The date created.</value>
		public string created { get; set; }
		/// <summary>
		/// Gets or sets the description of the workspace
		/// </summary>
		/// <value>The description of the workspace.</value>
		public string description { get; set; }
		/// <summary>
		/// Gets or sets the name of the workspace.
		/// </summary>
		/// <value>The name of the workspace.</value>
		public string name { get; set; }
		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language of the workspace.</value>
		public string language { get; set; }
		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		/// <value>The status.</value>
		public string status { get; set; }
		/// <summary>
		/// Gets or sets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		public object metadata { get; set; }
	}
			
	#endregion

	#region Message
	/// <summary>
	/// Message response.
	/// </summary>
	[fsObject]
	public class MessageResponse
	{
		/// <summary>
		/// Gets or sets the intents.
		/// </summary>
		/// <value>The intents.</value>
		public MessageIntent[] intents { get; set; }
		/// <summary>
		/// Gets or sets the entities.
		/// </summary>
		/// <value>The entities.</value>
		public EntityExample[] entities { get; set; }
		/// <summary>
		/// Gets or sets the output.
		/// </summary>
		/// <value>The output.</value>
		public Output output { get; set; }
		/// <summary>
		/// Gets or sets the context.
		/// </summary>
		/// <value>The context.</value>
		public object context { get; set; }
	}
	#endregion

	#region Intent
	/// <summary>
	/// Intents.
	/// </summary>
	[fsObject]
	public class Intents
	{
		/// <summary>
		/// Gets or sets the intents.
		/// </summary>
		/// <value>The intents.</value>
		public Intent[] intents { get; set; }
	}

	/// <summary>
	/// This data class is contained by Intents. It represents a single intent.
	/// </summary>
	[fsObject]
	public class Intent
	{
		/// <summary>
		/// Gets or sets the intent.
		/// </summary>
		/// <value>The intent.</value>
		public string intent { get; set; }
		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		public string created { get;set; }
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string description { get; set; }
	}

	/// <summary>
	/// Intent Examples.
	/// </summary>
	[fsObject]
	public class Examples
	{
		/// <summary>
		/// Gets or sets the examples.
		/// </summary>
		/// <value>The examples.</value>
		public Example[] examples { get; set; }
	}

	/// <summary>
	/// This class is contained by IntentExamples. It represents a single IntentExample.
	/// </summary>
	[fsObject]
	public class Example
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string text { get; set; }
		/// <summary>
		/// Gets or sets the entities.
		/// </summary>
		/// <value>The entities.</value>
		public EntityExample entities { get; set; }
	}

	/// <summary>
	/// Entity example.
	/// </summary>
	[fsObject]
	public class EntityExample
	{
		/// <summary>
		/// Gets or sets the entity.
		/// </summary>
		/// <value>The entity.</value>
		public string entity { get; set; }
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string value { get; set; }
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>The location.</value>
		public int[] location { get; set; }
	}
	#endregion

	#region Entities
	/// <summary>
	/// Entities.
	/// </summary>
	[fsObject]
	public class Entities
	{
		/// <summary>
		/// Gets or sets the entities.
		/// </summary>
		/// <value>The entities.</value>
		public Entity[] entities { get; set; }
	}

	/// <summary>
	/// This class is contained by Entities. It represents a single entity.
	/// </summary>
	[fsObject]
	public class Entity
	{
		/// <summary>
		/// Gets or sets the entity.
		/// </summary>
		/// <value>The entity.</value>
		public string entity { get; set; }
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string description { get; set; }
		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		public string created { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this is an open list.
		/// </summary>
		/// <value><c>true</c> if open list; otherwise, <c>false</c>.</value>
		public bool open_list { get; set; }
		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		/// <value>The tags.</value>
		public string[] tags { get; set; }
	}

	/// <summary>
	/// Entity Values.
	/// </summary>
	[fsObject]
	public class Values
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string value { get; set; }
		/// <summary>
		/// Gets or sets the synonyms.
		/// </summary>
		/// <value>The synonyms.</value>
		public string[] synonyms { get; set; }
		/// <summary>
		/// Gets or sets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		public object metadata { get; set; }
	}
	#endregion

	#region Dialog Nodes
	/// <summary>
	/// The Dialog nodes.
	/// </summary>
	[fsObject]
	public class DialogNodes
	{
		/// <summary>
		/// Gets or sets the dialog nodes.
		/// </summary>
		/// <value>The dialog nodes.</value>
		public DialogNode[] dialog_nodes { get; set; }
	}

	/// <summary>
	/// This class is contained in DialogNodes. It represents a single DialogNode.
	/// </summary>
	[fsObject]
	public class DialogNode
	{
		/// <summary>
		/// Gets or sets the dialog node.
		/// </summary>
		/// <value>The dialog node.</value>
		public string dialog_node { get; set; }
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public string description { get; set; }
		/// <summary>
		/// Gets or sets the created.
		/// </summary>
		/// <value>The created.</value>
		public string created { get; set; }
		/// <summary>
		/// Gets or sets the conditions.
		/// </summary>
		/// <value>The conditions.</value>
		public string conditions { get; set; }
		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public string parent { get; set; }
		/// <summary>
		/// Gets or sets the previous sibling.
		/// </summary>
		/// <value>The previous sibling.</value>
		public string previous_sibling { get; set; }
		/// <summary>
		/// Gets or sets the output.
		/// </summary>
		/// <value>The output.</value>
		public Output output { get; set; }
		/// <summary>
		/// Gets or sets the context.
		/// </summary>
		/// <value>The context.</value>
		public object context { get; set; }
		/// <summary>
		/// Gets or sets the go to.
		/// </summary>
		/// <value>The go to.</value>
		public GoTo go_to { get; set; }
	}
	#endregion

	#region Common
	/// <summary>
	/// This class is contained by MessageIntents, it represents a single MessageIntent.
	/// </summary>
	[fsObject]
	public class MessageIntent
	{
		/// <summary>
		/// Gets or sets the intent.
		/// </summary>
		/// <value>The intent.</value>
		public string intent { get; set; }
		/// <summary>
		/// Gets or sets the confidence.
		/// </summary>
		/// <value>The confidence.</value>
		public double confidence { get; set; }
	}

	/// <summary>
	/// The output text of the conversation.
	/// </summary>
	[fsObject]
	public class Output
	{
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The output text.</value>
		public string text { get; set; }
	}

	/// <summary>
	/// Go to.
	/// </summary>
	[fsObject]
	public class GoTo
	{
		/// <summary>
		/// Gets or sets the dialog node.
		/// </summary>
		/// <value>The dialog node.</value>
		public string dialog_node { get; set; }
		/// <summary>
		/// Gets or sets the selector.
		/// </summary>
		/// <value>The selector.</value>
		public string selector { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this returns.
		/// </summary>
		/// <value><c>true</c> if m return; otherwise, <c>false</c>.</value>
		public bool m_return { get; set; }
	}
	#endregion

	/// <summary>
	/// The conversation service version.
	/// </summary>
	#region Version
	public class Version
    {
		/// <summary>
		/// The version.
		/// </summary>
        public const string VERSION = "2016-05-19";
    }
    #endregion
}
