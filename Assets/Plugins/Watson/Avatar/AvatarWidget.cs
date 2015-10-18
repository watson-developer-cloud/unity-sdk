using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Avatar;

namespace IBM.Watson.Widgets
{
	/// <summary>
	/// Avatar of Watson 
	/// </summary>
	public class AvatarWidget : Widget {

		#region Singleton Instance
		public static AvatarWidget Instance;
		#endregion

		#region Widget Name

		protected override string GetName (){
			return "Avatar";
		}

		#endregion

		#region Pebble Manager for Visualization
		private PebbleManager m_pebbleManager;

		/// <summary>
		/// Gets the pebble manager. Sound Visualization on the avatar. 
		/// </summary>
		/// <value>The pebble manager.</value>
		public PebbleManager pebbleManager{
			get{
				m_pebbleManager = transform.GetComponentInChildren<PebbleManager> ();
				if (m_pebbleManager == null) {
					Log.Error("AvatarManager", "PebbleManager couldn't found!");
				}

				return m_pebbleManager;
			}
		}
		#endregion


		#region Public Variables

		public float soundVisualizerModifier = 1.0f;
		public MicrophoneWidget microphoneWidget;

		#endregion

		#region Initialization
		void Awake(){
			Instance = this;
			MoodManager.Instance.currentMood = MoodType.Idle;
			BehaviorManager.Instance.currentBehavior = BehaviorType.Idle;
		}
		
		// Use this for initialization
		void Start () {
			microphoneWidget.Active = true;
		}

		#endregion

		#region AudioData Input

		/// <summary>
		/// Sets the audio data for Audio visualization on Avatar
		/// </summary>
		/// <param name="value">Value.</param>
		public static void SetAudioData(float value){
			Instance.pebbleManager.SetAudioData (value);
		}

		public Input m_levelInput = new Input("LevelInput", typeof(FloatData), "SetAudioFloatData");
		public void SetAudioFloatData(Widget.Data levelInputFloatData){
			FloatData levelInput = (FloatData)levelInputFloatData;
			SetAudioData(levelInput.Float * soundVisualizerModifier);
		}
		


		#endregion



	}
}
