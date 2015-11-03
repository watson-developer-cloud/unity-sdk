using UnityEngine;
using System.Collections;

using IBM.Watson.Utilities;

/// <summary>
/// Watson camera. The main responsible camera on the scene of the Watson applications which handles the camera movements via touch / keyboard inputs / voice commands.iu8889
/// </summary>
public class WatsonCamera : MonoBehaviour {

	#region Private Variables
	private bool m_isAnimationPaused = false;
	#endregion

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
