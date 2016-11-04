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

using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Utilities;
using System.IO;
using UnityEngine;
using System;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;

#pragma warning disable 0414

namespace IBM.Watson.DeveloperCloud.UnitTests
{
  public class TestVisualRecognition : UnitTest
  {
    VisualRecognition m_VisualRecognition = new VisualRecognition();
    bool m_TrainClasifierTested = false;
    bool m_GetClassifiersTested = false;
    bool m_FindClassifierTested = false;
    bool m_GetClassifierTested = false;
    bool m_UpdateClassifierTested = false;
    bool m_ClassifyGETTested = false;
    bool m_ClassifyPOSTTested = false;
    bool m_DetectFacesGETTested = false;
    bool m_DetectFacesPOSTTested = false;
    bool m_RecognizeTextGETTested = false;
    bool m_RecognizeTextPOSTTested = false;
    bool m_DeleteClassifierTested = false;

    bool m_ListCollectionsTested = false;
    bool m_CreateCollectionTested = false;
    bool m_DeleteCollectionTested = false;
    bool m_RetrieveCollectionDetailsTested = false;
    bool m_ListImagesTested = false;
    bool m_AddImagesToCollectionTested = false;
    bool m_DeleteImageFromCollectionTested = false;
    bool m_ListImageDetailsTested = false;
    bool m_DeleteImageMetadataTested = false;
    bool m_ListImageMetadataTested = false;
    bool m_FindSimilarTested = false;


    bool m_TrainClassifier = false;
    bool m_IsClassifierReady = false;
    bool m_HasUpdatedClassifier = false;
    bool m_IsUpdatedClassifierReady = false;
    string m_ClassifierId = null;
    string m_ClassifierName = "unity-integration-test-classifier";
    string m_ClassName_Giraffe = "giraffe";
    string m_ClassName_Turtle = "turtle";
    private string m_ImageURL = "https://c2.staticflickr.com/2/1226/1173659064_8810a06fef_b.jpg";   //  giraffe image
    private string m_ImageFaceURL = "https://upload.wikimedia.org/wikipedia/commons/e/e9/Official_portrait_of_Barack_Obama.jpg";    //  Obama image
    private string m_ImageTextURL = "http://i.stack.imgur.com/ZS6nH.png";   //  image with text

    private string m_CreatedCollectionID;
    private string m_CreatedCollectionImage;

