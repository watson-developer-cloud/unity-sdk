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
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Editor
{
    public class WatsonMenu : MonoBehaviour
    {
        [MenuItem("Watson/API Reference", false, 100)]
        private static void ShowAPIReference()
        {
            Application.OpenURL("https://watson-developer-cloud.github.io/unity-sdk/");
        }

        private static string FindFile(string directory, string name)
        {
            foreach (var f in Directory.GetFiles(directory))
                if (f.EndsWith(name))
                    return f;

            foreach (var d in Directory.GetDirectories(directory))
            {
                string found = FindFile(d, name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}
#endif