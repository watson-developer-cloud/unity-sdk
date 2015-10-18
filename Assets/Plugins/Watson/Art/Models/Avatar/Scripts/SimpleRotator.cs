using UnityEngine;
using System.Collections;

public class SimpleRotator : MonoBehaviour {

	public Vector3 rotationVector;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (rotationVector);
	}
}