    public override IEnumerator RunTest()
    {
      //  test get classifiers
      Log.Debug("TestVisualRecognition", "Getting all classifiers!");
      m_VisualRecognition.GetClassifiers(OnGetClassifiers);
      while (!m_GetClassifiersTested)
        yield return null;

      //  test find classifier
      Log.Debug("TestVisualRecognition", "Finding classifier {0}!", m_ClassifierName);
      m_VisualRecognition.FindClassifier(OnFindClassifier, m_ClassifierName);
      while (!m_FindClassifierTested)
        yield return null;

      if (m_TrainClassifier)
      {
        //  test train classifier
        Log.Debug("TestVisualRecognition", "Training classifier!");
        string m_positiveExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_positive_examples.zip";
        string m_negativeExamplesPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/negative_examples.zip";
        Dictionary<string, string> positiveExamples = new Dictionary<string, string>();
        positiveExamples.Add(m_ClassName_Giraffe, m_positiveExamplesPath);
        Test(m_VisualRecognition.TrainClassifier(OnTrainClassifier, m_ClassifierName, positiveExamples, m_negativeExamplesPath));
        while (!m_TrainClasifierTested)
          yield return null;
      }

      //  Wait until classifier is ready
      if (!m_IsClassifierReady)
      {
        Log.Debug("TestVisualRecognition", "Checking classifier {0} status!", m_ClassifierId);
        CheckClassifierStatus(OnCheckClassifierStatus);
        while (!m_IsClassifierReady)
          yield return null;
      }

      if (!string.IsNullOrEmpty(m_ClassifierId))
      {
        //  test get classifier
        Log.Debug("TestVisualRecognition", "Getting classifier {0}!", m_ClassifierId);
        m_VisualRecognition.GetClassifier(OnGetClassifier, m_ClassifierId);
        while (!m_GetClassifierTested)
          yield return null;

        //  Update classifier
        Log.Debug("TestVisualRecognition", "Updating classifier {0}", m_ClassifierId);
        string m_positiveUpdated = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/turtle_positive_examples.zip";
        Dictionary<string, string> positiveUpdatedExamples = new Dictionary<string, string>();
        positiveUpdatedExamples.Add(m_ClassName_Turtle, m_positiveUpdated);
        m_VisualRecognition.UpdateClassifier(OnUpdateClassifier, m_ClassifierId, m_ClassifierName, positiveUpdatedExamples);
        while (!m_UpdateClassifierTested)
          yield return null;

        //  Wait for updated classifier to be ready.
        Log.Debug("TestVisualRecognition", "Checking updated classifier {0} status!", m_ClassifierId);
        CheckClassifierStatus(OnCheckUpdatedClassifierStatus);
        while (!m_IsUpdatedClassifierReady)
          yield return null;

        string[] m_owners = { "IBM", "me" };
        string[] m_classifierIds = { "default", m_ClassifierId };

        //  test classify image get
        Log.Debug("TestVisualRecognition", "Classifying image using GET!");
        m_VisualRecognition.Classify(OnClassifyGet, m_ImageURL, m_owners, m_classifierIds);
        while (!m_ClassifyGETTested)
          yield return null;

        //  test classify image post
        Log.Debug("TestVisualRecognition", "Classifying image using POST!");
        string m_classifyImagePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
        m_VisualRecognition.Classify(m_classifyImagePath, OnClassifyPost, m_owners, m_classifierIds);
        while (!m_ClassifyPOSTTested)
          yield return null;
      }

      //  test detect faces get
      Log.Debug("TestVisualRecognition", "Detecting face image using GET!");
      m_VisualRecognition.DetectFaces(OnDetectFacesGet, m_ImageFaceURL);
      while (!m_DetectFacesGETTested)
        yield return null;

      //  test detect faces post
      Log.Debug("TestVisualRecognition", "Detecting face image using POST!");
      string m_detectFaceImagePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/obama.jpg";
      m_VisualRecognition.DetectFaces(m_detectFaceImagePath, OnDetectFacesPost);
      while (!m_DetectFacesPOSTTested)
        yield return null;

      //  test recognize text get
      Log.Debug("TestVisualRecognition", "Recognizing text image using GET!");
      m_VisualRecognition.RecognizeText(OnRecognizeTextGet, m_ImageTextURL);
      while (!m_RecognizeTextGETTested)
        yield return null;

      //  test recognize text post
      Log.Debug("TestVisualRecognition", "Recognizing text image using POST!");
      string m_recognizeTextImagePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/from_platos_apology.png";
      m_VisualRecognition.RecognizeText(m_recognizeTextImagePath, OnRecognizeTextPost);
      while (!m_RecognizeTextPOSTTested)
        yield return null;

      //  test delete classifier
      Log.Debug("TestVisualRecognition", "Deleting classifier {0}!", m_ClassifierId);
      m_VisualRecognition.DeleteClassifier(OnDeleteClassifier, m_ClassifierId);
      while (!m_DeleteClassifierTested)
        yield return null;

      //  test list collections
      Log.Debug("TestVisualRecognition", "Attempting to list collections!");
      m_VisualRecognition.GetCollections(OnGetCollections);
      while (!m_ListCollectionsTested)
        yield return null;

      //  test create collection
      Log.Debug("TestVisualRecognition", "Attempting to create collection!");
      m_VisualRecognition.CreateCollection(OnCreateCollection, "unity-integration-test-collection");
      while (!m_CreateCollectionTested)
        yield return null;

      //  test retrive collection details
      Log.Debug("TestVisualRecognition", "Attempting to retrieve collection details!");
      m_VisualRecognition.GetCollection(OnGetCollection, m_CreatedCollectionID);
      while (!m_RetrieveCollectionDetailsTested)
        yield return null;

      //  test add images to collection
      Log.Debug("TestVisualRecognition", "Attempting to add images to collection!");
      string m_collectionImagePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/visual-recognition-classifiers/giraffe_to_classify.jpg";
      Dictionary<string, string> imageMetadata = new Dictionary<string, string>();
      imageMetadata.Add("key1", "value1");
      imageMetadata.Add("key2", "value2");
      imageMetadata.Add("key3", "value3");
      m_VisualRecognition.AddCollectionImage(OnAddImageToCollection, m_CreatedCollectionID, m_collectionImagePath, imageMetadata);
      while (!m_AddImagesToCollectionTested)
        yield return null;

      //  test list images
      Log.Debug("TestVisualRecognition", "Attempting to list images!");
      m_VisualRecognition.GetCollectionImages(OnGetCollectionImages, m_CreatedCollectionID);
      while (!m_ListImagesTested)
        yield return null;

      //  test list image details
      Log.Debug("TestVisualRecognition", "Attempting to list image details!");
      m_VisualRecognition.GetImage(OnGetImage, m_CreatedCollectionID, m_CreatedCollectionImage);
      while (!m_ListImageDetailsTested)
        yield return null;

      //  test list image metadata
      Log.Debug("TestVisualRecognition", "Attempting to list image metadata!");
      m_VisualRecognition.GetMetadata(OnGetMetadata, m_CreatedCollectionID, m_CreatedCollectionImage);
      while (!m_ListImageMetadataTested)
        yield return null;

      //  test find similar
      Log.Debug("TestVisualRecognition", "Attempting to find similar!");
      m_VisualRecognition.FindSimilar(OnFindSimilar, m_CreatedCollectionID, m_collectionImagePath);
      while (!m_FindSimilarTested)
        yield return null;

      //  test delete image metadata
      Log.Debug("TestVisualRecognition", "Attempting to delete metadata!");
      m_VisualRecognition.DeleteCollectionImageMetadata(OnDeleteMetadata, m_CreatedCollectionID, m_CreatedCollectionImage);
      while (!m_DeleteImageMetadataTested)
        yield return null;

      //  test delete image from collection
      Log.Debug("TestVisualRecognition", "Attempting to delete image from collection!");
      m_VisualRecognition.DeleteCollectionImage(OnDeleteCollectionImage, m_CreatedCollectionID, m_CreatedCollectionImage);
      while (!m_DeleteImageFromCollectionTested)
        yield return null;

      //  test delete collection
      Log.Debug("TestVisualRecognition", "Attempting to delete collection!");
      m_VisualRecognition.DeleteCollection(OnDeleteCollection, m_CreatedCollectionID);
      while (!m_DeleteCollectionTested)
        yield return null;
      yield break;
    }

