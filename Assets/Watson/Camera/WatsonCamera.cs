using UnityEngine;
using System.Collections;

using IBM.Watson.Utilities;

/// <summary>
/// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.iu8889
/// </summary>
public class WatsonCamera : MonoBehaviour {

	void OnEnable(){
		//TODO: Delete All these stuff! Add Keyboard widget
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.S, AnimationSpeedUp);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.D, AnimationSpeedDown);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.R, AnimationSpeedDefault);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.O, AnimationResume);
		KeyEventManager.Instance.RegisterKeyEvent (KeyCode.P, AnimationPause);
	}

	void OnDisable(){
		//TODO: Delete All these stuff! Add Keyboard widget
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.S, AnimationSpeedUp);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.D, AnimationSpeedDown);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.R, AnimationSpeedDefault);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.O, AnimationResume);
		KeyEventManager.Instance.UnregisterKeyEvent (KeyCode.P, AnimationPause);
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
}
