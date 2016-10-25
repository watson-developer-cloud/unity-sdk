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
using IBM.Watson.DeveloperCloud.Services.PersonalityInsights.v3;
using IBM.Watson.DeveloperCloud.Logging;

public class ExamplePersonalityInsightsV3 : MonoBehaviour
{
  PersonalityInsights m_personalityInsights = new PersonalityInsights();
  private string testString = "<text-here>";
  private string dataPath;

  void Start()
  {
    LogSystem.InstallDefaultReactors();

    dataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/personalityInsights.json";

    if (!m_personalityInsights.GetProfile(OnGetProfileJson, dataPath, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
      Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");

    if (!m_personalityInsights.GetProfile(OnGetProfileText, testString, ContentType.TEXT_HTML, ContentLanguage.ENGLISH, ContentType.APPLICATION_JSON, AcceptLanguage.ENGLISH, true, true, true))
      Log.Debug("ExamplePersonalityInsights", "Failed to get profile!");
  }
  private void OnGetProfileText(Profile profile, string data)
  {
    if (profile != null)
    {
      if (!string.IsNullOrEmpty(profile.processed_language))
        Log.Debug("TestPersonalityInsightsV3", "processed_language: {0}", profile.processed_language);

      Log.Debug("TestPersonalityInsightsV3", "word_count: {0}", profile.word_count);

      if (!string.IsNullOrEmpty(profile.word_count_message))
        Log.Debug("TestPersonalityInsightsV3", "word_count_message: {0}", profile.word_count_message);

      if (profile.personality != null && profile.personality.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Personality trait tree");
        foreach (TraitTreeNode node in profile.personality)
          LogTraitTree(node);
      }

      if (profile.values != null && profile.values.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Values trait tree");
        foreach (TraitTreeNode node in profile.values)
          LogTraitTree(node);
      }

      if (profile.needs != null && profile.personality.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Needs trait tree");
        foreach (TraitTreeNode node in profile.needs)
          LogTraitTree(node);
      }

      if (profile.behavior != null && profile.behavior.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Behavior tree");
        foreach (BehaviorNode behavior in profile.behavior)
        {
          Log.Debug("TestPersonalityInsightsV3", "trait_id: {0}", behavior.trait_id);
          Log.Debug("TestPersonalityInsightsV3", "name: {0}", behavior.name);
          Log.Debug("TestPersonalityInsightsV3", "category: {0}", behavior.category);
          Log.Debug("TestPersonalityInsightsV3", "percentage: {0}", behavior.percentage.ToString());
          Log.Debug("TestPersonalityInsightsV3", "----------------");
        }
      }

      if (profile.consumption_preferences != null && profile.consumption_preferences.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "ConsumptionPreferencesCategories");
        foreach (ConsumptionPreferencesCategoryNode categoryNode in profile.consumption_preferences)
          LogConsumptionPreferencesCategory(categoryNode);
      }
    }
  }

  private void OnGetProfileJson(Profile profile, string data)
  {
    if (profile != null)
    {
      if (!string.IsNullOrEmpty(profile.processed_language))
        Log.Debug("TestPersonalityInsightsV3", "processed_language: {0}", profile.processed_language);

      Log.Debug("TestPersonalityInsightsV3", "word_count: {0}", profile.word_count);

      if (!string.IsNullOrEmpty(profile.word_count_message))
        Log.Debug("TestPersonalityInsightsV3", "word_count_message: {0}", profile.word_count_message);

      if (profile.personality != null && profile.personality.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Personality trait tree");
        foreach (TraitTreeNode node in profile.personality)
          LogTraitTree(node);
      }

      if (profile.values != null && profile.values.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Values trait tree");
        foreach (TraitTreeNode node in profile.values)
          LogTraitTree(node);
      }

      if (profile.needs != null && profile.personality.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Needs trait tree");
        foreach (TraitTreeNode node in profile.needs)
          LogTraitTree(node);
      }

      if (profile.behavior != null && profile.behavior.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "Behavior tree");
        foreach (BehaviorNode behavior in profile.behavior)
        {
          Log.Debug("TestPersonalityInsightsV3", "trait_id: {0}", behavior.trait_id);
          Log.Debug("TestPersonalityInsightsV3", "name: {0}", behavior.name);
          Log.Debug("TestPersonalityInsightsV3", "category: {0}", behavior.category);
          Log.Debug("TestPersonalityInsightsV3", "percentage: {0}", behavior.percentage.ToString());
          Log.Debug("TestPersonalityInsightsV3", "----------------");
        }
      }

      if (profile.consumption_preferences != null && profile.consumption_preferences.Length > 0)
      {
        Log.Debug("TestPersonalityInsightsV3", "ConsumptionPreferencesCategories");
        foreach (ConsumptionPreferencesCategoryNode categoryNode in profile.consumption_preferences)
          LogConsumptionPreferencesCategory(categoryNode);
      }
    }
  }

  private void LogTraitTree(TraitTreeNode traitTreeNode)
  {
    Log.Debug("TestPersonalityInsightsV3", "trait_id: {0} | name: {1} | category: {2} | percentile: {3} | raw_score: {4}",
        string.IsNullOrEmpty(traitTreeNode.trait_id) ? "null" : traitTreeNode.trait_id,
        string.IsNullOrEmpty(traitTreeNode.name) ? "null" : traitTreeNode.name,
        string.IsNullOrEmpty(traitTreeNode.category) ? "null" : traitTreeNode.category,
        string.IsNullOrEmpty(traitTreeNode.percentile.ToString()) ? "null" : traitTreeNode.percentile.ToString(),
        string.IsNullOrEmpty(traitTreeNode.raw_score.ToString()) ? "null" : traitTreeNode.raw_score.ToString());

    if (traitTreeNode.children != null && traitTreeNode.children.Length > 0)
      foreach (TraitTreeNode childNode in traitTreeNode.children)
        LogTraitTree(childNode);
  }

  private void LogConsumptionPreferencesCategory(ConsumptionPreferencesCategoryNode categoryNode)
  {
    Log.Debug("TestPersonalityInsightsV3", "consumption_preference_category_id: {0} | name: {1}", categoryNode.consumption_preference_category_id, categoryNode.name);

    foreach (ConsumptionPreferencesNode preferencesNode in categoryNode.consumption_preferences)
      Log.Debug("TestPersonalityInsightsV3", "\t consumption_preference_id: {0} | name: {1} | score: {2}",
          string.IsNullOrEmpty(preferencesNode.consumption_preference_id) ? "null" : preferencesNode.consumption_preference_id,
          string.IsNullOrEmpty(preferencesNode.name) ? "null" : preferencesNode.name,
          string.IsNullOrEmpty(preferencesNode.score.ToString()) ? "null" : preferencesNode.score.ToString());
  }
}
