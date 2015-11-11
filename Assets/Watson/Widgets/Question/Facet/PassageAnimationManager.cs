using IBM.Watson.Utilities;
using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
    public class PassageAnimationManager : WatsonBaseAnimationManager
    {

        #region Private Members

        private CubeAnimationManager m_CubeAnimMgr = null;
        private QuestionWidget m_QuestionWidget = null;
        private Transform[] m_PassageItems = null;
        private int m_LastFrameOneFingerDrag = 0;   //Used to identify the release finger

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

        public Transform[] PassageList
        {
            get
            {
                if (m_PassageItems == null)
                {
                    UpdatePassages();
                    if (m_PassageItems == null)
                    {
                        Log.Error("PassageAnimationManager", "PassageList couldn't find inside gameobject");
                    } 
                }

                return m_PassageItems;
            }
        }

        public int NumberOfPassages
        {
            get
            {
                int numberOfPassage = 0;
                if (PassageList != null && PassageList.Length > 0)
                    numberOfPassage = PassageList.Length;
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

            UpdatePassages();
        }


        // Update is called once per frame
        void Update()
        {
            //DragOneFingerOnPassageOnUpdate();
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ShowPassage(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowPassage(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ShowPassage(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShowPassage(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShowPassage(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShowPassage(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ShowPassage(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ShowPassage(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ShowPassage(8);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                ShowPassage(9);
            }
        }

        #endregion


        #region Events on Passage

        public void ReleasedFinger(System.Object[] args)
        {
            Log.Status("PassageAnimationManager", "ReleasedFinger");
            m_LastFrameOneFingerDrag = -1;
        }


        public void OneFingerDragOnCube(System.Object[] args)
        {
            Log.Status("PassageAnimationManager", "OneFingerDragOnCube");
        }

        public void TapOnCubeSide(System.Object[] args)
        {
            // Cube.SideFocused
            Log.Status("PassageAnimationManager", "TapOnCubeSide");
        }

        #endregion

        #region Passage Path Update
        
        void UpdatePassages()
        {
            m_PassageItems = Utility.FindObjects<Transform>(this.gameObject, "PassageItem", isContains: true, sortByName: true);
            UpdateBezierPathForPassages();
            m_AnimationLocationRatio = new float[NumberOfPassages];
            m_AnimationRotationRatio = new float[NumberOfPassages];
            for (int i = 0; i < NumberOfPassages; i++)
            {
                m_AnimationLocationRatio[i] = 0.0f;
                m_AnimationRotationRatio[i] = 0.0f;
            }

           

        }

        void UpdateBezierPathForPassages()
        {
            if(NumberOfPassages > 0)
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
                        m_PathToCenterForFirstItem[0] + (m_OffsetPathToCenter * i),
                        m_PathToCenterForFirstItem[1] + (m_OffsetPathToCenter * i),
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
                        m_PathToCenterForFirstItem[0]  + (m_OffsetPathToCenter * i),
                        m_PathToCenterForFirstItem[1]  + (m_OffsetPathToCenter * i),
                        m_PathToStackForFirstItem[2] + (m_OffsetPathToStack * i),
                        m_PathToStackForFirstItem[3] + (m_OffsetPathToStack * i)});

                    m_BezierPathOrientationFromInitialToStack[i] = new LTBezierPath(new Vector3[]{
                        m_PathOrientationToCenterForFirstItem[0] + (m_OffsetPathOrientationToCenter * i),
                        m_PathOrientationToCenterForFirstItem[1] + (m_OffsetPathOrientationToCenter * i),
                        m_PathOrientationToStackForFirstItem[2] + (m_OffsetPathOrientationToStack * i),
                        m_PathOrientationToStackForFirstItem[3] + (m_OffsetPathOrientationToStack * i)});
                }
            }
            
        }
        #endregion

        

        //[SerializeField]
        private float m_OneDragModifier = 0.002f;
        private int m_SelectedPassageIndex = -1;
        private float m_PercentCurrentPassage = 0.0f;

        private Vector3 worldOnPath = Vector3.up;

        public void DragOneFingerOnPassage(TouchScript.Gestures.ScreenTransformGesture OneFingerManipulationGesture)
        {

            if (m_PassageItems == null)
            {
                m_PassageItems = Utility.FindObjects<Transform>(this.gameObject, "PassageItem", isContains: true, sortByName: true);
            }

            if (m_PassageItems != null)
            {
                float movingInX = OneFingerManipulationGesture.DeltaPosition.x * m_OneDragModifier;

                if (m_SelectedPassageIndex != -1)
                {

                }
                else
                {
                    m_PercentCurrentPassage = Mathf.Clamp01(m_PercentCurrentPassage + movingInX);
                }


            }
            else
            {
                Log.Status("CubeAnimationManager", "NO PASSAGE - DragOneFingerOnPassage: {0}", OneFingerManipulationGesture.DeltaPosition);
            }
        }

        //[SerializeField]
        private float m_SpeedPassageAnimation = 4.0f;
        private float m_PercentToGoInitialPosition = 0.25f;
        private float m_PercentToGoStackPosition = 0.75f;
        private void DragOneFingerOnPassageOnUpdate()
        {

            if (m_PassageItems != null)
            {

                if (m_LastFrameOneFingerDrag < 0)
                {
                    if (m_PercentCurrentPassage > m_PercentToGoInitialPosition && m_PercentCurrentPassage < m_PercentToGoStackPosition)
                    {
                        m_PercentCurrentPassage = 0.5f;
                    }
                    else if (m_PercentCurrentPassage < m_PercentToGoInitialPosition)
                    {
                        m_PercentCurrentPassage = 0.0f;
                    }
                    else
                    {
                        m_PercentCurrentPassage = 1.0f;
                    }
                }

               
                for (int i = 0; i < m_PassageItems.Length; i++)
                {
                    if (i < m_SelectedPassageIndex)
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_TargetLocation[i], Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_TargetRotation[i]), Time.deltaTime * m_SpeedPassageAnimation);
                        
                        //m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToCenter[i].point(0.0f), Time.deltaTime * m_SpeedPassageAnimation);
                        //m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToCenter[i].point(0.0f)), Time.deltaTime * m_SpeedPassageAnimation);
                    }
                    else if (i > m_SelectedPassageIndex)
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_TargetLocation[i], Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_TargetRotation[i]), Time.deltaTime * m_SpeedPassageAnimation);

                        //m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToStack[i].point(1.0f), Time.deltaTime * m_SpeedPassageAnimation);
                        //m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToStack[i].point(1.0f)), Time.deltaTime * m_SpeedPassageAnimation);
                    }
                    else
                    {
                        LTBezierPath m_BezierPathCurrent;
                        LTBezierPath m_BezierPathOrientationCurrent;
                        float m_RatioBezierPathPassage = 0.0f;
                        if (m_PercentCurrentPassage <= 0.5f)
                        {
                            m_RatioBezierPathPassage = m_PercentCurrentPassage * 2.0f;
                            m_BezierPathCurrent = m_BezierPathToCenter[i];
                            m_BezierPathOrientationCurrent = m_BezierPathOrientationToCenter[i];
                        }
                        else
                        {
                            m_RatioBezierPathPassage = (m_PercentCurrentPassage - 0.5f) * 2.0f;
                            m_BezierPathCurrent = m_BezierPathToStack[i];
                            m_BezierPathOrientationCurrent = m_BezierPathOrientationToStack[i];
                        }

                        m_TargetLocation[i] = m_BezierPathCurrent.point(m_RatioBezierPathPassage);
                        m_TargetRotation[i] = m_BezierPathOrientationCurrent.point(m_RatioBezierPathPassage);

                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_TargetLocation[i], Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_TargetRotation[i]), Time.deltaTime * m_SpeedPassageAnimation);

                    }
                }

            }
        }

        private LTBezierPath getBezierPathFromInitialValue(Vector3[] currentPath, Vector3 initialValue, float percent = 0.2f)
        {

            return new LTBezierPath(new Vector3[] {
                    initialValue,
                    Vector3.Lerp(initialValue, currentPath[3], percent),
                    Vector3.Lerp(initialValue, currentPath[3], 1.0f - percent),
                    currentPath[3]
            });
        }

        private LTDescr[] m_AnimationToShowPositionPassage;
        private LTDescr[] m_AnimationToShowRotationPassage;
        private int m_PreviousPassageIndex = 0;
        private void ShowPassage(int passageIndexToShow)
        {
           // UnityEngine.Debug.Break();
            Log.Status("PassageAnimationManager", "ShowPassage : {0}, PreviousOne: {1}", passageIndexToShow, m_PreviousPassageIndex);

            StopAnimations();

            if (m_AnimationToShowPositionPassage == null)
                m_AnimationToShowPositionPassage = new LTDescr[NumberOfPassages];

            if (m_AnimationToShowRotationPassage == null)
                m_AnimationToShowRotationPassage = new LTDescr[NumberOfPassages];

            float animationTime = 1.0f;
            float delayOnPassage = 0.1f;
            LeanTweenType leanType = LeanTweenType.easeOutCirc;

            for (int i = 0; i < NumberOfPassages; i++)
            {

                //Going to initial position if they are in different position
                if (i > passageIndexToShow)
                {
                    //LTBezierPath pathFromCurrentPosition = getBezierPathFromInitialValue(m_BezierPathFromInitialToStack[i].pts, PassageList[i].localPosition);
                    //LTBezierPath pathFromCurrentRotation = getBezierPathFromInitialValue(m_BezierPathOrientationFromInitialToStack[i].pts, PassageList[i].localEulerAngles);

                    AnimatePassageToGivenRatio(animationTime, delayOnPassage * Mathf.Abs(m_PreviousPassageIndex - i), leanType, i, m_AnimationLocationRatio[i], 0.0f, m_BezierPathFromInitialToStack[i], m_BezierPathOrientationFromInitialToStack[i]);
                    //PassageList[i].SetAsFirstSibling();
                    PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);
                }
                else if (i < passageIndexToShow)
                {
                    //LTBezierPath pathFromCurrentPosition = getBezierPathFromInitialValue(m_BezierPathFromInitialToStack[i].pts, PassageList[i].localPosition);
                    //LTBezierPath pathFromCurrentRotation = getBezierPathFromInitialValue(m_BezierPathOrientationFromInitialToStack[i].pts, PassageList[i].localEulerAngles);

                    AnimatePassageToGivenRatio(animationTime, delayOnPassage * Mathf.Abs(m_PreviousPassageIndex - i), leanType, i, m_AnimationLocationRatio[i], 1.0f, m_BezierPathFromInitialToStack[i], m_BezierPathOrientationFromInitialToStack[i]);
                    // PassageList[i].SetAsFirstSibling();
                    PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);
                }
                else
                {
                    if(m_PreviousPassageIndex > passageIndexToShow)
                        PassageList[i].SetSiblingIndex(NumberOfPassages - 1 - i);
                    //PassageList[i].SetSiblingIndex(NumberOfPassages);

                    LTBezierPath pathToMove = m_AnimationLocationRatio[i] <= 0.5f ? m_BezierPathToCenter[i] : m_BezierPathToStack[i];
                    LTBezierPath pathToRotate = m_AnimationRotationRatio[i] <= 0.5f ? m_BezierPathOrientationToCenter[i] : m_BezierPathOrientationToStack[i];
                    float targetRatio = m_AnimationLocationRatio[i] <= 0.5f ? 1.0f : 0.0f;
                    float currentRatio = m_AnimationLocationRatio[i] <= 0.5f ? (m_AnimationLocationRatio[i] * 2.0f) : ((m_AnimationLocationRatio[i] - 0.5f) * 2.0f);

                    //pathToMove = getBezierPathFromInitialValue(pathToMove.pts, PassageList[i].localPosition);
                    //pathToRotate = getBezierPathFromInitialValue(pathToRotate.pts, PassageList[i].localEulerAngles);
                    
                    //PassageList[i].SetAsLastSibling();
                    AnimatePassageToGivenRatio(animationTime, delayOnPassage * Mathf.Abs(m_PreviousPassageIndex - i), leanType, i, currentRatio, targetRatio, pathToMove, pathToRotate, isUsingTwoAnimations: true);
                }
                //m_AnimationToShowRotationPassage[i] =

                //m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToCenter[i].point(0.0f), Time.deltaTime * m_SpeedPassageAnimation);
                //m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToCenter[i].point(0.0f)), Time.deltaTime * m_SpeedPassageAnimation);

            }
            m_PreviousPassageIndex = passageIndexToShow;
        }


        
        private void AnimatePassageToGivenRatio(float animationTime, float delayOnPassage, LeanTweenType leanType, int passageIndex, float currentRatio, float targetRatio, LTBezierPath bezierPathToMove, LTBezierPath bezierPathToRotate, bool isUsingTwoAnimations = false)
        {

            float timeModifier = Mathf.Abs(targetRatio - currentRatio);

            if (m_AnimationLocationRatio[passageIndex] != targetRatio)
            {

                bool hasChangeSiblingIndex = false;
                m_AnimationToShowPositionPassage[passageIndex] = LeanTween.value(PassageList[passageIndex].gameObject, currentRatio, targetRatio, animationTime * timeModifier).setDelay(delayOnPassage).setEase(leanType).setOnUpdate(
                (float f) =>
                {
                    //PassageList[passageIndex].localPosition = bezierPathToMove.pointNotNAN(f);
                    m_TargetLocation[passageIndex] = bezierPathToMove.pointNotNAN(f);
                    if (isUsingTwoAnimations)
                    {
                        if (targetRatio == 1.0f)    //this is when passage goes from initial to center
                        {
                            m_AnimationLocationRatio[passageIndex] = f / 2.0f;
                        }
                        else if( targetRatio == 0.0f)
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

                    if( Mathf.Abs(f - targetRatio) < 0.05f && !hasChangeSiblingIndex)
                    {
                        hasChangeSiblingIndex = true;
                        if (isUsingTwoAnimations)
                            PassageList[passageIndex].SetSiblingIndex(NumberOfPassages);
                        else
                            PassageList[passageIndex].SetSiblingIndex(NumberOfPassages - 1 - passageIndex);
                    }
                    
                }).setOnComplete(()=> {
                    //if (isUsingTwoAnimations)
                    //    PassageList[passageIndex].SetSiblingIndex(NumberOfPassages);
                    //else
                    //    PassageList[passageIndex].SetSiblingIndex(NumberOfPassages - 1 - passageIndex);
                });
                
            }
            else
            {
                //no ned to create movement animation - passage is already in initial position.
            }

            if (m_AnimationRotationRatio[passageIndex] != targetRatio)
            {
                m_AnimationToShowRotationPassage[passageIndex] = LeanTween.value(PassageList[passageIndex].gameObject, currentRatio, targetRatio, animationTime * timeModifier).setDelay(delayOnPassage).setEase(leanType).setOnUpdate(
                    (float f) =>
                    {
                        //Log.Status("PassageAnimationManager", "Rotation : {0} at {1}  - pts: {2}-{3}-{4}-{5} ", bezierPathToRotate.pointNotNAN(f), f, bezierPathToRotate.pts[0], bezierPathToRotate.pts[1], bezierPathToRotate.pts[2], bezierPathToRotate.pts[3]);
                        //PassageList[passageIndex].localEulerAngles = bezierPathToRotate.pointNotNAN(f);
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

        private void StopAnimations()
        {
            if(m_AnimationToShowPositionPassage != null)
            {
                for (int i = 0; i < m_AnimationToShowPositionPassage.Length; i++)
                {
                    if (m_AnimationToShowPositionPassage[i] != null)
                    {
                        m_AnimationToShowRotationPassage[i].hasUpdateCallback = false;
                        LeanTween.cancel(m_AnimationToShowPositionPassage[i].uniqueId);
                    }
                    else
                        Log.Warning("PassageAnimationManager", "There is no animation defined for animation: {0} ", i);
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
                    }
                    else
                        Log.Warning("PassageAnimationManager", "There is no animation defined for animation: {0} ", i);
                }
            }
        }
        
    }
    
}