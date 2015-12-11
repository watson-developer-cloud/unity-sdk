using UnityEngine;
using System.Collections;

namespace IBM.Watson.Utilities
{
	/// <summary>
	/// General application related event handlers and some configurations
	/// </summary>
	public class AppController : MonoBehaviour {

		#region Private Members
		private bool m_isAnimationPaused = false;
		#endregion

		#region Start
		// Use this for initialization
		void Start () {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		#endregion

		#region Input / Output Related
		
		/// <summary>
		/// Shows the virtual keyboard.
		/// </summary>
		/// <param name="args">Arguments.</param>
		public void ShowVirtualKeyboard(System.Object[] args)
		{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			try
			{
				System.Diagnostics.Process.Start("TabTip.exe");
			}
			catch (System.Exception e)
			{
				Log.Error("WatsonCamera", "ShowVirtualKeyboard has exception: {0}", e.Message);
			}
			#elif UNITY_IOS || UNITY_ANDROID
			TouchScreenKeyboard.Open("",TouchScreenKeyboardType.Default, false, false, false,false,"");
			#endif
		}


		#endregion

		#region Animation Related General Methods

		/// <summary>
		/// Event firing to speed up animation in entire app
		/// </summary>
		public void AnimationSpeedUp()
		{
			EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_UP);
		}
		
		/// <summary>
		/// Event firing to speed down animation in entire app
		/// </summary>
		public void AnimationSpeedDown()
		{
			EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_DOWN);
		}

		/// <summary>
		/// Event firing to speed set default animation in entire app
		/// </summary>
		public void AnimationSpeedDefault()
		{
			EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_SPEED_DEFAULT);
		}

		/// <summary>
		/// Event firing to pause all animations 
		/// </summary>
		public void AnimationPause()
		{
			EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_PAUSE);
		}

		/// <summary>
		/// Event firing to resume all animations paused
		/// </summary>
		public void AnimationResume()
		{
			EventManager.Instance.SendEvent(Constants.Event.ON_ANIMATION_RESUME);
		}

		/// <summary>
		/// Event firing to pause / resume all animations paused
		/// </summary>
		public void AnimationPauseResume()
		{
			m_isAnimationPaused = !m_isAnimationPaused;
			
			if (m_isAnimationPaused)
				AnimationPause();
			else
				AnimationResume();
		}
		
		#endregion

		#region Application General

		/// <summary>
		/// Event handler to application quit
		/// </summary>
		/// <param name="args">Arguments.</param>
		public void ApplicationQuit(System.Object[] args = null)
		{
			Application.Quit();
		}

		#endregion
	}
}
