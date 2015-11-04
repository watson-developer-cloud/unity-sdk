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
using IBM.Watson.Utilities;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// LightRingManager manages the Avatar 3d models' animated light rings according to mood / behavior change. 
    /// </summary>
	public class LightRingManager : BaseRingManager
    {

        #region Private Variables

        [SerializeField]
        private LeanTweenType m_LightFlareEase = LeanTweenType.easeInOutQuad;

        [SerializeField]
        private float m_AnimationTime = 1.0f;

        private int m_NumberOfLightFlare = 3;

        [SerializeField]
        private GameObject[] m_LightFlarePivotParentList;
        private GameObject[] m_LightFlareChild;
        private Vector3[][] m_ListFlareBezierPathList;

        private Material m_SharedMaterialLightFlare;
        private Color m_TintColorSharedMaterialLightFlareInitial = Color.white;
        private Color m_ColorAnimationFlareLast = Color.white;

        private LTDescr m_ColorAnimationOnRing = null;
        private LTDescr m_ColorAnimationOnFlareInitial = null;
        private LTDescr m_ColorAnimationOnFlareLoop = null;
        private LTDescr[] m_MoveAnimationOnFlare = null;

        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        private void OnApplicationQuit()
        {
            if (m_SharedMaterialLightFlare != null)
            {
                m_SharedMaterialLightFlare.SetColor("_TintColor", m_TintColorSharedMaterialLightFlareInitial);
            }
        }

        /// <exclude />
        protected override void Start()
        {
            base.Start();

            m_ListFlareBezierPathList = new Vector3[3][];

            m_ListFlareBezierPathList[0] = new Vector3[] {
                new Vector3 (28.365f, -6.06f, -16.385f),
                new Vector3 (22.58f, -6.06f, 32.78f),
                new Vector3 (41.7f, -6.06f, 8.4f),
                new Vector3 (0.062f, -6.06f, 32.764f)
            };

            m_ListFlareBezierPathList[1] = new Vector3[] {
                new Vector3 (-28.37233f, -6.06f, -16.3723f),
                new Vector3 (17.0983f, -6.06f, -35.94485f),
                new Vector3 (-13.57539f, -6.06f, -40.31325f),
                new Vector3 (28.34345f, -6.06f, -16.4357f)
            };

            m_ListFlareBezierPathList[2] = new Vector3[] {
                new Vector3 (0.007372856f, -6.060182f, 32.75727f),
                new Vector3 (-39.67829f, -6.060145f, 3.164852f),
                new Vector3 (-28.12456f, -6.060268f, 31.91325f),
                new Vector3 (-28.40545f, -6.06f, -16.32832f)
            };

            if (m_LightFlarePivotParentList == null || m_LightFlarePivotParentList.Length == 0 || m_LightFlarePivotParentList[0] == null)
            {
                m_LightFlarePivotParentList = new GameObject[m_NumberOfLightFlare];
                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_LightFlarePivotParentList[i] = 
                        Utility.FindObject(this.gameObject, string.Concat((i + 1).ToString(), "/LightFlareParent"));
                }
            }


            if (m_LightFlarePivotParentList != null)
            {
                m_LightFlareChild = new GameObject[m_LightFlarePivotParentList.Length];
                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_LightFlareChild[i] = m_LightFlarePivotParentList[i].GetComponentInChildren<MeshRenderer>().gameObject;
                }
                m_SharedMaterialLightFlare = m_LightFlareChild[0].transform.GetComponent<MeshRenderer>().sharedMaterial;
                m_TintColorSharedMaterialLightFlareInitial = m_SharedMaterialLightFlare.GetColor("_TintColor");
            }
        }

        #endregion

        #region Change Mood / Behavior 

        /// <summary>
        /// After Mood Change, this function gets called. 
        /// It changes light flare movement on mood change.
        /// </summary>
        /// <param name="colorToChange">Color to change.</param>
        /// <param name="speedModifier">Speed modifier.</param>
        /// <param name="timeModifier">Time modifier.</param>
        public override void ChangedMood(Color colorToChange, float timeModifier)
        {
            //We are changing movement in flare in mood change! (not color, color change depends on behavior)
            AnimateLightFlareMovement(m_AnimationTime * timeModifier);
        }

        /// <summary>
        /// On Behavior Change, this function gets called. 
        /// Light Ring and Flare colors are changing on behavior change on avatar.
        /// </summary>
        /// <param name="colorToChange">Color to be used in animations of Avatar Model</param>
        /// <param name="speedModifier">Speed modifier to be used in animations of Avatar Model</param>
        /// <param name="color">Color.</param>
        /// <param name="timeModifier">Time modifier.</param>
        public override void ChangedBehavior(Color color, float timeModifier)
        {
            AnimateLightRingColor(color, m_AnimationTime * timeModifier);
            AnimateLightFlareColor(color, m_AnimationTime * timeModifier);
        }

        #endregion

        #region Animations on Light Ring 

        private void AnimateLightRingColor(Color color, float animationTime)
        {
            if (m_ColorAnimationOnRing != null)
            {
                LeanTween.cancel(m_ColorAnimationOnRing.uniqueId);
            }

            m_ColorAnimationOnRing = LeanTween.color(gameObject, color, animationTime); //.setFromColor (Color.white).setLoopPingPong ();
        }

        private void AnimateLightFlareColor(Color color, float animationTime)
        {
            if (m_SharedMaterialLightFlare == null)
            {
                Log.Warning("LightRingManager", "AnimateLightFlareColor : light flare should have a shared material.");
                return;
            }

            if (m_ColorAnimationOnFlareLoop != null)
            {
                LeanTween.cancel(m_ColorAnimationOnFlareLoop.uniqueId);
            }

            if (m_ColorAnimationOnFlareInitial != null)
            {
                LeanTween.cancel(m_ColorAnimationOnFlareInitial.uniqueId);
            }

            m_ColorAnimationOnFlareInitial = LeanTween.value(gameObject, m_ColorAnimationFlareLast, color, animationTime).setOnUpdateColor(
                (Color colorToFadeIn) =>
                {
                    m_ColorAnimationFlareLast = colorToFadeIn;
                    m_SharedMaterialLightFlare.SetColor("_TintColor", colorToFadeIn);
                }).setOnComplete(
                () =>
                {

                    m_ColorAnimationOnFlareLoop = LeanTween.value(gameObject, color, Color.white, animationTime).setLoopPingPong().setOnUpdateColor(
                        (Color colorToLoop) =>
                        {
                            m_SharedMaterialLightFlare.SetColor("_TintColor", colorToLoop);
                            m_ColorAnimationFlareLast = colorToLoop;
                        });

                });
        }

        private void AnimateLightFlareMovement(float animationTime)
        {
            if (m_LightFlarePivotParentList == null || m_LightFlarePivotParentList[0] == null)
            {
                Log.Warning("LightRingManager", "AnimateLightFlareMovement : light flare should have a pivot parent assigned.");
                return;
            }

            for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
            {
                m_LightFlarePivotParentList[i].gameObject.SetActive((animationTime > 0.0f));
            }

            if (m_MoveAnimationOnFlare != null)
            {
                for (int i = 0; i < m_MoveAnimationOnFlare.Length; i++)
                {
                    if (m_MoveAnimationOnFlare[i] != null)
                    {
                        m_MoveAnimationOnFlare[i].setTime(animationTime);
                    }
                }
            }
            else
            {
                if (m_LightFlarePivotParentList.Length == m_ListFlareBezierPathList.Length && animationTime > 0.0f)
                {
                    m_MoveAnimationOnFlare = new LTDescr[m_LightFlarePivotParentList.Length];
                    for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                    {
                        m_MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], m_ListFlareBezierPathList[i], animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(m_LightFlareEase).setLoopPingPong();
                    }
                }
            }
        }

        #endregion

    }

}
