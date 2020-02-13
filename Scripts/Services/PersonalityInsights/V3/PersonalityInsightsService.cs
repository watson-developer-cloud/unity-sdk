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
using IBM.Watson.PersonalityInsights.V3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

namespace IBM.Watson.PersonalityInsights.V3
{
    public partial class PersonalityInsightsService : BaseService
    {
        private const string serviceId = "personality_insights";
        private const string defaultServiceUrl = "https://gateway.watsonplatform.net/personality-insights/api";

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
        /// PersonalityInsightsService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        public PersonalityInsightsService(string versionDate) : this(versionDate, ConfigBasedAuthenticatorFactory.GetAuthenticator(serviceId)) {}

        /// <summary>
        /// PersonalityInsightsService constructor.
        /// </summary>
        /// <param name="versionDate">The service version date in `yyyy-mm-dd` format.</param>
        /// <param name="authenticator">The service authenticator.</param>
        public PersonalityInsightsService(string versionDate, Authenticator authenticator) : base(versionDate, authenticator, serviceId)
        {
            Authenticator = authenticator;

            if (string.IsNullOrEmpty(versionDate))
            {
                throw new ArgumentNullException("A versionDate (format `yyyy-mm-dd`) is required to create an instance of PersonalityInsightsService");
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
        /// Get profile.
        ///
        /// Generates a personality profile for the author of the input text. The service accepts a maximum of 20 MB of
        /// input content, but it requires much less text to produce an accurate profile. The service can analyze text
        /// in Arabic, English, Japanese, Korean, or Spanish. It can return its results in a variety of languages.
        ///
        /// **See also:**
        /// * [Requesting a
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#input)
        /// * [Providing sufficient
        /// input](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#sufficient)
        ///
        /// ### Content types
        ///
        ///  You can provide input content as plain text (`text/plain`), HTML (`text/html`), or JSON
        /// (`application/json`) by specifying the **Content-Type** parameter. The default is `text/plain`.
        /// * Per the JSON specification, the default character encoding for JSON content is effectively always UTF-8.
        /// * Per the HTTP specification, the default encoding for plain text and HTML is ISO-8859-1 (effectively, the
        /// ASCII character set).
        ///
        /// When specifying a content type of plain text or HTML, include the `charset` parameter to indicate the
        /// character encoding of the input text; for example, `Content-Type: text/plain;charset=utf-8`.
        ///
        /// **See also:** [Specifying request and response
        /// formats](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#formats)
        ///
        /// ### Accept types
        ///
        ///  You must request a response as JSON (`application/json`) or comma-separated values (`text/csv`) by
        /// specifying the **Accept** parameter. CSV output includes a fixed number of columns. Set the **csv_headers**
        /// parameter to `true` to request optional column headers for CSV output.
        ///
        /// **See also:**
        /// * [Understanding a JSON
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-output#output)
        /// * [Understanding a CSV
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-outputCSV#outputCSV).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="content">A maximum of 20 MB of content to analyze, though the service requires much less text;
        /// for more information, see [Providing sufficient
        /// input](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#sufficient). For
        /// JSON input, provide an object of type `Content`.</param>
        /// <param name="contentType">The type of the input. For more information, see **Content types** in the method
        /// description. (optional, default to text/plain)</param>
        /// <param name="contentLanguage">The language of the input text for the request: Arabic, English, Japanese,
        /// Korean, or Spanish. Regional variants are treated as their parent language; for example, `en-US` is
        /// interpreted as `en`.
        ///
        /// The effect of the **Content-Language** parameter depends on the **Content-Type** parameter. When
        /// **Content-Type** is `text/plain` or `text/html`, **Content-Language** is the only way to specify the
        /// language. When **Content-Type** is `application/json`, **Content-Language** overrides a language specified
        /// with the `language` parameter of a `ContentItem` object, and content items that specify a different language
        /// are ignored; omit this parameter to base the language on the specification of the content items. You can
        /// specify any combination of languages for **Content-Language** and **Accept-Language**. (optional, default to
        /// en)</param>
        /// <param name="acceptLanguage">The desired language of the response. For two-character arguments, regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. You can specify
        /// any combination of languages for the input and response content. (optional, default to en)</param>
        /// <param name="rawScores">Indicates whether a raw score in addition to a normalized percentile is returned for
        /// each characteristic; raw scores are not compared with a sample population. By default, only normalized
        /// percentiles are returned. (optional, default to false)</param>
        /// <param name="csvHeaders">Indicates whether column labels are returned with a CSV response. By default, no
        /// column labels are returned. Applies only when the response type is CSV (`text/csv`). (optional, default to
        /// false)</param>
        /// <param name="consumptionPreferences">Indicates whether consumption preferences are returned with the
        /// results. By default, no consumption preferences are returned. (optional, default to false)</param>
        /// <returns><see cref="Profile" />Profile</returns>
        public bool Profile(Callback<Profile> callback, Content content, string contentType = null, string contentLanguage = null, string acceptLanguage = null, bool? rawScores = null, bool? csvHeaders = null, bool? consumptionPreferences = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `Profile`");
            if (content == null)
                throw new ArgumentNullException("`content` is required for `Profile`");

            RequestObject<Profile> req = new RequestObject<Profile>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("personality_insights", "V3", "Profile"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (rawScores != null)
            {
                req.Parameters["raw_scores"] = (bool)rawScores ? "true" : "false";
            }
            if (csvHeaders != null)
            {
                req.Parameters["csv_headers"] = (bool)csvHeaders ? "true" : "false";
            }
            if (consumptionPreferences != null)
            {
                req.Parameters["consumption_preferences"] = (bool)consumptionPreferences ? "true" : "false";
            }
            req.Headers["Accept"] = "application/json";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }

            if (!string.IsNullOrEmpty(contentLanguage))
            {
                req.Headers["Content-Language"] = contentLanguage;
            }

            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content));

            req.OnResponse = OnProfileResponse;

            Connector.URL = GetServiceUrl() + "/v3/profile";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnProfileResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<Profile> response = new DetailedResponse<Profile>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            try
            {
                string json = Encoding.UTF8.GetString(resp.Data);
                response.Result = JsonConvert.DeserializeObject<Profile>(json);
                response.Response = json;
            }
            catch (Exception e)
            {
                Log.Error("PersonalityInsightsService.OnProfileResponse()", "Exception: {0}", e.ToString());
                resp.Success = false;
            }

            if (((RequestObject<Profile>)req).Callback != null)
                ((RequestObject<Profile>)req).Callback(response, resp.Error);
        }
        /// <summary>
        /// Get profile as csv.
        ///
        /// Generates a personality profile for the author of the input text. The service accepts a maximum of 20 MB of
        /// input content, but it requires much less text to produce an accurate profile. The service can analyze text
        /// in Arabic, English, Japanese, Korean, or Spanish. It can return its results in a variety of languages.
        ///
        /// **See also:**
        /// * [Requesting a
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#input)
        /// * [Providing sufficient
        /// input](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#sufficient)
        ///
        /// ### Content types
        ///
        ///  You can provide input content as plain text (`text/plain`), HTML (`text/html`), or JSON
        /// (`application/json`) by specifying the **Content-Type** parameter. The default is `text/plain`.
        /// * Per the JSON specification, the default character encoding for JSON content is effectively always UTF-8.
        /// * Per the HTTP specification, the default encoding for plain text and HTML is ISO-8859-1 (effectively, the
        /// ASCII character set).
        ///
        /// When specifying a content type of plain text or HTML, include the `charset` parameter to indicate the
        /// character encoding of the input text; for example, `Content-Type: text/plain;charset=utf-8`.
        ///
        /// **See also:** [Specifying request and response
        /// formats](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#formats)
        ///
        /// ### Accept types
        ///
        ///  You must request a response as JSON (`application/json`) or comma-separated values (`text/csv`) by
        /// specifying the **Accept** parameter. CSV output includes a fixed number of columns. Set the **csv_headers**
        /// parameter to `true` to request optional column headers for CSV output.
        ///
        /// **See also:**
        /// * [Understanding a JSON
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-output#output)
        /// * [Understanding a CSV
        /// profile](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-outputCSV#outputCSV).
        /// </summary>
        /// <param name="callback">The callback function that is invoked when the operation completes.</param>
        /// <param name="content">A maximum of 20 MB of content to analyze, though the service requires much less text;
        /// for more information, see [Providing sufficient
        /// input](https://cloud.ibm.com/docs/personality-insights?topic=personality-insights-input#sufficient). For
        /// JSON input, provide an object of type `Content`.</param>
        /// <param name="contentType">The type of the input. For more information, see **Content types** in the method
        /// description. (optional, default to text/plain)</param>
        /// <param name="contentLanguage">The language of the input text for the request: Arabic, English, Japanese,
        /// Korean, or Spanish. Regional variants are treated as their parent language; for example, `en-US` is
        /// interpreted as `en`.
        ///
        /// The effect of the **Content-Language** parameter depends on the **Content-Type** parameter. When
        /// **Content-Type** is `text/plain` or `text/html`, **Content-Language** is the only way to specify the
        /// language. When **Content-Type** is `application/json`, **Content-Language** overrides a language specified
        /// with the `language` parameter of a `ContentItem` object, and content items that specify a different language
        /// are ignored; omit this parameter to base the language on the specification of the content items. You can
        /// specify any combination of languages for **Content-Language** and **Accept-Language**. (optional, default to
        /// en)</param>
        /// <param name="acceptLanguage">The desired language of the response. For two-character arguments, regional
        /// variants are treated as their parent language; for example, `en-US` is interpreted as `en`. You can specify
        /// any combination of languages for the input and response content. (optional, default to en)</param>
        /// <param name="rawScores">Indicates whether a raw score in addition to a normalized percentile is returned for
        /// each characteristic; raw scores are not compared with a sample population. By default, only normalized
        /// percentiles are returned. (optional, default to false)</param>
        /// <param name="csvHeaders">Indicates whether column labels are returned with a CSV response. By default, no
        /// column labels are returned. Applies only when the response type is CSV (`text/csv`). (optional, default to
        /// false)</param>
        /// <param name="consumptionPreferences">Indicates whether consumption preferences are returned with the
        /// results. By default, no consumption preferences are returned. (optional, default to false)</param>
        /// <returns><see cref="System.IO.MemoryStream" />System.IO.MemoryStream</returns>
        public bool ProfileAsCsv(Callback<System.IO.MemoryStream> callback, Content content, string contentType = null, string contentLanguage = null, string acceptLanguage = null, bool? rawScores = null, bool? csvHeaders = null, bool? consumptionPreferences = null)
        {
            if (callback == null)
                throw new ArgumentNullException("`callback` is required for `ProfileAsCsv`");
            if (content == null)
                throw new ArgumentNullException("`content` is required for `ProfileAsCsv`");

            RequestObject<System.IO.MemoryStream> req = new RequestObject<System.IO.MemoryStream>
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

            foreach (KeyValuePair<string, string> kvp in Common.GetSdkHeaders("personality_insights", "V3", "ProfileAsCsv"))
            {
                req.Headers.Add(kvp.Key, kvp.Value);
            }

            req.Parameters["version"] = VersionDate;
            if (rawScores != null)
            {
                req.Parameters["raw_scores"] = (bool)rawScores ? "true" : "false";
            }
            if (csvHeaders != null)
            {
                req.Parameters["csv_headers"] = (bool)csvHeaders ? "true" : "false";
            }
            if (consumptionPreferences != null)
            {
                req.Parameters["consumption_preferences"] = (bool)consumptionPreferences ? "true" : "false";
            }
            req.Headers["Accept"] = "text/csv";

            if (!string.IsNullOrEmpty(contentType))
            {
                req.Headers["Content-Type"] = contentType;
            }

            if (!string.IsNullOrEmpty(contentLanguage))
            {
                req.Headers["Content-Language"] = contentLanguage;
            }

            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                req.Headers["Accept-Language"] = acceptLanguage;
            }
            req.Send = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content));

            req.OnResponse = OnProfileAsCsvResponse;

            Connector.URL = GetServiceUrl() + "/v3/profile";
            Authenticator.Authenticate(Connector);

            return Connector.Send(req);
        }

        private void OnProfileAsCsvResponse(RESTConnector.Request req, RESTConnector.Response resp)
        {
            DetailedResponse<System.IO.MemoryStream> response = new DetailedResponse<System.IO.MemoryStream>();
            foreach (KeyValuePair<string, string> kvp in resp.Headers)
            {
                response.Headers.Add(kvp.Key, kvp.Value);
            }
            response.StatusCode = resp.HttpResponseCode;

            response.Result = new System.IO.MemoryStream(resp.Data);

            if (((RequestObject<System.IO.MemoryStream>)req).Callback != null)
                ((RequestObject<System.IO.MemoryStream>)req).Callback(response, resp.Error);
        }
    }
}
