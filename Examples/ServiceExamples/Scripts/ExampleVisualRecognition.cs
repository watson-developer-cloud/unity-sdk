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
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

public class ExampleVisualRecognition : MonoBehaviour {
    private VisualRecognition m_VisualRecognition = new VisualRecognition();
    private string m_classifierName = "integration-test-classifier";
    private string m_classifierID = "integrationtestclassifier_1745947114";
    private string m_classifierToDelete = "unitytestclassifier2b_37849361";
    private string m_version = "2016-05-19";
    private string m_imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";
    private string m_imageTextURL = "http://i.stack.imgur.com/ZS6nH.png";
	
	void Start ()
    {
        LogSystem.InstallDefaultReactors();

        Config.Instance.FindCredentials(m_VisualRecognition.GetServiceID());

        //  Get all classifiers
//        if(!m_VisualRecognition.GetClassifiers(OnGetClassifiers))
//            Log.Debug("ExampleVisualRecognition", "Getting classifiers failed!");

        //  Find classifier by name
//        m_VisualRecognition.FindClassifier(m_classifierName, OnFindClassifier);

        //  Find classifier by ID
//        if(!m_VisualRecognition.GetClassifier(m_classifierID, OnGetClassifier))
//            Log.Debug("ExampleVisualRecognition", "Getting classifier failed!");

        //  Delete classifier by ID
//        if(!m_VisualRecognition.DeleteClassifier(m_classifierToDelete, m_version, OnDeleteClassifier))
//            Log.Debug("ExampleVisualRecognition", "Deleting classifier failed!");

        //  Train classifier
//        string m_positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/taj_positive_examples.zip";
//        string m_negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/negative_examples.zip";
//        if(!m_VisualRecognition.TrainClassifier("unity-test-classifier5", "taj", m_positiveExamplesPath, m_negativeExamplesPath, m_version, OnTrainClassifier))
//            Log.Debug("ExampleVisualRecognition", "Train classifier failed!");

        //  Classify get
//        if(!m_VisualRecognition.Classify(m_imageURL, OnClassify))
//            Log.Debug("ExampleVisualRecognition", "Classify image failed!");

        //  Classify post URL
//        string[] m_owners = {"IBM", "me"};
//        string[] m_classifierIDs = {"default"};
//        if(!m_VisualRecognition.Classify(OnClassify, null, m_imageURL, m_owners, m_classifierIDs, 0.5f))
//            Log.Debug("ExampleVisualRecognition", "Classify image failed!");

        //  Classify post image
//        string m_imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/fruitbowl.jpg";
//        string[] m_owners = {"IBM", "me"};
//        string[] m_classifierIDs = {"default"};
//        if(!m_VisualRecognition.Classify(OnClassify, m_imagesPath, null, m_owners, m_classifierIDs, 0.5f))
//            Log.Debug("ExampleVisualRecognition", "Classify image failed!");

        //  Detect faces get
//        if(!m_VisualRecognition.DetectFaces(m_imageURL, OnDetectFaces))
//            Log.Debug("ExampleVisualRecogntiion", "Detect faces failed!");

        //  Recognize text get
        if(!m_VisualRecognition.RecognizeText(m_imageTextURL, OnRecognizeText))
            Log.Debug("ExampleVisualRecognition", "Recognize text failed!");
	}

    private void OnGetClassifiers (GetClassifiersTopLevelBrief classifiers)
    {
        if(classifiers != null && classifiers.classifiers.Length > 0)
        {
            foreach(GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
            {
                Log.Debug("ExampleVisualRecognition", "Classifier: " + classifier.name + ", " + classifier.classifier_id);
            }
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Failed to get classifiers!");
        }
    }

    private void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier)
    {
        if(classifier != null)
        {
            Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierName + " found! ClassifierID: " + classifier.classifier_id);
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Failed to find classifier by name!");
        }
    }

    private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier)
    {
        if(classifier != null)
        {
            Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierID + " found! Classifier name: " + classifier.name);
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Failed to find classifier by ID!");
        }
    }

    private void OnDeleteClassifier(bool success)
    {
        if(success)
        {
            Log.Debug("ExampleVisualRecognition", "Deleted classifier " + m_classifierToDelete);
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Failed to delete classifier by ID!");
        }
    }
    
    private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier)
    {
        if(classifier != null)
        {
            Log.Debug("ExampleVisualRecognition", "Classifier is training! " + classifier);
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Failed to train classifier!");
        }
    }

    private void OnClassify(ClassifyTopLevelMultiple classify)
    {
        if(classify != null)
        {
            Log.Debug("ExampleVisualRecognition", "images processed: " + classify.images_processed);
            foreach(ClassifyTopLevelSingle image in classify.images)
            {
                Log.Debug("ExampleVisualRecognition", "\tsource_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
                foreach(ClassifyPerClassifier classifier in image.classifiers)
                {
                    Log.Debug("ExampleVisualRecognition", "\t\tclassifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
                    foreach(ClassResult classResult in classifier.classes)
                        Log.Debug("ExampleVisualRecognition", "\t\t\tclass: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
                }
            }
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Classification failed!");
        }
    }

    private void OnDetectFaces(FacesTopLevelMultiple multipleImages)
    {
        if(multipleImages != null)
        {
            Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
            foreach(FacesTopLevelSingle faces in multipleImages.images)
            {
                Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
                foreach(OneFaceResult face in faces.faces)
                {
                    Log.Debug("ExampleVisulaRecognition", "\t\tFace location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
                    Log.Debug("ExampleVisulaRecognition", "\t\tGender: {0}, Score: {1}", face.gender.gender, face.gender.score);
                    Log.Debug("ExampleVisulaRecognition", "\t\tAge Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
                    Log.Debug("ExampleVisulaRecognition", "\t\tName: {0}, Score: {1}, Type Heiarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
                }
            }
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
        }
    }

    private void OnRecognizeText(TextRecogTopLevelMultiple multipleImages)
    {
        if(multipleImages != null)
        {
            Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
            foreach(TextRecogTopLevelSingle texts in multipleImages.images)
            {
                Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
                Log.Debug("ExampleVisualRecognition", "\ttext: {0}", texts.text);
                foreach(TextRecogOneWord text in texts.words)
                {
                    Log.Debug("ExampleVisulaRecognition", "\t\ttext location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
                    Log.Debug("ExampleVisulaRecognition", "\t\tLine number: {0}", text.line_number);
                    Log.Debug("ExampleVisulaRecognition", "\t\tword: {0}, Score: {1}", text.word, text.score);
                }
            }
        }
        else
        {
            Log.Debug("ExampleVisualRecognition", "RecognizeText failed!");
        }
    }
}
