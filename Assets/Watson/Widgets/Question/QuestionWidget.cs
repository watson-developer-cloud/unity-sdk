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
* @author Richard Lyle (rolyle@us.ibm.com)
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

using IBM.Watson.Widgets.Avatar;
using IBM.Watson.Logging;
using IBM.Watson.Data.XRAY;
using IBM.Watson.Data.QA;
using UnityEngine;
using System.Collections.Generic;


namespace IBM.Watson.Widgets.Question
{
    /// <summary>
    /// This class manages the answers, question, and other data related to a question asked of the AvatarWidget.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class QuestionWidget : Widget
    {
        #region Private Data
        private CubeAnimationManager m_CubeAnimMgr = null;
		private bool m_Focused = false;

		[Header("Available Facets")]
		[SerializeField]
		private GameObject m_FacetTitle;
		[SerializeField]
		private GameObject m_FacetAnswers;
		[SerializeField]
		private GameObject m_FacetQuestion;
		[SerializeField]
		private GameObject m_FacetEvidence;
		[SerializeField]
		private GameObject m_FacetLocation;
		[SerializeField]
		private GameObject m_FacetChat;
		[SerializeField]
		private GameObject m_FacetPassage;

		[SerializeField]
		private List<GameObject> m_Facets = new List<GameObject>();

		private List<GameObject> m_DefaultFacets = new List<GameObject>();
		private List<GameObject> m_ThunderstoneFacets = new List<GameObject>();
		private List<GameObject> m_WoodsideFacets = new List<GameObject>();

		[SerializeField]
		private List<Transform> m_SidePresentations = new List<Transform>();

		private List<GameObject> m_GeneratedSides = new List<GameObject>();

        #endregion

        #region Widget Interface
        protected override string GetName()
        {
            return "Question";
        }
        #endregion

        #region Public Properties - Question widget Focus and attached CubeAnimationManager

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IBM.Watson.Widgets.Question.QuestionWidget"/> is focused.
        /// </summary>
        /// <value><c>true</c> if focused; otherwise, <c>false</c>.</value>
        public bool Focused
        {
            get
            {
                return m_Focused;
            }
            set
            {
                m_Focused = value;
                EnableEvents(value);
				Cube.enabled = value;	//we are not disabling the animation from focused. 
            }
        }
		/// <summary>
		/// Gets the Cube Animation Manager attached with question widget. 
		/// </summary>
		/// <value>The cube.</value>
        public CubeAnimationManager Cube
        {
            get
            {
                if (m_CubeAnimMgr == null)
                    m_CubeAnimMgr = GetComponentInChildren<CubeAnimationManager>();

                if (m_CubeAnimMgr == null)
                    Log.Error("QuestionWidget", "CubeAnimationManager is not found under Question widget");

                return m_CubeAnimMgr;
            }
		}

        #endregion

        #region Event Handlers of Question Widget

        /// <summary>
        /// On Behavior Change of the avatar, this function gets called
        /// </summary>
        /// <param name="args">If there 2 parameters we consider 1st parameter is AvatarWidget object, otherwise we are using initial AvatarWidget object's values to call other functions</param>
        public void OnChangedAvatarState(object[] args)
        {
            if (args.Length == 2 && args[0] is AvatarWidget  && args[1] != null)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                AvatarWidget.AvatarState avatarState = (AvatarWidget.AvatarState)args[1];

                if (avatarWidget != null)
                {
                    switch (avatarState)
                    {
                        case AvatarWidget.AvatarState.CONNECTING:
                            break;
                        case AvatarWidget.AvatarState.LISTENING:
                            break;
                        case AvatarWidget.AvatarState.THINKING:
                            break;
                        case AvatarWidget.AvatarState.ANSWERING:
                            Cube.Fold();
                            break;
                        case AvatarWidget.AvatarState.ERROR:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Log.Error("AvatarModelManager", "OnChangedAvatarState the argument is not AvatarWidget object");
                }
            }
            else
            {
                Log.Error("QuestionWidget", "OnChangedAvatarState the argument length is undefined {0}", args.Length);
            }
        }

        /// <summary>
        /// Method called on Tapping on Question Widget 
        /// </summary>
        /// <param name="tapGesture">Tap Gesture with all touch information</param>
        /// <param name="hitTransform">Hit Tranform of tap</param>
		public void OnTapInside(object[] args)
        {
            if (args != null && args.Length == 2 && args[0] is TouchScript.Gestures.TapGesture && args[1] is RaycastHit)
            {
                Log.Status("Question Widget", "OnTapInside");
                TouchScript.Gestures.TapGesture tapGesture = args [0] as TouchScript.Gestures.TapGesture; 
				RaycastHit raycastHit = (RaycastHit) args[1];

				Cube.OnTapInside(tapGesture, raycastHit);
          
            }
            else
			{
                Log.Warning("Question Widget", "OnTapInside has invalid arguments!");
            }

        }

        /// <summary>
        /// Method called on Tapping outside of the Question Widget 
        /// </summary>
        /// <param name="tapGesture">Tap Gesture with all touch information</param>
        /// <param name="hitTransform">Hit Tranform of tap</param>
		public void OnTapOutside(object[] args)
        {
			if (args != null && args.Length == 2 && args[0] is TouchScript.Gestures.TapGesture && args[1] is RaycastHit)
            {
                Log.Status("Question Widget", "OnTapOutside");
                TouchScript.Gestures.TapGesture tapGesture = args [0] as TouchScript.Gestures.TapGesture; 
				RaycastHit raycastHit = (RaycastHit) args [1];

				Cube.OnTapOutside(tapGesture, raycastHit);
            }
            else
            {
                Log.Warning("Question Widget", "OnTapOutside has invalid arguments!");
            }

        }

		/// <summary>
		/// Event handler for dragging with one finger.
		/// </summary>
		/// <param name="args">Arguments of event. args[0] should be TouchScript.Gestures.ScreenTransformGesture</param>
        public void DragOneFingerFullScreen(object[] args)
        {
			//Log.Warning("QuestWidget", "DragOneFingerOnObject FULLSCREEN ");
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
            {
                TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

                if (Cube != null)
                {
                    Cube.DragOneFingerFullScreen(OneFingerManipulationGesture);
                }
            }
        }

        public void ReleasedFinger(object[] args)
        {
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ReleaseGesture)
            {
                TouchScript.Gestures.ReleaseGesture releaseGesture = args[0] as TouchScript.Gestures.ReleaseGesture;

                if (Cube != null)
                {
                    Cube.ReleasedFinger(releaseGesture);
                }
            }
        }

