using UnityEngine;
using System.Collections;
using IBM.Watson.Widgets;

public class QuestionComponentBase : MonoBehaviour {
	protected QuestionWidget qWidget;

	protected void Start () {
		qWidget = gameObject.GetComponent<QuestionWidget>();
	}
}
