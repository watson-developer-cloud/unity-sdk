using UnityEngine;
using System.Collections;

public class AvatarCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	//LeanTween.moveLocal(gameObject, '

		Vector3[] path = new Vector3[] {
			new Vector3 (0.999876f, 0f, -0.01570719f),
			new Vector3 (0.6364506f, 1.00301f, -0.00999476f),
			new Vector3 (1.002888f, 0.6365298f, -0.0157558f),
			new Vector3 (0f, 1f, 0f),
			new Vector3 (0f, 1f, 0f),
			new Vector3 (-1.002888f, 0.6365297f, 0.0157558f),
			new Vector3 (-0.6364506f, 1.00301f, 0.00999476f),
			new Vector3 (-0.999876f, 0f, 0.01570719f),
			new Vector3 (-0.999876f, 0f, 0.01570719f),
			new Vector3 (-0.6364506f, -1.003011f, 0.00999476f),
			new Vector3 (-1.002888f, -0.6365299f, 0.0157558f),
			new Vector3 (0f, -1f, 0f),
			new Vector3 (0f, -1f, 0f),
			new Vector3 (1.002888f, -0.6365299f, -0.0157558f),
			new Vector3 (0.6364506f, -1.00301f, -0.00999476f),
			new Vector3 (0.999876f, 0f, -0.01570719f)
		};

		LeanTween.moveLocal (gameObject, path, 1.0f).setLoopPingPong ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
