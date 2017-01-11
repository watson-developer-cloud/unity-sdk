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
using System;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.Discovery.v1
{
    public class Discovery : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "DiscoveryV1";
        private static fsSerializer sm_Serializer = new fsSerializer();

        private const string SERVICE_ENVIRONMENTS = "/v1/environments";
        private const string SERVICE_ENVIRONMENT = "/v1/environments/{0}";
        private const string SERVICE_ENVIRONMENT_PREVIEW = "/v1/environments/{0}/preview";
        private const string SERVICE_ENVIRONMENT_CONFIGURATIONS = "/v1/environments/{0}/configurations";
        private const string SERVICE_ENVIRONMENT_CONFIGURATION = "/v1/environments/{0}/configurations/{1}";
        private const string SERVICE_ENVIRONMENT_COLLECTIONS = "/v1/environments/{0}/collections";
        private const string SERVICE_ENVIRONMENT_COLLECTION = "/v1/environments/{0}/collections/{1}";
        private const string SERVICE_ENVIRONMENT_COLLECTION_FIELDS = "/v1/environments/{0}/collections/{1}/fields";
        private const string SERVICE_ENVIRONMENT_COLLECTION_DOCUMENTS = "/v1/environments/{0}/collections/{1}/documents";
        private const string SERVICE_ENVIRONMENT_COLLECTION_DOCUMENT = "/v1/environments/{0}/collections/{1}/documents/{2}";
        private const string SERVICE_ENVIRONMENT_COLLECTION_QUERY = "/v1/environments/{0}/collections/{1}/query";
        #endregion

        #region Public Types
        /// <summary>
        /// The delegate for loading a file, used by TrainClassifier().
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        /// <returns>Should return a byte array of the file contents or null of failure.</returns>
        public delegate byte[] LoadFileDelegate(string filename);
        /// <summary>
        /// Set this property to overload the internal file loading of this class.
        /// </summary>
        public LoadFileDelegate LoadFile { get; set; }
        /// <summary>
        /// The delegate for saving a file, used by DownloadDialog().
        /// </summary>
        /// <param name="filename">The filename to save.</param>
        /// <param name="data">The data to save into the file.</param>
        public delegate void SaveFileDelegate(string filename, byte[] data);

        /// <summary>
        /// Set this property to overload the internal file saving for this class.
        /// </summary>
        public SaveFileDelegate SaveFile { get; set; }
        #endregion

        #region GetEnvironments
        public delegate void OnGetEnvironments(GetEnvironmentsResponse resp, string customData);

        public bool GetEnvironments(OnGetEnvironments callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            GetEnvironmentsRequest req = new GetEnvironmentsRequest();
            req.Callback = callback;
            req.Data = customData;
            req.Parameters["version"] = DiscoveryVersion.Version;
            req.OnResponse = OnGetEnvironmentsResponse;

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_ENVIRONMENTS);
            if (connector == null)
                return false;

            return connector.Send(req);
        }

        private class GetEnvironmentsRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetEnvironments Callback { get; set; }
        }

        private void OnGetEnvironmentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetEnvironmentsResponse environmentsData = new GetEnvironmentsResponse();

            if(resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = environmentsData;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch(Exception e)
                {
                    Log.Error("Discovery", "OnGetEnvironmentsResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetEnvironmentsRequest)req).Callback != null)
                ((GetEnvironmentsRequest)req).Callback(resp.Success ? environmentsData : null, ((GetEnvironmentsRequest)req).Data);

        }
        #endregion

        #region IWatsonService Interface
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private Discovery m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(Discovery service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if (!m_Service.GetEnvironments(OnGetEnvironments, "CheckServiceStatus"))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnGetEnvironments(GetEnvironmentsResponse environmentsData, string customData)
            {
                if (m_Callback != null)
                    m_Callback(SERVICE_ID, environmentsData != null);
            }
        }
        #endregion
    }
}
