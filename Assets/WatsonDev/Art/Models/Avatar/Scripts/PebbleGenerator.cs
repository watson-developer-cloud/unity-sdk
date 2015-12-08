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
using IBM.Watson.Widgets.Avatar;

[System.Serializable]
public class PebbleGeneratorConfig{
	private float m_angleSlice = 1.0f;
	private float m_distanceFromCenter = 10.0f;
	private int m_numberOfRow = 1;
	private float m_distanceBetweenRows = 1.0f;
	private Vector3 m_sphereSize = Vector3.one;

	public string qualityName;
	public float angleSlice = 1.0f;
	public float distanceFromCenter = 10.0f;
	public int numberOfRow = 1;
	public float distanceBetweenRows = 1.0f;
	public Vector3 sphereSize = Vector3.one;

	public bool IsThereAnyChange
	{
		get
		{
			return (m_angleSlice != angleSlice)
				|| (m_distanceFromCenter != distanceFromCenter)
				|| (m_numberOfRow != numberOfRow)
				|| (m_distanceBetweenRows != distanceBetweenRows)
				|| (m_sphereSize != sphereSize);
		}
		
	}
	
	public void SetValuesAsDefault()
	{
		m_angleSlice = angleSlice;
		m_distanceFromCenter = distanceFromCenter;
		m_numberOfRow = numberOfRow;
		m_distanceBetweenRows = distanceBetweenRows;
		m_sphereSize = sphereSize;
	}

	public void ResetDefaultValues(){
		m_angleSlice = 0;
		m_distanceFromCenter = 0;
		m_numberOfRow = 0;
		m_distanceBetweenRows = 0;
		m_sphereSize = Vector3.zero;
	}
}

/// <summary>
/// Editor class for generating pebbles for the avatar.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(PebbleManager))]
public class PebbleGenerator : MonoBehaviour
{
	[SerializeField]
	private PebbleGeneratorConfig[] m_PebbleConfig;
	private PebbleGeneratorConfig m_CurrentConfig = null;

	public PebbleGeneratorConfig CurrentConfig{
		get{
			return m_CurrentConfig;
		}
		set{
			m_CurrentConfig = value;
			if(m_CurrentConfig != null){
				m_CurrentConfig.ResetDefaultValues();
			}
		}
	}

    public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    public bool receiveShadows = false;
    public PrimitiveType primitiveType = PrimitiveType.Sphere;
	public Mesh meshToUseAsPebble;
    public Material material;

    public bool isUsingSharedMesh = false;

    private GameObject[] rowObjects;
    private GameObject[] spehereList;

    private PebbleManager m_pebbleManager;
    // Use this for initialization
    void Start()
    {
        m_pebbleManager = transform.GetComponent<PebbleManager>();
        ClearAllSpheres();

		if (m_PebbleConfig == null || m_PebbleConfig.Length == 0) {
			m_PebbleConfig = new PebbleGeneratorConfig[QualitySettings.names.Length];
			for (int i = 0; i < m_PebbleConfig.Length ; i++) {
				m_PebbleConfig[i] = new PebbleGeneratorConfig();
				m_PebbleConfig[i].qualityName = QualitySettings.names [i];

				if(m_PebbleConfig [i].qualityName == QualitySettings.names [QualitySettings.GetQualityLevel()]){
					CurrentConfig = m_PebbleConfig [i];
				}
			}
		} else {
			for (int i = 0; i < m_PebbleConfig.Length; i++) {
				if(m_PebbleConfig [i].qualityName == QualitySettings.names [QualitySettings.GetQualityLevel()]){
					CurrentConfig = m_PebbleConfig [i];
					break;
				}
			}
		}

		if (CurrentConfig == null) {
			if(m_PebbleConfig != null && m_PebbleConfig.Length > 0)
				CurrentConfig = m_PebbleConfig[0];
			else 
				CurrentConfig = new PebbleGeneratorConfig();
		}

		ResetPebblesIfNeeded ();
    }

	#if UNITY_EDITOR

    void Update()
    {
		ResetPebblesIfNeeded ();
		if (CurrentConfig != null && CurrentConfig.qualityName != QualitySettings.names [QualitySettings.GetQualityLevel ()]) {
			Start();
		}
    }
	#endif

	void ResetPebblesIfNeeded(){
		if (CurrentConfig != null && CurrentConfig.IsThereAnyChange) {
			ResetPebbles();
		}
	}

	void ResetPebbles(){
		CurrentConfig.SetValuesAsDefault ();
		ClearAllSpheres ();
		ClearList ();
		CreateSpheres ();
		if (isUsingSharedMesh) {
			MakeAllIndividualRowsAsSharedMesh ();
		}
	}
   

    void ClearAllSpheres()
    {
        Transform[] childerenTransform = transform.GetComponentsInChildren<Transform>();
        for (int i = childerenTransform.Length - 1; i >= 0; i--)
        {
            if (childerenTransform[i] != null && childerenTransform[i] != this.transform)
            {
                GameObject.DestroyImmediate(childerenTransform[i].gameObject);
            }
        }
    }

