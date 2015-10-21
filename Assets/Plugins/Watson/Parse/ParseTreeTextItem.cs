using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ParseTreeTextItem : MonoBehaviour {
	[SerializeField]
	private Text m_ParseTreeTextField;

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

	private string _m_slot;
	public string m_slot
	{
		get { return _m_slot; }
		set { _m_slot = value; }
	}

	public List<string> m_Features = new List<string>();

	private void UpdateParseTreeTextField()
	{
		m_ParseTreeTextField.text = _m_ParseTreeWord;
	}
}
