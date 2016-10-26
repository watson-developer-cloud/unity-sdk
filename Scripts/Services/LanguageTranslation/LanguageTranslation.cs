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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Text;
using MiniJSON;
using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.LanguageTranslation.v1
{
  /// <summary>
  /// This class wraps the Language Translation service.
  /// <a href="http://www.ibm.com/watson/developercloud/language-translation.html">Language Translation Service</a>
  /// </summary>
  public class LanguageTranslation : IWatsonService
  {
    #region Public Types
    /// <summary>
    /// Callback for GetModels() method.
    /// </summary>
    /// <param name="models"></param>
    public delegate void GetModelsCallback(TranslationModels models);
    /// <summary>
    /// Callback for GetModel() method.
    /// </summary>
    /// <param name="model"></param>
    public delegate void GetModelCallback(TranslationModel model);
    /// <summary>
    /// Callback for GetLanguages() method.
    /// </summary>
    /// <param name="languages"></param>
    public delegate void GetLanguagesCallback(Languages languages);
    /// <summary>
    /// Callback for Identify() method.
    /// </summary>
    /// <param name="languages"></param>
    public delegate void IdentifyCallback(string languages);
    /// <summary>
    /// Callback for Translate() method.
    /// </summary>
    /// <param name="translation"></param>
    public delegate void TranslateCallback(Translations translation);
    #endregion

    #region Private Data
    private const string SERVICE_ID = "LanguageTranslationV1";
    private static fsSerializer sm_Serializer = new fsSerializer();
    #endregion

    #region GetTranslation Functions
    /// <summary>
    /// Translate the provided text using the specified model.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="model_id">The ID of the model to use.</param>
    /// <param name="callback">The callback to receive the translated text.</param>
    /// <returns>Returns true on success.</returns>
    public bool GetTranslation(string text, string model_id, TranslateCallback callback)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");
      if (string.IsNullOrEmpty(model_id))
        throw new ArgumentNullException("model_id");

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters["model_id"] = model_id;
      parameters["text"] = new string[] { text };

      return GetTranslation(Json.Serialize(parameters), callback);
    }
    /// <summary>
    /// Translate the provided text using the specified source and target.
    /// </summary>
    /// <param name="text">The text to translate.</param>
    /// <param name="source">The ID of the source language.</param>
    /// <param name="target">The ID of the target language.</param>
    /// <param name="callback">The callback to receive the translated text.</param>
    /// <returns>Returns true on success.</returns>
    public bool GetTranslation(string text, string source, string target, TranslateCallback callback)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");
      if (string.IsNullOrEmpty(source))
        throw new ArgumentNullException("source");
      if (string.IsNullOrEmpty(target))
        throw new ArgumentNullException("target");

      Dictionary<string, object> parameters = new Dictionary<string, object>();
      parameters["source"] = source;
      parameters["target"] = target;
      parameters["text"] = new string[] { text };

      return GetTranslation(Json.Serialize(parameters), callback);
    }
    private bool GetTranslation(string json, TranslateCallback callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/translate");
      if (connector == null)
        return false;

      TranslateReq req = new TranslateReq();
      req.Callback = callback;
      req.OnResponse = TranslateResponse;
      req.Send = Encoding.UTF8.GetBytes(json);
      req.Headers["accept"] = "application/json";
      req.Headers["Content-Type"] = "application/json";

      return connector.Send(req);
    }

    private class TranslateReq : RESTConnector.Request
    {
      public TranslateCallback Callback { get; set; }
    };
    private void TranslateResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Translations translations = new Translations();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = translations;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Natural Language Classifier", "GetTranslation Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((TranslateReq)req).Callback != null)
        ((TranslateReq)req).Callback(resp.Success ? translations : null);
    }
    #endregion

    #region Models Functions
    /// <summary>
    /// This determines the types of models to return with GetModels.
    /// </summary>
    public enum TypeFilter
    {
      /// <summary>
      /// Default types
      /// </summary>
      DEFAULT,
      /// <summary>
      /// Non-Default types
      /// </summary>
      NON_DEFAULT,
      /// <summary>
      /// All types are returned.
      /// </summary>
      ALL
    }

    /// <summary>
    /// Retrieve the translation models with optional filters.
    /// </summary>
    /// <param name="callback">The callback to invoke with the array of models.</param>
    /// <param name="sourceFilter">Optional source language filter.</param>
    /// <param name="targetFilter">Optional target language filter.</param>
    /// <param name="defaults">Controls if we get default, non-default, or all models.</param>
    /// <returns>Returns a true on success, false if it failed to submit the request.</returns>
    public bool GetModels(GetModelsCallback callback,
        string sourceFilter = null,
        string targetFilter = null,
        TypeFilter defaults = TypeFilter.ALL)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/models");
      if (connector == null)
        return false;

      GetModelsReq req = new GetModelsReq();
      req.Callback = callback;
      req.OnResponse = GetModelsResponse;

      if (!string.IsNullOrEmpty(sourceFilter))
        req.Parameters["source"] = sourceFilter;
      if (!string.IsNullOrEmpty(targetFilter))
        req.Parameters["target"] = targetFilter;
      if (defaults == TypeFilter.DEFAULT)
        req.Parameters["default"] = "true";
      else if (defaults == TypeFilter.NON_DEFAULT)
        req.Parameters["default"] = "false";

      return connector.Send(req);
    }

    private class GetModelsReq : RESTConnector.Request
    {
      public GetModelsCallback Callback { get; set; }
    }

    private void GetModelsResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      TranslationModels models = new TranslationModels();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = models;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Natural Language Classifier", "GetModels Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetModelsReq)req).Callback != null)
        ((GetModelsReq)req).Callback(resp.Success ? models : null);
    }

    /// <summary>
    /// Get a specific model by it's ID.
    /// </summary>
    /// <param name="model_id"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool GetModel(string model_id, GetModelCallback callback)
    {
      if (string.IsNullOrEmpty(model_id))
        throw new ArgumentNullException("model_id");
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/models/");
      if (connector == null)
        return false;

      GetModelReq req = new GetModelReq();
      req.Callback = callback;
      req.Function = WWW.EscapeURL(model_id);
      req.OnResponse = GetModelResponse;

      return connector.Send(req);
    }

    private class GetModelReq : RESTConnector.Request
    {
      public GetModelCallback Callback { get; set; }
    }

    private void GetModelResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      TranslationModel model = new TranslationModel();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = model;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Natural Language Classifier", "GetModel Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetModelReq)req).Callback != null)
        ((GetModelReq)req).Callback(resp.Success ? model : null);
    }
    #endregion

    #region GetLanguages Functions
    /// <summary>
    /// This function returns a list to the callback of all identifiable languages.
    /// </summary>
    /// <param name="callback">The callback to invoke with a Language array, null on error.</param>
    /// <returns>Returns true if the request was submitted.</returns>
    public bool GetLanguages(GetLanguagesCallback callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/identifiable_languages");
      if (connector == null)
        return false;

      GetLanguagesReq req = new GetLanguagesReq();
      req.Callback = callback;
      req.OnResponse = GetLanguagesResponse;

      return connector.Send(req);
    }

    private class GetLanguagesReq : RESTConnector.Request
    {
      public GetLanguagesCallback Callback { get; set; }
    }

    private void GetLanguagesResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Languages langs = new Languages();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = langs;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Natural Language Classifier", "GetLanguages Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetLanguagesReq)req).Callback != null)
        ((GetLanguagesReq)req).Callback(resp.Success ? langs : null);
    }
    #endregion

    #region Identify Functions
    /// <summary>
    /// Identifies a language from the given text.
    /// </summary>
    /// <param name="text">The text sample to ID.</param>
    /// <param name="callback">The callback to receive the results.</param>
    /// <returns></returns>
    public bool Identify(string text, IdentifyCallback callback)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v2/identify");
      if (connector == null)
        return false;

      IdentifyReq req = new IdentifyReq();
      req.Callback = callback;
      req.Send = Encoding.UTF8.GetBytes(text);
      req.Headers["Content-Type"] = "text/plain";
      req.OnResponse = OnIdentifyResponse;

      return connector.Send(req);
    }

    private class IdentifyReq : RESTConnector.Request
    {
      public IdentifyCallback Callback { get; set; }
    };

    private void OnIdentifyResponse(RESTConnector.Request r, RESTConnector.Response resp)
    {
      IdentifyReq req = r as IdentifyReq;
      if (req == null)
        throw new WatsonException("Unexpected Request type.");

      if (resp.Success)
      {
        if (req.Callback != null)
          req.Callback(Encoding.UTF8.GetString(resp.Data));
      }
      else
      {
        Log.Error("Translate", "Identify() failed: {0}", resp.Error);
        if (req.Callback != null)
          req.Callback(null);
      }
    }
    #endregion

    #region IWatsonService interface
    /// <exclude />
    public string GetServiceID()
    {
      return SERVICE_ID;
    }

    /// <exclude />
    public void GetServiceStatus(ServiceStatus callback)
    {
      if (Config.Instance.FindCredentials(SERVICE_ID) != null)
        new CheckServiceStatus(this, callback);
      else
        callback(SERVICE_ID, false);
    }

    private class CheckServiceStatus
    {
      private LanguageTranslation m_Service = null;
      private ServiceStatus m_Callback = null;

      public CheckServiceStatus(LanguageTranslation service, ServiceStatus callback)
      {
        m_Service = service;
        m_Callback = callback;

        if (!m_Service.GetLanguages(OnCheckService))
          m_Callback(SERVICE_ID, false);
      }

      private void OnCheckService(Languages langs)
      {
        if (m_Callback != null)
          m_Callback(SERVICE_ID, langs != null);
      }
    };
    #endregion
  }
}
