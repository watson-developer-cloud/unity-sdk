using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LeanTweenPathControl : MonoBehaviour {

	private Vector3 lastPos;
	private Vector3[] lastPosControl;
	public LeanTweenPathControl[] controlRef;
	public int i;

	public bool isControl;
	public bool isBezier;

	private static Material matPoints;
	private static Material matControls;

	public void createMaterials() {
	    if( !matPoints ) {
	        matPoints = new Material(Shader.Find("GUI/Text Shader") );
	        matPoints.color = Color.cyan;
	        matPoints.hideFlags = HideFlags.HideAndDontSave;
	        matControls = new Material(Shader.Find("GUI/Text Shader") );
	        matControls.color = Color.white;
	        matControls.hideFlags = HideFlags.HideAndDontSave;
	    }
	}

	void OnEnable(){
		
	}

	void OnApplicationFocus(){
		OnDisable();
	}

	void OnBecameInvisible(){
		OnDisable();
	}

	void OnDisable(){
		//DestroyImmediate(matPoints);
		//DestroyImmediate(matControls);
		matPoints = null;
		matControls = null;
	}

	public void init( LeanTweenPathControl[] controlRef, int i, bool isControl ){
		this.isControl = isControl;
		this.i = i;
		createMaterials();
		gameObject.GetComponent<Renderer>().material = isControl ? matPoints : matControls;

		lastPos = transform.position;
		this.controlRef = controlRef;
		lastPosControl = new Vector3[ 2 ];
		for(int j=0;j<controlRef.Length;j++){
			if(controlRef[j])
				lastPosControl[j] = controlRef[j].transform.position;
		}
		isBezier = true;
	}

	public void init( int i ){
		this.i = i;
		createMaterials();
		gameObject.GetComponent<Renderer>().material = matPoints;
		isBezier = false;
	}

	void OnDrawGizmos(){
		if(controlRef != null && controlRef.Length>0 && controlRef[0]){
			if(lastPosControl!=null){
				if(lastPos!=transform.position && lastPosControl[0] == controlRef[0].transform.position){
					Vector3 diff = transform.position - lastPos;
					lastPos = transform.position;
					// Debug.Log("diff:"+diff);
					controlRef[0].transform.position += diff;
					lastPosControl[0] = controlRef[0].transform.position;
					if(controlRef[1]){
						controlRef[1].transform.position += diff;
						lastPosControl[1] = controlRef[1].transform.position;
					}
				}
			}

			controlRef[0].transform.LookAt( transform.position );
			if(controlRef[1])
				controlRef[1].transform.LookAt( transform.position );

			//LeanTweenPath path = gameObject.transform.parent.gameObject.GetComponent<LeanTweenPath>();
			//Debug.Log("path:"+path+" parent:"+gameObject.transform.parent);
			//path.OnDrawGizmos();
		}
	}

}
