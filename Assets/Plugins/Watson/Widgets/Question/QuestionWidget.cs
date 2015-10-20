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


using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Avatar;
using IBM.Watson.Services.v1;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Avatar of Watson 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class QuestionWidget : Widget
    {
        #region Private Data
        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "Question";
        }
        #endregion

        #region Public Properties
        public ITM.Questions Questions { get; set; }
        public ITM.Answers Answers { get; set; }
        #endregion




    }
}
