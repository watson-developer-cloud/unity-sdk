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
using System.Collections.Generic;
using System.IO;
using MiniJSON;

namespace IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3
{
    public class VisualRecognition : IWatsonService
    {
        #region Public Types
        /// <summary>
        /// Callback used by FindClassifier().
        /// </summary>
        /// <param name="classifier">The classifer found by name.</param>
        public delegate void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier);
        /// <summary>
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        /// <param name="classifiers"></param>
        public delegate void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers);
        /// <summary>
        /// Callback used by the GetClassifier() method.
        /// </summary>
        /// <param name="classifier">The classifier found by ID.</param>
        public delegate void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier);
        /// <summary>
        /// This callback is used by the DeleteClassifier() method.
        /// </summary>
        /// <param name="success"></param>
        public delegate void OnDeleteClassifier(bool success);
        /// <summary>
        /// Callback used by the TrainClassifier() method.
        /// </summary>
        /// <param name="classifier">The classifier created.</param>
        public delegate void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier);
        /// <summary>
        /// This callback is used by the Classify() method.
        /// </summary>
        /// <param name="classify"></param>
        public delegate void OnClassify(ClassifyTopLevelMultiple classify);
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
        #endregion

        #region Private Data
        private const string SERVICE_ID = "VisualRecognitionV3";
        private const string SERVICE_CLASSIFY = "/v3/classify";
        private const string SERVICE_DETECT_FACES = "/v3/detect_faces";
        private const string SERVICE_RECOGNIZE_TEXT = "/v3/recognize_text";
        private const string SERVICE_CLASSIFIERS = "/v3/classifiers";
        private static string mp_ApiKey = null;
        private static fsSerializer sm_Serializer = new fsSerializer();
        private const float REQUEST_TIMEOUT = 10.0f * 60.0f;
        #endregion

        #region Classify Image
        public bool Classify(string url, OnClassify callback, string[] owners = default(string[]), 
            string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en")
        {
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if(callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFY);
            if(connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.OnResponse = OnClassifyResp;
            req.AcceptLanguage = acceptLanguage;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            if(owners != default(string[]))
                req.Parameters["owners"] = string.Join(",", owners);
            if(classifierIDs != default(string[]))
                req.Parameters["classifier_ids"] = string.Join(",", classifierIDs);
            if(threshold != default(float))
                req.Parameters["threshold"] = threshold;

            return connector.Send(req);
        }

        public bool Classify(OnClassify callback, string imagePath = default(string), string[] urls = default(string[]), string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en" )
        {
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            if(callback == null)
                throw new ArgumentNullException("callback");

            byte[] imageData = null;
            string ext = "";
            string mimeType = "";
            if(imagePath != default(string))
            {
                if(LoadFile != null)
                {
                    imageData = LoadFile(imagePath);
                }
                else
                {
                    #if !UNITY_WEBPLAYER
                    imageData = File.ReadAllBytes(imagePath);
                    #endif
                }

                switch(Path.GetExtension(imagePath))
                {
                    case ".jpg":
                    case ".jpeg":
                        ext = ".jpg";
                        mimeType = "image/jpeg";
                        break;
                    case ".png":
                        ext = ".png";
                        mimeType = "image/png";
                        break;
                    case ".gif":
                        ext = ".gif";
                        mimeType = "image/gif";
                        break;
                    case ".zip":
                        ext = ".zip";
                        mimeType = "application/zip";
                        break;
                    default:
                        throw new WatsonException("Cannot classify unsupported file format " + Path.GetExtension(imagePath) + ". Please use jpg, gif, png or zip!");
                }
            }
                
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFY);
            if(connector == null)
                return false;
            
            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.AcceptLanguage = acceptLanguage;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["parameters"] = new RESTConnector.Form(BuildParametersJson(urls, owners, classifierIDs, threshold));
            if(imageData != null && !string.IsNullOrEmpty(ext) && !string.IsNullOrEmpty(mimeType))
            {
                req.Forms["images_file"] = new RESTConnector.Form(imageData, "images_file" + ext, mimeType); 
            }

            return connector.Send(req);
        }

        private string BuildParametersJson(string[] urls, string[] owners, string[] classifierIDs, float threshold)
        {
            ClassifyParameters cParameters = new ClassifyParameters();
            cParameters.urls = urls;
            cParameters.owners = owners;
            cParameters.classifier_ids = classifierIDs;
            cParameters.threshold = threshold;

            return Json.Serialize(cParameters);
        }

        private class ClassifyReq : RESTConnector.Request
        {
            public OnClassify Callback { get; set; }
            public string AcceptLanguage { get; set; }
        }

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyTopLevelMultiple classify = null;
            if(resp.Success)
            {
                classify = ProcessClassifyResult(resp.Data);
            }

            if(((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(classify);
        }

        private ClassifyTopLevelMultiple ProcessClassifyResult(byte[] json_data)
        {
            ClassifyTopLevelMultiple classify = null;
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                classify = new ClassifyTopLevelMultiple();

                object obj = classify;
                r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);
            }
            catch(Exception e)
            {
                Log.Error("Visual Recognition", "Classify exception: {0}", e.ToString());
            }

            return classify;
        }
        #endregion

        #region Detect Faces
        #endregion

        #region Recognize Text
        #endregion

        #region Find Classifier by name
        public void FindClassifier(string classifierName, OnFindClassifier callback)
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

                Service.GetClassifiers(OnGetClassifiers);
            }

            public VisualRecognition Service { get; set; }
            public string ClassifierName { get; set; }
            public OnFindClassifier Callback { get; set; }

            private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers)
            {
                bool bFound = false;
                foreach(var c in classifiers.classifiers)
                {
                    if(c.name.ToLower().StartsWith(ClassifierName.ToLower()))
                    {
                        bFound = Service.GetClassifier(c.classifier_id, OnGetClassifier);
                        break;
                    }
                }

                if(!bFound)
                {
                    Log.Error("VisualRecognition", "Failed to find classifier {0}", ClassifierName);
                    Callback(null);
                }
            }

            private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier)
            {
                if(Callback != null)
                    Callback(classifier);
            }
        }
        #endregion

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

        #region Get Classifier
        public bool GetClassifier(string classifierId, OnGetClassifier callback)
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFIERS + "/" + classifierId);
            if (connector == null)
                return false;

            GetClassifierReq req = new GetClassifierReq();
            req.Callback = callback;
            req.Parameters["api_key"] = mp_ApiKey;
            req.OnResponse = OnGetClassifierResp;

            return connector.Send(req);
        }
        private class GetClassifierReq : RESTConnector.Request
        {
            public OnGetClassifier Callback { get; set; }
        };
        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersPerClassifierVerbose classifier = new GetClassifiersPerClassifierVerbose();
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
                    Log.Error("Visual Recognition", "GetClassifier Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (((GetClassifierReq)req).Callback != null)
                ((GetClassifierReq)req).Callback(resp.Success ? classifier : null);
        }
        #endregion

        #region Create Classifier
        public bool TrainClassifier(string classifierName, string className, string positiveExamplesPath, string negativeExamplesPath, OnTrainClassifier callback)
        {
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            if(string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if(string.IsNullOrEmpty(positiveExamplesPath))
                throw new ArgumentNullException("positiveExamplesPath");
            if(string.IsNullOrEmpty(negativeExamplesPath))
                throw new ArgumentNullException("negativeExamplesPath");
            if(callback == null)
                throw new ArgumentNullException("callback");

            byte[] positiveExamplesData = null;
            byte[] negativeExamplesData = null;
            if(LoadFile != null)
            {
                positiveExamplesData = LoadFile(positiveExamplesPath);
                negativeExamplesData = LoadFile(negativeExamplesPath);
            }
            else
            {
                #if !UNITY_WEBPLAYER
                positiveExamplesData = File.ReadAllBytes(positiveExamplesPath);
                negativeExamplesData = File.ReadAllBytes(negativeExamplesPath);
                #endif
            }

            return UploadClassifier(classifierName, className, positiveExamplesData, negativeExamplesData, callback);
        }

        public bool UploadClassifier(string classifierName, string className, byte[] positiveExamplesData, byte[] negativeExamplesData, OnTrainClassifier callback)
        {
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            if(string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if(positiveExamplesData == null)
                throw new ArgumentNullException("positiveExamplesData");
            if(negativeExamplesData == null)
                throw new ArgumentNullException("negativeExamplesData");
            if(callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFIERS);
            if(connector == null)
                return false;

//            data.Name + "/" + DateTime.Now.ToString();

            TrainClassifierReq req = new TrainClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnTrainClassifierResp;
            req.Timeout = REQUEST_TIMEOUT;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["name"] = new RESTConnector.Form(classifierName);
            req.Forms[className + "_positive_examples"] = new RESTConnector.Form(positiveExamplesData, className + "_positive_examples.zip", "application/zip");
            req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamplesData, "negative_examples.zip", "application/zip");

            return connector.Send(req);
        }

        private class TrainClassifierReq:RESTConnector.Request
        {
            public OnTrainClassifier Callback {get; set;}
        }

        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersPerClassifierVerbose classifier = new GetClassifiersPerClassifierVerbose();
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

        #region Delete Classifier
        public bool DeleteClassifier(string classifierId, OnDeleteClassifier callback)
        {
            if(string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("GetClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            
            Log.Debug("VisualRecognition", "Attempting to delete classifier {0}", classifierId);
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFIERS + "/" + classifierId);
            if(connector == null)
                return false;

            DeleteClassifierReq req = new DeleteClassifierReq();
            req.Callback = callback;
            req.Timeout = REQUEST_TIMEOUT;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            req.OnResponse = OnDeleteClassifierResp;
            req.Delete = true;

            return connector.Send(req);
        }
        private class DeleteClassifierReq : RESTConnector.Request
        {
            public OnDeleteClassifier Callback { get; set; }
        }
        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            if(((DeleteClassifierReq)req).Callback != null)
                ((DeleteClassifierReq)req).Callback(resp.Success);
        }
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
