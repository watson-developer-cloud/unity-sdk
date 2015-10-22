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
using System.Collections.Generic;
using UnityEngine.UI;

public class ParseTreeTextItem : MonoBehaviour {
	[SerializeField]
	private Text m_ParseTreeTextField;

	private bool _isHighlighted = false;
	public bool isHighlighted
	{
		get { return _isHighlighted; }
		set {
			_isHighlighted = value;
			m_rectTransform = m_ParseTreeTextField.gameObject.GetComponent<RectTransform>();
			LeanTween.textColor(m_rectTransform, isHighlighted ? colorLight : colorDark, m_transitionTime);
		}
	}

	private string _m_ParseTreeWord;
	public string m_ParseTreeWord
	{
		get { return _m_ParseTreeWord; }
		set 
		{
			_m_ParseTreeWord = value;
			UpdateParseTreeTextField();
		}
	}

	[SerializeField]
	private string _m_pos;
	public string m_pos {
		get { return _m_pos; }
		set {
			_m_pos = value;
		}
	}

	[SerializeField]
	private string _m_slot;
	public string m_slot
	{
		get { return _m_slot; }
		set { _m_slot = value; }
	}

	public List<string> m_Features = new List<string>();

	private RectTransform m_rectTransform;
	private Color colorLight = new Color (0.8f, 0.8f, 0.8f);
	private Color colorDark = new Color (0.3f, 0.3f, 0.3f);
	private float m_transitionTime = 0.5f;

	void Start()
	{
		m_rectTransform = m_ParseTreeTextField.gameObject.GetComponent<RectTransform>();
	}

	private void UpdateParseTreeTextField()
	{
		m_ParseTreeTextField.text = m_ParseTreeWord;
	}
}