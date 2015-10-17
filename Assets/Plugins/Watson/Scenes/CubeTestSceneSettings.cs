using UnityEngine;
using System.Collections;

public class CubeTestSceneSettings : MonoBehaviour {
	public float smooth = 2f;
	private bool _isOpen = true;
	public bool isOpen {
		get { return _isOpen; }
		set
		{
			_isOpen = value;
//			if(isOpen) 
//			{
//				openCube();
//			} else {
//				closeCube();
//			}
		}
	}

	[SerializeField]
	private GameObject p0;
	[SerializeField]
	private GameObject p1;
	[SerializeField]
	private GameObject p2;
	[SerializeField]
	private GameObject p3;
	[SerializeField]
	private GameObject p4;
	[SerializeField]
	private GameObject p5;

	private Vector3 p0_position_open = new Vector3 (0f, 0f, 0f);
	private Quaternion p0_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p0_position_closed = new Vector3 (10f, 0f, -10f);
	private Quaternion p0_rotation_closed = Quaternion.Euler (0f, -180f, 0f);

	private Vector3 p1_position_open = new Vector3 (0f, 0f, -10f);
	private Quaternion p1_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p1_position_closed = new Vector3 (0f, 0f, -10f);
	private Quaternion p1_rotation_closed = Quaternion.Euler (0f, 90f, 0f);

	private Vector3 p2_position_open = new Vector3 (0f, 0f, -20f);
	private Quaternion p2_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p2_position_closed = new Vector3 (0f, 0f, -20f);
	private Quaternion p2_rotation_closed = Quaternion.Euler (0f, 0f, 0f);

	private Vector3 p3_position_open = new Vector3 (0f, 0f, -20f);
	private Quaternion p3_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p3_position_closed = new Vector3 (0f, 0f, -20f);
	private Quaternion p3_rotation_closed = Quaternion.Euler (0f, 270f, 0f);

	private Vector3 p4_position_open = new Vector3 (0f, 5f, -15f);
	private Quaternion p4_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p4_position_closed = new Vector3 (0f, 5f, -15f);
	private Quaternion p4_rotation_closed = Quaternion.Euler (0f, 0f, 270f);

	private Vector3 p5_position_open = new Vector3 (0f, -5f, -15f);
	private Quaternion p5_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p5_position_closed = new Vector3 (0f, -5f, -15f);
	private Quaternion p5_rotation_closed = Quaternion.Euler (0f, 0f, 90f);

	void Start ()
	{
//		QualitySettings.antiAliasing = 16;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			isOpen = !isOpen;
		}

//		if (isOpen) {
//			p0.transform.rotation = Quaternion.Slerp (p0.transform.rotation, p0_open, Time.deltaTime * smooth);
//		} else {
//
//		}

		p0.transform.localRotation = (isOpen) ? p0.transform.localRotation = Quaternion.Slerp (p0.transform.localRotation, p0_rotation_open, Time.deltaTime * smooth) : p0.transform.localRotation = Quaternion.Slerp (p0.transform.localRotation, p0_rotation_closed, Time.deltaTime * smooth);
		p1.transform.localRotation = (isOpen) ? p1.transform.localRotation = Quaternion.Slerp (p1.transform.localRotation, p1_rotation_open, Time.deltaTime * smooth) : p1.transform.localRotation = Quaternion.Slerp (p1.transform.localRotation, p1_rotation_closed, Time.deltaTime * smooth);
		p2.transform.localRotation = (isOpen) ? p2.transform.localRotation = Quaternion.Slerp (p2.transform.localRotation, p2_rotation_open, Time.deltaTime * smooth) : p2.transform.localRotation = Quaternion.Slerp (p2.transform.localRotation, p2_rotation_closed, Time.deltaTime * smooth);
		p3.transform.localRotation = (isOpen) ? p3.transform.localRotation = Quaternion.Slerp (p3.transform.localRotation, p3_rotation_open, Time.deltaTime * smooth) : p3.transform.localRotation = Quaternion.Slerp (p3.transform.localRotation, p3_rotation_closed, Time.deltaTime * smooth);
		p4.transform.localRotation = (isOpen) ? p4.transform.localRotation = Quaternion.Slerp (p4.transform.localRotation, p4_rotation_open, Time.deltaTime * smooth) : p4.transform.localRotation = Quaternion.Slerp (p4.transform.localRotation, p4_rotation_closed, Time.deltaTime * smooth);
		p5.transform.localRotation = (isOpen) ? p5.transform.localRotation = Quaternion.Slerp (p5.transform.localRotation, p5_rotation_open, Time.deltaTime * smooth) : p5.transform.localRotation = Quaternion.Slerp (p5.transform.localRotation, p5_rotation_closed, Time.deltaTime * smooth);

//		p0.transform.localPosition = (isOpen) ? p0.transform.localPosition = Quaternion.Slerp (p0.transform.localPosition, p0_position_open, Time.deltaTime * smooth) : p0.transform.localPosition = Quaternion.Slerp (p0.transform.localPosition, p0_position_closed, Time.deltaTime * smooth);
//		p1.transform.localPosition = (isOpen) ? p1.transform.localPosition = Quaternion.Slerp (p1.transform.localPosition, p1_position_open, Time.deltaTime * smooth) : p1.transform.localPosition = Quaternion.Slerp (p1.transform.localPosition, p1_position_closed, Time.deltaTime * smooth);
//		p2.transform.localPosition = (isOpen) ? p2.transform.localPosition = Quaternion.Slerp (p2.transform.localPosition, p2_position_open, Time.deltaTime * smooth) : p2.transform.localPosition = Quaternion.Slerp (p2.transform.localPosition, p2_position_closed, Time.deltaTime * smooth);
//		p3.transform.localPosition = (isOpen) ? p3.transform.localPosition = Quaternion.Slerp (p3.transform.localPosition, p3_position_open, Time.deltaTime * smooth) : p3.transform.localPosition = Quaternion.Slerp (p3.transform.localPosition, p3_position_closed, Time.deltaTime * smooth);
//		p4.transform.localPosition = (isOpen) ? p4.transform.localPosition = Quaternion.Slerp (p4.transform.localPosition, p4_position_open, Time.deltaTime * smooth) : p4.transform.localPosition = Quaternion.Slerp (p4.transform.localPosition, p4_position_closed, Time.deltaTime * smooth);
//		p5.transform.localPosition = (isOpen) ? p5.transform.localPositio`n = Quaternion.Slerp (p5.transform.localPosition, p5_position_open, Time.deltaTime * smooth) : p5.transform.localPosition = Quaternion.Slerp (p5.transform.localPosition, p5_position_closed, Time.deltaTime * smooth);

		p0.transform.localPosition = (isOpen) ? Vector3.Lerp(p0.transform.localPosition, p0_position_open, Time.deltaTime * smooth) : Vector3.Lerp(p0.transform.localPosition, p0_position_closed, Time.deltaTime * smooth);
	}

//	private void openCube()
//	{
//		Debug.Log ("open \tcube");
//	}
//
//	private void closeCube()
//	{
//		Debug.Log ("close cube");
//	}
}
