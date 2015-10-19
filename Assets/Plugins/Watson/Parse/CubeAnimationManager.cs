using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;

public class CubeAnimationManager : MonoBehaviour {
	public float smooth = 2f;
	public float statinoaryRotationSpeed = .01f;
	public Vector3  statinoaryRotationVector;

	public Camera mainCamera;
	public Camera parseViewCamera;

	private bool isRotating = true;

	private bool _isOpen = false;
	public bool isUnFolding {
		get { return _isOpen; }
		set
		{
			_isOpen = value;
		}
	}


	[Header("UI Faces")]
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

	[Header("Render Textures")]
	[SerializeField]
	private RenderTexture renderTex_0;
	[SerializeField]
	private RenderTexture renderTex_1;
	[SerializeField]
	private RenderTexture renderTex_2;
	[SerializeField]
	private RenderTexture renderTex_3;
	[SerializeField]
	private RenderTexture renderTex_4;
	[SerializeField]
	private RenderTexture renderTex_5;

	private Vector3 p0_position_open = new Vector3 (0f, 0f, 10f);
	private Quaternion p0_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p0_position_closed = new Vector3 (0f, 0f, 10f);
	private Quaternion p0_rotation_closed = Quaternion.Euler (0f, 90f, 0f);

	private Vector3 p1_position_open = new Vector3 (-5f + 2.5f, 0f, 5f);
	private Quaternion p1_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p1_position_closed = new Vector3 (-5f, 0f, 5f);
	private Quaternion p1_rotation_closed = Quaternion.Euler (0f, 90f, 0f);

	private Vector3 p2_position_open = new Vector3 (-5f + 2.5f, 0f, -5f);
	private Quaternion p2_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p2_position_closed = new Vector3 (-5f, 0f, -5f);
	private Quaternion p2_rotation_closed = Quaternion.Euler (0f, 0f, 0f);

	private Vector3 p3_position_open = new Vector3 (-5f + 2.5f, 0f, -5f);
	private Quaternion p3_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p3_position_closed = new Vector3 (-5f, 0f, -5f);
	private Quaternion p3_rotation_closed = Quaternion.Euler (0f, 270f, 0f);

	private Vector3 p4_position_open = new Vector3 (-5f + 2.5f, 5f, 0f);
	private Quaternion p4_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p4_position_closed = new Vector3 (-5f, 5f, 0f);
	private Quaternion p4_rotation_closed = Quaternion.Euler (0f, 0f, 270f);

	private Vector3 p5_position_open = new Vector3 (-5f + 2.5f, -5f, 0f);
	private Quaternion p5_rotation_open = Quaternion.Euler (0f, 0f, 0f);
	private Vector3 p5_position_closed = new Vector3 (-5f, -5f, 0f);
	private Quaternion p5_rotation_closed = Quaternion.Euler (0f, 0f, 90f);

	void Start ()
	{
		renderTex_0.useMipMap = true;
		renderTex_1.useMipMap = true;
		renderTex_2.useMipMap = true;
		renderTex_3.useMipMap = true;
		renderTex_4.useMipMap = true;
		renderTex_5.useMipMap = true;

		checkFoldingStatus ();
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {

			if(isUnFolding){
				Fold();
			}
			else{
				UnFold();
			}
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			isRotating = !isRotating;
		}
	
		CubeFoldingAnimation ();
		CubeStatinoaryAnimation ();
	}

	#region Public Functions for Fold / Unfold / Focus On Side / Defocus etc.
		
	public void Fold(){
		isUnFolding = false;

	}
	
	public void UnFold(){
		isUnFolding = true;
	}

	#endregion


	private void CubeStatinoaryAnimation(){
		if (isRotating && !isUnFolding) {	//If it is fold already and rotation is true we are rotating.
			transform.Rotate (statinoaryRotationVector * Time.deltaTime * statinoaryRotationSpeed);
		}
	}

	public bool m_isAlreadyUnfold = false;
	public bool m_isAlreadyFold = false;

	#region Folding - Unfolding Animations

	private void CubeFoldingAnimation(){

		if (isUnFolding && !m_isAlreadyUnfold) {	//UnFold the Cube
			CubeUnfoldOnUpdate ();
			CheckUnfoldOnUpdateRotation ();
			checkFoldingStatus ();
		}else if(isUnFolding && m_isAlreadyUnfold){
			CheckUnfoldOnUpdateRotation();
		} else if (!isUnFolding && !m_isAlreadyFold) {	//Fold the Cube
			CubeFoldOnUpdate();
			checkFoldingStatus();
		} else {
			// Do nothing - It is already unfold or aldready fold, no need to do more action!
		}
	}


