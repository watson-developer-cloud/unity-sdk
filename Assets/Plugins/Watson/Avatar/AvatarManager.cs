using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

public class AvatarManager : MonoBehaviour {

	public static AvatarManager Instance;

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

	void Awake(){
		Instance = this;
	}
	// Use this for initialization
	void Start () {

	}

	public float timeLimit = 1.0f;
	public float modifier = 1.0f;
	public bool test = false;

	// Update is called once per frame
	void Update () {
		if (test) {
			test = false;
			float data = (Mathf.PingPong (Time.time, timeLimit) / timeLimit);
			SetAudioData (data * modifier);
		}
	}

	public static void SetAudioData(float value){
		Instance.pebbleManager.SetAudioData (value);
	}
}
