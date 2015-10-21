using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ParseTreeTextItem : MonoBehaviour {
	[SerializeField]
	private Text m_ParseTreeTextField;

	private string _m_ParseTreeText;
	public string m_ParseTreeText
	{
		get { return _m_ParseTreeText; }
		set 
		{
			_m_ParseTreeText = value;
			UpdateParseTreeTextField();
		}
	}

	private void UpdateParseTreeTextField()
	{
		m_ParseTreeTextField.text = m_ParseTreeText;
	}
}
