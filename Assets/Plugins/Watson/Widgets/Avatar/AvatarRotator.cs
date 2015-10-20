using UnityEngine;
using System.Collections;

public class AvatarRotator : MonoBehaviour {

	public Vector3 rotationVector;
	public float speed = .01f;
	private bool isRotating = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isRotating) {
			transform.Rotate (rotationVector * Time.deltaTime * speed);
		}
	}
}
