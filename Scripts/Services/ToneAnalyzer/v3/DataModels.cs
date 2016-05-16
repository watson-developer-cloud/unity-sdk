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
   
    #region Tone
    [fsObject]
    public class ToneAnalysis
    {
        public object document_tone { get; set; }
        public SentenceAnalysis[] sentences_tone { get; set; }

        public override string ToString()
        {
            string sentenceTone = ( sentences_tone != null && sentences_tone.Length > 0 )? sentences_tone[0].ToString() : "No Sentence Tone";
            return string.Format("[ToneAnalyzerResponse: document_tone={0}, sentenceTone={1}]", document_tone, sentenceTone);
        }
    }

    [fsObject]
    public class SentenceAnalysis
    {
        public int sentence_id { get; set; }
        public int input_from { get; set; }
        public int input_to { get; set; }
        public string text { get; set; }
        public ToneCategory[] tone_categories { get; set; }
    }

    [fsObject]
    public class ToneCategory
    {
        public string category_name { get; set; }
        public string category_id { get; set; }
        public ToneScore[] tones { get; set; }
    }

    [fsObject]
    public class ToneScore
    {
        public string tone_name { get; set; }
        public string tone_id { get; set; }
        public double score { get; set; }
    }
    #endregion
}
