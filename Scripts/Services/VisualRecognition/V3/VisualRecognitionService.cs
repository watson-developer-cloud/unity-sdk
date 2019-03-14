/**
* Copyright 2018, 2019 IBM Corp. All Rights Reserved.
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

using System.Collections.Generic;
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.VisualRecognition.V3.Model;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.VisualRecognition.V3
{
    public class VisualRecognitionService : BaseService
    {
        private const string serviceId = "watson_vision_combined";
        private const string defaultUrl = "https://gateway.watsonplatform.net/visual-recognition/api";

        #region Credentials
        /// <summary>
        /// Gets and sets the credentials of the service. Replace the default endpoint if endpoint is defined.
        /// </summary>
        public Credentials Credentials
        {
            get { return credentials; }
            set
            {
                credentials = value;
                if (!string.IsNullOrEmpty(credentials.Url))
                {
                    Url = credentials.Url;
                }
            }
        }
        #endregion

        #region Url
        /// <summary>
        /// Gets and sets the endpoint URL for the service.
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion

        #region VersionDate
        private string versionDate;
        /// <summary>
        /// Gets and sets the versionDate of the service.
        /// </summary>
        public string VersionDate
        {
            get { return versionDate; }
            set { versionDate = value; }
        }
        #endregion

        #region DisableSslVerification
        private bool disableSslVerification = false;
        /// <summary>
        /// Gets and sets the option to disable ssl verification
        /// </summary>
        public bool DisableSslVerification
        {
            get { return disableSslVerification; }
            set { disableSslVerification = value; }
        }
        #endregion

        /// <summary>
        /// VisualRecognitionService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public VisualRecognitionService(string versionDate) : base(versionDate, serviceId)
        {
            VersionDate = versionDate;
        }

        /// <summary>
        /// VisualRecognitionService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="credentials">The service credentials.</param>
        public VisualRecognitionService(string versionDate, Credentials credentials) : base(versionDate, credentials, serviceId)
        {
            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of VisualRecognitionService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (credentials.HasCredentials() || credentials.HasIamTokenData())
            {
                Credentials = credentials;

                if (string.IsNullOrEmpty(credentials.Url))
                {
                    credentials.Url = defaultUrl;
                }
            }
            else
            {
                throw new IBMException("Please provide a username and password or authorization token to use the VisualRecognition service. For more information, see https://github.com/watson-developer-cloud/unity-sdk/#configuring-your-service-credentials");
            }
        }

        /// <summary>
        /// Classify images.
        ///
        /// Classify images with built-in or custom classifiers.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="imagesFile">An image file (.gif, .jpg, .png, .tif) or .zip file with images. Maximum image size
        /// is 10 MB. Include no more than 20 images and limit the .zip file to 100 MB. Encode the image and .zip file
        /// names in UTF-8 if they contain non-ASCII characters. The service assumes UTF-8 encoding if it encounters
        /// non-ASCII characters.
        ///
        /// You can also include an image with the **url** parameter. (optional)</param>
        /// <param name="url">The URL of an image (.gif, .jpg, .png, .tif) to analyze. The minimum recommended pixel
        /// density is 32X32 pixels, but the service tends to perform better with images that are at least 224 x 224
        /// pixels. The maximum image size is 10 MB.
        ///
        /// You can also include images with the **images_file** parameter. (optional)</param>
        /// <param name="threshold">The minimum score a class must have to be displayed in the response. Set the
        /// threshold to `0.0` to return all identified classes. (optional, default to 0.5)</param>
        /// <param name="owners">The categories of classifiers to apply. The **classifier_ids** parameter overrides
        /// **owners**, so make sure that **classifier_ids** is empty.
        /// - Use `IBM` to classify against the `default` general classifier. You get the same result if both
        /// **classifier_ids** and **owners** parameters are empty.
        /// - Use `me` to classify against all your custom classifiers. However, for better performance use
        /// **classifier_ids** to specify the specific custom classifiers to apply.
        /// - Use both `IBM` and `me` to analyze the image against both classifier categories. (optional)</param>
        /// <param name="classifierIds">Which classifiers to apply. Overrides the **owners** parameter. You can specify
        /// both custom and built-in classifier IDs. The built-in `default` classifier is used if both
        /// **classifier_ids** and **owners** parameters are empty.
        ///
        /// The following built-in classifier IDs require no training:
        /// - `default`: Returns classes from thousands of general tags.
        /// - `food`: Enhances specificity and accuracy for images of food items.
        /// - `explicit`: Evaluates whether the image might be pornographic. (optional)</param>
        /// <param name="acceptLanguage">The desired language of parts of the response. See the response for details.
        /// (optional, default to en)</param>
        /// <param name="imagesFileContentType">The content type of imagesFile. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="ClassifiedImages" />ClassifiedImages</returns>
        public bool Classify(Callback<ClassifiedImages> callback, Dictionary<string, object> customData = null, System.IO.FileStream imagesFile = null, string url = null, float? threshold = null, List<string> owners = null, List<string> classifierIds = null, string acceptLanguage = null, string imagesFileContentType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Classify`");

            RequestObject<ClassifiedImages> req = new RequestObject<ClassifiedImages>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "Classify"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (imagesFile != null)
            {
                req.Forms["images_file"] = new RESTConnector.Form(imagesFile, imagesFile.Name, imagesFileContentType);
            }
            if (!string.IsNullOrEmpty(url))
            {
                req.Forms["url"] = new RESTConnector.Form(url);
            }
            if (threshold != null)
            {
                req.Forms["threshold"] = new RESTConnector.Form(threshold.ToString());
            }
            if (owners != null && owners.Count > 0)
            {
                req.Forms["owners"] = new RESTConnector.Form(string.Join(", ", owners.ToArray()));
            }
            if (classifierIds != null && classifierIds.Count > 0)
            {
                req.Forms["classifier_ids"] = new RESTConnector.Form(string.Join(", ", classifierIds.ToArray()));
            }
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }

            req.OnResponse = OnClassifyResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/classify");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnClassifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassifiedImages> response = new DetailedResponse<ClassifiedImages>();
            Dictionary<string, object> customData = ((RequestObject<ClassifiedImages>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassifiedImages>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnClassifyResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassifiedImages>)req).Callback != null)
                ((RequestObject<ClassifiedImages>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Detect faces in images.
        ///
        /// **Important:** On April 2, 2018, the identity information in the response to calls to the Face model was
        /// removed. The identity information refers to the `name` of the person, `score`, and `type_hierarchy`
        /// knowledge graph. For details about the enhanced Face model, see the [Release
        /// notes](https://cloud.ibm.com/docs/services/visual-recognition/release-notes.html#2april2018).
        ///
        /// Analyze and get data about faces in images. Responses can include estimated age and gender. This feature
        /// uses a built-in model, so no training is necessary. The Detect faces method does not support general
        /// biometric facial recognition.
        ///
        /// Supported image formats include .gif, .jpg, .png, and .tif. The maximum image size is 10 MB. The minimum
        /// recommended pixel density is 32X32 pixels, but the service tends to perform better with images that are at
        /// least 224 x 224 pixels.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="imagesFile">An image file (gif, .jpg, .png, .tif.) or .zip file with images. Limit the .zip
        /// file to 100 MB. You can include a maximum of 15 images in a request.
        ///
        /// Encode the image and .zip file names in UTF-8 if they contain non-ASCII characters. The service assumes
        /// UTF-8 encoding if it encounters non-ASCII characters.
        ///
        /// You can also include an image with the **url** parameter. (optional)</param>
        /// <param name="url">The URL of an image to analyze. Must be in .gif, .jpg, .png, or .tif format. The minimum
        /// recommended pixel density is 32X32 pixels, but the service tends to perform better with images that are at
        /// least 224 x 224 pixels. The maximum image size is 10 MB. Redirects are followed, so you can use a shortened
        /// URL.
        ///
        /// You can also include images with the **images_file** parameter. (optional)</param>
        /// <param name="acceptLanguage">The desired language of parts of the response. See the response for details.
        /// (optional, default to en)</param>
        /// <param name="imagesFileContentType">The content type of imagesFile. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="DetectedFaces" />DetectedFaces</returns>
        public bool DetectFaces(Callback<DetectedFaces> callback, Dictionary<string, object> customData = null, System.IO.FileStream imagesFile = null, string url = null, string acceptLanguage = null, string imagesFileContentType = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DetectFaces`");

            RequestObject<DetectedFaces> req = new RequestObject<DetectedFaces>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "DetectFaces"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (imagesFile != null)
            {
                req.Forms["images_file"] = new RESTConnector.Form(imagesFile, imagesFile.Name, imagesFileContentType);
            }
            if (!string.IsNullOrEmpty(url))
            {
                req.Forms["url"] = new RESTConnector.Form(url);
            }
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }

            req.OnResponse = OnDetectFacesResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/detect_faces");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDetectFacesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DetectedFaces> response = new DetailedResponse<DetectedFaces>();
            Dictionary<string, object> customData = ((RequestObject<DetectedFaces>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DetectedFaces>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnDetectFacesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DetectedFaces>)req).Callback != null)
                ((RequestObject<DetectedFaces>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Create a classifier.
        ///
        /// Train a new multi-faceted classifier on the uploaded image data. Create your custom classifier with positive
        /// or negative examples. Include at least two sets of examples, either two positive example files or one
        /// positive and one negative file. You can upload a maximum of 256 MB per call.
        ///
        /// Encode all names in UTF-8 if they contain non-ASCII characters (.zip and image file names, and classifier
        /// and class names). The service assumes UTF-8 encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The name of the new classifier. Encode special characters in UTF-8.</param>
        /// <param name="positiveExamples">A .zip file of images that depict the visual subject of a class in the new
        /// classifier. You can include more than one positive example file in a call.
        ///
        /// Specify the parameter name by appending `_positive_examples` to the class name. For example,
        /// `goldenretriever_positive_examples` creates the class **goldenretriever**.
        ///
        /// Include at least 10 images in .jpg or .png format. The minimum recommended image resolution is 32X32 pixels.
        /// The maximum number of images is 10,000 images or 100 MB per .zip file.
        ///
        /// Encode special characters in the file name in UTF-8.</param>
        /// <param name="negativeExamples">A .zip file of images that do not depict the visual subject of any of the
        /// classes of the new classifier. Must contain a minimum of 10 images.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool CreateClassifier(Callback<Classifier> callback, string name, Dictionary<string, System.IO.FileStream> positiveExamples, Dictionary<string, object> customData = null, System.IO.FileStream negativeExamples = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateClassifier`");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("`name` is required for `CreateClassifier`");
            if (positiveExamples == null)
                throw new ArgumentNullException("`positiveExamples` is required for `CreateClassifier`");
            if (positiveExamples.Count == 0)
                throw new ArgumentException("`positiveExamples` must contain at least one dictionary entry");

            RequestObject<Classifier> req = new RequestObject<Classifier>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "CreateClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (!string.IsNullOrEmpty(name))
            {
                req.Forms["name"] = new RESTConnector.Form(name);
            }
            if (positiveExamples != null && positiveExamples.Count > 0)
            {
                foreach (KeyValuePair<string, System.IO.FileStream> entry in positiveExamples)
                {
                    var partName = string.Format("{0}_positive_examples", entry.Key);
                    req.Forms[partName] = new RESTConnector.Form(entry.Value, entry.Value.Name, "application/octet-stream");
                }
            }
            if (negativeExamples != null)
            {
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamples, negativeExamples.Name, "application/octet-stream");
            }

            req.OnResponse = OnCreateClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/classifiers");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnCreateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            Dictionary<string, object> customData = ((RequestObject<Classifier>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnCreateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete a classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteClassifier(Callback<object> callback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `DeleteClassifier`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "DeleteClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v3/classifiers/{0}", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnDeleteClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Retrieve classifier details.
        ///
        /// Retrieve information about a custom classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool GetClassifier(Callback<Classifier> callback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `GetClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "GetClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v3/classifiers/{0}", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            Dictionary<string, object> customData = ((RequestObject<Classifier>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Retrieve a list of classifiers.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="verbose">Specify `true` to return details about the classifiers. Omit this parameter to return
        /// a brief list of classifiers. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Classifiers" />Classifiers</returns>
        public bool ListClassifiers(Callback<Classifiers> callback, Dictionary<string, object> customData = null, bool? verbose = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListClassifiers`");

            RequestObject<Classifiers> req = new RequestObject<Classifiers>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "ListClassifiers"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (verbose != null)
            {
                req.Parameters["verbose"] = (bool)verbose ? "true" : "false";
            }

            req.OnResponse = OnListClassifiersResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/classifiers");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnListClassifiersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifiers> response = new DetailedResponse<Classifiers>();
            Dictionary<string, object> customData = ((RequestObject<Classifiers>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifiers>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnListClassifiersResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifiers>)req).Callback != null)
                ((RequestObject<Classifiers>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Update a classifier.
        ///
        /// Update a custom classifier by adding new positive or negative classes or by adding new images to existing
        /// classes. You must supply at least one set of positive or negative examples. For details, see [Updating
        /// custom
        /// classifiers](https://cloud.ibm.com/docs/services/visual-recognition/customizing.html#updating-custom-classifiers).
        ///
        /// Encode all names in UTF-8 if they contain non-ASCII characters (.zip and image file names, and classifier
        /// and class names). The service assumes UTF-8 encoding if it encounters non-ASCII characters.
        ///
        /// **Tip:** Don't make retraining calls on a classifier until the status is ready. When you submit retraining
        /// requests in parallel, the last request overwrites the previous requests. The retrained property shows the
        /// last time the classifier retraining finished.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="positiveExamples">A .zip file of images that depict the visual subject of a class in the
        /// classifier. The positive examples create or update classes in the classifier. You can include more than one
        /// positive example file in a call.
        ///
        /// Specify the parameter name by appending `_positive_examples` to the class name. For example,
        /// `goldenretriever_positive_examples` creates the class `goldenretriever`.
        ///
        /// Include at least 10 images in .jpg or .png format. The minimum recommended image resolution is 32X32 pixels.
        /// The maximum number of images is 10,000 images or 100 MB per .zip file.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="negativeExamples">A .zip file of images that do not depict the visual subject of any of the
        /// classes of the new classifier. Must contain a minimum of 10 images.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool UpdateClassifier(Callback<Classifier> callback, string classifierId, Dictionary<string, object> customData = null, Dictionary<string, System.IO.FileStream> positiveExamples = null, System.IO.FileStream negativeExamples = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `UpdateClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "UpdateClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (positiveExamples != null && positiveExamples.Count > 0)
            {
                foreach (KeyValuePair<string, System.IO.FileStream> entry in positiveExamples)
                {
                    var partName = string.Format("{0}_positive_examples", entry.Key);
                    req.Forms[partName] = new RESTConnector.Form(entry.Value, entry.Value.Name, "application/octet-stream");
                }
            }
            if (negativeExamples != null)
            {
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamples, negativeExamples.Name, "application/octet-stream");
            }

            req.OnResponse = OnUpdateClassifierResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v3/classifiers/{0}", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnUpdateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            Dictionary<string, object> customData = ((RequestObject<Classifier>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnUpdateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Retrieve a Core ML model of a classifier.
        ///
        /// Download a Core ML model file (.mlmodel) of a custom classifier that returns <tt>"core_ml_enabled":
        /// true</tt> in the classifier details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="System.IO.MemoryStream" />System.IO.MemoryStream</returns>
        public bool GetCoreMlModel(Callback<System.IO.MemoryStream> callback, string classifierId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCoreMlModel`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `GetCoreMlModel`");

            RequestObject<System.IO.MemoryStream> req = new RequestObject<System.IO.MemoryStream>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "GetCoreMlModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCoreMlModelResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, string.Format("/v3/classifiers/{0}/core_ml_model", classifierId));
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnGetCoreMlModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<System.IO.MemoryStream> response = new DetailedResponse<System.IO.MemoryStream>();
            Dictionary<string, object> customData = ((RequestObject<System.IO.MemoryStream>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            response.Result = new System.IO.MemoryStream(resp.Data);

            if (((RequestObject<System.IO.MemoryStream>)req).Callback != null)
                ((RequestObject<System.IO.MemoryStream>)req).Callback(response, resp.Error, customData);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/services/visual-recognition/information-security.html).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <param name="customData">A Dictionary<string, object> of data that will be passed to the callback. The raw
        /// json output from the REST call will be passed in this object as the value of the 'json'
        /// key.</string></param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId, Dictionary<string, object> customData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification,
                CustomData = customData == null ? new Dictionary<string, object>() : customData
            };

            if (req.CustomData.ContainsKey(Constants.String.CUSTOM_REQUEST_HEADERS))
            {
                foreach (KeyValuePair<string, string> kvp in req.CustomData[Constants.String.CUSTOM_REQUEST_HEADERS] as Dictionary<string, string>)
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in Common.GetDefaultheaders("watson_vision_combined", "V3", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            RESTConnector connector = RESTConnector.GetConnector(Credentials, "/v3/user_data");
            if (connector == null)
            {
                return false;
            }

            return connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            Dictionary<string, object> customData = ((RequestObject<object>)req).CustomData;
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                if (!customData.ContainsKey("json"))
                {
                    customData.Add("json", json);
                }
                else
                {
                    customData["json"] = json;
                }
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error, customData);
        }
    }
}