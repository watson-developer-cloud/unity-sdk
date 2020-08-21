/**
* (C) Copyright IBM Corp. 2019, 2020.
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
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Discovery.V2.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.Discovery.V2
{
    public partial class DiscoveryService : BaseService
    {
        private const string serviceId = "discovery";
        private const string defaultServiceUrl = "https://api.us-south.discovery.watson.cloud.ibm.com";

        #region VersionDate
        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
        }
        #endregion

        #region DisableSslVerification
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        /// <summary>
        /// DiscoveryService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public DiscoveryService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// DiscoveryService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public DiscoveryService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of DiscoveryService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// List collections.
        ///
        /// Lists existing collections for the specified project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="ListCollectionsResponse" />ListCollectionsResponse</returns>
        public bool ListCollections(Callback<ListCollectionsResponse> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCollections`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `ListCollections`");

            RequestObject<ListCollectionsResponse> req = new RequestObject<ListCollectionsResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "ListCollections"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCollectionsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCollectionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListCollectionsResponse> response = new DetailedResponse<ListCollectionsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListCollectionsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListCollectionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListCollectionsResponse>)req).Callback != null)
                ((RequestObject<ListCollectionsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a collection.
        ///
        /// Create a new collection in the specified project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="name">The name of the collection.</param>
        /// <param name="description">A description of the collection. (optional)</param>
        /// <param name="language">The language of the collection. (optional, default to en)</param>
        /// <param name="enrichments">An array of enrichments that are applied to this collection. (optional)</param>
        /// <returns><see cref="CollectionDetails" />CollectionDetails</returns>
        public bool CreateCollection(Callback<CollectionDetails> callback, string projectId, string name, string description = null, string language = null, List<CollectionEnrichment> enrichments = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCollection`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `CreateCollection`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateCollection`");

            RequestObject<CollectionDetails> req = new RequestObject<CollectionDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "CreateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (!string.IsNullOrEmpty(language))
                bodyObject["language"] = language;
            if (enrichments != null && enrichments.Count > 0)
                bodyObject["enrichments"] = JToken.FromObject(enrichments);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CollectionDetails> response = new DetailedResponse<CollectionDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CollectionDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CollectionDetails>)req).Callback != null)
                ((RequestObject<CollectionDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get collection.
        ///
        /// Get details about the specified collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="CollectionDetails" />CollectionDetails</returns>
        public bool GetCollection(Callback<CollectionDetails> callback, string projectId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCollection`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetCollection`");

            RequestObject<CollectionDetails> req = new RequestObject<CollectionDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}", projectId, collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CollectionDetails> response = new DetailedResponse<CollectionDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CollectionDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CollectionDetails>)req).Callback != null)
                ((RequestObject<CollectionDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a collection.
        ///
        /// Updates the specified collection's name, description, and enrichments.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="name">The name of the collection. (optional)</param>
        /// <param name="description">A description of the collection. (optional)</param>
        /// <param name="enrichments">An array of enrichments that are applied to this collection. (optional)</param>
        /// <returns><see cref="CollectionDetails" />CollectionDetails</returns>
        public bool UpdateCollection(Callback<CollectionDetails> callback, string projectId, string collectionId, string name = null, string description = null, List<CollectionEnrichment> enrichments = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCollection`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `UpdateCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateCollection`");

            RequestObject<CollectionDetails> req = new RequestObject<CollectionDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "UpdateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            if (enrichments != null && enrichments.Count > 0)
                bodyObject["enrichments"] = JToken.FromObject(enrichments);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}", projectId, collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CollectionDetails> response = new DetailedResponse<CollectionDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CollectionDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CollectionDetails>)req).Callback != null)
                ((RequestObject<CollectionDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a collection.
        ///
        /// Deletes the specified collection from the project. All documents stored in the specified collection and not
        /// shared is also deleted.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteCollection(Callback<object> callback, string projectId, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCollection`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `DeleteCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteCollection`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}", projectId, collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Query a project.
        ///
        /// By using this method, you can construct queries. For details, see the [Discovery
        /// documentation](https://cloud.ibm.com/docs/discovery-data?topic=discovery-data-query-concepts). The default
        /// query parameters are defined by the settings for this project, see the [Discovery
        /// documentation](https://cloud.ibm.com/docs/discovery-data?topic=discovery-data-project-defaults) for an
        /// overview of the standard default settings, and see [the Projects API documentation](#create-project) for
        /// details about how to set custom default query settings.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionIds">A comma-separated list of collection IDs to be queried against.
        /// (optional)</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. Use a query search when you want to find the most
        /// relevant search results. (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. (optional)</param>
        /// <param name="aggregation">An aggregation search that returns an exact answer by combining query search with
        /// filters. Useful for applications to build lists, tables, and time series. For a full list of possible
        /// aggregations, see the Query reference. (optional)</param>
        /// <param name="count">Number of results to return. (optional)</param>
        /// <param name="_return">A list of the fields in the document hierarchy to return. If this parameter not
        /// specified, then all top-level fields are returned. (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. (optional)</param>
        /// <param name="sort">A comma-separated list of fields in the document to sort on. You can optionally specify a
        /// sort direction by prefixing the field with `-` for descending or `+` for ascending. Ascending is the default
        /// sort direction if no prefix is specified. This parameter cannot be used in the same query as the **bias**
        /// parameter. (optional)</param>
        /// <param name="highlight">When `true`, a highlight field is returned for each result which contains the fields
        /// which match the query with `<em></em>` tags around the matching query terms. (optional)</param>
        /// <param name="spellingSuggestions">When `true` and the **natural_language_query** parameter is used, the
        /// **natural_language_query** parameter is spell checked. The most likely correction is returned in the
        /// **suggested_query** field of the response (if one exists). (optional)</param>
        /// <param name="tableResults">Configuration for table retrieval. (optional)</param>
        /// <param name="suggestedRefinements">Configuration for suggested refinements. (optional)</param>
        /// <param name="passages">Configuration for passage retrieval. (optional)</param>
        /// <returns><see cref="QueryResponse" />QueryResponse</returns>
        public bool Query(Callback<QueryResponse> callback, string projectId, List<string> collectionIds = null, string filter = null, string query = null, string naturalLanguageQuery = null, string aggregation = null, long? count = null, List<string> _return = null, long? offset = null, string sort = null, bool? highlight = null, bool? spellingSuggestions = null, QueryLargeTableResults tableResults = null, QueryLargeSuggestedRefinements suggestedRefinements = null, QueryLargePassages passages = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Query`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `Query`");

            RequestObject<QueryResponse> req = new RequestObject<QueryResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "Query"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (collectionIds != null && collectionIds.Count > 0)
                bodyObject["collection_ids"] = JToken.FromObject(collectionIds);
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            if (!string.IsNullOrEmpty(query))
                bodyObject["query"] = query;
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (!string.IsNullOrEmpty(aggregation))
                bodyObject["aggregation"] = aggregation;
            if (count != null)
                bodyObject["count"] = JToken.FromObject(count);
            if (_return != null && _return.Count > 0)
                bodyObject["return"] = JToken.FromObject(_return);
            if (offset != null)
                bodyObject["offset"] = JToken.FromObject(offset);
            if (!string.IsNullOrEmpty(sort))
                bodyObject["sort"] = sort;
            if (highlight != null)
                bodyObject["highlight"] = JToken.FromObject(highlight);
            if (spellingSuggestions != null)
                bodyObject["spelling_suggestions"] = JToken.FromObject(spellingSuggestions);
            if (tableResults != null)
                bodyObject["table_results"] = JToken.FromObject(tableResults);
            if (suggestedRefinements != null)
                bodyObject["suggested_refinements"] = JToken.FromObject(suggestedRefinements);
            if (passages != null)
                bodyObject["passages"] = JToken.FromObject(passages);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnQueryResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/query", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryResponse> response = new DetailedResponse<QueryResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryResponse>)req).Callback != null)
                ((RequestObject<QueryResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get Autocomplete Suggestions.
        ///
        /// Returns completion query suggestions for the specified prefix.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="prefix">The prefix to use for autocompletion. For example, the prefix `Ho` could autocomplete
        /// to `Hot`, `Housing`, or `How do I upgrade`. Possible completions are.</param>
        /// <param name="collectionIds">Comma separated list of the collection IDs. If this parameter is not specified,
        /// all collections in the project are used. (optional)</param>
        /// <param name="field">The field in the result documents that autocompletion suggestions are identified from.
        /// (optional)</param>
        /// <param name="count">The number of autocompletion suggestions to return. (optional)</param>
        /// <returns><see cref="Completions" />Completions</returns>
        public bool GetAutocompletion(Callback<Completions> callback, string projectId, string prefix, List<string> collectionIds = null, string field = null, long? count = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetAutocompletion`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetAutocompletion`");
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("`prefix` is required for `GetAutocompletion`");

            RequestObject<Completions> req = new RequestObject<Completions>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetAutocompletion"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(prefix))
            {
                req.Parameters["prefix"] = prefix;
            }
            if (collectionIds != null && collectionIds.Count > 0)
            {
                req.Parameters["collection_ids"] = string.Join(",", collectionIds.ToArray());
            }
            if (!string.IsNullOrEmpty(field))
            {
                req.Parameters["field"] = field;
            }
            if (count != null)
            {
                req.Parameters["count"] = count;
            }

            req.OnResponse = OnGetAutocompletionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/autocompletion", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetAutocompletionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Completions> response = new DetailedResponse<Completions>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Completions>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetAutocompletionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Completions>)req).Callback != null)
                ((RequestObject<Completions>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Query system notices.
        ///
        /// Queries for notices (errors or warnings) that might have been generated by the system. Notices are generated
        /// when ingesting documents and performing relevance training.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="filter">A cacheable query that excludes documents that don't mention the query content. Filter
        /// searches are better for metadata-type searches and for assessing the concepts in the data set.
        /// (optional)</param>
        /// <param name="query">A query search returns all documents in your data set with full enrichments and full
        /// text, but with the most relevant documents listed first. (optional)</param>
        /// <param name="naturalLanguageQuery">A natural language query that returns relevant documents by utilizing
        /// training data and natural language understanding. (optional)</param>
        /// <param name="count">Number of results to return. The maximum for the **count** and **offset** values
        /// together in any one query is **10000**. (optional)</param>
        /// <param name="offset">The number of query results to skip at the beginning. For example, if the total number
        /// of results that are returned is 10 and the offset is 8, it returns the last two results. The maximum for the
        /// **count** and **offset** values together in any one query is **10000**. (optional)</param>
        /// <returns><see cref="QueryNoticesResponse" />QueryNoticesResponse</returns>
        public bool QueryNotices(Callback<QueryNoticesResponse> callback, string projectId, string filter = null, string query = null, string naturalLanguageQuery = null, long? count = null, long? offset = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `QueryNotices`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `QueryNotices`");

            RequestObject<QueryNoticesResponse> req = new RequestObject<QueryNoticesResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "QueryNotices"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(filter))
            {
                req.Parameters["filter"] = filter;
            }
            if (!string.IsNullOrEmpty(query))
            {
                req.Parameters["query"] = query;
            }
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
            {
                req.Parameters["natural_language_query"] = naturalLanguageQuery;
            }
            if (count != null)
            {
                req.Parameters["count"] = count;
            }
            if (offset != null)
            {
                req.Parameters["offset"] = offset;
            }

            req.OnResponse = OnQueryNoticesResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/notices", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnQueryNoticesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<QueryNoticesResponse> response = new DetailedResponse<QueryNoticesResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<QueryNoticesResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnQueryNoticesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<QueryNoticesResponse>)req).Callback != null)
                ((RequestObject<QueryNoticesResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List fields.
        ///
        /// Gets a list of the unique fields (and their types) stored in the the specified collections.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionIds">Comma separated list of the collection IDs. If this parameter is not specified,
        /// all collections in the project are used. (optional)</param>
        /// <returns><see cref="ListFieldsResponse" />ListFieldsResponse</returns>
        public bool ListFields(Callback<ListFieldsResponse> callback, string projectId, List<string> collectionIds = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListFields`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `ListFields`");

            RequestObject<ListFieldsResponse> req = new RequestObject<ListFieldsResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "ListFields"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (collectionIds != null && collectionIds.Count > 0)
            {
                req.Parameters["collection_ids"] = string.Join(",", collectionIds.ToArray());
            }

            req.OnResponse = OnListFieldsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/fields", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListFieldsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListFieldsResponse> response = new DetailedResponse<ListFieldsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListFieldsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListFieldsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListFieldsResponse>)req).Callback != null)
                ((RequestObject<ListFieldsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List component settings.
        ///
        /// Returns default configuration settings for components.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="ComponentSettingsResponse" />ComponentSettingsResponse</returns>
        public bool GetComponentSettings(Callback<ComponentSettingsResponse> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetComponentSettings`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetComponentSettings`");

            RequestObject<ComponentSettingsResponse> req = new RequestObject<ComponentSettingsResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetComponentSettings"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetComponentSettingsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/component_settings", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetComponentSettingsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ComponentSettingsResponse> response = new DetailedResponse<ComponentSettingsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ComponentSettingsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetComponentSettingsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ComponentSettingsResponse>)req).Callback != null)
                ((RequestObject<ComponentSettingsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add a document.
        ///
        /// Add a document to a collection with optional metadata.
        ///
        ///  Returns immediately after the system has accepted the document for processing.
        ///
        ///   * The user must provide document content, metadata, or both. If the request is missing both document
        /// content and metadata, it is rejected.
        ///
        ///   * The user can set the **Content-Type** parameter on the **file** part to indicate the media type of the
        /// document. If the **Content-Type** parameter is missing or is one of the generic media types (for example,
        /// `application/octet-stream`), then the service attempts to automatically detect the document's media type.
        ///
        ///   * The following field names are reserved and will be filtered out if present after normalization: `id`,
        /// `score`, `highlight`, and any field with the prefix of: `_`, `+`, or `-`
        ///
        ///   * Fields with empty name values after normalization are filtered out before indexing.
        ///
        ///   * Fields containing the following characters after normalization are filtered out before indexing: `#` and
        /// `,`
        ///
        ///   If the document is uploaded to a collection that has it's data shared with another collection, the
        /// **X-Watson-Discovery-Force** header must be set to `true`.
        ///
        ///  **Note:** Documents can be added with a specific **document_id** by using the
        /// **_/v2/projects/{project_id}/collections/{collection_id}/documents** method.
        ///
        /// **Note:** This operation only works on collections created to accept direct file uploads. It cannot be used
        /// to modify a collection that connects to an external source such as Microsoft SharePoint.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size when adding a file
        /// to a collection is 50 megabytes, the maximum supported file size when testing a configuration is 1 megabyte.
        /// Files larger than the supported size are rejected. (optional)</param>
        /// <param name="filename">The filename for file. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="metadata">The maximum supported metadata file size is 1 MB. Metadata parts larger than 1 MB are
        /// rejected.
        ///
        ///
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <param name="xWatsonDiscoveryForce">When `true`, the uploaded document is added to the collection even if
        /// the data for that collection is shared with other collections. (optional, default to false)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        public bool AddDocument(Callback<DocumentAccepted> callback, string projectId, string collectionId, System.IO.MemoryStream file = null, string filename = null, string fileContentType = null, string metadata = null, bool? xWatsonDiscoveryForce = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddDocument`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `AddDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `AddDocument`");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "AddDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata);
            }

            req.OnResponse = OnAddDocumentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}/documents", projectId, collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentAccepted> response = new DetailedResponse<DocumentAccepted>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnAddDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a document.
        ///
        /// Replace an existing document or add a document with a specified **document_id**. Starts ingesting a document
        /// with optional metadata.
        ///
        /// If the document is uploaded to a collection that has it's data shared with another collection, the
        /// **X-Watson-Discovery-Force** header must be set to `true`.
        ///
        /// **Note:** When uploading a new document with this method it automatically replaces any document stored with
        /// the same **document_id** if it exists.
        ///
        /// **Note:** This operation only works on collections created to accept direct file uploads. It cannot be used
        /// to modify a collection that connects to an external source such as Microsoft SharePoint.
        ///
        /// **Note:** If an uploaded document is segmented, all segments will be overwritten, even if the updated
        /// version of the document has fewer segments.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="file">The content of the document to ingest. The maximum supported file size when adding a file
        /// to a collection is 50 megabytes, the maximum supported file size when testing a configuration is 1 megabyte.
        /// Files larger than the supported size are rejected. (optional)</param>
        /// <param name="filename">The filename for file. (optional)</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="metadata">The maximum supported metadata file size is 1 MB. Metadata parts larger than 1 MB are
        /// rejected.
        ///
        ///
        /// Example:  ``` {
        ///   "Creator": "Johnny Appleseed",
        ///   "Subject": "Apples"
        /// } ```. (optional)</param>
        /// <param name="xWatsonDiscoveryForce">When `true`, the uploaded document is added to the collection even if
        /// the data for that collection is shared with other collections. (optional, default to false)</param>
        /// <returns><see cref="DocumentAccepted" />DocumentAccepted</returns>
        public bool UpdateDocument(Callback<DocumentAccepted> callback, string projectId, string collectionId, string documentId, System.IO.MemoryStream file = null, string filename = null, string fileContentType = null, string metadata = null, bool? xWatsonDiscoveryForce = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateDocument`");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `UpdateDocument`");

            RequestObject<DocumentAccepted> req = new RequestObject<DocumentAccepted>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "UpdateDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(metadata))
            {
                req.Forms["metadata"] = new RESTConnector.Form(metadata);
            }

            req.OnResponse = OnUpdateDocumentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}/documents/{2}", projectId, collectionId, documentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentAccepted> response = new DetailedResponse<DocumentAccepted>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentAccepted>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentAccepted>)req).Callback != null)
                ((RequestObject<DocumentAccepted>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a document.
        ///
        /// If the given document ID is invalid, or if the document is not found, then the a success response is
        /// returned (HTTP status code `200`) with the status set to 'deleted'.
        ///
        /// **Note:** This operation only works on collections created to accept direct file uploads. It cannot be used
        /// to modify a collection that connects to an external source such as Microsoft SharePoint.
        ///
        /// **Note:** Segments of an uploaded document cannot be deleted individually. Delete all segments by deleting
        /// using the `parent_document_id` of a segment result.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="collectionId">The ID of the collection.</param>
        /// <param name="documentId">The ID of the document.</param>
        /// <param name="xWatsonDiscoveryForce">When `true`, the uploaded document is added to the collection even if
        /// the data for that collection is shared with other collections. (optional, default to false)</param>
        /// <returns><see cref="DeleteDocumentResponse" />DeleteDocumentResponse</returns>
        public bool DeleteDocument(Callback<DeleteDocumentResponse> callback, string projectId, string collectionId, string documentId, bool? xWatsonDiscoveryForce = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `DeleteDocument`");

            RequestObject<DeleteDocumentResponse> req = new RequestObject<DeleteDocumentResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteDocumentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/collections/{1}/documents/{2}", projectId, collectionId, documentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteDocumentResponse> response = new DetailedResponse<DeleteDocumentResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteDocumentResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteDocumentResponse>)req).Callback != null)
                ((RequestObject<DeleteDocumentResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List training queries.
        ///
        /// List the training queries for the specified project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="TrainingQuerySet" />TrainingQuerySet</returns>
        public bool ListTrainingQueries(Callback<TrainingQuerySet> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListTrainingQueries`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `ListTrainingQueries`");

            RequestObject<TrainingQuerySet> req = new RequestObject<TrainingQuerySet>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "ListTrainingQueries"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListTrainingQueriesResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/training_data/queries", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListTrainingQueriesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuerySet> response = new DetailedResponse<TrainingQuerySet>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuerySet>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListTrainingQueriesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuerySet>)req).Callback != null)
                ((RequestObject<TrainingQuerySet>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete training queries.
        ///
        /// Removes all training queries for the specified project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteTrainingQueries(Callback<object> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteTrainingQueries`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `DeleteTrainingQueries`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteTrainingQueries"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteTrainingQueriesResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/training_data/queries", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteTrainingQueriesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteTrainingQueriesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create training query.
        ///
        /// Add a query to the training data for this project. The query can contain a filter and natural language
        /// query.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="naturalLanguageQuery">The natural text query for the training query.</param>
        /// <param name="examples">Array of training examples.</param>
        /// <param name="filter">The filter used on the collection before the **natural_language_query** is applied.
        /// (optional)</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        public bool CreateTrainingQuery(Callback<TrainingQuery> callback, string projectId, string naturalLanguageQuery, List<TrainingExample> examples, string filter = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateTrainingQuery`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `CreateTrainingQuery`");
            if (string.IsNullOrEmpty(naturalLanguageQuery))
                throw new ArgumentNullException("`naturalLanguageQuery` is required for `CreateTrainingQuery`");
            if (examples == null)
                throw new ArgumentNullException("`examples` is required for `CreateTrainingQuery`");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "CreateTrainingQuery"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (examples != null && examples.Count > 0)
                bodyObject["examples"] = JToken.FromObject(examples);
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateTrainingQueryResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/training_data/queries", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateTrainingQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuery> response = new DetailedResponse<TrainingQuery>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateTrainingQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a training data query.
        ///
        /// Get details for a specific training data query, including the query string and all examples.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        public bool GetTrainingQuery(Callback<TrainingQuery> callback, string projectId, string queryId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTrainingQuery`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetTrainingQuery`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `GetTrainingQuery`");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetTrainingQuery"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetTrainingQueryResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/training_data/queries/{1}", projectId, queryId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetTrainingQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuery> response = new DetailedResponse<TrainingQuery>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetTrainingQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a training query.
        ///
        /// Updates an existing training query and it's examples.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="queryId">The ID of the query used for training.</param>
        /// <param name="naturalLanguageQuery">The natural text query for the training query.</param>
        /// <param name="examples">Array of training examples.</param>
        /// <param name="filter">The filter used on the collection before the **natural_language_query** is applied.
        /// (optional)</param>
        /// <returns><see cref="TrainingQuery" />TrainingQuery</returns>
        public bool UpdateTrainingQuery(Callback<TrainingQuery> callback, string projectId, string queryId, string naturalLanguageQuery, List<TrainingExample> examples, string filter = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateTrainingQuery`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `UpdateTrainingQuery`");
            if (string.IsNullOrEmpty(queryId))
                throw new ArgumentNullException("`queryId` is required for `UpdateTrainingQuery`");
            if (string.IsNullOrEmpty(naturalLanguageQuery))
                throw new ArgumentNullException("`naturalLanguageQuery` is required for `UpdateTrainingQuery`");
            if (examples == null)
                throw new ArgumentNullException("`examples` is required for `UpdateTrainingQuery`");

            RequestObject<TrainingQuery> req = new RequestObject<TrainingQuery>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "UpdateTrainingQuery"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(naturalLanguageQuery))
                bodyObject["natural_language_query"] = naturalLanguageQuery;
            if (examples != null && examples.Count > 0)
                bodyObject["examples"] = JToken.FromObject(examples);
            if (!string.IsNullOrEmpty(filter))
                bodyObject["filter"] = filter;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateTrainingQueryResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/training_data/queries/{1}", projectId, queryId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateTrainingQueryResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingQuery> response = new DetailedResponse<TrainingQuery>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingQuery>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateTrainingQueryResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingQuery>)req).Callback != null)
                ((RequestObject<TrainingQuery>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List Enrichments.
        ///
        /// List the enrichments available to this project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="Enrichments" />Enrichments</returns>
        public bool ListEnrichments(Callback<Enrichments> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListEnrichments`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `ListEnrichments`");

            RequestObject<Enrichments> req = new RequestObject<Enrichments>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "ListEnrichments"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListEnrichmentsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/enrichments", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListEnrichmentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Enrichments> response = new DetailedResponse<Enrichments>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Enrichments>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListEnrichmentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Enrichments>)req).Callback != null)
                ((RequestObject<Enrichments>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create an enrichment.
        ///
        /// Create an enrichment for use with the specified project/.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="enrichment"></param>
        /// <param name="file">The enrichment file to upload. (optional)</param>
        /// <returns><see cref="Enrichment" />Enrichment</returns>
        public bool CreateEnrichment(Callback<Enrichment> callback, string projectId, CreateEnrichment enrichment, System.IO.MemoryStream file = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateEnrichment`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `CreateEnrichment`");
            if (enrichment == null)
                throw new ArgumentNullException("`enrichment` is required for `CreateEnrichment`");

            RequestObject<Enrichment> req = new RequestObject<Enrichment>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "CreateEnrichment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (enrichment != null)
            {
                req.Forms["enrichment"] = new RESTConnector.Form(JsonConvert.SerializeObject(enrichment));
            }
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, "filename", "application/octet-stream");
            }

            req.OnResponse = OnCreateEnrichmentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/enrichments", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateEnrichmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Enrichment> response = new DetailedResponse<Enrichment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Enrichment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateEnrichmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Enrichment>)req).Callback != null)
                ((RequestObject<Enrichment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get enrichment.
        ///
        /// Get details about a specific enrichment.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="enrichmentId">The ID of the enrichment.</param>
        /// <returns><see cref="Enrichment" />Enrichment</returns>
        public bool GetEnrichment(Callback<Enrichment> callback, string projectId, string enrichmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetEnrichment`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetEnrichment`");
            if (string.IsNullOrEmpty(enrichmentId))
                throw new ArgumentNullException("`enrichmentId` is required for `GetEnrichment`");

            RequestObject<Enrichment> req = new RequestObject<Enrichment>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetEnrichment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetEnrichmentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/enrichments/{1}", projectId, enrichmentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetEnrichmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Enrichment> response = new DetailedResponse<Enrichment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Enrichment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetEnrichmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Enrichment>)req).Callback != null)
                ((RequestObject<Enrichment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update an enrichment.
        ///
        /// Updates an existing enrichment's name and description.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="enrichmentId">The ID of the enrichment.</param>
        /// <param name="name">A new name for the enrichment.</param>
        /// <param name="description">A new description for the enrichment. (optional)</param>
        /// <returns><see cref="Enrichment" />Enrichment</returns>
        public bool UpdateEnrichment(Callback<Enrichment> callback, string projectId, string enrichmentId, string name, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateEnrichment`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `UpdateEnrichment`");
            if (string.IsNullOrEmpty(enrichmentId))
                throw new ArgumentNullException("`enrichmentId` is required for `UpdateEnrichment`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `UpdateEnrichment`");

            RequestObject<Enrichment> req = new RequestObject<Enrichment>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "UpdateEnrichment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateEnrichmentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/enrichments/{1}", projectId, enrichmentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateEnrichmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Enrichment> response = new DetailedResponse<Enrichment>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Enrichment>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateEnrichmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Enrichment>)req).Callback != null)
                ((RequestObject<Enrichment>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete an enrichment.
        ///
        /// Deletes an existing enrichment from the specified project.
        ///
        /// **Note:** Only enrichments that have been manually created can be deleted.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="enrichmentId">The ID of the enrichment.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteEnrichment(Callback<object> callback, string projectId, string enrichmentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteEnrichment`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `DeleteEnrichment`");
            if (string.IsNullOrEmpty(enrichmentId))
                throw new ArgumentNullException("`enrichmentId` is required for `DeleteEnrichment`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteEnrichment"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteEnrichmentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}/enrichments/{1}", projectId, enrichmentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteEnrichmentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteEnrichmentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List projects.
        ///
        /// Lists existing projects for this instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="ListProjectsResponse" />ListProjectsResponse</returns>
        public bool ListProjects(Callback<ListProjectsResponse> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListProjects`");

            RequestObject<ListProjectsResponse> req = new RequestObject<ListProjectsResponse>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "ListProjects"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListProjectsResponse;

            Connector.URL = GetServiceUrl() + "/v2/projects";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListProjectsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ListProjectsResponse> response = new DetailedResponse<ListProjectsResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ListProjectsResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnListProjectsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ListProjectsResponse>)req).Callback != null)
                ((RequestObject<ListProjectsResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a Project.
        ///
        /// Create a new project for this instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The human readable name of this project.</param>
        /// <param name="type">The project type of this project.</param>
        /// <param name="defaultQueryParameters">Default query parameters for this project. (optional)</param>
        /// <returns><see cref="ProjectDetails" />ProjectDetails</returns>
        public bool CreateProject(Callback<ProjectDetails> callback, string name, string type, DefaultQueryParams defaultQueryParameters = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateProject`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateProject`");
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException("`type` is required for `CreateProject`");

            RequestObject<ProjectDetails> req = new RequestObject<ProjectDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "CreateProject"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(type))
                bodyObject["type"] = type;
            if (defaultQueryParameters != null)
                bodyObject["default_query_parameters"] = JToken.FromObject(defaultQueryParameters);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateProjectResponse;

            Connector.URL = GetServiceUrl() + "/v2/projects";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateProjectResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ProjectDetails> response = new DetailedResponse<ProjectDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ProjectDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnCreateProjectResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ProjectDetails>)req).Callback != null)
                ((RequestObject<ProjectDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get project.
        ///
        /// Get details on the specified project.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="ProjectDetails" />ProjectDetails</returns>
        public bool GetProject(Callback<ProjectDetails> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetProject`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `GetProject`");

            RequestObject<ProjectDetails> req = new RequestObject<ProjectDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "GetProject"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetProjectResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetProjectResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ProjectDetails> response = new DetailedResponse<ProjectDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ProjectDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnGetProjectResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ProjectDetails>)req).Callback != null)
                ((RequestObject<ProjectDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a project.
        ///
        /// Update the specified project's name.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <param name="name">The new name to give this project. (optional)</param>
        /// <returns><see cref="ProjectDetails" />ProjectDetails</returns>
        public bool UpdateProject(Callback<ProjectDetails> callback, string projectId, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateProject`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `UpdateProject`");

            RequestObject<ProjectDetails> req = new RequestObject<ProjectDetails>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "UpdateProject"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateProjectResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateProjectResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ProjectDetails> response = new DetailedResponse<ProjectDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ProjectDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnUpdateProjectResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ProjectDetails>)req).Callback != null)
                ((RequestObject<ProjectDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a project.
        ///
        /// Deletes the specified project.
        ///
        /// **Important:** Deleting a project deletes everything that is part of the specified project, including all
        /// collections.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="projectId">The ID of the project. This information can be found from the deploy page of the
        /// Discovery administrative tooling.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteProject(Callback<object> callback, string projectId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteProject`");
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentNullException("`projectId` is required for `DeleteProject`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteProject"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteProjectResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v2/projects/{0}", projectId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteProjectResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteProjectResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the **X-Watson-Metadata** header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/discovery-data?topic=discovery-data-information-security#information-security).
        ///
        ///
        /// **Note:** This method is only supported on IBM Cloud instances of Discovery.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("discovery", "V2", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v2/user_data";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("DiscoveryService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}
