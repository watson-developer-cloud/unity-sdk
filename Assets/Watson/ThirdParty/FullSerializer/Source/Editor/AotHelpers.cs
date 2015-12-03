using System;
using System.IO;
using System.Reflection;
using FullSerializer;
using UnityEngine;

namespace FullSerializer
{
    public static class AotHelpers
    {
        public const string OutputDirectory = "Assets/Watson/Data/fsAotCompilations/";

        [UnityEditor.MenuItem("FullSerializer/Add Seen Aot Compilations (minimal output)")]
        public static void AddSeenAotCompilations()
        {
            if (Directory.Exists(OutputDirectory) == false)
            {
                Directory.CreateDirectory(OutputDirectory);
            }

            foreach (var aot in fsAotCompilationManager.AvailableAotCompilations)
            {
                Debug.Log("Performing AOT compilation for " + aot.Key.CSharpName(true));
                var path = Path.Combine(OutputDirectory, "AotConverter_" + aot.Key.CSharpName(true, true) + ".cs");
                var compilation = aot.Value;
                File.WriteAllText(path, compilation);
            }
        }

        [UnityEditor.MenuItem("FullSerializer/Add Discoverable Aot Compilations (more output)")]
        public static void AddAllDiscoverableAotCompilations()
        {
            if (Directory.Exists(OutputDirectory) == false)
            {
                Directory.CreateDirectory(OutputDirectory);
            }

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
                            string path = Path.Combine(OutputDirectory, "AotConverter_" + t.CSharpName(true, true) + ".cs");
                            File.WriteAllText(path, compilation);
                        }
                        else
                        {
                            Debug.Log("Failed AOT compilation for " + t.CSharpName(true));
                        }
                    }
                }
            }
        }
    }

}
