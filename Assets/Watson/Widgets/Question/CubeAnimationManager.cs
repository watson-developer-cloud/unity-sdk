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
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;
using IBM.Watson.Widgets.Avatar;

/// <summary>
/// Cube animation manager. All animations related with Cube located here.
/// </summary>
public class CubeAnimationManager : WatsonBaseAnimationManager
{

    #region Enumerations Related with Animation and Cube Sides

    public enum CubeSideType
    {
		NONE = -1,
        TITLE = 0,
        ANSWERS,
        PARSE,
        EVIDENCE,
        LOCATION,
        CHAT
    }

    public enum CubeAnimationState
    {
        NOT_PRESENT = 0,    //Cube is not created OR not visible on the scene
        COMING_TO_SCENE,
        IDLE_AS_FOLDED,
        UNFOLDING,
        IDLE_AS_UNFOLDED,
        FOLDING,
        FOCUSING_TO_SIDE,
        IDLE_AS_FOCUSED,
        GOING_FROM_SCENE        //Animating just before destroying
    }

    #endregion

	#region Private Members

	private CubeAnimationState m_currentState;

	//Initial Values
	private Vector3 m_initialPosition;
	private Vector3 m_initialLocalScale;
	private Quaternion m_initialLocalRotation;
	private Vector3 m_initialPositionMainCamera;
	private CubeSideType m_LastCubeSideFocused = CubeSideType.NONE;
	private Vector3 m_PositionAfterLeaving;
	//All animation position / rotation values
	private Vector3[] m_positionUnfold;
	private Quaternion[] m_rotationUnfold;
	private Vector3[] m_positionFold;
	private Quaternion[] m_rotationFold;
	private Vector3[] m_positionFocus;

	//All UI Elements - Faces 
	[Header("UI Faces")]
	[SerializeField]
	private GameObject[] m_uiFaceOnSide;
	
	[Header("UI Faces for Zoom")]
	[SerializeField]
	private GameObject[] m_presentationSide;

	[Header("Projection of Faces")]
	[SerializeField]
	private GameObject[] m_ProjectionSide;
	
	[Header("Render Textures")]
	[SerializeField]
	private RenderTexture[] m_renderTexSide;

	//Stationary Rotation Related values - Folded Idle
	[SerializeField]
	private float m_StatinoaryRotationSpeed = 10.0f;
	private float m_InitialStationaryRotationSpeed;
	private float m_StationaryRotationBackupSpeed = 5.0f;
	[SerializeField]
	private Vector3 m_StatinoaryRotationVector = Vector3.up;
	private bool m_isRotating = true;
	private float m_OneFingerRotationModifier = 100.0f;
	private float m_OneFingerRotationAnimationSpeed = 5.0f;
	private Quaternion m_OneFingerCubeRotation;
	private int m_LastFrameOneFingerDrag = 0;
	private LeanTweenType m_EaseForFolding = LeanTweenType.easeInOutCubic;

	
	//Coming to scene values
	[SerializeField]
	private float m_TimeForComingToScene = 1.0f;
	[SerializeField]
	private LeanTweenType m_EaseForComingToScene = LeanTweenType.easeOutElastic;
	[SerializeField]
	private Vector3 m_PositionBeforeComing = Vector3.zero;
	[SerializeField]
	private Vector3 m_PositionAfterComing = Vector3.zero;

	//Unfolding Values
	[SerializeField]
	private float m_TimeForFoldingUnfolding = 1.0f;
	[SerializeField]
	private LeanTweenType m_EaseForUnfolding = LeanTweenType.easeInOutCubic;
	[SerializeField]
	private Vector3 m_PositionCubeOffsetUnfold = Vector3.zero;
	[SerializeField]
	private float m_DistanceFromCameraInZAfterUnfolding = 100f;


	//Focusing side
	[SerializeField]
	private float m_TimeForFocusingUnfocusing = 1.0f;
	[SerializeField]
	private LeanTweenType m_EaseForFocusing = LeanTweenType.easeInOutCubic;

	//Unfocusing - going back to unfolded state
	[SerializeField]
	public LeanTweenType m_EaseForUnfocusing = LeanTweenType.easeInOutCubic;

	//Leaving the scene
	[SerializeField]
	private float timeForLeavingTheScene = 1.0f;
	[SerializeField]
	private LeanTweenType easeForGoingFromScene = LeanTweenType.easeOutCirc;

	//Passage One Finger Animation
	LTBezierPath m_BezierPathToCenter;
	LTBezierPath m_BezierPathOrientationToCenter;
	LTBezierPath m_BezierPathToStack;
	LTBezierPath m_BezierPathOrientationToStack;

	//All animation Descriptions
	private LTDescr m_animationMoveForComingScene;
	private LTDescr m_animationScaleForComingScene;
	private LTDescr[] m_animationPositionOnSide;
	private LTDescr[] m_animationRotationOnSide;
	private LTDescr m_animationRotationCube;
	private LTDescr m_animationPositionCube;
	private LTDescr[] m_animationPositionOnSidePresentation;
	private LTDescr m_animationAvatarPosition;

	#endregion
	
	#region Public Members

	public static float sm_DelayShowCube = 0.0f;

