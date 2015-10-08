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
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;
using UnityEngine;
using System.Text;

namespace IBM.Watson.Connection
{
    class RESTConnector
    {
        #region Public Types
        public delegate void ResponseEvent(Request req, Response resp);

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
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Config.CredentialsInfo Authentication { get; set; }
        #endregion

        #region Send Interface
        /// <summary>
        /// Send a request to the server. The request contains a callback that is invoked
        /// when a response is received. The request may be queued if multiple requests are
        /// made at once.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>true is returned on success, false is returned if the Request can't be sent.</returns>
        public bool Send(Request request)
        {
            if ( request == null )
                throw new ArgumentNullException("request");

            m_Requests.Enqueue(request);

            // if we are not already running a co-routine to send the Requests
            // then start one at this point.
            if ( m_ActiveConnections < Config.Instance.MaxConnections )
            {
                // This co-routine will increment m_ActiveConnections then yield back to us so
                // we can return from the Send() as quickly as possible.
                Runnable.Run(ProcessRequestQueue());
            }

            return true;
        }
        #endregion

        #region Private Data
        private int m_ActiveConnections = 0;
        private Queue<Request> m_Requests = new Queue<Request>();
        #endregion

        #region Private Functions
        private string CreateAuthorization()
        {
            return "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(Authentication.m_User + ":" + Authentication.m_Password));
        }

        private void AddAuthorizationHeader(Dictionary<string, string> headers)
        {
            if ( headers == null )
                throw new ArgumentNullException("headers");

            headers.Add("Authorization", CreateAuthorization());
        }

        private IEnumerator ProcessRequestQueue()
        {
            // yield AFTER we increment the connection count, so the Send() function can return immediately
            m_ActiveConnections += 1;
            //Log.Debug( "RESTConnector", "ActiveConnections {0}", m_ActiveConnections );
            yield return null;

            while (m_Requests.Count > 0)
            {
                Request req = m_Requests.Dequeue();
                string url = string.Concat(Authentication.m_URL , req.Function);

                StringBuilder args = null;
                foreach (var kp in req.Parameters)
                {
                    var key = kp.Key;
                    var value = kp.Value;

                    if (value is string)
                        value = WWW.EscapeURL((string)value);             // escape the value
                    else if (value is byte[])
                        value = Convert.ToBase64String((byte[])value);    // convert any byte data into base64 string
                    else
                        Log.Warning( "RESTConnector", "Unsupported parameter value type {0}", value.GetType().Name );

                    if (args == null)
                        args = new StringBuilder();
                    else
                        args.Append("&");                  // append seperator

                    args.Append(key + "=" + value);       // append key=value
                }

                if (args != null && args.Length > 0)
                    url += "?" + args.ToString();

                float startTime = Time.time;

                WWW www = null;
                if (req.Send == null)
                {
                    Dictionary<string,string> headers = new Dictionary<string, string>();
                    AddAuthorizationHeader( headers );

                    www = new WWW( url, null, headers );
                }
                else
                {
                    Dictionary<string,string> headers = new Dictionary<string, string>();
                    AddAuthorizationHeader( headers );

                    string sContentType = req.ContentType;
                    if ( string.IsNullOrEmpty( sContentType ) )
                        sContentType = "application/json";
                    headers["Content-Type"] = sContentType;

                    www = new WWW( url, req.Send, headers );
                }

                // wait for the request to complete.
                while(! www.isDone )
                {
                    if ( Time.time > (startTime + Config.Instance.TimeOut) )
                        break;
                    yield return null;
                }

                // generate the Response object now..
                Response resp = new Response();
                if ( www.isDone && string.IsNullOrEmpty( www.error ) )
                {
                    resp.Success = true;
                    resp.Data = www.bytes;
                }
                else
                {
                    resp.Success = false;
                    resp.Error = string.Format( "Request Error.\nURL: {0}\nError: {1}\nResponse: {2}", url, string.IsNullOrEmpty( www.error ) ? "Timeout" : www.error, www.text );
                }

                // provide the time to took to get a response from the server..
                resp.ElapsedTime = Time.time - startTime;

                if ( req.OnResponse != null )
                    req.OnResponse( req, resp );

				www.Dispose();
            }

            // reduce the connection count before we exit..
            m_ActiveConnections -= 1;
            //Log.Debug( "RESTConnector", "ActiveConnections {0}", m_ActiveConnections );
            yield break;
        }
        #endregion
    }
}
