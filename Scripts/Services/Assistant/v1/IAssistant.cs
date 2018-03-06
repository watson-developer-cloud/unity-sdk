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

using System.Collections.Generic;
using static IBM.Watson.DeveloperCloud.Services.Assistant.v1.Assistant;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    public interface IAssistant
    {
        /// <summary>
        /// Create workspace. Create a workspace based on component objects. You must provide workspace components defining the content of the new workspace.
        /// </summary>
        /// <param name="properties">Valid data defining the content of the new workspace. (optional)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        bool CreateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, CreateWorkspace properties = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete workspace. Delete a workspace from the service instance.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteWorkspace(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get information about a workspace. Get information about a workspace, optionally including all workspace content.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceExport" />WorkspaceExport</returns>
        bool GetWorkspace(SuccessCallback<WorkspaceExport> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List workspaces. List the workspaces associated with an Assistant service instance.
        /// </summary>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="WorkspaceCollection" />WorkspaceCollection</returns>
        bool ListWorkspaces(SuccessCallback<WorkspaceCollection> successCallback, FailCallback failCallback, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update workspace. Update an existing workspace with new or modified data. You must provide component objects defining the content of the updated workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="properties">Valid data defining the new workspace content. Any elements included in the new data will completely replace the existing elements, including all subelements. Previously existing subelements are not retained unless they are included in the new data. (optional)</param>
        /// <param name="append">Specifies that the elements included in the request body are to be appended to the existing data in the workspace. The default value is `false`. (optional, default to false)</param>
        /// <returns><see cref="Workspace" />Workspace</returns>
        bool UpdateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, string workspaceId, UpdateWorkspace properties = null, bool? append = null, Dictionary<string, object> customData = null);
        /// <summary>
        /// Get a response to a user's input. 
        /// </summary>
        /// <param name="workspaceId">Unique identifier of the workspace.</param>
        /// <param name="request">The user's input, with optional intents, entities, and other properties from the response. (optional)</param>
        /// <param name="nodesVisitedDetails">Whether to include additional diagnostic information about the dialog nodes that were visited during processing of the message. (optional, default to false)</param>
        /// <returns><see cref="MessageResponse" />MessageResponse</returns>
        bool Message(SuccessCallback<MessageResponse> successCallback, FailCallback failCallback, string workspaceId, MessageRequest request = null, bool? nodesVisitedDetails = null, Dictionary<string, object> customData = null);
        /// <summary>
        /// Create intent. Create a new intent.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="body">A CreateIntent object defining the content of the new intent.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        bool CreateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, CreateIntent body, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete intent. Delete an intent from a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteIntent(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get intent. Get information about an intent, optionally including all intent content.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="IntentExport" />IntentExport</returns>
        bool GetIntent(SuccessCallback<IntentExport> successCallback, FailCallback failCallback, string workspaceId, string intent, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List intents. List the intents for a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="IntentCollection" />IntentCollection</returns>
        bool ListIntents(SuccessCallback<IntentCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update intent. Update an existing intent with new or modified data. You must provide data defining the content of the updated intent.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="body">An UpdateIntent object defining the updated content of the intent.</param>
        /// <returns><see cref="Intent" />Intent</returns>
        bool UpdateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, string intent, UpdateIntent body, Dictionary<string, object> customData = null);
        /// <summary>
        /// Create user input example. Add a new user input example to an intent.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="body">A CreateExample object defining the content of the new user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        bool CreateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, CreateExample body, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete user input example. Delete a user input example from an intent.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="text">The text of the user input example.</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteExample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get user input example. Get information about a user input example.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Example" />Example</returns>
        bool GetExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List user input examples. List the user input examples for an intent.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ExampleCollection" />ExampleCollection</returns>
        bool ListExamples(SuccessCallback<ExampleCollection> successCallback, FailCallback failCallback, string workspaceId, string intent, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update user input example. Update the text of a user input example.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="intent">The intent name (for example, `pizza_order`).</param>
        /// <param name="text">The text of the user input example.</param>
        /// <param name="body">An UpdateExample object defining the new text for the user input example.</param>
        /// <returns><see cref="Example" />Example</returns>
        bool UpdateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, UpdateExample body, Dictionary<string, object> customData = null);
        /// <summary>
        /// Create entity. Create a new entity.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="properties">A CreateEntity object defining the content of the new entity.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        bool CreateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, CreateEntity properties, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete entity. Delete an entity from a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteEntity(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get entity. Get information about an entity, optionally including all entity content.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="EntityExport" />EntityExport</returns>
        bool GetEntity(SuccessCallback<EntityExport> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List entities. List the entities for a workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="EntityCollection" />EntityCollection</returns>
        bool ListEntities(SuccessCallback<EntityCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update entity. Update an existing entity with new or modified data.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">An UpdateEntity object defining the updated content of the entity.</param>
        /// <returns><see cref="Entity" />Entity</returns>
        bool UpdateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, string entity, UpdateEntity properties, Dictionary<string, object> customData = null);
        /// <summary>
        /// Add entity value. Create a new value for an entity.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="properties">A CreateValue object defining the content of the new value for the entity.</param>
        /// <returns><see cref="Value" />Value</returns>
        bool CreateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, CreateValue properties, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete entity value. Delete a value for an entity.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteValue(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get entity value. Get information about an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ValueExport" />ValueExport</returns>
        bool GetValue(SuccessCallback<ValueExport> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List entity values. List the values for an entity.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="export">Whether to include all element content in the returned data. If export=`false`, the returned data includes only information about the element itself. If export=`true`, all content, including subelements, is included. The default value is `false`. (optional, default to false)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="ValueCollection" />ValueCollection</returns>
        bool ListValues(SuccessCallback<ValueCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update entity value. Update the content of a value for an entity.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="properties">An UpdateValue object defining the new content for value for the entity.</param>
        /// <returns><see cref="Value" />Value</returns>
        bool UpdateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, UpdateValue properties, Dictionary<string, object> customData = null);
        /// <summary>
        /// Add entity value synonym. Add a new synonym to an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="body">A CreateSynonym object defining the new synonym for the entity value.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        bool CreateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, CreateSynonym body, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete entity value synonym. Delete a synonym for an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteSynonym(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get entity value synonym. Get information about a synonym for an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        bool GetSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List entity value synonyms. List the synonyms for an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="SynonymCollection" />SynonymCollection</returns>
        bool ListSynonyms(SuccessCallback<SynonymCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update entity value synonym. Update the information about a synonym for an entity value.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="entity">The name of the entity.</param>
        /// <param name="value">The text of the entity value.</param>
        /// <param name="synonym">The text of the synonym.</param>
        /// <param name="body">An UpdateSynonym object defining the new information for an entity value synonym.</param>
        /// <returns><see cref="Synonym" />Synonym</returns>
        bool UpdateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, UpdateSynonym body, Dictionary<string, object> customData = null);
        /// <summary>
        /// Create dialog node. Create a dialog node.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="properties">A CreateDialogNode object defining the content of the new dialog node.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        bool CreateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, CreateDialogNode properties, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete dialog node. Delete a dialog node from the workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteDialogNode(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get dialog node. Get information about a dialog node.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        bool GetDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List dialog nodes. List the dialog nodes in the workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="DialogNodeCollection" />DialogNodeCollection</returns>
        bool ListDialogNodes(SuccessCallback<DialogNodeCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update dialog node. Update information for a dialog node.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="dialogNode">The dialog node ID (for example, `get_order`).</param>
        /// <param name="properties">An UpdateDialogNode object defining the new contents of the dialog node.</param>
        /// <returns><see cref="DialogNode" />DialogNode</returns>
        bool UpdateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, UpdateDialogNode properties, Dictionary<string, object> customData = null);
        /// <summary>
        /// List log events in all workspaces. List log events in all workspaces in the service instance.
        /// </summary>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter. You must specify a filter query that includes a value for `language`, as well as a value for `workspace_id` or `request.context.metadata.deployment`. For more information, see the [documentation](https://console.bluemix.net/docs/services/conversation/filter-reference.html#filter-query-syntax).</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        bool ListAllLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string filter, string sort = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List log events in a workspace. List log events in a specific workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="filter">A cacheable parameter that limits the results to those matching the specified filter. For more information, see the [documentation](https://console.bluemix.net/docs/services/conversation/filter-reference.html#filter-query-syntax). (optional)</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <returns><see cref="LogCollection" />LogCollection</returns>
        bool ListLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string workspaceId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null);
        /// <summary>
        /// Create counterexample. Add a new counterexample to a workspace. Counterexamples are examples that have been marked as irrelevant input.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="body">An object defining the content of the new user input counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        bool CreateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, CreateCounterexample body, Dictionary<string, object> customData = null);

        /// <summary>
        /// Delete counterexample. Delete a counterexample from a workspace. Counterexamples are examples that have been marked as irrelevant input.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <returns><see cref="object" />object</returns>
        bool DeleteCounterexample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string text, Dictionary<string, object> customData = null);

        /// <summary>
        /// Get counterexample. Get information about a counterexample. Counterexamples are examples that have been marked as irrelevant input.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        bool GetCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// List counterexamples. List the counterexamples for a workspace. Counterexamples are examples that have been marked as irrelevant input.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="pageLimit">The number of records to return in each page of results. The default page limit is 100. (optional)</param>
        /// <param name="includeCount">Whether to include information about the number of records returned. (optional, default to false)</param>
        /// <param name="sort">Sorts the response according to the value of the specified property, in ascending or descending order. (optional)</param>
        /// <param name="cursor">A token identifying the last value from the previous page of results. (optional)</param>
        /// <param name="includeAudit">Whether to include the audit properties (`created` and `updated` timestamps) in the response. (optional, default to false)</param>
        /// <returns><see cref="CounterexampleCollection" />CounterexampleCollection</returns>
        bool ListCounterexamples(SuccessCallback<CounterexampleCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        /// <summary>
        /// Update counterexample. Update the text of a counterexample. Counterexamples are examples that have been marked as irrelevant input.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="text">The text of a user input counterexample (for example, `What are you wearing?`).</param>
        /// <param name="body">An object defining the new text for the counterexample.</param>
        /// <returns><see cref="Counterexample" />Counterexample</returns>
        bool UpdateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, UpdateCounterexample body, Dictionary<string, object> customData = null);
    }
}
