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
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3
{
  /// <summary>
  /// This class wraps the Personality Insights service.
  /// <a href="http://www.ibm.com/watson/developercloud/personality-insights.html">Personality Insights Service</a>
  /// </summary>
  public class PersonalityInsights : IWatsonService
  {
    #region Private Data
    private const string SERVICE_ID = "PersonalityInsightsV3";
    private static fsSerializer sm_Serializer = new fsSerializer();
    #endregion

    #region Profile
    private const string SERVICE_GET_PROFILE = "/v3/profile";

    public delegate void OnGetProfile(Profile profile, string data);

    public bool GetProfile(OnGetProfile callback, string source,
        string contentType = ContentType.TEXT_PLAIN,
        string contentLanguage = ContentLanguage.ENGLISH,
        string accept = ContentType.APPLICATION_JSON,
        string acceptLanguage = AcceptLanguage.ENGLISH,
        bool raw_scores = false,
        bool csv_headers = false,
        bool consumption_preferences = false,
        string version = PersonalityInsightsVersion.Version,
        string data = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(source))
        throw new ArgumentNullException("A JSON or Text source is required for GetProfile!");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, SERVICE_GET_PROFILE);
      if (connector == null)
        return false;

      GetProfileRequest req = new GetProfileRequest();
      req.Source = source;
      req.Callback = callback;
      req.Data = data;
      req.OnResponse = GetProfileResponse;

      req.Parameters["raw_scores"] = raw_scores.ToString();
      req.Parameters["csv_headers"] = csv_headers.ToString();
      req.Parameters["consumption_preferences"] = consumption_preferences.ToString();
      req.Parameters["version"] = version;

      req.Headers["Content-Type"] = contentType;
      req.Headers["Content-Language"] = contentLanguage;
      req.Headers["Accept"] = accept;
      req.Headers["Accept-Language"] = acceptLanguage;

      if (source.StartsWith(Application.dataPath))
      {
        string jsonData = default(string);
        jsonData = File.ReadAllText(source);
        req.Send = System.Text.Encoding.UTF8.GetBytes(jsonData);
      }
      else
      {
        req.Send = System.Text.Encoding.UTF8.GetBytes(source);
      }

      return connector.Send(req);
    }

    /// <summary>
    /// Get profile request.
    /// </summary>
    public class GetProfileRequest : RESTConnector.Request
    {
      /// <summary>
      /// The source string.
      /// </summary>
      public string Source { get; set; }
      /// <summary>
      /// Custom data.
      /// </summary>
      public string Data { get; set; }
      /// <summary>
      /// The callback.
      /// </summary>
      public OnGetProfile Callback { get; set; }
    }

    private void GetProfileResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Profile response = new Profile();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = response;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("PersonalityInsights", "GetProfileResponse Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetProfileRequest)req).Callback != null)
        ((GetProfileRequest)req).Callback(resp.Success ? response : null, ((GetProfileRequest)req).Data);
    }
    #endregion

    #region IWatsonService implementation
    public string GetServiceID()
    {
      return SERVICE_ID;
    }

    public void GetServiceStatus(ServiceStatus callback)
    {
      if (Utilities.Config.Instance.FindCredentials(SERVICE_ID) != null)
        new CheckServiceStatus(this, callback);
      else
        callback(SERVICE_ID, false);
    }

    private class CheckServiceStatus
    {
      private PersonalityInsights m_Service = null;
      private ServiceStatus m_Callback = null;

      public CheckServiceStatus(PersonalityInsights service, ServiceStatus callback)
      {
        m_Service = service;
        m_Callback = callback;
        string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";
        if (!m_Service.GetProfile(OnGetProfile, dataPath, ContentType.TEXT_PLAIN, ContentLanguage.ENGLISH))
          m_Callback(SERVICE_ID, false);
      }

      private void OnGetProfile(Profile resp, string data)
      {
        if (m_Callback != null)
          m_Callback(SERVICE_ID, resp != null);
      }
    }
    #endregion
  }
}