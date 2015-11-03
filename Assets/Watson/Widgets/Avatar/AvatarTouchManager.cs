using UnityEngine;
using System.Collections;
using TouchScript.Gestures;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets.Question;

public class AvatarTouchManager: MonoBehaviour
{
    private GameObject m_cubeObject;
    public GameObject cubeObject
    {
        get
        {
            if(m_cubeObject == null || m_cubeObject.transform == null)
            {
                GameObject[] questionObjects= GameObject.FindGameObjectsWithTag("QuestionOnFocus");
                for (int i = 0; i < questionObjects.Length; i++)
                {
                    if(questionObjects[i] != null && questionObjects[i].transform != null) {
                        m_cubeObject = questionObjects[i].GetComponentInChildren<CubeAnimationManager>().gameObject;
                        break;
                    }
                }
            }

            return m_cubeObject;
        }
    }
    
    public ScreenTransformGesture OneFingerManipulationGesture;
    public ScreenTransformGesture TwoFingerMoveGesture;
//    public TapGesture tapGesture;

    public LayerMask layerForQuestionWidget;
    public float PanSpeed = 100.0f;
    public float RotationSpeed = 100.0f;
    public float ZoomSpeed = 0.1f;

    public LeanTweenType easeCameraReset = LeanTweenType.easeInOutCubic;
    public float timeForCameraResetAnimation = 1.0f;
    public float speedForCameraAnimation = 1f;
    public float speedForCubeRotationAnimation = 1f;

    private Vector3 initialCameraLocation;
    private Vector3 targetCameraLocation;
    private Quaternion targetCubeRotation;

    private bool isActive = true;
    private GameObject mainCamera;

    private void OnEnable()
    {
        Log.Status("AvatarTouchManager", "OnEnable");
        mainCamera = Camera.main.gameObject;
        initialCameraLocation = mainCamera.transform.localPosition;
        targetCameraLocation = initialCameraLocation;
        targetCubeRotation = Quaternion.identity;
        
        TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
        OneFingerManipulationGesture.Transformed += oneFingerManipulationTransformedHandler;
//        tapGesture.Tapped += TapGesture_Tapped;

        EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, CubeAnimationStateChanged);
    }

	private void OnDisable()
	{
		Log.Status("AvatarTouchManager", "OnDisable");
		
		TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
		OneFingerManipulationGesture.Transformed -= oneFingerManipulationTransformedHandler;
//		tapGesture.Tapped -= TapGesture_Tapped;

		EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, CubeAnimationStateChanged);
	}

   
    private void FocusOnSide(Transform hitTransform)
    {
        int touchedSide = 0;
        int.TryParse(hitTransform.name.Substring(1, 1), out touchedSide);
        CubeAnimationManager.Instance.FocusOnSide((CubeAnimationManager.CubeSideType)touchedSide);
    }

