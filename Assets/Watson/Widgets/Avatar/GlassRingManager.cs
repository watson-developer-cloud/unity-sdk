/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

using UnityEngine;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// GlassRingManager manages the Avatar's 3D models' ring material color according to mood and behavior change.
    /// </summary>
	public class GlassRingManager : BaseRingManager
    {

        #region Private Variables
        [SerializeField]
        private float m_AnimationTime = 1.0f;
        private Material m_GlassRingMaterial;
        private Color m_InitialColorOfGlassRingMaterial;
        private int m_ColorAnimationOnGlass = -1;
		private int m_ColorAnimationOnGlassLoop = -1;
        private Color m_LastColorUsedInAnimation = Color.white;
        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        private void OnApplicationQuit()
        {
            if (m_GlassRingMaterial != null)
            {
                m_GlassRingMaterial.SetColor("_SpecColor", m_InitialColorOfGlassRingMaterial);
            }
        }

        /// <exclude />
		protected override void Start()
        {
			base.Start ();

			if(m_AvatarWidgetAttached == null)
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

			if (m_AvatarWidgetAttached != null) {
				ChangedBehavior(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.BehaviorTimeModifier);
			}

        }

        #endregion

        #region Changing Mood / Avatar State

        /// <summary>
        /// Event handler for ON_CHANGE_AVATAR_MOOD_FINISH
        /// </summary>
        /// <param name="color"></param>
        /// <param name="timeModifier"></param>
        public override void ChangedBehavior(Color color, float timeModifier)
        {
            if (m_GlassRingMaterial != null)
            {

				StopAnimationGlassRingColor();

                m_ColorAnimationOnGlass = LeanTween.value(gameObject, m_LastColorUsedInAnimation, color, m_AnimationTime).setOnUpdateColor(
                    (Color colorToFadeIn) =>
                    {
                        m_LastColorUsedInAnimation = colorToFadeIn;
                        m_GlassRingMaterial.SetColor("_SpecColor", colorToFadeIn);
                    }).setOnComplete(
                    (System.Object o) =>
                    {
						
						m_ColorAnimationOnGlass = -1;
						
						if(o is Color){
							Color colorToBe = (Color) o;
							m_ColorAnimationOnGlassLoop = LeanTween.value(gameObject, colorToBe, Color.white, m_AnimationTime * timeModifier).setLoopPingPong().setOnUpdateColor(
								(Color colorToLoop) =>
								{
									m_GlassRingMaterial.SetColor("_SpecColor", colorToLoop);
									m_LastColorUsedInAnimation = colorToLoop;
								}).id;
						}
                       

                    }, color).id;

            }
        }

		private void StopAnimationGlassRingColor(){
			if (LeanTween.descr(m_ColorAnimationOnGlass) != null)
			{
				LeanTween.descr(m_ColorAnimationOnGlass).onComplete = null;
				LeanTween.cancel(m_ColorAnimationOnGlass);
				m_ColorAnimationOnGlass = -1;
			}
			
			
			if (LeanTween.descr(m_ColorAnimationOnGlassLoop) != null)
			{
				LeanTween.descr(m_ColorAnimationOnGlassLoop).setLoopOnce();
				LeanTween.descr(m_ColorAnimationOnGlassLoop).onUpdateColor = null;
				LeanTween.cancel(m_ColorAnimationOnGlassLoop);
				m_ColorAnimationOnGlassLoop = -1;
			}
		}

        #endregion
    }

}
