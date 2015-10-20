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

	private float _m_FeatureIndex;
	public float m_featureIndex
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
		m_FeatureText.text = m_feature;
	}

	private void UpdateFeatureIndex()
	{
		float featureIndex = (float)m_featureIndex;
		m_FeatureIndexText.text = featureIndex.ToString ("f1");
	}
}
