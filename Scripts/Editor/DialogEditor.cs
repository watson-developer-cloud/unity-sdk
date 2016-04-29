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

using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Dialog.v1;
using UnityEditor;
using UnityEngine;
using System.IO;
using IBM.Watson.DeveloperCloud.Logging;

namespace IBM.Watson.DeveloperCloud.Editor
{

    class DialogEditor : EditorWindow
    {
        private void OnEnable()
        {
#if UNITY_5
            titleContent.text = "Dialog Editor";
#endif
            m_WatsonIcon = (Texture2D)Resources.Load(Constants.Resources.WATSON_ICON, typeof(Texture2D));

            Runnable.EnableRunnableInEditor();
        }

        [MenuItem("Watson/Dialog Editor", false, 1)]
        private static void EditConfig()
        {
            DialogEditor window = GetWindow<DialogEditor>();
            window.Show(true);
        }

        private Texture m_WatsonIcon = null;
        private Vector2 m_ScrollPos = Vector2.zero;
        private Dialog m_Dialog = new Dialog();
        private Dialogs m_Dialogs = null;
        private string m_NewDialogName = null;
        private bool m_Refreshing = false;
        private string m_CurrentDirectory = Application.dataPath;

        private void OnGetDialogs(Dialogs dialogs)
        {
            m_Dialogs = dialogs;
            m_Refreshing = false;
        }

        private void OnDeleteDialog(bool success)
        {
            if (!success)
                Log.Error("DialogEditor", "Failed to delete dialog.");
            else
                OnRefresh();
        }

        private void OnDownloadDialog(bool success)
        {
            if (!success)
                Log.Error("DialogEditor", "Failed to download dialog.");
            else
                Log.Status("DialogEditor", "Dialog downloaded.");
        }

        private void OnUploadDialog(string dialogId)
        {
            if (string.IsNullOrEmpty(dialogId))
                Log.Error("DialogEditor", "Failed to upload dialog.");
            else
                OnRefresh();
        }

        private void OnRefresh()
        {
            if (!m_Refreshing)
            {
                if (!m_Dialog.GetDialogs(OnGetDialogs))
                    Log.Error("DialogEditor", "Failed to request dialogs.");
                else
                    m_Refreshing = true;
            }
        }

        private void OnGUI()
        {
            GUILayout.Label(m_WatsonIcon);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);

            if (m_Refreshing)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Refreshing...");
                EditorGUI.EndDisabledGroup();
            }
            else if (m_Dialogs == null || GUILayout.Button("Refresh"))
                OnRefresh();

            EditorGUILayout.LabelField("Dialogs:");
            EditorGUILayout.BeginVertical();
            EditorGUI.indentLevel += 1;
            if (m_Dialogs != null)
            {
                for (int i = 0; i < m_Dialogs.dialogs.Length; ++i)
                {
                    BeginWindows();

                    EditorGUILayout.BeginHorizontal();
                    DialogEntry d = m_Dialogs.dialogs[i];

                    EditorGUILayout.LabelField(string.Format("Name: {0}, ID: {1}", d.name, d.dialog_id));

                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Confirm", string.Format("Confirm delete of dialog {0}", d.dialog_id), "YES", "NO"))
                            m_Dialog.DeleteDialog(d.dialog_id, OnDeleteDialog);
                    }
                    if (GUILayout.Button("Download"))
                    {
                        var path = EditorUtility.SaveFilePanel("Save Dialog", m_CurrentDirectory, "Dialog", "xml");
                        if (!string.IsNullOrEmpty(path))
                        {
                            m_CurrentDirectory = Path.GetDirectoryName(path);
                            m_Dialog.DownloadDialog(d.dialog_id, path, OnDownloadDialog);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    EndWindows();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.LabelField("Upload Dialog:");
            EditorGUI.indentLevel += 1;

            m_NewDialogName = EditorGUILayout.TextField("Name", m_NewDialogName);
            if (GUILayout.Button("Upload"))
            {
                var path = EditorUtility.OpenFilePanel("Select Dialog File", m_CurrentDirectory, "xml");
                if (!string.IsNullOrEmpty(path))
                {
                    m_CurrentDirectory = Path.GetDirectoryName(path);
                    if (string.IsNullOrEmpty(m_NewDialogName))
                        m_NewDialogName = Path.GetFileNameWithoutExtension(path);

                    if (!m_Dialog.UploadDialog(m_NewDialogName, OnUploadDialog, path))
                        Log.Error("DialogEditor", "Failed to upload dialog.");
                }

                m_NewDialogName = null;
            }
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.EndScrollView();

            EndWindows();
        }
    }
}

#endif
