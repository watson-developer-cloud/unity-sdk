using UnityEngine;
using System.Collections;

using IBM.Watson.Utilities;

/// <summary>
/// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.iu8889
/// </summary>
public class WatsonCamera : MonoBehaviour {

	private bool m_isAnimationPaused = false;
	void OnEnable(){
		//TODO: Delete All these stuff! Add Keyboard widget
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.S, KeyModifiers.NONE, AnimationSpeedUp);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.D, KeyModifiers.NONE, AnimationSpeedDown);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.R, KeyModifiers.NONE, AnimationSpeedDefault);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.O, KeyModifiers.NONE, AnimationResume);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.P, KeyModifiers.NONE, AnimationPause);
	}

	void OnDisable(){
		//TODO: Delete All these stuff! Add Keyboard widget
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.S, KeyModifiers.NONE, AnimationSpeedUp);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.D, KeyModifiers.NONE, AnimationSpeedDown);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.R, KeyModifiers.NONE, AnimationSpeedDefault);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.O, KeyModifiers.NONE, AnimationResume);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.P, KeyModifiers.NONE, AnimationPause);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
}
