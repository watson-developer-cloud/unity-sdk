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

[RequireComponent(typeof(MeshFilter))]
public class ReverseNormals : MonoBehaviour
{
  void Start()
  {
    MeshFilter meshFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
    if (meshFilter != null)
    {
      Mesh mesh = meshFilter.mesh;

      Vector3[] normals = mesh.normals;
      for (int i = 0; i < normals.Length; i++)
        normals[i] = -normals[i];
      mesh.normals = normals;

      for (int m = 0; m < mesh.subMeshCount; m++)
      {
        int[] triangles = mesh.GetTriangles(m);
        for (int i = 0; i < triangles.Length; i += 3)
        {
          int temp = triangles[i + 0];
          triangles[i + 0] = triangles[i + 1];
          triangles[i + 1] = temp;
        }
        mesh.SetTriangles(triangles, m);
      }
    }
  }
}
