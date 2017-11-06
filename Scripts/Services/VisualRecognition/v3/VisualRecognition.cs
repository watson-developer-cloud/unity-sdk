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
        /// The callback used by the GetClassifiers() method.
        /// </summary>
        public delegate void OnGetClassifiers(RESTConnector.ParsedResponse<GetClassifiersTopLevelBrief> resp);
        /// <summary>
        /// Callback used by the GetClassifier() method.
        /// </summary>
        public delegate void OnGetClassifier(RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> resp);
        /// <summary>
        /// This callback is used by the DeleteClassifier() method.
        /// </summary>
        public delegate void OnDeleteClassifier(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// Callback used by the TrainClassifier() method.
        /// </summary>
        public delegate void OnTrainClassifier(RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> resp);
        /// <summary>
        /// This callback is used by the Classify() method.
        /// </summary>
        public delegate void OnClassify(RESTConnector.ParsedResponse<ClassifyTopLevelMultiple> resp);
        /// <summary>
        /// This callback is used by the DetectFaces() method.
        /// </summary>
        public delegate void OnDetectFaces(RESTConnector.ParsedResponse<FacesTopLevelMultiple> resp);
        /// <summary>
        /// This callback is used by the RecognizeText() method.
        /// </summary>
        public delegate void OnRecognizeText(RESTConnector.ParsedResponse<TextRecogTopLevelMultiple> resp);
        /// <summary>
        /// This callback is used by the GetCollections() method.
        /// </summary>
        public delegate void OnGetCollections(RESTConnector.ParsedResponse<GetCollections> resp);
        /// <summary>
        /// This callback is used by the CreateCollection() method.
        /// </summary>
        public delegate void OnCreateCollection(RESTConnector.ParsedResponse<CreateCollection> resp);
        /// <summary>
        /// This callback is used by the DeleteCollection() method.
        /// </summary>
        public delegate void OnDeleteCollection(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// This callback is used y the GetCollection() method.
        /// </summary>
        public delegate void OnGetCollection(RESTConnector.ParsedResponse<CreateCollection> resp);
        /// <summary>
        /// This callback is used by the GetCollectionImages() method.
        /// </summary>
        public delegate void OnGetCollectionImages(RESTConnector.ParsedResponse<GetCollectionImages> resp);
        /// <summary>
        /// This callback is used by the AddCollectionImage() method.
        /// </summary>
        public delegate void OnAddCollectionImage(RESTConnector.ParsedResponse<CollectionsConfig> resp);
        /// <summary>
        /// This callback is used by the DeleteCollectionImage() method.
        /// </summary>
        public delegate void OnDeleteCollectionImage(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// This callback is used by the GetImageDetails() method.
        /// </summary>
        public delegate void OnGetImageDetails(RESTConnector.ParsedResponse<GetCollectionsBrief> resp);
        /// <summary>
        /// This callback is used by the DeleteImageMetadata() method.
        /// </summary>
        public delegate void OnDeleteImageMetadata(RESTConnector.ParsedResponse<object> resp);
        /// <summary>
        /// This callback is used by the GetImageMetadata() method.
        /// </summary>
        public delegate void OnGetImageMetadata(RESTConnector.ParsedResponse<GetCollectionsBrief> resp);
        /// <summary>
        /// This callback is used by the FindSimilar() method.
        /// </summary>
        public delegate void OnFindSimilar(RESTConnector.ParsedResponse<SimilarImagesConfig> resp);

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
        private const float REQUEST_TIMEOUT = 10.0f * 60.0f;
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

        #region Classify Image
        /// <summary>
        /// Classifies image specified by URL.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">Classifier IDs to be classified against.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accept language.</param>
        /// <param name="customData">Custom data.</param>
        public bool Classify(OnClassify callback, string url, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifyEndpoint);
            if (connector == null)
                return false;

            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.Data = customData;
            req.OnResponse = OnClassifyResp;
            req.Timeout = REQUEST_TIMEOUT;
            req.AcceptLanguage = acceptLanguage;
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
        /// <param name="callback">Callback.</param>
        /// <param name="imagePath">Image path.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">Classifier I ds.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accept language.</param>
        /// <param name="customData">Custom data.</param>
        public bool Classify(string imagePath, OnClassify callback, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (callback == null)
                throw new ArgumentNullException("callback");
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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return Classify(callback, imageData, owners, classifierIDs, threshold, acceptLanguage);
        }

        /// <summary>
        /// Classifies an image using byte data.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="imageData">Byte array of image data.</param>
        /// <param name="owners">Owners.</param>
        /// <param name="classifierIDs">An array of classifier identifiers.</param>
        /// <param name="threshold">Threshold.</param>
        /// <param name="acceptLanguage">Accepted language.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool Classify(OnClassify callback, byte[] imageData, string[] owners = default(string[]), string[] classifierIDs = default(string[]), float threshold = default(float), string acceptLanguage = "en", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (imageData == null)
                throw new ArgumentNullException("Image data is required to classify!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifyEndpoint);
            if (connector == null)
                return false;
            ClassifyReq req = new ClassifyReq();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnClassifyResp;
            req.AcceptLanguage = acceptLanguage;
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
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnClasify callback delegate.
            /// </summary>
            public OnClassify Callback { get; set; }
            /// <summary>
            /// Accept language string.
            /// </summary>
            public string AcceptLanguage { get; set; }
        }

        private void OnClassifyResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((ClassifyReq)req).Data;

            RESTConnector.ParsedResponse<ClassifyTopLevelMultiple> parsedResp = new RESTConnector.ParsedResponse<ClassifyTopLevelMultiple>(resp, customData, _serializer);

            if (((ClassifyReq)req).Callback != null)
                ((ClassifyReq)req).Callback(parsedResp);
        }
        #endregion

        #region Detect Faces
        /// <summary>
        /// Detects faces in a given image URL.
        /// </summary>
        /// <returns><c>true</c>, if faces was detected, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFaces(OnDetectFaces callback, string url, string customData = default(string))
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DetectFacesEndpoint);
            if (connector == null)
                return false;

            DetectFacesReq req = new DetectFacesReq();
            req.Callback = callback;
            req.Data = customData;
            req.OnResponse = OnDetectFacesResp;
            req.Timeout = REQUEST_TIMEOUT;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VersionDate;

            return connector.Send(req);
        }

        /// <summary>
        /// Detects faces in a jpg, gif, png or zip file.
        /// </summary>
        /// <returns><c>true</c>, if faces was detected, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="imagePath">Image path.</param>
        /// <param name="customData">Custom data.</param>
        public bool DetectFaces(string imagePath, OnDetectFaces callback, string customData = default(string))
        {
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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return DetectFaces(callback, imageData, customData);
        }

        /// <summary>
        /// Detect faces in an image's byteData.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="imageData">ByteArray of image data.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool DetectFaces(OnDetectFaces callback, byte[] imageData = default(byte[]), string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (imageData == null)
                throw new ArgumentNullException("Image data is required to DetectFaces!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, DetectFacesEndpoint);
            if (connector == null)
                return false;
            DetectFacesReq req = new DetectFacesReq();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
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
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnDetectFaces callback delegate.
            /// </summary>
            public OnDetectFaces Callback { get; set; }
        }

        private void OnDetectFacesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DetectFacesReq)req).Data;

            RESTConnector.ParsedResponse<FacesTopLevelMultiple> parsedResp = new RESTConnector.ParsedResponse<FacesTopLevelMultiple>(resp, customData, _serializer);

            if (((DetectFacesReq)req).Callback != null)
                ((DetectFacesReq)req).Callback(parsedResp);
        }
        #endregion

        #region Recognize Text
        /// <summary>
        /// Recognizes text given an image url.
        /// </summary>
        /// <returns><c>true</c>, if text was recognized, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        /// <param name="callback">Callback.</param>
        /// <param name="customData">Custom data.</param>
        public bool RecognizeText(OnRecognizeText callback, string url, string customData = default(string))
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, RecognizeTextEndpoint);
            if (connector == null)
                return false;

            RecognizeTextReq req = new RecognizeTextReq();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnRecognizeTextResp;
            req.Timeout = REQUEST_TIMEOUT;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["url"] = url;
            req.Parameters["version"] = VersionDate;

            return connector.Send(req);
        }

        /// <summary>
        /// Recognizes text in a given image.
        /// </summary>
        /// <returns><c>true</c>, if text was recognized, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="imagePath">Image path.</param>
        /// <param name="customData">Custom data.</param>
        public bool RecognizeText(string imagePath, OnRecognizeText callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("Define an image path to classify!");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API KEy was found!");

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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return RecognizeText(callback, imageData, customData);
        }

        /// <summary>
        /// Recognizes text in image bytedata.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="imageData">Image's byte array data.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns></returns>
        public bool RecognizeText(OnRecognizeText callback, byte[] imageData = default(byte[]), string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (imageData == null)
                throw new ArgumentNullException("Image data is required to RecognizeText!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, RecognizeTextEndpoint);
            if (connector == null)
                return false;
            RecognizeTextReq req = new RecognizeTextReq();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
            req.OnResponse = OnRecognizeTextResp;

            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;

            if (imageData != null)
                req.Send = imageData;

            return connector.Send(req);
        }

        public class RecognizeTextReq : RESTConnector.Request
        {
            /// <summary>
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnRecognizeText callback delegate.
            /// </summary>
            public OnRecognizeText Callback { get; set; }
        }

        private void OnRecognizeTextResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((RecognizeTextReq)req).Data;

            RESTConnector.ParsedResponse<TextRecogTopLevelMultiple> parsedResp = new RESTConnector.ParsedResponse<TextRecogTopLevelMultiple>(resp, customData, _serializer);

            if (((RecognizeTextReq)req).Callback != null)
                ((RecognizeTextReq)req).Callback(parsedResp);
        }
        #endregion
        
        #region Get Classifiers
        /// <summary>
        /// Gets a list of all classifiers.
        /// </summary>
        /// <returns><c>true</c>, if classifiers was gotten, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="customData">CustomData.</param>
        public bool GetClassifiers(OnGetClassifiers callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint);
            if (connector == null)
                return false;

            GetClassifiersReq req = new GetClassifiersReq();
            req.Callback = callback;
            req.Data = customData;
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
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnGetClassifier callback delegate.
            /// </summary>
            public OnGetClassifiers Callback { get; set; }
        }

        private void OnGetClassifiersResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetClassifiersReq)req).Data;

            RESTConnector.ParsedResponse<GetClassifiersTopLevelBrief> parsedResp = new RESTConnector.ParsedResponse<GetClassifiersTopLevelBrief>(resp, customData, _serializer);

            if (((GetClassifiersReq)req).Callback != null)
                ((GetClassifiersReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Classifier
        /// <summary>
        /// Gets a classifier by classifierId.
        /// </summary>
        /// <returns><c>true</c>, if classifier was gotten, <c>false</c> otherwise.</returns>
        /// <param name="classifierId">Classifier identifier.</param>
        /// <param name="callback">Callback.</param>
        public bool GetClassifier(OnGetClassifier callback, string classifierId, string customData = default(string))
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierId);
            if (connector == null)
                return false;

            GetClassifierReq req = new GetClassifierReq();
            req.Callback = callback;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnGetClassifierResp;

            return connector.Send(req);
        }

        public class GetClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// OnGetClassifier callback delegate
            /// </summary>
            public OnGetClassifier Callback { get; set; }
        }

        private void OnGetClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetClassifierReq)req).Data;

            RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> parsedResp = new RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose>(resp, customData, _serializer);

            if (((GetClassifierReq)req).Callback != null)
                ((GetClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region TrainClassifier
        /// <summary>
        /// Trains a classifier. Training requires a total of at least two zip files: Two positive example zips with 
        /// at least 10 positive examples each or one zip file of 10 positive examples and a zip file of 10 negative examples.
        /// The total size of all files must be below 256mb. Additional training can be done by using UpdateClassifier.
        /// </summary>
        /// <returns><c>true</c>, if classifier was trained, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamples">Dictionary of class name and positive example paths.</param>
        /// <param name="negativeExamplesPath">Negative example file path.</param>
        /// <param name="mimeType">Mime type of the positive examples and negative examples data. Use GetMimeType to get Mimetype from filename.</param>
        public bool TrainClassifier(OnTrainClassifier callback, string classifierName, Dictionary<string, string> positiveExamples, string negativeExamplesPath = default(string), string mimeType = "application/zip", string customData = default(string))
        {
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
            if (callback == null)
                throw new ArgumentNullException("callback");

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
                Log.Error("VisualRecognition", "Failed to upload positive or negative examples!");

            return TrainClassifier(callback, classifierName, positiveExamplesData, negativeExamplesData, mimeType, customData);
        }

        /// <summary>
        /// Trains a classifier  
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamplesData">Dictionary of class name and class training zip or image byte data.</param>
        /// <param name="negativeExamplesData">Negative examples zip or image byte data.</param>
        /// <param name="mimeType">Mime type of the positive examples and negative examples data.</param>
        /// <returns></returns>
        public bool TrainClassifier(OnTrainClassifier callback, string classifierName, Dictionary<string, byte[]> positiveExamplesData, byte[] negativeExamplesData = null, string mimeType = "application/zip", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamplesData.Count < 2 && negativeExamplesData == null)
                throw new ArgumentNullException("At least two positive example zips or one positive example zip and one negative example zip are required to train a classifier!");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint);
            if (connector == null)
                return false;

            TrainClassifierReq req = new TrainClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnTrainClassifierResp;
            req.Timeout = REQUEST_TIMEOUT;
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
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnTrainClassifier callback delegate.
            /// </summary>
            public OnTrainClassifier Callback { get; set; }
        }

        private void OnTrainClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((TrainClassifierReq)req).Data;

            RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose> parsedResp = new RESTConnector.ParsedResponse<GetClassifiersPerClassifierVerbose>(resp, customData, _serializer);

            if (((TrainClassifierReq)req).Callback != null)
                ((TrainClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region UpdateClassifier
        /// <summary>
        /// Updates a trained classifier. The total size of all files must be below 256mb.
        /// </summary>
        /// <returns><c>true</c>, if classifier was updated, <c>false</c> otherwise.</returns>
        /// <param name="callback">Callback.</param>
        /// <param name="classifierID">Classifier identifier.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamples">Dictionary of class name and positive example paths.</param>
        /// <param name="negativeExamplesPath">Negative example file path.</param>
        /// <param name="mimeType">Mimetype of the file. Use GetMimeType to get Mimetype from filename.</param>
        public bool UpdateClassifier(OnTrainClassifier callback, string classifierID, string classifierName, Dictionary<string, string> positiveExamples, string negativeExamplesPath = default(string), string mimeType = "application/zip", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamples.Count == 0 && string.IsNullOrEmpty(negativeExamplesPath))
                throw new ArgumentNullException("Need at least one positive example or one negative example!");
            if (callback == null)
                throw new ArgumentNullException("callback");

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
                Log.Error("VisualRecognition", "Failed to upload positive or negative examples!");

            return UpdateClassifier(callback, classifierID, classifierName, positiveExamplesData, negativeExamplesData, mimeType, customData);
        }

        /// <summary>
        /// Updates a classifier using byte data.
        /// </summary>
        /// <param name="callback">Callback.</param>
        /// <param name="classifierID">Classifier identifier.</param>
        /// <param name="classifierName">Classifier name.</param>
        /// <param name="positiveExamplesData">Dictionary of class name and class training zip or image byte data.</param>
        /// <param name="negativeExamplesData">Negative examples zip or image byte data.</param>
        /// <param name="mimeType">Mimetype of the file. Use GetMimeType to get Mimetype from filename.</param>
        /// <returns></returns>
        public bool UpdateClassifier(OnTrainClassifier callback, string classifierID, string classifierName, Dictionary<string, byte[]> positiveExamplesData, byte[] negativeExamplesData = null, string mimeType = "application/zip", string customData = default(string))
        {
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");
            if (string.IsNullOrEmpty(classifierName))
                throw new ArgumentNullException("ClassifierName");
            if (positiveExamplesData.Count == 0 && negativeExamplesData == null)
                throw new ArgumentNullException("Need at least one positive example or one negative example!");
            if (callback == null)
                throw new ArgumentNullException("callback");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierID);
            if (connector == null)
                return false;

            TrainClassifierReq req = new TrainClassifierReq();
            req.Callback = callback;
            req.OnResponse = OnTrainClassifierResp;
            req.Timeout = REQUEST_TIMEOUT;
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
        /// <param name="classifierId">Classifier identifier.</param>
        /// <param name="callback">Callback.</param>
        public bool DeleteClassifier(OnDeleteClassifier callback, string classifierId, string customData = default(string))
        {
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("classifierId");
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            Log.Debug("VisualRecognition", "Attempting to delete classifier {0}", classifierId);
            RESTConnector connector = RESTConnector.GetConnector(Credentials, ClassifiersEndpoint + "/" + classifierId);
            if (connector == null)
                return false;

            DeleteClassifierReq req = new DeleteClassifierReq();
            req.Callback = callback;
            req.Data = customData;
            req.Timeout = REQUEST_TIMEOUT;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.OnResponse = OnDeleteClassifierResp;
            req.Delete = true;

            return connector.Send(req);
        }

        public class DeleteClassifierReq : RESTConnector.Request
        {
            /// <summary>
            /// Custom data.
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// The OnDeleteClassifier callback delegate.
            /// </summary>
            public OnDeleteClassifier Callback { get; set; }
        }

        private void OnDeleteClassifierResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteClassifierReq)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteClassifierReq)req).Callback != null)
                ((DeleteClassifierReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Collections
        /// <summary>
        /// Get all collections.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool GetCollections(OnGetCollections callback, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, CollectionsEndpoint);
            if (connector == null)
                return false;

            GetCollectionsReq req = new GetCollectionsReq();
            req.Callback = callback;
            req.Data = customData;

            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetCollectionsResp;

            return connector.Send(req);
        }

        private class GetCollectionsReq : RESTConnector.Request
        {
            /// <summary>
            /// OnGetCollections callback.
            /// </summary>
            public OnGetCollections Callback { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnGetCollectionsResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionsReq)req).Data;

            RESTConnector.ParsedResponse<GetCollections> parsedResp = new RESTConnector.ParsedResponse<GetCollections>(resp, customData, _serializer);

            if (((GetCollectionsReq)req).Callback != null)
                ((GetCollectionsReq)req).Callback(parsedResp);
        }
        #endregion

        #region Create collection
        /// <summary>
        /// Create a new collection of images to search. You can create a maximum of 5 collections.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="name">The name of the created collection.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool CreateCollection(OnCreateCollection callback, string name, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, CollectionsEndpoint);
            if (connector == null)
                return false;

            CreateCollectionReq req = new CreateCollectionReq();
            req.Callback = callback;
            req.Name = name;
            req.Data = customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["name"] = new RESTConnector.Form(name);
            req.Forms["disregard"] = new RESTConnector.Form(new byte[4]);

            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnCreateCollectionResp;

            return connector.Send(req);
        }

        private class CreateCollectionReq : RESTConnector.Request
        {
            /// <summary>
            /// OnCreateCollection callback.
            /// </summary>
            public OnCreateCollection Callback { get; set; }
            /// <summary>
            /// Name of the collection to create.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnCreateCollectionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((CreateCollectionReq)req).Data;

            RESTConnector.ParsedResponse<CreateCollection> parsedResp = new RESTConnector.ParsedResponse<CreateCollection>(resp, customData, _serializer);

            if (((CreateCollectionReq)req).Callback != null)
                ((CreateCollectionReq)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Collection
        /// <summary>
        /// Deletes a collection.
        /// </summary>
        /// <param name="callback">The OnDeleteCollection callback.</param>
        /// <param name="collectionID">The collection identifier to delete.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool DeleteCollection(OnDeleteCollection callback, string collectionID, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CollectionEndpoint, collectionID));
            if (connector == null)
                return false;

            DeleteCollectionReq req = new DeleteCollectionReq();
            req.Callback = callback;
            req.CollectionID = collectionID;
            req.Data = customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Delete = true;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnDeleteCollectionResp;

            return connector.Send(req);
        }

        private class DeleteCollectionReq : RESTConnector.Request
        {
            /// <summary>
            /// OnDeleteCollection callback.
            /// </summary>
            public OnDeleteCollection Callback { get; set; }
            /// <summary>
            /// Collection identifier of the collection to be deleted.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnDeleteCollectionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCollectionReq)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCollectionReq)req).Callback != null)
                ((DeleteCollectionReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Collection
        /// <summary>
        /// Retrieve information about a specific collection. Only user-created collections can be specified.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The requested collection identifier.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool GetCollection(OnGetCollection callback, string collectionID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException(collectionID);
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(CollectionEndpoint, collectionID));
            if (connector == null)
                return false;

            GetCollectionReq req = new GetCollectionReq();
            req.Callback = callback;
            req.Data = customData;
            req.CollectionID = collectionID;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetCollectionResp;

            return connector.Send(req);
        }

        private class GetCollectionReq : RESTConnector.Request
        {
            /// <summary>
            /// OnGetCollections callback.
            /// </summary>
            public OnGetCollection Callback { get; set; }
            /// <summary>
            /// Collection identifier of the requested collection.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnGetCollectionResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionReq)req).Data;

            RESTConnector.ParsedResponse<CreateCollection> parsedResp = new RESTConnector.ParsedResponse<CreateCollection>(resp, customData, _serializer);

            if (((GetCollectionReq)req).Callback != null)
                ((GetCollectionReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Collection Images
        /// <summary>
        /// List 100 images in a collection
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The requested collection identifier.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool GetCollectionImages(OnGetCollectionImages callback, string collectionID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException(collectionID);
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(ImagesEndpoint, collectionID));
            if (connector == null)
                return false;

            GetCollectionImagesReq req = new GetCollectionImagesReq();
            req.Callback = callback;
            req.Data = customData;
            req.CollectionID = collectionID;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetCollectionImagesResp;

            return connector.Send(req);
        }

        private class GetCollectionImagesReq : RESTConnector.Request
        {
            /// <summary>
            /// OnGetCollections callback.
            /// </summary>
            public OnGetCollectionImages Callback { get; set; }
            /// <summary>
            /// Collection identifier of the requested collection.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnGetCollectionImagesResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionImagesReq)req).Data;

            RESTConnector.ParsedResponse<GetCollectionImages> parsedResp = new RESTConnector.ParsedResponse<GetCollectionImages>(resp, customData, _serializer);

            if (((GetCollectionImagesReq)req).Callback != null)
                ((GetCollectionImagesReq)req).Callback(parsedResp);
        }
        #endregion

        #region Add Collection Images
        /// <summary>
        /// Add an image to a collection via image path on file system and metadata as dictionary.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The identifier of the collection to add images to.</param>
        /// <param name="imagePath">The path in the filesystem of the image to add.</param>
        /// <param name="metadata">Optional Dictionary key value pairs of metadata associated with the specified image.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool AddCollectionImage(OnAddCollectionImage callback, string collectionID, string imagePath, Dictionary<string, string> metadata = null, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("imagePath");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return AddCollectionImage(callback, collectionID, imageData, Path.GetFileName(imagePath), GetMetadataJson(metadata), customData);
        }

        /// <summary>
        /// Add an image to a collection via image path on file system and metadata path on file system.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The identifier of the collection to add images to.</param>
        /// <param name="imagePath">The path in the filesystem of the image to add.</param>
        /// <param name="metadataPath">Optional path to metadata json associated with the specified image.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool AddCollectionImage(OnAddCollectionImage callback, string collectionID, string imagePath, string metadataPath = null, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("imagePath");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            string metadata = null;
            if (!string.IsNullOrEmpty(metadataPath))
            {
                metadata = File.ReadAllText(metadataPath);

                if (string.IsNullOrEmpty(metadata))
                    Log.Error("VisualRecognition", "Failed to read {0}!", imagePath);
            }

            return AddCollectionImage(callback, collectionID, imageData, Path.GetFileName(imagePath), metadata, customData);
        }

        /// <summary>
        /// Add an image to a collection.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The identifier of the collection to add images to.</param>
        /// <param name="imageData">The byte[] data of the image to add.</param>
        /// <param name="metadata">Optional json metadata associated with the specified image.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool AddCollectionImage(OnAddCollectionImage callback, string collectionID, byte[] imageData, string filename, string metadata = null, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (imageData == default(byte[]))
                throw new WatsonException("Image data is required!");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(ImagesEndpoint, collectionID));
            if (connector == null)
                return false;

            AddCollectionImageReq req = new AddCollectionImageReq();
            req.Callback = callback;
            req.CollectionID = collectionID;
            req.ImageData = imageData;
            req.Metadata = metadata;
            req.Data = customData;

            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["image_file"] = new RESTConnector.Form(imageData, filename, GetMimeType(filename));
            req.Forms["metadata"] = new RESTConnector.Form(Encoding.UTF8.GetBytes(metadata), "application/json");

            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnAddCollectionImageResp;

            return connector.Send(req);
        }

        private class AddCollectionImageReq : RESTConnector.Request
        {
            /// <summary>
            /// OnCreateCollection callback.
            /// </summary>
            public OnAddCollectionImage Callback { get; set; }
            /// <summary>
            /// The collection identifier to add images to.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Byte array of Image Data to add to the collection.
            /// </summary>
            public byte[] ImageData { get; set; }
            /// <summary>
            /// Json metadata associated with this image.
            /// </summary>
            public string Metadata { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnAddCollectionImageResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((AddCollectionImageReq)req).Data;

            RESTConnector.ParsedResponse<CollectionsConfig> parsedResp = new RESTConnector.ParsedResponse<CollectionsConfig>(resp, customData, _serializer);

            if (((AddCollectionImageReq)req).Callback != null)
                ((AddCollectionImageReq)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Image
        /// <summary>
        /// Deletes an image from a collection.
        /// </summary>
        /// <param name="callback">The OnDeleteCollection callback.</param>
        /// <param name="collectionID">The collection identifier holding the image to delete.</param>
        /// <param name="imageID">The identifier of the image to delete.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool DeleteCollectionImage(OnDeleteCollectionImage callback, string collectionID, string imageID, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(imageID))
                throw new ArgumentNullException("imageID");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(ImageEndpoint, collectionID, imageID));
            if (connector == null)
                return false;

            DeleteCollectionImageReq req = new DeleteCollectionImageReq();
            req.Callback = callback;
            req.CollectionID = collectionID;
            req.ImageID = imageID;
            req.Data = customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Delete = true;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnDeleteCollectionImageResp;

            return connector.Send(req);
        }

        private class DeleteCollectionImageReq : RESTConnector.Request
        {
            /// <summary>
            /// OnDeleteCollection callback.
            /// </summary>
            public OnDeleteCollectionImage Callback { get; set; }
            /// <summary>
            /// Collection identifier containing the image to be deleted.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// The identifier of the image to be deleted.
            /// </summary>
            public string ImageID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnDeleteCollectionImageResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCollectionImageReq)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCollectionImageReq)req).Callback != null)
                ((DeleteCollectionImageReq)req).Callback(parsedResp);
        }
        #endregion

        #region Get Image
        /// <summary>
        /// List an image's details.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The requested collection identifier.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool GetImage(OnGetImageDetails callback, string collectionID, string imageID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException(collectionID);
            if (string.IsNullOrEmpty(imageID))
                throw new ArgumentNullException(imageID);
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(ImageEndpoint, collectionID, imageID));
            if (connector == null)
                return false;

            GetCollectionImageReq req = new GetCollectionImageReq();
            req.Callback = callback;
            req.Data = customData;
            req.CollectionID = collectionID;
            req.ImageID = imageID;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetCollectionImageResp;

            return connector.Send(req);
        }

        private class GetCollectionImageReq : RESTConnector.Request
        {
            /// <summary>
            /// OnGetCollections callback.
            /// </summary>
            public OnGetImageDetails Callback { get; set; }
            /// <summary>
            /// Collection identifier of the requested collection.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Image identifier for the requested collection.
            /// </summary>
            public string ImageID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnGetCollectionImageResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionImageReq)req).Data;

            RESTConnector.ParsedResponse<GetCollectionsBrief> parsedResp = new RESTConnector.ParsedResponse<GetCollectionsBrief>(resp, customData, _serializer);

            if (((GetCollectionImageReq)req).Callback != null)
                ((GetCollectionImageReq)req).Callback(parsedResp);
        }
        #endregion

        #region Delete Image Metadata
        /// <summary>
        /// Deletes an image metadata.
        /// </summary>
        /// <param name="callback">The Callback.</param>
        /// <param name="collectionID">The collection identifier holding the image metadata to delete.</param>
        /// <param name="imageID">The identifier of the image metadata to delete.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool DeleteCollectionImageMetadata(OnDeleteImageMetadata callback, string collectionID, string imageID, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(imageID))
                throw new ArgumentNullException("imageID");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(MetadataEndpoint, collectionID, imageID));
            if (connector == null)
                return false;

            DeleteCollectionImageMetadataReq req = new DeleteCollectionImageMetadataReq();
            req.Callback = callback;
            req.CollectionID = collectionID;
            req.ImageID = imageID;
            req.Data = customData;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Delete = true;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnDeleteCollectionImageMetadataResp;

            return connector.Send(req);
        }

        private class DeleteCollectionImageMetadataReq : RESTConnector.Request
        {
            /// <summary>
            /// OnDeleteCollection callback.
            /// </summary>
            public OnDeleteImageMetadata Callback { get; set; }
            /// <summary>
            /// Collection identifier containing the image to be deleted.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// The identifier of the image to be deleted.
            /// </summary>
            public string ImageID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnDeleteCollectionImageMetadataResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((DeleteCollectionImageMetadataReq)req).Data;

            RESTConnector.ParsedResponse<object> parsedResp = new RESTConnector.ParsedResponse<object>(resp, customData, null);

            if (((DeleteCollectionImageMetadataReq)req).Callback != null)
                ((DeleteCollectionImageMetadataReq)req).Callback(parsedResp);
        }
        #endregion

        #region List Image Metadata
        /// <summary>
        /// List image metadata..
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The requested collection identifier.</param>
        /// <param name="imageID">The requested image identifier.</param>
        /// <param name="customData">Custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool GetMetadata(OnGetImageMetadata callback, string collectionID, string imageID, string customData = default(string))
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException(collectionID);
            if (string.IsNullOrEmpty(imageID))
                throw new ArgumentNullException(imageID);
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(MetadataEndpoint, collectionID, imageID));
            if (connector == null)
                return false;

            GetCollectionImageMetadataReq req = new GetCollectionImageMetadataReq();
            req.Callback = callback;
            req.Data = customData;
            req.CollectionID = collectionID;
            req.ImageID = imageID;
            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnGetCollectionImageMetadataResp;

            return connector.Send(req);
        }

        private class GetCollectionImageMetadataReq : RESTConnector.Request
        {
            /// <summary>
            /// OnGetCollections callback.
            /// </summary>
            public OnGetImageMetadata Callback { get; set; }
            /// <summary>
            /// Collection identifier of the requested metadata.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Image identifier for the requested metadata.
            /// </summary>
            public string ImageID { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnGetCollectionImageMetadataResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((GetCollectionImageMetadataReq)req).Data;

            RESTConnector.ParsedResponse<GetCollectionsBrief> parsedResp = new RESTConnector.ParsedResponse<GetCollectionsBrief>(resp, customData, _serializer);

            if (((GetCollectionImageMetadataReq)req).Callback != null)
                ((GetCollectionImageMetadataReq)req).Callback(parsedResp);
        }
        #endregion

        #region Find Similar Images
        /// <summary>
        /// Find Similar Images by image path.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The identifier of the collection to add images to.</param>
        /// <param name="imagePath">The path in the filesystem of the image to query.</param>
        /// <param name="limit">The number of similar results you want returned. Default limit is 10 results, you can specify a maximum limit of 100 results.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns>Returns true if succeess, false if failure.</returns>
        public bool FindSimilar(OnFindSimilar callback, string collectionID, string imagePath, int limit = 10, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException("imagePath");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

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
                    Log.Error("VisualRecognition", "Failed to upload {0}!", imagePath);
            }

            return FindSimilar(callback, collectionID, imageData, limit, customData);
        }

        /// <summary>
        /// Find Similar Images by byte[].
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="collectionID">The identifier of the collection to add images to.</param>
        /// <param name="imageData">The byte[] data of the image to query.</param>
        /// <param name="limit">The number of similar results you want returned. Default limit is 10 results, you can specify a maximum limit of 100 results.</param>
        /// <param name="customData">Optional custom data.</param>
        /// <returns></returns>
        public bool FindSimilar(OnFindSimilar callback, string collectionID, byte[] imageData, int limit = 10, string customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");
            if (string.IsNullOrEmpty(collectionID))
                throw new ArgumentNullException("collectionID");
            if (imageData == default(byte[]))
                throw new WatsonException("Image data is required!");
            if (string.IsNullOrEmpty(_apikey))
                _apikey = Credentials.ApiKey;
            if (string.IsNullOrEmpty(_apikey))
                throw new WatsonException("No API Key was found!");

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format(FindSimilarEndpoint, collectionID));
            if (connector == null)
                return false;

            FindSimilarReq req = new FindSimilarReq();
            req.Callback = callback;
            req.CollectionID = collectionID;
            req.ImageData = imageData;
            req.Limit = limit;
            req.Data = customData;

            req.Parameters["api_key"] = _apikey;
            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            req.Forms["image_file"] = new RESTConnector.Form(imageData);

            req.Timeout = 20.0f * 60.0f;
            req.OnResponse = OnFindSimilarResp;

            return connector.Send(req);
        }

        private class FindSimilarReq : RESTConnector.Request
        {
            /// <summary>
            /// OnCreateCollection callback.
            /// </summary>
            public OnFindSimilar Callback { get; set; }
            /// <summary>
            /// The collection identifier to add images to.
            /// </summary>
            public string CollectionID { get; set; }
            /// <summary>
            /// Byte array of Image Data to add to the collection.
            /// </summary>
            public byte[] ImageData { get; set; }
            /// <summary>
            /// Json metadata associated with this image.
            /// </summary>
            public int Limit { get; set; }
            /// <summary>
            /// Optional data.
            /// </summary>
            public string Data { get; set; }
        }

        private void OnFindSimilarResp(RESTConnector.Request req, RESTConnector.Response resp)
        {
            string customData = ((FindSimilarReq)req).Data;

            RESTConnector.ParsedResponse<SimilarImagesConfig> parsedResp = new RESTConnector.ParsedResponse<SimilarImagesConfig>(resp, customData, _serializer);

            if (((FindSimilarReq)req).Callback != null)
                ((FindSimilarReq)req).Callback(parsedResp);
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
