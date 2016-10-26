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

namespace IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v1
{
  /// <summary>
  /// Language data class.
  /// </summary>
  [fsObject]
  public class Language
  {
    /// <summary>
    /// String that contains the country code.
    /// </summary>
    public string language { get; set; }
    /// <summary>
    /// The language name.
    /// </summary>
    public string name { get; set; }
  }
  /// <summary>
  /// Languages data class.
  /// </summary>
  [fsObject]
  public class Languages
  {
    /// <summary>
    /// Array of language objects.
    /// </summary>
    public Language[] languages { get; set; }
  }
  /// <summary>
  /// Translation data class.
  /// </summary>
  [fsObject]
  public class Translation
  {
    /// <summary>
    /// Translation text.
    /// </summary>
    public string translation { get; set; }
  };
  /// <summary>
  /// Translate data class returned by the TranslateCallback.
  /// </summary>
  [fsObject]
  public class Translations
  {
    /// <summary>
    /// Number of words in the translation.
    /// </summary>
    public long word_count { get; set; }
    /// <summary>
    /// Number of characters in the translation.
    /// </summary>
    public long character_count { get; set; }
    /// <summary>
    /// A array of translations.
    /// </summary>
    public Translation[] translations { get; set; }
  }
  /// <summary>
  /// Language model data class.
  /// </summary>
  [fsObject]
  public class TranslationModel
  {
    /// <summary>
    /// The language model ID.
    /// </summary>
    public string model_id { get; set; }
    /// <summary>
    /// The name of the language model.
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// The source language ID.
    /// </summary>
    public string source { get; set; }
    /// <summary>
    /// The target language ID.
    /// </summary>
    public string target { get; set; }
    /// <summary>
    /// The model of which this model was based.
    /// </summary>
    public string base_model_id { get; set; }
    /// <summary>
    /// The domain of the language model.
    /// </summary>
    public string domain { get; set; }
    /// <summary>
    /// Is this model customizable?
    /// </summary>
    public bool customizable { get; set; }
    /// <summary>
    /// Is this model default.
    /// </summary>
    public bool @default { get; set; }
    /// <summary>
    /// Who is the owner of this model.
    /// </summary>
    public string owner { get; set; }
    /// <summary>
    /// What is the status of this model.
    /// </summary>
    public string status { get; set; }
  }
  /// <summary>
  /// Models data class.
  /// </summary>
  [fsObject]
  public class TranslationModels
  {
    /// <summary>
    /// The array of TranslationModel objects.
    /// </summary>
    public TranslationModel[] models { get; set; }
  }
}
