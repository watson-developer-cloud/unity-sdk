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
        public delegate void OnGetClassifier(Classifier classifier);
        /// <summary>
        /// Callback used by the TrainClassifier() method.
        /// </summary>
        /// <param name="classifier">The classifier created.</param>
        public delegate void OnTrainClassifier(Classifier classifier);
        /// <summary>
        /// Callback used by FindClassifier().
        /// </summary>
        /// <param name="classifier">The classifer found by name.</param>
        public delegate void OnFindClassifier(Classifier classifier);

        /// <summary>
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        /// <param name="classifiers"></param>
        public delegate void OnGetClassifiers(Classifiers classifiers);

        /// <summary>
        /// This callback is used by the Classify() method.
        /// </summary>
        /// <param name="classify"></param>
        public delegate void OnClassify(ClassifyResult classify);
        /// <summary>
        /// This callback is used by the DeleteClassifier() method.
        /// </summary>
        /// <param name="success"></param>
        public delegate void OnDeleteClassifier(bool success);
        #endregion

        #region Public Properties
        /// <summary>
        /// Disable the classify cache.
        /// </summary>
        public bool DisableCache { get; set; }
        #endregion

        #region Private Data
        private const string SERVICE_ID = "NaturalLanguageClassifierV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        private Dictionary<string, DataCache> m_ClassifyCache = new Dictionary<string, DataCache>();
        #endregion

        #region FindClassifier
        /// <summary>
        /// Find a classifier by name.
        /// </summary>
        /// <param name="classifierName"></param>
        /// <param name="callback"></param>
        public void FindClassifier(string classifierName, OnFindClassifier callback)
        {
            new FindClassifierReq(this, classifierName, callback);
        }

        private class FindClassifierReq
        {
            public FindClassifierReq(NaturalLanguageClassifier service, string classifierName, OnFindClassifier callback)
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if (string.IsNullOrEmpty(classifierName))
                    throw new ArgumentNullException("classifierName");
                if (callback == null)
                    throw new ArgumentNullException("callback");

                Service = service;
                ClassifierName = classifierName;
                Callback = callback;

                Service.GetClassifiers(GetClassifiers);
            }

            public NaturalLanguageClassifier Service { get; set; }
            public string ClassifierName { get; set; }
            public OnFindClassifier Callback { get; set; }

            private void GetClassifiers(Classifiers classifiers)
            {
                bool bFound = false;
                foreach (var c in classifiers.classifiers)
                    if (c.name.ToLower().StartsWith(ClassifierName.ToLower()))
                    {
                        // now get the classifier details..
                        bFound = Service.GetClassifier(c.classifier_id, GetClassifier);
                        break;
                    }

                if (!bFound)
                {
                    Log.Error("Natural Language Classifier", "Fail to find classifier {0}", ClassifierName);
                    Callback(null);
                }
            }

            private void GetClassifier(Classifier classifier)
            {
                if (Callback != null)
                    Callback(classifier);
            }
        };
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers");
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
        };
        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifiers classifiers = new Classifiers();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifiers;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClassifiersReq)req).Callback != null)
                ((GetClassifiersReq)req).Callback(resp.Success ? classifiers : null);
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers/" + classifierId);
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
        };
        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifier classifier = new Classifier();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifier;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClassifierReq)req).Callback != null)
                ((GetClassifierReq)req).Callback(resp.Success ? classifier : null);
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers");
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
        };
        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Classifier classifier = new Classifier();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classifier;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((TrainClassifierReq)req).Callback != null)
                ((TrainClassifierReq)req).Callback(resp.Success ? classifier : null);
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

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers/" + classifierId);
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
        };
        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if (((DeleteClassifierReq)req).Callback != null)
                ((DeleteClassifierReq)req).Callback(resp.Success);
        }
        #endregion

        #region Classify
        /// <summary>
        /// Flush all classifier caches or a specific cache.
        /// </summary>
        /// <param name="classifierId">If not null or empty, then the specific cache will be flushed.</param>
        public void FlushClassifyCache(string classifierId = null)
        {
            if (!string.IsNullOrEmpty(classifierId))
            {
                DataCache cache = null;
                if (m_ClassifyCache.TryGetValue(classifierId, out cache))
                    cache.Flush();
            }
            else
            {
                foreach (var kp in m_ClassifyCache)
                    kp.Value.Flush();
            }
        }

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

            string textId = Utility.GetMD5(text);
            if (!DisableCache)
            {
                DataCache cache = null;
                if (!m_ClassifyCache.TryGetValue(classifierId, out cache))
                {
                    cache = new DataCache("NaturalLanguageClassifier_" + classifierId);
                    m_ClassifyCache[classifierId] = cache;
                }

                byte[] cached = cache.Find(textId);
                if (cached != null)
                {
                    ClassifyResult res = ProcessClassifyResult(cached);
                    if (res != null)
                    {
                        callback(res);
                        return true;
                    }
                }
            }

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers");
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.TextId = textId;
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
            public string TextId { get; set; }
            public string ClassiferId { get; set; }
            public OnClassify Callback { get; set; }
        };

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyResult classify = null;
            if (resp.Success)
            {
                classify = ProcessClassifyResult(resp.Data);
                if (classify != null)
                {
                    DataCache cache = null;
                    if (m_ClassifyCache.TryGetValue(((ClassifyReq)req).ClassiferId, out cache))
                        cache.Save(((ClassifyReq)req).TextId, resp.Data);
                }
            }

            if (((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(classify);
        }

        private ClassifyResult ProcessClassifyResult(byte[] json_data)
        {
            ClassifyResult classify = null;
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                classify = new ClassifyResult();

                object obj = classify;
                r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);
            }
            catch (Exception e)
            {
                Log.Error("Natural Language Classifier", "GetClassifiers Exception: {0}", e.ToString());
            }

            return classify;
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
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private NaturalLanguageClassifier m_Service = null;
            private ServiceStatus m_Callback = null;
            private int m_GetClassifierCount = 0;
            private int m_ClassifyCount = 0;

            public CheckServiceStatus(NaturalLanguageClassifier service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                string customClassifierID = Config.Instance.GetVariableValue(SERVICE_ID + "_ID");
                m_Service.DisableCache = true;
                //If custom classifierID is defined then we are using it to check the service health
                if (!string.IsNullOrEmpty(customClassifierID))
                {

                    if (!m_Service.GetClassifier(customClassifierID, OnCheckService))
                    {
                        OnFailure("Failed to call GetClassifier()");
                    }
                    else
                    {
                        m_GetClassifierCount += 1;
                    }
                }
                else
                {
                    if (!m_Service.GetClassifiers(OnCheckServices))
                        OnFailure("Failed to call GetClassifiers()");
                }

            }

            private void OnCheckServices(Classifiers classifiers)
            {
                if (m_Callback != null)
                {
                    if (classifiers.classifiers.Length > 0)
                    {
                        foreach (var classifier in classifiers.classifiers)
                        {
                            // check the status of one classifier, if it's listed as "Unavailable" then fail 
                            if (!m_Service.GetClassifier(classifier.classifier_id, OnCheckService))
                            {
                                OnFailure("Failed to call GetClassifier()");
                                break;
                            }
                            else
                                m_GetClassifierCount += 1;
                        }
                    }
                    else
                    {
                        if (m_Callback != null && m_Callback.Target != null)
                        {
                            m_Callback(SERVICE_ID, true);     // no classifiers to check, just return success then..
                        }
                    }
                }
                else
                {
                    if (m_Callback != null && m_Callback.Target != null)
                    {
                        m_Callback(SERVICE_ID, false);
                    }
                }
            }

            private void OnCheckService(Classifier classifier)
            {
                if (m_GetClassifierCount > 0)
                {
                    m_GetClassifierCount -= 1;
                    if (classifier != null)
                    {
                        if (classifier.status == "Unavailable" || classifier.status == "Failed")
                        {
                            OnFailure(string.Format("Status of classifier {0} came back as {1}.",
                                classifier.classifier_id, classifier.status));
                        }
                        else
                        {
                            // try to classify something with this classifier..
                            if (!m_Service.Classify(classifier.classifier_id, "Hello World", OnClassify))
                                OnFailure("Failed to invoke Classify");
                            else
                                m_ClassifyCount += 1;
                        }
                    }
                    else
                        OnFailure("Failed to get classifier.");
                }
            }

            private void OnClassify(ClassifyResult result)
            {
                if (m_ClassifyCount > 0)
                {
                    m_ClassifyCount -= 1;
                    if (result != null)
                    {
                        // success!
                        if (m_ClassifyCount == 0 && m_Callback != null && m_Callback.Target != null)
                        {
                            m_Callback(SERVICE_ID, true);
                        }
                    }
                    else
                        OnFailure("Failed to classify.");
                }
            }

            void OnFailure(string msg)
            {
                Log.Error("NaturalLanguageClassifier", msg);
                if (m_Callback != null && m_Callback.Target != null)
                {
                    m_Callback(SERVICE_ID, false);
                }
                m_GetClassifierCount = m_ClassifyCount = 0;
            }

        };
        #endregion
    }
}
