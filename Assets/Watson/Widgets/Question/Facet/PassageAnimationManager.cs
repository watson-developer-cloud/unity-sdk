using IBM.Watson.Utilities;
using UnityEngine;
using System.Collections;


namespace IBM.Watson.Widgets.Question
{
    public class PassageAnimationManager : WatsonBaseAnimationManager
    {

        /*

        //Passage One Finger Animation
        Vector3[] m_PathToCenterForFirstItem;
        Vector3[] m_PathOrientationToCenterForFirstItem;
        Vector3[] m_PathToStackForFirstItem;
        Vector3[] m_PathOrientationToStackForFirstItem;

        LTBezierPath[] m_BezierPathToCenter;
        LTBezierPath[] m_BezierPathOrientationToCenter;
        LTBezierPath[] m_BezierPathToStack;
        LTBezierPath[] m_BezierPathOrientationToStack;

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
            m_BezierPathToCenter = new LTBezierPath[numberOfPassages];
            m_BezierPathOrientationToCenter = new LTBezierPath[numberOfPassages];
            m_BezierPathToStack = new LTBezierPath[numberOfPassages];
            m_BezierPathOrientationToStack = new LTBezierPath[numberOfPassages];

            for (int i = 0; i < numberOfPassages; i++)
            {
                
            }
            m_BezierPathToCenter = new LTBezierPath();

            m_BezierPathOrientationToCenter = new LTBezierPath();

            m_BezierPathToStack = new LTBezierPath();

            m_BezierPathOrientationToStack = new LTBezierPath();

        }

        // Update is called once per frame
        void Update()
        {

        }

        //[SerializeField]
        private float sleepPassageSmooth = 4.0f;

        private void DragOneFingerOnPassageOnUpdate()
        {

            if (m_PassageItems != null && m_PassageItems[0] != null)
            {

                if (m_LastFrameOneFingerDrag < 0)
                {
                    if (m_PercentCurrentPassage > 0.25f && m_PercentCurrentPassage < 0.75f)
                    {
                        m_PercentCurrentPassage = 0.5f;
                    }
                    else if (m_PercentCurrentPassage < 0.25f)
                    {
                        m_PercentCurrentPassage = 0.0f;
                    }
                    else
                    {
                        m_PercentCurrentPassage = 1.0f;
                    }
                }

                LTBezierPath m_BezierPathCurrent;
                LTBezierPath m_BezierPathOrientationCurrent;
                float m_RatioBezierPathPassage = 0.0f;
                if (m_PercentCurrentPassage <= 0.5f)
                {
                    m_RatioBezierPathPassage = m_PercentCurrentPassage * 2.0f;
                    m_BezierPathCurrent = m_BezierPathToCenter;
                    m_BezierPathOrientationCurrent = m_BezierPathOrientationToCenter;
                }
                else
                {
                    m_RatioBezierPathPassage = (m_PercentCurrentPassage - 0.5f) * 2.0f;
                    m_BezierPathCurrent = m_BezierPathToStack;
                    m_BezierPathOrientationCurrent = m_BezierPathOrientationToStack;
                }

                for (int i = 0; i < m_PassageItems.Length; i++)
                {
                    if (i < m_SelectedPassageIndex)
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToCenter.point(0.0f), Time.deltaTime * sleepPassageSmooth);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToCenter.point(0.0f)), Time.deltaTime * sleepPassageSmooth);
                    }
                    else if (i > m_SelectedPassageIndex)
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathToStack.point(1.0f), Time.deltaTime * sleepPassageSmooth);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationToStack.point(1.0f)), Time.deltaTime * sleepPassageSmooth);
                    }
                    else
                    {
                        m_PassageItems[i].transform.localPosition = Vector3.Lerp(m_PassageItems[i].transform.localPosition, m_BezierPathCurrent.point(m_RatioBezierPathPassage), Time.deltaTime * sleepPassageSmooth);
                        m_PassageItems[i].transform.localRotation = Quaternion.Lerp(m_PassageItems[i].transform.localRotation, Quaternion.Euler(m_BezierPathOrientationCurrent.point(m_RatioBezierPathPassage)), Time.deltaTime * sleepPassageSmooth);

                    }
                }

            }
        }
        */
    }
    
}