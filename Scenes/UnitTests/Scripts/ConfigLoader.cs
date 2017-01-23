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
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;

//! This helper class makes sure the Watson configuration is fully loaded before we try to access any of the services.
public class ConfigLoader : MonoBehaviour
{
  [SerializeField]
  private GameObject m_Prefab = null;
  private GameObject m_CreatedObject = null;

  #region OnEnable / OnDisable - Registering events
  void OnEnable()
  {
    EventManager.Instance.RegisterEventReceiver("OnUserToLogout", OnUserLogOut);
  }

  void OnDisable()
  {
    EventManager.Instance.UnregisterEventReceiver("OnUserToLogout", OnUserLogOut);
  }
  #endregion

  IEnumerator Start()
  {
    // wait for the configuration to be loaded first..
    while (!Config.Instance.ConfigLoaded)
      yield return null;

    // then initiate a prefab after we are done loading the config.
    m_CreatedObject = GameObject.Instantiate(m_Prefab);
  }

  /// <summary>
  /// Handler for user logout
  /// </summary>
  /// <param name="args"></param>
  public void OnUserLogOut(System.Object[] args)
  {
    if (m_CreatedObject != null)
    {
      if (!m_CreatedObject.activeSelf)
        m_CreatedObject.SetActive(true);

      m_CreatedObject.SendMessage("DestroyCreatedObject", SendMessageOptions.DontRequireReceiver);
    }
    StartCoroutine(Start());
  }


}