    private void OnFindClassifier(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      if (classifier != null)
      {
        Log.Status("TestVisualRecognition", "Find Result, Classifier ID: {0}, Status: {1}", classifier.classifier_id, classifier.status);
        if (classifier.status == "ready")
        {
          m_TrainClassifier = false;
          m_IsClassifierReady = true;
          m_ClassifierId = classifier.classifier_id;
        }
        else
        {
          m_TrainClassifier = false;
        }
      }
      else
      {
        m_TrainClassifier = true;
      }

      m_FindClassifierTested = true;
    }

    private void OnTrainClassifier(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      Test(classifier != null);
      if (classifier != null)
      {
        Log.Status("TestVisualRecognition", "Classifier ID: {0}, Classifier name: {1}, Status: {2}", classifier.classifier_id, classifier.name, classifier.status);
        //  store classifier id
        m_ClassifierId = classifier.classifier_id;
      }

      m_TrainClasifierTested = true;
    }

    private void OnGetClassifiers(GetClassifiersTopLevelBrief classifiers, string customData)
    {
      Test(classifiers != null);
      if (classifiers != null && classifiers.classifiers.Length > 0)
      {
        Log.Debug("TestVisualRecognition", "{0} classifiers found!", classifiers.classifiers.Length);
        //                foreach(GetClassifiersPerClassifierBrief classifier in classifiers.classifiers)
        //                {
        //                    Log.Debug("TestVisualRecognition", "Classifier: " + classifier.name + ", " + classifier.classifier_id);
        //                }
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Failed to get classifiers!");
      }

      m_GetClassifiersTested = true;
    }

    private void OnGetClassifier(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      Test(classifier != null);
      if (classifier != null)
      {
        Log.Debug("TestVisualRecognition", "Classifier {0} found! Classifier name: {1}", classifier.classifier_id, classifier.name);
        foreach (Class classifierClass in classifier.classes)
          if (classifierClass.m_Class == m_ClassName_Turtle)
            m_HasUpdatedClassifier = true;
      }

      m_GetClassifierTested = true;
    }

    private void OnUpdateClassifier(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      if (classifier != null)
      {
        Log.Status("TestVisualRecognition", "Classifier ID: {0}, Classifier name: {1}, Status: {2}", classifier.classifier_id, classifier.name, classifier.status);
        foreach (Class classifierClass in classifier.classes)
          if (classifierClass.m_Class == m_ClassName_Turtle)
            m_HasUpdatedClassifier = true;
        //  store classifier id
        //m_ClassifierId = classifier.classifier_id;
      }

      m_UpdateClassifierTested = true;
    }

