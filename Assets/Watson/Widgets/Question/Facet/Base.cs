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
* @author Taj Santiago
*/

using UnityEngine;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Base class for all QuestionWidget Facets except for Chat.
	/// </summary>
    public class Base : MonoBehaviour
    {

        protected virtual void Show() { }
        protected virtual void Hide() { }

        /// <summary>
        /// Sets reference to Avatar and Question Widgets
        /// </summary>
        protected virtual void Start()
        {}

        /// <summary>
        /// Initialization function
        /// </summary>
        public virtual void Init() 
		{
			Clear ();
		}

        /// <summary>
        /// Clears dynamically generated Facet Elements when a question is answered. Called from answer event handler.
        /// </summary>
        public virtual void Clear() { }
    }
}
