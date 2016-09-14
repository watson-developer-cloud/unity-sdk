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
        /// <summary>
        /// The number of images processed.
        /// </summary>
        public int images_processed { get; set; }
        /// <summary>
        /// Array of classified images.
        /// </summary>
        public ClassifyTopLevelSingle[] images { get; set; }
        /// <summary>
        /// Array of warnings.
        /// </summary>
        public WarningInfo[] warnings { get; set; }
    }

    /// <summary>
    /// One classification.
    /// </summary>
    [fsObject]
    public class ClassifyTopLevelSingle
    {
        /// <summary>
        /// The source URL.
        /// </summary>
        public string source_url { get; set; }
        /// <summary>
        /// The resolved URL.
        /// </summary>
        public string resolved_url { get; set; }
        /// <summary>
        /// The Image.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The error.
        /// </summary>
        public ErrorInfoNoCode error { get; set; }
        /// <summary>
        /// The classification results.
        /// </summary>
        public ClassifyPerClassifier[] classifiers { get; set; }
    }

    /// <summary>
    /// One classifier.
    /// </summary>
    [fsObject]
    public class ClassifyPerClassifier
    {
        /// <summary>
        /// The name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The classifier identifier.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// Array of classification results.
        /// </summary>
        public ClassResult[] classes { get; set; }
    }

    /// <summary>
    /// One class result.
    /// </summary>
    [fsObject]
    public class ClassResult
    {
        /// <summary>
        /// The class result.
        /// </summary>
        [fsProperty("class")]
        public string m_class { get; set; }
        /// <summary>
        /// The score.
        /// </summary>
        public double score { get; set; }
        /// <summary>
        /// The type hierarchy.
        /// </summary>
        public string type_hierarchy { get; set; }
    }

    /// <summary>
    /// The classify parameters.
    /// </summary>
    [fsObject]
    public class ClassifyParameters
    {
        /// <summary>
        /// The URL.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The clasifier identifiers.
        /// </summary>
        public string[] classifier_ids { get; set; }
        /// <summary>
        /// The owners.
        /// </summary>
        public string[] owners { get; set; }
        /// <summary>
        /// The classification threshold.
        /// </summary>
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
        /// <summary>
        /// Number of images processed.
        /// </summary>
        public int images_processed { get; set; }
        /// <summary>
        /// Array of face classifications.
        /// </summary>
        public FacesTopLevelSingle[] images { get; set; }
        /// <summary>
        /// Warning info.
        /// </summary>
        public WarningInfo[] warnings { get; set; }
    }

    /// <summary>
    /// One face classification.
    /// </summary>
    [fsObject]
    public class FacesTopLevelSingle
    {
        /// <summary>
        /// The source URL.
        /// </summary>
        public string source_url { get; set; }
        /// <summary>
        /// The resolved URL.
        /// </summary>
        public string resolved_url { get; set; }
        /// <summary>
        /// The image.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The error.
        /// </summary>
        public ErrorInfoNoCode error { get; set; }
        /// <summary>
        /// The face results.
        /// </summary>
        public OneFaceResult[] faces { get; set; }
    }

    /// <summary>
    /// One face result.
    /// </summary>
    [fsObject]
    public class OneFaceResult
    {
        /// <summary>
        /// The face age.
        /// </summary>
        public Age age { get; set; }
        /// <summary>
        /// The face gender.
        /// </summary>
        public Gender gender { get; set; }
        /// <summary>
        /// The face location in pixels.
        /// </summary>
        public FaceLocation face_location { get; set; }
        /// <summary>
        /// The face identity.
        /// </summary>
        public Identity identity { get; set; }
    }

    /// <summary>
    /// Detect faces parameters.
    /// </summary>
    [fsObject]
    public class DetectFacesParameters
    {
        /// <summary>
        /// The face URL.
        /// </summary>
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
        /// <summary>
        /// Number of images processed.
        /// </summary>
        public int images_processed { get; set; }
        /// <summary>
        /// Array of text image classifications.
        /// </summary>
        public TextRecogTopLevelSingle[] images { get; set; }
        /// <summary>
        /// The warnings.
        /// </summary>
        public WarningInfo[] warnings { get; set; }
    }

    /// <summary>
    /// One text page.
    /// </summary>
    [fsObject]
    public class TextRecogTopLevelSingle
    {
        /// <summary>
        /// The source URL.
        /// </summary>
        public string source_url { get; set; }
        /// <summary>
        /// The resolved URL.
        /// </summary>
        public string resolved_url { get; set; }
        /// <summary>
        /// The image.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The error.
        /// </summary>
        public ErrorInfoNoCode error { get; set; }
        /// <summary>
        /// The text.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The words.
        /// </summary>
        public TextRecogOneWord[] words { get; set; }
    }

    /// <summary>
    /// One word.
    /// </summary>
    [fsObject]
    public class TextRecogOneWord
    {
        /// <summary>
        /// The word.
        /// </summary>
        public string word { get; set; }
        /// <summary>
        /// The word location in pixels.
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// The classification score.
        /// </summary>
        public double score { get; set; }
        /// <summary>
        /// The line number.
        /// </summary>
        public double line_number { get; set; }
    }

    /// <summary>
    /// Word location.
    /// </summary>
    [fsObject]
    public class Location
    {
        /// <summary>
        /// The location width.
        /// </summary>
        public double width { get; set; }
        /// <summary>
        /// The location height.
        /// </summary>
        public double height { get; set; }
        /// <summary>
        /// The location left.
        /// </summary>
        public double left { get; set; }
        /// <summary>
        /// The loction top.
        /// </summary>
        public double top { get; set; }
    }

    /// <summary>
    /// Recognize text parameters.
    /// </summary>
    [fsObject]
    public class RecognizeTextParameters
    {
        /// <summary>
        /// The URL.
        /// </summary>
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
        /// <summary>
        /// Array of classifiers.
        /// </summary>
        public GetClassifiersPerClassifierBrief[] classifiers { get; set; }
    }

    /// <summary>
    /// Classifier breif.
    /// </summary>
    [fsObject]
    public class GetClassifiersPerClassifierBrief
    {
        /// <summary>
        /// The classifier identifier.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// The classifier name.
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// Classifier verbose.
    /// </summary>
    [fsObject]
    public class GetClassifiersPerClassifierVerbose
    {
        /// <summary>
        /// The classifier identifier.
        /// </summary>
        public string classifier_id { get; set; }
        /// <summary>
        /// The classifier name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The classifier owner.
        /// </summary>
        public string owner { get; set; }
        /// <summary>
        /// The classifier status.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The classifier explanation.
        /// </summary>
        public string explanation { get; set; }
        /// <summary>
        /// The classifier created.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// Array of classes.
        /// </summary>
        public Class[] classes { get; set; }
    }

    /// <summary>
    /// The class.
    /// </summary>
    [fsObject]
    public class Class
    {
        /// <summary>
        /// The class.
        /// </summary>
        [fsProperty("class")]
        public string m_Class { get; set; }
    }
    #endregion

    #region Similarity Search
    /// <summary>
    /// Collecitons response object.
    /// </summary>
    [fsObject]
    public class GetCollections
    {
        /// <summary>
        /// Array of collections.
        /// </summary>
        public CreateCollection[] collections;
    }

    /// <summary>
    /// A collection.
    /// </summary>
    [fsObject]
    public class CreateCollection
    {
        /// <summary>
        /// The ID of the new collection.
        /// </summary>
        public string collection_id { get; set; }
        /// <summary>
        /// The ID of the new collection.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The ID of the new collection.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// The ID of the new collection.
        /// </summary>
        public int images { get; set; }
        /// <summary>
        /// The status of collection creation. Returns available when the collection is available to add images, and unavailable when the collection is being created or trained.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The number of images possible in the collection. Each collection can contain 1000000 images.
        /// </summary>
        public string capacity { get; set; }
    }

    /// <summary>
    /// Collections brief object.
    /// </summary>
    [fsObject]
    public class GetCollectionImages
    {
        /// <summary>
        /// Array of collections.
        /// </summary>
        public GetCollectionsBrief[] images { get; set; }
    }

    /// <summary>
    /// Collection brief object.
    /// </summary>
    [fsObject]
    public class GetCollectionsBrief
    {
        /// <summary>
        /// The unique ID of the image. Save this to add or remove it from the collection.
        /// </summary>
        public string image_id { get; set; }
        /// <summary>
        /// Date the image was added to the collection.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// File name of the image.
        /// </summary>
        public string image_file { get; set; }
        /// <summary>
        /// Metadat JSON object (key value pairs).
        /// </summary>
        public object metadata { get; set; }
    }

    /// <summary>
    /// The collections config
    /// </summary>
    [fsObject]
    public class CollectionsConfig
    {
        /// <summary>
        /// Array of collection images config.
        /// </summary>
        public CollectionImagesConfig[] images { get; set; }
        /// <summary>
        /// The number of images processed in this call.
        /// </summary>
        public int images_processed { get; set; }
    }

    /// <summary>
    /// The collection config.
    /// </summary>
    [fsObject]
    public class CollectionImagesConfig
    {
        /// <summary>
        /// The unique ID of the image. Save this to add or remove it from the collection.
        /// </summary>
        public string image_id { get; set; }
        /// <summary>
        /// Date the image was added to the collection.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// File name of the image.
        /// </summary>
        public string image_file { get; set; }
        /// <summary>
        /// Metadat JSON object (key value pairs).
        /// </summary>
        public object metadata { get; set; }
    }

    /// <summary>
    /// Similar images result.
    /// </summary>
    [fsObject]
    public class SimilarImagesConfig
    {
        /// <summary>
        /// The similar images.
        /// </summary>
        public SimilarImageConfig[] similar_images { get; set; }
        /// <summary>
        /// The number of images processed in this call.
        /// </summary>
        public int images_processed { get; set; }
    }

    /// <summary>
    /// Similar image result.
    /// </summary>
    [fsObject]
    public class SimilarImageConfig
    {
        /// <summary>
        /// The unique ID of the image. Save this to add or remove it from the collection.
        /// </summary>
        public string image_id { get; set; }
        /// <summary>
        /// Date the image was added to the collection.
        /// </summary>
        public string created { get; set; }
        /// <summary>
        /// File name of the image.
        /// </summary>
        public string image_file { get; set; }
        /// <summary>
        /// Metadat JSON object (key value pairs).
        /// </summary>
        public object metadata { get; set; }
        /// <summary>
        /// Confidence in the match.
        /// </summary>
        public float score { get; set; }
    }
    #endregion

    #region Common
    /// <summary>
    /// Warning info.
    /// </summary>
    [fsObject]
    public class WarningInfo
    {
		/// <summary>
		/// The warning identifier.
		/// </summary>
        public string warning_id { get; set; }
		/// <summary>
		/// The warning description.
		/// </summary>
        public string description { get; set; }
    }

	/// <summary>
	/// Error info.
	/// </summary>
    [fsObject]
    public class ErrorInfoNoCode
    {
		/// <summary>
		/// The error identifier.
		/// </summary>
        public string error_id { get; set; }
		/// <summary>
		/// The error description.
		/// </summary>
        public string description { get; set; }
    }

	/// <summary>
	/// Age of the face.
	/// </summary>
    [fsObject]
    public class Age
    {
		/// <summary>
		/// The minimum age.
		/// </summary>
        public int min { get; set; }
		/// <summary>
		/// The maximum age.
		/// </summary>
        public int max { get; set; }
		/// <summary>
		/// The age classification score.
		/// </summary>
        public double score { get; set; }
    }

	/// <summary>
	/// Gender of the face.
	/// </summary>
    [fsObject]
    public class Gender
    {
		/// <summary>
		/// The gener.
		/// </summary>
        public string gender { get; set; }
		/// <summary>
		/// The gender classification score.
		/// </summary>
        public double score { get; set; }
    }

	/// <summary>
	/// Location of the face.
	/// </summary>
    [fsObject]
    public class FaceLocation
    {
		/// <summary>
		/// The face location width.
		/// </summary>
        public double width { get; set; }
		/// <summary>
		/// The face location height.
		/// </summary>
        public double height { get; set; }
		/// <summary>
		/// The face location left.
		/// </summary>
        public double left { get; set; }
		/// <summary>
		/// The face location top.
		/// </summary>
        public double top { get; set; }
    }

	/// <summary>
	/// Identity of the face.
	/// </summary>
    [fsObject]
    public class Identity
    {
		/// <summary>
		/// The name.
		/// </summary>
        public string name { get; set; }
		/// <summary>
		/// The identity classification score.
		/// </summary>
        public double score { get; set; }
		/// <summary>
		/// The identity classification type hierarchy.
		/// </summary>
        public string type_hierarchy { get; set; }
    }

	/// <summary>
	/// The Visual Recognition version.
	/// </summary>
    public class VisualRecognitionVersion
    {
		/// <summary>
		/// The version.
		/// </summary>
        public const string Version = "2016-05-20";
    }
    #endregion
}
