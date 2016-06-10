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