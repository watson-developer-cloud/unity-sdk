using UnityEngine;
using System.Collections;

public class animationTest : MonoBehaviour {

	public GameObject gameObjectToAnimate;
	public Vector3[] bezierPathPointsToCenter;
	public Vector3[] bezierPathPointsToStack;
	public Vector3[] bezierPathOrientationToCenter;
	public Vector3[] bezierPathOrientationToStack;
	public float ratio;
	public bool play = true;
	public LTBezierPath bezierPath;
	public LTBezierPath bezierPathOrientation;
	public float timeLength = 1.0f;
	// Use this for initialization
	void Start () {
	
	}
	

	// Update is called once per frame
	void Update () {
		if(play)
			ratio = Mathf.PingPong (Time.time, timeLength) / timeLength;

		float ratioToUse = 0.0f;
		if (ratio <= 0.5f) {
			ratioToUse = ratio * 2.0f;
			bezierPath = new LTBezierPath (bezierPathPointsToCenter);
			bezierPathOrientation = new LTBezierPath (bezierPathOrientationToCenter);
		} else {
			ratioToUse = (ratio - 0.5f) * 2.0f;
			bezierPath = new LTBezierPath (bezierPathPointsToStack);
			bezierPathOrientation = new LTBezierPath (bezierPathOrientationToStack);
		}

		gameObjectToAnimate.transform.localPosition = bezierPath.point (ratioToUse);
		gameObjectToAnimate.transform.localRotation = Quaternion.Euler( bezierPathOrientation.point (ratioToUse));
		//bezierPath.placeLocal (gameObjectToAnimate.transform, ratio);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.green;
		if(bezierPath != null)
			bezierPath.gizmoDraw (0.5f);
		//Gizmos.dra
	}
}
