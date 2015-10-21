using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FeatureItem : MonoBehaviour {
	[SerializeField]
	private Text m_FeatureText;
	[SerializeField]
	private Text m_FeatureIndexText;

	private string _m_Feature;
	public string m_feature
	{
		get { return _m_Feature; }
		set
		{
			_m_Feature = value;
			UpdateFeature();
		}
	}

	private double _m_FeatureIndex;
	public double m_featureIndex
	{
		get { return _m_FeatureIndex; }
		set
		{
			_m_FeatureIndex = value;
			UpdateFeatureIndex();
		}
	}

	private void UpdateFeature()
	{
		if (m_feature != "") {
			gameObject.SetActive (true);
			if(m_feature.Length > 15) {
				string temp = m_feature.Substring (0, 15);
				m_FeatureText.text = temp + "...";
			} else {
				m_FeatureText.text = m_feature;
			}
		} else {
			gameObject.SetActive(false);
		}
	}

	private void UpdateFeatureIndex()
	{
		float featureIndex = (float)m_featureIndex;
		m_FeatureIndexText.text = featureIndex.ToString ("f2");
	}
}
