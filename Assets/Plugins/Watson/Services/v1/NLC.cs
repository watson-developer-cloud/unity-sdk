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

using IBM.Watson.Logging;
using IBM.Watson.Connection;
using IBM.Watson.Utilities;
using MiniJSON;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;
using System.Net;

namespace IBM.Watson.Services.v1
{
    public class NLC
    {
        #region Public Types
        public class Classifier
        {
            public string name { get; set; }
            public string language { get; set; }
            public string url { get; set; }
            public string classifier_id { get; set; }
            public string created { get; set; }
            public string status { get; set; }
            public string status_description { get; set; }
        };
        public delegate void OnGetClassifier(Classifier classifier);
        public delegate void OnTrainClassifier(Classifier classifier);
        public delegate void OnFindClassifier(Classifier classifier);

        public class Classifiers
        {
            public Classifier[] classifiers { get; set; }
        };
        public delegate void OnGetClassifiers(Classifiers classifiers);

        public class Class
        {
            public double confidence { get; set; }
            public string class_name { get; set; }
        };
        public class ClassifyResult
        {
            public string classifier_id { get; set; }
            public string url { get; set; }
            public string text { get; set; }
            public string top_class { get; set; }
            public Class[] classes { get; set; }
        };
        public delegate void OnClassify( ClassifyResult classify );
        public delegate void OnDeleteClassifier( bool success );
        #endregion

        #region Private Data
        private const string SERVICE_ID = "NlcV1";
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region FindClassifier
        /// <summary>
        /// Find a classifier by name.
        /// </summary>
        /// <param name="classifierName"></param>
        /// <param name="callback"></param>
        public void FindCLassiifer( string classifierName, OnFindClassifier callback )
        {
            new FindClassifier( this, classifierName, callback );
        }

        private class FindClassifier
        {
            public FindClassifier( NLC service, string classifierName, OnFindClassifier callback )
            {
                if (service == null)
                    throw new ArgumentNullException("service");
                if ( string.IsNullOrEmpty( classifierName ) )
                    throw new ArgumentNullException("classifierName");
                if (callback == null)
                    throw new ArgumentNullException("callback");

                Service = service;
                ClassifierName = classifierName;
                Callback = callback;
                
                Service.GetClassifiers( GetClassifiers );
            }

            public NLC Service { get; set; }
            public string ClassifierName { get; set; }
            public OnFindClassifier Callback { get; set; }

            private void GetClassifiers( Classifiers classifiers )
            {
                bool bFound = false;
                foreach( var c in classifiers.classifiers )
                    if ( c.name == ClassifierName )
                    {
                        // now get the classifier details..
                        bFound = Service.GetClassifier( c.classifier_id, GetClassifier );
                        break;
                    }

                if (! bFound )
                    Callback( null );
            }

            private void GetClassifier( Classifier classifier )
            {
                if ( Callback != null )
                    Callback( classifier );
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
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
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
        /// <param name="callback">The callback to invoke with the Classifier object.</param>
        /// <returns>Returns true if the request is submitted.</returns>
        public bool GetClassifier(string classifierId, OnGetClassifier callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers/" + classifierId );
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
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
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
        public bool TrainClassifier( string classifierName, string language, string trainingData, OnTrainClassifier callback)
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

            Dictionary<string,object> trainingMetaData = new Dictionary<string, object>();
            trainingMetaData["language"] = language;
            trainingMetaData["name"] = classifierName;

            TrainClassifierReq req = new TrainClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnTrainClassifierResp;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["training_metadata"] = new RESTConnector.Form( Encoding.UTF8.GetBytes( Json.Serialize( trainingMetaData ) ) );
            req.Forms["training_data"] = new RESTConnector.Form( Encoding.UTF8.GetBytes( trainingData ) );

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
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
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
        /// <param name="classiferId">The ID of the classifier.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if we failed to submit a request.</returns>
        public bool DeleteClassifer( string classifierId, OnDeleteClassifier callback )
        {
            if ( string.IsNullOrEmpty( classifierId ) )
                throw new ArgumentNullException( "classiferId" );
            if ( callback == null )
                throw new ArgumentNullException("callback" );

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers/" + classifierId );
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
        /// Classifies the given text, invokes the callback with the results.
        /// </summary>
        /// <param name="classifierId">The ID of the classifier to use.</param>
        /// <param name="text">The text to classify.</param>
        /// <param name="callback">The callback to invoke with the results.</param>
        /// <returns>Returns false if we failed to submit the request.</returns>
        public bool Classify( string classifierId, string text, OnClassify callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/classifiers");
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.OnResponse = OnClassifyResp;
            req.Function = "/" + classifierId + "/classify";
            req.Headers["Content-Type"] = "application/json";

            Dictionary<string,object> body = new Dictionary<string, object>();
            body["text"] = text;
            req.Send = Encoding.UTF8.GetBytes( Json.Serialize( body ) );

            return connector.Send(req);
        }
        private class ClassifyReq : RESTConnector.Request
        {
            public OnClassify Callback { get; set; }
        };
        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyResult classify = new ClassifyResult();
            if (resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    object obj = classify;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("NLC", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(resp.Success ? classify : null);
        }
        #endregion
    }
}
