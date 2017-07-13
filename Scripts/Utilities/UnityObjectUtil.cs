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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IBM.Watson.DeveloperCloud.Utilities
{
    /// <summary>
    /// AudioClip helper functions.
    /// </summary>
    public static class UnityObjectUtil
    {
        private static Queue<UnityEngine.Object> _destroyQueue = new Queue<UnityEngine.Object>();
        private static int _destroyQueueID = 0;

        /// <summary>
        /// Returns the state of the AudioClip destroy queue.
        /// </summary>
        /// <returns>Returns true if the destoy queue processor is active.</returns>
        public static bool IsDestroyQueueActive()
        {
            return _destroyQueueID != 0;
        }

        /// <summary>
        /// Start up the AudioClip destroy queue processor.
        /// </summary>
        public static void StartDestroyQueue()
        {
            if (_destroyQueueID == 0)
                _destroyQueueID = Runnable.Run(ProcessDestroyQueue());
        }

        /// <summary>
        /// Stop the AudioClip destroy processor.
        /// </summary>
        public static void StopDestroyQueue()
        {
            if (_destroyQueueID != 0)
            {
                Runnable.Stop(_destroyQueueID);
                _destroyQueueID = 0;
            }
        }

        /// <summary>
        /// Queue an AudioClip for destruction on the main thread. This function is thread-safe.
        /// </summary>
        /// <param name="clip">The AudioClip to destroy.</param>
        public static void DestroyUnityObject(UnityEngine.Object obj)
        {
            if (_destroyQueueID == 0)
                throw new WatsonException("Destroy queue not started.");

            lock (_destroyQueue)
                _destroyQueue.Enqueue(obj);
        }

        private static IEnumerator ProcessDestroyQueue()
        {
            yield return null;

            while (_destroyQueueID != 0)
            {
                yield return new WaitForSeconds(1.0f);

                lock (_destroyQueue)
                {
                    while (_destroyQueue.Count > 0)
                    {
                        Object obj = _destroyQueue.Dequeue();
                        Object.DestroyImmediate(obj, true);
                    }
                }
            }
        }
    }
}
