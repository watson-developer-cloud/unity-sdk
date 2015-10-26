using UnityEngine;
using System.Collections;
using IBM.Watson.Widgets;

public class FacetBase: MonoBehaviour
{
	protected QuestionWidget m_questionWidget;

	public virtual void Init()
	{
		m_questionWidget = gameObject.GetComponent<QuestionWidget>();
	}
}
