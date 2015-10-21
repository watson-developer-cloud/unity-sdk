using UnityEngine;
using System.Collections;
using IBM.Watson.Utilities;

public class MainUI : MonoBehaviour {

    public IEnumerator Start()
    {
        while(! Config.Instance.ConfigLoaded )
            yield return null;
    }

    public void OnUnitTests()
    {
        Application.LoadLevel( "UnitTests" );
    }

    public void OnTestSTT()
    {
        Application.LoadLevel( "TestSTT" );
    }

    public void OnTestTTS()
    {
        Application.LoadLevel( "TestTTS" );
    }

    public void OnTestNLC()
    {
        Application.LoadLevel( "TestNLC" );
    }

    public void OnXRAY()
    {
        Application.LoadLevel( "XRay" );
    }

    public void OnCubeTest()
    {
        Application.LoadLevel( "CubeTest" );
    }

    public void OnMain()
    {
        Application.LoadLevel( "Main" );
    }

	public void OnTouchTestCamera()
	{
		Application.LoadLevel( "Camera" );
	}
	public void OnTouchTestCheckers()
	{
		Application.LoadLevel( "Checkers" );
	}
	public void OnTouchTestColors()
	{
		Application.LoadLevel( "Colors" );
	}
	public void OnTouchTestInput()
	{
		Application.LoadLevel( "Input" );
	}
	public void OnTouchTestMultiUser()
	{
		Application.LoadLevel( "Multiuser" );
	}
	public void OnTouchTestPhotos()
	{
		Application.LoadLevel( "Photos" );
	}
	public void OnTouchTestPhotos2D()
	{
		Application.LoadLevel( "Photos2D" );
	}
	public void OnTouchTestPortal()
	{
		Application.LoadLevel( "Portal" );
	}
	public void OnTouchTestTaps()
	{
		Application.LoadLevel( "Taps" );
	}
	public void OnTouchTestUI()
	{
		Application.LoadLevel( "InputModule" );
	}

	public void OnBack()
	{
		if (Application.loadedLevelName != "Main") {
			OnMain ();
		} else {
			Application.Quit();
		}
	}

	private static MainUI _instance = null;
	void Awake() {
		if (!_instance) {	//first-time opening
			_instance = this;
		} else if (_instance != this) {
			Destroy (_instance.gameObject);
			_instance = this;
			MakeActiveEventSystem(false);
			StartCoroutine(MakeActiveEventSystemWithDelay(true));

		}
		else {
			//do nothing - the other instance will destroy the current instance.
		}

		DontDestroyOnLoad(transform.gameObject);

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	IEnumerator MakeActiveEventSystemWithDelay(bool active){
		yield return new WaitForEndOfFrame ();
		MakeActiveEventSystem (active);
	}
	void MakeActiveEventSystem(bool active){
		Transform[] transformList = gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < transformList.Length; i++) {
			if(transformList[i].name == "EventSystem"){
				transformList[i].gameObject.SetActive(active);
				break;
			}
		}
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.LeftArrow) ||	Input.GetKeyDown (KeyCode.Escape)) {
			OnBack();
		} 
	}
}
