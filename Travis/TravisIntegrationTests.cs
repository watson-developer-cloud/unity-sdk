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
using System;
using IBM.Watson.DeveloperCloud.UnitTests;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Editor
{
  /// <summary>
  /// This class is executed from batch mode during Travis continuous integration.
  /// </summary>
  public class TravisIntegrationTests : MonoBehaviour
  {
    public static void RunTests()
    {
      Log.Debug("TravisIntegrationTests", "***** Running Integration tests!");

#if UNITY_EDITOR
      Runnable.EnableRunnableInEditor();
#endif
      string ProjectToTest = "";
      string[] args = Environment.GetCommandLineArgs();
      for (int i = 0; i < args.Length; ++i)
      {
        if (args[i] == "-packageOptions" && (i + 1) < args.Length)
        {
          string[] options = args[i + 1].Split(',');
          foreach (string option in options)
          {
            if (string.IsNullOrEmpty(option))
              continue;

            string[] kv = option.Split('=');
            if (kv[0] == "ProjectName")
            {
              ProjectToTest = kv.Length > 1 ? kv[1] : "";
              Log.Status("RunUnitTest", "AutoLunchOptions ProjectToTest:{0}", ProjectToTest);
              break;
            }
          }
        }
      }

      UnitTestManager.ProjectToTest = ProjectToTest;
      UnitTestManager instance = UnitTestManager.Instance;
      instance.QuitOnTestsComplete = true;
      instance.OnTestCompleteCallback = OnTravisIntegrationTestsComplete;
      instance.QueueTests(Utility.FindAllDerivedTypes(typeof(UnitTest)), true);
    }
    static void OnTravisIntegrationTestsComplete()
    {
      Log.Debug("TravisIntegrationTests", " ***** Integration tests complete!");
    }
  }
}
