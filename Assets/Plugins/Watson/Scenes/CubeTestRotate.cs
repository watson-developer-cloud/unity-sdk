using UnityEngine;
using System.Collections;

public class CubeTestRotate : MonoBehaviour {
	public float speed = .01f;
	private bool isRotating = true;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			isRotating = !isRotating;

//		if (isRotating)
//			transform.RotateAround (Vector3.zero, Vector3.up, speed);
	}
}
