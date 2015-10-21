using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class POSTextItem : MonoBehaviour {
	[SerializeField]
	private Text m_POSTextField;
	
	private string _m_POSWord;
	public string m_POSWord
	{
		get { return _m_POSWord; }
		set 
		{
			_m_POSWord = value;
			UpdatePOSTextField();
		}
	}

	private void UpdatePOSTextField()
	{
		m_POSTextField.text = m_POSWord;
	}
}
