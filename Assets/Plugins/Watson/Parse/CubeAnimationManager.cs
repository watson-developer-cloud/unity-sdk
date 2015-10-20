using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

public class CubeAnimationManager : MonoBehaviour {

	public enum CubeSideType
	{
		Title = 0,
		Answers,
		Parse,
		Evidence,
		Location,
		Chat
	}

	public float statinoaryRotationSpeed = 10.0f;
	public Vector3  statinoaryRotationVector;
	public float distanceFromCameraInZAfterUnfolding = 100f;
	public Vector3 m_initialPosition;

	public Camera mainCamera;
	public Camera parseViewCamera;

	private bool isRotating = true;

	public float timeForFoldingUnfolding = 2.0f;
	public LeanTweenType easeForFoldungUnfolding = LeanTweenType.easeInOutCubic;

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
	private GameObject[] uiFaceOnSide;

	[Header("Render Textures")]
	[SerializeField]
	private RenderTexture[] renderTexSide;

	private Vector3[] positionUnfold;
	private Quaternion[] rotationUnfold;
	private Vector3[] positionFold;
	private Quaternion[] rotationFold;

	private Vector3[] positionZoom;

	private LTDescr[] animationPositionOnSide;
	private LTDescr[] animationRotationOnSide;
	private LTDescr animationRotationCube;
	private LTDescr animationPositionCube;

	void Awake ()
	{
		positionUnfold = new Vector3[]{
			new Vector3 (0f, 0f, 10f), 
			new Vector3 (-5f + 2.5f, 0f, 5f), 
			new Vector3 (-5f + 2.5f, 0f, -5f), 
			new Vector3 (-5f + 2.5f, 0f, -5f), 
			new Vector3 (-5f + 2.5f, 5f, 0f), 
			new Vector3 (-5f + 2.5f, -5f, 0f)};

		rotationUnfold = new Quaternion[]{
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 0f, 0f)
		};

		positionFold = new Vector3[]{
			new Vector3 (0f, 0f, 10f),
			new Vector3 (-5f, 0f, 5f),
			new Vector3 (-5f, 0f, -5f),
			new Vector3 (-5f, 0f, -5f),
			new Vector3 (-5f, 5f, 0f),
			new Vector3 (-5f, -5f, 0f)
		};

		rotationFold = new Quaternion[]{
			Quaternion.Euler (0f, 90f, 0f),
			Quaternion.Euler (0f, 90f, 0f),
			Quaternion.Euler (0f, 0f, 0f),
			Quaternion.Euler (0f, 270f, 0f),
			Quaternion.Euler (0f, 0f, 270f),
			Quaternion.Euler (0f, 0f, 90f)
		};

		positionZoom = new Vector3[]{
			new Vector3 (20f, 0f, 22.0f),	//XRay Logo on right most 
			new Vector3 (-20f, 0f, 3.15f),	//Our Hero Side
			new Vector3 (20f, 2.5f, 14f),
			new Vector3 (20f, 0f, 13.5f),
			new Vector3 (20f, -2.5f, 14f),
			new Vector3 (20f, -2.5f, 17.5f)
		};


		if (uiFaceOnSide.Length != renderTexSide.Length 
			|| uiFaceOnSide.Length != positionUnfold.Length 
			|| uiFaceOnSide.Length != rotationUnfold.Length 
			|| uiFaceOnSide.Length != positionFold.Length 
			|| uiFaceOnSide.Length != rotationFold.Length) {

			Log.Error("CubeAnimationManager", "Cube Animation has some missing variables!");
		}

		for (int i = 0; i < renderTexSide.Length; i++) {
			renderTexSide[i].useMipMap = true;
		}

		m_initialPosition = transform.position;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			isUnFolding = !isUnFolding;

			if(isUnFolding){
				UnFold();
			}
			else{
				Fold();
			}
		}

		if (Input.GetKeyDown (KeyCode.Z)) {

			//FocusOnSide();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			isRotating = !isRotating;
		}
	
		//CubeFoldingAnimation ();
		CubeStatinoaryAnimation ();
	}

	private void CubeStatinoaryAnimation(){
		if (isRotating && !isUnFolding) {	//If it is fold already and rotation is true we are rotating.
			transform.Rotate (statinoaryRotationVector * Time.deltaTime * statinoaryRotationSpeed);
		}
	}

	#region Public Functions for Fold / Unfold / Focus On Side / Defocus etc.
		
	public void Fold(){

		CubeAnimationStop ();

		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionFold[i], timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationFold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
		}

		//Move object cloase to camera
		animationPositionCube = LeanTween.move (gameObject, m_initialPosition, timeForFoldingUnfolding);
	}
	
	public void UnFold(){
		CubeAnimationStop ();
		
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionUnfold[i], timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationUnfold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
		}

		//Rotate to camera 
		animationRotationCube = LeanTween.rotateLocal(gameObject, parseViewCamera.transform.rotation.eulerAngles, timeForFoldingUnfolding).setEase(LeanTweenType.easeInOutCubic);

		//Move object cloase to camera
		animationPositionCube = LeanTween.move (gameObject, parseViewCamera.transform.position + distanceFromCameraInZAfterUnfolding * parseViewCamera.transform.forward, timeForFoldingUnfolding).setEase(LeanTweenType.easeInOutCubic);
	}

	private void CubeAnimationStop(){
		if (animationPositionOnSide == null) {
			animationPositionOnSide = new LTDescr[uiFaceOnSide.Length];
		} else {
			for (int i = 0; i < animationPositionOnSide.Length; i++) {
				LeanTween.cancel(animationPositionOnSide[i].uniqueId);
			}
		}
		
		if (animationRotationOnSide == null) {
			animationRotationOnSide = new LTDescr[uiFaceOnSide.Length];
		} else {
			for (int i = 0; i < animationRotationOnSide.Length; i++) {
				LeanTween.cancel(animationRotationOnSide[i].uniqueId);
			}
		}

		if (animationRotationCube != null) {
			LeanTween.cancel(animationRotationCube.uniqueId);
		}

		if (animationPositionCube != null) {
			LeanTween.cancel (animationPositionCube.uniqueId);
		}
	}

	#endregion


	#region Folding - Unfolding Animations

	public void FocusOnSide(CubeSideType sideType){
		if(uiFaceOnSide.Length < (int) sideType){
			Log.Error("CubeAnimationManager", "CubeAnimationManager - FocusOnSide {0} has wrong number as side. ", (int) sideType);
			return;
		}



	}

	#endregion

	#region Focus

	#endregion

}
