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
using IBM.Watson.Widgets.Avatar;

/// <summary>
/// Cube animation manager. All animations related with Cube located here.
/// </summary>
public class CubeAnimationManager : MonoBehaviour {

	#region Enumerations Related with Animation and Cube Sides

	public enum CubeSideType
	{
		TITLE = 0,
		ANSWERS,
		PARSE,
		EVIDENCE,
		LOCATION,
		CHAT
	}

	public enum CubeAnimationState{
		NOT_PRESENT = 0, 	//Cube is not created OR not visible on the scene
		COMING_TO_SCENE,
		IDLE_AS_FOLDED,
		UNFOLDING,
		IDLE_AS_UNFOLDED,
		FOLDING,
		FOCUSING_TO_SIDE,
		IDLE_AS_FOCUSED,
		GOING_FROM_SCENE		//Animating just before destroying
	}

    #endregion

    #region Private and Public Members

    private static CubeAnimationManager sm_instance;
	/// <summary>
	/// Gets the instance CubeAnimationManager
	/// </summary>
	/// <value>The instance.</value>
    public static CubeAnimationManager Instance { get { return sm_instance; } }

    private CubeAnimationState m_currentState;

	/// <summary>
	/// Gets the state of the current animation.
	/// </summary>
	/// <value>The state of the current animation.</value>
	public CubeAnimationState AnimationState{
		get{
			return m_currentState;
		}
        set
        {
            bool hasStateChanged = (m_currentState != value);
            m_currentState = value;

            if (hasStateChanged){
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION);
            }

        }
	}

	private CubeSideType m_LastCubeSideFocused = CubeSideType.TITLE;

	public GameObject avatarGameobject;
	public Vector3 avatarPositionUnfold = new Vector3 (0, -9.5f, 0);
	public Vector3 positionCubeOffsetUnfold = Vector3.zero;

	public float statinoaryRotationSpeed = 10.0f;
    private float m_InitialStationaryRotationSpeed;
    private float m_StationaryRotationBackupSpeed = 5.0f;
    public Vector3  statinoaryRotationVector;
	public float distanceFromCameraInZAfterUnfolding = 100f;
	public Vector3 m_initialPosition;
	private Vector3 m_initialLocalScale;
    private Quaternion m_initialLocalRotation;
    private Vector3 m_initialPositionMainCamera;

    private float m_OneFingerRotationModifier = 100.0f;
    private float m_OneFingerRotationAnimationSpeed = 5.0f;
    private Quaternion m_OneFingerCubeRotation;

    private bool m_isRotating = true;

	public float timeForComingToScene = 1.0f;
	private float timeForLeavingTheScene = 5.0f;
	public LeanTweenType easeForComingToScene = LeanTweenType.easeOutElastic;
	public LeanTweenType easeForGoingFromScene = LeanTweenType.easeOutCirc;
	public float timeForFoldingUnfolding = 1.0f;
	public LeanTweenType easeForFolding = LeanTweenType.easeInOutCubic;
	public LeanTweenType easeForUnfolding = LeanTweenType.easeInOutCubic;
	public float timeForFocusing = 1.0f;
	public LeanTweenType easeForFocusing = LeanTweenType.easeInOutCubic;
    public LeanTweenType easeForUnfocusing = LeanTweenType.easeInOutCubic;

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
        sm_instance = this;

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
        m_InitialStationaryRotationSpeed = statinoaryRotationSpeed;

        m_initialPositionMainCamera = new Vector3(0, 17.5f, -135);
        m_initialLocalRotation = transform.localRotation;

        if (avatarGameobject == null) {
			AvatarWidget avatarWidget = GameObject.FindObjectOfType<AvatarWidget>();
			if(avatarWidget != null){
				avatarGameobject = Utility.FindObject(avatarWidget.gameObject, "Avatar_01");
			}
		}

		SetInitialConditions();

