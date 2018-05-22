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

using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Services.Assistant.v1
{
    /// <summary>
    /// Success callback delegate.
    /// </summary>
    /// <typeparam name="T">Type of the returned object.</typeparam>
    /// <param name="response">The returned object.</param>
    /// <param name="customData">user defined custom data including raw json.</param>
    public delegate void SuccessCallback<T>(T response, Dictionary<string, object> customData);

    /// <summary>
    /// Fail callback delegate.
    /// </summary>
    /// <param name="error">The error object.</param>
    /// <param name="customData">User defined custom data</param>
    public delegate void FailCallback(RESTConnector.Error error, Dictionary<string, object> customData);

    public interface IAssistant
    {
        bool Message(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, MessageRequest request = null, bool? nodesVisitedDetails = null, Dictionary<string, object> customData = null);
        bool CreateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, CreateWorkspace properties = null, Dictionary<string, object> customData = null);

        bool DeleteWorkspace(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, Dictionary<string, object> customData = null);

        bool GetWorkspace(SuccessCallback<WorkspaceExport> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListWorkspaces(SuccessCallback<WorkspaceCollection> successCallback, FailCallback failCallback, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateWorkspace(SuccessCallback<Workspace> successCallback, FailCallback failCallback, string workspaceId, UpdateWorkspace properties = null, bool? append = null, Dictionary<string, object> customData = null);
        bool CreateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, CreateIntent body, Dictionary<string, object> customData = null);

        bool DeleteIntent(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, Dictionary<string, object> customData = null);

        bool GetIntent(SuccessCallback<IntentExport> successCallback, FailCallback failCallback, string workspaceId, string intent, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListIntents(SuccessCallback<IntentCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateIntent(SuccessCallback<Intent> successCallback, FailCallback failCallback, string workspaceId, string intent, UpdateIntent body, Dictionary<string, object> customData = null);
        bool CreateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, CreateExample body, Dictionary<string, object> customData = null);

        bool DeleteExample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, Dictionary<string, object> customData = null);

        bool GetExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListExamples(SuccessCallback<ExampleCollection> successCallback, FailCallback failCallback, string workspaceId, string intent, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateExample(SuccessCallback<Example> successCallback, FailCallback failCallback, string workspaceId, string intent, string text, UpdateExample body, Dictionary<string, object> customData = null);
        bool CreateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, CreateCounterexample body, Dictionary<string, object> customData = null);

        bool DeleteCounterexample(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string text, Dictionary<string, object> customData = null);

        bool GetCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListCounterexamples(SuccessCallback<CounterexampleCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateCounterexample(SuccessCallback<Counterexample> successCallback, FailCallback failCallback, string workspaceId, string text, UpdateCounterexample body, Dictionary<string, object> customData = null);
        bool CreateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, CreateEntity properties, Dictionary<string, object> customData = null);

        bool DeleteEntity(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, Dictionary<string, object> customData = null);

        bool GetEntity(SuccessCallback<EntityExport> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListEntities(SuccessCallback<EntityCollection> successCallback, FailCallback failCallback, string workspaceId, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateEntity(SuccessCallback<Entity> successCallback, FailCallback failCallback, string workspaceId, string entity, UpdateEntity properties, Dictionary<string, object> customData = null);
        bool CreateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, CreateValue properties, Dictionary<string, object> customData = null);

        bool DeleteValue(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, Dictionary<string, object> customData = null);

        bool GetValue(SuccessCallback<ValueExport> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, bool? export = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListValues(SuccessCallback<ValueCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, bool? export = null, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateValue(SuccessCallback<Value> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, UpdateValue properties, Dictionary<string, object> customData = null);
        bool CreateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, CreateSynonym body, Dictionary<string, object> customData = null);

        bool DeleteSynonym(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, Dictionary<string, object> customData = null);

        bool GetSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListSynonyms(SuccessCallback<SynonymCollection> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateSynonym(SuccessCallback<Synonym> successCallback, FailCallback failCallback, string workspaceId, string entity, string value, string synonym, UpdateSynonym body, Dictionary<string, object> customData = null);
        bool CreateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, CreateDialogNode properties, Dictionary<string, object> customData = null);

        bool DeleteDialogNode(SuccessCallback<object> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, Dictionary<string, object> customData = null);

        bool GetDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool ListDialogNodes(SuccessCallback<DialogNodeCollection> successCallback, FailCallback failCallback, string workspaceId, long? pageLimit = null, bool? includeCount = null, string sort = null, string cursor = null, bool? includeAudit = null, Dictionary<string, object> customData = null);

        bool UpdateDialogNode(SuccessCallback<DialogNode> successCallback, FailCallback failCallback, string workspaceId, string dialogNode, UpdateDialogNode properties, Dictionary<string, object> customData = null);
        bool ListAllLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string filter, string sort = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null);

        bool ListLogs(SuccessCallback<LogCollection> successCallback, FailCallback failCallback, string workspaceId, string sort = null, string filter = null, long? pageLimit = null, string cursor = null, Dictionary<string, object> customData = null);

        bool DeleteUserData(SuccessCallback<object> successCallback, FailCallback failCallback, string customerId, Dictionary<string, object> customData = null);
    }
}
