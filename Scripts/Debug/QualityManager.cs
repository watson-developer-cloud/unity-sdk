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
using IBM.Watson.DeveloperCloud.Utilities;

namespace IBM.Watson.DeveloperCloud.Debug
{
  /// <summary>
  /// This singleton manages the quality level of all rendering in the application.
  /// </summary>
  public class QualityManager : MonoBehaviour
  {
    /// <summary>
    /// Returns the singleton instance of the QualityManager object.
    /// </summary>
    public static QualityManager Instance { get { return Singleton<QualityManager>.Instance; } }

    void Start()
    {
      //KeyEventManager.Instance.RegisterKeyEvent(Constants.KeyCodes.CHANGE_QUALITY, KeyModifiers.NONE, OnNextQualityLevel ); 
      DebugConsole.Instance.RegisterDebugInfo("QUALITY", OnQualityDebugInfo);
    }

    /// <summary>
    /// Event handler to move to the next quality level.
    /// </summary>
    public void OnNextQualityLevel()
    {
      QualitySettings.SetQualityLevel((QualitySettings.GetQualityLevel() + 1) % QualitySettings.names.Length, true);
    }
    private string OnQualityDebugInfo()
    {
      return QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
  }
}