//    private void TapGesture_Tapped(object sender, System.EventArgs e)
//    {
//        if (CubeAnimationManager.Instance == null)
//            return;
//
//       Log.Status("AvatarTouchManager", "TapGesture_Tapped: {0}", tapGesture.ScreenPosition);
//        Ray rayForTab = mainCamera.GetComponent<Camera>().ScreenPointToRay(tapGesture.ScreenPosition);
//
//        RaycastHit hit;
//        if (Physics.Raycast(rayForTab, out hit, Mathf.Infinity, layerForQuestionWidget))
//        {
//            //Touch on side
//            switch (CubeAnimationManager.Instance.AnimationState)
//            {
//                case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
//                    CubeAnimationManager.Instance.UnFold();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.UNFOLDING:
//                    FocusOnSide(hit.transform);
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
//                    FocusOnSide(hit.transform);
//                    break;
//                case CubeAnimationManager.CubeAnimationState.FOLDING:
//                    CubeAnimationManager.Instance.UnFold();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
//                    FocusOnSide(hit.transform);
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
//                    FocusOnSide(hit.transform);
//                    break;
//                case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
//                    break;
//                default:
//                    break;
//            }
//        }
//        else
//        {
//            Log.Status("AvatarTouchManager", "Touch-outside current state: " + CubeAnimationManager.Instance.AnimationState);
//            //Touch out-side
//            switch (CubeAnimationManager.Instance.AnimationState)
//            {
//                case CubeAnimationManager.CubeAnimationState.NOT_PRESENT:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.COMING_TO_SCENE:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.UNFOLDING:
//                    CubeAnimationManager.Instance.Fold();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_UNFOLDED:
//                    CubeAnimationManager.Instance.Fold();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.FOLDING:
//                    break;
//                case CubeAnimationManager.CubeAnimationState.FOCUSING_TO_SIDE:
//                    CubeAnimationManager.Instance.UnFocus();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED:
//                    CubeAnimationManager.Instance.UnFocus();
//                    break;
//                case CubeAnimationManager.CubeAnimationState.GOING_FROM_SCENE:
//                    break;
//                default:
//                    break;
//            }
//        }
//       
//    }

   
    private void oneFingerManipulationTransformedHandler(object sender, System.EventArgs e)
    {
        //Log.Status("AvatarTouchManager", "oneFingerManipulationTransformedHandler: {0}" , OneFingerManipulationGesture.DeltaPosition);

        //Quaternion rotation = Quaternion.Euler( OneFingerManipulationGesture.DeltaPosition.y / Screen.height * RotationSpeed,
        //                                        -OneFingerManipulationGesture.DeltaPosition.x / Screen.width * RotationSpeed,
        //                                        0.0f);

        //targetCubeRotation *= rotation;
        //Log.Status("AvatarTouchManager", "Rotation: {0} , Target rotation : {1} ", rotation.eulerAngles, targetCubeRotation.eulerAngles);


       // cubeObject.transform.rotation *= rotation;
        //cam.LookAt (camTarget.transform.position);

    }

    private void twoFingerTransformHandler(object sender, System.EventArgs e)
    {
        //Log.Status("AvatarTouchManager", "twoFingerTransformHandler: {0} , DeltaScale: {1}, PanSpeed: {2}, ZoomSpeed:{3}", 
        //    TwoFingerMoveGesture.DeltaPosition, 
        //    TwoFingerMoveGesture.DeltaScale,
        //    PanSpeed,
        //    ZoomSpeed);
        
       // targetCameraLocation += (TwoFingerMoveGesture.DeltaPosition * PanSpeed * -1.0f);
        
       // targetCameraLocation += mainCamera.transform.forward * (TwoFingerMoveGesture.DeltaScale - 1.0f) * ZoomSpeed;
    }

    private void CubeAnimationStateChanged(System.Object[] args)
    {
        isActive = false;
        targetCameraLocation = initialCameraLocation;
        targetCubeRotation = Quaternion.identity;
        //Reset all camera animation.
        //LeanTween.moveLocal(mainCamera.gameObject, initialCameraLocation, timeForCameraResetAnimation).setEase(easeCameraReset).setOnComplete(()=>
        //{
        //    isActive = true;
        //});
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha3))
            PanSpeed -= 0.1f;
        if (Input.GetKey(KeyCode.Alpha4))
            PanSpeed += 0.1f;
        if (Input.GetKey(KeyCode.Alpha5))
            ZoomSpeed -= 0.1f;
        if (Input.GetKey(KeyCode.Alpha6))
            ZoomSpeed += 0.1f;

        if (isActive && CubeAnimationManager.Instance != null)
        {
    //        if (CubeAnimationManager.Instance.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED)
    //        {
    //            //For Rotating the cube
    //            targetCubeRotation = Quaternion.Lerp(targetCubeRotation, Quaternion.identity, Time.deltaTime * speedForCubeRotationAnimation);
				//if(cubeObject != null)
    //            	cubeObject.transform.Rotate(targetCubeRotation.eulerAngles, Space.World);
    //        }

            //For Zooming
            //mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, targetCameraLocation, Time.deltaTime * speedForCameraAnimation);
        }


    }

}
