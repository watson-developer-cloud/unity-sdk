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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1
{
    /// <summary>
    /// This class wraps the Natural Language Classifier service.
    /// <a href="http://www.ibm.com/watson/developercloud/nl-classifier.html">Natural Language Classifier Service</a>
    /// </summary>
    public class NaturalLanguageClassifier : IWatsonService
    {
        #region Public Types
        /// <summary>
        /// Callback used by the GetClassifier() method.
        /// </summary>
        public delegate void OnGetClassifier(RESTConnector.ParsedResponse<Classifier> resp);
        /// <summary>
        /// Callback used by the TrainClassifier() method.
        /// </summary>
        public delegate void OnTrainClassifier(RESTConnector.ParsedResponse<Classifier> resp);
        /// <summary>
        /// Callback used by FindClassifier().
        /// </summary>
        public delegate void OnFindClassifier(RESTConnector.ParsedResponse<Classifier> resp);

        /// <summary>
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        public delegate void OnGetClassifiers(RESTConnector.ParsedResponse<Classifiers> resp);

        /// <summary>
        /// This callback is used by the Classify() method.
        /// </summary>
        public delegate void OnClassify(RESTConnector.ParsedResponse<ClassifyResult> resp);
        /// <summary>
        /// This callback is used by the DeleteClassifier() method.
        /// </summary>
        public delegate void OnDeleteClassifier(RESTConnector.ParsedResponse<object> resp);
        #endregion

        #region Constructor
        public NaturalLanguageClassifier(Credentials credentials)
        {
            if (credentials.HasCredentials() || credentials.HasAuthorizationToken())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide a username and password or authorization token to use the Natural Language Classifier service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
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

        #region Private Data
        private const string ServiceId = "NaturalLanguageClassifierV1";
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/natural-language-classifier/api";
        #endregion

        #region GetClassifiers
        /// <summary>
        /// Returns an array of all classifiers to the callback function.
        /// </summary>
        /// <param name="callback">The callback to invoke with the Classifiers object.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool GetClassifiers(OnGetClassifiers callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            GetClassifiersReq req = new GetClassifiersReq();
            req.Callback = callback;
            req.OnResponse = OnGetClassifiersResp;

            return connector.Send(req);
        }

        private class GetClassifiersReq : RESTConnector.Request
        {
            public OnGetClassifiers Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetClassifiersReq)req).Data;

            RESTConnector.ParsedResponse<Classifiers> parsedResp = new RESTConnector.ParsedResponse<Classifiers>(resp, customData, _serializer);

            if (((GetClassifiersReq)req).Callback != null)
                ((GetClassifiersReq)req).Callback(parsedResp);
        }
        #endregion

        #region GetClassifier
        /// <summary>
        /// Returns a specific classifer.
        /// </summary>
        /// <param name="classifierId">The ID of the classifier to get.</param>
        /// <param name="callback">The callback to invoke with the Classifier object.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool GetClassifier(string classifierId, OnGetClassifier callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers/" + classifierId);
            if (connector == null)
                return false;

            GetClassifierReq req = new GetClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnGetClassifierResp;

            return connector.Send(req);
        }

        private class GetClassifierReq : RESTConnector.Request
        {
            public OnGetClassifier Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetClassifierReq)req).Data;

            RESTConnector.ParsedResponse<Classifier> parsedResp = new RESTConnector.ParsedResponse<Classifier>(resp, customData, _serializer);

            if (((GetClassifierReq)req).Callback != null)
                ((GetClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region TrainClassifier
        /// <summary>
        /// Train a new classifier. 
        /// </summary>
        /// <param name="classifierName">A name to give the classifier.</param>
        /// <param name="language">Language of the classifier.</param>
        /// <param name="trainingData">CSV training data.</param>
        /// <param name="callback">Callback to invoke with the results.</param>
        /// <returns>Returns true if training data was submitted correctly.</returns>
        public bool TrainClassifier(string classifierName, string language, string trainingData, OnTrainClassifier callback)
        {
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");
            if (string.IsNullOrEmpty(trainingData))
                throw new ArgumentNullException("trainingData");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            Dictionary<string, object> trainingMetaData = new Dictionary<string, object>();
            trainingMetaData["language"] = language;
            trainingMetaData["name"] = classifierName;

            TrainClassifierReq req = new TrainClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnTrainClassifierResp;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["training_metadata"] = new RESTConnector.Form(Encoding.UTF8.GetBytes(Json.Serialize(trainingMetaData)));
            req.Forms["training_data"] = new RESTConnector.Form(Encoding.UTF8.GetBytes(trainingData));

            return connector.Send(req);
        }

        private class TrainClassifierReq : RESTConnector.Request
        {
            public OnTrainClassifier Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((TrainClassifierReq)req).Data;

            RESTConnector.ParsedResponse<Classifier> parsedResp = new RESTConnector.ParsedResponse<Classifier>(resp, customData, _serializer);

            if (((TrainClassifierReq)req).Callback != null)
                ((TrainClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region DeleteClassifier
        /// <summary>
        /// Deletes the specified classifier.
        /// </summary>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if we failed to submit a request.</returns>
        public bool DeleteClassifer(string classifierId, OnDeleteClassifier callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classiferId");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers/" + classifierId);
            if (connector == null)
                return false;

            DeleteClassifierReq req = new DeleteClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnDeleteClassifierResp;
            req.Delete = true;

            return connector.Send(req);
        }

        private class DeleteClassifierReq : RESTConnector.Request
        {
            public OnDeleteClassifier Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteClassifierReq)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteClassifierReq)req).Callback != null)
                ((DeleteClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region Classify
        /// <summary>
        /// Classifies the given text, invokes the callback with the results.
        /// </summary>
        /// <param name="classifierId">The ID of the classifier to use.</param>
        /// <param name="text">The text to classify.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if we failed to submit the request.</returns>
        public bool Classify(string classifierId, string text, OnClassify callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v1/classifiers");
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.ClassiferId = classifierId;
            req.Callback = callback;
            req.OnResponse = OnClassifyResp;
            req.Function = "/" + classifierId + "/classify";
            req.Headers["Content-Type"] = "application/json";

            Dictionary<string, object> body = new Dictionary<string, object>();
            body["text"] = text;
            req.Send = Encoding.UTF8.GetBytes(Json.Serialize(body));

            return connector.Send(req);
        }
        private class ClassifyReq : RESTConnector.Request
        {
            public string ClassiferId { get; set; }
            public OnClassify Callback { get; set; }
            public string Data { get; set; }
        };

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((ClassifyReq)req).Data;

            RESTConnector.ParsedResponse<ClassifyResult> parsedResp = new RESTConnector.ParsedResponse<ClassifyResult>(resp, customData, _serializer);

            if (((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(parsedResp);
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
