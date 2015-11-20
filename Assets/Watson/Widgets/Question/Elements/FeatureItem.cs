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
* @author Taj Santiago (asantiago@us.ibm.com)
*/

using UnityEngine;
using UnityEngine.UI;

namespace IBM.Watson.Widgets.Question
{
	/// <summary>
	/// Controls FeatureItem View. Attached to FeatureItem prewfab.
	/// </summary>
    public class FeatureItem : MonoBehaviour
    {
        [SerializeField]
        private Text m_FeatureText;
        [SerializeField]
        private Text m_FeatureIndexText;

        private string m_FeatureString;
        public string FeatureString
        {
            get { return m_FeatureString; }
            set
            {
                m_FeatureString = value;
                UpdateFeature();
            }
        }

        private double m_FeatureIndex;
        public double FeatureIndex
        {
            get { return m_FeatureIndex; }
            set
            {
                m_FeatureIndex = value;
                UpdateFeatureIndex();
            }
        }

        /// <summary>
        /// Updates the Features. Displays only the first 15 characters.
        /// </summary>
        private void UpdateFeature()
        {
            if (FeatureString != "")
            {
                gameObject.SetActive(true);
                if (FeatureString.Length > 15)
                {
                    string temp = FeatureString.Substring(0, 15);
                    m_FeatureText.text = temp + "...";
                }
                else
                {
                    m_FeatureText.text = FeatureString;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates the Feature Index. 
        /// </summary>
        private void UpdateFeatureIndex()
        {
            float featureIndex = (float)FeatureIndex;
            m_FeatureIndexText.text = featureIndex.ToString("f2");
        }
    }
}
