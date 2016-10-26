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
using IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1;
using System.Collections;
using IBM.Watson.DeveloperCloud.Logging;

public class ExampleDocumentConversion : MonoBehaviour
{
  private DocumentConversion m_DocumentConversion = new DocumentConversion();

  void Start()
  {
    LogSystem.InstallDefaultReactors(); LogSystem.InstallDefaultReactors();
    string examplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

    if (!m_DocumentConversion.ConvertDocument(OnConvertDocument, examplePath, ConversionTarget.NORMALIZED_TEXT))
      Log.Debug("ExampleDocumentConversion", "Document conversion failed!");
  }

  private void OnConvertDocument(ConvertedDocument documentConversionResponse, string data)
  {
    if (documentConversionResponse != null)
    {
      if (!string.IsNullOrEmpty(documentConversionResponse.media_type_detected))
        Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.media_type_detected);
      if (!string.IsNullOrEmpty(documentConversionResponse.source_document_id))
        Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.source_document_id);
      if (!string.IsNullOrEmpty(documentConversionResponse.timestamp))
        Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.timestamp);
      if (documentConversionResponse.metadata != null && documentConversionResponse.metadata.Length > 0)
      {
        Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.metadata.Length);
        foreach (Metadata metadata in documentConversionResponse.metadata)
          Log.Debug("ExampleDocumentConversion", "metadata | name: {0}, content: {1}", metadata.name, metadata.content);
      }
      if (documentConversionResponse.answer_units != null && documentConversionResponse.answer_units.Length > 0)
      {
        Log.Debug("ExampleDocumentConversion", "mediaTypeDetected: {0}", documentConversionResponse.answer_units.Length);
        foreach (AnswerUnit answerUnit in documentConversionResponse.answer_units)
        {
          Log.Debug("ExampleDocumentConversion", "answerUnit | type: {0}, title: {1}, parent_id: {2}, id: {3}, direction: {4}", answerUnit.type, answerUnit.title, answerUnit.parent_id, answerUnit.id, answerUnit.direction);
          if (answerUnit.content != null && answerUnit.content.Length > 0)
            foreach (Content content in answerUnit.content)
              Log.Debug("ExampleDocumentConversion", "content | mediaType: {0}, text: {1}", content.media_type, content.text);
        }
      }

      if (!string.IsNullOrEmpty(documentConversionResponse.htmlContent))
        Log.Debug("ExampleDocumentConversion", "HTMLContent: {0}", documentConversionResponse.htmlContent);
      if (!string.IsNullOrEmpty(documentConversionResponse.textContent))
        Log.Debug("ExampleDocumentConversion", "TextContent: {0}", documentConversionResponse.textContent);
    }
  }
}
