using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Utilities;

[RequireComponent(typeof (Text))]
public class SettingsBehavior : MonoBehaviour {

	BehaviorType[] behaviorTypeList = null;
	private int currentBehavior = 0;
	private Text m_Text;
	const string display = "Behavior: {0}";
	// Use this for initialization
	void Start () {
		currentBehavior = (int)BehaviorManager.Instance.currentBehavior;
		behaviorTypeList = BehaviorManager.Instance.behaviorTypeList;
		m_Text = GetComponent<Text>();
		m_Text.text = string.Format(display, behaviorTypeList[currentBehavior].ToString());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.B)){
			currentBehavior = (currentBehavior + 1) % behaviorTypeList.Length;
			EventManager.Instance.SendEvent(EventManager.onBehaviorChange, currentBehavior);
			m_Text.text = string.Format(display, behaviorTypeList[currentBehavior].ToString());
		}
	}
}
