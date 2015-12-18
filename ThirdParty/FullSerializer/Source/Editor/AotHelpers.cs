using System;
using System.IO;
using System.Reflection;
using FullSerializer;
using UnityEngine;
using UnityEditor;

namespace FullSerializer
{
    public static class AotHelpers
    {
        //[MenuItem("Watson/FullSerializer/Add Seen Aot Compilations", false, 5 )]
        public static void AddSeenAotCompilations()
        {
            var outputDirectory = EditorUtility.SaveFolderPanel( "Please select save path?", Application.dataPath,  "fsAotCompilations" );           
            if (!Directory.Exists(outputDirectory) == false)
                Directory.CreateDirectory(outputDirectory);

            foreach (var aot in fsAotCompilationManager.AvailableAotCompilations)
            {
                Debug.Log("Performing AOT compilation for " + aot.Key.CSharpName(true));
                var path = Path.Combine(outputDirectory, "AotConverter_" + aot.Key.CSharpName(true, true) + ".cs");
                var compilation = aot.Value;
                File.WriteAllText(path, compilation);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Watson/FullSerializer/Build AOT", false, 5 )]
        public static void AddAllDiscoverableAotCompilations()
        {
            var outputDirectory = Application.dataPath + "/fsAotCompilations";           
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    bool performAot = false;

                    // check for [fsObject]
                    {
                        var props = t.GetCustomAttributes(typeof(fsObjectAttribute), true);
                        if (props != null && props.Length > 0) performAot = true;
                    }

                    // check for [fsProperty]
                    if (!performAot)
                    {
                        foreach (PropertyInfo p in t.GetProperties())
                        {
                            var props = p.GetCustomAttributes(typeof(fsPropertyAttribute), true);
                            if (props.Length > 0)
                            {
                                performAot = true;
                                break;
                            }
                        }
                    }

                    if (performAot)
                    {
                        string compilation = null;
                        if (fsAotCompilationManager.TryToPerformAotCompilation(t, out compilation))
                        {
                            Debug.Log("Performing AOT compilation for " + t);
                            string path = Path.Combine(outputDirectory, "AotConverter_" + t.CSharpName(true, true) + ".cs");
                            File.WriteAllText(path, compilation);
                        }
                        else
                        {
                            Debug.Log("Failed AOT compilation for " + t.CSharpName(true));
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Watson/FullSerializer/Clean AOT", false, 6 )]
        public static void CleanAOT()
        {
            var outputDirectory = Application.dataPath + "/fsAotCompilations";           
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete( outputDirectory, true );
                File.Delete( outputDirectory + ".meta" );

                AssetDatabase.Refresh();
            }
        }

    }

}