//		GameObject[] questionsCreated = GameObject.FindGameObjectsWithTag ("QuestionOnFocus");
//		if (questionsCreated != null) {
//			for (int i = 0; i < questionsCreated.Length; i++) {
//				if(questionsCreated[i] != transform.parent.gameObject){
//					CubeAnimationManager animationManager = questionsCreated[i].GetComponentInChildren<CubeAnimationManager>();
//					if(animationManager != null){
//						animationManager.DestroyCube();
//					}
//				}
//			}
//		}

		//SetVisible (false);

		//RotateCube ();
		//ShowCube ();
	}

	void SetVisible(bool isVisible){
		MeshRenderer[] meshRenderer = transform.parent.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer item in meshRenderer) {
			item.enabled = isVisible;
		}
	}

	void Update()
	{
        CubeOneFingerDragAnimationOnUpdate();
        CubeStatinoaryAnimation();
    }

	private void CubeStatinoaryAnimation(){
		if (m_isRotating && (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING)) {    //If it is fold already and rotation is true we are rotating.

            if((Time.frameCount - m_LastFrameOneFingerDrag) > 1)
            {
                statinoaryRotationSpeed = Mathf.Lerp(statinoaryRotationSpeed, m_InitialStationaryRotationSpeed, Time.deltaTime * m_StationaryRotationBackupSpeed);
                transform.Rotate(statinoaryRotationVector * Time.deltaTime * statinoaryRotationSpeed, Space.World);
            }
                
		}
	}


	private void RotateCube(){
		LeanTween.value (gameObject, 0.0f, 1.0f, 1.0f).setLoopType (LeanTweenType.linear).setOnUpdate ((float f) => {
			Debug.Log("RotateCube = " + f);
		});

	}

	public void RotateOrPause(){
		m_isRotating = !m_isRotating;
	}

	#region Folding - Unfolding Animations
		
	public void Fold(){
		StopAllCubeAnimations ();

		if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED) {
			AnimateUnfocus (AnimateFold, null);
		}else if( AnimationState == CubeAnimationState.GOING_FROM_SCENE){
			//do nothing - it is going from scene
		} else {
			AnimateFold (null, null);
		}
	}

	private void AnimateFold(System.Object paramOnComplete){
		AnimateFold (null, null);
	}

	private void AnimateFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null){
		AnimationState = CubeAnimationState.FOLDING;
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionFold[i], timeForFoldingUnfolding).setEase (easeForFolding);
			if(i == uiFaceOnSide.Length - 1)
				animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationFold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForFolding).setOnComplete(
					()=>{
					AnimationState = CubeAnimationState.IDLE_AS_FOLDED;	//Folding finish

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
		if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED) {
			AnimateUnfocus(AnimateUnFold, null);
		} else if( AnimationState == CubeAnimationState.GOING_FROM_SCENE){
			//do nothing - it is going from scene
		}else {
			AnimateUnFold (null, null);
		}

	}

	private void AnimateUnFold(System.Object paramOnComplete){
		AnimateUnFold (null, null);
	}

	private void AnimateUnFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null){
		AnimationState = CubeAnimationState.UNFOLDING;
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			animationPositionOnSide[i] = LeanTween.moveLocal (uiFaceOnSide[i], positionUnfold[i], timeForFoldingUnfolding).setEase (easeForUnfolding);
			animationRotationOnSide[i] = LeanTween.rotateLocal (uiFaceOnSide[i], rotationUnfold[i].eulerAngles, timeForFoldingUnfolding).setEase (easeForUnfolding);
		}
		
		//Rotate to camera 
		animationRotationCube = LeanTween.rotateLocal(gameObject, Camera.main.transform.rotation.eulerAngles - new Vector3(0.0f, 90.0f, 0.0f), timeForFoldingUnfolding).setEase(easeForUnfolding).setOnComplete(
			() => {
			AnimationState = CubeAnimationState.IDLE_AS_UNFOLDED;	//unfolding finish

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
		m_LastCubeSideFocused = sideType;
		if(presentationSide.Length < (int) sideType){
			Log.Error("CubeAnimationManager", "CubeAnimationManager - FocusOnSide {0} has wrong number as side. ", (int) sideType);
			return;
		}

		StopAllCubeAnimations ();

		if (AnimationState == CubeAnimationState.IDLE_AS_UNFOLDED || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED  || AnimationState == CubeAnimationState.FOCUSING_TO_SIDE) {
			AnimateFocusOnSide(sideType);
		} else if( AnimationState == CubeAnimationState.GOING_FROM_SCENE){
			//do nothing - it is going from scene
		}else {
			AnimateUnFold (AnimateFocusOnSide, (System.Object)sideType);
		}
	}

	/// <summary>
	/// Focuses the on next side. 
	/// </summary>
	public void FocusOnNextSide(){
		m_LastCubeSideFocused = (CubeSideType) (((int)m_LastCubeSideFocused + 1) % uiFaceOnSide.Length);
		FocusOnSide(m_LastCubeSideFocused);
	}

	private void AnimateFocusOnSide(System.Object sideType){
		AnimateFocusOnSide ((CubeSideType)sideType);
	}
	               
	private void AnimateFocusOnSide(CubeSideType sideType){
		AnimationState = CubeAnimationState.FOCUSING_TO_SIDE;

		for (int i = 0; i < presentationSide.Length; i++) {
			Vector3 offsetPosition = presentationSide[i].transform.parent.localPosition + presentationSide[i].transform.parent.parent.localPosition;
			if(i == 0){
				offsetPosition += presentationSide[i].transform.parent.parent.parent.localPosition;
			}

			if(i == (int) sideType){	//Our hero object!
				animationPositionOnSidePresentation[i] = LeanTween.moveLocal (presentationSide[i], positionFocus[0] - offsetPosition, timeForFocusing).setEase (easeForFocusing).setOnComplete(()=>{
					AnimationState = CubeAnimationState.IDLE_AS_FOCUSED;
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
		if (AnimationState == CubeAnimationState.IDLE_AS_FOCUSED || AnimationState == CubeAnimationState.FOCUSING_TO_SIDE) {
			StopAllCubeAnimations ();
			AnimateUnfocus (null, null);
		}
	}

	private void AnimateUnfocus(System.Action<System.Action<System.Object>, System.Object> callBackOnComplete = null, System.Object paramOnComplete = null, System.Action<System.Object> callBackOnCompleteLoop = null)
    {
		
		AnimationState = CubeAnimationState.FOCUSING_TO_SIDE;	
		for (int i = 0; i < uiFaceOnSide.Length; i++) {
			if(i == uiFaceOnSide.Length - 1){
				animationPositionOnSide [i] = LeanTween.moveLocal (presentationSide [i], Vector3.zero, timeForFocusing).setEase (easeForUnfocusing).setOnComplete(()=>{
					AnimationState = CubeAnimationState.IDLE_AS_UNFOLDED;

					if(callBackOnComplete != null){
						callBackOnComplete(callBackOnCompleteLoop, paramOnComplete);
					}
				});
			}
			else{
				animationPositionOnSide [i] = LeanTween.moveLocal (presentationSide [i], Vector3.zero, timeForFocusing).setEase (easeForUnfocusing);
			}
		}
	}

	#endregion

	#region Animation Stop / Reset

	private void StopAllCubeAnimations(){
		if( AnimationState == CubeAnimationState.GOING_FROM_SCENE){
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
        SetInitialConditions();
        ShowCube();
    }

	void OnDisable()
	{
		StopAllCubeAnimations();
		
		//Avatar Object position change
		if(avatarGameobject)
			animationAvatarPosition = LeanTween.moveLocal(avatarGameobject, Vector3.zero, timeForFoldingUnfolding).setEase(easeForUnfolding);
	}

	void SetInitialConditions()
    {
        for (int i = 0; i < uiFaceOnSide.Length; i++)
        {
            presentationSide[i].transform.localPosition = Vector3.zero;
            uiFaceOnSide[i].transform.localPosition = positionFold[i];
            uiFaceOnSide[i].transform.localRotation = rotationFold[i];
        }

		transform.position = new Vector3 (0, -40, 0); //m_initialPosition;
		transform.localScale = Vector3.one; // m_initialLocalScale;
        transform.localRotation = m_initialLocalRotation;

    }


    #endregion

    #region Focus

    public void ShowCube(){
		StopAllCubeAnimations ();
		AnimateShowingCube ();
	}

	private void AnimateShowingCube(){
		AnimationState = CubeAnimationState.COMING_TO_SCENE;
		LeanTween.moveLocal (gameObject, new Vector3 (0, 40, 0), timeForComingToScene).setFrom (new Vector3 (0, -40, 0)).setEase (easeForComingToScene);
        
        //Avatar Object position change
        animationAvatarPosition = LeanTween.moveLocal(avatarGameobject, Vector3.zero, timeForFoldingUnfolding).setEase(easeForUnfolding);

		LeanTween.scale (gameObject, m_initialLocalScale, timeForComingToScene).setFrom (Vector3.one).setEase (easeForComingToScene).setOnStart(()=>{
			SetVisible (true);
		}).setOnComplete(()=>{
			AnimationState = CubeAnimationState.IDLE_AS_FOLDED;
		});
	}

	public void LeaveTheSceneAndDestroy(){
        //StopAllCubeAnimations ();
        //timeForFoldingUnfolding = timeForFoldingUnfolding / 2.0f;
        //timeForFocusing = timeForFocusing / 2.0f;
        easeForUnfocusing = LeanTweenType.easeInCubic;
        easeForFolding = LeanTweenType.linear;
        easeForUnfolding = LeanTweenType.linear;

        if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED)
        {
            AnimateUnfocus(AnimateFold, null, AnimateDestroyingCube);
        }
        else if(AnimationState == CubeAnimationState.UNFOLDING || AnimationState == CubeAnimationState.IDLE_AS_UNFOLDED || AnimationState == CubeAnimationState.FOLDING)
        {
            AnimateFold(AnimateDestroyingCube, null);
        }
        else if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene
        }
        else
        {
            AnimateDestroyingCube(true);
        }

        //AnimateDestroyingCube (true);
	}

    private void AnimateDestroyingCube(System.Object destroy)
    {
        AnimateDestroyingCube(true);
    }
    private void AnimateDestroyingCube(bool destroy){
		AnimationState = CubeAnimationState.GOING_FROM_SCENE;
		LeanTween.moveLocal (gameObject, new Vector3 (0, 240, 0), timeForLeavingTheScene).setEase (easeForGoingFromScene).setOnComplete(()=>{
            if(destroy){
				Camera[] cameraList = transform.parent.GetComponentsInChildren<Camera>();
				foreach (Camera itemCamera in cameraList) {
					itemCamera.targetTexture = null;
					itemCamera.enabled = false;
				}
			    Destroy(transform.parent.gameObject);
			}
		});

        //Avatar Object position change
        animationAvatarPosition = LeanTween.moveLocal(avatarGameobject, Vector3.zero, timeForFoldingUnfolding).setEase(easeForUnfolding);
    }


	#endregion

	#region Zoom 

	public void ChangeScale(float zoom){
		float zoomDiff = Mathf.Abs (1.0f - zoom);
		if (zoomDiff == 0.0f)
			zoomDiff = 0.0001f;

		LeanTween.scale (gameObject, transform.localScale * zoom, 1.0f * zoomDiff).setEase (LeanTweenType.linear);
	}

    #endregion

    #region Dragging One Finger

    private int frameCountOneFinger;
    public void DragOneFinger(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture)
    {
        if (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING)
        {
            Log.Status("CubeAnimationManager", "oneFingerManipulationTransformedHandler: {0}", OneFingerManipulationGesture.DeltaPosition);

            Quaternion rotation = Quaternion.Euler(OneFingerManipulationGesture.DeltaPosition.y / Screen.height * m_OneFingerRotationModifier,
                                                    -OneFingerManipulationGesture.DeltaPosition.x / Screen.width * m_OneFingerRotationModifier,
                                                    0.0f);

            m_OneFingerCubeRotation *= rotation;
            m_LastFrameOneFingerDrag = Time.frameCount;
            statinoaryRotationSpeed = 0.0f; //stop the statinoary rotation
            //Log.Status("CubeAnimationManager", "Rotation: {0} , Target rotation : {1} ", rotation.eulerAngles, m_OneFingerCubeRotation.eulerAngles);
        }
    }

    private int m_LastFrameOneFingerDrag = 0;
    private void CubeOneFingerDragAnimationOnUpdate()
    {
        if (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING)
        {
            //For Rotating the cube by one finger 
            m_OneFingerCubeRotation = Quaternion.Lerp(m_OneFingerCubeRotation, Quaternion.identity, Time.deltaTime * m_OneFingerRotationAnimationSpeed);
            transform.Rotate(m_OneFingerCubeRotation.eulerAngles, Space.World);
        }
    }

    #endregion

    void OnDestroy()
	{
		for (int i = 0; i < renderTexSide.Length; i++) {
			renderTexSide[i].Release();
		}
	}
}
