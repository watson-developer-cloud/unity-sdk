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

// uncomment to enable debugging
//#define ENABLE_DEBUGGING

using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

#if !NETFX_CORE
using System.Net;
using System.Net.Security;
#endif

namespace IBM.Watson.DeveloperCloud.Connection
{
    /// <summary>
    /// REST connector class.
    /// </summary>
    public class RESTConnector
    {
        public const string AUTHENTICATION_TOKEN_AUTHORIZATION_HEADER = "X-Watson-Authorization-Token";
        public const string AUTHENTICATION_AUTHORIZATION_HEADER = "Authorization";
        public const long HTTP_STATUS_OK = 200;
        public const long HTTP_STATUS_CREATED = 201;
        public const long HTTP_STATUS_ACCEPTED = 202;
        public const long HTTP_STATUS_NO_CONTENT = 204;
        #region Public Types
        /// <summary>
        /// This delegate type is declared for a Response handler function.
        /// </summary>
        /// <param name="req">The original request object.</param>
        /// <param name="resp">The response object.</param>
        public delegate void ResponseEvent(Request req, Response resp);

        /// <summary>
        /// This delegate is invoked to provide download progress.
        /// </summary>
        /// <param name="progress"></param>
        public delegate void ProgressEvent(float progress);
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
            public Error Error { get; set; }
            /// <summary>
            /// The data returned by the request.
            /// </summary>
            public byte[] Data { get; set; }
            /// <summary>
            /// The amount of time in seconds it took to get this response from the server.
            /// </summary>
            public float ElapsedTime { get; set; }
            /// <summary>
            /// The http response code from the server
            /// </summary>
            public long HttpResponseCode { get; set; }
            /// <summary>
            /// The response headers
            /// </summary>
            public Dictionary<string, string> Headers { get; set; }
            #endregion
        };

        /// <summary>
        /// Class to encapsulate an error returned from a server request.
        /// </summary>
        public class Error
        {
            /// <summary>
            /// The url that generated the error.
            /// </summary>
            public string URL { get; set; }
            /// <summary>
            /// The error code returned from the server.
            /// </summary>
            public long ErrorCode { get; set; }
            /// <summary>
            /// The error message returned from the server.
            /// </summary>
            public string ErrorMessage { get; set; }
            /// <summary>
            /// The contents of the response from the server.
            /// </summary>
            public string Response { get; set; }
            /// <summary>
            /// Dictionary of headers returned by the request.
            /// </summary>
            public Dictionary<string, string> ResponseHeaders { get; set; }

            public override string ToString()
            {
                return string.Format("URL: {0}, ErrorCode: {1}, Error: {2}, Response: {3}", URL, ErrorCode,
                                     string.IsNullOrEmpty(ErrorMessage) ? "" : ErrorMessage,
                                     string.IsNullOrEmpty(Response) ? "" : Response);
            }
        }

        /// <summary>
        /// Multi-part form data class.
        /// </summary>
        public class Form
        {
            /// <summary>
            /// Make a multi-part form object from a string.
            /// </summary>
            /// <param name="s">The string data.</param>
            public Form(string s)
            {
                IsBinary = false;
                BoxedObject = s;
            }
            /// <summary>
            /// Make a multi-part form object from an int.
            /// </summary>
            /// <param name="n">The int data.</param>
            public Form(int n)
            {
                IsBinary = false;
                BoxedObject = n;
            }

            /// <summary>
            /// Make a multi-part form object from binary data.
            /// </summary>
            /// <param name="contents">The binary data.</param>
            /// <param name="fileName">The filename of the binary data.</param>
            /// <param name="mimeType">The mime type of the data.</param>
            public Form(byte[] contents, string fileName = null, string mimeType = null)
            {
                IsBinary = true;
                Contents = contents;
                FileName = fileName;
                MimeType = mimeType;
            }

            /// <summary>
            /// True if the contained data is binary.
            /// </summary>
            public bool IsBinary { get; set; }
            /// <summary>
            /// The boxed POD data type, only set if IsBinary is false.
            /// </summary>
            public object BoxedObject { get; set; }
            /// <summary>
            /// If IsBinary is true, then this will contain the binary data.
            /// </summary>
            public byte[] Contents { get; set; }
            /// <summary>
            /// The filename of the binary data.
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// The Mime-Type of the binary data.
            /// </summary>
            public string MimeType { get; set; }
        };

        /// <summary>
        /// This class is created to make a request to send to the server.
        /// </summary>
        public class Request
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public Request()
            {
                Parameters = new Dictionary<string, object>();
                Headers = new Dictionary<string, string>();
            }

