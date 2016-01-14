using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Utilities;

//! This helper class makes sure the Watson configuration is fully loaded before we try to access any of the services.
public class ConfigLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Prefab = null;

	// Use this for initialization
	IEnumerator Start ()
    {
        // wait for the configuration to be loaded first..
        while (!Config.Instance.ConfigLoaded)
            yield return null;
        	
        // then initiate a prefab after we are done loading the config.
        GameObject.Instantiate( m_Prefab );
	}
	
}