	public void checkFoldingStatus(){
		m_isAlreadyUnfold = 
				//Checking the rotation
				Utility.CheckEqualityQuaternion(p0.transform.localRotation, p0_rotation_open) && 
				Utility.CheckEqualityQuaternion(p1.transform.localRotation, p1_rotation_open) &&
				Utility.CheckEqualityQuaternion(p2.transform.localRotation, p2_rotation_open) &&
				Utility.CheckEqualityQuaternion(p3.transform.localRotation, p3_rotation_open) &&
				Utility.CheckEqualityQuaternion(p4.transform.localRotation, p4_rotation_open) &&
				Utility.CheckEqualityQuaternion(p5.transform.localRotation, p5_rotation_open) &&
				//Checking the position
				p0.transform.localPosition == p0_position_open && 
				p1.transform.localPosition == p1_position_open &&
				p2.transform.localPosition == p2_position_open &&
				p3.transform.localPosition == p3_position_open &&
				p4.transform.localPosition == p4_position_open &&
				p5.transform.localPosition == p5_position_open ;

		m_isAlreadyFold = 
				//Checking the rotation
				Utility.CheckEqualityQuaternion(p0.transform.localRotation, p0_rotation_closed) && 
				Utility.CheckEqualityQuaternion(p1.transform.localRotation, p1_rotation_closed) &&
				Utility.CheckEqualityQuaternion(p2.transform.localRotation, p2_rotation_closed) &&
				Utility.CheckEqualityQuaternion(p3.transform.localRotation, p3_rotation_closed) &&
				Utility.CheckEqualityQuaternion(p4.transform.localRotation, p4_rotation_closed) &&
				Utility.CheckEqualityQuaternion(p5.transform.localRotation, p5_rotation_closed) &&
				//Checking the position
				p0.transform.localPosition == p0_position_closed && 
				p1.transform.localPosition == p1_position_closed &&
				p2.transform.localPosition == p2_position_closed &&
				p3.transform.localPosition == p3_position_closed &&
				p4.transform.localPosition == p4_position_closed &&
				p5.transform.localPosition == p5_position_closed ;
	}

	private void CubeUnfoldOnUpdate(){
		p0.transform.localRotation = Quaternion.Slerp (p0.transform.localRotation, p0_rotation_open, Time.deltaTime * smooth);
		p1.transform.localRotation = Quaternion.Slerp (p1.transform.localRotation, p1_rotation_open, Time.deltaTime * smooth);
		p2.transform.localRotation = Quaternion.Slerp (p2.transform.localRotation, p2_rotation_open, Time.deltaTime * smooth);
		p3.transform.localRotation = Quaternion.Slerp (p3.transform.localRotation, p3_rotation_open, Time.deltaTime * smooth);
		p4.transform.localRotation = Quaternion.Slerp (p4.transform.localRotation, p4_rotation_open, Time.deltaTime * smooth);
		p5.transform.localRotation = Quaternion.Slerp (p5.transform.localRotation, p5_rotation_open, Time.deltaTime * smooth);
		
		p0.transform.localPosition = Vector3.Lerp(p0.transform.localPosition, p0_position_open, Time.deltaTime * smooth);
		p1.transform.localPosition = Vector3.Lerp(p1.transform.localPosition, p1_position_open, Time.deltaTime * smooth);
		p2.transform.localPosition = Vector3.Lerp(p2.transform.localPosition, p2_position_open, Time.deltaTime * smooth);
		p3.transform.localPosition = Vector3.Lerp(p3.transform.localPosition, p3_position_open, Time.deltaTime * smooth);
		p4.transform.localPosition = Vector3.Lerp(p4.transform.localPosition, p4_position_open, Time.deltaTime * smooth);
		p5.transform.localPosition = Vector3.Lerp(p5.transform.localPosition, p5_position_open, Time.deltaTime * smooth);
	}

	private void CubeFoldOnUpdate(){
		p0.transform.localRotation = Quaternion.Slerp (p0.transform.localRotation, p0_rotation_closed, Time.deltaTime * smooth);
		p1.transform.localRotation = Quaternion.Slerp (p1.transform.localRotation, p1_rotation_closed, Time.deltaTime * smooth);
		p2.transform.localRotation = Quaternion.Slerp (p2.transform.localRotation, p2_rotation_closed, Time.deltaTime * smooth);
		p3.transform.localRotation = Quaternion.Slerp (p3.transform.localRotation, p3_rotation_closed, Time.deltaTime * smooth);
		p4.transform.localRotation = Quaternion.Slerp (p4.transform.localRotation, p4_rotation_closed, Time.deltaTime * smooth);
		p5.transform.localRotation = Quaternion.Slerp (p5.transform.localRotation, p5_rotation_closed, Time.deltaTime * smooth);
		
		p0.transform.localPosition = Vector3.Lerp(p0.transform.localPosition, p0_position_closed, Time.deltaTime * smooth);
		p1.transform.localPosition = Vector3.Lerp(p1.transform.localPosition, p1_position_closed, Time.deltaTime * smooth);
		p2.transform.localPosition = Vector3.Lerp(p2.transform.localPosition, p2_position_closed, Time.deltaTime * smooth);
		p3.transform.localPosition = Vector3.Lerp(p3.transform.localPosition, p3_position_closed, Time.deltaTime * smooth);
		p4.transform.localPosition = Vector3.Lerp(p4.transform.localPosition, p4_position_closed, Time.deltaTime * smooth);
		p5.transform.localPosition = Vector3.Lerp(p5.transform.localPosition, p5_position_closed, Time.deltaTime * smooth);

	}

	private void CheckUnfoldOnUpdateRotation(){
		transform.localRotation = Quaternion.Slerp (transform.localRotation, parseViewCamera.transform.rotation, Time.deltaTime * smooth);
		//LeanTween.rotateLocal (gameObject, parseViewCamera.transform.rotation.eulerAngles , smooth);
	}

	#endregion

	#region Focus

	#endregion

}
