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
using IBM.Watson.DeveloperCloud.Widgets;
using System.IO;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Logging;

/// <summary>
/// This is an example class showing how to use Visual Recognition with the WebCam.
/// </summary>
public class WebCamRecognition : MonoBehaviour
{
  #region Private Data
  [SerializeField]
  private WebCamWidget m_WebCamWidget;
  [SerializeField]
  private WebCamDisplayWidget m_WebCamDisplayWidget;

  VisualRecognition m_VisualRecognition;
  #endregion

  void Start()
  {
    m_VisualRecognition = new VisualRecognition();
  }

  #region Public Functions
  public void SaveFile()
  {
    Log.Debug("WebCamRecognition", "Attempting to save file!");
    Runnable.Run(SaveImageFile());
  }

  public void SaveFile(string filePath = default(string), string fileName = default(string))
  {
    Log.Debug("WebCamRecognition", "Attempting to save file!");
    Runnable.Run(SaveImageFile(filePath, fileName));
  }

  public void Classify()
  {
    Log.Debug("WebCamRecognition", "Attempting to classify image!");
    Runnable.Run(ClassifyImage());
  }

  public void DetectFaces()
  {
    Log.Debug("WebCamRecognition", "Attempting to detect faces!");
    Runnable.Run(DetectFacesInImage());
  }

  public void RecognizeText()
  {
    Log.Debug("WebCamRecognition", "Attempting to recognize text!");
    Runnable.Run(RecognizeTextInImage());
  }
  #endregion


  #region Private Functions
  private IEnumerator SaveImageFile(string filePath = default(string), string fileName = default(string))
  {
    yield return new WaitForEndOfFrame();

    if (filePath == default(string))
      filePath = Application.dataPath + "/../";
    if (fileName == default(string))
      fileName = "WebCamImage.png";
    Texture2D image = new Texture2D(m_WebCamWidget.WebCamTexture.width, m_WebCamWidget.WebCamTexture.height, TextureFormat.RGB24, false);
    image.SetPixels32(m_WebCamWidget.WebCamTexture.GetPixels32());
    image.Apply();

    File.WriteAllBytes(filePath + fileName, image.EncodeToPNG());

    Log.Debug("WebCamRecognition", "File writeen to {0}{1}", filePath, fileName);
  }

  private IEnumerator ClassifyImage()
  {
    yield return new WaitForEndOfFrame();

    //Color32[] colors = m_WebCamWidget.WebCamTexture.GetPixels32();
    //byte[] rawImageData = Utility.Color32ArrayToByteArray(colors);

    Texture2D image = new Texture2D(m_WebCamWidget.WebCamTexture.width, m_WebCamWidget.WebCamTexture.height, TextureFormat.RGB24, false);
    image.SetPixels32(m_WebCamWidget.WebCamTexture.GetPixels32());

    byte[] imageData = image.EncodeToPNG();

    m_VisualRecognition.Classify(OnClassify, imageData);
  }

  private IEnumerator DetectFacesInImage()
  {
    yield return new WaitForEndOfFrame();

    Texture2D image = new Texture2D(m_WebCamWidget.WebCamTexture.width, m_WebCamWidget.WebCamTexture.height, TextureFormat.RGB24, false);
    image.SetPixels32(m_WebCamWidget.WebCamTexture.GetPixels32());

    byte[] imageData = image.EncodeToPNG();

    m_VisualRecognition.DetectFaces(OnDetectFaces, imageData);
  }

  private IEnumerator RecognizeTextInImage()
  {
    yield return new WaitForEndOfFrame();

    Texture2D image = new Texture2D(m_WebCamWidget.WebCamTexture.width, m_WebCamWidget.WebCamTexture.height, TextureFormat.RGB24, false);
    image.SetPixels32(m_WebCamWidget.WebCamTexture.GetPixels32());

    byte[] imageData = image.EncodeToPNG();

    m_VisualRecognition.RecognizeText(OnRecognizeText, imageData);
  }
  #endregion

  #region Event Handlers
  private void OnClassify(ClassifyTopLevelMultiple classify, string data)
  {
    if (classify != null)
    {
      Log.Debug("WebCamRecognition", "images processed: " + classify.images_processed);
      foreach (ClassifyTopLevelSingle image in classify.images)
      {
        Log.Debug("WebCamRecognition", "\tsource_url: " + image.source_url + ", resolved_url: " + image.resolved_url);
        foreach (ClassifyPerClassifier classifier in image.classifiers)
        {
          Log.Debug("WebCamRecognition", "\t\tclassifier_id: " + classifier.classifier_id + ", name: " + classifier.name);
          foreach (ClassResult classResult in classifier.classes)
            Log.Debug("WebCamRecognition", "\t\t\tclass: " + classResult.m_class + ", score: " + classResult.score + ", type_hierarchy: " + classResult.type_hierarchy);
        }
      }
    }
    else
    {
      Log.Debug("WebCamRecognition", "Classification failed!");
    }
  }

  private void OnDetectFaces(FacesTopLevelMultiple multipleImages, string data)
  {
    if (multipleImages != null)
    {
      Log.Debug("WebCamRecognition", "images processed: {0}", multipleImages.images_processed);
      foreach (FacesTopLevelSingle faces in multipleImages.images)
      {
        Log.Debug("WebCamRecognition", "\tsource_url: {0}, resolved_url: {1}", faces.source_url, faces.resolved_url);
        foreach (OneFaceResult face in faces.faces)
        {
          if (face.face_location != null)
            Log.Debug("WebCamRecognition", "\t\tFace location: {0}, {1}, {2}, {3}", face.face_location.left, face.face_location.top, face.face_location.width, face.face_location.height);
          if (face.gender != null)
            Log.Debug("WebCamRecognition", "\t\tGender: {0}, Score: {1}", face.gender.gender, face.gender.score);
          if (face.age != null)
            Log.Debug("WebCamRecognition", "\t\tAge Min: {0}, Age Max: {1}, Score: {2}", face.age.min, face.age.max, face.age.score);

          if (face.identity != null)
            Log.Debug("WebCamRecognition", "\t\tName: {0}, Score: {1}, Type Heiarchy: {2}", face.identity.name, face.identity.score, face.identity.type_hierarchy);
        }
      }
    }
    else
    {
      Log.Debug("WebCamRecognition", "Detect faces failed!");
    }
  }

  private void OnRecognizeText(TextRecogTopLevelMultiple multipleImages, string data)
  {
    if (multipleImages != null)
    {
      Log.Debug("WebCamRecognition", "images processed: {0}", multipleImages.images_processed);
      foreach (TextRecogTopLevelSingle texts in multipleImages.images)
      {
        Log.Debug("WebCamRecognition", "\tsource_url: {0}, resolved_url: {1}", texts.source_url, texts.resolved_url);
        Log.Debug("WebCamRecognition", "\ttext: {0}", texts.text);
        foreach (TextRecogOneWord text in texts.words)
        {
          Log.Debug("WebCamRecognition", "\t\ttext location: {0}, {1}, {2}, {3}", text.location.left, text.location.top, text.location.width, text.location.height);
          Log.Debug("WebCamRecognition", "\t\tLine number: {0}", text.line_number);
          Log.Debug("WebCamRecognition", "\t\tword: {0}, Score: {1}", text.word, text.score);
        }
      }
    }
    else
    {
      Log.Debug("WebCamRecognition", "RecognizeText failed!");
    }
  }
  #endregion
}