    /// <summary>
    /// Gets the state of the current animation.
    /// </summary>
    /// <value>The state of the current animation.</value>
    public CubeAnimationState AnimationState
    {
        get
        {
            return m_currentState;
        }
        set
        {
            bool hasStateChanged = (m_currentState != value);
            m_currentState = value;

            if (hasStateChanged)
            {
                EventManager.Instance.SendEvent(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION);
            }

        }
    }

	public CubeSideType SideFocused{
		get{
			return m_LastCubeSideFocused;
		}
	}

    #endregion

	#region OnEnable / SetInitialConditions / OnDisable / Awake / Update / OnDestroy

	protected override void OnEnable()
	{
		StopAllCubeAnimations();
		SetInitialConditions();
		ShowCube();
	}
	
	protected override void OnDisable()
	{
		//StopAllCubeAnimations();
		sm_DelayShowCube = 0.3f;
		LeaveTheSceneAndDestroy ();
	}
	
	void SetInitialConditions()
	{
		for (int i = 0; i < m_uiFaceOnSide.Length; i++)
		{
			m_presentationSide[i].transform.localPosition = Vector3.zero;
			m_uiFaceOnSide[i].transform.localPosition = m_positionFold[i];
			m_uiFaceOnSide[i].transform.localRotation = m_rotationFold[i];
		}
		
		transform.position = m_PositionBeforeComing; //m_initialPosition;
		transform.localScale = Vector3.one; // m_initialLocalScale;
		transform.localRotation = m_initialLocalRotation;
	}

    void Awake()
    {
        m_positionUnfold = new Vector3[]{
            new Vector3 (0f, 0f, 10f),
            new Vector3 (-5f + 2.5f, 0f, 5f),
            new Vector3 (-5f + 2.5f, 0f, -5f),
            new Vector3 (-5f + 2.5f, 0f, -5f),
            new Vector3 (-5f + 2.5f, 5f, 0f),
            new Vector3 (-5f + 2.5f, -5f, 0f)};

        m_rotationUnfold = new Quaternion[]{
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 0f, 0f)
        };

        m_positionFold = new Vector3[]{
            new Vector3 (0f, 0f, 10f),
            new Vector3 (-5f, 0f, 5f),
            new Vector3 (-5f, 0f, -5f),
            new Vector3 (-5f, 0f, -5f),
            new Vector3 (-5f, 5f, 0f),
            new Vector3 (-5f, -5f, 0f)
        };

        m_rotationFold = new Quaternion[]{
            Quaternion.Euler (0f, 90f, 0f),
            Quaternion.Euler (0f, 90f, 0f),
            Quaternion.Euler (0f, 0f, 0f),
            Quaternion.Euler (0f, 270f, 0f),
            Quaternion.Euler (0f, 0f, 270f),
            Quaternion.Euler (0f, 0f, 90f)
        };

        m_positionFocus = new Vector3[]{
            new Vector3 (-21.0f, -0.6f,     -2f), 	//Our Hero Side - Zoomed to the camera
			new Vector3 (17.5f, 2.5f,   25.0f),	//XRay Logo on right most 
			new Vector3 (17.5f, 10f,    11.5f),	//Top - backup
			new Vector3 (17.5f, 2.5f,   16f),	//Between XRay logo and hero
			new Vector3 (17.5f, -5f,    11.5f),	//Bottom close to hero
			new Vector3 (17.5f, -5f,    20.5f) 	//Bottom close to logo
		};

		m_BezierPathToCenter = new LTBezierPath ( new Vector3[]{ 
			new Vector3(-555,-640.2f,-125),
			new Vector3(-555,-640.2f,-125), 
			new Vector3(-888, -406, -125), 
			new Vector3(-888, -406, -125)});

		m_BezierPathOrientationToCenter = new LTBezierPath (new Vector3[]{ 
			new Vector3(0,0,0),
			new Vector3(0,45,0), 
			new Vector3(0,45,0), 
			new Vector3(45,45,0)});
		
		m_BezierPathToStack =  new LTBezierPath ( new Vector3[]{ 
				new Vector3(-888,	-406.2f,		-125),
				new Vector3(600,	-680.1f,		-200), 
				new Vector3(600, 	-680.1f, 	-300), 
				new Vector3(600, 	-680.1f, 	-400)});

		m_BezierPathOrientationToStack =   new LTBezierPath (new Vector3[]{ 
				new Vector3(45,45,0),
				new Vector3(0,45,0), 
				new Vector3(0,35,0), 
				new Vector3(0,0,0)});


        if (m_uiFaceOnSide.Length != m_renderTexSide.Length
            || m_uiFaceOnSide.Length != m_positionUnfold.Length
            || m_uiFaceOnSide.Length != m_rotationUnfold.Length
            || m_uiFaceOnSide.Length != m_positionFold.Length
            || m_uiFaceOnSide.Length != m_rotationFold.Length)
        {

            Log.Error("CubeAnimationManager", "Cube Animation has some missing variables!");
        }

        for (int i = 0; i < m_renderTexSide.Length; i++)
        {
            if (!m_renderTexSide[i].useMipMap)
                m_renderTexSide[i].useMipMap = true;
        }

        m_initialPosition = transform.position;
        m_initialLocalScale = transform.localScale;
        m_InitialStationaryRotationSpeed = m_StatinoaryRotationSpeed;

        m_initialPositionMainCamera = new Vector3(0, 17.5f, -135);
        m_initialLocalRotation = transform.localRotation;

		m_PositionAfterLeaving = new Vector3 (0, 240, 0);

