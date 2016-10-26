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

namespace IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1
{
  /// <summary>
  /// This class wraps the Text to Speech service.
  /// <a href="http://www.ibm.com/watson/developercloud/text-to-speech.html">Text to Speech Service</a>
  /// </summary>
  public class TextToSpeech : IWatsonService
  {
    #region Private Data
    private DataCache m_SpeechCache = null;
    private VoiceType m_Voice = VoiceType.en_US_Michael;
    private AudioFormatType m_AudioFormat = AudioFormatType.WAV;
    private Dictionary<VoiceType, string> m_VoiceTypes = new Dictionary<VoiceType, string>()
        {
            { VoiceType.en_US_Michael, "en-US_MichaelVoice" },
            { VoiceType.en_US_Lisa, "en-US_LisaVoice" },
            { VoiceType.en_US_Allison, "en-US_AllisonVoice" },
            { VoiceType.en_GB_Kate, "en-GB_KateVoice" },
            { VoiceType.es_ES_Enrique, "es-ES_EnriqueVoice" },
            { VoiceType.es_ES_Laura, "es-ES_LauraVoice" },
            { VoiceType.es_US_Sofia, "es-US_SofiaVoice" },
            { VoiceType.de_DE_Dieter, "de-DE_DieterVoice" },
            { VoiceType.de_DE_Birgit, "de-DE_BirgitVoice" },
            { VoiceType.fr_FR_Renee, "fr-FR_ReneeVoice" },
            { VoiceType.it_IT_Francesca, "it-IT_FrancescaVoice" },
            { VoiceType.ja_JP_Emi, "ja-JP_EmiVoice" },
      { VoiceType.pt_BR_Isabela, "pt-BR_IsabelaVoice"},
        };
    private Dictionary<AudioFormatType, string> m_AudioFormats = new Dictionary<AudioFormatType, string>()
        {
            { AudioFormatType.OGG, "audio/ogg;codecs=opus" },
            { AudioFormatType.WAV, "audio/wav" },
            { AudioFormatType.FLAC, "audio/flac" },
        };
    private const string SERVICE_ID = "TextToSpeechV1";
    private static fsSerializer sm_Serializer = new fsSerializer();
    private const float REQUEST_TIMEOUT = 10.0f * 60.0f;
    #endregion

    #region Public Properties
    /// <summary>
    /// Disable the local cache.
    /// </summary>
    public bool DisableCache { get; set; }
    /// <summary>
    /// This property allows the user to set the AudioFormat to use. Currently, only WAV is supported.
    /// </summary>
    public AudioFormatType AudioFormat { get { return m_AudioFormat; } set { m_AudioFormat = value; } }
    /// <summary>
    /// This property allows the user to specify the voice to use.
    /// </summary>
    public VoiceType Voice
    {
      get { return m_Voice; }
      set
      {
        if (m_Voice != value)
        {
          m_Voice = value;
          m_SpeechCache = null;
        }
      }
    }
    #endregion

    #region GetVoiceType
    private string GetVoiceType(VoiceType voiceType)
    {
      if (m_VoiceTypes.ContainsKey(voiceType))
      {
        string voiceName = "";
        m_VoiceTypes.TryGetValue(voiceType, out voiceName);
        return voiceName;
      }
      else
      {
        Log.Warning("TextToSpeech", "There is no voicetype for {0}!", voiceType);
        return null;
      }
    }
    #endregion

    #region GetVoices 
    /// <summary>
    /// This callback is used by the GetVoices() function.
    /// </summary>
    /// <param name="voices">The Voices object.</param>
    public delegate void GetVoicesCallback(Voices voices);
    /// <summary>
    /// Returns all available voices that can be used.
    /// </summary>
    /// <param name="callback">The callback to invoke with the list of available voices.</param>
    /// <returns>Returns ture if the request was submitted.</returns>
    public bool GetVoices(GetVoicesCallback callback)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/voices");
      if (connector == null)
        return false;

      GetVoicesReq req = new GetVoicesReq();
      req.Callback = callback;
      req.OnResponse = OnGetVoicesResp;

