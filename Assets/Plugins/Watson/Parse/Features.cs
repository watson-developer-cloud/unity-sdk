using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.Utilities;

public class Features : QuestionComponentBase {
	private List<FeatureItem> FeatureItems = new List<FeatureItem>();
	private ObservedList<string> m_FeatureData = new ObservedList<string>();

	[Header("Features")]
	[SerializeField]
	private FeatureItem[] m_FeatureItems;
	
	new void Start()
	{
		base.Start ();
	}

	new public void Init()
	{
		base.Init ();
		//	TODO do this correctly
		for (int i = 0; i < m_FeatureItems.Length; i++) {
			m_FeatureItems[i].m_feature = qWidget.Answers.answers[0].features[i].displayLabel;
			m_FeatureItems[i].m_featureIndex = qWidget.Answers.answers[0].features[i].weightedScore;
		}
		m_FeatureData.Added += onAdd;
	}
	
	private void onAdd()
	{
		Debug.Log ("feature added");
		FeatureItem featureItem = new FeatureItem ();
		FeatureItems.Add (featureItem);
		featureItem.m_feature = m_FeatureData [m_FeatureData.Count];
	}
}
