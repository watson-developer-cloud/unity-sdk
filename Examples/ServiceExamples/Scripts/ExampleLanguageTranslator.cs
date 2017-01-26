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
using IBM.Watson.DeveloperCloud.Services.LanguageTranslator.v1;

public class ExampleLanguageTranslator : MonoBehaviour
{
  private LanguageTranslator m_Translate = new LanguageTranslator();
  private string m_PharseToTranslate = "How do I get to the disco?";

  void Start()
  {
    Debug.Log("English Phrase to translate: " + m_PharseToTranslate);
    m_Translate.GetTranslation(m_PharseToTranslate, "en", "es", OnGetTranslation);
  }

  private void OnGetTranslation(Translations translation)
  {
    if (translation != null && translation.translations.Length > 0)
      Debug.Log("Spanish Translation: " + translation.translations[0].translation);
  }
}
