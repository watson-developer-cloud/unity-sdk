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


using IBM.Watson.Data;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace IBM.Watson.Widgets
{
    /// <summary>
    /// Simple widget for displaying the NLC classification in the UI.
    /// </summary>
    public class ClassDisplayWidget : Widget
    {
        #region Widget interface
        protected override string GetName()
        {
            return "ClassDisplay";
        }
        #endregion

        #region Private Data
        [SerializeField]
        private Text m_ClassDisplay = null;
        [SerializeField]
        private Input m_ClassInput = new Input( "ClassInput", typeof(ClassifyResultData), "OnClassInput" );
        #endregion

        private void OnClassInput( Data data )
        {
            ClassifyResultData results = (ClassifyResultData)data;
            if ( m_ClassDisplay != null )
            {
                m_ClassDisplay.text = string.Format( "Top class: {0} ({1:0.00})", 
                    results.Result.top_class, results.Result.topConfidence );
            }
        }
    }
}
