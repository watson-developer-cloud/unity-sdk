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

public class POSControl : MonoBehaviour {
	private RectTransform m_rectTransform;
	private Color colorDark;
	private Color colorLight;
	private float m_transitionTime = 0.5f;

	private bool _isHighlighted = true;
	public bool isHighlighted
	{
		get { return _isHighlighted; }
		set {
			_isHighlighted = value;
			Debug.Log("isHighlighted: " + isHighlighted);
			LeanTween.textColor(m_rectTransform, isHighlighted ? colorLight : colorDark, m_transitionTime);
		}
	}

	void Start()
	{
		colorLight = new Color (0.8f, 0.8f, 0.8f);
		colorDark = new Color (0.3f, 0.3f, 0.3f);
		m_rectTransform = gameObject.GetComponent<RectTransform>();
	}
}
