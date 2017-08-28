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
        /// <param name="classifier">The classifier found by ID.</param>
        public delegate void OnGetClassifier(Classifier classifier, string customData);
        /// <summary>
        /// Callback used by the TrainClassifier() method.
        /// </summary>
        /// <param name="classifier">The classifier created.</param>
        public delegate void OnTrainClassifier(Classifier classifier, string customData);
        /// <summary>
        /// Callback used by FindClassifier().
        /// </summary>
        /// <param name="classifier">The classifer found by name.</param>
        public delegate void OnFindClassifier(Classifier classifier, string customData);

        /// <summary>
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        /// <param name="classifiers"></param>
        public delegate void OnGetClassifiers(Classifiers classifiers, string customData);

        /// <summary>
        /// This callback is used by the Classify() method.
        /// </summary>
        /// <param name="classify"></param>
        public delegate void OnClassify(ClassifyResult classify, string customData);
        /// <summary>
        /// This callback is used by the DeleteClassifier() method.
        /// </summary>
        /// <param name="success"></param>
        public delegate void OnDeleteClassifier(bool success, string customData);
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
            Classifiers classifiers = new Classifiers();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifiers;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetClassifiersReq)req).Data;
            if (((GetClassifiersReq)req).Callback != null)
                ((GetClassifiersReq)req).Callback(resp.Success ? classifiers : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
            Classifier classifier = new Classifier();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifier;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((GetClassifierReq)req).Data;
            if (((GetClassifierReq)req).Callback != null)
                ((GetClassifierReq)req).Callback(resp.Success ? classifier : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
            Classifier classifier = new Classifier();
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifier;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            string customData = ((TrainClassifierReq)req).Data;
            if (((TrainClassifierReq)req).Callback != null)
                ((TrainClassifierReq)req).Callback(resp.Success ? classifier : null, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
            if (((DeleteClassifierReq)req).Callback != null)
                ((DeleteClassifierReq)req).Callback(resp.Success, customData);
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
            ClassifyResult classify = null;
            fsData data = null;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    classify = new ClassifyResult();

                    object obj = classify;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                }

            }

            string customData = ((ClassifyReq)req).Data;
            if (((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(classify, !string.IsNullOrEmpty(customData) ? customData : data.ToString());
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
