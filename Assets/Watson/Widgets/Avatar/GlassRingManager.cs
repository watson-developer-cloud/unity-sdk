using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// GlassRingManager manages the Avatar's 3D models' ring material color according to mood and behavior change.
    /// </summary>
	public class GlassRingManager : AvatarModelManager
    {

        #region Private Variables
        Material m_GlassRingMaterial;
        Color m_InitialColorOfGlassRingMaterial;
        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        void OnApplicationQuit()
        {
            if (m_GlassRingMaterial != null)
            {
                m_GlassRingMaterial.SetColor("_SpecColor", m_InitialColorOfGlassRingMaterial);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();
            if (m_AvatarWidgetAttached != null)
            {
                MeshRenderer childMeshRenderer = transform.GetComponentInChildren<MeshRenderer>();
                if (childMeshRenderer != null)
                {
                    m_GlassRingMaterial = childMeshRenderer.sharedMaterial;
                    m_InitialColorOfGlassRingMaterial = m_GlassRingMaterial.GetColor("_SpecColor");
                }
                else
                {
                    Log.Error("GlassRingManager", "There is not mesh renderer in any child object");
                    this.enabled = false;
                }
            }
            else
            {
                Log.Error("GlassRingManager", "There is no Avatar Widget on any parent.");
                this.enabled = false;
            }

        }

        #endregion

        #region Changing Mood / Avatar State

		LTDescr colorAnimationOnGlass = null;
        LTDescr colorAnimationOnGlassLoop = null;
		Color lastColor = Color.white;
        public override void ChangedBehavior(Color color, float timeModifier)
        {
            if (m_GlassRingMaterial != null)
            {
				if(colorAnimationOnGlass != null){
					LeanTween.cancel(colorAnimationOnGlass.uniqueId);
				}

                if (colorAnimationOnGlassLoop != null)
                {
                    LeanTween.cancel(colorAnimationOnGlassLoop.uniqueId);
                }

				colorAnimationOnGlass = LeanTween.value(gameObject, lastColor, color, timeModifier).setOnUpdateColor(
					(Color colorToFadeIn)=>{
						
						m_GlassRingMaterial.SetColor("_SpecColor", colorToFadeIn);

					}).setOnComplete(
					()=>{

						colorAnimationOnGlassLoop = LeanTween.value(gameObject, Color.white, color, timeModifier).setLoopPingPong().setOnUpdateColor(
						(Color colorToLoop) =>
						{
							m_GlassRingMaterial.SetColor("_SpecColor", colorToLoop);
							lastColor = colorToLoop;
						});	

					});

                
            }
        }

        #endregion
    }

}
