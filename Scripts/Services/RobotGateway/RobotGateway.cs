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

using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.RobotGateway.v1
{
    public class RobotGateway : IWatsonService
    {
        #region Types
        public delegate void OnPingCallback( bool success );
        #endregion

        #region Private Data
        private const string SERVICE_ID = "RobotGatewayV1";
        //private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Ping
        public bool Ping( OnPingCallback callback )
        {
            if ( callback == null )
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/webApp/v1/en");
            if (connector == null)
                return false;

            PingReq req = new PingReq();
            req.Callback = callback;
            req.OnResponse = PingResp;

            return connector.Send(req);
        }
        private class PingReq : RESTConnector.Request
        {
            public OnPingCallback Callback { get; set; }
        };
        private void PingResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string response = Encoding.UTF8.GetString(resp.Data);
            if (((PingReq)req).Callback != null)
                ((PingReq)req).Callback(resp.Success);
        }
        #endregion

        #region IWatsonService interface
        /// <exclude />
        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            if ( Config.Instance.FindCredentials( SERVICE_ID ) != null )
                new CheckServiceStatus( this, callback );
            else
                callback( SERVICE_ID, false );
        }

        private class CheckServiceStatus
        {
            private RobotGateway m_Service = null;
            private ServiceStatus m_Callback = null;

            public CheckServiceStatus( RobotGateway service, ServiceStatus callback )
            {
                m_Service = service;
                m_Callback = callback;

                if (! m_Service.Ping( OnPing ) )
                    m_Callback( SERVICE_ID, false );
            }

            private void OnPing( bool success )
            {
                if ( m_Callback != null ) 
                    m_Callback( SERVICE_ID, success );
            }
        };
        #endregion
    }
}
