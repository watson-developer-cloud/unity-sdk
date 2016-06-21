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
using System.Text;
using IBM.Watson.DeveloperCloud.Utilities;
using UnityEngine;
using System.Collections;
using System;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v2
{
    public class PersonalityInsights : IWatsonService 
    {
        #region Private Data
        private const string SERVICE_ID = "PersonalityInsights_V2";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Profile
        private const string SERVICE_GET_PROFILE = "/v2/profile";
        public delegate void OnGetProfile(DataModels.Profile profile, string data);

        public bool GetProfile(OnGetProfile callback, string source, 
            string contentType = DataModels.ContentType.TEXT_PLAIN, 
            string contentLanguage = DataModels.Language.ENGLISH, 
            string accept = DataModels.ContentType.APPLICATION_JSON, 
            string acceptLanguage = DataModels.Language.ENGLISH, 
            bool includeRaw = false, 
            bool headers = false, 
            string data = default(string))
        {
            if(callback == null)
                throw new ArgumentNullException("callback");

            if(string.IsNullOrEmpty(source))
                throw new ArgumentNullException("A JSON or Text source is required for GetProfile!");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_PROFILE);
            if(connector == null)
                return false;

            GetProfileRequest req = new GetProfileRequest();
            req.Callback = callback;
            req.OnResponse = GetProfileResponse;

            req.Parameters["include_raw"] = includeRaw.ToString();
            req.Parameters["headers"] = headers.ToString();

            req.Headers["Content-Type"] = contentType;
            req.Headers["Content-Language"] = contentLanguage;
            req.Headers["Accept"] = accept;
            req.Headers["Accept-Language"] = acceptLanguage;

            req.Send = System.Text.Encoding.UTF8.GetBytes(source);

            return connector.Send(req);
        }

        private class GetProfileRequest:RESTConnector.Request
        {
            public string Data { get; set; }
            public OnGetProfile Callback { get; set; }
        }

        private void GetProfileResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DataModels.Profile response = new DataModels.Profile();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = response;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("PersonalityInsights", "GetProfileResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetProfileRequest)req).Callback != null)
                ((GetProfileRequest)req).Callback(resp.Success ? response : null, ((GetProfileRequest)req).Data);
        }
        #endregion

        #region IWatsonService implementation
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if ( Utilities.Config.Instance.FindCredentials( SERVICE_ID ) != null )
                new CheckServiceStatus( this, callback );
            else
                callback( SERVICE_ID, false );
        }

        private class CheckServiceStatus
        {
            private PersonalityInsights m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus(PersonalityInsights service, ServiceStatus callback )
            {
                m_Service = service;
                m_Callback = callback;

                DataModels.ContentListContainer contentListContainer = new DataModels.ContentListContainer();
                DataModels.ContentItem item0 = new DataModels.ContentItem();
                item0.content = "test0";
                DataModels.ContentItem item1 = new DataModels.ContentItem();
                item1.content = "test1";
                contentListContainer.contentItems = new DataModels.ContentItem[] {item0, item1};

                if (!m_Service.GetProfile(OnGetProfile, "hello"))
                    m_Callback(SERVICE_ID, false);
            }

            private void OnGetProfile(DataModels.Profile resp , string data)
            {
                if(m_Callback != null )
                    m_Callback(SERVICE_ID, resp != null);
            }
        };
        #endregion
    }
}