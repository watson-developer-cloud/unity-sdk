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

		#region Enumerations Related with Animation

		public enum LightIndicatorAnimationState
		{
			NAN = -1,
			NOT_PRESENT = 0,    //Light indicator is not created OR not visible on the scene
			LISTENING,
			THINKING,
			ANSWERING,
			NOT_UNDERSTAND
		}

		#endregion

		#region Public Member

		public LightIndicatorAnimationState AnimationState{
			get{
				return m_AnimationState;
			}
			set{
				m_AnimationStatePrevious = m_AnimationState;
				m_AnimationState = value;
			}
		}

		public LightIndicatorAnimationState AnimationStatePrevious{
			get{
				return m_AnimationStatePrevious;
			}
		}

		#endregion

        #region Private Variables

        [SerializeField]
        private LeanTweenType m_LightFlareEase = LeanTweenType.easeInOutQuad;

        [SerializeField]
        private float m_AnimationTime = 1.0f;

        private int m_NumberOfLightFlare = 3;
		private LightIndicatorAnimationState m_AnimationState = LightIndicatorAnimationState.NOT_PRESENT;
		private LightIndicatorAnimationState m_AnimationStatePrevious = LightIndicatorAnimationState.NAN;

        [SerializeField]
        private GameObject[] m_LightFlarePivotParentList;
        private GameObject[] m_LightFlareChild;
        //private Vector3[][] m_ListFlareBezierPathList;
		private readonly Vector3[] m_FullCircleBezierPathPoints = {
			new Vector3 (27.75156f, 0.0005040757f, -17.34112f),
			new Vector3 (25.43105f, 0.0005034769f, -20.74109f),
			new Vector3 (26.53236f, 0.0005166732f, -19.31231f),
			new Vector3 (23.83523f, 0.0004974342f, -22.42198f),
			new Vector3 (23.83523f, 0.0004974342f, -22.42198f),
			new Vector3 (20.89601f, 0.0004844714f, -25.3039f),
			new Vector3 (22.2549f, 0.0004917146f, -24.11743f),
			new Vector3 (19.00293f, 0.0004716767f, -26.64117f),
			new Vector3 (19.00293f, 0.0004716767f, -26.64117f),
			new Vector3 (15.55795f, 0.0004468478f, -28.89431f),
			new Vector3 (17.1222f, 0.0004591883f, -27.99574f),
			new Vector3 (13.44036f, 0.0004277928f, -29.83656f),
			new Vector3 (13.44036f, 0.0004277928f, -29.83656f),
			new Vector3 (9.622017f, 0.0003920521f, -31.37432f),
			new Vector3 (11.3315f, 0.0004090156f, -30.79818f),
			new Vector3 (7.361289f, 0.0003674691f, -31.88534f),
			new Vector3 (7.361289f, 0.0003674691f, -31.88534f),
			new Vector3 (3.316306f, 0.0003221901f, -32.64863f),
			new Vector3 (5.105343f, 0.0003431247f, -32.41707f),
			new Vector3 (0.9993219f, 0.0002930238f, -32.70879f),
			new Vector3 (0.9993219f, 0.0002930238f, -32.70879f),
			new Vector3 (-3.116846f, 0.0002399465f, -32.66827f),
			new Vector3 (-1.317015f, 0.0002640477f, -32.79019f),
			new Vector3 (-5.401047f, 0.0002073177f, -32.27525f),
			new Vector3 (-5.401047f, 0.0002073177f, -32.27525f),
			new Vector3 (-9.430225f, 0.0001484818f, -31.43249f),
			new Vector3 (-7.688756f, 0.0001748235f, -31.9032f),
			new Vector3 (-11.59386f, 0.0001136445f, -30.6014f),
			new Vector3 (-11.59386f, 0.0001136445f, -30.6014f),
			new Vector3 (-15.3812f, 5.131117E-05f, -28.98878f),
			new Vector3 (-13.76502f, 7.888097E-05f, -29.79018f),
			new Vector3 (-17.34113f, 1.560402E-05f, -27.75156f),
			new Vector3 (-17.34113f, 1.560402E-05f, -27.75156f),
			new Vector3 (-20.74109f, -4.783142E-05f, -25.43105f),
			new Vector3 (-19.31231f, -2.009299E-05f, -26.53235f),
			new Vector3 (-22.42198f, -8.30361E-05f, -23.83523f),
			new Vector3 (-22.42198f, -8.30361E-05f, -23.83523f),
			new Vector3 (-25.30391f, -0.0001451358f, -20.89601f),
			new Vector3 (-24.11743f, -0.0001182947f, -22.25489f),
			new Vector3 (-26.64117f, -0.0001784852f, -19.00293f),
			new Vector3 (-26.64117f, -0.0001784852f, -19.00293f),
			new Vector3 (-28.89431f, -0.0002368628f, -15.55795f),
			new Vector3 (-27.99574f, -0.0002119505f, -17.12219f),
			new Vector3 (-29.83656f, -0.0002670753f, -13.44036f),
			new Vector3 (-29.83656f, -0.0002670753f, -13.44036f),
			new Vector3 (-31.37432f, -0.0003194871f, -9.622015f),
			new Vector3 (-30.79818f, -0.0002974611f, -11.3315f),
			new Vector3 (-31.88534f, -0.0003454017f, -7.361287f),
			new Vector3 (-31.88534f, -0.0003454017f, -7.361287f),
			new Vector3 (-32.64863f, -0.0003898339f, -3.316301f),
			new Vector3 (-32.41707f, -0.0003715405f, -5.105342f),
			new Vector3 (-32.70879f, -0.0004104546f, -0.99932f),
			new Vector3 (-32.70879f, -0.0004104546f, -0.99932f),
			new Vector3 (-32.66827f, -0.0004451995f, 3.116853f),
			new Vector3 (-32.79019f, -0.0004313418f, 1.317016f),
			new Vector3 (-32.27526f, -0.0004597339f, 5.401053f),
			new Vector3 (-32.27526f, -0.0004597339f, 5.401053f),
			new Vector3 (-31.4325f, -0.0004834563f, 9.430223f),
			new Vector3 (-31.9032f, -0.0004745668f, 7.688761f),
			new Vector3 (-30.6014f, -0.0004913458f, 11.59386f),
			new Vector3 (-30.6014f, -0.0004913458f, 11.59386f),
			new Vector3 (-28.98879f, -0.0005031343f, 15.3812f),
			new Vector3 (-29.79019f, -0.0004995545f, 13.76502f),
			new Vector3 (-27.75156f, -0.0005040757f, 17.34113f),
			new Vector3 (-27.75156f, -0.0005040757f, 17.34113f),
			new Vector3 (-25.43105f, -0.0005034769f, 20.74109f),
			new Vector3 (-26.53236f, -0.0005053447f, 19.31231f),
			new Vector3 (-23.83523f, -0.0004974342f, 22.42198f),
			new Vector3 (-23.83523f, -0.0004974342f, 22.42198f),
			new Vector3 (-20.89602f, -0.0004844714f, 25.3039f),
			new Vector3 (-22.25489f, -0.0004917146f, 24.11744f),
			new Vector3 (-19.00293f, -0.0004716767f, 26.64117f),
			new Vector3 (-19.00293f, -0.0004716767f, 26.64117f),
			new Vector3 (-15.55795f, -0.0004468478f, 28.89431f),
			new Vector3 (-17.12219f, -0.0004591883f, 27.99574f),
			new Vector3 (-13.44036f, -0.0004277928f, 29.83656f),
			new Vector3 (-13.44036f, -0.0004277928f, 29.83656f),
			new Vector3 (-9.622013f, -0.0003920521f, 31.37432f),
			new Vector3 (-11.3315f, -0.0004090156f, 30.79818f),
			new Vector3 (-7.361283f, -0.000367469f, 31.88534f),
			new Vector3 (-7.361283f, -0.000367469f, 31.88534f),
			new Vector3 (-3.316298f, -0.00032219f, 32.64863f),
			new Vector3 (-5.105337f, -0.0003431247f, 32.41707f),
			new Vector3 (-0.9993143f, -0.0002930237f, 32.70879f),
			new Vector3 (-0.9993143f, -0.0002930237f, 32.70879f),
			new Vector3 (3.116858f, -0.0002399464f, 32.66827f),
			new Vector3 (1.317023f, -0.0002640476f, 32.79019f),
			new Vector3 (5.401059f, -0.0002073175f, 32.27525f),
			new Vector3 (5.401059f, -0.0002073175f, 32.27525f),
			new Vector3 (9.430223f, -0.0001484819f, 31.4325f),
			new Vector3 (7.688766f, -0.0001748234f, 31.90319f),
			new Vector3 (11.59385f, -0.0001136445f, 30.6014f),
			new Vector3 (11.59385f, -0.0001136445f, 30.6014f),
			new Vector3 (15.3812f, -5.131118E-05f, 28.98878f),
			new Vector3 (13.76502f, -7.888099E-05f, 29.79019f),
			new Vector3 (17.34113f, -1.560404E-05f, 27.75156f),
			new Vector3 (17.34113f, -1.560404E-05f, 27.75156f),
			new Vector3 (20.74109f, 4.783141E-05f, 25.43105f),
			new Vector3 (19.31231f, 2.009297E-05f, 26.53236f),
			new Vector3 (22.42198f, 8.303614E-05f, 23.83523f),
			new Vector3 (22.42198f, 8.303614E-05f, 23.83523f),
			new Vector3 (25.30391f, 0.0001451359f, 20.89601f),
			new Vector3 (24.11744f, 0.0001182948f, 22.25489f),
			new Vector3 (26.64117f, 0.0001784853f, 19.00293f),
			new Vector3 (26.64117f, 0.0001784853f, 19.00293f),
			new Vector3 (28.89431f, 0.0002368628f, 15.55795f),
			new Vector3 (27.99574f, 0.0002119506f, 17.12219f),
			new Vector3 (29.83656f, 0.0002670753f, 13.44036f),
			new Vector3 (29.83656f, 0.0002670753f, 13.44036f),
			new Vector3 (31.37432f, 0.0003194872f, 9.622005f),
			new Vector3 (30.79819f, 0.0002974612f, 11.33149f),
			new Vector3 (31.88534f, 0.0003454018f, 7.361278f),
			new Vector3 (31.88534f, 0.0003454018f, 7.361278f),
			new Vector3 (32.64864f, 0.000389834f, 3.316292f),
			new Vector3 (32.41708f, 0.0003715406f, 5.105331f),
			new Vector3 (32.70879f, 0.0004104545f, 0.9993248f),
			new Vector3 (32.70879f, 0.0004104545f, 0.9993248f),
			new Vector3 (32.66828f, 0.0004451995f, -3.116849f),
			new Vector3 (32.79019f, 0.0004313417f, -1.317011f),
			new Vector3 (32.27526f, 0.0004597339f, -5.401049f),
			new Vector3 (32.27526f, 0.0004597339f, -5.401049f),
			new Vector3 (31.4325f, 0.0004834563f, -9.430225f),
			new Vector3 (31.9032f, 0.0004745668f, -7.688756f),
			new Vector3 (30.6014f, 0.0004913459f, -11.59386f),
			new Vector3 (30.6014f, 0.0004913459f, -11.59386f),
			new Vector3 (28.98878f, 0.0005031343f, -15.3812f),
			new Vector3 (29.79018f, 0.0004995545f, -13.76503f),
			new Vector3 (27.75155f, 0.0005040758f, -17.34113f)
		};

        private Material m_SharedMaterialLightFlare;
        private Color m_TintColorSharedMaterialLightFlareInitial = Color.white;
        private Color m_ColorAnimationFlareLast = Color.white;

        private int m_ColorAnimationOnRing = -1;
        private int m_ColorAnimationOnFlareInitial = -1;
        private int m_ColorAnimationOnFlareLoop = -1;
		private int[] m_MoveAnimationOnFlare = null;
        private float[] m_MoveAnimationLastRatioOnFlare = null;
		private float[] m_MoveAnimationDirectionOnFlare = null;

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

			if (m_SharedMaterialLightFlare != null)
			{
				m_SharedMaterialLightFlare.SetColor("_TintColor", m_TintColorSharedMaterialLightFlareInitial);
			}
		}
		
		#endregion

        #region ApplicationQuit / Awake

   
        /// <exclude />
		protected override void Start()
        {
			base.Start ();

			if(m_AvatarWidgetAttached == null)
				m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();

			for (int i = 0; i < m_FullCircleBezierPathPoints.Length; i++) {
				m_FullCircleBezierPathPoints[i] = new Vector3(m_FullCircleBezierPathPoints[i].x,  -6.06f, m_FullCircleBezierPathPoints[i].z);
			}


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
			if (AnimationState == LightIndicatorAnimationState.NOT_UNDERSTAND)
				return;

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

			if (AnimationState == LightIndicatorAnimationState.NOT_UNDERSTAND)
				return;

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
			else if (m_AvatarWidgetAttached.State == AvatarWidget.AvatarState.DONT_UNDERSTAND)
			{
				AnimateLightFlareForNotUnderstanding();
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

		private void ResetLastValueAnimationFlare(){
			for (int i = 0; m_MoveAnimationLastRatioOnFlare != null && i < m_MoveAnimationLastRatioOnFlare.Length; i++) {
				m_MoveAnimationLastRatioOnFlare[i] = (1.0f / m_MoveAnimationLastRatioOnFlare.Length) * i;
			}
		}

        public float[] LastValueAnimationFlare
        {
            get
            {
                if (m_MoveAnimationLastRatioOnFlare == null && MoveAnimationOnFlare != null){
                    m_MoveAnimationLastRatioOnFlare = new float[MoveAnimationOnFlare.Length];
					ResetLastValueAnimationFlare();
				} 

                return m_MoveAnimationLastRatioOnFlare;
            }
        }

		private void ResetLastDirectionAnimationFlare(){
			for (int i = 0; m_MoveAnimationDirectionOnFlare != null && i < m_MoveAnimationDirectionOnFlare.Length; i++) {
				m_MoveAnimationDirectionOnFlare[i] = 1.0f;
			}
		}

		public float[] DirectionAnimationFlare
		{
			get
			{
				if (m_MoveAnimationDirectionOnFlare == null && MoveAnimationOnFlare != null){
					m_MoveAnimationDirectionOnFlare = new float[MoveAnimationOnFlare.Length];
					ResetLastDirectionAnimationFlare();
				} 
				
				return m_MoveAnimationDirectionOnFlare;
			}
		}

		public int[] MoveAnimationOnFlare{
			get{
				if(m_MoveAnimationOnFlare == null){
					m_MoveAnimationOnFlare = new int[m_LightFlarePivotParentList.Length];
					for (int i = 0; i < m_MoveAnimationOnFlare.Length; i++) {
						m_MoveAnimationOnFlare[i] = -1;
					}
				}
				return m_MoveAnimationOnFlare;
			}
		}
        private void StopLightFlareAnimation()
        {
            m_LightFlareIsUnderMouth = false;
            //Stop the movement animation
            if (MoveAnimationOnFlare != null && MoveAnimationOnFlare.Length > 0)
            {
                for (int i = 0; i < MoveAnimationOnFlare.Length; i++)
                {
					if (LeanTween.descr (MoveAnimationOnFlare[i]) != null)
                    {
						LeanTween.descr (MoveAnimationOnFlare[i]).setLoopOnce();
						LastValueAnimationFlare[i] = getRationInOneRange( LeanTween.descr (MoveAnimationOnFlare[i]).lastVal );
						DirectionAnimationFlare[i] = LeanTween.descr (MoveAnimationOnFlare[i]).direction;
						//Log.Warning("LightRingManager", "MoveAnimationOnFlare[i].lastVal : {0}", LastValueAnimationFlare[i]); 
                        LeanTween.cancel(MoveAnimationOnFlare[i]);
                        MoveAnimationOnFlare[i] = -1;
                    }
                }
            }
        }

		private float distanceForNotUnderstanding = 0.05f;

		private int numberOfLoopOnNotUnderstanding = 3;
        private void AnimateLightFlareForNotUnderstanding()
        {

            StopLightFlareAnimation();
			AnimationState = LightIndicatorAnimationState.NOT_UNDERSTAND;
			//UnityEngine.Debug.LogWarning("Animation Started - Didn't understand");

            //TODO: Finish the understanding part
			for (int i = 0; i < m_LightFlarePivotParentList.Length; i++) {

				float initialValue = getRationInOneRange(LastValueAnimationFlare[i]);
				float targetRatioEnd = initialValue - distanceForNotUnderstanding * Mathf.Sign(LastValueAnimationFlare[i]);

				MoveAnimationOnFlare[i] = LeanTween.value(gameObject, initialValue, targetRatioEnd, m_AnimationTime * m_AvatarWidgetAttached.BehaviorTimeModifier).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(numberOfLoopOnNotUnderstanding).setOnUpdate((float f, System.Object o)=>{
					if (o is int){
						int indexPivot = (int)o;
						int currentLoop = LeanTween.descr(MoveAnimationOnFlare[indexPivot]).loopCount;

						float currentLoopNormalized = currentLoop / (numberOfLoopOnNotUnderstanding * 2.0f);
						//float valueToBe = initialValue + (f - initialValue) * currentLoopNormalized;
						if(currentLoop != 1)
							LastValueAnimationFlare[indexPivot] = Mathf.Lerp(LastValueAnimationFlare[indexPivot], f, currentLoopNormalized);
						else
							LastValueAnimationFlare[indexPivot] = Mathf.Lerp(LastValueAnimationFlare[indexPivot], initialValue, currentLoopNormalized);

						//float valueToBe = Mathf.Lerp(lastValue, 
						BezierPathAllInOne.placeLocal( m_LightFlarePivotParentList[indexPivot].transform, getRationInOneRange(LastValueAnimationFlare[indexPivot]) );

					}
					else{
						Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
					}
				}, i).setOnComplete((System.Object o)=>{
					if (o is int){
						int indexPivot = (int)o;
						if(indexPivot == m_LightFlarePivotParentList.Length - 1){
							//UnityEngine.Debug.LogWarning("Animation Finished - Didn't understand");
							AnimationState = LightIndicatorAnimationState.NAN;
							OnChangedBehavior(null);
						}
					}
					else{
						Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
					}
					
				}, i).id;

				/*
				MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, m_AnimationTime * m_AvatarWidgetAttached.BehaviorTimeModifier).setOrientToPath(true).setAxis(Vector3.forward).setFrom(Vector3.one * lastValue).setTo(Vector3.one * targetRatioEnd).setEase(LeanTweenType.easeOutSine).setLoopPingPong(3).setOnComplete((System.Object o)=>{
					if (o is int){
						int indexPivot = (int)o;
						if(indexPivot == m_LightFlarePivotParentList.Length - 1){
							UnityEngine.Debug.LogWarning("Animation Finished - Didn't understand");
							AnimationState = LightIndicatorAnimationState.NAN;
							OnChangedBehavior(null);
						}
					}
					else{
						Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
					}
					
				}, i).id;
				*/
			}

        }

        private void AnimateLightFlareForListening()
        {
            
            StopLightFlareAnimation();
			AnimationState = LightIndicatorAnimationState.LISTENING;

            float animationTime = m_AnimationTime * m_AvatarWidgetAttached.MoodTimeModifier;

            if (animationTime > 0.0f)
            {
                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
					float lastValue = LastValueAnimationFlare[i];
					float targetRatioInital = ((1.0f/ m_NumberOfLightFlare) * i);
					float targetRatioEnd = (1.0f/ m_NumberOfLightFlare) * (i + 1);

					if(LastValueAnimationFlare[i] > targetRatioEnd){
						targetRatioInital += 1.0f;
					}

					MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, 1.0f).setOrientToPath(true).setAxis(Vector3.forward).setFrom(Vector3.one * lastValue).setTo(Vector3.one * targetRatioEnd).setEase(m_LightFlareEase).setOnComplete((System.Object o)=>{
						if (o is int){
							int indexPivot = (int)o;
							MoveAnimationOnFlare[indexPivot] = LeanTween.moveLocal(m_LightFlarePivotParentList[indexPivot], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setFrom(Vector3.one * ((1.0f/ m_NumberOfLightFlare) * (indexPivot + 1))).setTo(Vector3.one *  ((1.0f/ m_NumberOfLightFlare) * (indexPivot))).setEase(m_LightFlareEase).setLoopPingPong().id;
						}
						else{
							Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
						}

					}, i).id;
                    //MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], m_ListFlareBezierPathList[i], animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(m_LightFlareEase).setLoopPingPong().id;
				}
            }
            
        }

        private void AnimateLightFlareForThinking()
        {
            StopLightFlareAnimation();
			AnimationState = LightIndicatorAnimationState.THINKING;

			float animationTime = 0.5f * m_AvatarWidgetAttached.MoodTimeModifier; // m_AnimationTime * m_AvatarWidgetAttached.MoodTimeModifier;

            if (animationTime > 0.0f)
            {
                
				//UnityEngine.Debug.Break();
                for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
                {
                    //MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setEase(LeanTweenType.linear).setLoopClamp();


					float lastValue = LastValueAnimationFlare[i];
					Vector3 meetingRatio = Vector3.one *  ((1.0f/ m_NumberOfLightFlare) * (m_NumberOfLightFlare + 1));

					if(BezierPathAllInOne.pts.Length != 128){
						UnityEngine.Debug.LogWarning("m_BezierPathAllInOne has CHANGED!");
					}

					MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setFrom(Vector3.one * lastValue).setTo(meetingRatio).setEase(LeanTweenType.easeInSine).setOnComplete((System.Object o)=>{
						if (o is int){
							if(BezierPathAllInOne.pts.Length != 128){
								UnityEngine.Debug.LogWarning("m_BezierPathAllInOne has CHANGED!");
							}

                            int indexPivot = (int)o;
							MoveAnimationOnFlare[indexPivot] = LeanTween.moveLocal(m_LightFlarePivotParentList[indexPivot], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setFrom(meetingRatio).setTo(meetingRatio + Vector3.one).setEase(LeanTweenType.linear).setLoopClamp().id;
                        }
                        else
                        {
                            Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
                        }
					}, i).id;


                }
            }
        }

        private LTBezierPath m_BezierPathAllInOne;
        LTBezierPath BezierPathAllInOne
        {
            get
            {
                if(m_BezierPathAllInOne == null)
                {
					m_BezierPathAllInOne = new LTBezierPath(m_FullCircleBezierPathPoints);
                }

				if(m_BezierPathAllInOne.pts.Length != 128){
					UnityEngine.Debug.LogWarning("m_BezierPathAllInOne has CHANGED!");
				}

                return m_BezierPathAllInOne;
            }
        }

		public float getRationInOneRange(float ratio){
			if (ratio >= 0.0f && ratio <= 1.0f) {
				return ratio;
			} else if (ratio < 0.0f) {
				return Mathf.Ceil(ratio) - ratio;	//if -1.4 => it returns 0.4
			} else {
				return ratio - Mathf.Floor(ratio);	//if 1.4 => it return 0.4
			}
		}

        private void AnimateLightFlareForAnswering()
        {
            StopLightFlareAnimation();
			AnimationState = LightIndicatorAnimationState.ANSWERING;
           
            LeanTweenType easeForMoveToMouthPosition = LeanTweenType.easeInOutCirc;
			float animationTime = 0.5f;
            //Log.Status("LightRingManager", "AnimateLightFlareForAnswering: {0}", m_LightFlarePivotParentList.Length);


            for (int i = 0; i < m_LightFlarePivotParentList.Length; i++)
            {
				float ratioPositionForMouth = ((1.0f / m_LightFlarePivotParentList.Length) * i) + (1.0f / (2 * m_LightFlarePivotParentList.Length));

                //Log.Status("LightRingManager", "animationTime: {0} - LastValueAnimationFlare[i] {1} to ratioPositionForMouth: {2}", animationTime, LastValueAnimationFlare[i], ratioPositionForMouth);
				if(AnimationStatePrevious == LightIndicatorAnimationState.THINKING){
					while(LastValueAnimationFlare[i] > ratioPositionForMouth){
						ratioPositionForMouth += 1.0f;
					}
				}

				MoveAnimationOnFlare[i] = LeanTween.moveLocal(m_LightFlarePivotParentList[i], BezierPathAllInOne, animationTime).setOrientToPath(true).setAxis(Vector3.forward).setFrom(Vector3.one * LastValueAnimationFlare[i]).setTo(Vector3.one * ratioPositionForMouth).setEase(easeForMoveToMouthPosition).setOnComplete((System.Object o)=>{
					if (o is int){
						int indexPivot = (int)o;

						LastValueAnimationFlare[indexPivot] = (1.0f / m_LightFlarePivotParentList.Length) * indexPivot + (1.0f / (2 * m_LightFlarePivotParentList.Length));
						MoveAnimationOnFlare[indexPivot] = -1;

						if(indexPivot == 0){
							m_LightFlareIsUnderMouth = true;
							StopAnimateLightFlareColor();
						}
					}
					else{
						Log.Warning("LightRingManager", "AnimateLightFlareForThinking has invalid parameter : {0}", o.ToString());
					}
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
				MeshRenderer flareRenderer = m_LightFlarePivotParentList[i].GetComponentInChildren<MeshRenderer>();
				if(flareRenderer != null)
					flareRenderer.enabled = (animationTime > 0.0f);
				
				if(animationTime == 0.0f){
					ResetLastValueAnimationFlare();	//reset the position of light flare
					ResetLastDirectionAnimationFlare();
					AnimationState = LightIndicatorAnimationState.NOT_PRESENT;
				}
            }

            if (MoveAnimationOnFlare != null)
            {
                for (int i = 0; i < MoveAnimationOnFlare.Length; i++)
                {
                    if (LeanTween.descr( MoveAnimationOnFlare[i]) != null)
                    {
						LeanTween.descr(MoveAnimationOnFlare[i]).setTime(animationTime);
                        
						Log.Warning("LightRingManager", "1) SetTimeOnLightFlareMovementAnimation[i].lastVal : {0}", LeanTween.descr(MoveAnimationOnFlare[i]).lastVal);
                    }
                }
            }
            else
            {
				//do nothing
            }
        }

        #endregion

    }

}
