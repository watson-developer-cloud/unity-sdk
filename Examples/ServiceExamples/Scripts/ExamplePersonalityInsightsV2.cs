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
using IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v2;
using IBM.Watson.DeveloperCloud.Logging;

public class ExamplePersonalityInsightsV2 : MonoBehaviour
{
  PersonalityInsights m_personalityInsights = new PersonalityInsights();

  void Start()
  {
    LogSystem.InstallDefaultReactors();
    string dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

    if (!m_personalityInsights.GetProfile(OnGetProfile, dataPath, ContentType.TEXT_PLAIN, Language.ENGLISH))
      Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
  }

  private void OnGetProfile(Profile profile, string data)
  {
    Log.Debug("ExamplePersonalityInsights", "data: {0}", data);
    if (profile != null)
    {
      if (!string.IsNullOrEmpty(profile.id))
        Log.Debug("ExamplePersonalityInsights", "id: {0}", profile.id);
      if (!string.IsNullOrEmpty(profile.source))
        Log.Debug("ExamplePersonalityInsights", "source: {0}", profile.source);
      if (!string.IsNullOrEmpty(profile.processed_lang))
        Log.Debug("ExamplePersonalityInsights", "proccessed_lang: {0}", profile.processed_lang);
      if (!string.IsNullOrEmpty(profile.word_count))
        Log.Debug("ExamplePersonalityInsights", "word_count: {0}", profile.word_count);
      if (!string.IsNullOrEmpty(profile.word_count_message))
        Log.Debug("ExamplePersonalityInsights", "word_count_message: {0}", profile.word_count_message);

      if (profile.tree != null)
      {
        LogTraitTree(profile.tree);
      }
    }
    else
    {
      Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
    }
  }

  private void LogTraitTree(TraitTreeNode traitTreeNode)
  {
    if (!string.IsNullOrEmpty(traitTreeNode.id))
      Log.Debug("ExamplePersonalityInsights", "id: {0}", traitTreeNode.id);
    if (!string.IsNullOrEmpty(traitTreeNode.name))
      Log.Debug("ExamplePersonalityInsights", "name: {0}", traitTreeNode.name);
    if (!string.IsNullOrEmpty(traitTreeNode.category))
      Log.Debug("ExamplePersonalityInsights", "category: {0}", traitTreeNode.category);
    if (!string.IsNullOrEmpty(traitTreeNode.percentage))
      Log.Debug("ExamplePersonalityInsights", "percentage: {0}", traitTreeNode.percentage);
    if (!string.IsNullOrEmpty(traitTreeNode.sampling_error))
      Log.Debug("ExamplePersonalityInsights", "sampling_error: {0}", traitTreeNode.sampling_error);
    if (!string.IsNullOrEmpty(traitTreeNode.raw_score))
      Log.Debug("ExamplePersonalityInsights", "raw_score: {0}", traitTreeNode.raw_score);
    if (!string.IsNullOrEmpty(traitTreeNode.raw_sampling_error))
      Log.Debug("ExamplePersonalityInsights", "raw_sampling_error: {0}", traitTreeNode.raw_sampling_error);
    if (traitTreeNode.children != null && traitTreeNode.children.Length > 0)
      foreach (TraitTreeNode childNode in traitTreeNode.children)
        LogTraitTree(childNode);
  }
}
