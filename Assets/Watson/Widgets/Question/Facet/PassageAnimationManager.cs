using IBM.Watson.Utilities;
using UnityEngine;
using System.Collections;
using IBM.Watson.Logging;

namespace IBM.Watson.Widgets.Question
{
    public class PassageAnimationManager : WatsonBaseAnimationManager
    {
        
        //Passage One Finger Animation
        Vector3[] m_PathToCenterForFirstItem;
        Vector3[] m_PathOrientationToCenterForFirstItem;
        Vector3[] m_PathToStackForFirstItem;
        Vector3[] m_PathOrientationToStackForFirstItem;

        LTBezierPath[] m_BezierPathToCenter;
        LTBezierPath[] m_BezierPathOrientationToCenter;
        LTBezierPath[] m_BezierPathToStack;
        LTBezierPath[] m_BezierPathOrientationToStack;
        
        private int m_LastFrameOneFingerDrag = 0;

        private CubeAnimationManager m_CubeAnimMgr = null;
        private QuestionWidget m_QuestionWidget = null;
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

        // Use this for initialization
        void Awake()
        {
            Vector3[] m_PathToCenterForFirstItem = new Vector3[]{
            new Vector3(-555,   -77,    -125),
            new Vector3(-600,   0,      -125),
            new Vector3(-800,   300,    -125),
            new Vector3(-907,   355,    -125)};

            Vector3[] m_PathOrientationToCenterForFirstItem = new Vector3[]{
            new Vector3(0,0,0),
            new Vector3(10,10,0),
            new Vector3(35,35,0),
            new Vector3(45,45,0)};

            Vector3[] m_PathToStackForFirstItem = new Vector3[]{
                new Vector3(-907,   355f,       -125),
                new Vector3(-800,   300f,       -200),
                new Vector3(450,    -100f,      -350),
                new Vector3(575,    -120,       -407)};

            Vector3[] m_PathOrientationToStackForFirstItem = new Vector3[]{
                new Vector3(45,45,0),
                new Vector3(35,35,0),
                new Vector3(10,10,0),
                new Vector3(0,0,0)};
        }

        void UpdateBezierPathForPassages()
        {
            int numberOfPassages = 10;

            Vector3 offsetPathToCenter = new Vector3(0,0,0);
            Vector3 offsetPathOrientationToCenter = new Vector3(0, 0, 0);
            Vector3 offsetPathToStack = new Vector3(0, 0, 0);
            Vector3 offsetPathOrientationToStack = new Vector3(0, 0, 0);

            m_BezierPathToCenter = new LTBezierPath[numberOfPassages];
            m_BezierPathOrientationToCenter = new LTBezierPath[numberOfPassages];
            m_BezierPathToStack = new LTBezierPath[numberOfPassages];
            m_BezierPathOrientationToStack = new LTBezierPath[numberOfPassages];

            for (int i = 0; i < numberOfPassages; i++)
            {
                m_BezierPathToCenter[i] = new LTBezierPath(new Vector3[]{
                    m_PathToCenterForFirstItem[0] + (offsetPathToCenter * i),
                    m_PathToCenterForFirstItem[1] + (offsetPathToCenter * i),
                    m_PathToCenterForFirstItem[2] + (offsetPathToCenter * i),
                    m_PathToCenterForFirstItem[3] + (offsetPathToCenter * i)});

                m_BezierPathOrientationToCenter[i] = new LTBezierPath(new Vector3[]{
                    m_PathOrientationToCenterForFirstItem[0] + (offsetPathOrientationToCenter * i),
                    m_PathOrientationToCenterForFirstItem[1] + (offsetPathOrientationToCenter * i),
                    m_PathOrientationToCenterForFirstItem[2] + (offsetPathOrientationToCenter * i),
                    m_PathOrientationToCenterForFirstItem[3] + (offsetPathOrientationToCenter * i)});
                
                m_BezierPathToStack[i] = new LTBezierPath(new Vector3[]{
                    m_PathToStackForFirstItem[0] + (offsetPathToStack * i),
                    m_PathToStackForFirstItem[1] + (offsetPathToStack * i),
                    m_PathToStackForFirstItem[2] + (offsetPathToStack * i),
                    m_PathToStackForFirstItem[3] + (offsetPathToStack * i)});

                m_BezierPathOrientationToStack[i] = new LTBezierPath(new Vector3[]{
                    m_PathOrientationToStackForFirstItem[0] + (offsetPathOrientationToStack * i),
                    m_PathOrientationToStackForFirstItem[1] + (offsetPathOrientationToStack * i),
                    m_PathOrientationToStackForFirstItem[2] + (offsetPathOrientationToStack * i),
                    m_PathOrientationToStackForFirstItem[3] + (offsetPathOrientationToStack * i)});
            }
        
        }