        SetInitialConditions();

        //ShowCube ();
    }


    void Update()
    {
        CubeOneFingerDragAnimationOnUpdate();
        CubeStatinoaryAnimation();
    }

	void OnDestroy()
	{
		for (int i = 0; i < m_renderTexSide.Length; i++)
		{
			m_renderTexSide[i].Release();
		}
	}

	#endregion

	#region Coming to Scene
	
	public void ShowCube()
	{
		StopAllCubeAnimations();
		AnimateShowingCube();
	}
	
	private void AnimateShowingCube()
	{
		AnimationState = CubeAnimationState.COMING_TO_SCENE;

		//Avatar Object position change
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_MOVE_DEFAULT, m_TimeForFoldingUnfolding);
		
		m_animationScaleForComingScene = LeanTween.scale(gameObject, m_initialLocalScale, m_TimeForComingToScene).setFrom(Vector3.one).setEase(m_EaseForComingToScene).setDelay(sm_DelayShowCube).setOnComplete(() =>
		{
			AnimationState = CubeAnimationState.IDLE_AS_FOLDED;
		});

		m_animationMoveForComingScene = LeanTween.moveLocal(gameObject, m_PositionAfterComing, m_TimeForComingToScene).setEase(m_EaseForComingToScene).setDelay(sm_DelayShowCube);

	}

	#endregion

	#region Cube Stationary Rotation Animation - on idle

    private void CubeStatinoaryAnimation()
    {
        if (m_isRotating && (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING))
        {    //If it is fold already and rotation is true we are rotating.
			//If there is one finger drag we are not rotating as well
            if ((Time.frameCount - m_LastFrameOneFingerDrag) > 1)
            {
                m_StatinoaryRotationSpeed = Mathf.Lerp(m_StatinoaryRotationSpeed, m_InitialStationaryRotationSpeed, Time.deltaTime * m_StationaryRotationBackupSpeed);
                transform.Rotate(m_StatinoaryRotationVector * Time.deltaTime * m_StatinoaryRotationSpeed, Space.World);
            }

        }
    }

    public void RotateOrPause()
    {
        m_isRotating = !m_isRotating;
    }

	#endregion

    #region Folding - Unfolding Animations

    public void Fold()
    {
        StopAllCubeAnimations();

        if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED)
        {
            AnimateUnfocus(AnimateFold, null);
        }
        else if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene
        }
        else
        {
            AnimateFold(null, null);
        }
    }

    private void AnimateFold(System.Object paramOnComplete)
    {
        AnimateFold(null, null);
    }

    private void AnimateFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null)
    {
        AnimationState = CubeAnimationState.FOLDING;
        for (int i = 0; i < m_uiFaceOnSide.Length; i++)
        {
            m_animationPositionOnSide[i] = LeanTween.moveLocal(m_uiFaceOnSide[i], m_positionFold[i], m_TimeForFoldingUnfolding).setEase(m_EaseForFolding);
            if (i == m_uiFaceOnSide.Length - 1)
                m_animationRotationOnSide[i] = LeanTween.rotateLocal(m_uiFaceOnSide[i], m_rotationFold[i].eulerAngles, m_TimeForFoldingUnfolding).setEase(m_EaseForFolding).setOnComplete(
                    () =>
                    {
                        AnimationState = CubeAnimationState.IDLE_AS_FOLDED; //Folding finish

                        if (callBackOnComplete != null)
                            callBackOnComplete(paramOnComplete);
                    });
            else
                m_animationRotationOnSide[i] = LeanTween.rotateLocal(m_uiFaceOnSide[i], m_rotationFold[i].eulerAngles, m_TimeForFoldingUnfolding).setEase(m_EaseForFolding);
        }

        //Move object cloase to camera
        m_animationPositionCube = LeanTween.move(gameObject, m_initialPosition, m_TimeForFoldingUnfolding).setEase(m_EaseForFolding);

		//Avatar Object position change
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_MOVE_DEFAULT, m_TimeForFoldingUnfolding);
  
    }

    public void UnFold()
    {
        StopAllCubeAnimations();
        if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED)
        {
            AnimateUnfocus(AnimateUnFold, null);
        }
        else if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene
        }
        else
        {
            AnimateUnFold(null, null);
        }

    }

    private void AnimateUnFold(System.Object paramOnComplete)
    {
        AnimateUnFold(null, null);
    }

    private void AnimateUnFold(System.Action<System.Object> callBackOnComplete = null, System.Object paramOnComplete = null)
    {
        AnimationState = CubeAnimationState.UNFOLDING;
        for (int i = 0; i < m_uiFaceOnSide.Length; i++)
        {
            m_animationPositionOnSide[i] = LeanTween.moveLocal(m_uiFaceOnSide[i], m_positionUnfold[i], m_TimeForFoldingUnfolding).setEase(m_EaseForUnfolding);
            m_animationRotationOnSide[i] = LeanTween.rotateLocal(m_uiFaceOnSide[i], m_rotationUnfold[i].eulerAngles, m_TimeForFoldingUnfolding).setEase(m_EaseForUnfolding);
        }

        //Rotate to camera 
        m_animationRotationCube = LeanTween.rotateLocal(gameObject, Camera.main.transform.rotation.eulerAngles - new Vector3(0.0f, 90.0f, 0.0f), m_TimeForFoldingUnfolding).setEase(m_EaseForUnfolding).setOnComplete(
            () =>
            {
                AnimationState = CubeAnimationState.IDLE_AS_UNFOLDED;   //unfolding finish

                if (callBackOnComplete != null)
                    callBackOnComplete(paramOnComplete);
            });

        //Move object cloase to camera
        m_animationPositionCube = LeanTween.move(gameObject, m_initialPositionMainCamera + m_DistanceFromCameraInZAfterUnfolding * Camera.main.transform.forward + m_PositionCubeOffsetUnfold, m_TimeForFoldingUnfolding).setEase(m_EaseForUnfolding);

        //Avatar Object position change
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_MOVE_DOWN, m_TimeForFoldingUnfolding);
    }

    #endregion

    #region Focus - UnFocus Animations

    /// <summary>
    /// Focuses the on side of the cube
    /// </summary>
    /// <param name="sideType">Side type.</param>
    public void FocusOnSide(CubeSideType sideType)
    {
        m_LastCubeSideFocused = sideType;
        if (m_presentationSide.Length < (int)sideType)
        {
            Log.Error("CubeAnimationManager", "CubeAnimationManager - FocusOnSide {0} has wrong number as side. ", (int)sideType);
            return;
        }

        StopAllCubeAnimations();

        if (AnimationState == CubeAnimationState.IDLE_AS_UNFOLDED || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED || AnimationState == CubeAnimationState.FOCUSING_TO_SIDE)
        {
            AnimateFocusOnSide(sideType);
        }
        else if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene
        }
        else
        {
            AnimateUnFold(AnimateFocusOnSide, (System.Object)sideType);
        }
    }

    /// <summary>
    /// Focuses the on next side. 
    /// </summary>
    public void FocusOnNextSide()
    {
        m_LastCubeSideFocused = (CubeSideType)(((int)m_LastCubeSideFocused + 1) % m_uiFaceOnSide.Length);
        FocusOnSide(m_LastCubeSideFocused);
    }

    private void AnimateFocusOnSide(System.Object sideType)
    {
        AnimateFocusOnSide((CubeSideType)sideType);
    }

    private void AnimateFocusOnSide(CubeSideType sideType)
    {
        AnimationState = CubeAnimationState.FOCUSING_TO_SIDE;

        for (int i = 0; i < m_presentationSide.Length; i++)
        {
            Vector3 offsetPosition = m_presentationSide[i].transform.parent.localPosition + m_presentationSide[i].transform.parent.parent.localPosition;
            if (i == 0)
            {
                offsetPosition += m_presentationSide[i].transform.parent.parent.parent.localPosition;
            }

            if (i == (int)sideType)
            {   //Our hero object!
                m_animationPositionOnSidePresentation[i] = LeanTween.moveLocal(m_presentationSide[i], m_positionFocus[0] - offsetPosition, m_TimeForFocusingUnfocusing).setEase(m_EaseForFocusing).setOnComplete(() =>
                {
                    AnimationState = CubeAnimationState.IDLE_AS_FOCUSED;
                });
            }
            else if (i < (int)sideType)
            {
                //				Vector3[] splineVec = new Vector3[]{ presentationSide[i].transform.localPosition,presentationSide[i].transform.localPosition - Vector3.right * (i  * 2) * (i % 2 == 0 ? 1.0f : -1.0f), positionZoom[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition  - Vector3.right * (i  * 2) * (i % 2 == 0 ? 1.0f : -1.0f),positionZoom[ ((i + 1) % uiFaceOnSide.Length) ] - offsetPosition};
                m_animationPositionOnSidePresentation[i] = LeanTween.moveLocal(m_presentationSide[i], m_positionFocus[((i + 1) % m_uiFaceOnSide.Length)] - offsetPosition, m_TimeForFocusingUnfocusing).setEase(m_EaseForFocusing);
            }
            else
            {
                //				Vector3[] splineVec = new Vector3[]{presentationSide[i].transform.localPosition, presentationSide[i].transform.localPosition - Vector3.right * (i * 2) * (i % 2 == 0 ? 1.0f : -1.0f), positionZoom[i] - offsetPosition  - Vector3.right * (i * 2) * (i % 2 == 0 ? 1.0f : -1.0f),positionZoom[i] - offsetPosition};
                m_animationPositionOnSidePresentation[i] = LeanTween.moveLocal(m_presentationSide[i], m_positionFocus[i] - offsetPosition, m_TimeForFocusingUnfocusing).setEase(m_EaseForFocusing);
            }
        }

        //Make sure that our sides are in right rotation
        for (int i = 0; i < m_uiFaceOnSide.Length; i++)
        {
            m_animationRotationOnSide[i] = LeanTween.rotateLocal(m_uiFaceOnSide[i], m_rotationUnfold[i].eulerAngles, m_TimeForFocusingUnfocusing).setEase(m_EaseForFocusing);
        }

    }

    /// <summary>
    /// Unfocus from the current focus and returning back to unfold state
    /// </summary>
    public void UnFocus()
    {
        if (AnimationState == CubeAnimationState.IDLE_AS_FOCUSED || AnimationState == CubeAnimationState.FOCUSING_TO_SIDE)
        {
            StopAllCubeAnimations();
            AnimateUnfocus(null, null);
        }
    }

    private void AnimateUnfocus(System.Action<System.Action<System.Object>, System.Object> callBackOnComplete = null, System.Object paramOnComplete = null, System.Action<System.Object> callBackOnCompleteLoop = null)
    {

        AnimationState = CubeAnimationState.FOCUSING_TO_SIDE;
        for (int i = 0; i < m_uiFaceOnSide.Length; i++)
        {
            if (i == m_uiFaceOnSide.Length - 1)
            {
                m_animationPositionOnSide[i] = LeanTween.moveLocal(m_presentationSide[i], Vector3.zero, m_TimeForFocusingUnfocusing).setEase(m_EaseForUnfocusing).setOnComplete(() =>
                {
                    AnimationState = CubeAnimationState.IDLE_AS_UNFOLDED;

                    if (callBackOnComplete != null)
                    {
                        callBackOnComplete(callBackOnCompleteLoop, paramOnComplete);
                    }
                });
            }
            else
            {
                m_animationPositionOnSide[i] = LeanTween.moveLocal(m_presentationSide[i], Vector3.zero, m_TimeForFocusingUnfocusing).setEase(m_EaseForUnfocusing);
            }
        }
    }

    #endregion

    #region Animation Stop / Reset

    private void StopAllCubeAnimations()
    {
        if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene, early return before any animation stopping!
            return;
        }

		StopComingSceneAnimation ();
        StopFoldingAnimation();
        StopAvatarAnimation();
        StopFocusAnimation();
        StopCubeCameraFacingRotationAnimation();
    }

	private void StopComingSceneAnimation(){
		if (m_animationMoveForComingScene != null) {

		}

		if (m_animationScaleForComingScene != null) {

		}

	}
    private void StopAvatarAnimation()
    {
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_STOP_MOVE_DEFAULT);
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_STOP_MOVE_DOWN);
    }

    private void StopFoldingAnimation()
    {
        if (m_animationPositionOnSide == null)
        {
            m_animationPositionOnSide = new LTDescr[m_uiFaceOnSide.Length];
        }
        else
        {
            for (int i = 0; i < m_animationPositionOnSide.Length; i++)
            {
                if (m_animationPositionOnSide[i] != null)
                    LeanTween.cancel(m_animationPositionOnSide[i].uniqueId);
            }
        }

        if (m_animationRotationOnSide == null)
        {
            m_animationRotationOnSide = new LTDescr[m_uiFaceOnSide.Length];
        }
        else
        {
            for (int i = 0; i < m_animationRotationOnSide.Length; i++)
            {
                if (m_animationRotationOnSide[i] != null)
                    LeanTween.cancel(m_animationRotationOnSide[i].uniqueId);
            }
        }
    }

    private void StopFocusAnimation()
    {
        if (m_animationPositionOnSidePresentation == null)
        {
            m_animationPositionOnSidePresentation = new LTDescr[m_uiFaceOnSide.Length];
        }
        else
        {
            for (int i = 0; i < m_animationPositionOnSide.Length; i++)
            {
                if (m_animationPositionOnSidePresentation[i] != null)
                    LeanTween.cancel(m_animationPositionOnSidePresentation[i].uniqueId);
            }
        }
    }


    private void StopCubeCameraFacingRotationAnimation()
    {
        if (m_animationRotationCube != null)
        {
            LeanTween.cancel(m_animationRotationCube.uniqueId);
        }

        if (m_animationPositionCube != null)
        {
            LeanTween.cancel(m_animationPositionCube.uniqueId);
        }
    }

    #endregion

    #region Focus

    public void LeaveTheSceneAndDestroy()
    {
        StopAllCubeAnimations();
      
		m_TimeForFocusingUnfocusing = 0.6f;
		m_TimeForFoldingUnfolding = 0.6f;



        if (AnimationState == CubeAnimationState.FOCUSING_TO_SIDE || AnimationState == CubeAnimationState.IDLE_AS_FOCUSED)
        {
			m_EaseForUnfocusing = LeanTweenType.easeInSine;
			m_EaseForFolding = LeanTweenType.linear;
            AnimateUnfocus(AnimateFold, null, AnimateDestroyingCube);
        }
        else if(AnimationState == CubeAnimationState.UNFOLDING || AnimationState == CubeAnimationState.IDLE_AS_UNFOLDED || AnimationState == CubeAnimationState.FOLDING)
        {
			m_EaseForFolding = LeanTweenType.easeInSine;
            AnimateFold(AnimateDestroyingCube, null);
        }
        else if (AnimationState == CubeAnimationState.GOING_FROM_SCENE)
        {
            //do nothing - it is going from scene
        }
        else
        {
           //AnimateDestroyingCube(true);
        }


		//Avatar Object position change
		EventManager.Instance.SendEvent (Constants.Event.ON_AVATAR_MOVE_DEFAULT, m_TimeForFoldingUnfolding);
        AnimateDestroyingCube(true);
    }

    private void AnimateDestroyingCube(System.Object destroy)
    {
        AnimateDestroyingCube(true);
    }
    private void AnimateDestroyingCube(bool destroy)
    {
        AnimationState = CubeAnimationState.GOING_FROM_SCENE;


		Vector3[] vectorList = new Vector3[]{transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y, 0), m_PositionAfterLeaving,  m_PositionAfterLeaving};

		LeanTween.moveLocal(gameObject, vectorList, timeForLeavingTheScene).setEase(easeForGoingFromScene).setOnComplete(() =>
        {
            if (destroy)
            {
                Camera[] cameraList = transform.parent.GetComponentsInChildren<Camera>();
                foreach (Camera itemCamera in cameraList)
                {
                    itemCamera.targetTexture = null;
                    itemCamera.enabled = false;
                }
                Destroy(transform.parent.gameObject);
            }
        });
		    
    }


    #endregion

    #region Zoom 

    public void ChangeScale(float scaleToChange)
    {
        float scaleDiff = Mathf.Abs(1.0f - scaleToChange);
        if (scaleDiff == 0.0f)
            scaleDiff = 0.0001f;

        LeanTween.scale(gameObject, transform.localScale * scaleToChange, 1.0f * scaleDiff).setEase(LeanTweenType.linear);
    }

    #endregion

	#region Tap 

	/// <summary>
	/// Method called on Tapping on Question Widget 
	/// </summary>
	/// <param name="tapGesture">Tap Gesture with all touch information</param>
	/// <param name="hitTransform">Hit Tranform of tap</param>
	public void OnTapInside(TouchScript.Gestures.TapGesture tapGesture, RaycastHit raycastHit)
	{
		if (tapGesture == null || raycastHit.Equals(default(RaycastHit)) ) {
			Log.Warning("CubeAnimationManager", "OnTapInside has invalid arguments!");
			return;
		}

		CubeSideType sideTapped = SideOfTap (raycastHit.transform);
		if (AnimationState == CubeAnimationState.IDLE_AS_FOCUSED && sideTapped == SideFocused) {
			TapInsideOnFocusedSide(tapGesture, raycastHit, sideTapped);
		} else {
			//Touch on side
			switch (AnimationState)
			{
			case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
				break;
			case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
				break;
			case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
				UnFold();
				break;
			case CubeAnimationManager.CubeAnimationState.UNFOLDING:
				FocusOnSide(sideTapped);
				break;
			case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
				FocusOnSide(sideTapped);
				break;
			case CubeAnimationManager.CubeAnimationState.FOLDING:
				UnFold();
				break;
			case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
				FocusOnSide(sideTapped);
				break;
			case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
				FocusOnSide(sideTapped);
				break;
			case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
				break;
			default:
				break;
			}
		}
	}

	private CubeSideType SideOfTap(Transform hitTransform){
		int touchedSide = -1;
		int.TryParse(hitTransform.name.Substring(1, 1), out touchedSide);
		return (CubeAnimationManager.CubeSideType)touchedSide;
	}
	
	private void FocusOnSide(Transform hitTransform)
	{
		FocusOnSide(SideOfTap(hitTransform));
	}
	
	/// <summary>
	/// Raises the tap outside event.
	/// </summary>
	/// <param name="tapGesture">Tap gesture.</param>
	/// <param name="hitTransform">Hit transform.</param>
	public void OnTapOutside(TouchScript.Gestures.TapGesture tapGesture, RaycastHit raycastHit)
	{
		if (tapGesture == null || !raycastHit.Equals(default(RaycastHit)) ) {
			Log.Warning("CubeAnimationManager", "OnTapOutside has invalid arguments!");
			return;
		}

		//Touch out-side
		switch (AnimationState)
		{
		case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
			break;
		case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
			break;
		case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
			break;
		case CubeAnimationManager.CubeAnimationState.UNFOLDING:
			Fold();
			break;
		case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
			Fold();
			break;
		case CubeAnimationManager.CubeAnimationState.FOLDING:
			break;
		case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
			UnFocus();
			break;
		case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
			UnFocus();
			break;
		case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
			break;
		default:
			break;
		}
	}


	private Vector2 getScreenPositionForCamera(Vector2 screenPositionOnMainCamera, Camera cameraOnProjection, Transform hexagonalPlane){
		Vector2 normalizeScreenTouchRelativeToPlane = Vector2.zero;

		if (hexagonalPlane.parent != null) {
			MeshFilter meshFilter = hexagonalPlane.parent.GetComponent<MeshFilter> ();

			if (meshFilter != null) {
				Vector3 planePointMin = meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.min);
				Vector3 planePointMax = meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.max);
				
				//Log.Status ("CubeAnimationManager", "0 - MIN) meshFilter.mesh.bounds.min: " + meshFilter.mesh.bounds.min + " Global : " + meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.min) + " - Camera.main.WorldToScreenPoint: " + Camera.main.WorldToScreenPoint(meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.min)));
				//Log.Status ("CubeAnimationManager", "0 - MAX) meshFilter.mesh.bounds.max: " + meshFilter.mesh.bounds.max + " Global : " + meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.max) + " - Camera.main.WorldToScreenPoint: " + Camera.main.WorldToScreenPoint(meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.max)));
				
				Vector3 screenPlaneBottomLeft = Camera.main.WorldToScreenPoint(meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.max));
				Vector3 screenPlaneTopRight = Camera.main.WorldToScreenPoint(meshFilter.transform.TransformPoint (meshFilter.mesh.bounds.min));
				Vector3 screenPlaneFromLeftToRight = screenPlaneTopRight - screenPlaneBottomLeft;
				
				Vector2 screenTouchRelativeToPlane = new Vector2 (screenPositionOnMainCamera.x - screenPlaneBottomLeft.x, screenPositionOnMainCamera.y - screenPlaneBottomLeft.y);
				normalizeScreenTouchRelativeToPlane = new Vector2 (screenTouchRelativeToPlane.x / screenPlaneFromLeftToRight.x, screenTouchRelativeToPlane.y / screenPlaneFromLeftToRight.y);
				
				float onePixelOnProjectionScreenSize = Screen.height / (cameraOnProjection.orthographicSize * 2.0f);
				
				Vector2 targetTextureSize = Vector2.one * onePixelOnProjectionScreenSize; 
				if (cameraOnProjection.targetTexture != null) {
					targetTextureSize = new Vector2( cameraOnProjection.targetTexture.width, cameraOnProjection.targetTexture.height);
				}
				
				normalizeScreenTouchRelativeToPlane.Scale (targetTextureSize);
			} else {
				Log.Error("CubeAnimationManager", "getScreenPositionForCamera - There is no mesh filter on parent object");
			}
		} else {
			Log.Error("CubeAnimationManager", "getScreenPositionForCamera - There is no parent object");
		}

		return  normalizeScreenTouchRelativeToPlane;
	}

	private Camera[] m_ProjectionCameraList;
	private Ray m_RayOnTapFocusedSide;

	private void TapInsideOnFocusedSide(TouchScript.Gestures.TapGesture tapGesture, RaycastHit raycastHit, CubeSideType sideTapped)
	{
		Log.Status("CubeAnimationManager", "TapInsideOnFocusedSide " + tapGesture.ScreenPosition + " sideTapped: " + sideTapped + " Index: " + ((int)sideTapped).ToString());

		if(m_ProjectionCameraList == null)
			m_ProjectionCameraList = Utility.FindObjects<Camera> (transform.parent.gameObject, "cam", isContains: true, sortByName: true);

		if (m_ProjectionCameraList.Length > (int)sideTapped && m_ProjectionCameraList [(int)sideTapped] != null) {
			Camera cameraProjectionParse = m_ProjectionCameraList [(int)sideTapped];	
			if (cameraProjectionParse != null) {
				Vector2 screenPointOnCamera = getScreenPositionForCamera (tapGesture.ScreenPosition, cameraProjectionParse, raycastHit.transform);
				m_RayOnTapFocusedSide = cameraProjectionParse.ScreenPointToRay (screenPointOnCamera);
				//TODO: Tap inside while cube is on focused!
				RaycastHit2D hit = default(RaycastHit2D);
				int layerOfSide = LayerMask.NameToLayer("Side"+((int)sideTapped).ToString());
				LayerMask layerMaskForSide = 1 << layerOfSide;
				hit = Physics2D.Raycast(m_RayOnTapFocusedSide.origin, m_RayOnTapFocusedSide.direction, Mathf.Infinity, layerMaskForSide);
				if(hit.collider != null){
					Log.Status("CubeAnimationManager","isHitOnLayer : " + hit.collider.transform);
				}
				else{
					Log.Status("CubeAnimationManager","NOT HIT ");
				}
			}
		} else {
			Log.Error("CubeAnimationManager", "TapInsideOnFocusedSide - Projection Camera couldn't find!");
		}

	}

	private void TapOutsideOnFocusedSide(TouchScript.Gestures.TapGesture tapGesture, RaycastHit raycastHit, CubeSideType sideTapped)
	{
		//TODO: Tap outside while cube is on focused!
		Log.Status("CubeAnimationManager", "TapOutsideOnFocusedSide " + tapGesture.ScreenPosition + " sideTapped: " + sideTapped);
	}

	void OnDrawGizmos() {
		if(!m_RayOnTapFocusedSide.Equals(default(Ray))){
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(m_RayOnTapFocusedSide);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(m_RayOnTapFocusedSide.origin, m_RayOnTapFocusedSide.origin + m_RayOnTapFocusedSide.direction * 1000);
		}
	}

	#endregion
	
	#region Dragging One Finger
	
	public void DragOneFingerFullScreen(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture)
	{
		if (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING)
		{
			Quaternion rotation = Quaternion.Euler(OneFingerManipulationGesture.DeltaPosition.y / Screen.height * m_OneFingerRotationModifier,
			                                       -OneFingerManipulationGesture.DeltaPosition.x / Screen.width * m_OneFingerRotationModifier,
			                                       0.0f);
			
			m_OneFingerCubeRotation *= rotation;
			m_LastFrameOneFingerDrag = Time.frameCount;
			m_StatinoaryRotationSpeed = 0.0f; //stop the statinoary rotation
		}
	}
	
	
	private void CubeOneFingerDragAnimationOnUpdate()
	{
		if (AnimationState == CubeAnimationState.IDLE_AS_FOLDED || AnimationState == CubeAnimationState.FOLDING)
		{
			//For Rotating the cube by one finger 
			m_OneFingerCubeRotation = Quaternion.Lerp(m_OneFingerCubeRotation, Quaternion.identity, Time.deltaTime * m_OneFingerRotationAnimationSpeed);
			transform.Rotate(m_OneFingerCubeRotation.eulerAngles, Space.World);
		}
		else if (AnimationState == CubeAnimationState.IDLE_AS_FOCUSED && SideFocused == CubeSideType.TITLE) {
			//Passage animation
			DragOneFingerOnPassageOnUpdate();
		}
	}
	
	public void DragOneFingerOnSide(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture)
	{
		if (AnimationState == CubeAnimationState.IDLE_AS_FOCUSED && SideFocused != CubeSideType.NONE)
		{
			GameObject currentSideObject = m_presentationSide[(int)SideFocused];

			Ray rayForDrag = Camera.main.ScreenPointToRay(OneFingerManipulationGesture.ScreenPosition);
			RaycastHit hit;
			bool isHitOnFocusedSide = Physics.Raycast(rayForDrag, out hit, Mathf.Infinity, 1 << currentSideObject.layer);

			if(isHitOnFocusedSide){

				int touchedSide = -1;
				int.TryParse(hit.transform.name.Substring(1, 1), out touchedSide);
				CubeSideType cubeSideTouched = (CubeSideType)touchedSide;

				Log.Status("CubeAnimationManager", "cubeSideTouched: {0}", cubeSideTouched);
				if(cubeSideTouched == CubeSideType.TITLE && SideFocused == CubeSideType.TITLE)
				{
					m_LastFrameOneFingerDrag = Time.frameCount;
					DragOneFingerOnPassage(OneFingerManipulationGesture);
				}
			}

		}
	}

	private Transform[] m_PassageItems = null;

	//[SerializeField]
	private float m_OneDragModifier = 0.01f;
	private int m_SelectedPassageIndex = -1;
	private float m_PercentCurrentPassage = 0.0f;

	private Vector3 worldOnPath = Vector3.up;

	public void DragOneFingerOnPassage(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture){

		if (m_PassageItems == null) {
			m_PassageItems = Utility.FindObjects<Transform> (m_ProjectionSide [5], "PassageItem", isContains: true, sortByName: true);
		}

		if (m_PassageItems != null) {
			float movingInX = OneFingerManipulationGesture.DeltaPosition.x * m_OneDragModifier;
			//Log.Status("CubeAnimationManager", "Passages: {0} - DragOneFingerOnPassage: {1}", m_PassageItems.Length, OneFingerManipulationGesture.DeltaPosition);
//			for (int i = 0; i < m_PassageItems.Length; i++) {
//				Log.Status("CubeAnimationManager", "Passages: {0} - List[{1}] = {2}", m_PassageItems.Length, i, m_PassageItems[i].name);
//			}

			if(m_SelectedPassageIndex != -1){

			}
			else{

				m_PercentCurrentPassage = Mathf.Clamp01(m_PercentCurrentPassage + movingInX);


				//new Vector3[]{ new Vector3(0f,0f,0f), new Vector3(1f,1f,1f), new Vector3(2f,2f,2f), new Vector3(3f,3f,3f), new Vector3(3f,3f,3f), new Vector3(4f,4f,4f), new Vector3(5f,5f,5f), new Vector3(6f,6f,6f) }
				//LTSpline pathSplineOrientation = new LTSpline( Vector3.zero, new Vector3(45.0f,45.0f,0), new Vector3(45.0f,45.0f,0), Vector3.zero );
				//path.orientToPath = true;
				//path.placeLocal(m_PassageItems[0].transform, m_PercentCurrentPassage, worldOnPath);
				//m_PassageItems[0].transform.GetComponent<RectTransform>().anchoredPosition3D = path.point(m_PercentCurrentPassage);
				//m_PassageItems[0].transform.GetComponent<RectTransform>().localEulerAngles = pathOrientation.point(m_PercentCurrentPassage);

			}


		} else {
			Log.Status("CubeAnimationManager", "NO PASSAGE - DragOneFingerOnPassage: {0}", OneFingerManipulationGesture.DeltaPosition);
		}
	}

	//[SerializeField]
	private float sleepPassageSmooth = 4.0f;

	private void DragOneFingerOnPassageOnUpdate(){

		if (m_PassageItems != null && m_PassageItems[0] != null) {

			if((Time.frameCount - m_LastFrameOneFingerDrag) > 2 ){
				if(m_PercentCurrentPassage >0.25f && m_PercentCurrentPassage < 0.75f){
					m_PercentCurrentPassage = 0.5f;
				}
				else if(m_PercentCurrentPassage < 0.25f){
					m_PercentCurrentPassage = 0.0f;
				}
				else{
					m_PercentCurrentPassage = 1.0f;
				}
			}
				


			LTBezierPath m_BezierPathCurrent;
			LTBezierPath m_BezierPathOrientationCurrent;
			float m_RatioBezierPathPassage = 0.0f;
			if (m_PercentCurrentPassage <= 0.5f) {
				m_RatioBezierPathPassage = m_PercentCurrentPassage * 2.0f;
				m_BezierPathCurrent = m_BezierPathToCenter;
				m_BezierPathOrientationCurrent = m_BezierPathOrientationToCenter;
			} else {
				m_RatioBezierPathPassage = (m_PercentCurrentPassage - 0.5f) * 2.0f;
				m_BezierPathCurrent = m_BezierPathToStack;
				m_BezierPathOrientationCurrent = m_BezierPathOrientationToStack;
			}
			
			m_PassageItems[0].transform.localPosition = Vector3.Lerp(m_PassageItems[0].transform.localPosition, m_BezierPathCurrent.point (m_RatioBezierPathPassage), Time.deltaTime * sleepPassageSmooth);
			m_PassageItems[0].transform.localRotation = Quaternion.Lerp(m_PassageItems[0].transform.localRotation, Quaternion.Euler( m_BezierPathOrientationCurrent.point (m_RatioBezierPathPassage)), Time.deltaTime * sleepPassageSmooth);
		}
	}
	
	#endregion
	
	
}
