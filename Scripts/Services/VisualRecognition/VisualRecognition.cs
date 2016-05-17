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
using IBM.Watson.DeveloperCloud.Services;
using FullSerializer;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;
using IBM.Watson.DeveloperCloud.Connection;
using System.Text;

namespace IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3
{
    public class VisualRecognition : IWatsonService {
        #region Public Types
        /// <summary>
        /// Callback used by FindClassifier().
        /// </summary>
        /// <param name="classifier">The classifer found by name.</param>
        public delegate void OnFindClassifier(GetClassifiersPerClassifierBrief classifier);
        /// <summary>
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        /// <param name="classifiers"></param>
        public delegate void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers);
        #endregion

        #region Private Data
        private const string SERVICE_ID = "VisualRecognitionV3";
        private const string SERVICE_CLASSIFY = "/v3/classify";
        private const string SERVICE_DETECT_FACES = "/v3/detect_faces";
        private const string SERVICE_RECOGNIZE_TEXT = "/v3/recognize_text";
        private const string SERVICE_CLASSIFIERS = "/v3/classifiers";
        private static string mp_ApiKey = null;
        private static fsSerializer sm_Serializer = new fsSerializer();
        #endregion

        #region Classify Image
        #endregion

        #region Detect Faces
        #endregion

        #region Recognize Text
        #endregion
        /*
        #region Find Classifier
        private void FindClassifier(string classifierName, OnFindClassifier callback)
        {
            new FindClassifierReq(this, classifierName, callback);
        }

        private class FindClassifierReq
        {
            public FindClassifierReq(VisualRecognition service, string classifierName, OnFindClassifier callback)
            {
                if(callback == null)
                    throw new ArgumentNullException("callback");
                if(string.IsNullOrEmpty(classifierName))
                    throw new WatsonException("classifierName required");
                if(string.IsNullOrEmpty(mp_ApiKey))
                    mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
                if(string.IsNullOrEmpty(mp_ApiKey))
                    throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

                Service = service;
                ClassifierName = classifierName;
                Callback = callback;

                Service.GetClassifiers(GetClassifiers);
            }

            public VisualRecognition Service { get; set; }
            public string ClassifierName { get; set; }
            public OnFindClassifier Callback { get; set; }

            private void GetClassifiers(GetClassifiersTopLevelBreif classifiers)
            {

            }
        }
        #endregion
*/
        #region Get Classifiers
        public bool GetClassifiers(OnGetClassifiers callback)
        {
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFIERS);
            if(connector == null)
                return false;

            GetClassifiersReq req = new GetClassifiersReq();
            req.Callback = callback;
            req.Parameters["api_key"] = mp_ApiKey;
            req.OnResponse = OnGetClassifiersResp;

            return connector.Send(req);
        }
        private class GetClassifiersReq : RESTConnector.Request
        {
            public OnGetClassifiers Callback { get; set; }
        }
        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersTopLevelBrief classifiers = new GetClassifiersTopLevelBrief();
            if(resp.Success)
            {
                try
                {
                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    object obj = classifiers;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);

                    if(!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);
                }
                catch(Exception e)
                {
                    Log.Error("VisualRecognition", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if(((GetClassifiersReq)req).Callback != null)
                ((GetClassifiersReq)req).Callback(resp.Success ? classifiers : null);
        }
        #endregion

        #region Create Classifier
        #endregion

        #region Delete Classifier
        #endregion

        #region Get Classifier Details
        #endregion
        #region IWatsonService implementation

        public string GetServiceID()
        {
            return SERVICE_ID;
        }

        public void GetServiceStatus(ServiceStatus callback)
        {
            if (Config.Instance.FindCredentials(SERVICE_ID) != null)
                new CheckServiceStatus(this, callback);
            else
                callback(SERVICE_ID, false);
        }

        private class CheckServiceStatus
        {
            private VisualRecognition m_service = null;
            private ServiceStatus m_Callback = null;
            private int m_GetClassifierCount = 0;
            private int m_ClassifyCount = 0;

            public CheckServiceStatus(VisualRecognition service, ServiceStatus callback)
            {
                m_service = service;
                m_Callback = callback;

                //  if classifier id
                    //  if not get classifier with classifierid
                        //  onfailure
                    //  else classifierCount ++
                //  else
                    //  if not get classifiers
                        //  onfailure
                    //  else onCheckServices

            }

            public void OnCheckServices(ClassifyTopLevelMultiple classifiers)
            {

            }

            public void OnCheckService(ClassifyTopLevelSingle classifier)
            {

            }

            public void OnClassify(ClassResult result)
            {

            }

            void OnFailure(string msg)
            {
                Log.Error("VisualRecognition", msg);
                if(m_Callback != null && m_Callback.Target != null)
                {
                    m_Callback(SERVICE_ID, false);
                }

                m_GetClassifierCount = m_ClassifyCount = 0;
            }
        }

        #endregion
    	
    }
}
