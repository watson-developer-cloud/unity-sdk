using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof (Text))]
public class QualityManager : MonoBehaviour {

	string[] qualityNames = null;
	private int currentQuality = 0;
	private Text m_Text;
	const string display = "Quality: {0}";
	// Use this for initialization
	void Start () {
		currentQuality = QualitySettings.GetQualityLevel ();
		qualityNames = QualitySettings.names;
		m_Text = GetComponent<Text>();
		m_Text.text = string.Format(display, qualityNames[currentQuality]);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.A)){
			currentQuality = (currentQuality + 1) % qualityNames.Length;
			QualitySettings.SetQualityLevel(currentQuality, true);
			m_Text.text = string.Format(display, qualityNames[currentQuality]);
		}
	}
}
