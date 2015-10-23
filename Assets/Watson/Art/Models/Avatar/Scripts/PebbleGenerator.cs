using UnityEngine;
using System.Collections;
using IBM.Watson.Avatar;

#if UNITY_EDITOR
[ExecuteInEditMode]
[RequireComponent (typeof (PebbleManager))]
public class PebbleGenerator : MonoBehaviour {

	private float m_angleSlice = 1.0f;
	private float m_distanceFromCenter = 10.0f;
	private int m_numberOfRow = 1;
	private float m_distanceBetweenRows = 1.0f;
	private Vector3 m_sphereSize = Vector3.one;

	public float angleSlice = 1.0f;
	public float distanceFromCenter = 10.0f;
	public int numberOfRow = 1;
	public float distanceBetweenRows = 1.0f;
	public Vector3 sphereSize = Vector3.one;

	public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
	public bool receiveShadows = false;
	public PrimitiveType primitiveType = PrimitiveType.Sphere;
	public Material material;

	public bool isUsingSharedMesh = false;

	private GameObject[] rowObjects;
	private GameObject[] spehereList;

	private PebbleManager m_pebbleManager;
	// Use this for initialization
	void Start () {
		m_pebbleManager = transform.GetComponent<PebbleManager> ();
		ClearAllSpheres ();
	}
	
	// Update is called once per frame
	void Update () {
		if (IsThereAnyChange) {
			SetValuesAsDefault ();
			ClearAllSpheres ();
			ClearList ();
			CreateSpheres();
			if(isUsingSharedMesh){
				MakeAllIndividualRowsAsSharedMesh();
			}
		}
	}

	bool IsThereAnyChange{
		get{
			return (m_angleSlice != angleSlice) 
				|| (m_distanceFromCenter != distanceFromCenter)
				|| (m_numberOfRow != numberOfRow) 
				|| (m_distanceBetweenRows != distanceBetweenRows) 
				|| (m_sphereSize != sphereSize);
		}

	}

	void SetValuesAsDefault(){
		m_angleSlice = angleSlice;
		m_distanceFromCenter = distanceFromCenter;
		m_numberOfRow = numberOfRow;
		m_distanceBetweenRows = distanceBetweenRows;
		m_sphereSize = sphereSize;
	}

	void ClearAllSpheres(){
		Transform[] childerenTransform = transform.GetComponentsInChildren<Transform> ();
		for (int i = childerenTransform.Length - 1; i >= 0; i--) {
			if(childerenTransform[i] != null && childerenTransform[i] != this.transform){
				GameObject.DestroyImmediate(childerenTransform[i].gameObject);
			}
		}
	}

	void ClearList(){
		if (spehereList != null) {
			for (int i = 0; i < spehereList.Length; i++) {
				if(spehereList[i] != null)
					GameObject.DestroyImmediate(spehereList[i]);
			}
			spehereList = null;
		}
	}

	void CreateSpheres(){

		if (angleSlice <= 0) {
			Debug.LogError("Error on angle Slice value");
			angleSlice = 1.0f;
		}

		int numberOfSpheresInRow = (int) ( 360.0f / angleSlice) ;
		int numberOfTotalSpheres = numberOfSpheresInRow * numberOfRow;
		spehereList = new GameObject[numberOfTotalSpheres];

		rowObjects = new GameObject[numberOfRow];

		if(m_pebbleManager == null)
			m_pebbleManager = transform.GetComponent<PebbleManager> ();

		m_pebbleManager.pebbleRowList = new PebbleRow[numberOfRow];

		for (int indexOfRow = 0; indexOfRow < numberOfRow; indexOfRow++) {
			rowObjects[indexOfRow] = new GameObject();
			rowObjects[indexOfRow].name = indexOfRow.ToString();
			rowObjects[indexOfRow].transform.parent = this.transform;
			rowObjects[indexOfRow].transform.localRotation = Quaternion.identity;
			rowObjects[indexOfRow].transform.localPosition = Vector3.zero;
			rowObjects[indexOfRow].transform.localScale = Vector3.one;

			m_pebbleManager.pebbleRowList[indexOfRow] = new PebbleRow();
			m_pebbleManager.pebbleRowList[indexOfRow].pebbleList = new GameObject[numberOfSpheresInRow]; 

			for (int indexOfSpheresInRow = 0; indexOfSpheresInRow < numberOfSpheresInRow; indexOfSpheresInRow++) {
				float angle = angleSlice * indexOfSpheresInRow * Mathf.Deg2Rad;
				int indexOfSphere = indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow;

				if(indexOfSphere == 0){
					//first object creation
					spehereList[indexOfSphere] = GameObject.CreatePrimitive(primitiveType);
					MeshRenderer meshRenderer = spehereList[indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow].GetComponent<MeshRenderer>();
					meshRenderer.shadowCastingMode = shadowCastingMode;
					meshRenderer.receiveShadows = receiveShadows;
					//meshRenderer.material = material;
					meshRenderer.sharedMaterial = material;

					SphereCollider sphereCollider = spehereList[indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow].GetComponent<SphereCollider>();
					if(sphereCollider != null)
						DestroyImmediate(sphereCollider);

				}
				else{
					spehereList[indexOfSphere] = GameObject.Instantiate(spehereList[0]); 
				}

				Transform parentTransform = rowObjects[indexOfRow].transform;
				spehereList[indexOfSphere].transform.parent = parentTransform;
				spehereList[indexOfSphere].transform.position = parentTransform.position + new Vector3( Mathf.Sin(angle) * (distanceFromCenter + distanceBetweenRows * indexOfRow), 0.0f, Mathf.Cos(angle) * (distanceFromCenter + distanceBetweenRows * indexOfRow));
				spehereList[indexOfSphere].transform.localScale = Vector3.Scale( sphereSize, new Vector3(1.0f / parentTransform.lossyScale.x, 1.0f / parentTransform.lossyScale.y, 1.0f / parentTransform.lossyScale.z) );
				spehereList[indexOfSphere].name = indexOfSpheresInRow.ToString();

				m_pebbleManager.pebbleRowList[indexOfRow].pebbleList[indexOfSpheresInRow] = spehereList[indexOfSphere];
				
			}
		}
	}

	void MakeAllIndividualRowsAsSharedMesh(){
		//For each row we combine all meshes separetly
		for (int indexOfRow = 0; indexOfRow < numberOfRow; indexOfRow++) {

			MeshFilter[] meshFilters = rowObjects[indexOfRow].transform.GetComponentsInChildren<MeshFilter>();
			CombineInstance[] combine = new CombineInstance[meshFilters.Length];

			int i = 0;
			while (i < meshFilters.Length) {
				combine[i].mesh = meshFilters[i].sharedMesh;
				combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
				combine[i].subMeshIndex = i;
				meshFilters[i].gameObject.SetActive( false );
				i++;
			}

			MeshFilter meshFilterOnParent = rowObjects[indexOfRow].transform.GetComponent<MeshFilter>();
			if(meshFilterOnParent == null){
				meshFilterOnParent = rowObjects[indexOfRow].gameObject.AddComponent<MeshFilter>();
			}

			rowObjects[indexOfRow].transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			rowObjects[indexOfRow].transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
			rowObjects[indexOfRow].transform.gameObject.SetActive( true );
		}
	}
}

#endif
