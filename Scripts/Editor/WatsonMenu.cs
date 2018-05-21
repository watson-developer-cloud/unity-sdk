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
        private const string TITLE = "Watson Unity SDK";
        private const string SIGNUP_FOR_IBM_CLOUD_MSG = "Thanks for installing the Watson Unity SDK.\nSign up for IBM Cloud?";
        private const string MENU_ITEM_MSG = "The IBM Cloud signup link can be found in the \"Watson\" menu.";
        private const string YES = "Yes";
        private const string NO = "No";
        private const string OK = "Ok";
        private const string WAS_IBM_CLOUD_SIGNUP_PROMPTED = "WasIBMCloudSignupPrompted";

        [MenuItem("Watson/API Reference", false, 100)]
        private static void ShowAPIReference()
        {
            Application.OpenURL("https://watson-developer-cloud.github.io/unity-sdk/");
        }
        
        [MenuItem("Watson/Signup for IBM Cloud", false, 101)]
        private static void SignupForIBMCloud()
        {
            PlayerPrefs.SetInt(WAS_IBM_CLOUD_SIGNUP_PROMPTED, 1);
            PlayerPrefs.Save();

            IBMCloudSignup.OpenIBMCloudWebsite();
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if(!PlayerPrefs.HasKey(WAS_IBM_CLOUD_SIGNUP_PROMPTED))
            {
                OpenOnboardingDialog();
            }
        }

        private static void OpenOnboardingDialog()
        {
            PlayerPrefs.SetInt(WAS_IBM_CLOUD_SIGNUP_PROMPTED, 1);
            PlayerPrefs.Save();

            if (EditorUtility.DisplayDialog(TITLE, SIGNUP_FOR_IBM_CLOUD_MSG, YES, NO))
            {
                IBMCloudSignup.OpenIBMCloudWebsite();
            }
            else
            {
                EditorUtility.DisplayDialog(TITLE, MENU_ITEM_MSG, OK);
            }
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
