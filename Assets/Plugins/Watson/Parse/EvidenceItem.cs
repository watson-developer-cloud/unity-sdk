using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EvidenceItem : MonoBehaviour {
	[SerializeField]
	private Text m_EvidenceText;

	private string _m_Evidence;
	public string m_Evidence
	{
		get { return _m_Evidence; }
		set 
		{
			_m_Evidence = value;
			UpdateEvidence();
		}
	}

	private void UpdateEvidence()
	{
		m_EvidenceText.text = m_Evidence;
		//	TODO highlight evidence words
	}
}
