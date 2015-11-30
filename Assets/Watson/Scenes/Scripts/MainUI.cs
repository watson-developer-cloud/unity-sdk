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


using IBM.Watson.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.IO;
using IBM.Watson.Logging;

public class MainUI : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup m_ButtonLayout = null;
    [SerializeField]
    private Button m_ButtonPrefab = null;
    [SerializeField]
    private string [] m_SceneNames = null;

    private const string MAIN_SCENE = "Main";

#if UNITY_EDITOR
    [UnityEditor.MenuItem("CONTEXT/MainUI/Update Scene Names")]
     private static void UpdateNames(UnityEditor.MenuCommand command)
     {
        MainUI context = (MainUI)command.context;
        List<string> scenes = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if ( scene == null || !scene.enabled )
                continue;

            string name = Path.GetFileNameWithoutExtension(scene.path);
            if ( name == MAIN_SCENE )
                continue;

            scenes.Add(name);
        }
        scenes.Sort();
        context.m_SceneNames = scenes.ToArray();
    }
#endif

    private IEnumerator Start()
    {
        if ( m_ButtonLayout == null )
            throw new WatsonException( "m_ButtonLayout is null." );
        if ( m_ButtonPrefab == null )
            throw new WatsonException( "m_ButtonLayout is null." );

        // wait for the configuration to be loaded first..
        while (!Config.Instance.ConfigLoaded)
            yield return null;

        // create the buttons..
        UpdateButtons();
    }

    private void OnLevelWasLoaded(int level )
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        while( m_ButtonLayout.transform.childCount > 0 )
            DestroyImmediate( m_ButtonLayout.transform.GetChild(0).gameObject );

        //Log.Debug( "MainUI", "UpdateBottons, level = {0}", Application.loadedLevelName );
        if ( Application.loadedLevelName == MAIN_SCENE )
        {
            foreach( var scene in m_SceneNames )
            {
                if ( string.IsNullOrEmpty( scene ) )
                    continue;

                GameObject buttonObj = GameObject.Instantiate( m_ButtonPrefab.gameObject );
                buttonObj.transform.SetParent( m_ButtonLayout.transform, false );

                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if ( buttonText != null )
                    buttonText.text = scene;
                Button button = buttonObj.GetComponentInChildren<Button>();

                string captured = scene;
                button.onClick.AddListener( () => OnLoadLevel(captured) );
            }
        }
    }

    private void OnLoadLevel( string name )
    {
        Log.Debug( "MainUI", "OnLoadLevel, name = {0}", name );
        Application.LoadLevel( name );
    }

    public void OnBack()
    {
        Log.Debug( "MainUI", "OnBack invoked" );
        if (Application.loadedLevelName != MAIN_SCENE)
            Application.LoadLevel( MAIN_SCENE );
        else
            Application.Quit();
    }

    private static MainUI _instance = null;
    void Awake()
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

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    IEnumerator MakeActiveEventSystemWithDelay(bool active)
    {
        yield return new WaitForEndOfFrame();
        MakeActiveEventSystem(active);
    }

    void MakeActiveEventSystem(bool active)
    {
        Log.Debug( "MainUI", "MakeActiveEventSystem, active = {0}", active );
        object [] systems = Resources.FindObjectsOfTypeAll( typeof(EventSystem) );
        foreach( var system in systems )
            ((EventSystem)system).gameObject.SetActive( active );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Escape))
        {
            OnBack();
        }
    }
}
