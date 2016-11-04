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

using UnityEngine;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  class NestedPrefabs : MonoBehaviour
  {
    [SerializeField]
    private List<GameObject> m_Prefabs = new List<GameObject>();
    private List<GameObject> m_GameObjectCreated = new List<GameObject>();
    [SerializeField]
    private bool m_SetParent = true;

    private void Awake()
    {
      foreach (GameObject prefab in m_Prefabs)
      {
        if (prefab == null)
          continue;

        GameObject instance = Instantiate(prefab);
        if (m_SetParent)
          instance.transform.SetParent(transform, false);

        m_GameObjectCreated.Add(instance);
      }
    }

    #region Destroy objects

    /// <summary>
    /// It destroys the created object to set the initial state
    /// </summary>
    public void DestroyCreatedObject()
    {
      foreach (GameObject gameObject in m_GameObjectCreated)
      {
        if (gameObject == null)
          continue;

        gameObject.SendMessage("DestroyCreatedObject", SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
      }
      m_GameObjectCreated.Clear();
      Destroy(this.gameObject);
    }
    #endregion
  }
}
