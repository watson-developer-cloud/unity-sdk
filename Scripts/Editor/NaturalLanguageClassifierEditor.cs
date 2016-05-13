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
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;
using IBM.Watson.DeveloperCloud.Logging;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer;

namespace IBM.Watson.DeveloperCloud.Editor
{

    class NaturalLanguageClassifierEditor : EditorWindow
    {
        #region Private Types
        private class ClassifierData
        {
            [fsIgnore]
            public string FileName { get; set; }
            public bool Expanded { get; set; }
            public bool InstancesExpanded { get; set; }
            public bool ClassesExpanded { get; set; }
            public string Name { get; set; }
            public string Language { get; set; }
            public Dictionary<string, List<string>> Data { get; set; }
            public Dictionary<string, bool> DataExpanded { get; set; }

            public void Import(string filename)
            {
                if (Data == null)
                    Data = new Dictionary<string, List<string>>();

                string[] lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    int nSeperator = line.LastIndexOf(',');
                    if (nSeperator < 0)
                        continue;

                    string c = line.Substring(nSeperator + 1);
                    string phrase = line.Substring(0, nSeperator);

                    if (!Data.ContainsKey(c))
                        Data[c] = new List<string>();
                    Data[c].Add(phrase);
                }
            }

            public string Export()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kp in Data)
                {
                    foreach (var p in kp.Value)
                    {
                        sb.Append(p + "," + kp.Key + "\n");
                    }
                }