    private void OnClassifyGet(ClassifyTopLevelMultiple classify, string customData)
    {
      Test(classify != null);
      if (classify != null)
      {
        Log.Debug("TestVisualRecognition", "ClassifyImage GET images processed: " + classify.images_processed);
        foreach (ClassifyTopLevelSingle image in classify.images)
        {
          Log.Debug("TestVisualRecognition", "\tClassifyImage GET source_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
          foreach (ClassifyPerClassifier classifier in image.classifiers)
          {
            Log.Debug("TestVisualRecognition", "\t\tClassifyImage GET classifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
            foreach (ClassResult classResult in classifier.classes)
              Log.Debug("TestVisualRecognition", "\t\t\tClassifyImage GET class: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
          }
        }

        m_ClassifyGETTested = true;
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Classification GET failed!");
      }
    }

    private void OnClassifyPost(ClassifyTopLevelMultiple classify, string customData)
    {
      Test(classify != null);
      if (classify != null)
      {
        Log.Debug("TestVisualRecognition", "ClassifyImage POST images processed: " + classify.images_processed);
        foreach (ClassifyTopLevelSingle image in classify.images)
        {
          Log.Debug("TestVisualRecognition", "\tClassifyImage POST source_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
          foreach (ClassifyPerClassifier classifier in image.classifiers)
          {
            Log.Debug("TestVisualRecognition", "\t\tClassifyImage POST classifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
            foreach (ClassResult classResult in classifier.classes)
              Log.Debug("TestVisualRecognition", "\t\t\tClassifyImage POST class: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
          }
        }

        m_ClassifyPOSTTested = true;
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Classification POST failed!");
      }
    }

    private void OnDetectFacesGet(FacesTopLevelMultiple multipleImages, string customData)
    {
      Test(multipleImages != null);
      if (multipleImages != null)
      {
        Log.Debug("TestVisualRecognition", "DetectFaces GET  images processed: {0}", multipleImages.images_processed);
        foreach (FacesTopLevelSingle faces in multipleImages.images)
        {
          Log.Debug("TestVisualRecognition", "\tDetectFaces GET  source_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
          foreach (OneFaceResult face in faces.faces)
          {
            if(face.face_location != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces GET Face location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
            if(face.gender != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces GET Gender: {0}, Score: {1}", face.gender.gender, face.gender.score);
            if(face.age != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces GET Age Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
            if (face.identity != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces GET Name: {0}, Score: {1}, Type Heiarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
          }
        }

        m_DetectFacesGETTested = true;
      }
      else
      {
        Log.Debug("ExampleVisualRecognition", "DetectFaces GET Detect faces failed!");
      }
    }

    private void OnDetectFacesPost(FacesTopLevelMultiple multipleImages, string customData)
    {
      Test(multipleImages != null);
      if (multipleImages != null)
      {
        Log.Debug("TestVisualRecognition", "DetectFaces POST  images processed: {0}", multipleImages.images_processed);
        foreach (FacesTopLevelSingle faces in multipleImages.images)
        {
          Log.Debug("TestVisualRecognition", "\tDetectFaces POST  source_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
          foreach (OneFaceResult face in faces.faces)
          {
            if (face.face_location != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces POST Face location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
            if (face.gender != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces POST Gender: {0}, Score: {1}", face.gender.gender, face.gender.score);
            if (face.age != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces POST Age Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);
            if(face.identity != null)
              Log.Debug("TestVisualRecognition", "\t\tDetectFaces POST Name: {0}, Score: {1}, Type Heiarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
          }
        }

        m_DetectFacesPOSTTested = true;
      }
      else
      {
        Log.Debug("ExampleVisualRecognition", "DetectFaces POST Detect faces failed!");
      }
    }

    private void OnRecognizeTextGet(TextRecogTopLevelMultiple multipleImages, string customData)
    {
      Test(multipleImages != null);
      if (multipleImages != null)
      {
        Log.Debug("TestVisualRecognition", "RecognizeText GET images processed: {0}", multipleImages.images_processed);
        foreach (TextRecogTopLevelSingle texts in multipleImages.images)
        {
          Log.Debug("TestVisualRecognition", "\tRecognizeText GET source_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
          Log.Debug("TestVisualRecognition", "\tRecognizeText GET text: {0}", texts.text);
          //                    foreach(TextRecogOneWord text in texts.words)
          //                    {
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText GET text location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText GET Line number: {0}", text.line_number);
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText GET word: {0}, Score: {1}", text.word, text.score);
          //                    }
        }

        m_RecognizeTextGETTested = true;
      }
      else
      {
        Log.Debug("ExampleVisualRecognition", "RecognizeText GET failed!");
      }
    }

    private void OnRecognizeTextPost(TextRecogTopLevelMultiple multipleImages, string customData)
    {
      Test(multipleImages != null);
      if (multipleImages != null)
      {
        Log.Debug("TestVisualRecognition", "RecognizeText POST images processed: {0}", multipleImages.images_processed);
        foreach (TextRecogTopLevelSingle texts in multipleImages.images)
        {
          Log.Debug("TestVisualRecognition", "\tRecognizeText POST source_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
          Log.Debug("TestVisualRecognition", "\tRecognizeText POST text: {0}", texts.text);
          //                    foreach(TextRecogOneWord text in texts.words)
          //                    {
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText POST text location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText POST Line number: {0}", text.line_number);
          //                        Log.Debug("TestVisualRecognition", "\t\tRecognizeText POST word: {0}, Score: {1}", text.word, text.score);
          //                    }
        }

        m_RecognizeTextPOSTTested = true;
      }
      else
      {
        Log.Debug("ExampleVisualRecognition", "RecognizeText POST failed!");
      }
    }

    private void OnDeleteClassifier(bool success, string customData)
    {
      if (success)
      {
        m_VisualRecognition.FindClassifier(OnDeleteClassifierFinal, m_ClassifierName);
      }

      m_DeleteClassifierTested = true;
      Test(success);
    }

    private void CheckClassifierStatus(VisualRecognition.OnGetClassifier callback, string customData = default(string))
    {
      if (!m_VisualRecognition.GetClassifier(callback, m_ClassifierId))
        Log.Debug("TestVisualRecognition", "Get classifier failed!");
    }

    private void OnCheckClassifierStatus(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      Log.Debug("TestVisualRecognition", "classifier {0} is {1}!", classifier.classifier_id, classifier.status);
      if (classifier.status == "unavailable" || classifier.status == "failed")
      {
        Log.Debug("TestVisualRecognition", "Deleting classifier!");
        //  classifier failed - delete!
        if (!m_VisualRecognition.DeleteClassifier(OnCheckClassifierStatusDelete, classifier.classifier_id))
          Log.Debug("TestVisualRecognition", "Failed to delete classifier {0}!", m_ClassifierId);

      }
      else if (classifier.status == "training")
      {
        CheckClassifierStatus(OnCheckClassifierStatus);
      }
      else if (classifier.status == "ready")
      {
        m_IsClassifierReady = true;
        m_ClassifierId = classifier.classifier_id;
      }
    }

    private void OnCheckUpdatedClassifierStatus(GetClassifiersPerClassifierVerbose classifier, string customData = default(string))
    {
      Log.Debug("TestVisualRecognition", "classifier {0} is {1}!", classifier.classifier_id, classifier.status);

      if (classifier.status == "retraining")
      {
        CheckClassifierStatus(OnCheckUpdatedClassifierStatus);
      }
      else if (classifier.status == "ready")
      {
        m_IsUpdatedClassifierReady = true;
      }
    }

    private void OnCheckClassifierStatusDelete(bool success, string customData)
    {
      if (success)
      {
        //  train classifier again!
        m_TrainClasifierTested = false;
      }
    }

    private void OnDeleteClassifierFinal(GetClassifiersPerClassifierVerbose classifier, string customData)
    {
      if (classifier == null)
      {
        Log.Debug("TestVisualRecognition", "Classifier not found! Delete sucessful!");
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Classifier {0} found! Delete failed!", classifier.name);
      }
      Test(classifier == null);
    }

    private void OnGetCollections(GetCollections collections, string customData)
    {
      if (collections != null)
      {
        Log.Debug("TestVisualRecognition", "Get Collections succeeded!");
        foreach (CreateCollection collection in collections.collections)
        {
          Log.Debug("TestVisualRecognition", "collectionID: {0} | collection name: {1} | number of images: {2}", collection.collection_id, collection.name, collection.images);
        }
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Get Collections failed!");
      }

      m_ListCollectionsTested = true;
      Test(collections != null);
    }

    private void OnCreateCollection(CreateCollection collection, string customData)
    {
      if (collection != null)
      {
        Log.Debug("TestVisualRecognition", "Create Collection succeeded!");
        Log.Debug("TestVisualRecognition", "collectionID: {0} | collection name: {1} | collection images: {2}", collection.collection_id, collection.name, collection.images);

        m_CreatedCollectionID = collection.collection_id;
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Create Collection failed!");
      }

      m_CreateCollectionTested = true;
      Test(collection != null);
    }

    private void OnDeleteCollection(bool success, string customData)
    {
      if (success)
        Log.Debug("TestVisualRecognition", "Delete Collection succeeded!");
      else
        Log.Debug("TestVisualRecognition", "Delete Collection failed!");

      m_DeleteCollectionTested = true;
      Test(success);
    }

    private void OnGetCollection(CreateCollection collection, string customData)
    {
      if (collection != null)
      {
        Log.Debug("TestVisualRecognition", "Get Collection succeded!");
        Log.Debug("TestVisualRecognition", "collectionID: {0} | collection name: {1} | collection images: {2}", collection.collection_id, collection.name, collection.images);
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Get Collection failed!");

      }

      m_RetrieveCollectionDetailsTested = true;
      Test(collection != null);
    }

    private void OnGetCollectionImages(GetCollectionImages collections, string customData)
    {
      if (collections != null)
      {
        Log.Debug("TestVisualRecognition", "Get Collections succeded!");
        foreach (GetCollectionsBrief collection in collections.images)
          Log.Debug("TestVisualRecognition", "imageID: {0} | image file: {1} | image metadataOnGetCollections: {2}", collection.image_id, collection.image_file, collection.metadata.ToString());
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Get Collections failed!");
      }

      Test(collections != null);
      m_ListImagesTested = true;
    }

    private void OnAddImageToCollection(CollectionsConfig images, string customData)
    {
      if (images != null)
      {
        Log.Debug("TestVisualRecognition", "Add image to collection succeeded!");
        m_CreatedCollectionImage = images.images[0].image_id;
        Log.Debug("TestVisualRecognition", "images processed: {0}", images.images_processed);
        foreach (CollectionImagesConfig image in images.images)
          Log.Debug("TestVisualRecognition", "imageID: {0} | image_file: {1} | image metadata: {1}", image.image_id, image.image_file, image.metadata.ToString());
      }
      else
      {
        Log.Debug("TestVisualRecognition", "Add image to collection failed!");
      }

      Test(images != null);
      m_AddImagesToCollectionTested = true;
    }

    private void OnDeleteCollectionImage(bool success, string customData)
    {
      if (success)
        Log.Debug("TestVisualRecognition", "Delete collection image succeeded!");
      else
        Log.Debug("TestVisualRecognition", "Delete collection image failed!");

      Test(success);
      m_DeleteImageFromCollectionTested = true;
    }

    private void OnGetImage(GetCollectionsBrief image, string customData)
    {
      if (image != null)
      {
        Log.Debug("TestVisualRecognition", "GetImage succeeded!");
        Log.Debug("TestVisualRecognition", "imageID: {0} | created: {1} | image_file: {2} | metadata: {3}", image.image_id, image.created, image.image_file, image.metadata);
      }
      else
      {
        Log.Debug("TestVisualRecognition", "GetImage failed!");
      }

      Test(image != null);
      m_ListImageDetailsTested = true;

    }

    private void OnDeleteMetadata(bool success, string customData)
    {
      if (success)
        Log.Debug("TestVisualRecognition", "Delete image metadata succeeded!");
      else
        Log.Debug("TestVisualRecognition", "Delete image metadata failed!");

      Test(success);
      m_DeleteImageMetadataTested = true;
    }

    private void OnGetMetadata(object responseObject, string customData)
    {
      if (responseObject != null)
        Log.Debug("TestVisualRecognition", "ResponseObject: {0}", responseObject);

      Test(responseObject != null);
      m_ListImageMetadataTested = true;
    }

    private void OnFindSimilar(SimilarImagesConfig images, string customData)
    {
      if (images != null)
      {
        Log.Debug("TestVisualRecognition", "GetSimilar succeeded!");
        Log.Debug("TestVisualRecognition", "images processed: {0}", images.images_processed);
        foreach (SimilarImageConfig image in images.similar_images)
          Log.Debug("TestVisualRecognition", "image ID: {0} | image file: {1} | score: {2} | metadata: {3}", image.image_id, image.image_file, image.score, image.metadata.ToString());
      }
      else
      {
        Log.Debug("TestVisualRecognition", "GetSimilar failed!");
      }

      Test(images != null);
      m_FindSimilarTested = true;
    }
  }
}
