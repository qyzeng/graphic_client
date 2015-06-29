using UnityEngine;
using UnityEditor;
using System.Collections;

public class ReverseMeshNormals
{
	[MenuItem("CONTEXT/MeshFilter/Invert Normals")]
	private static void InverseMeshNormals ()
	{
		GameObject[] allSelectedObjects = Selection.gameObjects;

		foreach (GameObject selectedObject in allSelectedObjects) {
			MeshFilter[] meshFilters = selectedObject.GetComponentsInChildren<MeshFilter> ();
			foreach (MeshFilter meshFilter in meshFilters) {
				Mesh mesh = meshFilter.sharedMesh;
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

				meshFilter.sharedMesh = mesh;
			}
		}
	}
}
