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
*/

using System;
using System.Collections.Generic;
using IBM.Watson.Logging;

namespace IBM.Watson.Services
{
    /// <summary>
    ///  This abstract class is what all services use to transparently connect to the server backend. 
    /// </summary>
    abstract class Connector : IDisposable
    {
        #region Public Types
        public delegate void ConnectorEvent( Connector connection );
        public delegate void ResponseEvent( Response resp );

        public enum ConnectionState
        {
            /// <summary>
            /// Connector is connected to the server.
            /// </summary>
            CONNECTED,
            /// <summary>
            /// Connected has lost connection to the server.
            /// </summary>
            DISCONNECTED,
            /// <summary>
            /// Connected has been closed to the server.
            /// </summary>
            CLOSED
        }

        /// <summary>
        /// This object is required to authenticate with the server. If a SessionID is provided
        /// then no user or password is needed. 
        /// </summary>
        public class Credential
        {
            public string SessionID { get; set; }
            public string User { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// The class is returned by a Request object containing the response to a request made
        /// by the client.
        /// </summary>
        public class Response
        {
            public Response()
            {
                Parameters = new Dictionary<string, object>();
            }

            #region Public Properties
            /// <summary>
            /// The return code, anything other than 0 is considered an error response.
            /// </summary>
            public int ReponseCode { get; set; }
            /// <summary>
            /// Data returned back from the server.
            /// </summary>
            public Dictionary<string,object> Parameters { get; set; }
            #endregion
        };

        public enum RequestType
        {
            GET,
            POST
        };

        /// <summary>
        /// This class is created to make a request to send through the IConnector object to the server.
        /// </summary>
        public class Request
        {
            public Request()
            {
                Parameters = new Dictionary<string, object>();
            }

            #region Public Properties
            /// <summary>
            /// The name of the function to invoke on the server.
            /// </summary>
            public string Function { get; set; }
            /// <summary>
            /// The type of request, this is a recommendation to the connector
            /// on how to make the request. This value is not used by the WSConnector.
            /// </summary>
            public RequestType Type { get; set; }
            /// <summary>
            /// The parameters to pass to the function on the server.
            /// </summary>
            public Dictionary<string,object> Parameters { get; set; }
            /// <summary>
            /// The callback that is invoked when a response is received.
            /// </summary>
            public ResponseEvent OnResponse { get; set; }
            #endregion
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// This delegate is invoked when the connection is opened successfully.
        /// </summary>
        public ConnectorEvent OnOpen { get; set; }
        /// <summary>
        /// This delegate is invoked if our connection is lost.
        /// </summary>
        public ConnectorEvent OnDisconnect { get; set; }
        /// <summary>
        /// This delegeta is invoked when the connection is closed gracefully.
        /// </summary>
        public ConnectorEvent OnClose { get; set; }
        /// <summary>
        /// The URI of the connection. If this URI starts with ws:// or wss:// then a
        /// websocket wil be used, if it begins with http:// or https:// then a REST
        /// connection will be used.
        /// </summary>
        public string URI { get; set; }
        /// <summary>
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Credential Authentication { get; set; }
        /// <summary>
        /// The current state of this connector.
        /// </summary>
        public ConnectionState State { get { return m_ConnectionState; } set { m_ConnectionState = value; } }
        #endregion

        #region Abstract Interface
        /// <summary>
        /// Send a request to the server. The request contains a callback that is invoked
        /// when a response is received. The request may be queued if multiple requests are
        /// made at once.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>true is returned on success, false is returned if the Request can't be sent.</returns>
        public abstract bool Send( Request request );
        #endregion

        #region Connector Creation
        /// <summary>
        /// This function creates the correct type of Connector based on the provided URI. THe connector
        /// is initialized with the URI and credentials before being returned. 
        /// </summary>
        /// <param name="uri">The URI to the server.</param>
        /// <param name="credentials">The authentication information to use to connect with the server.</param>
        /// <returns>Returns a Connector object that can be used to send any number of requests to the server.
        /// null is returned if a correct Connector cannot be determined from the URI.</returns>
        public static Connector Create( string uri, Credential credentials )
        {
            Connector connector = null;
            if (uri.StartsWith("ws://", StringComparison.OrdinalIgnoreCase)
                || uri.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
            {
                connector = new WSConnector();
            }
            else if ( uri.StartsWith( "http://", StringComparison.OrdinalIgnoreCase ) 
                || uri.StartsWith( "https://", StringComparison.OrdinalIgnoreCase ) )
            {
                connector = new RESTConnector();
            }

            if ( connector == null )
            {
                Log.Error( "Connector", "Failed to create connector for URI: {0}", uri );
                return null;
            }

            connector.URI = uri;
            connector.Authentication = credentials;
            return connector;
        }
        #endregion

        #region IDisposable Inteface
        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Data
        private ConnectionState     m_ConnectionState = ConnectionState.CLOSED;
        #endregion
    }
}
