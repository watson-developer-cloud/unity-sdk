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
    private static Queue<UnityEngine.Object> sm_DestroyQueue = new Queue<UnityEngine.Object>();
    private static int sm_DestroyQueueID = 0;

    /// <summary>
    /// Returns the state of the AudioClip destroy queue.
    /// </summary>
    /// <returns>Returns true if the destoy queue processor is active.</returns>
    public static bool IsDestroyQueueActive()
    {
      return sm_DestroyQueueID != 0;
    }

    /// <summary>
    /// Start up the AudioClip destroy queue processor.
    /// </summary>
    public static void StartDestroyQueue()
    {
      if (sm_DestroyQueueID == 0)
        sm_DestroyQueueID = Runnable.Run(ProcessDestroyQueue());
    }

    /// <summary>
    /// Stop the AudioClip destroy processor.
    /// </summary>
    public static void StopDestroyQueue()
    {
      if (sm_DestroyQueueID != 0)
      {
        Runnable.Stop(sm_DestroyQueueID);
        sm_DestroyQueueID = 0;
      }
    }

    /// <summary>
    /// Queue an AudioClip for destruction on the main thread. This function is thread-safe.
    /// </summary>
    /// <param name="clip">The AudioClip to destroy.</param>
    public static void DestroyUnityObject(UnityEngine.Object obj)
    {
      if (sm_DestroyQueueID == 0)
        throw new WatsonException("Destroy queue not started.");

      lock (sm_DestroyQueue)
        sm_DestroyQueue.Enqueue(obj);
    }

    private static IEnumerator ProcessDestroyQueue()
    {
      yield return null;

      while (sm_DestroyQueueID != 0)
      {
        yield return new WaitForSeconds(1.0f);

        lock (sm_DestroyQueue)
        {
          while (sm_DestroyQueue.Count > 0)
          {
            UnityEngine.Object obj = sm_DestroyQueue.Dequeue();
            UnityEngine.Object.DestroyImmediate(obj, true);
          }
        }
      }
    }
  }
}
