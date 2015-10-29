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
using System.Threading;

#if UNITY_EDITOR
using System.Net;
using System.Net.Security;
#endif

namespace IBM.Watson.Connection
{
    /// <summary>
    /// REST connector class.
    /// </summary>
    public class RESTConnector
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
        /// multi-part form data class.
        /// </summary>
        public class Form
        {
            public Form( string s )
            {
                IsBinary = false;
                BoxedObject = s;
            }
            public Form( int n )
            {
                IsBinary = false;
                BoxedObject = n;
            }

            public Form( byte [] contents, string fileName = null, string mimeType = null )
            {
                IsBinary = true;
                Contents = contents;
                FileName = fileName;
                MimeType = mimeType;
            }

            public bool IsBinary { get; set; }
            public object BoxedObject { get; set; }
            public byte [] Contents { get; set; }
            public string FileName { get; set; }
            public string MimeType { get; set; }
        };

        /// <summary>
        /// This class is created to make a request to send through the IConnector object to the server.
        /// </summary>
        public class Request
        {
            public Request()
            {
                Parameters = new Dictionary<string, object>();
                Headers = new Dictionary<string, string>();
            }

            #region Public Properties
            /// <summary>
            /// True to send a delete method.
            /// </summary>
            public bool Delete { get; set; }
            /// <summary>
            /// The name of the function to invoke on the server.
            /// </summary>
            public string Function { get; set; }
            /// <summary>
            /// The parameters to pass to the function on the server.
            /// </summary>
            public Dictionary<string, object> Parameters { get; set; }
            /// <summary>
            /// Additional headers to provide in the request.
            /// </summary>
            public Dictionary<string,string> Headers { get; set; }
            /// <summary>
            /// The data to send through the connection. Do not use Forms if set.
            /// </summary>
            public byte [] Send { get; set; }
            /// <summary>
            /// Multi-part form data that needs to be sent. Do not use Send if set.
            /// </summary>
            public Dictionary<string,Form> Forms { get; set; }
            /// <summary>
            /// The callback that is invoked when a response is received.
            /// </summary>
            public ResponseEvent OnResponse { get; set; }
            #endregion
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Base URL for REST requests.
        /// </summary>
        public string URL { get; set; }
         /// <summary>
        /// Credentials used to authenticate with the server.
        /// </summary>
        public Credentials Authentication { get; set; }
        /// <summary>
        /// Additional headers to attach to all requests.
        /// </summary>
        public Dictionary<string,string> Headers { get; set; }
        /// <summary>
        /// Returns true if this connector is going through the gateway.
        /// </summary>
        public bool UsingGateway { get; set; }
        #endregion

        #region Private Data
        //! This dictionary is used to translated from a service ID & function into a service-type 
        //! value which is needed by the gateway. 
        private static Dictionary<string,string> sm_GatewayServiceTypes = new Dictionary<string,string>()
        {
            { "TextToSpeechV1/v1/synthesize", "tts" },
            // TODO: Uncomment once gateway is fixed.
            //{ "SpeechToTextV1/v1/recognize", "stt" },
            { "TranslateV1/v2/translate", "language-translation" },
            //{ "NlcV1/v1/delete", "natural-language-delete" },
        };
        //! Dictionary of connectors by service & function.
        private static Dictionary<string,RESTConnector > sm_Connectors = new Dictionary<string, RESTConnector>();
        #endregion

        public static RESTConnector GetConnector( string serviceID, string function )
        {
            RESTConnector connector = null;

            string connectorID = serviceID + function;
            if ( sm_Connectors.TryGetValue( connectorID, out connector ) )
                return connector;

            Config cfg = Config.Instance;
           
            string serviceType = null;
            if ( cfg.EnableGateway 
                && sm_GatewayServiceTypes.TryGetValue( connectorID, out serviceType ) )
            {
                connector = new RESTConnector();
                connector.UsingGateway = true;
                connector.URL = cfg.GatewayURL + "/v1/en/service";
                connector.Headers = new Dictionary<string, string>();
                connector.Headers["ROBOT_KEY"] = cfg.ProductKey;
                connector.Headers["MAC_ID"] = "UnitySDK";
                connector.Headers["Service-Type" ] = serviceType;

                sm_Connectors[ connectorID ] = connector;
                return connector;
            }

            Config.CredentialInfo cred = cfg.FindCredentials( serviceID );
            if (cred == null)
            {
                Log.Error( "Config", "Failed to find credentials for service {0}.", serviceID );
                return null;
            }

            connector = new RESTConnector();
            connector.UsingGateway = false;
            connector.URL = cred.m_URL + function;
            connector.Authentication = new Credentials( cred.m_User, cred.m_Password );
            sm_Connectors[ connectorID ] = connector;

            return connector;
        }

