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

using UnityEngine;
using System.Collections;
using FullSerializer;
using System;
using IBM.Watson.DeveloperCloud.Connection;
using System.Collections.Generic;
using System.Text;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.IO;

namespace IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1
{
    /// <summary>
    /// This class wraps the Document Conversion service.
    /// <a href="http://www.ibm.com/watson/developercloud/document-conversion.html">Document Conversion Service</a>
    /// </summary>
    public class DocumentConversion : IWatsonService
    {
        #region Private Data
        private const string ServiceId = "DocumentConversionV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/document-conversion/api";
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return _credentials; }
            set
            {
                _credentials = value;
                if (!string.IsNullOrEmpty(_credentials.Url))
                {
                    _url = _credentials.Url;
                }
            }
        }
        #endregion

        #region Constructor
        public DocumentConversion(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Document Conversion service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

        #region ConvertDocument
        private const string ConvertDocumentEndpoint = "/v1/convert_document";

        #region Callback delegates
        /// <summary>
        /// Success callback delegate.
        /// </summary>
        /// <typeparam name="T">Type of the returned object.</typeparam>
        /// <param name="response">The returned object.</param>
        /// <param name="customData">user defined custom data including raw json.</param>
        public delegate void SuccessCallback<T>(T response, Dictionary<string, object> customData);
        /// <summary>
        /// Fail callback delegate.
        /// </summary>
        /// <param name="error">The error object.</param>
        /// <param name="customData">User defined custom data</param>
        public delegate void FailCallback(RESTConnector.Error error, Dictionary<string, object> customData);
        #endregion

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
        /// Convert document to use in other Watson services.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="documentPath"></param>
        /// <param name="conversionTarget"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ConvertDocument(SuccessCallback<ConvertedDocument> successCallback, FailCallback failCallback, string documentPath, string conversionTarget, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(documentPath))
                throw new ArgumentNullException("A document path is needed to convert document.");
            if (string.IsNullOrEmpty(conversionTarget))
                throw new ArgumentNullException("A conversion target is needed to convert document.");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ConvertDocumentEndpoint);
            if (connector == null)
                return false;

            ConvertDocumentRequest req = new ConvertDocumentRequest();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.ConversionTarget = conversionTarget;
            req.Parameters["version"] = Version.DocumentConversion;
            req.OnResponse = ConvertDocumentResponse;

            byte[] documentData = null;
            if (documentPath != default(string))
            {
                if (LoadFile != null)
                {
                    documentData = LoadFile(documentPath);
                }
                else
                {
#if !UNITY_WEBPLAYER
                    documentData = File.ReadAllBytes(documentPath);
#endif
                }

                if (documentData == null)
                    Log.Error("DocumentConversion.ConvertDocument()", "Failed to upload {0}!", documentPath);
            }

            if (documentData != null)
            {
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";

                req.Forms = new Dictionary<string, RESTConnector.Form>();
                req.Forms["file"] = new RESTConnector.Form(documentData);
                req.Forms["config"] = new RESTConnector.Form(conversionTarget.ToString());
            }

            return connector.Send(req);
        }

        private class ConvertDocumentRequest : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ConvertedDocument> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
            /// <summary>
            /// The conversion target.
            /// </summary>
            public string ConversionTarget { get; set; }
        };

        private void ConvertDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ConvertedDocument result = new ConvertedDocument();
            fsData data = null;
            fsResult r;
            Dictionary<string, object> customData = ((ConvertDocumentRequest)req).CustomData;

            if (resp.Success)
            {
                if ((req as ConvertDocumentRequest).ConversionTarget == ConversionTarget.Answerunits)
                {
                    try
                    {
                        r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                        if (!r.Succeeded)
                            throw new WatsonException(r.FormattedMessages);

                        object obj = result;
                        r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                        if (!r.Succeeded)
                            throw new WatsonException(r.FormattedMessages);

                        customData.Add("json", data);
                    }
                    catch (Exception e)
                    {
                        Log.Error("DocumentConversion.ConvertDocumentResponse()", "ConvertDocumentResponse Exception: {0}", e.ToString());
                        resp.Success = false;
                    }
                }
                else if ((req as ConvertDocumentRequest).ConversionTarget == ConversionTarget.NormalizedHtml)
                {
#if NETFX_CORE
                    result.htmlContent = System.Text.Encoding.GetEncoding(0).GetString(resp.Data);
#else
                    result.htmlContent = System.Text.Encoding.Default.GetString(resp.Data);
#endif
                    customData.Add("json", data);
                }
                else if ((req as ConvertDocumentRequest).ConversionTarget == ConversionTarget.NormalizedText)
                {
#if NETFX_CORE
                    result.textContent = System.Text.Encoding.GetEncoding(0).GetString(resp.Data);
#else
                    result.textContent = System.Text.Encoding.Default.GetString(resp.Data);
#endif
                    customData.Add("json", data);
                }
            }

            if (resp.Success)
            {
                if (((ConvertDocumentRequest)req).SuccessCallback != null)
                    ((ConvertDocumentRequest)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ConvertDocumentRequest)req).FailCallback != null)
                    ((ConvertDocumentRequest)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region IWatsonService interface
        /// <exclude /> 
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}