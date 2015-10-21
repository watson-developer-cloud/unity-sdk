using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Utilities;

public class Evidence : QuestionComponentBase {
	private List<EvidenceItem> EvidenceItems = new List<EvidenceItem>();
	private ObservedList<string> m_EvidenceData = new ObservedList<string>();

	[Header("Evidence")]
	[SerializeField]
	private EvidenceItem[] m_EvidenceItems;

	new void Start()
	{
		base.Start();
	}

	new public void Init()
	{
		base.Init ();

		//	TODO instantiate FeatureItems from resources
		//	TODO replace <answer> with the outline
		if(qWidget.Answers.answers[0].evidence.Length == 1) {
			m_EvidenceItems[0].m_Evidence = qWidget.Answers.answers[0].evidence[0].decoratedPassage;
			m_EvidenceItems[1].gameObject.SetActive(false);
		} else {
			m_EvidenceItems[1].gameObject.SetActive(true);
			m_EvidenceItems[0].m_Evidence = qWidget.Answers.answers[0].evidence[0].decoratedPassage;
			m_EvidenceItems[1].m_Evidence = qWidget.Answers.answers[0].evidence[1].decoratedPassage;
		}

		m_EvidenceData.Added += onAdd;
	}

	private void onAdd()
	{
		base.Init ();
		EvidenceItem evidenceItem = new EvidenceItem ();
		EvidenceItems.Add (evidenceItem);
		evidenceItem.m_Evidence = m_EvidenceData [m_EvidenceData.Count];
	}
}
