/**
* (C) Copyright IBM Corp. 2018, 2020.
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
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.VisualRecognition.V3.Model;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.VisualRecognition.V3
{
    public partial class VisualRecognitionService : BaseService
    {
        private const string serviceId = "visual_recognition";
        private const string defaultServiceUrl = "https://api.us-south.visual-recognition.watson.cloud.ibm.com";

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
        public VisualRecognitionService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// VisualRecognitionService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public VisualRecognitionService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of VisualRecognitionService");
            }
            else
            {
                VersionDate = versionDate;
            }

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
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
        /// <param name="imagesFilename">The filename for imagesFile. (optional)</param>
        /// <param name="imagesFileContentType">The content type of imagesFile. (optional)</param>
        /// <param name="url">The URL of an image (.gif, .jpg, .png, .tif) to analyze. The minimum recommended pixel
        /// density is 32X32 pixels, but the service tends to perform better with images that are at least 224 x 224
        /// pixels. The maximum image size is 10 MB.
        ///
        /// You can also include images with the **images_file** parameter. (optional)</param>
        /// <param name="threshold">The minimum score a class must have to be displayed in the response. Set the
        /// threshold to `0.0` to return all identified classes. (optional)</param>
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
        /// <returns><see cref="ClassifiedImages" />ClassifiedImages</returns>
        public bool Classify(Callback<ClassifiedImages> callback, System.IO.MemoryStream imagesFile = null, string imagesFilename = null, string imagesFileContentType = null, string url = null, float? threshold = null, List<string> owners = null, List<string> classifierIds = null, string acceptLanguage = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Classify`");

            RequestObject<ClassifiedImages> req = new RequestObject<ClassifiedImages>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "Classify"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (imagesFile != null)
            {
                req.Forms["images_file"] = new RESTConnector.Form(imagesFile, imagesFilename, imagesFileContentType);
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

            req.OnResponse = OnClassifyResponse;

            Connector.URL = GetServiceUrl() + "/v3/classify";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnClassifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ClassifiedImages> response = new DetailedResponse<ClassifiedImages>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ClassifiedImages>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnClassifyResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ClassifiedImages>)req).Callback != null)
                ((RequestObject<ClassifiedImages>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a classifier.
        ///
        /// Train a new multi-faceted classifier on the uploaded image data. Create your custom classifier with positive
        /// or negative example training images. Include at least two sets of examples, either two positive example
        /// files or one positive and one negative file. You can upload a maximum of 256 MB per call.
        ///
        /// **Tips when creating:**
        ///
        /// - If you set the **X-Watson-Learning-Opt-Out** header parameter to `true` when you create a classifier, the
        /// example training images are not stored. Save your training images locally. For more information, see [Data
        /// collection](#data-collection).
        ///
        /// - Encode all names in UTF-8 if they contain non-ASCII characters (.zip and image file names, and classifier
        /// and class names). The service assumes UTF-8 encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The name of the new classifier. Encode special characters in UTF-8.</param>
        /// <param name="positiveExamples">A dictionary that contains the value for each classname. The value is a .zip
        /// file of images that depict the visual subject of a class in the new classifier. You can include more than
        /// one positive example file in a call.
        ///
        /// Specify the parameter name by appending `_positive_examples` to the class name. For example,
        /// `goldenretriever_positive_examples` creates the class **goldenretriever**. The string cannot contain the
        /// following characters: ``$ * - { } \ | / ' " ` [ ]``.
        ///
        /// Include at least 10 images in .jpg or .png format. The minimum recommended image resolution is 32X32 pixels.
        /// The maximum number of images is 10,000 images or 100 MB per .zip file.
        ///
        /// Encode special characters in the file name in UTF-8.</param>
        /// <param name="negativeExamples">A .zip file of images that do not depict the visual subject of any of the
        /// classes of the new classifier. Must contain a minimum of 10 images.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="negativeExamplesFilename">The filename for negativeExamples. (optional)</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool CreateClassifier(Callback<Classifier> callback, string name, Dictionary<string, System.IO.MemoryStream> positiveExamples, System.IO.MemoryStream negativeExamples = null, string negativeExamplesFilename = null)
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
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "CreateClassifier"))
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
                foreach (KeyValuePair<string, System.IO.MemoryStream> entry in positiveExamples)
                {
                    var partName = string.Format("{0}_positive_examples", entry.Key);
                    req.Forms[partName] = new RESTConnector.Form(entry.Value, entry.Key + ".zip", "application/octet-stream");
                }
            }
            if (negativeExamples != null)
            {
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamples, negativeExamplesFilename, "application/octet-stream");
            }

            req.OnResponse = OnCreateClassifierResponse;

            Connector.URL = GetServiceUrl() + "/v3/classifiers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnCreateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Retrieve a list of classifiers.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="verbose">Specify `true` to return details about the classifiers. Omit this parameter to return
        /// a brief list of classifiers. (optional)</param>
        /// <returns><see cref="Classifiers" />Classifiers</returns>
        public bool ListClassifiers(Callback<Classifiers> callback, bool? verbose = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListClassifiers`");

            RequestObject<Classifiers> req = new RequestObject<Classifiers>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "ListClassifiers"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (verbose != null)
            {
                req.Parameters["verbose"] = (bool)verbose ? "true" : "false";
            }

            req.OnResponse = OnListClassifiersResponse;

            Connector.URL = GetServiceUrl() + "/v3/classifiers";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListClassifiersResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifiers> response = new DetailedResponse<Classifiers>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifiers>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnListClassifiersResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifiers>)req).Callback != null)
                ((RequestObject<Classifiers>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Retrieve classifier details.
        ///
        /// Retrieve information about a custom classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool GetClassifier(Callback<Classifier> callback, string classifierId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `GetClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "GetClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetClassifierResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/classifiers/{0}", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a classifier.
        ///
        /// Update a custom classifier by adding new positive or negative classes or by adding new images to existing
        /// classes. You must supply at least one set of positive or negative examples. For details, see [Updating
        /// custom
        /// classifiers](https://cloud.ibm.com/docs/visual-recognition?topic=visual-recognition-customizing#updating-custom-classifiers).
        ///
        /// Encode all names in UTF-8 if they contain non-ASCII characters (.zip and image file names, and classifier
        /// and class names). The service assumes UTF-8 encoding if it encounters non-ASCII characters.
        ///
        /// **Tips about retraining:**
        ///
        /// - You can't update the classifier if the **X-Watson-Learning-Opt-Out** header parameter was set to `true`
        /// when the classifier was created. Training images are not stored in that case. Instead, create another
        /// classifier. For more information, see [Data collection](#data-collection).
        ///
        /// - Don't make retraining calls on a classifier until the status is ready. When you submit retraining requests
        /// in parallel, the last request overwrites the previous requests. The `retrained` property shows the last time
        /// the classifier retraining finished.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <param name="positiveExamples">A dictionary that contains the value for each classname. The value is a .zip
        /// file of images that depict the visual subject of a class in the classifier. The positive examples create or
        /// update classes in the classifier. You can include more than one positive example file in a call.
        ///
        /// Specify the parameter name by appending `_positive_examples` to the class name. For example,
        /// `goldenretriever_positive_examples` creates the class `goldenretriever`. The string cannot contain the
        /// following characters: ``$ * - { } \ | / ' " ` [ ]``.
        ///
        /// Include at least 10 images in .jpg or .png format. The minimum recommended image resolution is 32X32 pixels.
        /// The maximum number of images is 10,000 images or 100 MB per .zip file.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="negativeExamples">A .zip file of images that do not depict the visual subject of any of the
        /// classes of the new classifier. Must contain a minimum of 10 images.
        ///
        /// Encode special characters in the file name in UTF-8. (optional)</param>
        /// <param name="negativeExamplesFilename">The filename for negativeExamples. (optional)</param>
        /// <returns><see cref="Classifier" />Classifier</returns>
        public bool UpdateClassifier(Callback<Classifier> callback, string classifierId, Dictionary<string, System.IO.MemoryStream> positiveExamples = null, System.IO.MemoryStream negativeExamples = null, string negativeExamplesFilename = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `UpdateClassifier`");

            RequestObject<Classifier> req = new RequestObject<Classifier>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbPOST,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "UpdateClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (positiveExamples != null && positiveExamples.Count > 0)
            {
                foreach (KeyValuePair<string, System.IO.MemoryStream> entry in positiveExamples)
                {
                    var partName = string.Format("{0}_positive_examples", entry.Key);
                    req.Forms[partName] = new RESTConnector.Form(entry.Value, entry.Key + ".zip", "application/octet-stream");
                }
            }
            if (negativeExamples != null)
            {
                req.Forms["negative_examples"] = new RESTConnector.Form(negativeExamples, negativeExamplesFilename, "application/octet-stream");
            }

            req.OnResponse = OnUpdateClassifierResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/classifiers/{0}", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Classifier> response = new DetailedResponse<Classifier>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Classifier>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnUpdateClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Classifier>)req).Callback != null)
                ((RequestObject<Classifier>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a classifier.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteClassifier(Callback<object> callback, string classifierId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteClassifier`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `DeleteClassifier`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "DeleteClassifier"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteClassifierResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/classifiers/{0}", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteClassifierResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnDeleteClassifierResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Retrieve a Core ML model of a classifier.
        ///
        /// Download a Core ML model file (.mlmodel) of a custom classifier that returns <tt>"core_ml_enabled":
        /// true</tt> in the classifier details.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="classifierId">The ID of the classifier.</param>
        /// <returns><see cref="byte[]" />byte[]</returns>
        public bool GetCoreMlModel(Callback<byte[]> callback, string classifierId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCoreMlModel`");
            if (string.IsNullOrEmpty(classifierId))
                throw new ArgumentNullException("`classifierId` is required for `GetCoreMlModel`");

            RequestObject<byte[]> req = new RequestObject<byte[]>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbGET,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "GetCoreMlModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCoreMlModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/classifiers/{0}/core_ml_model", classifierId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCoreMlModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<byte[]> response = new DetailedResponse<byte[]>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            response.Result = resp.Data;

            if (((RequestObject<byte[]>)req).Callback != null)
                ((RequestObject<byte[]>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete labeled data.
        ///
        /// Deletes all data associated with a specified customer ID. The method has no effect if no data is associated
        /// with the customer ID.
        ///
        /// You associate a customer ID with data by passing the `X-Watson-Metadata` header with a request that passes
        /// data. For more information about personal data and customer IDs, see [Information
        /// security](https://cloud.ibm.com/docs/visual-recognition?topic=visual-recognition-information-security).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="customerId">The customer ID for which all data is to be deleted.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteUserData(Callback<object> callback, string customerId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteUserData`");
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException("`customerId` is required for `DeleteUserData`");

            RequestObject<object> req = new RequestObject<object>
            {
                Callback = callback,
                HttpMethod = UnityWebRequest.kHttpVerbDELETE,
                DisableSslVerification = DisableSslVerification
            };

            foreach (KeyValuePair<string, string> kvp in customRequestHeaders)
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            ClearCustomRequestHeaders();

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V3", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v3/user_data";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteUserDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<object> response = new DetailedResponse<object>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<object>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnDeleteUserDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
    }
}