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


using System;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1
{
    /// <summary>
    /// This data class holds the data for a given classifier returned by GetClassifier().
    /// </summary>
    [fsObject]
    public class Classifier
    {
        /// <summary>
        /// The name of the classifier.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The language ID of the classifier (e.g. en)
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The URL for the classifier.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The classifier ID.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// When was this classifier created.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Whats the current status of this classifier.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// A description of the classifier status.
        /// </summary>
        public string status_description { get; set; }
    };
    /// <summary>
    /// This data class wraps an array of Classifiers.
    /// </summary>
    [fsObject]
    public class Classifiers
    {
        /// <summary>
        /// An array of classifiers.
        /// </summary>
        public Classifier[] classifiers { get; set; }
    };
    /// <summary>
    /// A class returned by the ClassifyResult object.
    /// </summary>
    [fsObject]
    public class Class
    {
        /// <summary>
        /// The confidence in this class.
        /// </summary>
        public double confidence { get; set; }
        /// <summary>
        /// The name of the class.
        /// </summary>
        public string class_name { get; set; }
    };
    /// <summary>
    /// This result object is returned by the Classify() method.
    /// </summary>
    [fsObject]
    public class ClassifyResult
    {
        /// <summary>
        /// The ID of the classifier used.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// The URL of the classifier.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The input text into the classifier.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The top class found for the text.
        /// </summary>
        public string top_class { get; set; }
        /// <summary>
        /// A array of all classifications for the input text.
        /// </summary>
        public Class[] classes { get; set; }

        /// <summary>
        /// Helper function to return the top confidence value of all the returned classes.
        /// </summary>
        public double topConfidence
        {
            get
            {
                double fTop = 0.0;
                if (classes != null)
                {
                    foreach (var c in classes)
                        fTop = Math.Max(c.confidence, fTop);
                }
                return fTop;
            }
        }
    };
}