            #region Public Properties
            /// <summary>
            /// Custom timeout for this Request. This timeout is used if this timeout is larger than the value in the Config class.
            /// </summary>
            public float Timeout { get; set; }
            /// <summary>
            /// If true, then request will be cancelled.
            /// </summary>
            public bool Cancel { get; set; }
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
            public Dictionary<string, string> Headers { get; set; }
            /// <summary>
            /// The data to send through the connection. Do not use Forms if set.
            /// </summary>
            public byte[] Send { get; set; }
            /// <summary>
            /// Multi-part form data that needs to be sent. Do not use Send if set.
            /// </summary>
            public Dictionary<string, Form> Forms { get; set; }
            /// <summary>
            /// The callback that is invoked when a response is received.
            /// </summary>
            public ResponseEvent OnResponse { get; set; }
            /// <summary>
            /// This callback is invoked to provide progress on the WWW download.
            /// </summary>
            public ProgressEvent OnDownloadProgress { get; set; }
            /// <summary>
            /// This callback is invoked to provide upload progress.
            /// </summary>
            public ProgressEvent OnUploadProgress { get; set; }
            #endregion
        }
        #endregion

        #region Public Properties
        private static float _logResponseTime = 3.0f;
        /// <summary>
        /// Specify a time to log to the logging system when a response takes longer than this amount.
        /// </summary>
        public static float LogResponseTime { get { return _logResponseTime; } set { _logResponseTime = value; } }
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
        public Dictionary<string, string> Headers { get; set; }
        #endregion

        /// <summary>
        /// This function returns a RESTConnector object for the given service and function. 
        /// </summary>
        /// <param name="serviceID">The ID of the service.</param>
        /// <param name="function">The name of the function.</param>
        /// <returns>Returns a RESTConnector object or null on error.</returns>
        /// 


        public static RESTConnector GetConnector(Credentials credentials, string function)
        {
            RESTConnector connector = new RESTConnector();
            connector.URL = credentials.Url + function;
            connector.Authentication = credentials;
            if (connector.Authentication.HasIamTokenData())
                connector.Authentication.GetToken();

            return connector;
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
            if (request == null)
                throw new ArgumentNullException("request");

            _requests.Enqueue(request);

            // if we are not already running a co-routine to send the Requests
            // then start one at this point.
            if (_activeConnections < Constants.Config.MaxRestConnections)
            {
                // This co-routine will increment _activeConnections then yield back to us so
                // we can return from the Send() as quickly as possible.
                Runnable.Run(ProcessRequestQueue());
            }

            return true;
        }
        #endregion

        #region Private Data
        private int _activeConnections = 0;
        private Queue<Request> _requests = new Queue<Request>();
        #endregion

        #region Private Functions
        private void AddHeaders(Dictionary<string, string> headers)
        {
            if (Authentication != null)
            {
                if (headers == null)
                    throw new ArgumentNullException("headers");

                if (Authentication.HasWatsonAuthenticationToken())
                {
                    headers.Add(AUTHENTICATION_TOKEN_AUTHORIZATION_HEADER, Authentication.WatsonAuthenticationToken);
                }
                else if (Authentication.HasCredentials())
                {
                    headers.Add(AUTHENTICATION_AUTHORIZATION_HEADER, Authentication.CreateAuthorization());
                }
                else if(Authentication.HasIamTokenData())
                {
                    headers.Add(AUTHENTICATION_AUTHORIZATION_HEADER, string.Format("Bearer {0}", Authentication.IamAccessToken));
                }
            }

            if (Headers != null)
            {
                foreach (var kp in Headers)
                    headers[kp.Key] = kp.Value;
            }

            headers.Add("User-Agent", Constants.String.Version);
        }

        private IEnumerator ProcessRequestQueue()
        {
            // yield AFTER we increment the connection count, so the Send() function can return immediately
            _activeConnections += 1;
#if UNITY_EDITOR
            if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
                yield return null;
#else
                yield return null;
#endif

            while (_requests.Count > 0)
            {
                Request req = _requests.Dequeue();
                if (req.Cancel)
                    continue;
                string url = URL;
                if (!string.IsNullOrEmpty(req.Function))
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
                    else if (value is Int32 || value is Int64 || value is UInt32 || value is UInt64 || value is float || value is bool)
                        value = value.ToString();
                    else if (value != null)
                        Log.Warning("RESTConnector.ProcessRequestQueue()", "Unsupported parameter value type {0}", value.GetType().Name);
                    else
                        Log.Error("RESTConnector.ProcessRequestQueue()", "Parameter {0} value is null", key);

                    if (args == null)
                        args = new StringBuilder();
                    else
                        args.Append("&");                  // append separator

                    args.Append(key + "=" + value);       // append key=value
                }

                if (args != null && args.Length > 0)
                    url += "?" + args.ToString();

                AddHeaders(req.Headers);

                Response resp = new Response();

                DateTime startTime = DateTime.Now;
                if (!req.Delete)
                {
                    WWW www = null;
                    if (req.Forms != null)
                    {
                        if (req.Send != null)
                            Log.Warning("RESTConnector", "Do not use both Send & Form fields in a Request object.");

                        WWWForm form = new WWWForm();
                        try
                        {
                            foreach (var formData in req.Forms)
                            {
                                if (formData.Value.IsBinary)
                                    form.AddBinaryData(formData.Key, formData.Value.Contents, formData.Value.FileName, formData.Value.MimeType);
                                else if (formData.Value.BoxedObject is string)
                                    form.AddField(formData.Key, (string)formData.Value.BoxedObject);
                                else if (formData.Value.BoxedObject is int)
                                    form.AddField(formData.Key, (int)formData.Value.BoxedObject);
                                else if (formData.Value.BoxedObject != null)
                                    Log.Warning("RESTConnector.ProcessRequestQueue()", "Unsupported form field type {0}", formData.Value.BoxedObject.GetType().ToString());
                            }
                            foreach (var headerData in form.headers)
                                req.Headers[headerData.Key] = headerData.Value;
                        }
                        catch (Exception e)
                        {
                            Log.Error("RESTConnector.ProcessRequestQueue()", "Exception when initializing WWWForm: {0}", e.ToString());
                        }
                        www = new WWW(url, form.data, req.Headers);
                    }
                    else if (req.Send == null)
                        www = new WWW(url, null, req.Headers);
                    else
                        www = new WWW(url, req.Send, req.Headers);

#if ENABLE_DEBUGGING
                    Log.Debug("RESTConnector", "URL: {0}", url);
#endif

                    // wait for the request to complete.
                    float timeout = Mathf.Max(Constants.Config.Timeout, req.Timeout);
                    while (!www.isDone)
                    {
                        if (req.Cancel)
                            break;
                        if ((DateTime.Now - startTime).TotalSeconds > timeout)
                            break;
                        if (req.OnUploadProgress != null)
                            req.OnUploadProgress(www.uploadProgress);
                        if (req.OnDownloadProgress != null)
                            req.OnDownloadProgress(www.progress);

#if UNITY_EDITOR
                        if (!UnityEditorInternal.InternalEditorUtility.inBatchMode)
                            yield return null;
#else
                        yield return null;
#endif
                    }

                    if (req.Cancel)
                        continue;

                    bool bError = false;
                    Error error = null;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        long nErrorCode = -1;
                        int nSeperator = www.error.IndexOf(' ');
                        if (nSeperator > 0 && long.TryParse(www.error.Substring(0, nSeperator).Trim(), out nErrorCode))
                        {
                            switch (nErrorCode)
                            {
                                case HTTP_STATUS_OK:
                                case HTTP_STATUS_CREATED:
                                case HTTP_STATUS_ACCEPTED:
                                    bError = false;
                                    break;
                                default:
                                    bError = true;
                                    break;
                            }
                        }
                        else
                        {
                            bError = true;
                        }

                        error = new Error()
                        {
                            URL = url,
                            ErrorCode = resp.HttpResponseCode = nErrorCode,
                            ErrorMessage = www.error,
                            Response = www.text,
                            ResponseHeaders = www.responseHeaders
                        };

                        if (bError)
                            Log.Error("RESTConnector.ProcessRequestQueue()", "URL: {0}, ErrorCode: {1}, Error: {2}, Response: {3}", url, nErrorCode, www.error,
                                string.IsNullOrEmpty(www.text) ? "" : www.text);
                        else
                            Log.Warning("RESTConnector.ProcessRequestQueue()", "URL: {0}, ErrorCode: {1}, Error: {2}, Response: {3}", url, nErrorCode, www.error,
                                string.IsNullOrEmpty(www.text) ? "" : www.text);
                    }
                    if (!www.isDone)
                    {
                        Log.Error("RESTConnector.ProcessRequestQueue()", "Request timed out for URL: {0}", url);
                        bError = true;
                    }
                    /*if (!bError && (www.bytes == null || www.bytes.Length == 0))
                    {
                        Log.Warning("RESTConnector.ProcessRequestQueue()", "No data recevied for URL: {0}", url);
                        bError = true;
                    }*/


                    // generate the Response object now..
                    if (!bError)
                    {
                        resp.Success = true;
                        resp.Data = www.bytes;
                        resp.HttpResponseCode = GetResponseCode(www);
                    }
                    else
                    {
                        resp.Success = false;
                        resp.Error = error;
                    }

                    resp.Headers = www.responseHeaders;

                    resp.ElapsedTime = (float)(DateTime.Now - startTime).TotalSeconds;

                    // if the response is over a threshold, then log with status instead of debug
                    if (resp.ElapsedTime > LogResponseTime)
                        Log.Warning("RESTConnector.ProcessRequestQueue()", "Request {0} completed in {1} seconds.", url, resp.ElapsedTime);

                    if (req.OnResponse != null)
                        req.OnResponse(req, resp);

                    www.Dispose();
                }
                else
                {

#if ENABLE_DEBUGGING
                    Log.Debug("RESTConnector.ProcessRequestQueue90", "Delete Request URL: {0}", url);
#endif

                    float timeout = Mathf.Max(Constants.Config.Timeout, req.Timeout);

                    DeleteRequest deleteReq = new DeleteRequest();
                    Runnable.Run(deleteReq.Send(url, req.Headers));
                    while (!deleteReq.IsComplete)
                    {
                        if (req.Cancel)
                            break;
                        if ((DateTime.Now - startTime).TotalSeconds > timeout)
                            break;
                        yield return null;
                    }

                    if (req.Cancel)
                        continue;

                    resp.Success = deleteReq.Success;
                    resp.Data = deleteReq.Data;
                    resp.Error = deleteReq.Error;
                    resp.HttpResponseCode = deleteReq.HttpResponseCode;
                    resp.ElapsedTime = (float)(DateTime.Now - startTime).TotalSeconds;
                    resp.Headers = deleteReq.ResponseHeaders;
                    if (req.OnResponse != null)
                        req.OnResponse(req, resp);
                }
            }

            // reduce the connection count before we exit..
            _activeConnections -= 1;
            yield break;
        }

        public static int GetResponseCode(WWW request)
        {
            int ret = -1;
            if (request.responseHeaders == null)
            {
                Log.Error("RESTConnector.GetResponseCode()", "no response headers.");
            }
            else
            {
                if (!request.responseHeaders.ContainsKey("STATUS"))
                {
                    Log.Error("RESTConnector.GetResponseCode()", "response headers has no STATUS.");
                }
                else
                {
                    ret = ParseResponseCode(request.responseHeaders["STATUS"]);
                }
            }

            return ret;
        }

        public static int ParseResponseCode(string statusLine)
        {
            int ret = -1;

            string[] components = statusLine.Split(' ');
            if (components.Length < 3)
            {
                Log.Error("RESTConnector.ParseResponseCode()", "invalid response status: " + statusLine);
            }
            else
            {
                if (!int.TryParse(components[1], out ret))
                {
                    Log.Error("RESTConnector.ParseResponseCode()", "invalid response code: " + components[1]);
                }
            }

            return ret;
        }

        private class DeleteRequest
        {
            public string URL { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public bool IsComplete { get; set; }
            public bool Success { get; set; }
            public long HttpResponseCode { get; set; }
            public byte[] Data { get; set; }
            public Error Error { get; set; }
            public Dictionary<string, string> ResponseHeaders { get; set; }

            public IEnumerator Send(string url, Dictionary<string, string> headers)
            {
#if ENABLE_DEBUGGING
                Log.Debug("DeleteRequest.Send()", "DeleteRequest, Send: {0}", url);
#endif

                URL = url;
                Headers = new Dictionary<string, string>();
                foreach (var kp in headers)
                {
                    if (kp.Key != "User-Agent")
                        Headers[kp.Key] = kp.Value;
                }

#if !NETFX_CORE
                // This fixes the exception thrown by self-signed certificates.
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
#endif

#if ENABLE_DEBUGGING
                Log.Debug("DeleteRequest.Send()", "DeleteRequest, ProcessRequest {0}", URL);
#endif
                UnityWebRequest deleteReq = UnityWebRequest.Delete(URL);
                deleteReq.method = UnityWebRequest.kHttpVerbDELETE;
                deleteReq.downloadHandler = new DownloadHandlerBuffer();

                foreach (var kp in Headers)
                    deleteReq.SetRequestHeader(kp.Key, kp.Value);
#if UNITY_2017_2_OR_NEWER
                yield return deleteReq.SendWebRequest();
#else
                yield return deleteReq.Send();
#endif
                Error error = null;
                if (!string.IsNullOrEmpty(deleteReq.error))
                {
                    error = new Error()
                    {
                        URL = url,
                        ErrorCode = deleteReq.responseCode,
                        ErrorMessage = deleteReq.error,
                        Response = deleteReq.downloadHandler.text,
                        ResponseHeaders = deleteReq.GetResponseHeaders()
                    };
                }

                Success = deleteReq.responseCode == HTTP_STATUS_OK || deleteReq.responseCode == HTTP_STATUS_OK || deleteReq.responseCode == HTTP_STATUS_NO_CONTENT || deleteReq.responseCode == HTTP_STATUS_ACCEPTED;
                HttpResponseCode = deleteReq.responseCode;
                ResponseHeaders = deleteReq.GetResponseHeaders();
                Data = deleteReq.downloadHandler.data;
                Error = error;
                IsComplete = true;
            }
        };
        #endregion
    }
}
