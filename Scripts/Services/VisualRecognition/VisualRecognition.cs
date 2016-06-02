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
        /// This callback is used by the DetectFaces() method.
        /// </summary>
        /// <param name="faces"></param>
        public delegate void OnDetectFaces(FacesTopLevelMultiple faces);
        /// <summary>
        /// This callback is used by the RecognizeText() method.
        /// </summary>
        /// <param name="faces"></param>
        public delegate void OnRecognizeText(TextRecogTopLevelMultiple text);
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
//        private const string SERVICE_ID = "TestVisualRecognition";
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
            req.Headers["Accepted-Language"] = acceptLanguage;
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

        public bool Classify(OnClassify callback, string imagePath = default(string), string imageURL = default(string), string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en")
        {
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(imagePath) && imageURL == default(string))
                throw new ArgumentNullException("Define an image path and/or image URL to classify!");

            byte[] imageData = null;
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

                if(imageData == null)
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return Classify(imagePath, imageData, callback, imageURL, owners, classifierIDs, threshold, acceptLanguage);
        }

        private bool Classify(string imagePath, byte[] imageData, OnClassify callback, string imageURL = default(string), string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en")
        {
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_CLASSIFY);
            if(connector == null)
                return false;
            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnClassifyResp;
            req.AcceptLanguage = acceptLanguage;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            req.Headers["Accept-Language"] = acceptLanguage;
            req.Forms = new Dictionary<string, RESTConnector.Form>();

            if(!string.IsNullOrEmpty(imageURL) || owners != default(string[]) || classifierIDs != default(string[]) || threshold != default(float))
                req.Forms["parameters"] = new RESTConnector.Form(BuildClassifyParametersJson(imageURL, owners, classifierIDs, threshold));

            if(imageData != null)
                req.Forms["images_file"] = new RESTConnector.Form(imageData, Path.GetFileName(imagePath), GetMimeType(imagePath)); 

            return connector.Send(req);
        }
        
        private string BuildClassifyParametersJson(string url, string[] owners, string[] classifierIDs, float threshold)
        {
            ClassifyParameters cParameters = new ClassifyParameters();
            cParameters.url = url;
            cParameters.owners = owners;
            cParameters.classifier_ids = classifierIDs;
            cParameters.threshold = threshold;
            
            fsData jsondata = new fsData();
            if (sm_Serializer.TrySerialize(cParameters, out jsondata).Succeeded)
            {
                return fsJsonPrinter.CompressedJson(jsondata, true);
            }
            else
            {
                Log.Error("VisualRecognition", "Error parsing to JSON!");
                return null;
            }
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
        public bool DetectFaces(string url, OnDetectFaces callback)
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_DETECT_FACES);
            if(connector == null)
                return false;

            DetectFacesReq req = new DetectFacesReq();
            req.Callback = callback;
            req.OnResponse = OnDetectFacesResp;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VisualRecognitionVersion.Version;

            return connector.Send(req);
        }

        public bool DetectFaces(OnDetectFaces callback, string imagePath = default(string), string imageURL = default(string))
        {
            if(string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(imageURL))
                throw new ArgumentNullException("Define an image path and/or image url to classify!");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            byte[] imageData = null;
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

                if(imageData == null)
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return DetectFaces(callback, imagePath, imageData, imageURL);
        }

        private bool DetectFaces(OnDetectFaces callback, string imagePath, byte[] imageData = default(byte[]), string imageURL = default(string))
        {
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_DETECT_FACES);
            if(connector == null)
                return false;
            DetectFacesReq req = new DetectFacesReq();
            req.Callback = callback;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnDetectFacesResp;

            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;

            req.Forms = new Dictionary<string, RESTConnector.Form>();

            if(!string.IsNullOrEmpty(imageURL))
                req.Forms["parameters"] = new RESTConnector.Form(BuildDetectFacesParametersJson(imageURL));

            if(imageData != null)
                req.Forms["images_file"] = new RESTConnector.Form(imageData, Path.GetFileName(imagePath), GetMimeType(imagePath));

            return connector.Send(req);
        }

        private string BuildDetectFacesParametersJson(string url)
        {
            DetectFacesParameters cParameters = new DetectFacesParameters();
            cParameters.url = url;

            fsData jsondata = new fsData();
            if (sm_Serializer.TrySerialize(cParameters, out jsondata).Succeeded)
            {
                return fsJsonPrinter.CompressedJson(jsondata, true);
            }
            else
            {
                Log.Error("VisualRecognition", "Error parsing to JSON!");
                return null;
            }
        }

        private class DetectFacesReq : RESTConnector.Request
        {
            public OnDetectFaces Callback { get; set; }
        }
            
        private void OnDetectFacesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FacesTopLevelMultiple faces = null;

            if(resp.Success)
            {
                faces = ProcessDetectFaceResult(resp.Data);
            }

            if(((DetectFacesReq)req).Callback != null)
                ((DetectFacesReq)req).Callback(faces);
        }

        private FacesTopLevelMultiple ProcessDetectFaceResult(byte[] json_data)
        {
            FacesTopLevelMultiple faces = null;
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                faces = new FacesTopLevelMultiple();

                object obj = faces;
                r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);
            }
            catch(Exception e)
            {
                Log.Error("Visual Recognition", "Detect faces exception: {0}", e.ToString());
            }

            return faces;
        }
        #endregion

        #region Recognize Text
        public bool RecognizeText(string url, OnRecognizeText callback)
        {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if(callback == null)
                throw new ArgumentNullException("callback");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_RECOGNIZE_TEXT);
            if(connector == null)
                return false;

            RecognizeTextReq req = new RecognizeTextReq();
            req.Callback = callback;
            req.OnResponse = OnRecognizeTextResp;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VisualRecognitionVersion.Version;

            return connector.Send(req);
        }

        /*public bool RecognizeText(OnRecognizeText callback, string imagePath = default(string), string imageURL = default(string))
        {
            if(string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(imageURL))
                throw new ArgumentNullException("Define an image path and/or image URL to classify!");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");
            
            byte[] imageData = null;
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
                
                if(imageData == null)
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }
            
            return RecognizeText(callback, imagePath, imageData, imageURL);
        }
        
        private bool RecognizeText(OnRecognizeText callback, string imagePath = default(string), byte[] imageData = default(byte[]), string imageURL = default(string))
        {
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_RECOGNIZE_TEXT);
            if(connector == null)
                return false;
            RecognizeTextReq req = new RecognizeTextReq();
            req.Callback = callback;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnRecognizeTextResp;
            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            
            if(!string.IsNullOrEmpty(imageURL))
                req.Forms["parameters"] = new RESTConnector.Form(BuildRecognizeTextParametersJson(imageURL));
            
            if(imageData != null)
                req.Forms["images_file"] = new RESTConnector.Form(imageData, Path.GetFileName(imagePath), GetMimeType(imagePath));
            
            return connector.Send(req);
        }*/
        public bool RecognizeText(OnRecognizeText callback, string imagePath = default(string), string imageURL = default(string))
        {
            if(string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(imageURL))
                throw new ArgumentNullException("Define an image path and/or image url to classify!");
            if(string.IsNullOrEmpty(mp_ApiKey))
                mp_ApiKey = Config.Instance.GetVariableValue("VISUAL_RECOGNITION_API_KEY");
            if(string.IsNullOrEmpty(mp_ApiKey))
                throw new WatsonException("FindClassifier - VISUAL_RECOGNITION_API_KEY needs to be defined in config.json");

            byte[] imageData = null;
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

                if(imageData == null)
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return RecognizeText(callback, imagePath, imageData, imageURL);
        }

        private bool RecognizeText(OnRecognizeText callback, string imagePath, byte[] imageData = default(byte[]), string imageURL = default(string))
        {
            RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_RECOGNIZE_TEXT);
            if(connector == null)
                return false;
            RecognizeTextReq req = new RecognizeTextReq();
            req.Callback = callback;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnRecognizeTextResp;

            req.Parameters["api_key"] = mp_ApiKey;
            req.Parameters["version"] = VisualRecognitionVersion.Version;

            req.Forms = new Dictionary<string, RESTConnector.Form>();

            if(!string.IsNullOrEmpty(imageURL))
                req.Forms["parameters"] = new RESTConnector.Form(BuildRecognizeTextParametersJson(imageURL));

            if(imageData != null)
                req.Forms["images_file"] = new RESTConnector.Form(imageData, Path.GetFileName(imagePath), GetMimeType(imagePath));

            return connector.Send(req);
        }

        private class RecognizeTextReq : RESTConnector.Request
        {
            public OnRecognizeText Callback { get; set; }
        }

        private string BuildRecognizeTextParametersJson(string url)
        {
            RecognizeTextParameters cParameters = new RecognizeTextParameters();
            cParameters.url = url;

            fsData jsondata = new fsData();
            if (sm_Serializer.TrySerialize(cParameters, out jsondata).Succeeded)
            {
                return fsJsonPrinter.CompressedJson(jsondata, true);
            }
            else
            {
                Log.Error("VisualRecognition", "Error parsing to JSON!");
                return null;
            }
        }

        private void OnRecognizeTextResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            TextRecogTopLevelMultiple text = null;
            if(resp.Success)
            {
                text = ProcessRecognizeTextResult(resp.Data);
            }

            if(((RecognizeTextReq)req).Callback != null)
                ((RecognizeTextReq)req).Callback(text);
        }

        private TextRecogTopLevelMultiple ProcessRecognizeTextResult(byte[] json_data)
        {
            TextRecogTopLevelMultiple text = null;
            try
            {
                fsData data = null;
                fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(json_data), out data);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);

                text = new TextRecogTopLevelMultiple();

                object obj = text;
                r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                if (!r.Succeeded)
                    throw new WatsonException(r.FormattedMessages);
            }
            catch(Exception e)
            {
                Log.Error("Visual Recognition", "Detect text exception: {0}", e.ToString());
            }

            return text;
        }
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
            req.Parameters["version"] = VisualRecognitionVersion.Version;
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
            req.Parameters["version"] = VisualRecognitionVersion.Version;
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

            if(positiveExamplesData == null || negativeExamplesData == null)
                Log.Error("VisualRecognition", "Failed to upload {0} or {1}!", positiveExamplesPath, negativeExamplesPath);

            return UploadClassifier(classifierName, className, positiveExamplesData, negativeExamplesData, callback);
        }

        private bool UploadClassifier(string classifierName, string className, byte[] positiveExamplesData, byte[] negativeExamplesData, OnTrainClassifier callback)
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

        #region private methods
        private string GetMimeType(string imagePath)
        {
            string mimeType = "";
            switch(Path.GetExtension(imagePath))
            {
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".zip":
                    mimeType = "application/zip";
                    break;
                default:
                    throw new WatsonException("Cannot classify unsupported file format " + Path.GetExtension(imagePath) + ". Please use jpg, gif, png or zip!");
            }

            return mimeType;
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
            private VisualRecognition m_Service = null;
            private ServiceStatus m_Callback = null;
            private int m_GetClassifierCount = 0;
            private int m_ClassifyCount = 0;

            public CheckServiceStatus(VisualRecognition service, ServiceStatus callback)
            {
                m_Service = service;
                m_Callback = callback;

                if(!m_Service.GetClassifiers(OnCheckServices))
                    OnFailure("Failed to get classifiers!");
            }

            private void OnCheckServices(GetClassifiersTopLevelBrief classifiers)
            {
                if (m_Callback != null)
                {
                    if(classifiers != null)
                    {
                        if(classifiers.classifiers != null)
                        {
                            if (classifiers.classifiers.Length > 0)
                            {
                                //  Check first five classifiers - too many classifiers currently!
                                int numClassifiers = (classifiers.classifiers.Length > 5) ? 5 : classifiers.classifiers.Length;
                                for(int i = 0; i < numClassifiers; i++)
                                {
                                    if(!m_Service.GetClassifier(classifiers.classifiers[i].classifier_id, OnCheckService))
                                    {
//                                        OnFailure("Failed to call GetClassifier()");
                                    }
                                    else
                                    {
                                        m_GetClassifierCount += 1;
                                    }
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
                            Log.Debug("VisualRecognition", "Classifiers.classifiers is null!");
                        }
                    }
                    else
                    {
                        Log.Debug("VisualRecognition", "Classifiers in null!");
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

            private void OnCheckService(GetClassifiersPerClassifierVerbose classifier)
            {
                if (m_GetClassifierCount > 0)
                {
                    m_GetClassifierCount -= 1;
                    if (classifier != null)
                    {
                        Log.Debug("VisualRecognition", "classifier status: {0}", classifier.status);
                        if (classifier.status == "unavailable" || classifier.status == "failed")
                        {
//                            OnFailure(string.Format("Status of classifier {0} came back as {1}.",
//                                classifier.classifier_id, classifier.status));
                        }
                        else
                        {
                            // try to classify something with this classifier.
                            if (!m_Service.Classify(OnClassify, null, "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg"))
                            {
                                Log.Debug("VisualRecognition", "Failed to invoke Classify!");
                            }
                            else
                            {
                                m_ClassifyCount += 1;
                            }
                        }
                    }
                    else
                    {
                        OnFailure("Failed to get classifier.");
                    }
                }
            }

            private void OnClassify(ClassifyTopLevelMultiple result)
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
                    {
                        OnFailure("Failed to classify.");
                    }
                }
            }

            void OnFailure(string msg)
            {
                Log.Error("VisualRecognition", msg);
                if(m_Callback != null && m_Callback.Target != null)
                {
                    m_Callback(SERVICE_ID, false);
                }
            }
        }

        #endregion
    	
    }
}
