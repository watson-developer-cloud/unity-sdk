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

namespace IBM.Watson.Utilities
{
    static class Utility
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
    }
}
