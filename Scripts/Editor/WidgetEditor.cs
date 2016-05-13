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
using IBM.Watson.DeveloperCloud.Widgets;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Editor
{
    public static class WidgetConnector
    {
        [MenuItem("Watson/Widgets/Resolve Connections", false, 4)]
        private static void AutoConnectWidgets()
        {
            Widget[] widgets = Object.FindObjectsOfType<Widget>();
            foreach (var widget in widgets)
                widget.ResolveConnections();
        }
    };

    [CustomPropertyDrawer(typeof(Widget.Input))]
    public class WidgetInputDrawer : PropertyDrawer
    {
        const float m_Rows = 3;
        private bool m_Expanded = true;
        private static Dictionary<string, bool> sm_ExpandedStates = new Dictionary<string, bool>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!sm_ExpandedStates.ContainsKey(property.propertyPath))
                sm_ExpandedStates[property.propertyPath] = true;

            if (sm_ExpandedStates[property.propertyPath])
                return base.GetPropertyHeight(property, label) * m_Rows;
            else
                return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            Widget.Input target = DrawerHelper.GetParent(property) as Widget.Input;
            if (target == null)
                return;
            if (target.Owner == null)
                target.Owner = property.serializedObject.targetObject as Widget;

            //EditorGUIUtility.LookLikeControls();
            bool expanded_state = sm_ExpandedStates[property.propertyPath];
            bool expanded = EditorGUI.Foldout(m_Expanded ? new Rect(pos.x, pos.y, pos.width / 2, pos.height / m_Rows) : pos, expanded_state, label);
            if (expanded_state)
            {
                EditorGUI.indentLevel += 1;
                EditorGUI.LabelField(new Rect(pos.x, pos.y += pos.height / m_Rows, pos.width, pos.height / m_Rows),
                    "Input Name: " + target.FullInputName);
                EditorGUI.LabelField(new Rect(pos.x, pos.y += pos.height / m_Rows, pos.width, pos.height / m_Rows),
                    "Data Type: " + target.DataTypeName);
                EditorGUI.indentLevel -= 1;
            }
            sm_ExpandedStates[property.propertyPath] = expanded;
        }
    }
}

#endif
