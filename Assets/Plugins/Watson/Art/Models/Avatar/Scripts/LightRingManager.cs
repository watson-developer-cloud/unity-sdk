using UnityEngine;
using System.Collections;

public class LightRingManager : MonoBehaviour {

	public GameObject lightFlare;
	// Use this for initialization
	void Start () {
		ChangeToColor (Color.red);
		AnimateLightFlare ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ChangeToColor(Color color){
		LeanTween.color (gameObject, color, 1.0f).setLoopPingPong();
	}

	void AnimateLightFlare(){

		LeanTween.moveLocal (lightFlare, new LTBezierPath (new Vector3[]{ new Vector3(28.365f,-6.06f,-16.385f), new Vector3(22.58f,-6.06f,32.78f), new Vector3(41.7f,-6.06f,8.4f), new Vector3(0.062f,-6.06f,32.764f) }), 1.0f).setOrientToPath(true).setAxis(Vector3.forward).setLoopPingPong ();

		
	}
}
