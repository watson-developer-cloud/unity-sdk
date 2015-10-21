using UnityEngine;
using System.Collections;
using IBM.Watson.Widgets;

public class QuestionComponentBase : MonoBehaviour {
	protected QuestionWidget qWidget;

	protected void Start () 
	{

	}

	public void Init()
	{
		qWidget = gameObject.GetComponent<QuestionWidget>();
	}
}