        // Update is called once per frame
        void Update()
        {
            DragOneFingerOnPassageOnUpdate();
        }




        #region Events on Passage

        public void ReleasedFinger(System.Object[] args)
        {
            TouchScript.Gestures.ReleaseGesture releaseGesture = null;
            m_LastFrameOneFingerDrag = -1;
        }


        public void OneFingerDragOnCube(System.Object[] args)
        {

        }

        public void TapOnCubeSide(System.Object[] args)
        {
           // Cube.SideFocused
        }
        
        #endregion

        private Transform[] m_PassageItems = null;

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

            if (m_PassageItems != null && m_PassageItems[0] != null)
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
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToCenter[i].point(0.0f), Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToCenter[i].point(0.0f)), Time.deltaTime * m_SpeedPassageAnimation);
                    }
                    else if (i > m_SelectedPassageIndex)
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToStack[i].point(1.0f), Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToStack[i].point(1.0f)), Time.deltaTime * m_SpeedPassageAnimation);
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
                        
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathCurrent.point(m_RatioBezierPathPassage), Time.deltaTime * m_SpeedPassageAnimation);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationCurrent.point(m_RatioBezierPathPassage)), Time.deltaTime * m_SpeedPassageAnimation);

                    }
                }

            }
        }

        private LTDescr[] m_AnimationToShowPositionPassage;
        private LTDescr[] m_AnimationToShowRotationPassage;
        private void ShowPassage(int passageIndexToShow)
        {
            StopAnimations();

            if (m_AnimationToShowPositionPassage == null)
                m_AnimationToShowPositionPassage = new LTDescr[m_PassageItems.Length];

            if (m_AnimationToShowRotationPassage == null)
                m_AnimationToShowRotationPassage = new LTDescr[m_PassageItems.Length];


            for (int i = 0; i < m_PassageItems.Length; i++)
            {
                m_AnimationToShowPositionPassage[i] = LeanTween.moveLocal(m_PassageItems[i].gameObject, m_BezierPathToCenter[i], 1.0f).setDelay(i * 0.1f).setOnComplete(() => 
                {
                    m_AnimationToShowPositionPassage[i] = null;
                //    m_AnimationToShowPositionPassage[i] = 
                });
                //m_AnimationToShowRotationPassage[i] =

                //m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToCenter[i].point(0.0f), Time.deltaTime * m_SpeedPassageAnimation);
                //m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToCenter[i].point(0.0f)), Time.deltaTime * m_SpeedPassageAnimation);

            }
        }

        private void StopAnimations()
        {
            if(m_AnimationToShowPositionPassage != null)
            {
                for (int i = 0; i < m_AnimationToShowPositionPassage.Length; i++)
                {
                    if (m_AnimationToShowPositionPassage[i] != null)
                        LeanTween.cancel(m_AnimationToShowPositionPassage[i].uniqueId);
                    else
                        Log.Warning("PassageAnimationManager", "There is no animation defined for animation: {0} ", i);
                }
            }

            if (m_AnimationToShowRotationPassage != null)
            {
                for (int i = 0; i < m_AnimationToShowRotationPassage.Length; i++)
                {
                    if (m_AnimationToShowRotationPassage[i] != null)
                        LeanTween.cancel(m_AnimationToShowRotationPassage[i].uniqueId);
                    else
                        Log.Warning("PassageAnimationManager", "There is no animation defined for animation: {0} ", i);
                }
            }
        }
        
    }
    
}