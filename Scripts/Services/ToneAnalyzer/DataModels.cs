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

namespace IBM.Watson.DeveloperCloud.Services.ToneAnalyzer.v1
{
   

    [fsObject]
    public class ToneAnalyzerResponse
    {
        public DocumentTone document_tone { get; set;}
        public SentenceTone[] sentences_tone { get; set;}

        public override string ToString()
        {
            string sentenceTone = ( sentences_tone != null && sentences_tone.Length > 0 )? sentences_tone[0].ToString() : "No Sentence Tone";
            return string.Format("[ToneAnalyzerResponse: document_tone={0}, sentenceTone={1}]", document_tone, sentenceTone);
        }
    };

    [fsObject]
    public class DocumentTone
    {
        public ToneCategory[] tone_categories { get; set;}

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

    [fsObject]
    public class ToneCategory
    {
        public string category_id { get; set;}
        public string category_name { get; set;}
        public Tone[] tones { get; set;}

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

    [fsObject]
    public class Tone
    {
        public double score { get; set;}
        public string tone_id { get; set;}
        public string tone_name { get; set;}

        public override string ToString()
        {
            return string.Format("[Tone: score={0}, tone_id={1}, tone_name={2}]", score, tone_id, tone_name);
        }
    }

    [fsObject]
    public class SentenceTone
    {
        public int sentence_id { get; set;}
        public int input_from { get; set;}
        public int input_to { get; set;}
        public string text { get; set;}
        public ToneCategory[] tone_categories { get; set;}

        private double m_HighestScore = -1;
        public double HighestScore
        {
            get
            {
                if(m_HighestScore < 0){
                    for (int i = 0; tone_categories != null && i < tone_categories.Length; i++)
                    {
                        for (int j = 0;  tone_categories[i].tones != null && j < tone_categories[i].tones.Length; j++) {

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

        private string m_HighestScoreToneName = null;
        public string HighestScoreToneName
        {
            get
            {
                if (string.IsNullOrEmpty(m_HighestScoreToneName))
                {
                    double testScore = HighestScore;
                }
                
                return m_HighestScoreToneName;
            }
        }

        private string m_HighestScoreToneCategoryName = null;
        public string HighestScoreToneCategoryName
        {
            get
            {
                if (string.IsNullOrEmpty(m_HighestScoreToneCategoryName))
                {
                    double testScore = HighestScore;
                }

                return m_HighestScoreToneCategoryName;
            }
        }

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


    /*
    {
        "document_tone": {
            "tone_categories": [
                    {
                        "tones": [
                                {
                                    "score": 0.25482,
                                    "tone_id": "anger",
                                    "tone_name": "Anger"
                                },
                                {
                                    "score": 0.345816,
                                    "tone_id": "disgust",
                                    "tone_name": "Disgust"
                                },
                                {
                                    "score": 0.121116,
                                    "tone_id": "fear",
                                    "tone_name": "Fear"
                                },
                                {
                                    "score": 0.078903,
                                    "tone_id": "joy",
                                    "tone_name": "Joy"
                                },
                                {
                                    "score": 0.199345,
                                    "tone_id": "sadness",
                                    "tone_name": "Sadness"
                                }
                        ],
                        "category_id": "emotion_tone",
                        "category_name": "Emotion Tone"
                    },
                    {
                        "tones": [
                                {
                                    "score": 0.999,
                                    "tone_id": "analytical",
                                    "tone_name": "Analytical"
                                },
                                {
                                    "score": 0.999,
                                    "tone_id": "confident",
                                    "tone_name": "Confident"
                                },
                                {
                                    "score": 0.694,
                                    "tone_id": "tentative",
                                    "tone_name": "Tentative"
                                }
                        ],
                        "category_id": "writing_tone",
                        "category_name": "Writing Tone"
                    },
                    {
                        "tones": [
                                {
                                    "score": 0.271,
                                    "tone_id": "openness_big5",
                                    "tone_name": "Openness"
                                },
                                {
                                    "score": 0.11,
                                    "tone_id": "conscientiousness_big5",
                                    "tone_name": "Conscientiousness"
                                },
                                {
                                    "score": 0.844,
                                    "tone_id": "extraversion_big5",
                                    "tone_name": "Extraversion"
                                },
                                {
                                    "score": 0.257,
                                    "tone_id": "agreeableness_big5",
                                    "tone_name": "Agreeableness"
                                },
                                {
                                    "score": 0.497,
                                    "tone_id": "neuroticism_big5",
                                    "tone_name": "Emotional Range"
                                }
                        ],
                        "category_id": "social_tone",
                        "category_name": "Social Tone"
                    }
            ]
        }

    "sentences_tone": [
        {
          "sentence_id": 0,
          "input_from": 0,
          "input_to": 0,
          "text": "string",
          "tone_categories": [
            {
              "category_name": "string",
              "category_id": "string",
              "tones": [
                {
                  "tone_name": "string",
                  "tone_id": "string",
                  "tone_category_name": "string",
                  "tone_category_id": "string",
                  "score": 0
                }
              ]
            }
          ]
    }
    */
}
