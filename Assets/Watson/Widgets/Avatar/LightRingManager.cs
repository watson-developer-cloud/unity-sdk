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

        private int m_ColorAnimationOnRing = -1;
        private int m_ColorAnimationOnFlareInitial = -1;
        private int m_ColorAnimationOnFlareLoop = -1;
        private int[] m_MoveAnimationOnFlare = null;
        private float[] m_MoveAnimationLastRatioOnFlare = null;

		private float m_AudioLevelOutput = 0.0f;
		private float m_AudioScaleModifier = 5.0f;
        private bool m_LightFlareIsUnderMouth = false;

        #endregion

		#region OnEnable / OnDisable For Event Registration
		
		protected override void OnEnable(){
			base.OnEnable ();
			EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_AVATAR_SPEAKING, AvatarSpeaking);
		}
		
		protected override void OnDisable(){
			base.OnDisable ();
			EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_AVATAR_SPEAKING, AvatarSpeaking);
		}
		
		#endregion

        #region ApplicationQuit / Awake

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
			base.Start ();

			if(m_AvatarWidgetAttached == null)
				m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();

            m_ListFlareBezierPathList = new Vector3[3][];

            /*
            m_ListFlareBezierPathList[0] = new Vector3[] {
                new Vector3 (28.365f, -6.06f, -16.385f),
                new Vector3 (22.58f, -6.06f, 32.78f),
                new Vector3 (41.7f, -6.06f, 8.4f),
                new Vector3 (0.062f, -6.06f, 32.764f)
            };

            m_ListFlareBezierPathList[1] = new Vector3[] {
                new Vector3 (0.007372856f, -6.060182f, 32.75727f),
                new Vector3 (-39.67829f, -6.060145f, 3.164852f),
                new Vector3 (-28.12456f, -6.060268f, 31.91325f),
                new Vector3 (-28.40545f, -6.06f, -16.32832f)
            };

            m_ListFlareBezierPathList[2] = new Vector3[] {
                new Vector3 (-28.37233f, -6.06f, -16.3723f),
                new Vector3 (17.0983f, -6.06f, -35.94485f),
                new Vector3 (-13.57539f, -6.06f, -40.31325f),
                new Vector3 (28.34345f, -6.06f, -16.4357f)
            };
            */

            m_ListFlareBezierPathList[0] = new Vector3[] {
                new Vector3 (0.062f, -6.06f, 32.764f),
                new Vector3 (41.7f, -6.06f, 8.4f),
                new Vector3 (22.58f, -6.06f, 32.78f),
                new Vector3 (28.365f, -6.06f, -16.385f)
            };

            m_ListFlareBezierPathList[1] = new Vector3[] {
                new Vector3 (28.34345f, -6.06f, -16.4357f),
                new Vector3 (-13.57539f, -6.06f, -40.31325f),
                new Vector3 (17.0983f, -6.06f, -35.94485f),
                new Vector3 (-28.37233f, -6.06f, -16.3723f)
            };

            m_ListFlareBezierPathList[2] = new Vector3[] {
                new Vector3 (-28.40545f, -6.06f, -16.32832f),
                new Vector3 (-28.12456f, -6.060268f, 31.91325f),
                new Vector3 (-39.67829f, -6.060145f, 3.164852f),
                new Vector3 (0.007372856f, -6.060182f, 32.75727f)
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

			if (m_AvatarWidgetAttached != null) {
				ChangedMood(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodTimeModifier);
				ChangedBehavior(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.BehaviorTimeModifier);
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
            SetTimeOnLightFlareMovementAnimation(m_AnimationTime * timeModifier);
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
            //Log.Warning("LightRingManager", "ChangedBehavior {0}", m_AvatarWidgetAttached.State);

            if (m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.SLEEPING_LISTENING || m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.LISTENING)
            {
                AnimateLightFlareForListening();
                //AnimateLightFlareForThinking();
            }
            else if(m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.THINKING)
            {
                AnimateLightFlareForThinking();
            }
            else if (m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.ANSWERING)
            {
                AnimateLightFlareForAnswering();
            }
            else
            {

            }

            AnimateLightRingColor(color, m_AnimationTime * timeModifier);
            AnimateLightFlareColor(color, m_AnimationTime * timeModifier);
        }

        #endregion

        #region Animations on Light Ring 

        private void AnimateLightRingColor(Color color, float animationTime)
        {
			if (LeanTween.descr(m_ColorAnimationOnRing) != null)
            {
                LeanTween.cancel(m_ColorAnimationOnRing);
				m_ColorAnimationOnRing = -1;
            }

            m_ColorAnimationOnRing = LeanTween.color(gameObject, color, animationTime).id; //.setFromColor (Color.white).setLoopPingPong ();
        }

        private void StopAnimateLightFlareColor()
        {

            if (LeanTween.descr (m_ColorAnimationOnFlareLoop) != null)
            {
				LeanTween.cancel(m_ColorAnimationOnFlareLoop);
                m_ColorAnimationOnFlareLoop = -1;
            }

			if (LeanTween.descr (m_ColorAnimationOnFlareInitial) != null)
            {
				LeanTween.descr(m_ColorAnimationOnFlareInitial).onComplete = null;
                LeanTween.cancel(m_ColorAnimationOnFlareInitial);
                m_ColorAnimationOnFlareInitial = -1;
            }
        }

        private void AnimateLightFlareColor(Color color, float animationTime)
        {
            if (m_SharedMaterialLightFlare == null)
            {
                Log.Warning("LightRingManager", "AnimateLightFlareColor : light flare should have a shared material.");
                return;
            }

            StopAnimateLightFlareColor();

            m_ColorAnimationOnFlareInitial = LeanTween.value(gameObject, m_ColorAnimationFlareLast, color, animationTime).setOnUpdateColor(
                (Color colorToFadeIn) =>
                {
                    m_ColorAnimationFlareLast = colorToFadeIn;
                    m_SharedMaterialLightFlare.SetColor("_TintColor", colorToFadeIn);
                }).setOnComplete(
                () =>
                {
					m_ColorAnimationOnFlareInitial = -1;
					
                    m_ColorAnimationOnFlareLoop = LeanTween.value(gameObject, color, Color.white, animationTime).setLoopPingPong().setOnUpdateColor(
                        (Color colorToLoop) =>
                        {
                            m_SharedMaterialLightFlare.SetColor("_TintColor", colorToLoop);
                            m_ColorAnimationFlareLast = colorToLoop;
                        }).id;

                }).id;
        }

        public float[] LastValueAnimationFlare
        {
            get
            {
                if (m_MoveAnimationLastRatioOnFlare == null && m_MoveAnimationOnFlare != null)
                    m_MoveAnimationLastRatioOnFlare = new float[m_MoveAnimationOnFlare.Length];

                return m_MoveAnimationLastRatioOnFlare;
            }
        }
        private void StopLightFlareAnimation()
        {
            //Log.Warning("LightRingManager", "StopLightFlareAnimation");
            m_LightFlareIsUnderMouth = false;
            //Stop the movement animation
            if (m_MoveAnimationOnFlare != null && m_MoveAnimationOnFlare.Length > 0)
            {
                for (int i = 0; i < m_MoveAnimationOnFlare.Length; i++)
                {
					if (LeanTween.descr (m_MoveAnimationOnFlare[i]) != null)
                    {
						LeanTween.descr (m_MoveAnimationOnFlare[i]).setLoopOnce();
						LastValueAnimationFlare[i] = LeanTween.descr (m_MoveAnimationOnFlare[i]).lastVal;
                        //Log.Warning("LightRingManager", "m_MoveAnimationOnFlare[i].lastVal : {0}", m_MoveAnimationOnFlare[i].lastVal); 
                        LeanTween.cancel(m_MoveAnimationOnFlare[i]);
                        m_MoveAnimationOnFlare[i] = 0;
                    }
                }
            }
        }

        private void AnimateLightFlreForUnderstandingCase()
        {

            StopLightFlareAnimation();
            //TODO: Finish the understanding part
        }

        private void AnimateLightFlareForListening()
        {
            
            StopLightFlareAnimation();

            float animationTime = m_AnimationTime * m_AvatarWidgetAttached.MoodTimeModifier;

            if (m_LightFlarePivotParentList.Length == m_ListFlareBezierPathList.Length && animationTime > 0.0f)
            {
                if(m_MoveAnimationOnFlare == null)
                    m_MoveAnimationOnFlare = new int[m_LightFlarePivotParentList.Length];

                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], m_ListFlareBezierPathList[i], animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(m_LightFlareEase).setLoopPingPong().id;
                }
            }
            
        }

        private void AnimateLightFlareForThinking()
        {
            StopLightFlareAnimation();

            float animationTime = 0.5f; // m_AnimationTime * m_AvatarWidgetAttached.MoodTimeModifier;

            if (m_LightFlarePivotParentList.Length == m_ListFlareBezierPathList.Length && animationTime > 0.0f)
            {
                if (m_MoveAnimationOnFlare == null)
                    m_MoveAnimationOnFlare = new int[m_LightFlarePivotParentList.Length];

                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
                    //m_MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(LeanTweenType.linear).setLoopClamp();
                    m_MoveAnimationOnFlare[i] = LeanTween.value(gameObject, 0.0f, 1.0f, animationTime).setOnUpdate((float f, System.Object o) => {
                        if (o is int)
                        {
                            int indexPivot = (int)o;
                            BezierPathAllInOne.placeLocal(m_LightFlarePivotParentList[indexPivot].transform, Mathf.Clamp(f, 0.0f, 0.999f));
                        }
                        else
                        {
                            Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
                        }
                    }, i).setLoopClamp().id;
                }
            }
        }

        private LTBezierPath[] m_BezierPathList;
        LTBezierPath[] BezierPathList
        {
            get
            {
                if (m_BezierPathList == null)
                {
                    m_BezierPathList = new LTBezierPath[m_LightFlarePivotParentList.Length];
                    for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                    {
                        m_BezierPathList[i] = new LTBezierPath(m_ListFlareBezierPathList[i]);
                    }
                   
                }
                return m_BezierPathList;
            }
        }

        private LTBezierPath m_BezierPathAllInOne;
        LTBezierPath BezierPathAllInOne
        {
            get
            {
                if(m_BezierPathAllInOne == null)
                {
                    System.Collections.Generic.List<Vector3> list = new System.Collections.Generic.List<Vector3>();
                    for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                    {
                        list.AddRange(m_ListFlareBezierPathList[i]);
                    }

                    if (list.Count > 0)
                    {
                        list[3] = list[4];
                        list[7] = list[8];
                        list[11] = list[0];
                    }
                    
                    m_BezierPathAllInOne = new LTBezierPath(list.ToArray());
                    
                }
                return m_BezierPathAllInOne;
            }
        }

        private void AnimateLightFlareForAnswering()
        {
            StopLightFlareAnimation();
            
            float ratioPositionForMouth = 0.0f;
            float timeToGoMouthPosition = 0.9f;
            LeanTweenType easeForMoveToMouthPosition = LeanTweenType.easeInOutCirc;
            m_MoveAnimationOnFlare = new int[m_LightFlarePivotParentList.Length];

            //Log.Status("LightRingManager", "AnimateLightFlareForAnswering: {0}", m_LightFlarePivotParentList.Length);

            for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
            {
                
                float animationTime = (Mathf.Abs(LastValueAnimationFlare[i] - ratioPositionForMouth) * timeToGoMouthPosition) + 0.1f;
                //Log.Status("LightRingManager", "animationTime: {0} - LastValueAnimationFlare[i] {1} to ratioPositionForMouth: {2}", animationTime, LastValueAnimationFlare[i], ratioPositionForMouth);
                
                m_MoveAnimationOnFlare[i] = LeanTween.value(m_LightFlarePivotParentList[i], LastValueAnimationFlare[i], ratioPositionForMouth, animationTime).setEase(easeForMoveToMouthPosition).setOnUpdate((float f, System.Object o) => {
                    
                    if(o is int)
                    {
                        int indexPivot = (int)o;
                        if (BezierPathList != null && BezierPathList.Length > indexPivot)
                        {
                            GameObject tweeningObject = m_LightFlarePivotParentList[indexPivot];
                            BezierPathList[indexPivot].placeLocal(tweeningObject.transform, f);
                            //Log.Status("LightRingManager", "{0} ) bezierPath ratio: {1} - Position : {2}", indexPivot, f, tweeningObject.transform.localPosition);
                        }
                        else
                        {
                            Log.Warning("LightRingManager", "AnimateLightFlare has invalid BezierPathList. Index: {0}", indexPivot);
                        }
                    }
                    else
                    {
                        Log.Warning("LightRingManager", "AnimateLightFlare has invalid parameter : {0}", o.ToString());
                    }
                    
                    
                }, i).setOnComplete( (System.Object o)=> {
					if(o is int)
					{
						int indexPivot = (int)o;
						m_MoveAnimationOnFlare[indexPivot] = -1;
					}

                    m_LightFlareIsUnderMouth = true;
                    StopAnimateLightFlareColor();
                    //for (int indexAnimation = 0; indexAnimation < m_MoveAnimationOnFlare.Length; indexAnimation++)
                    //{
                    //    LeanTween.scaleZ(m_LightFlarePivotParentList[indexAnimation],2, 1.0f).setLoopPingPong();
                    //}
                    

                }, i).id;
            }

        }

        
        public void AvatarSpeaking(System.Object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is float)
            {
                m_AudioLevelOutput = (float)args[0];
            }
        }

        void Update()
        {
            if (m_AvatarWidgetAttached != null && m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.ANSWERING && m_LightFlareIsUnderMouth)
            {
                for (int i = 0; m_LightFlarePivotParentList != null && i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_LightFlarePivotParentList[i].transform.localScale = Vector3.Lerp(m_LightFlarePivotParentList[i].transform.localScale, new Vector3(m_LightFlarePivotParentList[i].transform.localScale.x, m_LightFlarePivotParentList[i].transform.localScale.y, m_AudioLevelOutput) , Time.deltaTime * m_AudioScaleModifier);
                }

                m_SharedMaterialLightFlare.SetColor("_TintColor", Color.Lerp(m_AvatarWidgetAttached.BehaviourColor, Color.white, m_AudioLevelOutput * 0.5f));
            }
            else if (m_AvatarWidgetAttached != null && m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.THINKING)
            {
                for (int i = 0; m_LightFlarePivotParentList != null && i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_LightFlarePivotParentList[i].transform.localScale = Vector3.Lerp(m_LightFlarePivotParentList[i].transform.localScale, Vector3.one + Vector3.forward * 1.2f, Time.deltaTime * m_AudioScaleModifier);
                }
            }
            else
            {
                for (int i = 0; m_LightFlarePivotParentList != null && i < m_LightFlarePivotParentList.Length; i++)
                {
                    m_LightFlarePivotParentList[i].transform.localScale = Vector3.Lerp(m_LightFlarePivotParentList[i].transform.localScale, Vector3.one, Time.deltaTime * m_AudioScaleModifier);
                }
            }
        }
        

        private void SetTimeOnLightFlareMovementAnimation(float animationTime)
        {
            Log.Status("LightRingManager", "SetTimeOnLightFlareMovementAnimation {0}", animationTime);

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
                    if (LeanTween.descr( m_MoveAnimationOnFlare[i]) != null)
                    {
						LeanTween.descr(m_MoveAnimationOnFlare[i]).setTime(animationTime);
                        
						Log.Warning("LightRingManager", "1) SetTimeOnLightFlareMovementAnimation[i].lastVal : {0}", LeanTween.descr(m_MoveAnimationOnFlare[i]).lastVal);
                    }
                }
            }
            else
            {
                //if (m_LightFlarePivotParentList.Length == m_ListFlareBezierPathList.Length && animationTime > 0.0f)
                //{
                //    m_MoveAnimationOnFlare = new int[m_LightFlarePivotParentList.Length];
                //    for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                //    {
                //        m_MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], m_ListFlareBezierPathList[i], animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(m_LightFlareEase).setLoopPingPong();
                //        Log.Warning("LightRingManager", "2) SetTimeOnLightFlareMovementAnimation[i].lastVal : {0}", m_MoveAnimationOnFlare[i].lastVal);
                //    }
                //}
            }
        }

        #endregion

    }

}