        public static void FlushConnectors()
        {
            sm_Connectors.Clear();
        }

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
            if ( m_ActiveConnections < Config.Instance.MaxRestConnections )
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
        private void AddHeaders(Dictionary<string, string> headers)
        {
            if ( Authentication != null )
            {
                if ( headers == null )
                    throw new ArgumentNullException("headers");
                headers.Add("Authorization", Authentication.CreateAuthorization() );
            }

            if ( Headers != null )
            {
                foreach( var kp in Headers )
                    headers[ kp.Key ] = kp.Value;
            }
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
                string url = URL;
                if (! string.IsNullOrEmpty( req.Function ) )
                    url += req.Function;

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
                        args.Append("&");                  // append separator

                    args.Append(key + "=" + value);       // append key=value
                }

                if (args != null && args.Length > 0)
                    url += "?" + args.ToString();

                AddHeaders( req.Headers );

                Response resp = new Response();

                float startTime = Time.time;
                if (! req.Delete )
                {
                    WWW www = null;
                    if ( req.Forms != null )
                    {
                        if ( req.Send != null )
                            Log.Warning( "RESTConnector", "Do not use both Send & Form fields in a Request object." );

                        WWWForm form = new WWWForm();
                        foreach( var kp in req.Forms )
                        {
                            if ( kp.Value.IsBinary )
                                form.AddBinaryData( kp.Key, kp.Value.Contents, kp.Value.FileName, kp.Value.MimeType );
                            else if ( kp.Value.BoxedObject is string )
                                form.AddField( kp.Key, (string)kp.Value.BoxedObject );
                            else if ( kp.Value.BoxedObject is int )
                                form.AddField( kp.Key, (int)kp.Value.BoxedObject );
                            else if ( kp.Value.BoxedObject != null )
                                Log.Warning( "RESTCOnnector", "Unsupported form field type {0}", kp.Value.BoxedObject.GetType().ToString() );
                        }
                        foreach( var kp in form.headers )
                            req.Headers[ kp.Key ] = kp.Value;
                
                        www = new WWW( url, form.data, req.Headers );
                    }
                    else if (req.Send == null)
                        www = new WWW( url, null, req.Headers );
                    else 
                        www = new WWW( url, req.Send, req.Headers );

                    // wait for the request to complete.
                    while(! www.isDone )
                    {
                        if ( Time.time > (startTime + Config.Instance.TimeOut) )
                            break;
                        yield return null;
                    }

                    if (! string.IsNullOrEmpty( www.error ) )
                        Log.Warning( "RESTConnector", "WWW.error: {0}, response: {1}", www.error, www.text );

                    // generate the Response object now..
                    if ( www.isDone && www.bytes != null )
                    {
                        resp.Success = true;
                        resp.Data = www.bytes;
                    }
                    else
                    {
                        resp.Success = false;
                        resp.Error = string.Format( "Request Error.\nURL: {0}\nError: {1}\nResponse: {2}",
                            url, string.IsNullOrEmpty( www.error ) ? "Timeout" : www.error, www.text );
                    }

				    www.Dispose();
                }
                else
                {
#if UNITY_EDITOR
                    DeleteRequest deleteReq = new DeleteRequest( url, req.Headers );
                    while(! deleteReq.IsComplete )
                        yield return null;

                    resp.Success = deleteReq.Success;
#else
                    Log.Warning( "RESTConnector", "DELETE method is supported in the editor only." );
#endif
                }

                // provide the time to took to get a response from the server..
                resp.ElapsedTime = Time.time - startTime;

                if ( req.OnResponse != null )
                    req.OnResponse( req, resp );

            }

            // reduce the connection count before we exit..
            m_ActiveConnections -= 1;
            //Log.Debug( "RESTConnector", "ActiveConnections {0}", m_ActiveConnections );
            yield break;
        }

        private class DeleteRequest
        {
            public string URL { get; set; }
            public Dictionary<string,string> Headers { get; set; }
            public bool IsComplete { get; set; }
            public bool Success { get; set; }

            public DeleteRequest( string url, Dictionary<string,string> headers )
            {
                URL = url;
                Headers = new Dictionary<string, string>();
                foreach( var kp in headers )
                    Headers[kp.Key] = kp.Value;           
                
                m_Thread = new Thread( ProcessRequest );
                m_Thread.Start();
            }

            private Thread m_Thread = null;

            private void ProcessRequest()
            {
                WebRequest deleteReq = WebRequest.Create( URL );
                foreach( var kp in Headers )
                    deleteReq.Headers.Add( kp.Key, kp.Value );
                deleteReq.Method = "DELETE";

                HttpWebResponse deleteResp = deleteReq.GetResponse() as HttpWebResponse;
                Success = deleteResp.StatusCode == HttpStatusCode.OK;
                IsComplete = true;
            }
        };

        #endregion
    }
}
