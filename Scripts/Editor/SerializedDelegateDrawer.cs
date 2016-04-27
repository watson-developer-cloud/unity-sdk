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

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System;

namespace IBM.Watson.DeveloperCloud.Editor
{
    /// <summary>
    /// PropertyDrawer for SerializedDelegate type.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializedDelegate))]
    public class SerializedDelegateDrawer : PropertyDrawer
    {
        const float rows = 4;

        public override void OnGUI(Rect pos, SerializedProperty properties, GUIContent label)
        {
            SerializedDelegate target = DrawerHelper.GetParent(properties) as SerializedDelegate;
            if (target == null)
                return;

            SerializedProperty targetProperty = properties.FindPropertyRelative("m_Target");
            SerializedProperty methodProperty = properties.FindPropertyRelative("m_Method");
            SerializedProperty componentProperty = properties.FindPropertyRelative("m_Component");

            // pass through label
            EditorGUIUtility.LookLikeControls();
            EditorGUI.LabelField(
                new Rect(pos.x, pos.y, pos.width / 2, pos.height / rows),
                label
            );

            // target + method section
            EditorGUI.indentLevel++;

            // select target
            if (EditorGUI.PropertyField(new Rect(pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows), targetProperty))
                Log.Debug("SerializedDelegate", "PropertyField()");

            List<string> components = GetComponents(targetProperty);
            if (components != null)
            {
                int selected = components.IndexOf(componentProperty.stringValue);
                int select = EditorGUI.Popup(new Rect(pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows),
                    "Component", selected, components.ToArray());

                if (select != selected)
                    componentProperty.stringValue = components[select];

                List<string> methods = GetComponentMethods(target.DelegateType, targetProperty, componentProperty);
                if (methods != null)
                {
                    selected = methods.IndexOf(methodProperty.stringValue);
                    select = EditorGUI.Popup(new Rect(pos.x, pos.y += pos.height / rows, pos.width, pos.height / rows),
                        "Method", selected, methods.ToArray());

                    if (select != selected)
                        methodProperty.stringValue = methods[select];
                }
            }

            EditorGUI.indentLevel--;
        }

        public List<string> GetComponents(SerializedProperty targetProperty)
        {
            GameObject target = targetProperty.objectReferenceValue as GameObject;
            if (target == null)
                return null;

            List<string> components = new List<string>();

            foreach (var c in target.GetComponents(typeof(Component)))
            {
                try
                {
                    components.Add(c.GetType().Name);
                }
                catch (Exception)
                {

                }

            }

            return components;
        }

        public List<string> GetComponentMethods(Type delegateType, SerializedProperty targetProperty, SerializedProperty componentProperty)
        {
            GameObject target = targetProperty.objectReferenceValue as GameObject;
            if (target == null)
                return null;
            Component component = target.GetComponent(componentProperty.stringValue);
            if (component == null)
                return null;

            List<string> methods = new List<string>();
            foreach (var method in component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (!method.DeclaringType.IsSubclassOf(typeof(Component)))
                    continue;

                try
                {
                    Delegate.CreateDelegate(delegateType, component, method);
                    methods.Add(method.Name);
                }
                catch (ArgumentException)
                { }
            }

            return methods;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * rows;
        }
    }
}
