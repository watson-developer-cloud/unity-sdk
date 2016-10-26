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

using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Main UI menu item.
/// </summary>
[System.Serializable]
public class MenuScene
{
  /// <summary>
  /// The name of the scene.
  /// </summary>
  public string m_SceneName;
  /// <summary>
  /// the description of the scene.
  /// </summary>
  public string m_SceneDesc;
  /// <summary>
  /// Back button position for this scene.
  /// </summary>
  public Vector3 m_CustomBackButtonPosition = Vector3.zero;
  /// <summary>
  /// Back button scale for this scene.
  /// </summary>
  public Vector2 m_CustomBackButtonScale = Vector2.zero;
  /// <summary>
  /// Is the back button visible.
  /// </summary>
  public bool m_IsVisibleBackButton = true;
}

/// <summary>
/// Script for the main UI.
/// </summary>
public class MainUI : MonoBehaviour
{
  [SerializeField]
  private LayoutGroup m_ButtonLayout = null;
  [SerializeField]
  private Button m_ButtonPrefab = null;
  [SerializeField]
  private GameObject m_BackgroundUI = null;
  [SerializeField]
  private RectTransform m_ButtonBack = null;
  private Vector3 m_InitialBackButtonPosition;
  private Vector3 m_InitialBackButtonScale;
  private Color m_InitialBackButtonColor;

  [SerializeField]
  private MenuScene[] m_Scenes = null;

  private const string MAIN_SCENE = "Main";

#if UNITY_EDITOR
  [UnityEditor.MenuItem("CONTEXT/MainUI/Update Scene Names")]
  private static void UpdateNames(UnityEditor.MenuCommand command)
  {
    MainUI context = (MainUI)command.context;
    List<string> scenes = new List<string>();
    foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
    {
      if (scene == null || !scene.enabled)
        continue;

      string name = Path.GetFileNameWithoutExtension(scene.path);
      if (name == MAIN_SCENE)
        continue;

      scenes.Add(name);
    }
    scenes.Sort();
    context.m_Scenes = new MenuScene[scenes.Count];
    for (int i = 0; i < scenes.Count; i++)
    {
      context.m_Scenes[i] = new MenuScene();
      context.m_Scenes[i].m_SceneName = scenes[i];
      context.m_Scenes[i].m_SceneDesc = scenes[i];
    }
  }
#endif

  private IEnumerator Start()
  {
    if (m_BackgroundUI == null)
      throw new WatsonException("m_BackgroundUI is null.");
    if (m_ButtonLayout == null)
      throw new WatsonException("m_ButtonLayout is null.");
    if (m_ButtonPrefab == null)
      throw new WatsonException("m_ButtonPrefab is null.");
    if (m_ButtonBack == null)
      throw new WatsonException("m_ButtonBack is null.");
    else
    {
      if (m_ButtonBack.GetComponent<RectTransform>() != null)
      {
        m_InitialBackButtonPosition = m_ButtonBack.GetComponent<RectTransform>().anchoredPosition3D;
        m_InitialBackButtonScale = m_ButtonBack.GetComponent<RectTransform>().sizeDelta;
      }
      else
      {
        throw new WatsonException("m_ButtonBack doesn't have RectTransform");
      }

      if (m_ButtonBack.GetComponent<Image>() != null)
      {
        m_InitialBackButtonColor = m_ButtonBack.GetComponentInChildren<Image>().color;
      }
      else
      {
        throw new WatsonException("m_ButtonBack doesn't have Image");
      }

    }
    // wait for the configuration to be loaded first..
    while (!Config.Instance.ConfigLoaded)
      yield return null;

    // create the buttons..
    UpdateButtons();
  }

  private void OnLevelWasLoaded(int level)
  {
    UpdateButtons();
  }

