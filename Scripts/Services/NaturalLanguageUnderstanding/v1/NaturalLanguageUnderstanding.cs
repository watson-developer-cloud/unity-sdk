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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageUnderstanding.v1
{
    public class NaturalLanguageUnderstanding : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "NaturalLanguageUnderstandingV1";
        private static fsSerializer sm_Serializer = new fsSerializer();

        private const string SERVICE_ANALYZE = "/v1/analyze";
        private const string SERVICE_MODELS = "/v1/models";
        private const string SERVICE_MODEL = "/v1/models/{0}";
        #endregion

        #region Analyze
        /// <summary>
        /// The callback used by Analyze().
        /// </summary>
        /// <param name="resp">The AnalysisResult response.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void OnAnalyze(AnalysisResults resp, string customData);

        /// <summary>
        /// Creates a new environment. You can only create one environment per service instance.An attempt to create another environment 
        /// will result in an error. The size of the new environment can be controlled by specifying the size parameter.
        /// </summary>
        /// <param name="callback">The OnAnalyze callback.</param>
        /// <param name="parameters">The analyze parameters.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool Analyze(OnAnalyze callback, Parameters parameters, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            AnalyzeRequest req = new AnalyzeRequest();
            req.Callback = callback;
            req._Parameters = parameters;
            req.Data = customData;
            req.OnResponse = OnAnalyzeResponse;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;

            fsData data = null;
            fsResult r = sm_Serializer.TrySerialize(parameters, out data);
            string sendjson = data.ToString();
            req.Send = Encoding.UTF8.GetBytes(sendjson);
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_ANALYZE);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class AnalyzeRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public Parameters _Parameters { get; set; }
            public OnAnalyze Callback { get; set; }
        }

        private void OnAnalyzeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            AnalysisResults analysisResults = new AnalysisResults();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = analysisResults;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnAnalyzeResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((AnalyzeRequest)req).Callback != null)
                ((AnalyzeRequest)req).Callback(resp.Success ? analysisResults : null, ((AnalyzeRequest)req).Data);
        }
        #endregion

        #region Get Models
        /// <summary>
        /// The callback used by GetModels().
        /// </summary>
        /// <param name="resp">The GetModels response.</param>
        /// <param name="customData">Optional data.</param>
        public delegate void OnGetModels(ListModelsResults resp, string customData);

        /// <summary>
        /// Lists available models for Relations and Entities features, including Watson Knowledge Studio 
        /// custom models that you have created and linked to your Natural Language Understanding service.
        /// </summary>
        /// <param name="callback">The OnGetModels callback.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool GetModels(OnGetModels callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetModelsRequest req = new GetModelsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;
            req.OnResponse = OnGetModelsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_MODELS);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetModelsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetModels Callback { get; set; }
        }

        private void OnGetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ListModelsResults modelData = new ListModelsResults();

            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = modelData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Discovery", "OnGetModelssResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetModelsRequest)req).Callback != null)
                ((GetModelsRequest)req).Callback(resp.Success ? modelData : null, ((GetModelsRequest)req).Data);

        }
        #endregion

        #region Delete Model
        /// <summary>
        /// The callback used by DeleteModel().
        /// </summary>
        /// <param name="success">The success of the call.</param>
        /// <param name="customData">Optional custom data.</param>
        public delegate void OnDeleteModel(bool success, string customData);

        /// <summary>
        /// Deletes the specified model.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="modelId">The model identifier.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>True if the call succeeds, false if the call is unsuccessful.</returns>
        public bool DeleteModel(OnDeleteModel callback, string modelId, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("modelId");

            DeleteModelRequest req = new DeleteModelRequest();
            req.Callback = callback;
            req.ModelId = modelId;
            req.Data = customData;
            req.Parameters["version"] = NaturalLanguageUnderstandingVersion.Version;
            req.OnResponse = OnDeleteModelResponse;
            req.Delete = true;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(SERVICE_MODEL, modelId));
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class DeleteModelRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public string ModelId { get; set; }
            public OnDeleteModel Callback { get; set; }
        }

        private void OnDeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteModelRequest)req).Callback != null)
                ((DeleteModelRequest)req).Callback(resp.Success, ((DeleteModelRequest)req).Data);
        }
        #endregion

        #region IWatsonService Interface
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }
        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private NaturalLanguageUnderstanding m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(NaturalLanguageUnderstanding service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetModels(OnGetModels, "CheckServiceStatus"))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnGetModels(ListModelsResults modelsData, string customData)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, modelsData != null);
            }
        }
        #endregion
    }
}
