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
    public class DocumentConversion : IWatsonService
    {
        #region Private Data
        private const string SERVICE_ID = "DocumentConversionV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region ConvertDocument
        private const string FUNCTION_CONVERT_DOCUMENT = "/v1/convert_document";
        public delegate void OnConvertDocument(DocumentConversionResponse resp, string data);
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

        public bool ConvertDocument(OnConvertDocument callback, string documentPath, string conversionTarget = ConversionTargets.ANSWER_UNITS, string data = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(documentPath))
                throw new ArgumentNullException("A document path is needed to convert document.");
            if (string.IsNullOrEmpty(conversionTarget))
                throw new ArgumentNullException("A conversion target is needed to convert document.");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, FUNCTION_CONVERT_DOCUMENT);
            if (connector == null)
                return false;

            ConvertDocumentRequest req = new ConvertDocumentRequest();
            req.Callback = callback;
            req.OnResponse = ConvertDocumentResponse;
            req.Data = data;
            req.Parameters["version"] = Version.DOCUMENT_CONVERSION;

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
                    Log.Error("DocumentConversion", "Failed to upload {0}!", documentPath);
            }

            if (documentData != null)
            {
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";

                string config = string.Format("{\"conversion_target\":\"{0}\"}", conversionTarget);
                req.Forms = new Dictionary<string, RESTConnector.Form>();
                req.Forms["file"] = new RESTConnector.Form(documentData);
                req.Forms["config"] = new RESTConnector.Form(config);
            }

            return connector.Send(req);
        }

        private class ConvertDocumentRequest : RESTConnector.Request
        {
            public string Data { get; set; }
            public OnConvertDocument Callback { get; set; }
        };

        private void ConvertDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DocumentConversionResponse response = new DocumentConversionResponse();
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
                    Log.Error("DocumentConversion", "ConvertDocumentResponse Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((ConvertDocumentRequest)req).Callback != null)
                ((ConvertDocumentRequest)req).Callback(resp.Success ? response : null, ((ConvertDocumentRequest)req).Data);
        }

        #region IWatsonService interface
        /// <exclude /> 
        public string GetServiceID()
        {
            throw new NotImplementedException();
        }

        /// <exclude />
        public void GetServiceStatus(ServiceStatus callback)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
