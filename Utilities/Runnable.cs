// Uncomment to enable debugging of the Runnable class.
//#define ENABLE_RUNNABLE_DEBUGGING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.Utilities
{
    // Helper class for running co-routines without having to inherit from MonoBehavior.
    public class Runnable : MonoBehaviour
    {
        private class Routine : IEnumerator
        {
            #region Public Properties
            public int ID { get; private set; }
            public bool Stop { get; set; }
            #endregion

            #region Private Data
            private bool m_bMoveNext = false;
            private IEnumerator m_Enumerator = null;
            #endregion

            public Routine(IEnumerator a_enumerator)
            {
                m_Enumerator = a_enumerator;
                Runnable.Instance.StartCoroutine(this);
                Stop = false;
                ID = Runnable.Instance.m_NextRoutineId++;

                Runnable.Instance.m_Routines[ID] = this;
#if ENABLE_RUNNABLE_DEBUGGING
                Debug.Log( string.Format("Coroutine {0} started.", ID ) ); 
#endif
            }

#region IEnumerator Interface
            public object Current { get { return m_Enumerator.Current; } }
            public bool MoveNext()
            {
                m_bMoveNext = m_Enumerator.MoveNext();
                if (m_bMoveNext && Stop)
                    m_bMoveNext = false;

                if (!m_bMoveNext)
                {
                    Runnable.Instance.m_Routines.Remove(ID);      // remove from the mapping
#if ENABLE_RUNNABLE_DEBUGGING
                    Debug.Log( string.Format("Coroutine {0} stopped.", ID ) );
#endif
                }

                return m_bMoveNext;
            }
            public void Reset() { m_Enumerator.Reset(); }
#endregion
        }

        public static int Run(IEnumerator a_Routine)
        {
            Routine r = new Routine(a_Routine);
            return r.ID;
        }

        public static void Stop(int a_ID)
        {
            Routine r = null;
            if (Instance.m_Routines.TryGetValue(a_ID, out r))
                r.Stop = true;
        }

#region Private Data
        private static Runnable sm_Instance = null;
        private Dictionary<int, Routine> m_Routines = new Dictionary<int, Routine>();
        private int m_NextRoutineId = 0;
#endregion

        public static Runnable Instance
        {
            get
            {
                if (sm_Instance == null)
                {
                    GameObject RunnableObject = new GameObject("_Runnable");
                    RunnableObject.hideFlags = HideFlags.HideAndDontSave;

                    sm_Instance = RunnableObject.AddComponent<Runnable>();
                }

                return sm_Instance;
            }
        }
    }
}