        /// <summary>
        /// Drags the one finger on object.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public void DragOneFingerOnObject(object[] args)
		{
			//Log.Warning("QuestWidget", "DragOneFingerOnObject - OBJECT");
			if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
			{
				TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;
				
				if (Cube != null)
				{
					Cube.DragOneFingerFullScreen(OneFingerManipulationGesture);
					Cube.DragOneFingerOnSide(OneFingerManipulationGesture);
				}
			}
            else
            {
                Log.Warning("QuestWidget", "DragOneFingerOnObject has invalid arguments");
            }
		}

		/// <summary>
		/// Raises the display answers event to show Answer face of the question widget.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnDisplayAnswers(object[] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.ANSWERS);
        }
		/// <summary>
		/// Raises the display chat event to show Dialog face of the question widget.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnDisplayChat(object[] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.CHAT);
        }
		/// <summary>
		/// Raises the display parse event to show Parse face of the question widget.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnDisplayParse(object[] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.PARSE);
        }

		/// <summary>
		/// Raises the display evidence event to show Evidence face of the question widget.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnDisplayEvidence(object[] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.EVIDENCE);
        }

		/// <summary>
		/// Raises the display location event to show Location face of the question widget.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnDisplayLocation(object[] args)
        {
            Cube.FocusOnSide(CubeAnimationManager.CubeSideType.LOCATION);
        }

		/// <summary>
		/// Raises the fold event to fold the question widget
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnFold(object[] args)
        {
            Cube.Fold();
        }

		/// <summary>
		/// Raises the unfold event to unfold the question widget
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnUnfold(object[] args)
        {
            Cube.UnFold();
        }

		/// <summary>
		/// Raises the rotate or pause event. 
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnRotateOrPause(object[] args)
        {
            Cube.RotateOrPause();
        }

		/// <summary>
		/// Raises the focus event to show question widget's focused face
		/// </summary>
		/// <param name="args">Arguments. args[0] should have the CubeSideType</param>
        public void OnFocus(object[] args)
        {
			if (args != null && args.Length > 0 && args [0] is CubeAnimationManager.CubeSideType)
				Cube.FocusOnSide ((CubeAnimationManager.CubeSideType)args [0]);
		}
		/// <summary>
		/// Raises the focus next event to show the next possible face of the question widget. 
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnFocusNext(object[] args)
        {
            Cube.FocusOnNextSide();
        }
		/// <summary>
		/// Raises the un-focus event to return the unfolded position of the question widget
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnUnFocus(object[] args)
        {
            Cube.UnFocus();
        }
		/// <summary>
		/// Raises the leave the scene and destroy event.
		/// </summary>
		/// <param name="args">Arguments.</param>
        public void OnLeaveTheSceneAndDestroy(object[] args = null)
        {
			Focused = false;
            //Cube.LeaveTheSceneAndDestroy();
        }

        public void OnQuestionPipeline( object[] args)
        {
            string pipeline = (args != null && args.Length > 0) ? args[0] as string : null;
            if (! string.IsNullOrEmpty( pipeline ) )
            {
				ClearSides();

				switch(pipeline)
				{
				case "thunderstone":
					m_Facets = m_ThunderstoneFacets;
					break;
				case "woodside":
					m_Facets = m_WoodsideFacets; 
					break;
				default:
					m_Facets = m_DefaultFacets;
					break;
				}

				GenerateSides();
            }
        }
        #endregion

		#region Awake / Start / EnableEvents

        /// <summary>
        /// Register events, set facet references, add facets to a List.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            //EnableEvents(false);  //TODO: Uncomment - commented for test purposes!
			Cube.enabled = false;

			//	populate facets
			m_DefaultFacets.Add(m_FacetTitle);
			m_DefaultFacets.Add(m_FacetTitle);
			m_DefaultFacets.Add(m_FacetTitle);
			m_DefaultFacets.Add(m_FacetTitle);
			m_DefaultFacets.Add(m_FacetTitle);
			m_DefaultFacets.Add(m_FacetTitle);
			
			m_ThunderstoneFacets.Add(m_FacetTitle);
			m_ThunderstoneFacets.Add(m_FacetAnswers);
			m_ThunderstoneFacets.Add(m_FacetQuestion);
			m_ThunderstoneFacets.Add(m_FacetEvidence);
			m_ThunderstoneFacets.Add(m_FacetLocation);
			m_ThunderstoneFacets.Add(m_FacetChat);

			m_WoodsideFacets.Add(m_FacetTitle);
			m_WoodsideFacets.Add(m_FacetPassage);
			m_WoodsideFacets.Add(m_FacetQuestion);
			m_WoodsideFacets.Add(m_FacetEvidence);
			m_WoodsideFacets.Add(m_FacetLocation);
			m_WoodsideFacets.Add(m_FacetChat);

			GenerateSides();
        }

		public void GenerateSides()
		{
			ClearSides();

			for(int i = 0; i < m_Facets.Count; i++)
			{
				GameObject facetGameObject = Instantiate(m_Facets[i], Vector3.zero, Quaternion.Euler(new Vector3(0f, 90f, 0f))) as GameObject;
				m_GeneratedSides.Add(facetGameObject);
				Transform facetTransform = facetGameObject.GetComponent<Transform>();
				RectTransform facetCanvasRectTransform = facetTransform.GetComponentInChildren<RectTransform>();
				facetCanvasRectTransform.localScale = new Vector3(0.004885f, 0.004885f, 0f);
				facetTransform.SetParent(m_SidePresentations[i], false);
			}
		}

		private void ClearSides()
		{
			while (m_GeneratedSides.Count != 0)
			{
				Destroy(m_GeneratedSides[0].gameObject);
				m_GeneratedSides.Remove(m_GeneratedSides[0]);
			}
		}
		
		private void EnableEvents(bool enable)
        {
            EventWidget eventWidget = GetComponentInChildren<EventWidget>();
            if (eventWidget != null)
                eventWidget.enabled = enable;

            TouchWidget touchWidget = GetComponentInChildren<TouchWidget>();
            if (touchWidget != null)
                touchWidget.enabled = enable;

            KeyboardWidget keyboardWidget = GetComponentInChildren<KeyboardWidget>();
            if (keyboardWidget != null)
                keyboardWidget.enabled = enable;
        }

        protected override void Start()
        {
            base.Start();
        }

		#endregion
    }
}
