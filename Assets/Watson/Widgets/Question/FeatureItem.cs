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
using System.Collections;
using UnityEngine.UI;

public class FeatureItem : MonoBehaviour {
	[SerializeField]
	private Text m_FeatureText;
	[SerializeField]
	private Text m_FeatureIndexText;

	private string _m_Feature;
	public string m_feature
	{
		get { return _m_Feature; }
		set
		{
			_m_Feature = value;
			UpdateFeature();
		}
	}

	private double _m_FeatureIndex;
	public double m_featureIndex
	{
		get { return _m_FeatureIndex; }
		set
		{
			_m_FeatureIndex = value;
			UpdateFeatureIndex();
		}
	}

	private void UpdateFeature()
	{
		if (m_feature != "") {
			gameObject.SetActive (true);
			if(m_feature.Length > 15) {
				string temp = m_feature.Substring (0, 15);
				m_FeatureText.text = temp + "...";
			} else {
				m_FeatureText.text = m_feature;
			}
		} else {
			gameObject.SetActive(false);
		}
	}

	private void UpdateFeatureIndex()
	{
		float featureIndex = (float)m_featureIndex;
		m_FeatureIndexText.text = featureIndex.ToString ("f2");
	}
}
