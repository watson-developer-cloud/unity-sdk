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

namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3
{
  /// <summary>
  /// Tone analyzer response.
  /// </summary>
  [fsObject]
  public class ToneAnalyzerResponse
  {
    /// <summary>
    /// Gets or sets the document tone.
    /// </summary>
    /// <value>The document tone.</value>
    public DocumentTone document_tone { get; set; }
    /// <summary>
    /// Gets or sets the sentences tone.
    /// </summary>
    /// <value>The sentences tone.</value>
    public SentenceTone[] sentences_tone { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.ToneAnalyzerResponse"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.ToneAnalyzerResponse"/>.</returns>
    public override string ToString()
    {
      string sentenceTone = (sentences_tone != null && sentences_tone.Length > 0) ? sentences_tone[0].ToString() : "No Sentence Tone";
      return string.Format("[ToneAnalyzerResponse: document_tone={0}, sentenceTone={1}]", document_tone, sentenceTone);
    }
  }

  /// <summary>
  /// Document tone.
  /// </summary>
  [fsObject]
  public class DocumentTone
  {
    /// <summary>
    /// Gets or sets the tone categories.
    /// </summary>
    /// <value>The tone categories.</value>
    public ToneCategory[] tone_categories { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.DocumentTone"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.DocumentTone"/>.</returns>
    public override string ToString()
    {
      string toneCategoryString = "";
      for (int i = 0; tone_categories != null && i < tone_categories.Length; i++)
      {
        toneCategoryString += tone_categories[i].ToString();
      }

      return string.Format("[DocumentTone: tone_categories={0}]", toneCategoryString);
    }
  }

  /// <summary>
  /// Tone category.
  /// </summary>
  [fsObject]
  public class ToneCategory
  {
    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    /// <value>The category identifier.</value>
    public string category_id { get; set; }
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    /// <value>The name of the category.</value>
    public string category_name { get; set; }
    /// <summary>
    /// Gets or sets the tones.
    /// </summary>
    /// <value>The tones.</value>
    public Tone[] tones { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.ToneCategory"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.ToneCategory"/>.</returns>
    public override string ToString()
    {
      string tonesString = "";
      for (int i = 0; tones != null && i < tones.Length; i++)
      {
        tonesString += tones[i].ToString();
      }

      return string.Format("[ToneCategory: category_id={0}, category_name={1}, tones={2}]", category_id, category_name, tonesString);
    }
  }

  /// <summary>
  /// Tone.
  /// </summary>
  [fsObject]
  public class Tone
  {
    /// <summary>
    /// Gets or sets the score.
    /// </summary>
    /// <value>The score.</value>
    public double score { get; set; }
    /// <summary>
    /// Gets or sets the tone identifier.
    /// </summary>
    /// <value>The tone identifier.</value>
    public string tone_id { get; set; }
    /// <summary>
    /// Gets or sets the name of the tone.
    /// </summary>
    /// <value>The name of the tone.</value>
    public string tone_name { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.Tone"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.Tone"/>.</returns>
    public override string ToString()
    {
      return string.Format("[Tone: score={0}, tone_id={1}, tone_name={2}]", score, tone_id, tone_name);
    }
  }

  /// <summary>
  /// Sentence tone.
  /// </summary>
  [fsObject]
  public class SentenceTone
  {
    /// <summary>
    /// Gets or sets the sentence identifier.
    /// </summary>
    /// <value>The sentence identifier.</value>
    public int sentence_id { get; set; }
    /// <summary>
    /// Gets or sets the input from.
    /// </summary>
    /// <value>The input from.</value>
    public int input_from { get; set; }
    /// <summary>
    /// Gets or sets the input to.
    /// </summary>
    /// <value>The input to.</value>
    public int input_to { get; set; }
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string text { get; set; }
    /// <summary>
    /// Gets or sets the tone categories.
    /// </summary>
    /// <value>The tone categories.</value>
    public ToneCategory[] tone_categories { get; set; }

    /// <summary>
    /// Gets the highest score.
    /// </summary>
    /// <value>The highest score.</value>
    private double m_HighestScore = -1;
    /// <summary>
    /// Returns the highest score.
    /// </summary>
    public double HighestScore
    {
      get
      {
        if (m_HighestScore < 0)
        {
          for (int i = 0; tone_categories != null && i < tone_categories.Length; i++)
          {
            for (int j = 0; tone_categories[i].tones != null && j < tone_categories[i].tones.Length; j++)
            {

              if (m_HighestScore < tone_categories[i].tones[j].score)
              {
                m_HighestScore = tone_categories[i].tones[j].score;
                m_HighestScoreToneName = tone_categories[i].tones[j].tone_name;
                m_HighestScoreToneCategoryName = tone_categories[i].category_name;
              }
            }

          }
        }
        return m_HighestScore;
      }
    }

    /// <summary>
    /// Gets the name of the highest score tone.
    /// </summary>
    /// <value>The name of the highest score tone.</value>
    private string m_HighestScoreToneName = null;
    /// <summary>
    /// The highest score tone name.
    /// </summary>
    public string HighestScoreToneName
    {
      get
      {
        return m_HighestScoreToneName;
      }
    }

    /// <summary>
    /// Gets the name of the highest score tone category.
    /// </summary>
    /// <value>The name of the highest score tone category.</value>
    private string m_HighestScoreToneCategoryName = null;
    /// <summary>
    /// The highest score category name.
    /// </summary>
    public string HighestScoreToneCategoryName
    {
      get
      {
        return m_HighestScoreToneCategoryName;
      }
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.SentenceTone"/>.
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current <see cref="IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v3.SentenceTone"/>.</returns>
    public override string ToString()
    {
      string toneCategoryString = "";
      for (int i = 0; tone_categories != null && i < tone_categories.Length; i++)
      {
        toneCategoryString += tone_categories[i].ToString();
      }

      return string.Format("[SentenceTone: sentence_id={0}, input_from={1}, input_to={2}, text={3}, tone_categories={4}]", sentence_id, input_from, input_to, text, toneCategoryString);
    }
  }
}
