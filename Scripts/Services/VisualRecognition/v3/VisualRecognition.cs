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

using FullSerializer;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

namespace IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3
{
    /// <summary>
    /// This class wraps the Visual Recognition service.
    /// <a href="http://www.ibm.com/watson/developercloud/visual-recognition.html">Visual Recognition Service</a>
    /// </summary>
    public class VisualRecognition : IWatsonService
    {
        #region Public Types
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
        private const string ServiceId = "VisualRecognitionV3";
        private const string ClassifyEndpoint = "/v3/classify";
        private const string DetectFacesEndpoint = "/v3/detect_faces";
        private const string RecognizeTextEndpoint = "/v3/recognize_text";
        private const string ClassifiersEndpoint = "/v3/classifiers";
        private const string CollectionsEndpoint = "/v3/collections";
        private const string CollectionEndpoint = "/v3/collections/{0}";
        private const string ImagesEndpoint = "/v3/collections/{0}/images";
        private const string ImageEndpoint = "/v3/collections/{0}/images/{1}";
        private const string MetadataEndpoint = "/v3/collections/{0}/images/{1}/metadata";
        private const string FindSimilarEndpoint = "/v3/collections/{0}/find_similar";
        private string _apikey = null;
        private fsSerializer _serializer = new fsSerializer();
        private Credentials _credentials = null;
        private string _url = "https://gateway.watsonplatform.net/tone-analyzer/api";
        private string _versionDate;
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
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get
            {
                if (string.IsNullOrEmpty(_versionDate))
                    throw new ArgumentNullException("VersionDate cannot be null. Use VersionDate `2016-05-20`");

                return _versionDate;
            }
            set { _versionDate = value; }
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
        public VisualRecognition(Credentials credentials)
        {
            if (credentials.HasApiKey())
            {
                Credentials = credentials;
            }
            else
            {
                throw new WatsonException("Please provide an apikey to use the Visual Recognition service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }
        #endregion

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

        #region Classify Image
        /// <summary>
        /// Classifies image specified by URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">Classifier IDs to be classified against.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accept language.</param>
        /// <param name="customData">Custom data.</param>
        public bool Classify(string url, SuccessCallback<ClassifyTopLevelMultiple> successCallback, FailCallback failCallback, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifyEndpoint);
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnClassifyResp;
            req.Headers["Accepted-Language"] = acceptLanguage;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VersionDate;
            if (owners != default(string[]))
                req.Parameters["owners"] = string.Join(",", owners);
            if (classifierIDs != default(string[]))
                req.Parameters["classifier_ids"] = string.Join(",", classifierIDs);
            if (threshold != default(float))
                req.Parameters["threshold"] = threshold;

            return connector.Send(req);
        }

        /// <summary>
        /// Classifies an image from the file system.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="imagePath">Image path.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">Classifier I ds.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accept language.</param>
        /// <param name="customData">Custom data.</param>
        public bool Classify(SuccessCallback<ClassifyTopLevelMultiple> successCallback, FailCallback failCallback, string imagePath, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("Define an image path to classify!");

            byte[] imageData = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                if (LoadFile != null)
                {
                    imageData = LoadFile(imagePath);
                }
                else
                {
#if !UNITY_WEBPLAYER
                    imageData = File.ReadAllBytes(imagePath);
#endif
                }

                if (imageData == null)
                    Log.Error("VisualRecognition.Classify()", "Failed to upload {0}!", imagePath);
            }

            return Classify(successCallback, failCallback, imageData, owners, classifierIDs, threshold, acceptLanguage, customData);
        }

        /// <summary>
        /// Classifies an image using byte data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="imageData">Byte array of image data.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">An array of classifier identifiers.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accepted language.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool Classify(SuccessCallback<ClassifyTopLevelMultiple> successCallback, FailCallback failCallback, byte[] imageData, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (imageData == null)
                throw new ArgumentNullException("Image data is required to classify!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifyEndpoint);
            if (connector == null)
                return false;
            ClassifyReq req = new ClassifyReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnClassifyResp;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            req.Headers["Accept-Language"] = acceptLanguage;

            if (owners != default(string[]))
                req.Parameters["owners"] = string.Join(",", owners);
            if (classifierIDs != default(string[]))
                req.Parameters["classifier_ids"] = string.Join(",", classifierIDs);
            if (threshold != default(float))
                req.Parameters["threshold"] = threshold;

            if (imageData != null)
                req.Send = imageData;

            return connector.Send(req);
        }

        /// <summary>
        /// The Classify request
        /// </summary>
        public class ClassifyReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<ClassifyTopLevelMultiple> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            ClassifyTopLevelMultiple result = null;
            fsData data = null;
            Dictionary<string, object> customData = ((ClassifyReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    result = new ClassifyTopLevelMultiple();

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("VisualRecognition.OnClassifyResp()", "Classify exception: {0}", e.ToString());
                }
            }

            if (resp.Success)
            {
                if (((ClassifyReq)req).SuccessCallback != null)
                    ((ClassifyReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((ClassifyReq)req).FailCallback != null)
                    ((ClassifyReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Detect Faces
        /// <summary>
        /// Detects faces in a given image URL.
        /// </summary>
        /// <returns><c>true</c>, if faces was detected, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFaces(string url, SuccessCallback<FacesTopLevelMultiple> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DetectFacesEndpoint);
            if (connector == null)
                return false;

            DetectFacesReq req = new DetectFacesReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnDetectFacesResp;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VersionDate;

            return connector.Send(req);
        }

        /// <summary>
        /// Detects faces in a jpg, gif, png or zip file.
        /// </summary>
        /// <returns><c>true</c>, if faces was detected, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="imagePath">Image path.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFaces(SuccessCallback<FacesTopLevelMultiple> successCallback, FailCallback failCallback, string imagePath, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("Define an image path to classify!");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            byte[] imageData = null;
            if (imagePath != default(string))
            {
                if (LoadFile != null)
                {
                    imageData = LoadFile(imagePath);
                }
                else
                {
#if !UNITY_WEBPLAYER
                    imageData = File.ReadAllBytes(imagePath);
#endif
                }

                if (imageData == null)
                    Log.Error("VisualRecognition.DetectFaces()", "Failed to upload {0}!", imagePath);
            }

            return DetectFaces(successCallback, failCallback, imageData, customData);
        }

        /// <summary>
        /// Detect faces in an image's byteData.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="imageData">ByteArray of image data.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool DetectFaces(SuccessCallback<FacesTopLevelMultiple> successCallback, FailCallback failCallback, byte[] imageData = default(byte[]), Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (imageData == null)
                throw new ArgumentNullException("Image data is required to DetectFaces!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DetectFacesEndpoint);
            if (connector == null)
                return false;
            DetectFacesReq req = new DetectFacesReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnDetectFacesResp;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;

            if (imageData != null)
            {
                req.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                req.Send = imageData;
            }

            return connector.Send(req);
        }

        /// <summary>
        /// The DetectFaces request
        /// </summary>
        public class DetectFacesReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<FacesTopLevelMultiple> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDetectFacesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            FacesTopLevelMultiple result = null;
            fsData data = null;
            Dictionary<string, object> customData = ((DetectFacesReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    result = new FacesTopLevelMultiple();

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("VisualRecognition.OnDetectFacesResp()", "Detect faces exception: {0}", e.ToString());
                }
            }

            if (resp.Success)
            {
                if (((DetectFacesReq)req).SuccessCallback != null)
                    ((DetectFacesReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((DetectFacesReq)req).FailCallback != null)
                    ((DetectFacesReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Classifiers
        /// <summary>
        /// Gets a list of all classifiers.
        /// </summary>
        /// <returns><c>true</c>, if classifiers was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="customData">CustomData.</param>
        public bool GetClassifiers(SuccessCallback<GetClassifiersTopLevelBrief> successCallback, FailCallback failCallback, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint);
            if (connector == null)
                return false;

            GetClassifiersReq req = new GetClassifiersReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetClassifiersResp;

            return connector.Send(req);
        }

        /// <summary>
        /// The GetClassifier request.
        /// </summary>
        public class GetClassifiersReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetClassifiersTopLevelBrief> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersTopLevelBrief result = new GetClassifiersTopLevelBrief();
            fsData data = null;
            Dictionary<string, object> customData = ((GetClassifiersReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);

                    object obj = result;
                    r = _serializer.TryDeserialize(data, obj.GetType(), ref obj);

                    if (!r.Succeeded)
                        throw new WatsonException(r.FormattedMessages);

                    customData.Add("json", data);
                }
                catch (Exception e)
                {
                    Log.Error("VisualRecognition.OnGetClassifiersResp()", "GetClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetClassifiersReq)req).SuccessCallback != null)
                    ((GetClassifiersReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetClassifiersReq)req).FailCallback != null)
                    ((GetClassifiersReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region Get Classifier
        /// <summary>
        /// Gets a classifier by classifierId.
        /// </summary>
        /// <returns><c>true</c>, if classifier was gotten, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">Classifier identifier.</param>
        public bool GetClassifier(SuccessCallback<GetClassifiersPerClassifierVerbose> successCallback, FailCallback failCallback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierId);
            if (connector == null)
                return false;

            GetClassifierReq req = new GetClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetClassifierResp;

            return connector.Send(req);
        }

        public class GetClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetClassifiersPerClassifierVerbose> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersPerClassifierVerbose result = new GetClassifiersPerClassifierVerbose();
            fsData data = null;
            Dictionary<string, object> customData = ((GetClassifierReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
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
                    Log.Error("VisualRecognition.OnGetClassifierResp()", "GetClassifier Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }

            if (resp.Success)
            {
                if (((GetClassifierReq)req).SuccessCallback != null)
                    ((GetClassifierReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((GetClassifierReq)req).FailCallback != null)
                    ((GetClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region TrainClassifier
        /// <summary>
        /// Trains a classifier. Training requires a total of at least two zip files: Two positive example zips with 
        /// at least 10 positive examples each or one zip file of 10 positive examples and a zip file of 10 negative examples.
        /// The total size of all files must be below 256mb. Additional training can be done by using UpdateClassifier.
        /// </summary>
        /// <returns><c>true</c>, if classifier was trained, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamples">Dictionary of class name and positive example paths.</param>
        /// <param name="negativeExamplesPath">Negative example file path.</param>
        /// <param name="mimeType">Mime type of the positive examples and negative examples data. Use GetMimeType to get Mimetype from filename.</param>
        public bool TrainClassifier(SuccessCallback<GetClassifiersPerClassifierVerbose> successCallback, FailCallback failCallback, string classifierName, Dictionary<string, string> positiveExamples, string negativeExamplesPath = default(string), string mimeType = "application/zip", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamples == null)
                throw new ArgumentNullException("Need at least one positive example!");
            if (positiveExamples.Count < 2 && string.IsNullOrEmpty(negativeExamplesPath))
                throw new ArgumentNullException("At least two positive example zips or one positive example zip and one negative example zip are required to train a classifier!");

            Dictionary<string, byte[]> positiveExamplesData = new Dictionary<string, byte[]>();
            byte[] negativeExamplesData = null;

            if (LoadFile != null)
            {
                foreach (KeyValuePair<string, string> kv in positiveExamples)
                    positiveExamplesData.Add(kv.Key, LoadFile(kv.Value));
                negativeExamplesData = LoadFile(negativeExamplesPath);
            }
            else
            {
#if !UNITY_WEBPLAYER
                foreach (KeyValuePair<string, string> kv in positiveExamples)
                    positiveExamplesData.Add(kv.Key, File.ReadAllBytes(kv.Value));
                negativeExamplesData = File.ReadAllBytes(negativeExamplesPath);
#endif
            }

            if (positiveExamplesData.Count == 0 || negativeExamplesData == null)
                Log.Error("VisualRecognition.TrainClassifier()", "Failed to upload positive or negative examples!");

            return TrainClassifier(successCallback, failCallback, classifierName, positiveExamplesData, negativeExamplesData, mimeType, customData);
        }

        /// <summary>
        /// Trains a classifier  
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamplesData">Dictionary of class name and class training zip or image byte data.</param>
        /// <param name="negativeExamplesData">Negative examples zip or image byte data.</param>
        /// <param name="mimeType">Mime type of the positive examples and negative examples data.</param>
        /// <returns></returns>
        public bool TrainClassifier(SuccessCallback<GetClassifiersPerClassifierVerbose> successCallback, FailCallback failCallback, string classifierName, Dictionary<string, byte[]> positiveExamplesData, byte[] negativeExamplesData = null, string mimeType = "application/zip", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamplesData.Count < 2 && negativeExamplesData == null)
                throw new ArgumentNullException("At least two positive example zips or one positive example zip and one negative example zip are required to train a classifier!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint);
            if (connector == null)
                return false;

            TrainClassifierReq req = new TrainClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnTrainClassifierResp;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["name"] = new RESTConnector.Form(classifierName);

            foreach (KeyValuePair<string, byte[]> kv in positiveExamplesData)
                req.Forms[kv.Key + "_positive_examples"] = new RESTConnector.Form(kv.Value, kv.Key + "_positive_examples" + GetExtension(mimeType), mimeType);
            if (negativeExamplesData != null)
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamplesData, "negative_examples" + GetExtension(mimeType), mimeType);

            return connector.Send(req);
        }

        /// <summary>
        /// The TrainClassifier request.
        /// </summary>
        public class TrainClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<GetClassifiersPerClassifierVerbose> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            GetClassifiersPerClassifierVerbose result = new GetClassifiersPerClassifierVerbose();
            fsData data = null;
            Dictionary<string, object> customData = ((TrainClassifierReq)req).CustomData;

            if (resp.Success)
            {
                try
                {
                    fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
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
                    Log.Error("VisualRecognition.OnTrainClassifierResp()", "TrainClassifiers Exception: {0}", e.ToString());
                    resp.Success = false;
                }
            }
            
            if (resp.Success)
            {
                if (((TrainClassifierReq)req).SuccessCallback != null)
                    ((TrainClassifierReq)req).SuccessCallback(result, customData);
            }
            else
            {
                if (((TrainClassifierReq)req).FailCallback != null)
                    ((TrainClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region UpdateClassifier
        /// <summary>
        /// Updates a trained classifier. The total size of all files must be below 256mb.
        /// </summary>
        /// <returns><c>true</c>, if classifier was updated, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierID">Classifier identifier.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamples">Dictionary of class name and positive example paths.</param>
        /// <param name="negativeExamplesPath">Negative example file path.</param>
        /// <param name="mimeType">Mimetype of the file. Use GetMimeType to get Mimetype from filename.</param>
        public bool UpdateClassifier(SuccessCallback<GetClassifiersPerClassifierVerbose> successCallback, FailCallback failCallback, string classifierID, string classifierName, Dictionary<string, string> positiveExamples, string negativeExamplesPath = default(string), string mimeType = "application/zip", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamples.Count == 0 && string.IsNullOrEmpty(negativeExamplesPath))
                throw new ArgumentNullException("Need at least one positive example or one negative example!");

            Dictionary<string, byte[]> positiveExamplesData = new Dictionary<string, byte[]>();
            byte[] negativeExamplesData = null;

            if (LoadFile != null)
            {
                foreach (KeyValuePair<string, string> kv in positiveExamples)
                    positiveExamplesData.Add(kv.Key, LoadFile(kv.Value));

                if (!string.IsNullOrEmpty(negativeExamplesPath))
                    negativeExamplesData = LoadFile(negativeExamplesPath);
            }
            else
            {
#if !UNITY_WEBPLAYER
                foreach (KeyValuePair<string, string> kv in positiveExamples)
                    positiveExamplesData.Add(kv.Key, File.ReadAllBytes(kv.Value));

                if (!string.IsNullOrEmpty(negativeExamplesPath))
                    negativeExamplesData = File.ReadAllBytes(negativeExamplesPath);
#endif
            }

            if (positiveExamplesData.Count == 0 && negativeExamplesData == null)
                Log.Error("VisualRecognition.UpdateClassifier()", "Failed to upload positive or negative examples!");

            return UpdateClassifier(successCallback, failCallback, classifierID, classifierName, positiveExamplesData, negativeExamplesData, mimeType, customData);
        }

        /// <summary>
        /// Updates a classifier using byte data.
        /// </summary>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierID">Classifier identifier.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamplesData">Dictionary of class name and class training zip or image byte data.</param>
        /// <param name="negativeExamplesData">Negative examples zip or image byte data.</param>
        /// <param name="mimeType">Mimetype of the file. Use GetMimeType to get Mimetype from filename.</param>
        /// <returns></returns>
        public bool UpdateClassifier(SuccessCallback<GetClassifiersPerClassifierVerbose> successCallback, FailCallback failCallback, string classifierID, string classifierName, Dictionary<string, byte[]> positiveExamplesData, byte[] negativeExamplesData = null, string mimeType = "application/zip", Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamplesData.Count == 0 && negativeExamplesData == null)
                throw new ArgumentNullException("Need at least one positive example or one negative example!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierID);
            if (connector == null)
                return false;

            TrainClassifierReq req = new TrainClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.OnResponse = OnTrainClassifierResp;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["name"] = new RESTConnector.Form(classifierName);

            foreach (KeyValuePair<string, byte[]> kv in positiveExamplesData)
                req.Forms[kv.Key + "_positive_examples"] = new RESTConnector.Form(kv.Value, kv.Key + "_positive_examples" + GetExtension(mimeType), mimeType);
            if (negativeExamplesData != null)
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamplesData, "negative_examples" + GetExtension(mimeType), mimeType);

            return connector.Send(req);
        }
        #endregion

        #region Delete Classifier
        /// <summary>
        /// Deletes the classifier by classifierID.
        /// </summary>
        /// <returns><c>true</c>, if classifier was deleted, <c>false</c> otherwise.</returns>
        /// <param name="successCallback">The success callback.</param>
        /// <param name="failCallback">The fail callback.</param>
        /// <param name="classifierId">Classifier identifier.</param>
        public bool DeleteClassifier(SuccessCallback<bool> successCallback, FailCallback failCallback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (successCallback == null)
                throw new ArgumentNullException("successCallback");
            if (failCallback == null)
                throw new ArgumentNullException("failCallback");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierId);
            if (connector == null)
                return false;

            DeleteClassifierReq req = new DeleteClassifierReq();
            req.SuccessCallback = successCallback;
            req.FailCallback = failCallback;
            req.CustomData = customData == null ? new Dictionary<string, object>() : customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteClassifierResp;
            req.Delete = true;

            return connector.Send(req);
        }

        public class DeleteClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// The success callback.
            /// </summary>
            public SuccessCallback<bool> SuccessCallback { get; set; }
            /// <summary>
            /// The fail callback.
            /// </summary>
            public FailCallback FailCallback { get; set; }
            /// <summary>
            /// Custom data.
            /// </summary>
            public Dictionary<string, object> CustomData { get; set; }
        }

        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            Dictionary<string, object> customData = ((DeleteClassifierReq)req).CustomData;

            if (resp.Success)
            {
                customData.Add("json", "code: " + resp.HttpResponseCode + ", success: " + resp.Success);

                if (((DeleteClassifierReq)req).SuccessCallback != null)
                    ((DeleteClassifierReq)req).SuccessCallback(resp.Success, customData);
            }
            else
            {
                if (((DeleteClassifierReq)req).FailCallback != null)
                    ((DeleteClassifierReq)req).FailCallback(resp.Error, customData);
            }
        }
        #endregion

        #region private methods
        private string GetMimeType(string imagePath)
        {
            string mimeType = "";
            switch (Path.GetExtension(imagePath))
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

        private string GetMetadataJson(Dictionary<string, string> metadata)
        {
            string json = "{";
            string metadataItem = "\n\t\"{0}\":\"{1}\"";

            int i = 0;
            foreach (KeyValuePair<string, string> kv in metadata)
            {
                i++;
                string comma = i < metadata.Count ? "," : "";
                json += string.Format(metadataItem, kv.Key, kv.Value) + comma;
            }

            json += "\n}";

            return json;
        }

        private string GetExtension(string mimeType)
        {
            string extension = "";
            switch (mimeType)
            {
                case "image/jpeg":
                    extension = ".jpg";
                    break;
                case "image/png":
                    extension = ".png";
                    break;
                case "image/gif":
                    extension = ".gif";
                    break;
                case "application/zip":
                    extension = ".zip";
                    break;
                default:
                    throw new WatsonException("Cannot classify unsupported mime type " + mimeType);
            }

            return extension;
        }
        #endregion

        #region IWatsonService implementation
        /// <exclude />
        public string GetServiceID()
        {
            return ServiceId;
        }
        #endregion
    }
}
