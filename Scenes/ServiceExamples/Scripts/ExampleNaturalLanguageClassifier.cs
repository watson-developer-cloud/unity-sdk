using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.NaturalLanguageClassifier.v1;

public class ExampleNaturalLanguageClassifier : MonoBehaviour
{
	private NaturalLanguageClassifier m_NaturalLanguageClassifier = new NaturalLanguageClassifier();
	private string m_ClassifierId = "3a84d1x62-nlc-768";
	private string m_InputString = "Is it hot outside?";

	void Start ()
	{
		Debug.Log("Input String: " + m_InputString);
		m_NaturalLanguageClassifier.Classify(m_ClassifierId, m_InputString, OnClassify);
	}

	private void OnClassify(ClassifyResult result)
	{
		if (result != null)
		{
			Debug.Log("Classify Result: " + result.top_class);
		}
	}
}
