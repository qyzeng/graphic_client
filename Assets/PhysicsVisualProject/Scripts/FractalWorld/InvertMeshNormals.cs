using UnityEngine;
using System.Collections;

public class InvertMeshNormals : MonoBehaviour
{
	void Awake ()
	{
		InvertRenderNormals ();
		RefreshCollider ();
	}

	void InvertRenderNormals ()
	{
		MeshFilter[] meshFilters = GetComponents<MeshFilter> ();
		foreach (MeshFilter meshFilter in meshFilters) {
			Mesh mesh = meshFilter.mesh;
			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; ++i) {
				normals [i] = -normals [i];
			}
			mesh.normals = normals;
			
			for (int i=0; i<mesh.subMeshCount; ++i) {
				int[] triangles = mesh.GetTriangles (i);
				for (int j=0; j<triangles.Length; j+=3) {
					int temp = triangles [j];
					triangles [j] = triangles [j + 1];
					triangles [j + 1] = temp;
				}
				mesh.SetTriangles (triangles, i);
			}
			
			meshFilter.mesh = mesh;
		}
	}

	void RefreshCollider ()
	{
		MeshCollider collide = GetComponent<MeshCollider> ();
		if (collide) {
			Destroy (collide);
			collide = gameObject.AddComponent<MeshCollider> ();
		}
	}
}
