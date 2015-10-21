using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Location : QuestionComponentBase {
	[SerializeField]
	private Text m_LocationText;

	private string _m_Location;
	public string m_Location
	{
		get { return _m_Location; }
		set 
		{
			_m_Location = value;
			UpdateLocation();
		}
	}

	new void Start ()
	{
		base.Start ();	
	}

	new public void Init()
	{
		base.Init ();
		//m_Location = qWidget.Questions.questions[0].question.;
	}

	private void UpdateLocation()
	{
		m_LocationText.text = m_Location;
	}
}
