/**
* (C) Copyright IBM Corp. 2019, 2020.
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

/**
* IBM OpenAPI SDK Code Generator Version: 99-SNAPSHOT-a45d89ef-20201209-153452
*/
 
using System.Collections.Generic;
using System.Text;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Connection;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.LanguageTranslator.V3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.LanguageTranslator.V3
{
    public partial class LanguageTranslatorService : BaseService
    {
        private const string defaultServiceName = "language_translator";
        private const string defaultServiceUrl = "https://api.us-south.language-translator.watson.cloud.ibm.com";

        #region Version
        private string version;
        /// <summary>
        /// Gets and sets the version of the service.
        /// Release date of the version of the API you want to use. Specify dates in YYYY-MM-DD format. The current
        /// version is `2018-05-01`.
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
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
        /// LanguageTranslatorService constructor.
        /// </summary>
        /// <param name="version">Release date of the version of the API you want to use. Specify dates in YYYY-MM-DD
        /// format. The current version is `2018-05-01`.</param>
        public LanguageTranslatorService(string version) : this(version, defaultServiceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(defaultServiceName)) {}

        /// <summary>
        /// LanguageTranslatorService constructor.
        /// </summary>
        /// <param name="version">Release date of the version of the API you want to use. Specify dates in YYYY-MM-DD
        /// format. The current version is `2018-05-01`.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public LanguageTranslatorService(string version, Authenticator authenticator) : this(version, defaultServiceName, authenticator) {}

        /// <summary>
        /// LanguageTranslatorService constructor.
        /// </summary>
        /// <param name="version">Release date of the version of the API you want to use. Specify dates in YYYY-MM-DD
        /// format. The current version is `2018-05-01`.</param>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        public LanguageTranslatorService(string version, string serviceName) : this(version, serviceName, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceName)) {}

        /// <summary>
        /// LanguageTranslatorService constructor.
        /// </summary>
        /// <param name="version">Release date of the version of the API you want to use. Specify dates in YYYY-MM-DD
        /// format. The current version is `2018-05-01`.</param>
        /// <param name="serviceName">The service name to be used when configuring the client instance</param>
        /// <param name="authenticator">The service authenticator.</param>
        public LanguageTranslatorService(string version, string serviceName, Authenticator authenticator) : base(authenticator, serviceName)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("`version` is required");
            }
            else
            {
                Version = version;
            }

            if (string.IsNullOrEmpty(GetServiceUrl()))
            {
                SetServiceUrl(defaultServiceUrl);
            }
        }

        /// <summary>
        /// List supported languages.
        ///
        /// Lists all supported languages for translation. The method returns an array of supported languages with
        /// information about each language. Languages are listed in alphabetical order by language code (for example,
        /// `af`, `ar`). In addition to basic information about each language, the response indicates whether the
        /// language is `supported_as_source` for translation and `supported_as_target` for translation. It also lists
        /// whether the language is `identifiable`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="Languages" />Languages</returns>
        public bool ListLanguages(Callback<Languages> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListLanguages`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<Languages> req = new RequestObject<Languages>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "ListLanguages"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListLanguagesResponse;

            Connector.URL = GetServiceUrl() + "/v3/languages";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Languages> response = new DetailedResponse<Languages>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Languages>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnListLanguagesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Languages>)req).Callback != null)
                ((RequestObject<Languages>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Translate.
        ///
        /// Translates the input text from the source language to the target language. Specify a model ID that indicates
        /// the source and target languages, or specify the source and target languages individually. You can omit the
        /// source language to have the service attempt to detect the language from the input text. If you omit the
        /// source language, the request must contain sufficient input text for the service to identify the source
        /// language.
        ///
        /// You can translate a maximum of 50 KB (51,200 bytes) of text with a single request. All input text must be
        /// encoded in UTF-8 format.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="text">Input text in UTF-8 encoding. Submit a maximum of 50 KB (51,200 bytes) of text with a
        /// single request. Multiple elements result in multiple translations in the response.</param>
        /// <param name="modelId">The model to use for translation. For example, `en-de` selects the IBM-provided base
        /// model for English-to-German translation. A model ID overrides the `source` and `target` parameters and is
        /// required if you use a custom model. If no model ID is specified, you must specify at least a target
        /// language. (optional)</param>
        /// <param name="source">Language code that specifies the language of the input text. If omitted, the service
        /// derives the source language from the input text. The input must contain sufficient text for the service to
        /// identify the language reliably. (optional)</param>
        /// <param name="target">Language code that specifies the target language for translation. Required if model ID
        /// is not specified. (optional)</param>
        /// <returns><see cref="TranslationResult" />TranslationResult</returns>
        public bool Translate(Callback<TranslationResult> callback, List<string> text, string modelId = null, string source = null, string target = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Translate`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (text == null)
                throw new ArgumentNullException("`text` is required for `Translate`");

            RequestObject<TranslationResult> req = new RequestObject<TranslationResult>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "Translate"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }
            req.Headers["Content-Type"] = "application/json";
            req.Headers["Accept"] = "application/json";

            JObject bodyObject = new JObject();
            if (text != null && text.Count > 0)
                bodyObject["text"] = JToken.FromObject(text);
            if (!string.IsNullOrEmpty(modelId))
                bodyObject["model_id"] = modelId;
            if (!string.IsNullOrEmpty(source))
                bodyObject["source"] = source;
            if (!string.IsNullOrEmpty(target))
                bodyObject["target"] = target;
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyObject));

            req.OnResponse = OnTranslateResponse;

            Connector.URL = GetServiceUrl() + "/v3/translate";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnTranslateResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TranslationResult> response = new DetailedResponse<TranslationResult>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TranslationResult>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnTranslateResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TranslationResult>)req).Callback != null)
                ((RequestObject<TranslationResult>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List identifiable languages.
        ///
        /// Lists the languages that the service can identify. Returns the language code (for example, `en` for English
        /// or `es` for Spanish) and name of each language.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="IdentifiableLanguages" />IdentifiableLanguages</returns>
        public bool ListIdentifiableLanguages(Callback<IdentifiableLanguages> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListIdentifiableLanguages`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<IdentifiableLanguages> req = new RequestObject<IdentifiableLanguages>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "ListIdentifiableLanguages"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListIdentifiableLanguagesResponse;

            Connector.URL = GetServiceUrl() + "/v3/identifiable_languages";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListIdentifiableLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<IdentifiableLanguages> response = new DetailedResponse<IdentifiableLanguages>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IdentifiableLanguages>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnListIdentifiableLanguagesResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<IdentifiableLanguages>)req).Callback != null)
                ((RequestObject<IdentifiableLanguages>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Identify language.
        ///
        /// Identifies the language of the input text.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="text">Input text in UTF-8 format.</param>
        /// <returns><see cref="IdentifiedLanguages" />IdentifiedLanguages</returns>
        public bool Identify(Callback<IdentifiedLanguages> callback, string text)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Identify`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("`text` is required for `Identify`");

            RequestObject<IdentifiedLanguages> req = new RequestObject<IdentifiedLanguages>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "Identify"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }
            req.Headers["Content-Type"] = "text/plain";
            req.Headers["Accept"] = "application/json";
            req.Send = Encoding.UTF8.GetBytes(text);

            req.OnResponse = OnIdentifyResponse;

            Connector.URL = GetServiceUrl() + "/v3/identify";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnIdentifyResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<IdentifiedLanguages> response = new DetailedResponse<IdentifiedLanguages>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<IdentifiedLanguages>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnIdentifyResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<IdentifiedLanguages>)req).Callback != null)
                ((RequestObject<IdentifiedLanguages>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List models.
        ///
        /// Lists available translation models.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="source">Specify a language code to filter results by source language. (optional)</param>
        /// <param name="target">Specify a language code to filter results by target language. (optional)</param>
        /// <param name="_default">If the `default` parameter isn't specified, the service returns all models (default
        /// and non-default) for each language pair. To return only default models, set this parameter to `true`. To
        /// return only non-default models, set this parameter to `false`. There is exactly one default model, the
        /// IBM-provided base model, per language pair. (optional)</param>
        /// <returns><see cref="TranslationModels" />TranslationModels</returns>
        public bool ListModels(Callback<TranslationModels> callback, string source = null, string target = null, bool? _default = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListModels`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<TranslationModels> req = new RequestObject<TranslationModels>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "ListModels"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }
            if (!string.IsNullOrEmpty(source))
            {
                req.Parameters["source"] = source;
            }
            if (!string.IsNullOrEmpty(target))
            {
                req.Parameters["target"] = target;
            }
            if (_default != null)
            {
                req.Parameters["default"] = (bool)_default ? "true" : "false";
            }

            req.OnResponse = OnListModelsResponse;

            Connector.URL = GetServiceUrl() + "/v3/models";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TranslationModels> response = new DetailedResponse<TranslationModels>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TranslationModels>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnListModelsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TranslationModels>)req).Callback != null)
                ((RequestObject<TranslationModels>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Create model.
        ///
        /// Uploads training files to customize a translation model. You can customize a model with a forced glossary or
        /// with a parallel corpus:
        /// * Use a *forced glossary* to force certain terms and phrases to be translated in a specific way. You can
        /// upload only a single forced glossary file for a model. The size of a forced glossary file for a custom model
        /// is limited to 10 MB.
        /// * Use a *parallel corpus* when you want your custom model to learn from general translation patterns in
        /// parallel sentences in your samples. What your model learns from a parallel corpus can improve translation
        /// results for input text that the model has not been trained on. You can upload multiple parallel corpora
        /// files with a request. To successfully train with parallel corpora, the corpora files must contain a
        /// cumulative total of at least 5000 parallel sentences. The cumulative size of all uploaded corpus files for a
        /// custom model is limited to 250 MB.
        ///
        /// Depending on the type of customization and the size of the uploaded files, training time can range from
        /// minutes for a glossary to several hours for a large parallel corpus. To create a model that is customized
        /// with a parallel corpus and a forced glossary, customize the model with a parallel corpus first and then
        /// customize the resulting model with a forced glossary.
        ///
        /// You can create a maximum of 10 custom models per language pair. For more information about customizing a
        /// translation model, including the formatting and character restrictions for data files, see [Customizing your
        /// model](https://cloud.ibm.com/docs/language-translator?topic=language-translator-customizing).
        ///
        /// #### Supported file formats
        ///
        ///  You can provide your training data for customization in the following document formats:
        /// * **TMX** (`.tmx`) - Translation Memory eXchange (TMX) is an XML specification for the exchange of
        /// translation memories.
        /// * **XLIFF** (`.xliff`) - XML Localization Interchange File Format (XLIFF) is an XML specification for the
        /// exchange of translation memories.
        /// * **CSV** (`.csv`) - Comma-separated values (CSV) file with two columns for aligned sentences and phrases.
        /// The first row must have two language codes. The first column is for the source language code, and the second
        /// column is for the target language code.
        /// * **TSV** (`.tsv` or `.tab`) - Tab-separated values (TSV) file with two columns for aligned sentences and
        /// phrases. The first row must have two language codes. The first column is for the source language code, and
        /// the second column is for the target language code.
        /// * **JSON** (`.json`) - Custom JSON format for specifying aligned sentences and phrases.
        /// * **Microsoft Excel** (`.xls` or `.xlsx`) - Excel file with the first two columns for aligned sentences and
        /// phrases. The first row contains the language code.
        ///
        /// You must encode all text data in UTF-8 format. For more information, see [Supported document formats for
        /// training
        /// data](https://cloud.ibm.com/docs/language-translator?topic=language-translator-customizing#supported-document-formats-for-training-data).
        ///
        ///
        /// #### Specifying file formats
        ///
        ///  You can indicate the format of a file by including the file extension with the file name. Use the file
        /// extensions shown in **Supported file formats**.
        ///
        /// Alternatively, you can omit the file extension and specify one of the following `content-type`
        /// specifications for the file:
        /// * **TMX** - `application/x-tmx+xml`
        /// * **XLIFF** - `application/xliff+xml`
        /// * **CSV** - `text/csv`
        /// * **TSV** - `text/tab-separated-values`
        /// * **JSON** - `application/json`
        /// * **Microsoft Excel** - `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
        ///
        /// For example, with `curl`, use the following `content-type` specification to indicate the format of a CSV
        /// file named **glossary**:
        ///
        /// `--form "forced_glossary=@glossary;type=text/csv"`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="baseModelId">The ID of the translation model to use as the base for customization. To see
        /// available models and IDs, use the `List models` method. Most models that are provided with the service are
        /// customizable. In addition, all models that you create with parallel corpora customization can be further
        /// customized with a forced glossary.</param>
        /// <param name="forcedGlossary">A file with forced glossary terms for the source and target languages. The
        /// customizations in the file completely overwrite the domain translation data, including high frequency or
        /// high confidence phrase translations.
        ///
        /// You can upload only one glossary file for a custom model, and the glossary can have a maximum size of 10 MB.
        /// A forced glossary must contain single words or short phrases. For more information, see **Supported file
        /// formats** in the method description.
        ///
        /// *With `curl`, use `--form forced_glossary=@{filename}`.*. (optional)</param>
        /// <param name="parallelCorpus">A file with parallel sentences for the source and target languages. You can
        /// upload multiple parallel corpus files in one request by repeating the parameter. All uploaded parallel
        /// corpus files combined must contain at least 5000 parallel sentences to train successfully. You can provide a
        /// maximum of 500,000 parallel sentences across all corpora.
        ///
        /// A single entry in a corpus file can contain a maximum of 80 words. All corpora files for a custom model can
        /// have a cumulative maximum size of 250 MB. For more information, see **Supported file formats** in the method
        /// description.
        ///
        /// *With `curl`, use `--form parallel_corpus=@{filename}`.*. (optional)</param>
        /// <param name="name">An optional model name that you can use to identify the model. Valid characters are
        /// letters, numbers, dashes, underscores, spaces, and apostrophes. The maximum length of the name is 32
        /// characters. (optional)</param>
        /// <returns><see cref="TranslationModel" />TranslationModel</returns>
        public bool CreateModel(Callback<TranslationModel> callback, string baseModelId, System.IO.MemoryStream forcedGlossary = null, System.IO.MemoryStream parallelCorpus = null, string name = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `CreateModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(baseModelId))
                throw new ArgumentNullException("`baseModelId` is required for `CreateModel`");

            RequestObject<TranslationModel> req = new RequestObject<TranslationModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "CreateModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (forcedGlossary != null)
            {
                req.Forms["forced_glossary"] = new RESTConnector.Form(forcedGlossary, "filename", "application/octet-stream");
            }
            if (parallelCorpus != null)
            {
                req.Forms["parallel_corpus"] = new RESTConnector.Form(parallelCorpus, "filename", "application/octet-stream");
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }
            if (!string.IsNullOrEmpty(baseModelId))
            {
                req.Parameters["base_model_id"] = baseModelId;
            }
            if (!string.IsNullOrEmpty(name))
            {
                req.Parameters["name"] = name;
            }

            req.OnResponse = OnCreateModelResponse;

            Connector.URL = GetServiceUrl() + "/v3/models";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnCreateModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TranslationModel> response = new DetailedResponse<TranslationModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TranslationModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnCreateModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TranslationModel>)req).Callback != null)
                ((RequestObject<TranslationModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete model.
        ///
        /// Deletes a custom translation model.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">Model ID of the model to delete.</param>
        /// <returns><see cref="DeleteModelResult" />DeleteModelResult</returns>
        public bool DeleteModel(Callback<DeleteModelResult> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `DeleteModel`");

            RequestObject<DeleteModelResult> req = new RequestObject<DeleteModelResult>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "DeleteModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnDeleteModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/models/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DeleteModelResult> response = new DetailedResponse<DeleteModelResult>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DeleteModelResult>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnDeleteModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DeleteModelResult>)req).Callback != null)
                ((RequestObject<DeleteModelResult>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get model details.
        ///
        /// Gets information about a translation model, including training status for custom models. Use this API call
        /// to poll the status of your customization request. A successfully completed training has a status of
        /// `available`.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="modelId">Model ID of the model to get.</param>
        /// <returns><see cref="TranslationModel" />TranslationModel</returns>
        public bool GetModel(Callback<TranslationModel> callback, string modelId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetModel`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(modelId))
                throw new ArgumentNullException("`modelId` is required for `GetModel`");

            RequestObject<TranslationModel> req = new RequestObject<TranslationModel>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "GetModel"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetModelResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/models/{0}", modelId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<TranslationModel> response = new DetailedResponse<TranslationModel>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<TranslationModel>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnGetModelResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<TranslationModel>)req).Callback != null)
                ((RequestObject<TranslationModel>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// List documents.
        ///
        /// Lists documents that have been submitted for translation.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <returns><see cref="DocumentList" />DocumentList</returns>
        public bool ListDocuments(Callback<DocumentList> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ListDocuments`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");

            RequestObject<DocumentList> req = new RequestObject<DocumentList>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "ListDocuments"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnListDocumentsResponse;

            Connector.URL = GetServiceUrl() + "/v3/documents";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnListDocumentsResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentList> response = new DetailedResponse<DocumentList>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentList>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnListDocumentsResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentList>)req).Callback != null)
                ((RequestObject<DocumentList>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Translate document.
        ///
        /// Submit a document for translation. You can submit the document contents in the `file` parameter, or you can
        /// reference a previously submitted document by document ID. The maximum file size for document translation is
        /// * 20 MB for service instances on the Standard, Advanced, and Premium plans
        /// * 2 MB for service instances on the Lite plan.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="file">The contents of the source file to translate. The maximum file size for document
        /// translation is 20 MB for service instances on the Standard, Advanced, and Premium plans, and 2 MB for
        /// service instances on the Lite plan. For more information, see [Supported file formats
        /// (Beta)](https://cloud.ibm.com/docs/language-translator?topic=language-translator-document-translator-tutorial#supported-file-formats).</param>
        /// <param name="filename">The filename for file.</param>
        /// <param name="fileContentType">The content type of file. (optional)</param>
        /// <param name="modelId">The model to use for translation. For example, `en-de` selects the IBM-provided base
        /// model for English-to-German translation. A model ID overrides the `source` and `target` parameters and is
        /// required if you use a custom model. If no model ID is specified, you must specify at least a target
        /// language. (optional)</param>
        /// <param name="source">Language code that specifies the language of the source document. If omitted, the
        /// service derives the source language from the input text. The input must contain sufficient text for the
        /// service to identify the language reliably. (optional)</param>
        /// <param name="target">Language code that specifies the target language for translation. Required if model ID
        /// is not specified. (optional)</param>
        /// <param name="documentId">To use a previously submitted document as the source for a new translation, enter
        /// the `document_id` of the document. (optional)</param>
        /// <returns><see cref="DocumentStatus" />DocumentStatus</returns>
        public bool TranslateDocument(Callback<DocumentStatus> callback, System.IO.MemoryStream file, string filename, string fileContentType = null, string modelId = null, string source = null, string target = null, string documentId = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `TranslateDocument`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (file == null)
                throw new ArgumentNullException("`file` is required for `TranslateDocument`");
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("`filename` is required for `TranslateDocument`");

            RequestObject<DocumentStatus> req = new RequestObject<DocumentStatus>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "TranslateDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Forms = new Dictionary<string, RESTConnector.Form>();
            if (file != null)
            {
                req.Forms["file"] = new RESTConnector.Form(file, filename, fileContentType);
            }
            if (!string.IsNullOrEmpty(modelId))
            {
                req.Forms["model_id"] = new RESTConnector.Form(modelId);
            }
            if (!string.IsNullOrEmpty(source))
            {
                req.Forms["source"] = new RESTConnector.Form(source);
            }
            if (!string.IsNullOrEmpty(target))
            {
                req.Forms["target"] = new RESTConnector.Form(target);
            }
            if (!string.IsNullOrEmpty(documentId))
            {
                req.Forms["document_id"] = new RESTConnector.Form(documentId);
            }
            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnTranslateDocumentResponse;

            Connector.URL = GetServiceUrl() + "/v3/documents";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnTranslateDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentStatus> response = new DetailedResponse<DocumentStatus>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentStatus>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnTranslateDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentStatus>)req).Callback != null)
                ((RequestObject<DocumentStatus>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get document status.
        ///
        /// Gets the translation status of a document.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="documentId">The document ID of the document.</param>
        /// <returns><see cref="DocumentStatus" />DocumentStatus</returns>
        public bool GetDocumentStatus(Callback<DocumentStatus> callback, string documentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetDocumentStatus`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `GetDocumentStatus`");

            RequestObject<DocumentStatus> req = new RequestObject<DocumentStatus>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "GetDocumentStatus"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetDocumentStatusResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/documents/{0}", documentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetDocumentStatusResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<DocumentStatus> response = new DetailedResponse<DocumentStatus>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<DocumentStatus>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("LanguageTranslatorService.OnGetDocumentStatusResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<DocumentStatus>)req).Callback != null)
                ((RequestObject<DocumentStatus>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Delete document.
        ///
        /// Deletes a document.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="documentId">Document ID of the document to delete.</param>
        /// <returns><see cref="object" />object</returns>
        public bool DeleteDocument(Callback<object> callback, string documentId)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `DeleteDocument`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `DeleteDocument`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "DeleteDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnDeleteDocumentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/documents/{0}", documentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnDeleteDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
                Log.Error("LanguageTranslatorService.OnDeleteDocumentResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<object>)req).Callback != null)
                ((RequestObject<object>)req).Callback(response, resp.Error);
        }

        /// <summary>
        /// Get translated document.
        ///
        /// Gets the translated document associated with the given document ID.
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="documentId">The document ID of the document that was submitted for translation.</param>
        /// <param name="accept">The type of the response: application/powerpoint, application/mspowerpoint,
        /// application/x-rtf, application/json, application/xml, application/vnd.ms-excel,
        /// application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-powerpoint,
        /// application/vnd.openxmlformats-officedocument.presentationml.presentation, application/msword,
        /// application/vnd.openxmlformats-officedocument.wordprocessingml.document,
        /// application/vnd.oasis.opendocument.spreadsheet, application/vnd.oasis.opendocument.presentation,
        /// application/vnd.oasis.opendocument.text, application/pdf, application/rtf, text/html, text/json, text/plain,
        /// text/richtext, text/rtf, or text/xml. A character encoding can be specified by including a `charset`
        /// parameter. For example, 'text/html;charset=utf-8'. (optional)</param>
        /// <returns><see cref="byte[]" />byte[]</returns>
        public bool GetTranslatedDocument(Callback<byte[]> callback, string documentId, string accept = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `GetTranslatedDocument`");
            if (string.IsNullOrEmpty(Version))
                throw new ArgumentNullException("`Version` is required");
            if (string.IsNullOrEmpty(documentId))
                throw new ArgumentNullException("`documentId` is required for `GetTranslatedDocument`");

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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("language_translator", "V3", "GetTranslatedDocument"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            if (!string.IsNullOrEmpty(Version))
            {
                req.Parameters["version"] = Version;
            }

            req.OnResponse = OnGetTranslatedDocumentResponse;

            Connector.URL = GetServiceUrl() + string.Format("/v3/documents/{0}/translated_document", documentId);
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnGetTranslatedDocumentResponse(RESTConnector.Request req, RESTConnector.Response resp)
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
    }
}