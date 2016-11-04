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

namespace IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1
{
  /// <summary>
  /// Audio format types that can be requested from the service.
  /// </summary>
  public enum AudioFormatType
  {
    /// <summary>
    /// OGG Vorbis format
    /// </summary>
    OGG = 0,
    /// <summary>
    /// Linear PCM format.
    /// </summary>
    WAV,                     //Currently used
                             /// <summary>
                             /// FLAC audio format.
                             /// </summary>
    FLAC
  };

  #region Voices
  /// <summary>
  /// The available voices for synthesized speech.
  /// </summary>
  public enum VoiceType
  {
    /// <summary>
    /// US male voice.
    /// </summary>
    en_US_Michael = 0,
    /// <summary>
    /// US female voice.
    /// </summary>
    en_US_Lisa,
    /// <summary>
    /// US female voice.
    /// </summary>
    en_US_Allison,
    /// <summary>
    /// Great Britan female voice.
    /// </summary>
    en_GB_Kate,
    /// <summary>
    /// Spanish male voice.
    /// </summary>
    es_ES_Enrique,
    /// <summary>
    /// Spanish female voice.
    /// </summary>
    es_ES_Laura,
    /// <summary>
    /// US female voice.
    /// </summary>
    es_US_Sofia,
    /// <summary>
    /// German male voice.
    /// </summary>
    de_DE_Dieter,
    /// <summary>
    /// German female voice.
    /// </summary>
    de_DE_Birgit,
    /// <summary>
    /// French female voice.
    /// </summary>
    fr_FR_Renee,
    /// <summary>
    /// Italian female voice.
    /// </summary>
    it_IT_Francesca,
    /// <summary>
    /// Japanese female voice.
    /// </summary>
    ja_JP_Emi,
    /// <summary>
    /// Brazilian Portugese female voice.
    /// </summary>
    pt_BR_Isabela,
  };

  /// <summary>
  /// A voice model object for TextToSpeech.
  /// </summary>
  [fsObject]
  public class Voice
  {
    /// <summary>
    /// The name of the voice model.
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// The language ID of this voice model.
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// The gender of the voice model.
    /// </summary>
    public string gender { get; set; }
    /// <summary>
    /// The URL of the voice model.
    /// </summary>
    public string url { get; set; }
    /// <summary>
    /// Textual description of the voice.
    /// </summary>
    public string description { get; set; }
    /// <summary>
    /// If true, the voice can be customized; if false, the voice cannot be customized.
    /// </summary>
    public bool customizable { get; set; }
  };

  /// <summary>
  /// This object contains a list of voices.
  /// </summary>
  [fsObject]
  public class Voices
  {
    /// <summary>
    /// The array of Voice objects.
    /// </summary>
    public Voice[] voices { get; set; }

    /// <summary>
    /// Check to see if object has data.
    /// </summary>
    /// <returns>True if has voices, False if no voices.</returns>
    public bool HasData()
    {
      return voices != null && voices.Length > 0;
    }
  };
  #endregion

  #region Pronunciation
  /// <summary>
  /// This object contains the pronunciation.
  /// </summary>
  [fsObject]
  public class Pronunciation
  {
    /// <summary>
    /// The pronunciation.
    /// </summary>
    public string pronunciation { get; set; }
  }
  #endregion

  #region Customization
  /// <summary>
  /// Customizations for voice models.
  /// </summary>
  [fsObject]
  public class Customizations
  {
    /// <summary>
    /// A list of voice model customizations.
    /// </summary>
    public Customization[] customizations { get; set; }

    public bool HasData()
    {
      return customizations != null && customizations.Length > 0;
    }
  }

  /// <summary>
  /// A single voice model customization.
  /// </summary>
  [fsObject]
  public class Customization
  {
    /// <summary>
    /// GUID of the custom voice model
    /// </summary>
    public string customization_id { get; set; }
    /// <summary>
    /// Name of the custom voice model
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Language of the custom voice model. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// GUID of the service credentials for the owner of the custom voice model.
    /// </summary>
    public string owner { get; set; }
    /// <summary>
    /// UNIX timestamp that indicates when the custom voice model was created. The timestamp is a count of seconds since the UNIX Epoch of January 1, 1970 Coordinated Universal Time (UTC).
    /// </summary>
    public double created { get; set; }
    /// <summary>
    /// UNIX timestamp that indicates when the custom voice model was last modified. Equals `created` when a new voice model is first added but has yet to be changed.
    /// </summary>
    public double last_modified { get; set; }
    /// <summary>
    /// Description of the custom voice model.
    /// </summary>
    public string description { get; set; }

