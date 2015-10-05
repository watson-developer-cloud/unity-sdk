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
*/

using UnityEngine;
using UnityEditor;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.UnitTests;
using System.Collections.Generic;
using System.Collections;
using System;

namespace IBM.Watson.Editor
{
    [ExecuteInEditMode]
    public class UnitTestManager : MonoBehaviour
    {
        /// <summary>
        /// Returns the instance of the UnitTestManager.
        /// </summary>
        public static UnitTestManager Instance { get { return Singleton<UnitTestManager>.Instance; } }

        /// <summary>
        /// Number of tests that have failed.
        /// </summary>
        public int TestsFailed { get; private set; }
        /// <summary>
        /// Number of tests that have completed.
        /// </summary>
        public int TestsComplete { get; private set; }
        /// <summary>
        /// If true, then the editor will exit with an error code once the last test is completed.
        /// </summary>
        public bool QuitOnTestsComplete { get; set; }

        /// <summary>
        /// Public functions invoked from the command line to run all UnitTest objects.
        /// (e.g. "C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -executemethod UnitTestManager.RunAll)
        /// </summary>
        static public void RunAll()
        {
            UnitTestManager instance = Instance;
            Type[] tests = Utility.FindAllDerivedTypes(typeof(UnitTest));
            foreach (var t in tests)
                instance.QueueTest(t);

            instance.QuitOnTestsComplete = true;
            instance.RunTests();
        }

        /// <summary>
        /// Queue test by Type to run.
        /// </summary>
        /// <param name="test">The type of the UnitTest to run.</param>
        /// <param name="run">If true, then the test co-routine will be started after queueing.</param>
        public void QueueTest(Type test, bool run = false)
        {
            m_QueuedTests.Enqueue(test);
            if (run)
                RunTests();
        }

        /// <summary>
        /// Start running all queued tests.
        /// </summary>
        public void RunTests()
        {
            TestsFailed = TestsComplete = 0;
            StartCoroutine(RunTestsCR());
        }

        #region Private Data
        private Queue<Type> m_QueuedTests = new Queue<Type>();
        private Type[] m_TestsAvailable = null;
        private UnitTest m_ActiveTest = null;
        #endregion

        #region Private Functions
        private void Start()
        {
            Logger.InstallDefaultReactors();
        }

        private IEnumerator RunTestsCR()
        {
            while (m_QueuedTests.Count > 0)
            {
                Type testType = m_QueuedTests.Dequeue();

                m_ActiveTest = Activator.CreateInstance(testType) as UnitTest;
                if (m_ActiveTest != null)
                {
                    Log.Status("UnitTestManager", "STARTING UnitTest {0} ...", testType.Name);

                    // wait for the test to complete..
                    IEnumerator e = m_ActiveTest.RunTest();
                    while ( e.MoveNext() )
                        yield return null;

                    if (m_ActiveTest.TestFailed)
                    {
                        Log.Error("UnitTestManager", "... UnitTest {0} FAILED.", testType.Name);
                        TestsFailed += 1;
                    }
                    else
                    {
                        Log.Status("UnitTestManager", "... UnitTest {0} COMPLETED.", testType.Name);
                        TestsComplete += 1;
                    }
                }
                else
                {
                    Log.Error("UnitTestManager", "Failed to instantiate test {0}.", testType.Name);
                    TestsFailed += 1;
                }

            }

            if (QuitOnTestsComplete)
            {
                Log.Error("UnitTestManager", "Exiting, Tests Completed: {0}, Tests Failed: {1}", TestsComplete, TestsFailed);
                EditorApplication.Exit(TestsFailed > 0 ? 1 : 0);
            }
        }

        private void OnGUI()
        {
            if (m_TestsAvailable == null)
                m_TestsAvailable = Utility.FindAllDerivedTypes(typeof(UnitTest));

            if (m_TestsAvailable != null)
            {
                foreach (var t in m_TestsAvailable)
                {
                    string sButtonLabel = "Run " + t.Name;
                    if (GUILayout.Button(sButtonLabel))
                    {
                        QueueTest(t, true);
                    }
                }
            }
        }
        #endregion
    }
}
