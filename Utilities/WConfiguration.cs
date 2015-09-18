using UnityEngine;
using System.Collections;

public class Watson{

	public static void WatsonStart(){
		WConfiguration wConfiguration = WConfiguration.Instance;

	}

	public static void WatsonSaveState(){
		//TODO: save all values and state into a file and retrieve if needed.
	}

	public static void WatsonReloadState(){
		//TODO: load all values and state from a file that saved
	}

	public static void WatsonStop(){
		//TODO: clearing all the values (used for when closing the application to clear memory)
	}
}

public class WConfiguration : MonoBehaviour {

	#region Variables Watson uses globally 
		public static string objectWatsonNameInScene = "~Watson";
	#endregion

	#region Singleton For WConfiguration and GameObject Watson

	private static WConfiguration _instance = null;
	/// <summary>
	/// Gets the instance of Watson Constanct Class. It holds the all information about Watson and related services. 
	/// </summary>
	/// <value>The instance class of Watson Contstants</value>
	public static WConfiguration Instance{
		get{
			if(_instance == null){
				_instance = gameObjectWatson.AddComponent<WConfiguration>();
			}
			return _instance;
		}
	}
	
	private static GameObject _gameObjectWatson = null;
	/// <summary>
	/// Gets the game object Watson. It is not exist in the scene, it will be created on the runtime. 
	/// It holds all the components related with Watson services. 
	/// </summary>
	/// <value>The game object Watson.</value>
	public static GameObject gameObjectWatson{
		get{
			if(_gameObjectWatson == null){
				_gameObjectWatson = new GameObject();
				_gameObjectWatson.name = objectWatsonNameInScene;
			}
			return _gameObjectWatson;
			
		}
	}

	#endregion
	

}
