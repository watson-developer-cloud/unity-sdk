using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;

public class TestRunnable : MonoBehaviour {

    private void OnGUI()
    {
        if ( GUILayout.Button( "Start Routine" ) )
        {
            Runnable.Run( TestCoroutine("Test") );
        }
    }

    private IEnumerator TestCoroutine( string a_Argument )
    {
        yield return null;
        Debug.Log( a_Argument );
    }
}

