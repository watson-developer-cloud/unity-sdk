using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

/// <summary>
/// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.iu8889
/// </summary>
public class WatsonCamera : MonoBehaviour {

	#region Private Variables
	private bool m_isAnimationPaused = false;
	private Vector3 m_TargetCameraLocation;
    private Vector3 m_CameraInitialLocation;
	[SerializeField]
	private float m_PanSpeed = 0.07f;
	[SerializeField]
	private float m_ZoomSpeed = 20.0f;
	[SerializeField]
	private float m_SpeedForCameraAnimation = 2f;
    [SerializeField]
    private LeanTweenType m_EaseCameraReset = LeanTweenType.easeInOutCubic;
  
    #endregion

    #region OnEnable / OnDisable to register some events

    void OnEnable(){
        m_CameraInitialLocation = transform.localPosition;
        m_TargetCameraLocation = m_CameraInitialLocation;
        TouchEventManager.Instance.RegisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
        EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
	}

	void OnDisable(){
		TouchEventManager.Instance.UnregisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
        EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, ResetCameraPosition);
    }

	#endregion

	#region Touch Drag Actions
	public void DragTwoFinger(TouchScript.Gestures.ScreenTransformGesture transformGesture){

        Log.Status("WatsonCamera", "twoFingerTransformHandler: {0} , DeltaScale: {1}, PanSpeed: {2}, ZoomSpeed:{3}",
            transformGesture.DeltaPosition,
            transformGesture.DeltaScale,
            m_PanSpeed,
            m_ZoomSpeed);

        //Pannning with 2-finger
        m_TargetCameraLocation += (transformGesture.DeltaPosition * m_PanSpeed * -1.0f);
        //Zooming with 2-finger
		m_TargetCameraLocation += transform.forward * (transformGesture.DeltaScale - 1.0f) * m_ZoomSpeed;
	}

	void Update(){
		//For Zooming and Panning
		transform.localPosition = Vector3.Lerp(transform.localPosition, m_TargetCameraLocation, Time.deltaTime * m_SpeedForCameraAnimation);
	}

    #endregion

    #region Camera Events Received from Outside - Set default position 
    void ResetCameraPosition(System.Object[] args)
    {
        Log.Status("WatsonCamera", "Reset Camera Position");
        m_TargetCameraLocation = m_CameraInitialLocation;
    }

    #endregion

    #region Application Related Actions - Methods to call 

    public void AnimationSpeedUp(){
		EventManager.Instance.SendEvent (Constants.Event.ON_ANIMATION_SPEED_UP);
	}

	public void AnimationSpeedDown(){
		EventManager.Instance.SendEvent (Constants.Event.ON_ANIMATION_SPEED_DOWN);
	}

	public void AnimationSpeedDefault(){
		EventManager.Instance.SendEvent (Constants.Event.ON_ANIMATION_SPEED_DEFAULT);
	}

	public void AnimationPause(){
		EventManager.Instance.SendEvent (Constants.Event.ON_ANIMATION_PAUSE);
	}

	public void AnimationResume(){
		EventManager.Instance.SendEvent (Constants.Event.ON_ANIMATION_RESUME);
	}

	public void AnimationPauseResume(){
		m_isAnimationPaused = !m_isAnimationPaused;

		if(m_isAnimationPaused)
			AnimationPause();
		else
			AnimationResume();
	}

	public void ApplicationQuit(){
		Application.Quit ();
	}

	#endregion
}
