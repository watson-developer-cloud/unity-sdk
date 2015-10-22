/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Dogukan Erenel
*/

using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

/// <summary>
/// Cube animation manager. All animations related with Cube located here.
/// </summary>
public class CubeAnimationManager : MonoBehaviour {

	#region Enumerations Related with Animation and Cube Sides

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
		FocusingToSide,
		FocusedToSide,
		GoingFromScene		//Animating just before destroying
	}

    #endregion

    #region Private and Public Members

    private static CubeAnimationManager _instance;
    public static CubeAnimationManager instance { get { return _instance; } }

    private CubeAnimationState m_currentState;

	/// <summary>
	/// Gets the state of the current animation.
	/// </summary>
	/// <value>The state of the current animation.</value>
	public CubeAnimationState currentAnimationState{
		get{
			return m_currentState;
		}
        set
        {
            bool hasStateChanged = (m_currentState != value);
            m_currentState = value;

            if (hasStateChanged){
                EventManager.Instance.SendEvent(EventManager.onCubeAnimationStateChanged);
            }

        }
	}

	public GameObject avatarGameobject;
	public Vector3 avatarPositionUnfold = new Vector3 (0, -9.5f, 0);
	public Vector3 positionCubeOffsetUnfold = Vector3.zero;

	public float statinoaryRotationSpeed = 10.0f;
	public Vector3  statinoaryRotationVector;
	public float distanceFromCameraInZAfterUnfolding = 100f;
	public Vector3 m_initialPosition;
	private Vector3 m_initialLocalScale;
    private Vector3 m_initialPositionMainCamera;

	private bool isRotating = true;

	public float timeForComingToScene = 1.0f;
	public LeanTweenType easeForComingToScene = LeanTweenType.easeOutElastic;
	public LeanTweenType easeForGoingFromScene = LeanTweenType.easeOutCirc;
	public float timeForFoldingUnfolding = 1.0f;
	public LeanTweenType easeForFolding = LeanTweenType.easeInOutCubic;
	public LeanTweenType easeForUnfolding = LeanTweenType.easeInOutCubic;
	public float timeForFocusing = 1.0f;
	public LeanTweenType easeForFocusing = LeanTweenType.easeInOutCubic;

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

	private Vector3[] positionFocus;

	private LTDescr[] animationPositionOnSide;
	private LTDescr[] animationRotationOnSide;
	private LTDescr animationRotationCube;
	private LTDescr animationPositionCube;

	private LTDescr[] animationPositionOnSidePresentation;

	private LTDescr animationAvatarPosition;
	//private LTDescr animationCubeScale;
	#endregion

	void Awake ()
	{
        _instance = this;

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

		positionFocus = new Vector3[]{
			new Vector3 (-21.0f, -0.6f, 	-2f), 	//Our Hero Side - Zoomed to the camera
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
			if(!renderTexSide[i].useMipMap)
				renderTexSide[i].useMipMap = true;
		}

		m_initialPosition = transform.position;
		m_initialLocalScale = transform.localScale;
        m_initialPositionMainCamera = Camera.main.transform.position;

        if (avatarGameobject == null) {
			avatarGameobject = GameObject.Find("Avatar/Avatar_01");
		}

		GameObject[] questionsCreated = GameObject.FindGameObjectsWithTag ("QuestionOnFocus");
		if (questionsCreated != null) {
			for (int i = 0; i < questionsCreated.Length; i++) {
				if(questionsCreated[i] != transform.parent.gameObject){
					CubeAnimationManager animationManager = questionsCreated[i].GetComponentInChildren<CubeAnimationManager>();
					if(animationManager != null){
						animationManager.DestroyCube();
					}
				}
			}
		}

		//ShowCube ();
	}

	private int currentZoom = 0;
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			Fold();
		}

		if (Input.GetKeyDown (KeyCode.U)) {
			UnFold();
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

		if (Input.GetKey (KeyCode.Alpha9)) {
			MakeZoom(0.9f);	//Zoom-in
		}

		if (Input.GetKey (KeyCode.Alpha0)) {
			MakeZoom(1.1f);	//Zoom-in
		}

	
		//CubeFoldingAnimation ();
		CubeStatinoaryAnimation ();
	}

	private void CubeStatinoaryAnimation(){
		if (isRotating && (currentAnimationState == CubeAnimationState.IdleOnScene || currentAnimationState == CubeAnimationState.Folding)) {	//If it is fold already and rotation is true we are rotating.
			transform.Rotate (statinoaryRotationVector * Time.deltaTime * statinoaryRotationSpeed, Space.World);
		}
	}

	#region Folding - Unfolding Animations
		
	public void Fold(){
		StopAllCubeAnimations ();

		if (currentAnimationState == CubeAnimationState.FocusingToSide || currentAnimationState == CubeAnimationState.FocusedToSide) {
			AnimateUnfocus (AnimateFold, null);
		}else if( currentAnimationState == CubeAnimationState.GoingFromScene){
			//do nothing - it is going from scene
		} else {
			AnimateFold (null, null);
		}
	}

	private void AnimateFold(System.Object paramOnComplete = null){
		AnimateFold (null, null);
	}

	private void AnimateFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null){
		currentAnimationState = CubeAnimationState.Folding;
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionFold[i], timeForFoldingUnfolding).setEase (easeForFolding);
			if(i == uiFaceOnSide.Length - 1)
				animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationFold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFolding).setOnComplete(
					()=>{
					currentAnimationState = CubeAnimationState.IdleOnScene;	//Folding finish

					if(callBackOnComplete != null)
						callBackOnComplete(paramOnComplete);
				});
			else
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationFold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFolding);
		}

		//Move object cloase to camera
		animationPositionCube = LeanTween.move (gameObject, m_initialPosition, timeForFoldingUnfolding).setEase (easeForFolding);

		//Avatar Object position change
		animationAvatarPosition = LeanTween.moveLocal (avatarGameobject, Vector3.zero, timeForFoldingUnfolding).setEase(easeForUnfolding);
	}
	
	public void UnFold(){
		StopAllCubeAnimations ();
		if (currentAnimationState == CubeAnimationState.FocusingToSide || currentAnimationState == CubeAnimationState.FocusedToSide) {
			AnimateUnfocus(AnimateUnFold, null);
		} else if( currentAnimationState == CubeAnimationState.GoingFromScene){
			//do nothing - it is going from scene
		}else {
			AnimateUnFold (null, null);
		}

	}

	private void AnimateUnFold(System.Object paramOnComplete = null){
		AnimateUnFold (null, null);
	}

	private void AnimateUnFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null){
		currentAnimationState = CubeAnimationState.UnFolding;
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionUnfold[i], timeForFoldingUnfolding).setEase (easeForUnfolding);
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationUnfold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForUnfolding);
		}
		
		//Rotate to camera 
		animationRotationCube = LeanTween.rotateLocal(gameObject, Camera.main.transform.rotation.eulerAngles - new Vector3(0.0f, 90.0f, 0.0f), timeForFoldingUnfolding).setEase(easeForUnfolding).setOnComplete(
			() => {
			currentAnimationState = CubeAnimationState.Unfolded;	//unfolding finish

			if(callBackOnComplete != null)
				callBackOnComplete(paramOnComplete);
		});
		
		//Move object cloase to camera
		animationPositionCube = LeanTween.move (gameObject, m_initialPositionMainCamera + distanceFromCameraInZAfterUnfolding * Camera.main.transform.forward + positionCubeOffsetUnfold, timeForFoldingUnfolding).setEase(easeForUnfolding);

		//Avatar Object position change
		animationAvatarPosition = LeanTween.moveLocal (avatarGameobject, avatarPositionUnfold, timeForFoldingUnfolding).setEase(easeForUnfolding);
	}

	#endregion


	#region Focus - UnFocus Animations

	/// <summary>
	/// Focuses the on side of the cube
	/// </summary>
	/// <param name="sideType">Side type.</param>
	public void FocusOnSide(CubeSideType sideType){
		if(presentationSide.Length < (int) sideType){
			Log.Error("CubeAnimationManager", "CubeAnimationManager - FocusOnSide {0} has wrong number as side. ", (int) sideType);
			return;
		}

		StopAllCubeAnimations ();

		if (currentAnimationState == CubeAnimationState.Unfolded || currentAnimationState == CubeAnimationState.FocusedToSide  || currentAnimationState == CubeAnimationState.FocusingToSide) {
			AnimateFocusOnSide(sideType);
		} else if( currentAnimationState == CubeAnimationState.GoingFromScene){
			//do nothing - it is going from scene
		}else {
			AnimateUnFold (AnimateFocusOnSide, (System.Object)sideType);
		}
	}

	private void AnimateFocusOnSide(System.Object sideType){
		AnimateFocusOnSide ((CubeSideType)sideType);
	}
	               
	private void AnimateFocusOnSide(CubeSideType sideType){
		currentAnimationState = CubeAnimationState.FocusingToSide;

		for (int i = 0; i < presentationSide.Length; i++) {
			Vector3 offsetPosition = presentationSide[i].transform.parent.localPosition + presentationSide[i].transform.parent.parent.localPosition;
			if(i == 0){
				offsetPosition += presentationSide[i].transform.parent.parent.parent.localPosition;
			}

			if(i == (int) sideType){	//Our hero object!
				animationPositionOnSidePresentation[i] = LeanTween.moveLocal (presentationSide[i], positionFocus[0] - offsetPosition, timeForFocusing).setEase (easeForFocusing).setOnComplete(()=>{
					currentAnimationState = CubeAnimationState.FocusedToSide;
				});
			}
			else if(i < (int) sideType){
//				Vector3[] splineVec = new Vector3[]{ presentationSide[i].transform.localPosition,presentationSide[i].transform.localPosition - Vector3.right * (i  * 2) * (i % 2 == 0 ? 1.0f : -1.0f), positionZoom[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition  - Vector3.right * (i  * 2) * (i % 2 == 0 ? 1.0f : -1.0f),positionZoom[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition};
				animationPositionOnSidePresentation[i] = LeanTween.moveLocal (presentationSide[i], positionFocus[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition, timeForFocusing).setEase (easeForFocusing);
			}
			else{
//				Vector3[] splineVec = new Vector3[]{presentationSide[i].transform.localPosition, presentationSide[i].transform.localPosition - Vector3.right * (i * 2) * (i % 2 == 0 ? 1.0f : -1.0f), positionZoom[i] - offsetPosition  - Vector3.right * (i * 2) * (i % 2 == 0 ? 1.0f : -1.0f),positionZoom[i] - offsetPosition};
				animationPositionOnSidePresentation[i] = LeanTween.moveLocal (presentationSide[i], positionFocus[i] - offsetPosition, timeForFocusing).setEase (easeForFocusing);
			}
		}

		//Make sure that our sides are in right rotation
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationUnfold[i].eulerAngles, timeForFocusing).setEase (easeForFocusing);
		}

	}

	/// <summary>
	/// Unfocus from the current focus and returning back to unfold state
	/// </summary>
	public void UnFocus(){
		if (currentAnimationState == CubeAnimationState.FocusedToSide || currentAnimationState == CubeAnimationState.FocusingToSide) {
			StopAllCubeAnimations ();
			AnimateUnfocus (null, null);
		}
	}

	private void AnimateUnfocus(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null){
		
		currentAnimationState = CubeAnimationState.FocusingToSide;	
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			if(i == uiFaceOnSide.Length - 1){
				animationPositionOnSide [i] = LeanTween.moveLocal (presentationSide [i], Vector3.zero, timeForFocusing).setEase (easeForFocusing).setOnComplete(()=>{
					currentAnimationState = CubeAnimationState.Unfolded;

					if(callBackOnComplete != null){
						callBackOnComplete(paramOnComplete);
					}
				});
			}
			else{
				animationPositionOnSide [i] = LeanTween.moveLocal (presentationSide [i], Vector3.zero, timeForFocusing).setEase (easeForFocusing);
			}
		}
	}

	#endregion

	#region Animation Stop / Reset

	private void StopAllCubeAnimations(){
		if( currentAnimationState == CubeAnimationState.GoingFromScene){
			//do nothing - it is going from scene, early return before any animation stopping!
			return;
		}

		StopFoldingAnimation ();
		StopAvatarAnimation ();
		StopFocusAnimation ();
		StopCubeCameraFacingRotationAnimation ();
	}

	private void StopAvatarAnimation(){
		if (animationAvatarPosition != null) {
			LeanTween.cancel(animationAvatarPosition.uniqueId);
		}
	}

	private void StopFoldingAnimation(){
		if (animationPositionOnSide == null) {
			animationPositionOnSide = new LTDescr[uiFaceOnSide.Length];
		} else {
			for (int i = 0; i < animationPositionOnSide.Length; i++) {
				if(animationPositionOnSide[i] != null)
					LeanTween.cancel(animationPositionOnSide[i].uniqueId);
			}
		}
		
		if (animationRotationOnSide == null) {
			animationRotationOnSide = new LTDescr[uiFaceOnSide.Length];
		} else {
			for (int i = 0; i < animationRotationOnSide.Length; i++) {
				if(animationRotationOnSide[i] != null)
					LeanTween.cancel(animationRotationOnSide[i].uniqueId);
			}
		}
	}

	private void StopFocusAnimation(){
		if (animationPositionOnSidePresentation == null) {
			animationPositionOnSidePresentation = new LTDescr[uiFaceOnSide.Length];
		} else {
			for (int i = 0; i < animationPositionOnSide.Length; i++) {
				if(animationPositionOnSidePresentation[i] != null)
					LeanTween.cancel(animationPositionOnSidePresentation[i].uniqueId);
			}
		}
	}


	private void StopCubeCameraFacingRotationAnimation(){
		if (animationRotationCube != null) {
			LeanTween.cancel(animationRotationCube.uniqueId);
		}
		
		if (animationPositionCube != null) {
			LeanTween.cancel (animationPositionCube.uniqueId);
		}
	}

    #endregion

    #region On Enable / Disable
    void OnEnable()
    {
        StopAllCubeAnimations();
        ShowCube();
    }

    void OnDisable()
    {
        StopAllCubeAnimations();
        AnimateDestroyingCube(false);
    }
    #endregion

    #region Focus

    public void ShowCube(){
		if( currentAnimationState == CubeAnimationState.GoingFromScene){
			//do nothing - it is going from scene, early return before any animation stopping!
			return;
		}

		StopAllCubeAnimations ();
		AnimateShowingCube ();
	}

	private void AnimateShowingCube(){
		currentAnimationState = CubeAnimationState.ComingToScene;
		LeanTween.moveLocal (gameObject, new Vector3 (0, 40, 0), timeForComingToScene).setFrom (new Vector3 (0, -40, 0)).setEase (easeForComingToScene);
		LeanTween.scale (gameObject, gameObject.transform.localScale, timeForComingToScene).setFrom (Vector3.one).setEase (easeForComingToScene).setOnComplete(()=>{
			currentAnimationState = CubeAnimationState.IdleOnScene;
		});
	}

	public void DestroyCube(){
		StopAllCubeAnimations ();
		AnimateDestroyingCube (true);
	}

	private void AnimateDestroyingCube(bool destroy){
		currentAnimationState = CubeAnimationState.GoingFromScene;
		LeanTween.moveLocal (gameObject, new Vector3 (0, 240, 0), timeForComingToScene).setEase (easeForGoingFromScene).setOnComplete(()=>{
            if(destroy)
			    Destroy(transform.parent.gameObject);
		});
	}


	#endregion

	#region Zoom 



	public void MakeZoom(float zoom){
		float zoomDiff = Mathf.Abs (1.0f - zoom);
		if (zoomDiff == 0.0f)
			zoomDiff = 0.0001f;

		LeanTween.scale (gameObject, transform.localScale * zoom, 1.0f * zoomDiff).setEase (LeanTweenType.linear);
	}

	#endregion

	void OnDestroy()
	{
		for (int i = 0; i < renderTexSide.Length; i++) {
			renderTexSide[i].Release();
		}
	}
}
