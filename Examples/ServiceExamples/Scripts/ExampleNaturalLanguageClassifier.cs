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
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;

public class ExampleNaturalLanguageClassifier : MonoBehaviour
{
  private NaturalLanguageClassifier m_NaturalLanguageClassifier = new NaturalLanguageClassifier();
  private string m_ClassifierId = "3a84d1x62-nlc-768";
  private string m_InputString = "Is it hot outside?";

  void Start()
  {
    Debug.Log("Input String: " + m_InputString);
    m_NaturalLanguageClassifier.Classify(m_ClassifierId, m_InputString, OnClassify);
  }

  private void OnClassify(ClassifyResult result)
  {
    if (result != null)
    {
      Debug.Log("Classify Result: " + result.top_class);
    }
  }
}
