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
 * @author Dogukan Erenel (derenel@us.ibm.com)
 * @author Richard Lyle (rolyle@us.ibm.com)
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using IBM.Watson.Widgets;
using System.Reflection;

namespace IBM.Watson.Editor
{
    public static class WidgetConnector
    {
        [MenuItem("Watson/Widgets/Resolve Connections")]
        private static void AutoConnectWidgets()
        {
            Widget [] widgets = Object.FindObjectsOfType<Widget>();
            foreach( var widget in widgets )
                widget.ResolveConnections();
        }
    };

    [CustomPropertyDrawer(typeof(Widget.Input))]
    public class WidgetInputDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            // get the containing object.. (e.g. SpeechToTextWidget)
            object parent = property.serializedObject.targetObject;
            // find the property in question as a data member..
            MemberInfo [] infos = parent.GetType().GetMember( property.name, 
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField );
            foreach( var info in infos )
            {
                FieldInfo field = info as FieldInfo;
                if ( field == null )
                    continue;

                Widget.Input input = field.GetValue( parent ) as Widget.Input;
                if ( input == null )
                    continue;
                if ( input.Owner == null )
                    input.Owner = parent as Widget;

                // TODO: Make this read-only if possible.
                //EditorGUI.BeginDisabledGroup(true);
                EditorGUI.TextField( position, input.FullInputName );
                //EditorGUI.EndDisabledGroup();
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
	}
}

#endif