      return connector.Send(req);
    }
    private class GetVoicesReq : RESTConnector.Request
    {
      public GetVoicesCallback Callback { get; set; }
    };
    private void OnGetVoicesResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Voices voices = new Voices();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = voices;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Natural Language Classifier", "GetVoices Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetVoicesReq)req).Callback != null)
        ((GetVoicesReq)req).Callback(resp.Success ? voices : null);
    }
    #endregion

    #region GetVoice 
    /// <summary>
    /// This callback is used by the GetVoice() function.
    /// </summary>
    /// <param name="voice">The Voice object.</param>
    public delegate void GetVoiceCallback(Voice voice);
    /// <summary>
    /// Return specific voice.
    /// </summary>
    /// <param name="callback">The callback to invoke with the voice.</param>
    /// <param name="voice">The name of the voice you would like to get. If this is null, TextToSpeech will default to the set voice.</param>
    /// <returns>Returns ture if the request was submitted.</returns>
    public bool GetVoice(GetVoiceCallback callback, VoiceType? voice = null)
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (voice == null)
        voice = m_Voice;

      string service = "/v1/voices/{0}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, GetVoiceType((VoiceType)voice)));
      if (connector == null)
        return false;

      GetVoiceReq req = new GetVoiceReq();
      req.Callback = callback;
      req.OnResponse = OnGetVoiceResp;

      return connector.Send(req);
    }
    private class GetVoiceReq : RESTConnector.Request
    {
      public GetVoiceCallback Callback { get; set; }
    };
    private void OnGetVoiceResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Voice voice = new Voice();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = voice;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("TextToSpeech", "GetVoice Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetVoiceReq)req).Callback != null)
        ((GetVoiceReq)req).Callback(resp.Success ? voice : null);
    }
    #endregion

    #region Synthesize Functions
    /// <summary>
    /// This callback is passed into the ToSpeech() method.
    /// </summary>
    /// <param name="clip">The AudioClip containing the audio to play.</param>
    public delegate void ToSpeechCallback(AudioClip clip);
    /// <summary>
    /// Private Request object that holds data specific to the ToSpeech request.
    /// </summary>
    private class ToSpeechRequest : RESTConnector.Request
    {
      public string TextId { get; set; }
      public string Text { get; set; }
      public ToSpeechCallback Callback { get; set; }
    }

    /// <summary>
    /// Converts the given text into an AudioClip that can be played.
    /// </summary>
    /// <param name="text">The text to synthesis into speech.</param>
    /// <param name="callback">The callback to invoke with the AudioClip.</param>
    /// <param name="usePost">If true, then we use post instead of get, this allows for text that exceeds the 5k limit.</param>
    /// <returns>Returns true if the request is sent.</returns>
    public bool ToSpeech(string text, ToSpeechCallback callback, bool usePost = false)
    {
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");
      if (callback == null)
        throw new ArgumentNullException("callback");

      if (!m_AudioFormats.ContainsKey(m_AudioFormat))
      {
        Log.Error("TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString());
        return false;
      }
      if (!m_VoiceTypes.ContainsKey(m_Voice))
      {
        Log.Error("TextToSpeech", "Unsupported voice: {0}", m_Voice.ToString());
        return false;
      }

      string textId = Utility.GetMD5(text);
      if (!DisableCache)
      {
        if (m_SpeechCache == null)
          m_SpeechCache = new DataCache("TextToSpeech_" + m_VoiceTypes[m_Voice]);

        byte[] data = m_SpeechCache.Find(textId);
        if (data != null)
        {
          AudioClip clip = ProcessResponse(textId, data);
          callback(clip);
          return true;
        }
      }

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/synthesize");
      if (connector == null)
      {
        Log.Error("TextToSpeech", "Failed to get connector.");
        return false;
      }

      ToSpeechRequest req = new ToSpeechRequest();
      req.TextId = textId;
      req.Text = text;
      req.Callback = callback;
      req.Parameters["accept"] = m_AudioFormats[m_AudioFormat];
      req.Parameters["voice"] = m_VoiceTypes[m_Voice];
      req.OnResponse = ToSpeechResponse;

      if (usePost)
      {
        Dictionary<string, string> upload = new Dictionary<string, string>();
        upload["text"] = text;

        req.Send = Encoding.UTF8.GetBytes(Json.Serialize(upload));
        req.Headers["Content-Type"] = "application/json";
      }
      else
      {
        req.Parameters["text"] = text;
      }

      return connector.Send(req);
    }

    private void ToSpeechResponse(RESTConnector.Request req, RESTConnector.Response resp)
    {
      ToSpeechRequest speechReq = req as ToSpeechRequest;
      if (speechReq == null)
        throw new WatsonException("Wrong type of request object.");

      //Log.Debug( "TextToSpeech", "Request completed in {0} seconds.", resp.ElapsedTime );

      AudioClip clip = resp.Success ? ProcessResponse(speechReq.TextId, resp.Data) : null;
      if (clip == null)
        Log.Error("TextToSpeech", "Request Failed: {0}", resp.Error);
      if (m_SpeechCache != null && clip != null)
        m_SpeechCache.Save(speechReq.TextId, resp.Data);

      if (speechReq.Callback != null)
        speechReq.Callback(clip);
    }

    private AudioClip ProcessResponse(string textId, byte[] data)
    {
      switch (m_AudioFormat)
      {
        case AudioFormatType.WAV:
          return WaveFile.ParseWAV(textId, data);
        default:
          break;
      }

      Log.Error("TextToSpeech", "Unsupported audio format: {0}", m_AudioFormat.ToString());
      return null;
    }
    #endregion

    #region GetPronunciation
    /// <summary>
    /// This callback is used by the GetPronunciation() function.
    /// </summary>
    /// <param name="pronunciation">The pronunciation strting.</param>
    public delegate void GetPronunciationCallback(Pronunciation pronunciation);
    /// <summary>
    /// Returns the phonetic pronunciation for the word specified by the text parameter. You can request 
    /// the pronunciation for a specific format. You can also request the pronunciation for a specific
    /// voice to see the default translation for the language of that voice or for a specific custom voice
    /// model to see the translation for that voice model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The GetPronunciationCallback</param>
    /// <param name="text">The text string to pronounce.</param>
    /// <param name="voice">Specify a voice to obtain the pronunciation for the specified word in the language of that voice. All voices for the same language (for example, en-US) return the same translation. Do not specify both a voice and a customization_id. Retrieve available voices with the GET /v1/voices method. If this is null, TextToSpeech will default to the set voice.</param>
    /// <param name="format">Specify the phoneme set in which to return the pronunciation. Omit the parameter to obtain the pronunciation in the default format. Either ipa or spr.</param>
    /// <param name="customization_id">GUID of a custom voice model for which the pronunciation is to be returned. You must make the request with the service credentials of the model's owner. If the word is not defined in the specified voice model, the service returns the default translation for the model's language. Omit the parameter to see the translation for the specified voice with no customization. Do not specify both a voice and a customization_id.</param>
    /// <returns></returns>
    public bool GetPronunciation(GetPronunciationCallback callback, string text, VoiceType? voice = null, string format = "ipa", string customization_id = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(text))
        throw new ArgumentNullException("text");
      if (voice == null)
        voice = m_Voice;

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/pronunciation");
      if (connector == null)
        return false;

      GetPronunciationReq req = new GetPronunciationReq();
      req.Callback = callback;
      req.Text = text;
      req.Voice = (VoiceType)voice;
      req.Format = format;
      req.Customization_ID = customization_id;
      req.Parameters["text"] = text;
      req.Parameters["voice"] = GetVoiceType((VoiceType)voice);
      req.Parameters["format"] = format;
      if (!string.IsNullOrEmpty(customization_id))
        req.Parameters["customization_id"] = customization_id;
      req.OnResponse = OnGetPronunciationResp;

      return connector.Send(req);
    }

    private class GetPronunciationReq : RESTConnector.Request
    {
      public GetPronunciationCallback Callback { get; set; }
      public string Text { get; set; }
      public VoiceType Voice { get; set; }
      public string Format { get; set; }
      public string Customization_ID { get; set; }
    }

    private void OnGetPronunciationResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Pronunciation pronunciation = new Pronunciation();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = pronunciation;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "GetPronunciation Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetPronunciationReq)req).Callback != null)
        ((GetPronunciationReq)req).Callback(resp.Success ? pronunciation : null);
    }
    #endregion

    #region Get Customizations
    /// <summary>
    /// This callback is used by the GetCustomizations() function.
    /// </summary>
    /// <param name="customizations">The customizations</param>
    /// <param name="data">Optional custom data.</param>
    public delegate void GetCustomizationsCallback(Customizations customizations, string data);

    /// <summary>
    /// Lists metadata such as the name and description for the custom voice models that you own. Use the language query parameter to list the voice models that you own for the specified language only. Omit the parameter to see all voice models that you own for all languages. To see the words in addition to the metadata for a specific voice model, use the GET /v1/customizations/{customization_id} method. Only the owner of a custom voice model can use this method to list information about the model.
    /// Note: This method is currently a beta release that supports US English only
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool GetCustomizations(GetCustomizationsCallback callback, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");

      GetCustomizationsReq req = new GetCustomizationsReq();
      req.Callback = callback;
      req.Data = customData;
      req.OnResponse = OnGetCustomizationsResp;

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/customizations");
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class GetCustomizationsReq : RESTConnector.Request
    {
      public GetCustomizationsCallback Callback { get; set; }
      public string Data { get; set; }
    }

    private void OnGetCustomizationsResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Customizations customizations = new Customizations();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = customizations;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "GetCustomizations Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetCustomizationsReq)req).Callback != null)
        ((GetCustomizationsReq)req).Callback(resp.Success ? customizations : null, ((GetCustomizationsReq)req).Data);
    }
    #endregion

    #region Create Customization
    /// <summary>
    /// Thid callback is used by the CreateCustomization() function.
    /// </summary>
    /// <param name="customizationID">The customizationID.</param>
    /// <param name="data">Optional custom data.</param>
    public delegate void CreateCustomizationCallback(CustomizationID customizationID, string data);

    /// <summary>
    /// Creates a new empty custom voice model that is owned by the requesting user.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="name">Name of the new custom voice model.</param>
    /// <param name="language">Language of the new custom voice model. Omit the parameter to use the default language, en-US. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].</param>
    /// <param name="description">Description of the new custom voice model.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool CreateCustomization(CreateCustomizationCallback callback, string name, string language = default(string), string description = default(string), string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException("A name is required to create a custom voice model.");

      CustomVoice customVoice = new CustomVoice();
      customVoice.name = name;
      customVoice.language = language;
      customVoice.description = description;

      fsData data;
      sm_Serializer.TrySerialize(customVoice.GetType(), customVoice, out data).AssertSuccessWithoutWarnings();
      string customizationJson = fsJsonPrinter.CompressedJson(data);

      CreateCustomizationRequest req = new CreateCustomizationRequest();
      req.Callback = callback;
      req.CustomVoice = customVoice;
      req.Data = customData;
      req.Headers["Content-Type"] = "application/json";
      req.Headers["Accept"] = "application/json";
      req.Send = Encoding.UTF8.GetBytes(customizationJson);
      req.OnResponse = OnCreateCustomizationResp;

      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, "/v1/customizations");
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class CreateCustomizationRequest : RESTConnector.Request
    {
      public CreateCustomizationCallback Callback { get; set; }
      public CustomVoice CustomVoice { get; set; }
      public string Data { get; set; }
    }

    private void OnCreateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      CustomizationID customizationID = new CustomizationID();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = customizationID;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "CreateCustomization Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((CreateCustomizationRequest)req).Callback != null)
        ((CreateCustomizationRequest)req).Callback(resp.Success ? customizationID : null, ((CreateCustomizationRequest)req).Data);
    }
    #endregion

    #region Delete Customization
    /// <summary>
    /// This callback is used by the DeleteCustomization() function.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="data"></param>
    public delegate void OnDeleteCustomizationCallback(bool success, string data);
    /// <summary>
    /// Deletes the custom voice model with the specified `customization_id`. Only the owner of a custom voice model can use this method to delete the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The voice model to be deleted's identifier.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool DeleteCustomization(OnDeleteCustomizationCallback callback, string customizationID, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("A customizationID to delete is required for DeleteCustomization");

      DeleteCustomizationRequest req = new DeleteCustomizationRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Data = customData;
      req.Timeout = REQUEST_TIMEOUT;
      req.Delete = true;
      req.OnResponse = OnDeleteCustomizationResp;

      string service = "/v1/customizations/{0}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class DeleteCustomizationRequest : RESTConnector.Request
    {
      public OnDeleteCustomizationCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Data { get; set; }
    }

    private void OnDeleteCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      if (((DeleteCustomizationRequest)req).Callback != null)
        ((DeleteCustomizationRequest)req).Callback(resp.Success, ((DeleteCustomizationRequest)req).Data);
    }
    #endregion

    #region Get Customization
    /// <summary>
    /// This callback is used by the GetCusomization() function.
    /// </summary>
    /// <param name="customization"></param>
    /// <param name="data"></param>
    public delegate void GetCustomizationCallback(Customization customization, string data);
    /// <summary>
    /// Lists all information about the custom voice model with the specified `customization_id`. In addition to metadata such as the name and description of the voice model, the output includes the words in the model and their translations as defined in the model. To see just the metadata for a voice model, use the GET `/v1/customizations` method. Only the owner of a custom voice model can use this method to query information about the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The requested custom voice model's identifier.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool GetCustomization(GetCustomizationCallback callback, string customizationID, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("A customizationID to get a custom voice model.");

      GetCustomizationRequest req = new GetCustomizationRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Data = customData;
      req.OnResponse = OnGetCustomizationResp;

      string service = "/v1/customizations/{0}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class GetCustomizationRequest : RESTConnector.Request
    {
      public GetCustomizationCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Data { get; set; }
    }

    private void OnGetCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Customization customization = new Customization();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = customization;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "GetCustomization Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetCustomizationRequest)req).Callback != null)
        ((GetCustomizationRequest)req).Callback(resp.Success ? customization : null, ((GetCustomizationRequest)req).Data);
    }
    #endregion

    #region Update Customization
    /// <summary>
    /// This callback is used by the UpdateCustomization() function.
    /// </summary>
    /// <param name="success">Success</param>
    /// <param name="data">Optional custom data.</param>
    public delegate void UpdateCustomizationCallback(bool success, string data);

    /// <summary>
    /// Updates information for the custom voice model with the specified `customization_id`. You can update the metadata such as the name and description of the voice model. You can also update the words in the model and their translations. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to update the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
    /// <param name="customVoiceUpdate">Custom voice model update data.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool UpdateCustomization(UpdateCustomizationCallback callback, string customizationID, CustomVoiceUpdate customVoiceUpdate, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("customizationID");
      if (!customVoiceUpdate.HasData())
        throw new ArgumentNullException("Custom voice update data is required to update a custom voice model.");

      fsData data;
      sm_Serializer.TrySerialize(customVoiceUpdate.GetType(), customVoiceUpdate, out data).AssertSuccessWithoutWarnings();
      string customizationJson = fsJsonPrinter.CompressedJson(data);

      UpdateCustomizationRequest req = new UpdateCustomizationRequest();
      req.Callback = callback;
      req.CustomVoiceUpdate = customVoiceUpdate;
      req.CustomizationID = customizationID;
      req.Data = customData;
      req.Headers["Content-Type"] = "application/json";
      req.Headers["Accept"] = "application/json";
      req.Send = Encoding.UTF8.GetBytes(customizationJson);
      req.OnResponse = OnUpdateCustomizationResp;

      string service = "/v1/customizations/{0}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    //	TODO add UpdateCustomization overload using path to json file.

    private class UpdateCustomizationRequest : RESTConnector.Request
    {
      public UpdateCustomizationCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public CustomVoiceUpdate CustomVoiceUpdate { get; set; }
      public string Data { get; set; }
    }

    private void OnUpdateCustomizationResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      if (((UpdateCustomizationRequest)req).Callback != null)
        ((UpdateCustomizationRequest)req).Callback(resp.Success, ((UpdateCustomizationRequest)req).Data);
    }
    #endregion

    #region Get Customization Words
    /// <summary>
    /// This callback is used by the GetCusomizationWords() function.
    /// </summary>
    /// <param name="customization"></param>
    /// <param name="data"></param>
    public delegate void GetCustomizationWordsCallback(Words words, string data);
    /// <summary>
    /// Lists all of the words and their translations for the custom voice model with the specified `customization_id`. The output shows the translations as they are defined in the model. Only the owner of a custom voice model can use this method to query information about the model's words.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The requested custom voice model's identifier.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool GetCustomizationWords(GetCustomizationWordsCallback callback, string customizationID, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");

      GetCustomizationWordsRequest req = new GetCustomizationWordsRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Data = customData;
      req.OnResponse = OnGetCustomizationWordsResp;

      string service = "/v1/customizations/{0}/words";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class GetCustomizationWordsRequest : RESTConnector.Request
    {
      public GetCustomizationWordsCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Data { get; set; }
    }

    private void OnGetCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Words words = new Words();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = words;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "GetCustomizationWords Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetCustomizationWordsRequest)req).Callback != null)
        ((GetCustomizationWordsRequest)req).Callback(resp.Success ? words : null, ((GetCustomizationWordsRequest)req).Data);
    }
    #endregion

    #region Add Customization Words
    /// <summary>
    /// This callback is used by the AddCustomizationWords() function.
    /// </summary>
    /// <param name="success">Success</param>
    /// <param name="data">Optional custom data.</param>
    public delegate void AddCustomizationWordsCallback(bool success, string data);

    /// <summary>
    /// Adds one or more words and their translations to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add words to the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
    /// <param name="words">Words object to add to custom voice model.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool AddCustomizationWords(AddCustomizationWordsCallback callback, string customizationID, Words words, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("customizationID");
      if (!words.HasData())
        throw new ArgumentNullException("Words data is required to add words to a custom voice model.");

      fsData data;
      sm_Serializer.TrySerialize(words.GetType(), words, out data).AssertSuccessWithoutWarnings();
      string customizationJson = fsJsonPrinter.CompressedJson(data);

      AddCustomizationWordsRequest req = new AddCustomizationWordsRequest();
      req.Callback = callback;
      req.Words = words;
      req.CustomizationID = customizationID;
      req.Data = customData;
      req.Headers["Content-Type"] = "application/json";
      req.Headers["Accept"] = "application/json";
      req.Send = Encoding.UTF8.GetBytes(customizationJson);
      req.OnResponse = OnAddCustomizationWordsResp;

      string service = "/v1/customizations/{0}/words";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    //	TODO add AddCustomizationWords overload using path to json file.

    private class AddCustomizationWordsRequest : RESTConnector.Request
    {
      public AddCustomizationWordsCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public Words Words { get; set; }
      public string Data { get; set; }
    }

    private void OnAddCustomizationWordsResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      if (((AddCustomizationWordsRequest)req).Callback != null)
        ((AddCustomizationWordsRequest)req).Callback(resp.Success, ((AddCustomizationWordsRequest)req).Data);
    }
    #endregion

    #region Delete Customization Word
    /// <summary>
    /// This callback is used by the DeleteCustomizationWord() function.
    /// </summary>
    /// <param name="success"></param>
    /// <param name="data"></param>
    public delegate void OnDeleteCustomizationWordCallback(bool success, string data);
    /// <summary>
    /// Deletes a single word from the custom voice model with the specified customization_id. Only the owner of a custom voice model can use this method to delete a word from the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The voice model's identifier.</param>
    /// <param name="word">The word to be deleted.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool DeleteCustomizationWord(OnDeleteCustomizationWordCallback callback, string customizationID, string word, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("A customizationID is required for DeleteCustomizationWord");
      if (string.IsNullOrEmpty(word))
        throw new ArgumentNullException("A word to delete is required for DeleteCustomizationWord");

      DeleteCustomizationWordRequest req = new DeleteCustomizationWordRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Word = word;
      req.Data = customData;
      req.Timeout = REQUEST_TIMEOUT;
      req.Delete = true;
      req.OnResponse = OnDeleteCustomizationWordResp;

      string service = "/v1/customizations/{0}/words/{1}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID, word));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class DeleteCustomizationWordRequest : RESTConnector.Request
    {
      public OnDeleteCustomizationWordCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Word { get; set; }
      public string Data { get; set; }
    }

    private void OnDeleteCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      if (((DeleteCustomizationWordRequest)req).Callback != null)
        ((DeleteCustomizationWordRequest)req).Callback(resp.Success, ((DeleteCustomizationWordRequest)req).Data);
    }
    #endregion

    #region Get Customization Word
    /// <summary>
    /// This callback is used by the GetCusomizationWord() function.
    /// </summary>
    /// <param name="translation">Translation of the requested word.</param>
    /// <param name="data">optional custom data.</param>
    public delegate void GetCustomizationWordCallback(Translation translation, string data);
    /// <summary>
    /// Returns the translation for a single word from the custom model with the specified `customization_id`. The output shows the translation as it is defined in the model. Only the owner of a custom voice model can use this method to query information about a word from the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The requested custom voice model's identifier.</param>
    /// <param name="word">The requested word.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool GetCustomizationWord(GetCustomizationWordCallback callback, string customizationID, string word, string customData = default(string))
    {
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("A customizationID is required to get a custom voice model's words.");
      if (string.IsNullOrEmpty(word))
        throw new ArgumentNullException("A word is requrred to get a translation.");

      GetCustomizationWordRequest req = new GetCustomizationWordRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Word = word;
      req.Data = customData;
      req.OnResponse = OnGetCustomizationWordResp;

      string service = "/v1/customizations/{0}/words/{1}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID, word));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class GetCustomizationWordRequest : RESTConnector.Request
    {
      public GetCustomizationWordCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Word { get; set; }
      public string Data { get; set; }
    }

    private void OnGetCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      Translation translation = new Translation();
      if (resp.Success)
      {
        try
        {
          fsData data = null;
          fsResult r = fsJsonParser.Parse(Encoding.UTF8.GetString(resp.Data), out data);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

          object obj = translation;
          r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
          if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);
        }
        catch (Exception e)
        {
          Log.Error("Text To Speech", "GetCustomizationWord Exception: {0}", e.ToString());
          resp.Success = false;
        }
      }

      if (((GetCustomizationWordRequest)req).Callback != null)
        ((GetCustomizationWordRequest)req).Callback(resp.Success ? translation : null, ((GetCustomizationWordRequest)req).Data);
    }
    #endregion

    #region Add Customization Word
    /// <summary>
    /// This callback is used by the AddCustomizationWord() function.
    /// </summary>
    /// <param name="success">Success</param>
    /// <param name="data">Optional custom data.</param>
    public delegate void AddCustomizationWordCallback(bool success, string data);

    /// <summary>
    /// Adds a single word and its translation to the custom voice model with the specified `customization_id`. A custom model can contain no more than 20,000 entries. Only the owner of a custom voice model can use this method to add a word to the model.
    /// Note: This method is currently a beta release that supports US English only.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="customizationID">The identifier of the custom voice model to be updated.</param>
    /// <param name="words">Words object to add to custom voice model.</param>
    /// <param name="customData">Optional custom data.</param>
    /// <returns></returns>
    public bool AddCustomizationWord(AddCustomizationWordCallback callback, string customizationID, string word, string translation, string customData = default(string))
    {
      Log.Error("TextToSpeech", "AddCustomizationWord is not supported. Unity WWW does not support PUT method! Use AddCustomizationWords() instead!");
      if (callback == null)
        throw new ArgumentNullException("callback");
      if (string.IsNullOrEmpty(customizationID))
        throw new ArgumentNullException("customizationID");
      if (string.IsNullOrEmpty(word))
        throw new ArgumentNullException("word");
      if (string.IsNullOrEmpty(translation))
        throw new ArgumentNullException("translation");

      string json = "{\n\t\"translation\":\"" + translation + "\"\n}";

      AddCustomizationWordRequest req = new AddCustomizationWordRequest();
      req.Callback = callback;
      req.CustomizationID = customizationID;
      req.Word = word;
      req.Translation = translation;
      req.Data = customData;
      req.Headers["Content-Type"] = "application/json";
      req.Headers["Accept"] = "application/json";
      req.Headers["X-HTTP-Method-Override"] = "PUT";
      req.Send = Encoding.UTF8.GetBytes(json);
      req.OnResponse = OnAddCustomizationWordResp;

      string service = "/v1/customizations/{0}/words/{1}";
      RESTConnector connector = RESTConnector.GetConnector(SERVICE_ID, string.Format(service, customizationID, word));
      if (connector == null)
        return false;

      return connector.Send(req);
    }

    private class AddCustomizationWordRequest : RESTConnector.Request
    {
      public AddCustomizationWordCallback Callback { get; set; }
      public string CustomizationID { get; set; }
      public string Word { get; set; }
      public string Translation { get; set; }
      public string Data { get; set; }
    }

    private void OnAddCustomizationWordResp(RESTConnector.Request req, RESTConnector.Response resp)
    {
      if (((AddCustomizationWordRequest)req).Callback != null)
        ((AddCustomizationWordRequest)req).Callback(resp.Success, ((AddCustomizationWordRequest)req).Data);
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
      private TextToSpeech m_Service = null;
      private ServiceStatus m_Callback = null;

      public CheckServiceStatus(TextToSpeech service, ServiceStatus callback)
      {
        m_Service = service;
        m_Callback = callback;

        if (!m_Service.GetVoices(OnCheckService))
          m_Callback(SERVICE_ID, false);
      }

      private void OnCheckService(Voices voices)
      {
        if (m_Callback != null && m_Callback.Target != null)
          m_Callback(SERVICE_ID, voices != null);
      }
    };
    #endregion
  }

}
