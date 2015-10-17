using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;


public class AvatarWidget : Widget {

	public static AvatarWidget Instance;

	private PebbleManager m_pebbleManager;
	public PebbleManager pebbleManager{
		get{
			m_pebbleManager = transform.GetComponentInChildren<PebbleManager> ();
			if (m_pebbleManager == null) {
				Log.Error("AvatarManager", "PebbleManager couldn't found!");
			}

			return m_pebbleManager;
		}
	}

	public float modifier = 1.0f;

	void Awake(){
		Instance = this;
	}
	// Use this for initialization
	void Start () {
		microphoneWidget.Active = true;
	}

#if UNITY_EDITOR
	public float timeLimit = 1.0f;
	public bool test = false;
	
	// Update is called once per frame
	void Update () {
		if (test) {
			test = false;
			float data = (Mathf.PingPong (Time.time, timeLimit) / timeLimit);
			SetAudioData (data);
		}
	}
#endif


	public static void SetAudioData(float value){
		Instance.pebbleManager.SetAudioData (value);
	}

	public Input m_levelInput = new Input("LevelInput", typeof(FloatData), "SetAudioFloatData");

	public void SetAudioFloatData(Widget.Data levelInputFloatData){
		FloatData levelInput = (FloatData)levelInputFloatData;
		SetAudioData(levelInput.Float * modifier);
		Debug.Log ("SetAudioFloatData: " + levelInput.Float);
	}

	protected override string GetName ()
	{
		return "Avatar";
	}

	public MicrophoneWidget microphoneWidget;


}
