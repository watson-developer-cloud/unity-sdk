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
    public class ClassifiedImages
    {
        /// <summary>
        /// The number of custom classes identified in the images.
        /// </summary>
        public int custom_classes { get; set; }
        /// <summary>
        /// The number of images processed.
        /// </summary>
        public int images_processed { get; set; }
        /// <summary>
        /// Array of classified images.
        /// </summary>
        public ClassifiedImage[] images { get; set; }
        /// <summary>
        /// Array of warnings.
        /// </summary>
        public WarningInfo[] warnings { get; set; }
    }

    /// <summary>
    /// One classification.
    /// </summary>
    [fsObject]
    public class ClassifiedImage
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
        public ErrorInfo error { get; set; }
        /// <summary>
        /// The classification results.
        /// </summary>
        public ClassifierResult[] classifiers { get; set; }
    }

    /// <summary>
    /// One classifier.
    /// </summary>
    [fsObject]
    public class ClassifierResult
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
        public string _class { get; set; }
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
    public class DetectedFaces
    {
        /// <summary>
        /// Number of images processed.
        /// </summary>
        public int images_processed { get; set; }
        /// <summary>
        /// Array of face classifications.
        /// </summary>
        public ImageWithFaces[] images { get; set; }
        /// <summary>
        /// Warning info.
        /// </summary>
        public WarningInfo[] warnings { get; set; }
    }

    /// <summary>
    /// One face classification.
    /// </summary>
    [fsObject]
    public class ImageWithFaces
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
        public ErrorInfo error { get; set; }
        /// <summary>
        /// The face results.
        /// </summary>
        public Face[] faces { get; set; }
    }

    /// <summary>
    /// One face result.
    /// </summary>
    [fsObject]
    public class Face
    {
        /// <summary>
        /// The face age.
        /// </summary>
        public FaceAge age { get; set; }
        /// <summary>
        /// The face gender.
        /// </summary>
        public FaceGender gender { get; set; }
        /// <summary>
        /// The face location in pixels.
        /// </summary>
        public FaceLocation face_location { get; set; }
    }
    #endregion

    #region Classifiers
    /// <summary>
    /// Classifiers.
    /// </summary>
    [fsObject]
    public class ClassifiersBrief
    {
        /// <summary>
        /// Array of classifiers.
        /// </summary>
        public ClassifierBrief[] classifiers { get; set; }
    }

    /// <summary>
    /// Classifiers.
    /// </summary>
    [fsObject]
    public class ClassifiersVerbose
    {
        /// <summary>
        /// Array of classifiers.
        /// </summary>
        public ClassifierVerbose[] classifiers { get; set; }
    }

    /// <summary>
    /// Classifier brief.
    /// </summary>
    [fsObject]
    public class ClassifierBrief
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
    public class ClassifierVerbose
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
        /// Whether the classifier can be downloaded as a Core ML model after the training status is ready.
        /// </summary>
        public bool core_ml_enabled { get; set; }
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
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the classifier was updated. Returned when verbose=true. Might not be returned by some requests. Identical to updated and retained for backward compatibility. 
        /// </summary>
        public string retrained { get; set; }
        /// <summary>
        /// Date and time in Coordinated Universal Time (UTC) that the classifier was most recently updated. The field matches either retrained or created. Returned when verbose=true. Might not be returned by some requests.
        /// </summary>
        public string updated { get; set; }
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
        public string _class { get; set; }
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
    public class ErrorInfo
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
    public class FaceAge
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
    public class FaceGender
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
    #endregion
}