                return sb.ToString();
            }

            public void Save(string filename)
            {
                fsData data = null;
                fsResult r = sm_Serializer.TrySerialize(typeof(ClassifierData), this, out data);
                if (!r.Succeeded)
                    throw new Exception("Failed to serialize ClassifierData: " + r.FormattedMessages);

                File.WriteAllText(filename, fsJsonPrinter.PrettyJson(data));
                FileName = filename;

                AssetDatabase.Refresh();
            }

            public void Save()
            {
                Save(FileName);
            }

            public bool Load(string filename)
            {
                try
                {
                    string json = File.ReadAllText(filename);

                    fsData data = null;
                    fsResult r = fsJsonParser.Parse(json, out data);
                    if (!r.Succeeded)
                        throw new Exception(r.FormattedMessages);

                    object obj = this;
                    r = sm_Serializer.TryDeserialize(data, obj.GetType(), ref obj);
                    if (!r.Succeeded)
                        throw new Exception(r.FormattedMessages);
                }
                catch (Exception e)
                {
                    Log.Error("NaturalLanguageClassifierEditor", "Failed to load classifier data {1}: {0}", e.ToString(), filename);
                    return false;
                }

                FileName = filename;
                return true;
            }
        };
        #endregion

        private void OnEnable()
        {
#if UNITY_5
            titleContent.text = "Natural Language Classifier Editor";
#endif
            m_WatsonIcon = (Texture2D)Resources.Load(Constants.Resources.WATSON_ICON, typeof(Texture2D));

            Runnable.EnableRunnableInEditor();
        }

        [MenuItem("Watson/Natural Language Classifier Editor", false, 2)]
        private static void EditConfig()
        {
            GetWindow<NaturalLanguageClassifierEditor>().Show();
        }

        private string m_ClassifiersFolder = null;
        private Texture m_WatsonIcon = null;
        private Vector2 m_ScrollPos = Vector2.zero;
        private NaturalLanguageClassifier m_NaturalLanguageClassifier = new NaturalLanguageClassifier();
        private Classifiers m_Classifiers = null;
        private static fsSerializer sm_Serializer = new fsSerializer();
        private List<ClassifierData> m_ClassifierData = null;
        private string m_NewClassifierName = null;
        private string m_NewClassifierLang = "en";
        private bool m_Refreshing = false;

        private void OnGetClassifiers(Classifiers classifiers)
        {
            m_Refreshing = false;
            m_Classifiers = classifiers;
            foreach (var c in m_Classifiers.classifiers)
            {
                m_NaturalLanguageClassifier.GetClassifier(c.classifier_id, OnGetClassifier);
            }
        }

        private void OnGetClassifier(Classifier details)
        {
            foreach (var c in m_Classifiers.classifiers)
                if (c.classifier_id == details.classifier_id)
                {
                    c.status = details.status;
                    c.status_description = details.status_description;
                }
        }

        private void OnDeleteClassifier(bool success)
        {
            if (!success)
                Log.Error("Natural Language Classifier Trainer", "Failed to delete classifier.");
            else
                OnRefresh();
        }

        private void OnClassiferTrained(Classifier classifier)
        {
            if (classifier == null)
                EditorUtility.DisplayDialog("ERROR", "Failed to train classifier.", "OK");
            else
                OnRefresh();
        }

        private static string FindDirectory(string check, string name)
        {
            foreach (var d in Directory.GetDirectories(check))
            {
                string dir = d.Replace("\\", "/");        // normalize the slashes
                if (dir.EndsWith(name))
                    return d;

                string found = FindDirectory(d, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        private void OnRefresh()
        {
            if (!m_Refreshing)
            {
                m_ClassifiersFolder = Path.Combine(Application.dataPath, Config.Instance.ClassifierDirectory);
                if (!Directory.Exists(m_ClassifiersFolder))
                    Directory.CreateDirectory(m_ClassifiersFolder);

                m_ClassifierData = new List<ClassifierData>();
                foreach (var file in Directory.GetFiles(m_ClassifiersFolder, "*.json"))
                {
                    ClassifierData data = new ClassifierData();
                    if (data.Load(file))
                        m_ClassifierData.Add(data);
                }

                if (!m_NaturalLanguageClassifier.GetClassifiers(OnGetClassifiers))
                    Log.Error("Natural Language Classifier Trainer", "Failed to request classifiers, please make sure your NaturalLanguageClassifierV1 service has credentials configured.");
                else
                    m_Refreshing = true;
            }
        }

        private string m_NewClassName = string.Empty;
        private string m_NewPhrase = string.Empty;
        private bool m_DisplayClassifiers = false;
        private bool m_handleRepaintError = false;

        private void OnGUI()
        {
            if (Event.current.type == EventType.repaint && !m_handleRepaintError)
            {
                m_handleRepaintError = true;
                return;
            }

            GUILayout.Label(m_WatsonIcon);

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            EditorGUILayout.BeginVertical();

            if (m_Refreshing)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("Refreshing...");
                EditorGUI.EndDisabledGroup();
            }
            else if (m_ClassifierData == null || GUILayout.Button("Refresh"))
                OnRefresh();

            EditorGUILayout.LabelField("Classifiers:");
            //EditorGUI.indentLevel += 1;

            if (m_ClassifierData != null)
            {
                ClassifierData deleteClassifier = null;
                foreach (var data in m_ClassifierData)
                {
                    EditorGUILayout.BeginHorizontal();

                    bool expanded = data.Expanded;
                    data.Expanded = EditorGUILayout.Foldout(expanded, data.Name + " [Language: " + data.Language + "]");
                    if (data.Expanded != expanded)
                        data.Save();

                    if (GUILayout.Button("Import", GUILayout.Width(100)))
                    {
                        var path = EditorUtility.OpenFilePanel("Select Training File", "", "csv");
                        if (!string.IsNullOrEmpty(path))
                        {
                            try
                            {
                                data.Import(path);
                            }
                            catch
                            {
                                EditorUtility.DisplayDialog("Error", "Failed to load training data: " + path, "OK");
                            }
                        }
                    }
                    if (GUILayout.Button("Export", GUILayout.Width(100)))
                    {
                        var path = EditorUtility.SaveFilePanel("Export Training file", Application.dataPath, "", "csv");
                        if (!string.IsNullOrEmpty(path))
                            File.WriteAllText(path, data.Export());
                    }
                    if (GUILayout.Button("Save", GUILayout.Width(100)))
                        data.Save();
                    if (GUILayout.Button("Delete", GUILayout.Width(100)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm", "Please confirm you want to delete classifier: " + data.Name, "Yes", "No"))
                            deleteClassifier = data;
                    }
                    if (GUILayout.Button("Train", GUILayout.Width(100)))
                    {
                        string classifierName = data.Name + "/" + DateTime.Now.ToString();

                        if (EditorUtility.DisplayDialog("Confirm", "Please confirm you want to train a new instance: " + classifierName, "Yes", "No"))
                        {
                            if (!m_NaturalLanguageClassifier.TrainClassifier(classifierName, data.Language, data.Export(), OnClassiferTrained))
                                EditorUtility.DisplayDialog("Error", "Failed to train classifier.", "OK");
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (expanded)
                    {
                        EditorGUI.indentLevel += 1;

                        bool instancesExpanded = data.InstancesExpanded;
                        data.InstancesExpanded = EditorGUILayout.Foldout(instancesExpanded, "Instances");
                        if (instancesExpanded != data.InstancesExpanded)
                            data.Save();

                        if (instancesExpanded)
                        {
                            EditorGUI.indentLevel += 1;
                            if (m_Classifiers != null)
                            {
                                for (int i = 0; i < m_Classifiers.classifiers.Length; ++i)
                                {
                                    Classifier cl = m_Classifiers.classifiers[i];
                                    if (!cl.name.StartsWith(data.Name + "/"))
                                        continue;

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Name: " + cl.name);
                                    if (GUILayout.Button("Delete", GUILayout.Width(100)))
                                    {
                                        if (EditorUtility.DisplayDialog("Confirm", string.Format("Confirm delete of classifier {0}", cl.classifier_id), "YES", "NO")
                                            && !m_NaturalLanguageClassifier.DeleteClassifer(cl.classifier_id, OnDeleteClassifier))
                                        {
                                            EditorUtility.DisplayDialog("Error", "Failed to delete classifier.", "OK");
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUI.indentLevel += 1;
                                    EditorGUILayout.LabelField("ID: " + cl.classifier_id);
                                    EditorGUILayout.LabelField("Created: " + cl.created.ToString());
                                    EditorGUILayout.LabelField("Status: " + cl.status);

                                    EditorGUI.indentLevel -= 1;
                                }
                            }
                            EditorGUI.indentLevel -= 1;
                        }

                        if (data.Data == null)
                            data.Data = new Dictionary<string, List<string>>();
                        if (data.DataExpanded == null)
                            data.DataExpanded = new Dictionary<string, bool>();

                        bool classesExpanded = data.ClassesExpanded;
                        data.ClassesExpanded = EditorGUILayout.Foldout(classesExpanded, "Classes");
                        if (classesExpanded != data.ClassesExpanded)
                            data.Save();

                        if (classesExpanded)
                        {
                            EditorGUI.indentLevel += 1;

                            EditorGUILayout.BeginHorizontal();
                            m_NewClassName = EditorGUILayout.TextField(m_NewClassName);
                            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_NewClassName));

                            GUI.SetNextControlName("AddClass");
                            if (GUILayout.Button("Add Class", GUILayout.Width(100)))
                            {
                                data.Data[m_NewClassName] = new List<string>();
                                data.Save();

                                m_NewClassName = string.Empty;
                                GUI.FocusControl("AddClass");
                            }
                            EditorGUI.EndDisabledGroup();
                            EditorGUILayout.EndHorizontal();

                            string deleteClass = string.Empty;
                            foreach (var kp in data.Data)
                            {
                                bool classExpanded = true;
                                data.DataExpanded.TryGetValue(kp.Key, out classExpanded);

                                EditorGUILayout.BeginHorizontal();
                                data.DataExpanded[kp.Key] = EditorGUILayout.Foldout(classExpanded, "Class: " + kp.Key);
                                if (classExpanded != data.DataExpanded[kp.Key])
                                    data.Save();

                                if (GUILayout.Button("Delete", GUILayout.Width(100)))
                                {
                                    if (EditorUtility.DisplayDialog("Confirm", "Please confirm you want to delete class: " + kp.Key, "Yes", "No"))
                                        deleteClass = kp.Key;
                                }
                                EditorGUILayout.EndHorizontal();

                                if (classExpanded)
                                {
                                    EditorGUI.indentLevel += 1;

                                    EditorGUILayout.BeginHorizontal();
                                    m_NewPhrase = EditorGUILayout.TextField(m_NewPhrase);
                                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_NewPhrase));

                                    GUI.SetNextControlName("AddPhrase");
                                    if (GUILayout.Button("Add Phrase", GUILayout.Width(100)))
                                    {
                                        kp.Value.Add(m_NewPhrase);
                                        data.Save();

                                        m_NewPhrase = string.Empty;
                                        GUI.FocusControl("AddPhrase");
                                    }
                                    EditorGUI.EndDisabledGroup();
                                    EditorGUILayout.EndHorizontal();

                                    for (int i = 0; i < kp.Value.Count; ++i)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        kp.Value[i] = EditorGUILayout.TextField(kp.Value[i]);

                                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                                            kp.Value.RemoveAt(i--);

                                        EditorGUILayout.EndHorizontal();
                                    }

                                    EditorGUI.indentLevel -= 1;
                                }
                            }

                            if (!string.IsNullOrEmpty(deleteClass))
                            {
                                data.Data.Remove(deleteClass);
                                data.DataExpanded.Remove(deleteClass);
                                data.Save();
                            }

                            EditorGUI.indentLevel -= 1;
                        }

                        EditorGUI.indentLevel -= 1;
                    }
                }

                if (deleteClassifier != null)
                {
                    File.Delete(deleteClassifier.FileName);
                    m_ClassifierData.Remove(deleteClassifier);

                    AssetDatabase.Refresh();
                }
            }
            //EditorGUI.indentLevel -= 1;

            EditorGUILayout.LabelField("Create Classifier:");
            EditorGUI.indentLevel += 1;

            m_NewClassifierName = EditorGUILayout.TextField("Name", m_NewClassifierName);
            m_NewClassifierLang = EditorGUILayout.TextField("Language", m_NewClassifierLang);

            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_NewClassifierLang) || string.IsNullOrEmpty(m_NewClassifierName));
            if (GUILayout.Button("Create"))
            {
                m_NewClassifierName = m_NewClassifierName.Replace("/", "_");

                string classifierFile = m_ClassifiersFolder + "/" + m_NewClassifierName + ".json";
                if (!File.Exists(classifierFile)
                    || EditorUtility.DisplayDialog("Confirm", string.Format("Classifier file {0} already exists, are you sure you wish to overwrite?", classifierFile), "YES", "NO"))
                {
                    ClassifierData newClassifier = new ClassifierData();
                    newClassifier.Name = m_NewClassifierName;
                    newClassifier.Language = m_NewClassifierLang;
                    newClassifier.Save(classifierFile);
                    m_NewClassifierName = string.Empty;

                    OnRefresh();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel -= 1;


            bool showAllClassifiers = m_DisplayClassifiers;
            m_DisplayClassifiers = EditorGUILayout.Foldout(showAllClassifiers, "All Classifier Instances");

            if (showAllClassifiers)
            {
                EditorGUI.indentLevel += 1;

                if (m_Classifiers != null)
                {
                    for (int i = 0; i < m_Classifiers.classifiers.Length; ++i)
                    {
                        Classifier cl = m_Classifiers.classifiers[i];

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Name: " + cl.name);
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            if (EditorUtility.DisplayDialog("Confirm", string.Format("Confirm delete of classifier {0}", cl.classifier_id), "YES", "NO")
                                && !m_NaturalLanguageClassifier.DeleteClassifer(cl.classifier_id, OnDeleteClassifier))
                            {
                                EditorUtility.DisplayDialog("Error", "Failed to delete classifier.", "OK");
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUI.indentLevel += 1;
                        EditorGUILayout.LabelField("ID: " + cl.classifier_id);
                        EditorGUILayout.LabelField("Created: " + cl.created.ToString());
                        EditorGUILayout.LabelField("Status: " + cl.status);

                        EditorGUI.indentLevel -= 1;
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

#endif
