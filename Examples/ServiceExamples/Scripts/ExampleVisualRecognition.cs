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
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;

#pragma warning disable 0414

public class ExampleVisualRecognition : MonoBehaviour
{
  private VisualRecognition m_VisualRecognition = new VisualRecognition();
  private string m_classifierName = "unity-test-classifier-example";
  private string m_classifierID = "unitytestclassifierexample_487365485";
  private string m_classifierToDelete = "unitytestclassifierexample_263072401";
  private string m_imageURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";
  private string m_imageTextURL = "http://i.stack.imgur.com/ZS6nH.png";

  void Start()
  {
    LogSystem.InstallDefaultReactors();

    ////          Get all classifiers
    //Log.Debug("ExampleVisualRecognition", "Attempting to get all classifiers");
    //if (!m_VisualRecognition.GetClassifiers(OnGetClassifiers))
    //    Log.Debug("ExampleVisualRecognition", "Getting classifiers failed!");

    ////          Find classifier by name
    //Log.Debug("ExampleVisualRecognition", "Attempting to find classifier by name");
    //m_VisualRecognition.FindClassifier(OnFindClassifier, m_classifierName);

    ////          Find classifier by ID
    //Log.Debug("ExampleVisualRecognition", "Attempting to find classifier by ID");
    //if (!m_VisualRecognition.GetClassifier(OnGetClassifier, m_classifierID))
    //    Log.Debug("ExampleVisualRecognition", "Getting classifier failed!");

    ////          Delete classifier by ID
    //Log.Debug("ExampleVisualRecognition", "Attempting to delete classifier");
    //if (!m_VisualRecognition.DeleteClassifier(OnDeleteClassifier, m_classifierToDelete))
    //    Log.Debug("ExampleVisualRecognition", "Deleting classifier failed!");

    ////          Train classifier
    //Log.Debug("ExampleVisualRecognition", "Attempting to train classifier");
    //string positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
    //string negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
    //Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
    //positiveExamples.Add("giraffe", positiveExamplesPath);
    //if (!m_VisualRecognition.TrainClassifier(OnTrainClassifier, "unity-test-classifier-example", positiveExamples, negativeExamplesPath))
    //    Log.Debug("ExampleVisualRecognition", "Train classifier failed!");

    ////          Classify get
    //Log.Debug("ExampleVisualRecognition", "Attempting to get classify via URL");
    //if (!m_VisualRecognition.Classify(OnClassify, m_imageURL))
    //    Log.Debug("ExampleVisualRecognition", "Classify image failed!");

    ////          Classify post image
    Log.Debug("ExampleVisualRecognition", "Attempting to classify via image on file system");
    string imagesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
    string[] owners = { "IBM", "me" };
    string[] classifierIDs = { "default", m_classifierID };
    if (!m_VisualRecognition.Classify(imagesPath, OnClassify, owners, classifierIDs, 0.5f))
      Log.Debug("ExampleVisualRecognition", "Classify image failed!");

    ////          Detect faces get
    //Log.Debug("ExampleVisualRecognition", "Attempting to detect faces via URL");
    //if (!m_VisualRecognition.DetectFaces(OnDetectFaces, m_imageURL))
    //    Log.Debug("ExampleVisualRecogntiion", "Detect faces failed!");

    ////          Detect faces post image
    //Log.Debug("ExampleVisualRecognition", "Attempting to detect faces via image");
    //string faceExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
    //if (!m_VisualRecognition.DetectFaces(faceExamplePath, OnDetectFaces))
    //    Log.Debug("ExampleVisualRecognition", "Detect faces failed!");

    ////          Recognize text get
    //Log.Debug("ExampleVisualRecognition", "Attempting to recognizeText via URL");
    //if (!m_VisualRecognition.RecognizeText(OnRecognizeText, m_imageTextURL))
    //    Log.Debug("ExampleVisualRecognition", "Recognize text failed!");

    ////          Recognize text post image
    //Log.Debug("ExampleVisualRecognition", "Attempting to recognizeText via image");
    //string textExamplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/from_platos_apology.png";
    //if (!m_VisualRecognition.RecognizeText(textExamplePath, OnRecognizeText))
    //    Log.Debug("ExampleVisualRecognition", "Recognize text failed!");
  }

  private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, string data)
  {
    if (classifiers != null && classifiers.classifiers.Length > 0)
    {
      foreach (GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
      {
        Log.Debug("ExampleVisualRecognition", "Classifier: " + classifier.name + ", " + classifier.classifier_id);
      }
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Failed to get classifiers!");
    }
  }

  private void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
  {
    if (classifier != null)
    {
      Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierName + " found! ClassifierID: " + classifier.classifier_id);
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Failed to find classifier by name!");
    }
  }

  private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
  {
    if (classifier != null)
    {
      Log.Debug("ExampleVisualRecognition", "Classifier " + m_classifierID + " found! Classifier name: " + classifier.name);
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Failed to find classifier by ID!");
    }
  }

  private void OnDeleteClassifier(bool success, string data)
  {
    if (success)
    {
      Log.Debug("ExampleVisualRecognition", "Deleted classifier " + m_classifierToDelete);
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Failed to delete classifier by ID!");
    }
  }

  private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, string data)
  {
    if (classifier != null)
    {
      Log.Debug("ExampleVisualRecognition", "Classifier is training! " + classifier);
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Failed to train classifier!");
    }
  }

  private void OnClassify(ClassifyTopLevelMultiple classify, string data)
  {
    if (classify != null)
    {
      Log.Debug("ExampleVisualRecognition", "images processed: " + classify.images_processed);
      foreach (ClassifyTopLevelSingle image in classify.images)
      {
        Log.Debug("ExampleVisualRecognition", "\tsource_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
        if (image.classifiers != null && image.classifiers.Length > 0)
        {
          foreach (ClassifyPerClassifier classifier in image.classifiers)
          {
            Log.Debug("ExampleVisualRecognition", "\t\tclassifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
            foreach (ClassResult classResult in classifier.classes)
              Log.Debug("ExampleVisualRecognition", "\t\t\tclass: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
          }
        }
      }
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Classification failed!");
    }
  }

  private void OnDetectFaces(FacesTopLevelMultiple multipleImages, string data)
  {
    if (multipleImages != null)
    {
      Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
      foreach (FacesTopLevelSingle faces in multipleImages.images)
      {
        Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
        foreach (OneFaceResult face in faces.faces)
        {
          if (face.face_location != null)
            Log.Debug("ExampleVisulaRecognition", "\t\tFace location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
          if (face.gender != null)
            Log.Debug("ExampleVisulaRecognition", "\t\tGender: {0}, Score: {1}", face.gender.gender, face.gender.score);
          if (face.age != null)
            Log.Debug("ExampleVisulaRecognition", "\t\tAge Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
          if (face.identity != null)
            Log.Debug("ExampleVisulaRecognition", "\t\tName: {0}, Score: {1}, Type Heiarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
        }
      }
    }
    else
    {
      Log.Debug("ExampleVisualRecognition", "Detect faces failed!");
    }
  }

  private void OnRecognizeText(TextRecogTopLevelMultiple multipleImages, string data)
  {
    if (multipleImages != null)
    {
      Log.Debug("ExampleVisualRecognition", "images processed: {0}", multipleImages.images_processed);
      foreach (TextRecogTopLevelSingle texts in multipleImages.images)
      {
        Log.Debug("ExampleVisualRecognition", "\tsource_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
        Log.Debug("ExampleVisualRecognition", "\ttext: {0}", texts.text);
        foreach (TextRecogOneWord text in texts.words)
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
