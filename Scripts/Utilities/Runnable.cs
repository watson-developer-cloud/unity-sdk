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

// Uncomment to enable debugging of the Runnable class.
//#define ENABLE_RUNNABLE_DEBUGGING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// Helper class for running co-routines without having to inherit from MonoBehavior.
    /// </summary>
    public class Runnable : MonoBehaviour
    {
        #region Public Properties
        /// <summary>
        /// Returns the Runnable instance.
        /// </summary>
        public static Runnable Instance { get { return Singleton<Runnable>.Instance; } }
        #endregion

        #region Public Interface
        /// <summary>
        /// Start a co-routine function.
        /// </summary>
        /// <param name="routine">The IEnumerator returns by the co-routine function the user is invoking.</param>
        /// <returns>Returns a ID that can be passed into Stop() to halt the co-routine.</returns>
        public static int Run(IEnumerator routine)
        {
            Routine r = new Routine(routine);
            return r.ID;
        }

        /// <summary>
        /// Stops a active co-routine.
        /// </summary>
        /// <param name="ID">THe ID of the co-routine to stop.</param>
        public static void Stop(int ID)
        {
            Routine r = null;
            if (Instance._routines.TryGetValue(ID, out r))
                r.Stop = true;
        }

        /// <summary>
        /// Check if a routine is still running.
        /// </summary>
        /// <param name="id">The ID returned by Run().</param>
        /// <returns>Returns true if the routine is still active.</returns>
        static public bool IsRunning(int id)
        {
            return Instance._routines.ContainsKey(id);
        }

#if UNITY_EDITOR
        private static bool _editorRunnable = false;

        /// <summary>
        /// This function enables the Runnable in edit mode.
        /// </summary>
        public static void EnableRunnableInEditor()
        {
            if (!_editorRunnable)
            {
                _editorRunnable = true;
                UnityEditor.EditorApplication.update += UpdateRunnable;
            }
        }
        static void UpdateRunnable()
        {
            if (!Application.isPlaying)
                Instance.UpdateRoutines();
        }

#endif
        #endregion

        #region Private Types
        /// <summary>
        /// This class handles a running co-routine.
        /// </summary>
        private class Routine : IEnumerator
        {
            #region Public Properties
            public int ID { get; private set; }
            public bool Stop { get; set; }
            #endregion

            #region Private Data
            private bool _moveNext = false;
            private IEnumerator _enumerator = null;
            #endregion

            public Routine(IEnumerator a_enumerator)
            {
                _enumerator = a_enumerator;
                Runnable.Instance.StartCoroutine(this);
                Stop = false;
                ID = Runnable.Instance._nextRoutineId++;

                Runnable.Instance._routines[ID] = this;
#if ENABLE_RUNNABLE_DEBUGGING
                Log.Debug("Runnable.Routine()", "Coroutine {0} started.", ID ); 
#endif
            }

            #region IEnumerator Interface
            public object Current { get { return _enumerator.Current; } }
            public bool MoveNext()
            {
                _moveNext = _enumerator.MoveNext();
                if (_moveNext && Stop)
                    _moveNext = false;

                if (!_moveNext)
                {
                    Runnable.Instance._routines.Remove(ID);      // remove from the mapping
#if ENABLE_RUNNABLE_DEBUGGING
                    Log.Debug("Runnable.Routine()", "Coroutine {0} stopped.", ID );
#endif
                }

                return _moveNext;
            }
            public void Reset() { _enumerator.Reset(); }
            #endregion
        }
        #endregion

        #region Private Data
        private Dictionary<int, Routine> _routines = new Dictionary<int, Routine>();
        private int _nextRoutineId = 1;
        #endregion

        /// <summary>
        /// THis can be called by the user to force all co-routines to get a time slice, this is usually
        /// invoked from an EditorApplication.Update callback so we can use runnable in Editor mode.
        /// </summary>
        public void UpdateRoutines()
        {
            if (_routines.Count > 0)
            {
                // we are not in play mode, so we must manually update our co-routines ourselves
                List<Routine> routines = new List<Routine>();
                foreach (var kp in _routines)
                    routines.Add(kp.Value);

                foreach (var r in routines)
                    r.MoveNext();
            }
        }
    }
}