  private void UpdateButtons()
  {
    while (m_ButtonLayout.transform.childCount > 0)
      DestroyImmediate(m_ButtonLayout.transform.GetChild(0).gameObject);

    //Log.Debug( "MainUI", "UpdateBottons, level = {0}", Application.loadedLevelName );
    if (SceneManager.GetActiveScene().name == MAIN_SCENE)
    {
      m_BackgroundUI.SetActive(true);

      foreach (var scene in m_Scenes)
      {
        if (string.IsNullOrEmpty(scene.m_SceneName))
          continue;

        GameObject buttonObj = GameObject.Instantiate(m_ButtonPrefab.gameObject);
        buttonObj.transform.SetParent(m_ButtonLayout.transform, false);

        Text buttonText = buttonObj.GetComponentInChildren<Text>();
        if (buttonText != null)
          buttonText.text = scene.m_SceneDesc;
        Button button = buttonObj.GetComponentInChildren<Button>();

        string captured = scene.m_SceneName;
        button.onClick.AddListener(() => OnLoadLevel(captured));
      }
    }
    else
    {
      m_BackgroundUI.SetActive(false);
    }
  }

  private void OnLoadLevel(string name)
  {
    GameObject touchScript = GameObject.Find("TouchScript");
    if (touchScript != null)
    {
      DestroyImmediate(touchScript);
    }

    Log.Debug("MainUI", "OnLoadLevel, name = {0}", name);
    StartCoroutine(LoadLevelAsync(name));
  }

  private IEnumerator LoadLevelAsync(string name)
  {

    AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
    if (asyncOperation == null)
      yield break;

    while (!asyncOperation.isDone)
      yield return new WaitForSeconds(0.1f);

    for (int i = 0; m_Scenes != null && i < m_Scenes.Length; i++)
    {
      if (m_Scenes[i].m_SceneName == name)
      {
        if (m_Scenes[i].m_CustomBackButtonPosition != Vector3.zero)
        {
          m_ButtonBack.anchoredPosition3D = m_Scenes[i].m_CustomBackButtonPosition;
          ChangeVisibilityOfButton(m_ButtonBack, m_Scenes[i].m_IsVisibleBackButton);
        }
        else
        {
          m_ButtonBack.anchoredPosition3D = m_InitialBackButtonPosition;
          ChangeVisibilityOfButton(m_ButtonBack, true);
        }

        if (m_Scenes[i].m_CustomBackButtonScale != Vector2.zero)
        {
          m_ButtonBack.sizeDelta = m_Scenes[i].m_CustomBackButtonScale;
        }
        else
        {
          m_ButtonBack.sizeDelta = m_InitialBackButtonScale;
        }

        break;
      }
    }
  }

  private void ChangeVisibilityOfButton(RectTransform buttonBack, bool isVisible)
  {
    if (buttonBack.GetComponentInChildren<Text>() != null)
      buttonBack.GetComponentInChildren<Text>().enabled = isVisible;
    if (buttonBack.GetComponentInChildren<Image>() != null)
      buttonBack.GetComponentInChildren<Image>().color = isVisible ? m_InitialBackButtonColor :
          new Color(m_InitialBackButtonColor.r, m_InitialBackButtonColor.g, m_InitialBackButtonColor.b, 0.0f);
  }

  /// <summary>
  /// Back button handler for the MainUI.
  /// </summary>
  public void OnBack()
  {
    Log.Debug("MainUI", "OnBack invoked");
    if (SceneManager.GetActiveScene().name != MAIN_SCENE)
    {
      OnLoadLevel(MAIN_SCENE);
    }
    else
      Application.Quit();
  }

  private static MainUI _instance = null;
  private void Awake()
  {
    if (!_instance)
    {
      //first-time opening
      _instance = this;
    }
    else if (_instance != this)
    {
      Destroy(_instance.gameObject);
      _instance = this;

      MakeActiveEventSystem(false);
      StartCoroutine(MakeActiveEventSystemWithDelay(true));
    }
    else
    {
      //do nothing - the other instance will destroy the current instance.
    }

    DontDestroyOnLoad(transform.gameObject);
  }

  private IEnumerator MakeActiveEventSystemWithDelay(bool active)
  {
    yield return new WaitForEndOfFrame();
    MakeActiveEventSystem(active);
  }

  private void MakeActiveEventSystem(bool active)
  {
    Log.Debug("MainUI", "MakeActiveEventSystem, active = {0}", active);
    object[] systems = Resources.FindObjectsOfTypeAll(typeof(EventSystem));
    foreach (var system in systems)
      ((EventSystem)system).gameObject.SetActive(active);
  }

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Escape))
    {
      OnBack();
    }
  }
}