    void ClearList()
    {
        if (spehereList != null)
        {
            for (int i = 0; i < spehereList.Length; i++)
            {
                if (spehereList[i] != null)
                    GameObject.DestroyImmediate(spehereList[i]);
            }
            spehereList = null;
        }
    }

    void CreateSpheres()
    {

		if (CurrentConfig.angleSlice <= 0)
        {
            Debug.LogError("Error on angle Slice value");
			CurrentConfig.angleSlice = 1.0f;
        }

		int numberOfSpheresInRow = (int)(360.0f / CurrentConfig.angleSlice);
		int numberOfTotalSpheres = numberOfSpheresInRow * CurrentConfig.numberOfRow;
        spehereList = new GameObject[numberOfTotalSpheres];

		rowObjects = new GameObject[CurrentConfig.numberOfRow];

        if (m_pebbleManager == null)
            m_pebbleManager = transform.GetComponent<PebbleManager>();

		m_pebbleManager.PebbleRowList = new PebbleRow[CurrentConfig.numberOfRow];

		for (int indexOfRow = 0; indexOfRow < CurrentConfig.numberOfRow; indexOfRow++)
        {
            rowObjects[indexOfRow] = new GameObject();
            rowObjects[indexOfRow].name = indexOfRow.ToString();
            rowObjects[indexOfRow].transform.parent = this.transform;
            rowObjects[indexOfRow].transform.localRotation = Quaternion.identity;
            rowObjects[indexOfRow].transform.localPosition = Vector3.zero;
            rowObjects[indexOfRow].transform.localScale = Vector3.one;

            m_pebbleManager.PebbleRowList[indexOfRow] = new PebbleRow();
            m_pebbleManager.PebbleRowList[indexOfRow].pebbleList = new GameObject[numberOfSpheresInRow];

            for (int indexOfSpheresInRow = 0; indexOfSpheresInRow < numberOfSpheresInRow; indexOfSpheresInRow++)
            {
				float angle = CurrentConfig.angleSlice * indexOfSpheresInRow * Mathf.Deg2Rad;
                int indexOfSphere = indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow;

                if (indexOfSphere == 0)
                {
                    //first object creation
                    spehereList[indexOfSphere] = GameObject.CreatePrimitive(primitiveType);
                    MeshRenderer meshRenderer = spehereList[indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow].GetComponent<MeshRenderer>();
                    meshRenderer.shadowCastingMode = shadowCastingMode;
                    meshRenderer.receiveShadows = receiveShadows;
					meshRenderer.useLightProbes = false;
                    //meshRenderer.material = material;
                    meshRenderer.sharedMaterial = material;

					if(meshToUseAsPebble != null){
						MeshFilter meshFilter = spehereList[indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow].GetComponent<MeshFilter>();
						meshFilter.sharedMesh = meshToUseAsPebble;
					}

                    SphereCollider sphereCollider = spehereList[indexOfSpheresInRow + indexOfRow * numberOfSpheresInRow].GetComponent<SphereCollider>();
                    if (sphereCollider != null)
                        DestroyImmediate(sphereCollider);

                }
                else
                {
                    spehereList[indexOfSphere] = GameObject.Instantiate(spehereList[0]);
                }

                Transform parentTransform = rowObjects[indexOfRow].transform;
                spehereList[indexOfSphere].transform.parent = parentTransform;
				spehereList[indexOfSphere].transform.position = parentTransform.position + new Vector3(Mathf.Sin(angle) * (CurrentConfig.distanceFromCenter + CurrentConfig.distanceBetweenRows * indexOfRow), 0.0f, Mathf.Cos(angle) * (CurrentConfig.distanceFromCenter + CurrentConfig.distanceBetweenRows * indexOfRow));
				spehereList[indexOfSphere].transform.localScale = Vector3.Scale(CurrentConfig.sphereSize, new Vector3(1.0f / parentTransform.lossyScale.x, 1.0f / parentTransform.lossyScale.y, 1.0f / parentTransform.lossyScale.z));
                spehereList[indexOfSphere].name = indexOfSpheresInRow.ToString();

                m_pebbleManager.PebbleRowList[indexOfRow].pebbleList[indexOfSpheresInRow] = spehereList[indexOfSphere];

            }
        }
    }

    void MakeAllIndividualRowsAsSharedMesh()
    {
        //For each row we combine all meshes separately
		for (int indexOfRow = 0; indexOfRow < CurrentConfig.numberOfRow; indexOfRow++)
        {

            MeshFilter[] meshFilters = rowObjects[indexOfRow].transform.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                combine[i].subMeshIndex = i;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }

            MeshFilter meshFilterOnParent = rowObjects[indexOfRow].transform.GetComponent<MeshFilter>();
            if (meshFilterOnParent == null)
            {
                meshFilterOnParent = rowObjects[indexOfRow].gameObject.AddComponent<MeshFilter>();
            }

            rowObjects[indexOfRow].transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            rowObjects[indexOfRow].transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
            rowObjects[indexOfRow].transform.gameObject.SetActive(true);
        }
    }
}


