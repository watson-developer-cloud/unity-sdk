using UnityEngine;
using System.Collections;

public class CubeRotate : MonoBehaviour {
	public float speed = .01f;
	private bool isRotating = true;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			isRotating = !isRotating;
		}

		if (isRotating) {
			transform.Rotate (Vector3.up * Time.deltaTime * speed);
		}
	}
}
