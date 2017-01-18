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
using IBM.Watson.DeveloperCloud.Services.DocumentConversion.v1;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
  public class TestDocumentConversion : UnitTest
  {
    DocumentConversion m_DocumentConversion = new DocumentConversion();
    bool m_DocumentConversionAnswerUnitsTested = false;
    bool m_DocumentConversionTextTested = false;
    bool m_DocumentConversionHTMLTested = false;

    public override IEnumerator RunTest()
    {
      string examplePath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/watson_beats_jeopardy.html";

      //  test get classifiers
      Log.Debug("TestDocumentConversion", "Testing conversion by answerUnit!");
      m_DocumentConversion.ConvertDocument(OnConvertDocumentAnswerUnits, examplePath, ConversionTarget.ANSWER_UNITS);
      while (!m_DocumentConversionAnswerUnitsTested)
        yield return null;


      Log.Debug("TestDocumentConversion", "Testing conversion by Text!");
      m_DocumentConversion.ConvertDocument(OnConvertDocumentText, examplePath, ConversionTarget.NORMALIZED_TEXT);
      while (!m_DocumentConversionTextTested)
        yield return null;

      Log.Debug("TestDocumentConversion", "Testing conversion by HTML!");
      m_DocumentConversion.ConvertDocument(OnConvertDocumentHTML, examplePath, ConversionTarget.NORMALIZED_HTML);
      while (!m_DocumentConversionHTMLTested)
        yield return null;

      yield break;
    }

    private void OnConvertDocumentAnswerUnits(ConvertedDocument documentConversionResponse, string data)
    {
      Test(documentConversionResponse != null);

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
      }

      m_DocumentConversionAnswerUnitsTested = true;
    }

    private void OnConvertDocumentHTML(ConvertedDocument documentConversionResponse, string data)
    {
      Test(!string.IsNullOrEmpty(documentConversionResponse.htmlContent));

      if (!string.IsNullOrEmpty(documentConversionResponse.htmlContent))
        Log.Debug("ExampleDocumentConversion", "TextContent: {0}", documentConversionResponse.htmlContent);

      m_DocumentConversionHTMLTested = true;
    }

    private void OnConvertDocumentText(ConvertedDocument documentConversionResponse, string data)
    {
      Test(!string.IsNullOrEmpty(documentConversionResponse.textContent));

      if (!string.IsNullOrEmpty(documentConversionResponse.textContent))
        Log.Debug("ExampleDocumentConversion", "HTMLContent: {0}", documentConversionResponse.textContent);

      m_DocumentConversionTextTested = true;
    }
  }
}
