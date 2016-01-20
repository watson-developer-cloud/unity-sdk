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

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Utility functions.
    /// </summary>
    static public class Utility
    {
        /// <summary>
        /// This helper functions returns all Type's that inherit from the given type.
        /// </summary>
        /// <param name="type">The Type to find all types that inherit from the given type.</param>
        /// <returns>A array of all Types that inherit from type.</returns>
        public static Type[] FindAllDerivedTypes(Type type)
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t == type || t.IsAbstract)
                        continue;
                    if (type.IsAssignableFrom(t))
                        types.Add(t);
                }
            }

            return types.ToArray();
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(double a, double b, double tolerance = 0.0001)
        {
            return (System.Math.Abs(a - b) < tolerance);
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(float a, float b, float tolerance = 0.0001f)
        {
            return (Mathf.Abs(a - b) < tolerance);
        }

        /// <summary>
        /// Approximately the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return Approximately(a.x, b.x, tolerance) && Approximately(a.y, b.y, tolerance) && Approximately(a.z, b.z, tolerance);
        }

        /// <summary>
        /// Approximately Quaternion with the specified a, b and tolerance.
        /// </summary>
        /// <param name="a">The first component.</param>
        /// <param name="b">The second component.</param>
        /// <param name="tolerance">Tolerance.</param>
        public static bool Approximately(Quaternion a, Quaternion b, float tolerance = 0.0001f)
        {
            return
                (Approximately(a.eulerAngles.x, b.eulerAngles.x, tolerance) || Approximately((a.eulerAngles.x < 0 ? a.eulerAngles.x + 360.0f : a.eulerAngles.x), (b.eulerAngles.x < 0 ? b.eulerAngles.x + 360.0f : b.eulerAngles.x), tolerance)) &&
                    (Approximately(a.eulerAngles.y, b.eulerAngles.y, tolerance) || Approximately((a.eulerAngles.y < 0 ? a.eulerAngles.y + 360.0f : a.eulerAngles.y), (b.eulerAngles.y < 0 ? b.eulerAngles.y + 360.0f : b.eulerAngles.y), tolerance)) &&
                    (Approximately(a.eulerAngles.z, b.eulerAngles.z, tolerance) || Approximately((a.eulerAngles.z < 0 ? a.eulerAngles.z + 360.0f : a.eulerAngles.z), (b.eulerAngles.z < 0 ? b.eulerAngles.z + 360.0f : b.eulerAngles.z), tolerance));
        }

        /// <summary>
        /// Finds the object in child of parent object by name of child
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="parent">Parent Object.</param>
        /// <param name="nameChild">Name child.</param>
        public static GameObject FindObject(GameObject parent, string nameChild)
        {
            GameObject childObject = null;

            string[] childPath = nameChild.Split('/');

            for (int i = 0; i < childPath.Length; i++)
            {
                string childTransformName = childPath[i];

                Transform[] childerenTransform = parent.GetComponentsInChildren<Transform>(includeInactive: true);
                if (childerenTransform != null)
                {
                    foreach (Transform item in childerenTransform)
                    {
                        if (string.Equals(item.name, childTransformName))
                        {
                            if (i == childPath.Length - 1)
                            {
                                childObject = item.gameObject;
                            }
                            else
                            {
                                parent = item.gameObject;
                            }
                            break;
                        }
                    }
                }
            }
            return childObject;
        }

        /// <summary>
        /// Finds the objects in children of parent object by name of child
        /// </summary>
        /// <returns>The objects.</returns>
        /// <param name="parent">Parent.</param>
        /// <param name="nameChild">Name child.</param>
        /// <param name="isContains">Check string contains the name instead of equality.</param>
        /// <param name="sortByName">If true, children will be returned sorted by their name.</param>
        public static T[] FindObjects<T>(GameObject parent, string nameChild, bool isContains = false, bool sortByName = false) where T : Component
        {
            T[] childObjects = null;
            List<T> listGameObject = new List<T>();

            string[] childPath = nameChild.Split('/');

            for (int i = 0; i < childPath.Length; i++)
            {
                string childTransformName = childPath[i];
                T[] childerenTransform = parent.GetComponentsInChildren<T>(includeInactive: true);
                if (childerenTransform != null)
                {
                    foreach (T item in childerenTransform)
                    {
                        if ((isContains && item.name.Contains(childTransformName)) || string.Equals(item.name, childTransformName))
                        {
                            if (i == childPath.Length - 1)
                            {
                                listGameObject.Add(item);
                            }
                            else
                            {
                                parent = item.gameObject;
                            }
                        }
                    }
                }
            }

            if (listGameObject.Count > 0)
            {
                if (sortByName)
                {
                    listGameObject.Sort((x, y) => x.name.CompareTo(y.name));
                }
                childObjects = listGameObject.ToArray();
            }

            return childObjects;
        }

        /// <summary>
        /// Get the MD5 hash of a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.Default.GetBytes(s);
            byte[] result = md5.ComputeHash(data);

            StringBuilder output = new StringBuilder();
            foreach (var b in result)
                output.Append(b.ToString("x2"));

            return output.ToString();
        }

        /// <summary>
        /// Removes any tags from a string. (e.g. <title></title>)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveTags(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                int tagStart = s.IndexOf('<');
                while (tagStart >= 0)
                {
                    int tagEnd = s.IndexOf('>', tagStart);
                    if (tagEnd < 0)
                        break;

                    string pre = tagStart > 0 ? s.Substring(0, tagStart) : string.Empty;
                    string post = tagEnd < s.Length ? s.Substring(tagEnd + 1) : string.Empty;
                    s = pre + post;

                    tagStart = s.IndexOf('<');
                }
            }

            return s;
        }

    }


}