    public bool HasData()
    {
      return !string.IsNullOrEmpty(customization_id);
    }
  }

  /// <summary>
  /// A data object containing a customization ID when creating a new voice model.
  /// </summary>
  [fsObject]
  public class CustomizationID
  {
    /// <summary>
    /// GUID of the new custom voice model.
    /// </summary>
    public string customization_id { get; set; }
  }

  /// <summary>
  /// A data object that contains data to create a new empty custom voice mode3l.
  /// </summary>
  [fsObject]
  public class CustomVoice
  {
    /// <summary>
    /// Name of the new custom voice model.
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Language of the new custom voice model. Omit the parameter to use the default language, en-US. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// Description of the new custom voice model.
    /// </summary>
    public string description { get; set; }
  }

  [fsObject]
  public class CustomizationWords
  {
    /// <summary>
    /// GUID of the custom voice model.
    /// </summary>
    public string customization_id { get; set; }
    /// <summary>
    /// Name of the custom voice model
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Language of the custom voice model. = ['de-DE', 'en-US', 'en-GB', 'es-ES', 'es-US', 'fr-FR', 'it-IT', 'ja-JP', 'pt-BR'].
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// GUID of the service credentials for the owner of the custom voice model.
    /// </summary>
    public string owner { get; set; }
    /// <summary>
    /// UNIX timestamp that indicates when the custom voice model was created. The timestamp is a count of seconds since the UNIX Epoch of January 1, 1970 Coordinated Universal Time (UTC).
    /// </summary>
    public double created { get; set; }
    /// <summary>
    /// UNIX timestamp that indicates when the custom voice model was last modified. Equals created when the new voice model is first added but has yet to be changed.
    /// </summary>
    public double last_modified { get; set; }
    /// <summary>
    /// Description of the custom voice model.
    /// </summary>
    public string description { get; set; }
    /// <summary>
    /// List of words and their translations from the custom voice model.
    /// </summary>
    public Word[] words { get; set; }
  }

  /// <summary>
  /// Words object.
  /// </summary>
  [fsObject]
  public class Words
  {
    /// <summary>
    /// An array of Words.
    /// </summary>
    public Word[] words { get; set; }

    /// <summary>
    /// Check to see if there are any words.
    /// </summary>
    /// <returns>True if there are words, false if there ar no words.</returns>
    public bool HasData()
    {
      return words != null && words.Length > 0;
    }
  }

  /// <summary>
  /// The Word object.
  /// </summary>
  [fsObject]
  public class Word
  {
    /// <summary>
    /// Word from the custom voice model.
    /// </summary>
    public string word { get; set; }
    /// <summary>
    /// Phonetic or sounds-like translation for the word. A phonetic translation is based on the SSML format for representing the phonetic string of a word either as an IPA or IBM SPR translation. A sounds-like translation consists of one or more words that, when combined, sound like the word.
    /// </summary>
    public string translation { get; set; }
  }

  /// <summary>
  /// Update information for a custom voice model.
  /// </summary>
  [fsObject]
  public class CustomVoiceUpdate
  {
    /// <summary>
    /// Name of the new custom voice model.
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// Description of the new custom voice model.
    /// </summary>
    public string description { get; set; }
    /// <summary>
    /// List of words and their translations to be added to or updated in the custom voice model. Send an empty array to make no additions or updates.
    /// </summary>
    public Word[] words { get; set; }

    /// <summary>
    /// Check to see if there are any words to update.
    /// </summary>
    /// <returns>True if there are words, false if there are none.</returns>
    public bool HasData()
    {
      return words != null && words.Length > 0;
    }
  }

  /// <summary>
  /// Single word translation for a custom voice model.
  /// </summary>
  [fsObject]
  public class Translation
  {
    /// <summary>
    /// Phonetic or sounds-like translation for the word. A phonetic translation is based on the SSML format for representing the phonetic string of a word either as an IPA translation or as an IBM SPR translation. A sounds-like is one or more words that, when combined, sound like the word.
    /// </summary>
    public string translation { get; set; }
  }
  #endregion

  #region Errors
  /// <summary>
  /// The error response.
  /// </summary>
  [fsObject]
  public class ErrorModel
  {
    /// <summary>
    /// Description of the problem.
    /// </summary>
    public string error { get; set; }
    /// <summary>
    /// HTTP response code.
    /// </summary>
    public int code { get; set; }
    /// <summary>
    /// The response message.
    /// </summary>
    public string code_description { get; set; }
  }
  #endregion
}
