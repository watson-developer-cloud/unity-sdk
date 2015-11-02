using UnityEngine;
using System.Collections;

using IBM.Watson.Utilities;

/// <summary>
/// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.iu8889
/// </summary>
public class WatsonCamera : MonoBehaviour {

	#region Private Variables
	private bool m_isAnimationPaused = false;
	private Vector3 m_TargetCameraLocation;
	[SerializeField]
	private float m_PanSpeed = 100.0f;
	[SerializeField]
	private float m_ZoomSpeed = 0.1f;
	[SerializeField]
	private float m_SpeedForCameraAnimation = 1f;
	private Transform m_MainCamera;
	#endregion

	#region OnEnable / OnDisable to register some events

	void OnEnable(){
		m_MainCamera = Camera.main.transform;
		TouchEventManager.Instance.RegisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
	}

	void OnDisable(){
		TouchEventManager.Instance.UnregisterDragEvent (gameObject, DragTwoFinger, numberOfFinger: 2);
	}

	#endregion

	#region Touch Drag Actions
	public void DragTwoFinger(TouchScript.Gestures.ScreenTransformGesture transformGesture){
		m_TargetCameraLocation += (transformGesture.DeltaPosition * m_PanSpeed * -1.0f);
		m_TargetCameraLocation += m_MainCamera.transform.forward * (transformGesture.DeltaScale - 1.0f) * m_ZoomSpeed;
	}

	void Update(){
		
		//For Zooming
		m_MainCamera.transform.localPosition = Vector3.Lerp(m_MainCamera.transform.localPosition, m_TargetCameraLocation, Time.deltaTime * m_SpeedForCameraAnimation);
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
