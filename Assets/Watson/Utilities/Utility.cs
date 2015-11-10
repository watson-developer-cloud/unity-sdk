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
* @author Richard Lyle (rolyle@us.ibm.com)
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.Utilities
{
    static public class Utility
    {
        /// <summary>
        /// This helper functions returns all Type's that inherit from the given type.
        /// </summary>
        /// <param name="type">The Type to find all types that inherit from the given type.</param>
        /// <returns>A array of all Types that inherit from type.</returns>
        public static Type [] FindAllDerivedTypes( Type type )
        {
            List<Type> types = new List<Type>();
            foreach( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
            {
                foreach( var t in assembly.GetTypes() )
                {
                    if ( t == type || t.IsAbstract )
                        continue;
                    if ( type.IsAssignableFrom( t ) )
                        types.Add( t );                         
                }
            }

            return types.ToArray();
        }

		private static float deltaFloat = 0.0001f;
		/// <summary>
		/// Approximately the specified a, b and tolerance.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="tolerance">Tolerance.</param>
		public static bool Approximately(float a, float b, float tolerance)
		{
			return (Mathf.Abs(a - b) < tolerance);
		}

		/// <summary>
		/// Checks the equality quaternion.
		/// </summary>
		/// <returns><c>true</c>, if equality quaternion was checked, <c>false</c> otherwise.</returns>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool CheckEqualityQuaternion(Quaternion a, Quaternion b){
			return 
				(Approximately(a.eulerAngles.x, b.eulerAngles.x, deltaFloat) || Approximately((a.eulerAngles.x < 0 ? a.eulerAngles.x + 360.0f : a.eulerAngles.x) , (b.eulerAngles.x < 0 ? b.eulerAngles.x + 360.0f : b.eulerAngles.x), deltaFloat)) &&
					(Approximately(a.eulerAngles.y, b.eulerAngles.y, deltaFloat) || Approximately((a.eulerAngles.y < 0 ? a.eulerAngles.y + 360.0f : a.eulerAngles.y) , (b.eulerAngles.y < 0 ? b.eulerAngles.y + 360.0f : b.eulerAngles.y), deltaFloat)) &&
					(Approximately(a.eulerAngles.z, b.eulerAngles.z, deltaFloat) || Approximately((a.eulerAngles.z < 0 ? a.eulerAngles.z + 360.0f : a.eulerAngles.z) , (b.eulerAngles.z < 0 ? b.eulerAngles.z + 360.0f : b.eulerAngles.z), deltaFloat));
		}

		/// <summary>
		/// Finds the object in child of parent object by name of child
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="parent">Parent Object.</param>
		/// <param name="nameChild">Name child.</param>
		public static GameObject FindObject(GameObject parent, string nameChild){
			GameObject childObject = null;

			string[] childPath = nameChild.Split ('/');

			for (int i = 0; i < childPath.Length; i++) {
				string childTransformName = childPath[i];

				Transform[] childerenTransform = parent.GetComponentsInChildren<Transform> (includeInactive: true);
				if (childerenTransform != null) {
					foreach (Transform item in childerenTransform) {
						if(string.Equals(item.name, childTransformName)){
							if(i == childPath.Length - 1){
								childObject = item.gameObject;
							}
							else{
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
		/// Finds the objects in childeren of parent object by name of child
		/// </summary>
		/// <returns>The objects.</returns>
		/// <param name="parent">Parent.</param>
		/// <param name="nameChild">Name child.</param>
		/// <param name="isContains">Check string.contains instead of equality. </param>
		public static T[] FindObjects<T>(GameObject parent, string nameChild, bool isContains = false, bool sortByName = false) where T : Component{
			T[] childObjects = null;
			List<T> listGameObject = new List<T>();

			string[] childPath = nameChild.Split ('/');
			
			for (int i = 0; i < childPath.Length; i++) {
				string childTransformName = childPath[i];
				T[] childerenTransform = parent.GetComponentsInChildren<T> (includeInactive: true);
				if (childerenTransform != null) {
					foreach (T item in childerenTransform) {
						if( (isContains && item.name.Contains(childTransformName)) || string.Equals(item.name, childTransformName) ){
							if(i == childPath.Length - 1){
								listGameObject.Add(item);
							}
							else{
								parent = item.gameObject;
							}
						}
					}
				}
			}

			if (listGameObject.Count > 0) {
				if(sortByName){
					listGameObject.Sort((x, y) => x.name.CompareTo(y.name));
				}
				childObjects = listGameObject.ToArray ();
			}
			
			return childObjects;
		}
    }


}
