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

	public enum CubeAnimationState{
		NotPresent = 0, 	//Cube is not created OR not visible on the scene
		ComingToScene,
		IdleOnScene,
		UnFolding,
		Unfolded,
		Folding,
		ZoomingToSide,
		ZoomedToSide
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

	[Header("UI Faces for Zoom")]
	[SerializeField]
	private GameObject[] presentationSide;

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
			new Vector3 (-17.5f, 1f, 	-2f), 	//Our Hero Side - Zoomed to the camera
			new Vector3 (17.5f, 2.5f, 	25.0f),	//XRay Logo on right most 
			new Vector3 (17.5f, 10f, 	11.5f),	//Top - backup
			new Vector3 (17.5f, 2.5f, 	16f),	//Between XRay logo and hero
			new Vector3 (17.5f, -5f, 	11.5f),	//Bottom close to hero
			new Vector3 (17.5f, -5f, 	20.5f) 	//Bottom close to logo
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

		ShowCube ();
	}

	private int currentZoom = 0;
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
			FocusOnSide((CubeSideType) currentZoom );
			currentZoom = (currentZoom + 1) % uiFaceOnSide.Length;
		}

		if(Input.GetKeyDown (KeyCode.X)){
			UnFocus();
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

	#region Folding - Unfolding Animations
		
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
		animationRotationCube = LeanTween.rotateLocal(gameObject, parseViewCamera.transform.rotation.eulerAngles - new Vector3(0.0f, 90.0f, 0.0f), timeForFoldingUnfolding).setEase(LeanTweenType.easeInOutCubic);

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


	#region Focus - UnFocus Animations

	public void FocusOnSide(CubeSideType sideType){
		if(presentationSide.Length < (int) sideType){
			Log.Error("CubeAnimationManager", "CubeAnimationManager - FocusOnSide {0} has wrong number as side. ", (int) sideType);
			return;
		}

		CubeAnimationStop ();


		for (int i = 0; i < presentationSide.Length; i++) {

			Vector3 offsetPosition = presentationSide[i].transform.parent.localPosition + presentationSide[i].transform.parent.parent.localPosition;
			if(i == 0){
				offsetPosition += presentationSide[i].transform.parent.parent.parent.localPosition;
			}

			if(i == (int) sideType){	//Our hero object!
				//Our Hero object
				animationPositionOnSide[i] = LeanTween.moveLocal (presentationSide[i], positionZoom[0] - offsetPosition, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
			}
			else if(i < (int) sideType){
				animationPositionOnSide[i] = LeanTween.moveLocal (presentationSide[i], positionZoom[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
			}
			else{
				animationPositionOnSide[i] = LeanTween.moveLocal (presentationSide[i], positionZoom[i] - offsetPosition, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
			}

		}

		//Make sure that our sides are in right rotation
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationUnfold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
		}

	}

	public void UnFocus(){

		CubeAnimationStop ();

		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (presentationSide[i], Vector3.zero, timeForFoldingUnfolding).setEase (easeForFoldungUnfolding);
		}

	}

	#endregion

	#region Focus

	public void ShowCube(){
		LeanTween.moveLocal (gameObject, new Vector3 (0, 40, 0), 2.0f).setFrom (new Vector3 (0, -40, 0)).setEase (LeanTweenType.easeSpring);
		LeanTween.scale (gameObject, Vector3.one * 3.5f, 2.0f).setFrom (Vector3.one);
	}


	#endregion

}
