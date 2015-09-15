using UnityEngine;
using System.Collections;

public class WConstant : MonoBehaviour {

	#region Variables Watson uses globally 
		public static string objectWatsonNameInScene = "~Watson";
	#endregion

	#region Singleton For WConstant and GameObject Watson

	private static WConstant _instance = null;
	/// <summary>
	/// Gets the instance of Watson Constanct Class. It holds the all information about Watson and related services. 
	/// </summary>
	/// <value>The instance class of Watson Contstants</value>
	public static WConstant instance{
		get{
			if(_instance == null){
				_instance = gameObjectWatson.AddComponent<WConstant>();
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
