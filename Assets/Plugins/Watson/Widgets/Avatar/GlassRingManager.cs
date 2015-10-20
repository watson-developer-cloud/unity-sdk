using UnityEngine;
using System.Collections;
using IBM.Watson.AdaptiveComputing;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Avatar
{
	public class GlassRingManager : MonoBehaviour {

		Material glassRingMaterial;
		Color initialColor;
		void OnEnable(){
			EventManager.Instance.RegisterEventReceiver (EventManager.onMoodChangeFinish, OnChangedMood);
			EventManager.Instance.RegisterEventReceiver (EventManager.onBehaviorChangeFinish, OnChangedBehavior);
		}
		
		void OnDisable(){
			EventManager.Instance.UnregisterEventReceiver (EventManager.onMoodChangeFinish, OnChangedMood);
			EventManager.Instance.UnregisterEventReceiver (EventManager.onBehaviorChangeFinish, OnChangedBehavior);

			if (glassRingMaterial != null) {
				glassRingMaterial.SetColor("_SpecColor", initialColor);
			}
		}
		// Use this for initialization
		void Awake () {
			MeshRenderer childMeshRenderer = transform.GetComponentInChildren<MeshRenderer> ();
			if (childMeshRenderer != null) {
				glassRingMaterial = childMeshRenderer.sharedMaterial;
				initialColor = glassRingMaterial.GetColor("_SpecColor");
			} else {
				Log.Error("GlassRingManager", "There is not mesh renderer in any child object");
			}
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void OnChangedMood(System.Object[] args){
			ChangeToColor (MoodManager.Instance.currentMoodColor);
		} 

		public void OnChangedBehavior(System.Object[] args){
			ChangeToColor (BehaviorManager.Instance.currentBehaviourColor);
		}

		LTDescr colorAnimationOnGlass = null;
		public void ChangeToColor(Color color){
			if (glassRingMaterial != null) {
				if (colorAnimationOnGlass != null) {
					LeanTween.cancel(colorAnimationOnGlass.uniqueId);
				}
				
				colorAnimationOnGlass = LeanTween.value (gameObject, Color.white, color, 1.0f * MoodManager.Instance.currentMoodTimeModifier).setLoopPingPong ().setOnUpdateColor(
					(Color a)=>{
					glassRingMaterial.SetColor("_SpecColor", a);
				});
			}
		}
	}

}
