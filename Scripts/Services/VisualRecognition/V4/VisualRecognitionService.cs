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
using IBM.Cloud.SDK.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.VisualRecognition.V4.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.VisualRecognition.V4
{
    public partial class VisualRecognitionService : BaseService
    {
        private const string serviceId = "visual_recognition";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/visual-recognition/api";

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
        /// Analyze images.
        ///
        /// Analyze images by URL, by file, or both against your own collection. Make sure that
        /// **training_status.objects.ready** is `true` for the feature before you use a collection to analyze images.
        ///
        /// Encode the image and .zip file names in UTF-8 if they contain non-ASCII characters. The service assumes
        /// UTF-8 encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionIds">The IDs of the collections to analyze.</param>
        /// <param name="features">The features to analyze.</param>
        /// <param name="imagesFile">An array of image files (.jpg or .png) or .zip files with images.
        /// - Include a maximum of 20 images in a request.
        /// - Limit the .zip file to 100 MB.
        /// - Limit each image file to 10 MB.
        ///
        /// You can also include an image with the **image_url** parameter. (optional)</param>
        /// <param name="imageUrl">An array of URLs of image files (.jpg or .png).
        /// - Include a maximum of 20 images in a request.
        /// - Limit each image file to 10 MB.
        /// - Minimum width and height is 30 pixels, but the service tends to perform better with images that are at
        /// least 300 x 300 pixels. Maximum is 5400 pixels for either height or width.
        ///
        /// You can also include images with the **images_file** parameter. (optional)</param>
        /// <param name="threshold">The minimum score a feature must have to be returned. (optional)</param>
        /// <returns><see cref="AnalyzeResponse" />AnalyzeResponse</returns>
        public bool Analyze(Callback<AnalyzeResponse> callback, List<string> collectionIds, List<string> features, List<FileWithMetadata> imagesFile = null, List<string> imageUrl = null, float? threshold = null)
        {
            if (collectionIds == null || collectionIds.Count == 0)
            {
                throw new ArgumentNullException("`collectionIds` is required for `Analyze`");
            }
            if (features == null || features.Count == 0)
            {
                throw new ArgumentNullException("`features` is required for `Analyze`");
            }

            RequestObject<AnalyzeResponse> req = new RequestObject<AnalyzeResponse>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "Analyze"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (collectionIds != null)
            {
                req.Forms["collection_ids"] = new RESTConnector.Form(string.Join(",", collectionIds.ToArray()));
            }
            if (features != null)
            {
                req.Forms["features"] = new RESTConnector.Form(string.Join(",", features.ToArray()));
            }
            if (imagesFile != null)
            {
                    foreach (FileWithMetadata item in imagesFile)
                    {
                            req.Forms["images_file"] = new RESTConnector.Form(item.Data, item.Filename, item.ContentType);
                    }
            }
            if (imageUrl != null)
            {
                    foreach (string item in imageUrl)
                    {
                            req.Forms["image_url"] = new RESTConnector.Form(item);
                    }
            }
            if (threshold != null)
            {
                req.Forms["threshold"] = new RESTConnector.Form(threshold.ToString());
            }

            req.OnResponse = OnAnalyzeResponse;

            Connector.URL = GetServiceUrl() + "/v4/analyze";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAnalyzeResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<AnalyzeResponse> response = new DetailedResponse<AnalyzeResponse>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<AnalyzeResponse>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnAnalyzeResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<AnalyzeResponse>)req).Callback != null)
                ((RequestObject<AnalyzeResponse>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Create a collection.
        ///
        /// Create a collection that can be used to store images.
        ///
        /// To create a collection without specifying a name and description, include an empty JSON object in the
        /// request body.
        ///
        /// Encode the name and description in UTF-8 if they contain non-ASCII characters. The service assumes UTF-8
        /// encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="name">The name of the collection. The name can contain alphanumeric, underscore, hyphen, and
        /// dot characters. It cannot begin with the reserved prefix `sys-`. (optional)</param>
        /// <param name="description">The description of the collection. (optional)</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool CreateCollection(Callback<Collection> callback, string name = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "CreateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnCreateCollectionResponse;

            Connector.URL = GetServiceUrl() + "/v4/collections";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnCreateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List collections.
        ///
        /// Retrieves a list of collections for the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="CollectionsList" />CollectionsList</returns>
        public bool ListCollections(Callback<CollectionsList> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListCollections`");

            RequestObject<CollectionsList> req = new RequestObject<CollectionsList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "ListCollections"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListCollectionsResponse;

            Connector.URL = GetServiceUrl() + "/v4/collections";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListCollectionsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<CollectionsList> response = new DetailedResponse<CollectionsList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<CollectionsList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnListCollectionsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<CollectionsList>)req).Callback != null)
                ((RequestObject<CollectionsList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get collection details.
        ///
        /// Get details of one collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool GetCollection(Callback<Collection> callback, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "GetCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update a collection.
        ///
        /// Update the name or description of a collection.
        ///
        /// Encode the name and description in UTF-8 if they contain non-ASCII characters. The service assumes UTF-8
        /// encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="name">The name of the collection. The name can contain alphanumeric, underscore, hyphen, and
        /// dot characters. It cannot begin with the reserved prefix `sys-`. (optional)</param>
        /// <param name="description">The description of the collection. (optional)</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool UpdateCollection(Callback<Collection> callback, string collectionId, string name = null, string description = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateCollection`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "UpdateCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(name))
                bodyObject["name"] = name;
            if (!string.IsNullOrEmpty(description))
                bodyObject["description"] = description;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnUpdateCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete a collection.
        ///
        /// Delete a collection from the service instance.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteCollection(Callback<object> callback, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteCollection`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteCollection`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "DeleteCollection"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteCollectionResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteCollectionResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("VisualRecognitionService.OnDeleteCollectionResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add images.
        ///
        /// Add images to a collection by URL, by file, or both.
        ///
        /// Encode the image and .zip file names in UTF-8 if they contain non-ASCII characters. The service assumes
        /// UTF-8 encoding if it encounters non-ASCII characters.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="imagesFile">An array of image files (.jpg or .png) or .zip files with images.
        /// - Include a maximum of 20 images in a request.
        /// - Limit the .zip file to 100 MB.
        /// - Limit each image file to 10 MB.
        ///
        /// You can also include an image with the **image_url** parameter. (optional)</param>
        /// <param name="imageUrl">The array of URLs of image files (.jpg or .png).
        /// - Include a maximum of 20 images in a request.
        /// - Limit each image file to 10 MB.
        /// - Minimum width and height is 30 pixels, but the service tends to perform better with images that are at
        /// least 300 x 300 pixels. Maximum is 5400 pixels for either height or width.
        ///
        /// You can also include images with the **images_file** parameter. (optional)</param>
        /// <param name="trainingData">Training data for a single image. Include training data only if you add one image
        /// with the request.
        ///
        /// The `object` property can contain alphanumeric, underscore, hyphen, space, and dot characters. It cannot
        /// begin with the reserved prefix `sys-` and must be no longer than 32 characters. (optional)</param>
        /// <returns><see cref="ImageDetailsList" />ImageDetailsList</returns>
        public bool AddImages(Callback<ImageDetailsList> callback, string collectionId, List<FileWithMetadata> imagesFile = null, List<string> imageUrl = null, string trainingData = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddImages`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `AddImages`");

            RequestObject<ImageDetailsList> req = new RequestObject<ImageDetailsList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "AddImages"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (imagesFile != null)
            {
                    foreach (FileWithMetadata item in imagesFile)
                    {
                            req.Forms["images_file"] = new RESTConnector.Form(item.Data, item.Filename, item.ContentType);
                    }
            }
            if (imageUrl != null)
            {
                    foreach (string item in imageUrl)
                    {
                            req.Forms["image_url"] = new RESTConnector.Form(item);
                    }
            }
            if (!string.IsNullOrEmpty(trainingData))
            {
                req.Forms["training_data"] = new RESTConnector.Form(trainingData);
            }

            req.OnResponse = OnAddImagesResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddImagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ImageDetailsList> response = new DetailedResponse<ImageDetailsList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ImageDetailsList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnAddImagesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ImageDetailsList>)req).Callback != null)
                ((RequestObject<ImageDetailsList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// List images.
        ///
        /// Retrieves a list of images in a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <returns><see cref="ImageSummaryList" />ImageSummaryList</returns>
        public bool ListImages(Callback<ImageSummaryList> callback, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListImages`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListImages`");

            RequestObject<ImageSummaryList> req = new RequestObject<ImageSummaryList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "ListImages"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListImagesResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListImagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ImageSummaryList> response = new DetailedResponse<ImageSummaryList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ImageSummaryList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnListImagesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ImageSummaryList>)req).Callback != null)
                ((RequestObject<ImageSummaryList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get image details.
        ///
        /// Get the details of an image in a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="imageId">The identifier of the image.</param>
        /// <returns><see cref="ImageDetails" />ImageDetails</returns>
        public bool GetImageDetails(Callback<ImageDetails> callback, string collectionId, string imageId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetImageDetails`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetImageDetails`");
            if (string.IsNullOrEmpty(imageId))
                throw new ArgumentNullException("`imageId` is required for `GetImageDetails`");

            RequestObject<ImageDetails> req = new RequestObject<ImageDetails>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "GetImageDetails"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetImageDetailsResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images/{1}", collectionId, imageId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetImageDetailsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ImageDetails> response = new DetailedResponse<ImageDetails>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ImageDetails>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetImageDetailsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ImageDetails>)req).Callback != null)
                ((RequestObject<ImageDetails>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete an image.
        ///
        /// Delete one image from a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="imageId">The identifier of the image.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteImage(Callback<object> callback, string collectionId, string imageId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteImage`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteImage`");
            if (string.IsNullOrEmpty(imageId))
                throw new ArgumentNullException("`imageId` is required for `DeleteImage`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "DeleteImage"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteImageResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images/{1}", collectionId, imageId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteImageResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("VisualRecognitionService.OnDeleteImageResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get a JPEG file of an image.
        ///
        /// Download a JPEG representation of an image.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="imageId">The identifier of the image.</param>
        /// <param name="size">The image size. Specify `thumbnail` to return a version that maintains the original
        /// aspect ratio but is no larger than 200 pixels in the larger dimension. For example, an original 800 x 1000
        /// image is resized to 160 x 200 pixels. (optional, default to full)</param>
        /// <returns><see cref="byte[]" />byte[]</returns>
        public bool GetJpegImage(Callback<byte[]> callback, string collectionId, string imageId, string size = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetJpegImage`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetJpegImage`");
            if (string.IsNullOrEmpty(imageId))
                throw new ArgumentNullException("`imageId` is required for `GetJpegImage`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "GetJpegImage"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(size))
            {
                req.Parameters["size"] = size;
            }

            req.OnResponse = OnGetJpegImageResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images/{1}/jpeg", collectionId, imageId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetJpegImageResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
        /// List object metadata.
        ///
        /// Retrieves a list of object names in a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <returns><see cref="ObjectMetadataList" />ObjectMetadataList</returns>
        public bool ListObjectMetadata(Callback<ObjectMetadataList> callback, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListObjectMetadata`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `ListObjectMetadata`");

            RequestObject<ObjectMetadataList> req = new RequestObject<ObjectMetadataList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "ListObjectMetadata"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnListObjectMetadataResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/objects", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListObjectMetadataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ObjectMetadataList> response = new DetailedResponse<ObjectMetadataList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ObjectMetadataList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnListObjectMetadataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ObjectMetadataList>)req).Callback != null)
                ((RequestObject<ObjectMetadataList>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Update an object name.
        ///
        /// Update the name of an object. A successful request updates the training data for all images that use the
        /// object.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="_object">The name of the object.</param>
        /// <param name="newObject">The updated name of the object. The name can contain alphanumeric, underscore,
        /// hyphen, space, and dot characters. It cannot begin with the reserved prefix `sys-`.</param>
        /// <returns><see cref="UpdateObjectMetadata" />UpdateObjectMetadata</returns>
        public bool UpdateObjectMetadata(Callback<UpdateObjectMetadata> callback, string collectionId, string _object, string newObject)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `UpdateObjectMetadata`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `UpdateObjectMetadata`");
            if (string.IsNullOrEmpty(_object))
                throw new ArgumentNullException("`_object` is required for `UpdateObjectMetadata`");
            if (string.IsNullOrEmpty(newObject))
                throw new ArgumentNullException("`newObject` is required for `UpdateObjectMetadata`");

            RequestObject<UpdateObjectMetadata> req = new RequestObject<UpdateObjectMetadata>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "UpdateObjectMetadata"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (!string.IsNullOrEmpty(newObject))
                bodyObject["object"] = newObject;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnUpdateObjectMetadataResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/objects/{1}", collectionId, _object);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnUpdateObjectMetadataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<UpdateObjectMetadata> response = new DetailedResponse<UpdateObjectMetadata>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<UpdateObjectMetadata>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnUpdateObjectMetadataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<UpdateObjectMetadata>)req).Callback != null)
                ((RequestObject<UpdateObjectMetadata>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get object metadata.
        ///
        /// Get the number of bounding boxes for a single object in a collection.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="_object">The name of the object.</param>
        /// <returns><see cref="ObjectMetadata" />ObjectMetadata</returns>
        public bool GetObjectMetadata(Callback<ObjectMetadata> callback, string collectionId, string _object)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetObjectMetadata`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `GetObjectMetadata`");
            if (string.IsNullOrEmpty(_object))
                throw new ArgumentNullException("`_object` is required for `GetObjectMetadata`");

            RequestObject<ObjectMetadata> req = new RequestObject<ObjectMetadata>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "GetObjectMetadata"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnGetObjectMetadataResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/objects/{1}", collectionId, _object);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetObjectMetadataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<ObjectMetadata> response = new DetailedResponse<ObjectMetadata>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<ObjectMetadata>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetObjectMetadataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<ObjectMetadata>)req).Callback != null)
                ((RequestObject<ObjectMetadata>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Delete an object.
        ///
        /// Delete one object from a collection. A successful request deletes the training data from all images that use
        /// the object.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="_object">The name of the object.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteObject(Callback<object> callback, string collectionId, string _object)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteObject`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `DeleteObject`");
            if (string.IsNullOrEmpty(_object))
                throw new ArgumentNullException("`_object` is required for `DeleteObject`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "DeleteObject"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnDeleteObjectResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/objects/{1}", collectionId, _object);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteObjectResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("VisualRecognitionService.OnDeleteObjectResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Train a collection.
        ///
        /// Start training on images in a collection. The collection must have enough training data and untrained data
        /// (the **training_status.objects.data_changed** is `true`). If training is in progress, the request queues the
        /// next training job.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <returns><see cref="Collection" />Collection</returns>
        public bool Train(Callback<Collection> callback, string collectionId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Train`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `Train`");

            RequestObject<Collection> req = new RequestObject<Collection>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "Train"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;

            req.OnResponse = OnTrainResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/train", collectionId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnTrainResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Collection> response = new DetailedResponse<Collection>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Collection>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnTrainResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Collection>)req).Callback != null)
                ((RequestObject<Collection>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Add training data to an image.
        ///
        /// Add, update, or delete training data for an image. Encode the object name in UTF-8 if it contains non-ASCII
        /// characters. The service assumes UTF-8 encoding if it encounters non-ASCII characters.
        ///
        /// Elements in the request replace the existing elements.
        ///
        /// - To update the training data, provide both the unchanged and the new or changed values.
        ///
        /// - To delete the training data, provide an empty value for the training data.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="imageId">The identifier of the image.</param>
        /// <param name="objects">Training data for specific objects. (optional)</param>
        /// <returns><see cref="TrainingDataObjects" />TrainingDataObjects</returns>
        public bool AddImageTrainingData(Callback<TrainingDataObjects> callback, string collectionId, string imageId, List<TrainingDataObject> objects = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `AddImageTrainingData`");
            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("`collectionId` is required for `AddImageTrainingData`");
            if (string.IsNullOrEmpty(imageId))
                throw new ArgumentNullException("`imageId` is required for `AddImageTrainingData`");

            RequestObject<TrainingDataObjects> req = new RequestObject<TrainingDataObjects>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "AddImageTrainingData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (objects != null && objects.Count > 0)
                bodyObject["objects"] = JToken.FromObject(objects);
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnAddImageTrainingDataResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v4/collections/{0}/images/{1}/training_data", collectionId, imageId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnAddImageTrainingDataResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingDataObjects> response = new DetailedResponse<TrainingDataObjects>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingDataObjects>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnAddImageTrainingDataResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingDataObjects>)req).Callback != null)
                ((RequestObject<TrainingDataObjects>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get training usage.
        ///
        /// Information about the completed training events. You can use this information to determine how close you are
        /// to the training limits for the month.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="startTime">The earliest day to include training events. Specify dates in YYYY-MM-DD format. If
        /// empty or not specified, the earliest training event is included. (optional)</param>
        /// <param name="endTime">The most recent day to include training events. Specify dates in YYYY-MM-DD format.
        /// All events for the day are included. If empty or not specified, the current day is used. Specify the same
        /// value as `start_time` to request events for a single day. (optional)</param>
        /// <returns><see cref="TrainingEvents" />TrainingEvents</returns>
        public bool GetTrainingUsage(Callback<TrainingEvents> callback, string startTime = null, string endTime = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTrainingUsage`");

            RequestObject<TrainingEvents> req = new RequestObject<TrainingEvents>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "GetTrainingUsage"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(startTime))
            {
                req.Parameters["start_time"] = startTime;
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                req.Parameters["end_time"] = endTime;
            }

            req.OnResponse = OnGetTrainingUsageResponse;

            Connector.URL = GetServiceUrl() + "/v4/training_usage";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetTrainingUsageResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TrainingEvents> response = new DetailedResponse<TrainingEvents>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TrainingEvents>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("VisualRecognitionService.OnGetTrainingUsageResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TrainingEvents>)req).Callback != null)
                ((RequestObject<TrainingEvents>)req).Callback(response, resp.Error);
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("watson_vision_combined", "V4", "DeleteUserData"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (!string.IsNullOrEmpty(customerId))
            {
                req.Parameters["customer_id"] = customerId;
            }

            req.OnResponse = OnDeleteUserDataResponse;

            Connector.URL = GetServiceUrl() + "/v4/user_data";
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
