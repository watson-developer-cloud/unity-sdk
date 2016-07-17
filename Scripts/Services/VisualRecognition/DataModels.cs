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

namespace IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3
{
    #region Classify
	/// <summary>
	/// Holds multiple classifications.
	/// </summary>
    [fsObject]
    public class ClassifyTopLevelMultiple
    {
        public int images_processed { get; set; }
        public ClassifyTopLevelSingle[] images { get; set; }
        public WarningInfo[] warnings { get; set; }
    }

	/// <summary>
	/// One classification.
	/// </summary>
    [fsObject]
    public class ClassifyTopLevelSingle
    {
        public string source_url { get; set; }
        public string resolved_url { get; set; }
        public string image { get; set; }
        public ErrorInfoNoCode error { get; set; }
        public ClassifyPerClassifier[] classifiers { get; set; }
    }

	/// <summary>
	/// One classifier.
	/// </summary>
    [fsObject]
    public class ClassifyPerClassifier
    {
        public string name { get; set; }
        public string classifier_id { get; set; }
        public ClassResult[] classes { get; set; }
    }

	/// <summary>
	/// One class result.
	/// </summary>
    [fsObject]
    public class ClassResult
    {
        [fsProperty("class")]
        public string m_class { get; set; }
        public string score { get; set; }
        public string type_hierarchy { get; set; }
    }

	/// <summary>
	/// The classify parameters.
	/// </summary>
    [fsObject]
    public class ClassifyParameters
    {
        public string url { get; set; }
        public string[] classifier_ids { get; set; }
        public string[] owners { get; set; }
        public float threshold { get; set; }
    }
    #endregion

    #region Detect Faces
	/// <summary>
	/// Multiple faces.
	/// </summary>
    [fsObject]
    public class FacesTopLevelMultiple
    {
        public int images_processed { get; set; }
        public FacesTopLevelSingle[] images { get; set; }
        public WarningInfo[] warnings { get; set; }
    }

	/// <summary>
	/// One face classification.
	/// </summary>
    [fsObject]
    public class FacesTopLevelSingle
    {
        public string source_url { get; set; }
        public string resolved_url { get; set; }
        public string image { get; set; }
        public ErrorInfoNoCode error { get; set; }
        public OneFaceResult[] faces { get; set; }
    }

	/// <summary>
	/// One face result.
	/// </summary>
    [fsObject]
    public class OneFaceResult
    {
        public Age age { get; set; }
        public Gender gender { get; set; }
        public FaceLocation face_location { get; set; }
        public Identity identity { get; set; }
    }

	/// <summary>
	/// Detect faces parameters.
	/// </summary>
    [fsObject]
    public class DetectFacesParameters
    {
        public string url { get; set; }
    }
    #endregion

    #region Recognize Text
	/// <summary>
	/// Mulitple text pages.
	/// </summary>
    [fsObject]
    public class TextRecogTopLevelMultiple
    {
        public int images_processed { get; set; }
        public TextRecogTopLevelSingle[] images { get; set; }
        public WarningInfo[] warnings { get; set; }
    }

	/// <summary>
	/// One text page.
	/// </summary>
    [fsObject]
    public class TextRecogTopLevelSingle
    {
        public string source_url { get; set; }
        public string resolved_url { get; set; }
        public string image { get; set; }
        public ErrorInfoNoCode error { get; set; }
        public string text { get; set; }
        public TextRecogOneWord[] words { get; set; }
    }

	/// <summary>
	/// One word.
	/// </summary>
    [fsObject]
    public class TextRecogOneWord
    {
        public string word { get; set; }
        public Location location { get; set; }
        public double score { get; set; }
        public double line_number { get; set; }
    }

	/// <summary>
	/// Word location.
	/// </summary>
    [fsObject]
    public class Location
    {
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
    }

	/// <summary>
	/// Recognize text parameters.
	/// </summary>
    [fsObject]
    public class RecognizeTextParameters
    {
        public string url { get; set; }
    }
    #endregion

    #region Classifiers
	/// <summary>
	/// Classifiers breif.
	/// </summary>
    [fsObject]
    public class GetClassifiersTopLevelBrief
    {
        public GetClassifiersPerClassifierBrief[] classifiers { get; set; }
    }

	/// <summary>
	/// Classifier breif.
	/// </summary>
    [fsObject]
    public class GetClassifiersPerClassifierBrief
    {
        public string classifier_id { get; set; }
        public string name { get; set; }
    }

	/// <summary>
	/// Classifier verbose.
	/// </summary>
    [fsObject]
    public class GetClassifiersPerClassifierVerbose
    {
        public string classifier_id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public string status { get; set; }
        public string explanation { get; set; }
        public string created { get; set; }
        public Class[] classes { get; set; }
    }

	/// <summary>
	/// The class.
	/// </summary>
    [fsObject]
    public class Class
    {
        [fsProperty("class")]
        public string m_Class { get; set; }
    }
    #endregion

    #region Common
    [fsObject]
    public class WarningInfo
    {
        public string warning_id { get; set; }
        public string description { get; set; }
    }

    [fsObject]
    public class ErrorInfoNoCode
    {
        public string error_id { get; set; }
        public string description { get; set; }
    }

    [fsObject]
    public class Age
    {
        public int min { get; set; }
        public int max { get; set; }
        public double score { get; set; }
    }

    [fsObject]
    public class Gender
    {
        public string gender { get; set; }
        public double score { get; set; }
    }

    [fsObject]
    public class FaceLocation
    {
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
    }

    [fsObject]
    public class Identity
    {
        public string name { get; set; }
        public double score { get; set; }
        public string type_hierarchy { get; set; }
    }

    public class VisualRecognitionVersion
    {
        public const string Version = "2016-05-20";
    }
    #endregion
}
