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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using System;
using System.Collections.Generic;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Connection
{
    /// <summary>
    ///  This abstract class is what all services use to transparently connect to the server backend. 
    /// </summary>
    abstract class Connector : IDisposable
    {
        #region Public Types
        public delegate void ConnectorEvent(Connector connection);
        public delegate void ResponseEvent(Request req, Response resp);

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
        /// The class is returned by a Request object containing the response to a request made
        /// by the client.
        /// </summary>
        public class Response
        {
            #region Public Properties
            /// <summary>
            /// True if the request was successful.
            /// </summary>
            public bool Success { get; set; }
            /// <summary>
            /// Error message if Success is false.
            /// </summary>
            public string Error { get; set; }
            /// <summary>
            /// The data returned by the request.
            /// </summary>
            public byte[] Data { get; set; }
            /// <summary>
            /// The amount of time in seconds it took to get this response from the server.
            /// </summary>
            public float ElapsedTime { get; set; }
            #endregion
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
            /// The parameters to pass to the function on the server.
            /// </summary>
            public Dictionary<string, object> Parameters { get; set; }
            /// <summary>
            /// The data to send through the connection.
            /// </summary>
            public byte [] Send { get; set; }
            /// <summary>
            /// The type of content to send, the default is "application/json"
            /// </summary>
            public string ContentType { get; set; }
            /// <summary>
            /// The callback that is invoked when a response is received.
            /// </summary>
            public ResponseEvent OnResponse { get; set; }
            #endregion
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// This delegate is invoked when the connection is opened.
        /// </summary>
        public ConnectorEvent OnOpen { get; set; }
        /// <summary>
        /// This delegeta is invoked when the connection is closed.
        /// </summary>
        public ConnectorEvent OnClose { get; set; }
         /// <summary>
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Config.CredentialsInfo Authentication { get; set; }
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
        public abstract bool Send(Request request);
        #endregion

        #region Connector Creation
        /// <summary>
        /// This function creates the correct type of Connector based on the provided URI. THe connector
        /// is initialized with the URI and credentials before being returned. The returned object is ready
        /// for the user to call Send().
        /// </summary>
        /// <param name="uri">The URI to the server.</param>
        /// <param name="credentials">The authentication information to use to connect with the server.</param>
        /// <returns>Returns a Connector object that can be used to send any number of requests to the server.
        /// null is returned if a correct Connector cannot be determined from the URI.</returns>
        public static Connector Create( Config.CredentialsInfo credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials");

            string url = credentials.m_URL;
            
            Connector connector = null;
            if (url.StartsWith("ws://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("wss://", StringComparison.OrdinalIgnoreCase))
            {
                connector = new WSConnector();
            }
            else if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                connector = new RESTConnector();
            }

            if (connector == null)
            {
                Log.Error("Connector", "Failed to create connector for URI: {0}", url);
                return null;
            }

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
        private ConnectionState m_ConnectionState = ConnectionState.CLOSED;
        #endregion
    }
}
