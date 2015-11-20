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

using IBM.Watson.Utilities;
using IBM.Watson.Logging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace IBM.Watson.Widgets.Question
{
    public class PassageAnimationManager : WatsonBaseAnimationManager
    {

        #region Private Members

        private CubeAnimationManager m_CubeAnimMgr = null;
        private QuestionWidget m_QuestionWidget = null;
        private Transform[] m_PassageItems = null;
        private ScrollRect[] m_PassageScrollRect = null;
        private bool m_IsTouchOnDragging = false;   //Used to identify the release finger
        private int m_SelectedPassageIndex = -1;
        private int m_PreviousPassageIndex = 0;

        //Dragging modifier with pixel size
        private float m_OneDragModifier = 0.002f;
        private float m_SpeedPassageAnimation = 4.0f;
        private float m_PercentToGoInitialPosition = 0.26f;
        private float m_PercentToGoStackPosition = 0.65f;
        private LTDescr[] m_AnimationToShowPositionPassage;
        private LTDescr[] m_AnimationToShowRotationPassage;
        private float m_AnimationTimeForEachPassageToGoTheirLocation = 1.0f;
        private float m_DelayBetweenPassages = 0.07f;
        private float m_DelayExtraOnSelectedPassage = 0.07f;
        private LeanTweenType m_LeanTypeForPassageMovement = LeanTweenType.easeOutCirc;

        //Holds all animations current Ratio value
        float[] m_AnimationLocationRatio;
        float[] m_AnimationRotationRatio;

        //Passage One Finger Animation for first item path in Vector3
        Vector3[] m_PathToCenterForFirstItem;
        Vector3[] m_PathOrientationToCenterForFirstItem;
        Vector3[] m_PathToStackForFirstItem;
        Vector3[] m_PathOrientationToStackForFirstItem;

        //Passage one finger animation paths for each passage
        LTBezierPath[] m_BezierPathToCenter;
        LTBezierPath[] m_BezierPathOrientationToCenter;
        LTBezierPath[] m_BezierPathToStack;
        LTBezierPath[] m_BezierPathOrientationToStack;
        //For speedy movements we are using from Initial to Stack animation
        LTBezierPath[] m_BezierPathFromInitialToStack;
        LTBezierPath[] m_BezierPathOrientationFromInitialToStack;

        //Offset between passages
        Vector3 m_OffsetPathToCenter;
        Vector3 m_OffsetPathOrientationToCenter;
        Vector3 m_OffsetPathToStack;
        Vector3 m_OffsetPathOrientationToStack;

        //Target Locations / Orientations
        Vector3[] m_TargetLocation;
        Vector3[] m_TargetRotation;

        //Names we are cheking under passage
        const string m_NameElementPanel = "Panel";
        const string m_NameElementTabItem = "TabItem";
        const string m_NameElementTabImage = "Tab";
        const string m_NameElementPassageItem = "PassageItem_";

        #endregion

        #region Public Members
        /// <summary>
        /// Gets the Cube Animation Manager attached with question widget. 
        /// </summary>
        /// <value>The cube.</value>
        public CubeAnimationManager Cube
        {
            get
            {
                if (m_CubeAnimMgr == null)
                    m_CubeAnimMgr = GetComponentInParent<CubeAnimationManager>();

                if (m_CubeAnimMgr == null)
                    Log.Error("PassageAnimationManager", "CubeAnimationManager is not found on parent of passage animation manager");

                return m_CubeAnimMgr;
            }
        }

        /// <summary>
        /// Gets the Question Widget attached 
        /// </summary>
        public QuestionWidget Question
        {
            get
            {
                if (m_QuestionWidget == null)
                    m_QuestionWidget = GetComponentInParent<QuestionWidget>();

                if (m_QuestionWidget == null)
                    Log.Error("PassageAnimationManager", "QuestionWidget is not found on parent of passage animation manager");

                return m_QuestionWidget;
            }
        }

        /// <summary>
        /// Gets the Passage Transform List
        /// </summary>
        public Transform[] PassageList
        {
            get
            {
                if (m_PassageItems == null)
                {
                    Log.Error("PassageAnimationManager", "PassageList couldn't find inside gameobject");
                }

                return m_PassageItems;
            }
        }

        /// <summary>
        /// Gets the number of passages we can show
        /// </summary>
        public int NumberOfPassages
        {
            get
            {
                int numberOfPassage = 0;
				if (m_PassageItems != null && m_PassageItems.Length > 0)
                    numberOfPassage = m_PassageItems.Length;
                return numberOfPassage;
            }
        }
        #endregion

        #region Awake / Update
       
        // Use this for initialization
        void Awake()
        {
            m_PathToCenterForFirstItem = new Vector3[]{
            new Vector3(-555,   -77,    -125),
            new Vector3(-600,   0,      -125),
            new Vector3(-800,   300,    -125),
            new Vector3(-907,   355,    -125)};

            m_PathOrientationToCenterForFirstItem = new Vector3[]{
            new Vector3(0,0,0),
            new Vector3(10,10,0),
            new Vector3(35,35,0),
            new Vector3(45,45,0)};

            m_PathToStackForFirstItem = new Vector3[]{
                new Vector3(-907,   355f,       -125),
                new Vector3(-800,   300f,       -200),
                new Vector3(450,    -100f,      -350),
                new Vector3(575,    -120,       -407)};

            m_PathOrientationToStackForFirstItem = new Vector3[]{
                new Vector3(45,45,0),
                new Vector3(35,35,0),
                new Vector3(10,10,0),
                new Vector3(0,0,0)};

            m_OffsetPathToCenter = new Vector3(0, 0, 50);
            m_OffsetPathOrientationToCenter = new Vector3(0, 0, 0);
            m_OffsetPathToStack = new Vector3(0, 0, 50);
            m_OffsetPathOrientationToStack = new Vector3(0, 0, 0);

        }

        void Start()
        {
            UpdatePassages();
        }

        // Update is called once per frame
        void Update()
        {
            DragOneFingerOnPassageOnUpdate();
           
        }

        /// <summary>
        /// This is the main update function for drag functions and animations
        /// </summary>
        private void DragOneFingerOnPassageOnUpdate()
        {
            for (int i = 0; i < NumberOfPassages; i++)
            {
                if (m_PassageItems[i] != null && m_PassageItems[i].transform != null)
                {
                    m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_TargetLocation[i], Time.deltaTime * m_SpeedPassageAnimation);
                    m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_TargetRotation[i]), Time.deltaTime * m_SpeedPassageAnimation);
                }
            }
        }

        #endregion


        #region Event Handlers on Passage
        /// <summary>
        /// If cube if folding, we are reseting our passage list presentation
        /// </summary>
        /// <param name="args"></param>
        public void CubeAnimationStateChanged(System.Object[] args)
        {
            if (Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.FOLDING || Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOLDED))
            {
                ShowPassage(-1);
            }
            else if (Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED))
            {
                if(m_SelectedPassageIndex < 0 || m_SelectedPassageIndex >= NumberOfPassages)
                    ShowPassage(0);

            }
        }

        /// <summary>
        /// If there is a release finger while we are dragging then we are releasing the passages to corresponding positions
        /// </summary>
        /// <param name="args"></param>
        public void ReleasedFinger(System.Object[] args)
        {
            if (m_IsTouchOnDragging)
            {
                m_IsTouchOnDragging = false;
                FingerReleasedAfterDragging();
            }

        }

        public void ShowNextPassage(System.Object[] args)
        {
            if(Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED))
                ShowPassage((m_SelectedPassageIndex + 1) % NumberOfPassages);
        }

        public void ShowPreviousPassage(System.Object[] args)
        {
            if(Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED))
                ShowPassage((m_SelectedPassageIndex - 1) < 0 ? (NumberOfPassages - 1): (m_SelectedPassageIndex - 1));
        }

        private float m_SpeedAutoScrollOnY = 1000.0f;
        public void ScrollDown(System.Object[] args)
        {
            if(Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED) && m_SelectedPassageIndex >= 0 && m_SelectedPassageIndex < NumberOfPassages && m_PassageScrollRect != null && m_PassageScrollRect.Length > m_SelectedPassageIndex)
            {
                SetScrollingEnable(m_SelectedPassageIndex, true);
                m_PassageScrollRect[m_SelectedPassageIndex].velocity = new Vector2(0f, m_SpeedAutoScrollOnY);
            }
               
        }

        public void ScrollUp(System.Object[] args)
        {
            if (Cube != null && (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED) && m_SelectedPassageIndex >= 0 && m_SelectedPassageIndex < NumberOfPassages && m_PassageScrollRect != null && m_PassageScrollRect.Length > m_SelectedPassageIndex)
            {
                SetScrollingEnable(m_SelectedPassageIndex, true);
                m_PassageScrollRect[m_SelectedPassageIndex].velocity = new Vector2(0f, -m_SpeedAutoScrollOnY);
            }
               
        }

        public int GetPassageIndexTouch(Vector2 screenPosition)
        {
            int panelIndexToShow = -1;

            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = screenPosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, hits);
            
            
            foreach (RaycastResult hitResult in hits)
            {
                if (hitResult.gameObject.layer == this.gameObject.layer && (string.Equals(hitResult.gameObject.name, m_NameElementPanel) || string.Equals(hitResult.gameObject.name, m_NameElementTabItem) || string.Equals(hitResult.gameObject.name, m_NameElementTabImage)))
                {
                    if (string.Equals(hitResult.gameObject.name, m_NameElementTabImage))
                        int.TryParse(hitResult.gameObject.transform.parent.parent.name.Substring(m_NameElementPassageItem.Length, 2), out panelIndexToShow);
                    else
                        int.TryParse(hitResult.gameObject.transform.parent.name.Substring(m_NameElementPassageItem.Length, 2), out panelIndexToShow);

                    break;
                }
            }

            return panelIndexToShow;

        }
        /// <summary>
        /// If there is a tap on cube side, we are checking the location of the tap to show the corresponding passage
        /// </summary>
        /// <param name="args"></param>
        public void TapOnCubeSide(System.Object[] args)
        {
            if (args != null && args.Length == 2 && args[0] is TouchScript.Gestures.TapGesture && args[1] is RaycastHit)
            {
                if (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED)
                {

                    TouchScript.Gestures.TapGesture tapGesture = args[0] as TouchScript.Gestures.TapGesture;

                    if (EventSystem.current != null) //Without the eveny system we can't raycast properly!
                    {

                        int panelIndexToShow = GetPassageIndexTouch(tapGesture.ScreenPosition);
                        
                        if (panelIndexToShow >= 0)
                        {
                            ShowPassage(panelIndexToShow);
                        }
                        else
                        {
                            Log.Status("PassageAnimationManager", "TapOnCubeSide - Not Hit");
                        }
                    }
                    else
                    {
                        Log.Warning("PassageAnimationManager", "EventSystem couldn't find in the current scene");
                    }

                }
                else
                {
                    //do nothing - we are not instersted the taps in other states
                }
            }
            else
            {
                Log.Warning("PassageAnimationManager", "TapOnCubeSide has invalid arguments!");
            }
        }

        /// <summary>
        /// Event handler for one finger drag on cube - If cube is on focus and drag is on focus side we check if the finger is on passage or not to drag passage
        /// </summary>
        /// <param name="args"></param>
        public void OneFingerDragOnCube(System.Object[] args)
        {
            // Log.Status("PassageAnimationManager", "OneFingerDragOnCube");
            if (args != null && args.Length == 1 && args[0] is TouchScript.Gestures.ScreenTransformGesture)
            {
                TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture = args[0] as TouchScript.Gestures.ScreenTransformGesture;

                if (Cube.AnimationState == CubeAnimationManager.CubeAnimationState.IDLE_AS_FOCUSED)
                {
                    DragOneFingerOnFocusedSide(OneFingerManipulationGesture);
                }
                else
                {
                    //do nothing because cube is not in Focused state!
                }
            }
            else
            {
                Log.Warning("QuestWidget", "OneFingerDragOnCube has invalid arguments");
            }

        }

        /// <summary>
        /// Event handler for full screen drag - We consider as release finger if there was a drag on passage
        /// </summary>
        /// <param name="args"></param>
        public void OneFingerDragFullScreen(System.Object[] args)
        {
            if (m_IsTouchOnDragging)
            {
                m_IsTouchOnDragging = false;
                FingerReleasedAfterDragging();
            }
        }

        #endregion

        #region Passage Path Update

        public void UpdatePassages()
        {

            m_PassageItems = Utility.FindObjects<Transform>(this.gameObject, "PassageItem", isContains: true, sortByName: true);
            m_PassageScrollRect = new ScrollRect[NumberOfPassages];

            //Log.Status("PassageAnimationManager", "Updated Passages with number of {0} passages", NumberOfPassages);

            UpdateBezierPathForPassages();
            m_AnimationLocationRatio = new float[NumberOfPassages];
            m_AnimationRotationRatio = new float[NumberOfPassages];
            m_TargetLocation = new Vector3[NumberOfPassages];
            m_TargetRotation = new Vector3[NumberOfPassages];
            m_AnimationToShowPositionPassage = new LTDescr[NumberOfPassages];
            m_AnimationToShowRotationPassage = new LTDescr[NumberOfPassages];

            for (int i = 0; i < NumberOfPassages; i++)
            {
                m_AnimationLocationRatio[i] = 0.0f;
                m_AnimationRotationRatio[i] = 0.0f;
                m_TargetLocation[i] = m_BezierPathToCenter[i].pts[0];   // PassageList[i].localPosition;
                m_TargetRotation[i] = m_BezierPathOrientationToCenter[i].pts[0];   //PassageList[i].localEulerAngles;
                m_PassageScrollRect[i] = PassageList[i].GetComponentInChildren<ScrollRect>();
                SetScrollingEnable(i, false);
            }

        }

        void UpdateBezierPathForPassages()
        {
            if (m_PassageItems != null && m_PassageItems.Length > 0)
            {
                m_BezierPathToCenter = new LTBezierPath[NumberOfPassages];
                m_BezierPathOrientationToCenter = new LTBezierPath[NumberOfPassages];
                m_BezierPathToStack = new LTBezierPath[NumberOfPassages];
                m_BezierPathOrientationToStack = new LTBezierPath[NumberOfPassages];
                m_BezierPathFromInitialToStack = new LTBezierPath[NumberOfPassages];
                m_BezierPathOrientationFromInitialToStack = new LTBezierPath[NumberOfPassages];

                for (int i = 0; i < NumberOfPassages; i++)
                {
                    m_BezierPathToCenter[i] = new LTBezierPath(new Vector3[]{
                        m_PathToCenterForFirstItem[0] + (m_OffsetPathToCenter * i) + (m_OffsetPathToCenter * (10 - NumberOfPassages)),
                        m_PathToCenterForFirstItem[1] + (m_OffsetPathToCenter * i) + (m_OffsetPathToCenter * (10 - NumberOfPassages)),
                        m_PathToCenterForFirstItem[2] ,
                        m_PathToCenterForFirstItem[3] });

                    m_BezierPathOrientationToCenter[i] = new LTBezierPath(new Vector3[]{
                        m_PathOrientationToCenterForFirstItem[0] + (m_OffsetPathOrientationToCenter * i),
                        m_PathOrientationToCenterForFirstItem[1] + (m_OffsetPathOrientationToCenter * i),
                        m_PathOrientationToCenterForFirstItem[2],
                        m_PathOrientationToCenterForFirstItem[3]});

                    m_BezierPathToStack[i] = new LTBezierPath(new Vector3[]{
                        m_PathToStackForFirstItem[0] ,
                        m_PathToStackForFirstItem[1] ,
                        m_PathToStackForFirstItem[2] + (m_OffsetPathToStack * i),
                        m_PathToStackForFirstItem[3] + (m_OffsetPathToStack * i)});

                    m_BezierPathOrientationToStack[i] = new LTBezierPath(new Vector3[]{
                        m_PathOrientationToStackForFirstItem[0],
                        m_PathOrientationToStackForFirstItem[1],
                        m_PathOrientationToStackForFirstItem[2] + (m_OffsetPathOrientationToStack * i),
                        m_PathOrientationToStackForFirstItem[3] + (m_OffsetPathOrientationToStack * i)});

                    m_BezierPathFromInitialToStack[i] = new LTBezierPath(new Vector3[]{
                        m_BezierPathToCenter[i].pts[0],
                        m_BezierPathToCenter[i].pts[1],
                        m_BezierPathToStack[i].pts[2],
                        m_BezierPathToStack[i].pts[3]});

                    m_BezierPathOrientationFromInitialToStack[i] = new LTBezierPath(new Vector3[]{
                        m_BezierPathOrientationToCenter[i].pts[0],
                        m_BezierPathOrientationToCenter[i].pts[1],
                        m_BezierPathOrientationToStack[i].pts[2],
                        m_BezierPathOrientationToStack[i].pts[3]});
                }
            }

        }
        #endregion

        #region Finger Dragging / Release related actions

        void SetScrollingEnable(int passageIndex, bool enable)
        {
            if (m_PassageScrollRect != null && m_PassageScrollRect.Length > passageIndex && m_PassageScrollRect[passageIndex] != null)
            {
                //m_PassageScrollRect[passageIndex].velocity = Vector2.zero;
                m_PassageScrollRect[passageIndex].vertical = enable;
            }
        }

        void DragOneFingerOnFocusedSide(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture)
        {
            if (NumberOfPassages > 0)
            {
                m_IsTouchOnDragging = true;

                float movingInX = OneFingerManipulationGesture.DeltaPosition.x * m_OneDragModifier;

                if (m_SelectedPassageIndex < 0)
                {
                    m_SelectedPassageIndex = 0;
                }
                else if (m_SelectedPassageIndex >= NumberOfPassages)
                {
                    m_SelectedPassageIndex = NumberOfPassages - 1;
                }

                if (GetPassageIndexTouch(OneFingerManipulationGesture.ScreenPosition) == m_SelectedPassageIndex)
                {
                    SetScrollingEnable(m_SelectedPassageIndex, true); 
                    //do nothing - can't drag because passage is now in scroll mode
                }
                else
                {
                    SetScrollingEnable(m_SelectedPassageIndex, false);
                }
                //else
                //{
                //While finger is dragging on passage , we are changing the percent according to X location change
                m_AnimationLocationRatio[m_SelectedPassageIndex] = Mathf.Clamp01(m_AnimationLocationRatio[m_SelectedPassageIndex] + movingInX);
                m_AnimationRotationRatio[m_SelectedPassageIndex] = Mathf.Clamp01(m_AnimationRotationRatio[m_SelectedPassageIndex] + movingInX);

                SetTargetLocationAndRotationOfSelectedPassage();

                
                //}

            }
            else
            {
                Log.Status("PassageAnimationManager", "NO PASSAGE - DragOneFingerOnPassage: {0}", OneFingerManipulationGesture.DeltaPosition);
            }
        }

        /// <summary>
        /// After setting some ratio values on selected passage, we are checking the most accurate possible location and rotation for that passage
        /// </summary>
        void SetTargetLocationAndRotationOfSelectedPassage()
        {
            if (m_SelectedPassageIndex < 0 || m_SelectedPassageIndex >= NumberOfPassages)
                return;

            LTBezierPath m_BezierPathCurrent;
            LTBezierPath m_BezierPathOrientationCurrent;

            float m_RatioBezierPathPassage = 0.0f;
            if (m_AnimationLocationRatio[m_SelectedPassageIndex] <= 0.5f)
            {
                m_RatioBezierPathPassage = m_AnimationLocationRatio[m_SelectedPassageIndex] * 2.0f;
                m_BezierPathCurrent = m_BezierPathToCenter[m_SelectedPassageIndex];
                m_BezierPathOrientationCurrent = m_BezierPathOrientationToCenter[m_SelectedPassageIndex];
            }
            else
            {
                m_RatioBezierPathPassage = (m_AnimationLocationRatio[m_SelectedPassageIndex] - 0.5f) * 2.0f;
                m_BezierPathCurrent = m_BezierPathToStack[m_SelectedPassageIndex];
                m_BezierPathOrientationCurrent = m_BezierPathOrientationToStack[m_SelectedPassageIndex];
            }

            m_TargetLocation[m_SelectedPassageIndex] = m_BezierPathCurrent.point(m_RatioBezierPathPassage);
            m_TargetRotation[m_SelectedPassageIndex] = m_BezierPathOrientationCurrent.point(m_RatioBezierPathPassage);

        }

        /// <summary>
        /// After releasing the finger, we are checking the current percent of the passage to decide what to do.
        /// </summary>
        private void FingerReleasedAfterDragging()
        {
            if (!m_IsTouchOnDragging && m_SelectedPassageIndex >= 0)
            {
                bool canScroll = false;
                int prevSelectedPassageIndex = m_SelectedPassageIndex;

                if (m_AnimationLocationRatio[m_SelectedPassageIndex] < m_PercentToGoInitialPosition)
                {
                    ShowPassage(m_SelectedPassageIndex - 1);
                }
                else if (m_AnimationLocationRatio[m_SelectedPassageIndex] > m_PercentToGoStackPosition)
                {
                    ShowPassage(m_SelectedPassageIndex + 1);
                }
                else
                {
                    m_AnimationLocationRatio[m_SelectedPassageIndex] = 0.5f;
                    m_AnimationRotationRatio[m_SelectedPassageIndex] = 0.5f;
                    SetTargetLocationAndRotationOfSelectedPassage();
                    canScroll = true;
                }

                //if (m_PassageScrollRect != null && m_PassageScrollRect.Length > prevSelectedPassageIndex && m_PassageScrollRect[prevSelectedPassageIndex] != null)
                //{
                //    m_PassageScrollRect[prevSelectedPassageIndex].velocity = Vector2.zero;
                //    m_PassageScrollRect[prevSelectedPassageIndex].vertical = canScroll;
                //}
            }
        }

        #endregion

        #region Showing particular passage 

        private LTBezierPath getBezierPathFromInitialValue(Vector3[] currentPath, Vector3 initialValue, float percent = 0.2f)
        {

            return new LTBezierPath(new Vector3[] {
                    initialValue,
                    Vector3.Lerp(initialValue, currentPath[3], percent),
                    Vector3.Lerp(initialValue, currentPath[3], 1.0f - percent),
                    currentPath[3]
            });
        }

        private LTBezierPath getBezierPathToLastValue(Vector3[] currentPath, Vector3 lastValue, float percent = 0.2f)
        {

            return new LTBezierPath(new Vector3[] {
                    currentPath[0],
                    Vector3.Lerp(currentPath[0], lastValue, percent),
                    Vector3.Lerp(currentPath[0], lastValue, 1.0f - percent),
                    lastValue
            });
        }


        private void ShowPassage(int passageIndexToShow)
        {
            StopAnimations();

            m_PreviousPassageIndex = m_SelectedPassageIndex;
            m_SelectedPassageIndex = passageIndexToShow;

            if (m_AnimationToShowPositionPassage == null || m_AnimationToShowPositionPassage.Length != NumberOfPassages)
                m_AnimationToShowPositionPassage = new LTDescr[NumberOfPassages];

            if (m_AnimationToShowRotationPassage == null || m_AnimationToShowRotationPassage.Length != NumberOfPassages)
                m_AnimationToShowRotationPassage = new LTDescr[NumberOfPassages];


            for (int i = 0; i < NumberOfPassages; i++)
            {
                bool canScroll = false;
                if (PassageList[i] == null || PassageList[i].transform == null)
                {
                    Log.Warning("PassageAnimationManager", "PassageList doesn't have the element index: {0}", i);
                    continue;
                }
                else
                {
                    //Going to initial position if they are in different position
                    if (i > passageIndexToShow)
                    {
                        LTBezierPath pathFromCurrentPosition = getBezierPathToLastValue(m_BezierPathFromInitialToStack[i].pts, m_TargetLocation[i]);
                        AnimatePassageToGivenRatio(m_AnimationTimeForEachPassageToGoTheirLocation, m_DelayBetweenPassages * Mathf.Abs(m_PreviousPassageIndex - i), m_LeanTypeForPassageMovement, i, m_AnimationLocationRatio[i], 0.0f, pathFromCurrentPosition, m_BezierPathOrientationFromInitialToStack[i]);
                        PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);
                    }
                    else if (i < passageIndexToShow)
                    {
                        LTBezierPath pathFromCurrentPosition = getBezierPathFromInitialValue(m_BezierPathFromInitialToStack[i].pts, m_TargetLocation[i]);
                        AnimatePassageToGivenRatio(m_AnimationTimeForEachPassageToGoTheirLocation, m_DelayBetweenPassages * Mathf.Abs(m_PreviousPassageIndex - i), m_LeanTypeForPassageMovement, i, m_AnimationLocationRatio[i], 1.0f, pathFromCurrentPosition, m_BezierPathOrientationFromInitialToStack[i]);
                        PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);
                    }
                    else
                    {
                        if (m_PreviousPassageIndex > passageIndexToShow)
                            PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);


                        LTBezierPath pathToMove = m_AnimationLocationRatio[i] <= 0.5f ? m_BezierPathToCenter[i] : m_BezierPathToStack[i];
                        LTBezierPath pathToRotate = m_AnimationRotationRatio[i] <= 0.5f ? m_BezierPathOrientationToCenter[i] : m_BezierPathOrientationToStack[i];
                        float targetRatio = m_AnimationLocationRatio[i] <= 0.5f ? 1.0f : 0.0f;
                        float currentRatio = m_AnimationLocationRatio[i] <= 0.5f ? 0.0f : 1.0f; ; // m_AnimationLocationRatio[i] <= 0.5f ? (m_AnimationLocationRatio[i] * 2.0f) : ((m_AnimationLocationRatio[i] - 0.5f) * 2.0f);

                        if (targetRatio == 1.0f)
                        {
                            pathToMove = getBezierPathFromInitialValue(pathToMove.pts, PassageList[i].localPosition);
                            pathToRotate = getBezierPathFromInitialValue(pathToRotate.pts, new Vector3(PassageList[i].localEulerAngles.x, PassageList[i].localEulerAngles.y, 0.0f));
                        }
                        else
                        {
                            pathToMove = getBezierPathToLastValue(pathToMove.pts, PassageList[i].localPosition);
                            pathToRotate = getBezierPathToLastValue(pathToRotate.pts, new Vector3(PassageList[i].localEulerAngles.x, PassageList[i].localEulerAngles.y, 0.0f));
                        }

                        AnimatePassageToGivenRatio(m_AnimationTimeForEachPassageToGoTheirLocation, (m_DelayBetweenPassages * Mathf.Abs(m_PreviousPassageIndex - i)) + m_DelayExtraOnSelectedPassage, m_LeanTypeForPassageMovement, i, currentRatio, targetRatio, pathToMove, pathToRotate, isUsingTwoAnimations: true);
                        //canScroll = true;
                    }
                }

                //if (m_PassageScrollRect != null && m_PassageScrollRect.Length > i && m_PassageScrollRect[i] != null)
                //{
                //    m_PassageScrollRect[i].velocity = Vector2.zero;
                //    m_PassageScrollRect[i].vertical = canScroll;
                //}
            }

        }


        private void AnimatePassageToGivenRatio(float animationTime, float delayOnPassage, LeanTweenType leanType, int passageIndex, float currentRatio, float targetRatio, LTBezierPath bezierPathToMove, LTBezierPath bezierPathToRotate, bool isUsingTwoAnimations = false)
        {

            float timeModifier = Mathf.Abs(targetRatio - currentRatio);

            if (m_AnimationLocationRatio[passageIndex] != targetRatio || m_AnimationRotationRatio[passageIndex] != targetRatio)
            {

                bool hasChangeSiblingIndex = false;
                m_AnimationToShowPositionPassage[passageIndex] = LeanTween.value(PassageList[passageIndex].gameObject, currentRatio, targetRatio, animationTime * timeModifier).setDelay(delayOnPassage).setEase(leanType).setOnUpdate(
                (float f) =>
                {
                    m_TargetLocation[passageIndex] = bezierPathToMove.pointNotNAN(f);
                    if (isUsingTwoAnimations)
                    {
                        if (targetRatio == 1.0f)    //this is when passage goes from initial to center
                        {
                            m_AnimationLocationRatio[passageIndex] = f / 2.0f;
                        }
                        else if (targetRatio == 0.0f)
                        {
                            m_AnimationLocationRatio[passageIndex] = f / 2.0f + 0.5f;
                        }
                        else
                        {
                            Log.Warning("PassageAnimationMaanger", "Unknown Target ratio to animate Passages");
                        }
                    }
                    else
                    {
                        m_AnimationLocationRatio[passageIndex] = f;
                    }

                    if (Mathf.Abs(f - targetRatio) < 0.07f && !hasChangeSiblingIndex)
                    {
                        hasChangeSiblingIndex = true;
                        if (isUsingTwoAnimations)
                        {
                            PassageList[passageIndex].SetSiblingIndex(NumberOfPassages);
                        }
                        else
                        {
                            PassageList[passageIndex].SetSiblingIndex(NumberOfPassages - 1 - passageIndex);
                        }
                           
                    }

                }).setOnComplete(()=>
                {
                    //if (m_PassageScrollRect != null && m_PassageScrollRect.Length > passageIndex && m_PassageScrollRect[passageIndex] != null)
                    //{
                    //    m_PassageScrollRect[passageIndex].velocity = Vector2.zero;
                    //    m_PassageScrollRect[passageIndex].vertical = true;
                    //}
                });

            }
            else
            {
                //no ned to create movement animation - passage is already in initial position.
            }

            if (m_AnimationLocationRatio[passageIndex] != targetRatio || m_AnimationRotationRatio[passageIndex] != targetRatio)
            {
                m_AnimationToShowRotationPassage[passageIndex] = LeanTween.value(PassageList[passageIndex].gameObject, currentRatio, targetRatio, animationTime * timeModifier).setDelay(delayOnPassage).setEase(leanType).setOnUpdate(
                    (float f) =>
                    {
                        m_TargetRotation[passageIndex] = bezierPathToRotate.pointNotNAN(f);

                        if (isUsingTwoAnimations)
                        {
                            if (targetRatio == 1.0f)    //this is when passage goes from initial to center
                            {
                                m_AnimationRotationRatio[passageIndex] = f / 2.0f;
                            }
                            else if (targetRatio == 0.0f)
                            {
                                m_AnimationRotationRatio[passageIndex] = f / 2.0f + 0.5f;
                            }
                            else
                            {
                                Log.Warning("PassageAnimationMaanger", "Unknown Target ratio to animate Passages");
                            }
                        }
                        else
                        {
                            m_AnimationRotationRatio[passageIndex] = f;
                        }

                    });
            }
            else
            {
                //No need to create rotation animation - passage is already in initial rotation.
            }
        }

        #endregion

        #region General Animation Related actions - Stop Animations

        private void StopAnimations()
        {
            if (m_AnimationToShowPositionPassage != null)
            {
                for (int i = 0; i < m_AnimationToShowPositionPassage.Length; i++)
                {
                    if (m_AnimationToShowPositionPassage[i] != null)
                    {
                        m_AnimationToShowPositionPassage[i].hasUpdateCallback = false;
                        LeanTween.cancel(m_AnimationToShowPositionPassage[i].uniqueId);
                        //m_AnimationToShowPositionPassage[i] = null;
                    }
                    else
                    {
                        // Log.Warning("PassageAnimationManager", "There is no m_AnimationToShowPositionPassage defined for animation: {0} ", i);
                    }

                }
            }

            if (m_AnimationToShowRotationPassage != null)
            {
                for (int i = 0; i < m_AnimationToShowRotationPassage.Length; i++)
                {
                    if (m_AnimationToShowRotationPassage[i] != null)
                    {
                        m_AnimationToShowRotationPassage[i].hasUpdateCallback = false;
                        LeanTween.cancel(m_AnimationToShowRotationPassage[i].uniqueId);
                        // m_AnimationToShowRotationPassage[i] = null;
                    }
                    else
                    {
                        //  Log.Warning("PassageAnimationManager", "There is no m_AnimationToShowRotationPassage defined for animation: {0} ", i);
                    }

                }
            }
        }

        #endregion

    }

}