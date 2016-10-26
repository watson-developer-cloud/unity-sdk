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

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class RunTravisBuild
{
  public static string[] BuildScenes = {
    "Assets/Watson/Scenes/UnitTests/Main.unity",
    "Assets/Watson/Scenes/UnitTests/TestMic.unity",
    "Assets/Watson/Scenes/UnitTests/TestNaturalLanguageClassifier.unity",
    "Assets/Watson/Scenes/UnitTests/TestSpeechToText.unity",
    "Assets/Watson/Scenes/UnitTests/TestTextToSpeech.unity",
    "Assets/Watson/Scenes/UnitTests/UnitTests.unity",
    "Assets/Watson/Examples/WidgetExamples/ExampleDialog.unity",
    "Assets/Watson/Examples/WidgetExamples/ExampleLanguageTranslator.unity"
  };

  static public void OSX()
  {
    BuildPipeline.BuildPlayer(BuildScenes, Application.dataPath + "TestBuildOSX", BuildTarget.StandaloneOSXIntel64, BuildOptions.None);
  }

  static public void Windows()
  {
    BuildPipeline.BuildPlayer(BuildScenes, Application.dataPath + "TestBuildWindows", BuildTarget.StandaloneWindows64, BuildOptions.None);
  }

  static public void Linux()
  {
    BuildPipeline.BuildPlayer(BuildScenes, Application.dataPath + "TestBuildLinux", BuildTarget.StandaloneLinux64, BuildOptions.None);
  }
}
#endif