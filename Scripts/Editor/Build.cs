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

using UnityEngine;
using UnityEditor;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections.Generic;
using System.IO;
using System;

public class Build
{
    public static string BuildError { get; set; }

    #region Build Options
    public static bool IsBuilding
    {
        get { return EditorPrefs.GetInt(Application.productName + "IsBuilding") != 0; }
        set { EditorPrefs.SetInt(Application.productName + "IsBuilding", value ? 1 : 0); }
    }
    public static BuildTarget BuildTarget
    {
        get { return (BuildTarget)EditorPrefs.GetInt(Application.productName + "BuildTarget"); }
        set { EditorPrefs.SetInt(Application.productName + "BuildTarget", (int)value); }
    }
    public static BuildOptions BuildOptions
    {
        get { return (BuildOptions)EditorPrefs.GetInt(Application.productName + "BuildOptions"); }
        set { EditorPrefs.SetInt(Application.productName + "BuildOptions", (int)value); }
    }
    #endregion

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // start back up our build co-routine on script reloads..
        if (IsBuilding)
        {
            Runnable.EnableRunnableInEditor();
            Runnable.Run(ExecuteBuild());
        }
    }

    private static string[] GetBuildScenes()
    {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene == null || !scene.enabled)
                continue;
            scenes.Add(scene.path);
        }

        return scenes.ToArray();
    }

    /// <summary>
    /// Get the build path for the specified target.
    /// </summary>
    /// <param name="target">The BuildTarget.</param>
    /// <returns>The full path to the build.</returns>
    public static string GetBuildPath(BuildTarget target, bool bCleanTarget = false)
    {
        string projectName = Path.GetFileNameWithoutExtension(Application.productName);
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            projectName += ".exe";
        else if (target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal)
            projectName += ".app";
        else if (target == BuildTarget.Android)
            projectName += ".apk";
        else if (target == BuildTarget.iOS)
            projectName += ".ipa";

        string directory = Application.dataPath + "/../Clients/" + target.ToString();
        if (bCleanTarget && Directory.Exists(directory))
            Directory.Delete(directory, true);

        Directory.CreateDirectory(directory);
        return directory + "/" + projectName;
    }

    public static int StartBuild(BuildTarget target)
    {
        if (!IsBuilding)
        {
            IsBuilding = true;
            BuildTarget = target;

            Runnable.EnableRunnableInEditor();
            return Runnable.Run(ExecuteBuild());
        }

        return -1;
    }

    private static IEnumerator ExecuteBuild()
    {
        yield return null;

        if (BuildTarget == BuildTarget.iOS)
        {
            /// generate the AOT code, wait for it to be compiled..
            FullSerializer.AotHelpers.BuildAOT();
            while (EditorApplication.isCompiling)
                yield return null;
        }

        string[] buildScenes = GetBuildScenes();
        string buildPath = GetBuildPath(BuildTarget, true);

        BuildError = string.Empty;
        try
        {
            BuildError = BuildPipeline.BuildPlayer(buildScenes, buildPath, BuildTarget, BuildOptions);
        }
        catch (Exception e)
        {
            BuildError = e.ToString();
        }

        if (BuildTarget == BuildTarget.iOS)
            FullSerializer.AotHelpers.CleanAOT();
        IsBuilding = false;

        // if BuildPlayer returned no error, but we can't find the file, flag this build as a failure then..
        if (string.IsNullOrEmpty(BuildError) && !File.Exists(buildPath) && !Directory.Exists(buildPath))
            BuildError = "Failed to build player: " + buildPath;

        // check the command line arguments, if we find -executeMethod Build.* then quit this editor..
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; ++i)
        {
            if (args[i] == "-executemethod" && (i + 1) < args.Length && args[i + 1].StartsWith("Build."))
                EditorApplication.Exit(string.IsNullOrEmpty(BuildError) ? 0 : 1);
        }

        yield break;
    }


    #region Build Options
    [MenuItem("Watson/Build/Options/Development Build/On", false, 200)]
    public static void DevelopmentBuildOn()
    {
        BuildOptions |= BuildOptions.Development;
    }

    [MenuItem("Watson/Build/Options/Development Build/On", true, 200)]
    public static bool CanDevelopmentBuildOn()
    {
        return (BuildOptions & BuildOptions.Development) == 0;
    }

    [MenuItem("Watson/Build/Options/Development Build/Off", false, 200)]
    public static void DevelopmentBuildOff()
    {
        BuildOptions &= ~BuildOptions.Development;
    }

    [MenuItem("Watson/Build/Options/Development Build/Off", true, 200)]
    public static bool CanDevelopmentBuildOff()
    {
        return (BuildOptions & BuildOptions.Development) != 0;
    }
    #endregion

    #region Build Players
    [MenuItem("Watson/Build/Player/Windows x86", false, 200)]
    public static void BuildWindows()
    {
        StartBuild(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Watson/Build/Player/Windows x64", false, 200)]
    public static void BuildWindows64()
    {
        StartBuild(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Watson/Build/Player/OSX x86", false, 200)]
    public static void BuildOSX()
    {
        StartBuild(BuildTarget.StandaloneOSXIntel);
    }

    [MenuItem("Watson/Build/Player/OSX x64", false, 200)]
    public static void BuildOSX64()
    {
        StartBuild(BuildTarget.StandaloneOSXIntel64);
    }

    [MenuItem("Watson/Build/Player/OSX Universal", false, 200)]
    public static void BuildOSXUniversal()
    {
        StartBuild(BuildTarget.StandaloneOSXUniversal);
    }

    [MenuItem("Watson/Build/Player/Android", false, 200)]
    public static void BuildAndroid()
    {
        StartBuild(BuildTarget.Android);
    }

    [MenuItem("Watson/Build/Player/iOS", false, 200)]
    public static void BuildIOS()
    {
        StartBuild(BuildTarget.iOS);
    }
    #endregion

}

#endif
