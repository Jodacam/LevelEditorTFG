using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class uteCombineChildren : MonoBehaviour {
	
	public bool generateTriangleStrips = true;
	
	public void Batch (bool AddMeshColliders=false, bool RemoveLeftOvers=false, bool isItPatternExport=false)
	{
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		List<Hashtable> materialToMesh = new List<Hashtable>();
		int vertexCalc = 0;
		int hasIterations = 0;
		materialToMesh.Add(new Hashtable());

		for (int i=0;i<filters.Length;i++)
		{
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].GetComponent<Renderer>();
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			vertexCalc+=instance.mesh.vertexCount;
			
			if(curRenderer != null && curRenderer.enabled && instance.mesh != null)
			{
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;
				for(int m=0;m<materials.Length;m++)
				{
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[hasIterations][materials[m]];
					if(objects != null)
					{
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh[hasIterations].Add(materials[m], objects);
					}

					if(vertexCalc>64000)
					{
						vertexCalc = 0;
						hasIterations++;
						materialToMesh.Add(new Hashtable());
					}
				}
				
				if(!RemoveLeftOvers)
				{
					curRenderer.enabled = false;
				}
			}
		}

		int counter = 0;

		for(int i=0;i<hasIterations+1;i++)
		{
			foreach (DictionaryEntry de  in materialToMesh[i])
			{
				ArrayList elements = (ArrayList)de.Value;
				MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

				GameObject go = new GameObject("uteTagID_1555");
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent<MeshRenderer>();
				go.isStatic = true;
				go.GetComponent<Renderer>().material = (Material)de.Key;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);

				if(AddMeshColliders)
				{
					go.AddComponent<MeshCollider>();
				}
			}	
		}

		if(RemoveLeftOvers)
		{
			List<GameObject> children = new List<GameObject>();
			int counterpp = 0;
			foreach (Transform child in transform) 
			{
				children.Add(child.gameObject);
			}

			#if UNITY_EDITOR
			if(EditorApplication.isPlaying)
			{
			#endif
				for(int s=0;s<children.Count;s++)
				{
					if(children[s].name!="uteTagID_1555")
					{
						Destroy(children[s]);
					}
					else
					{
						children[s].name = "Batch_"+(counterpp++).ToString();
					}
				}							
			#if UNITY_EDITOR
			}
			else
			{
				for(int s=0;s<children.Count;s++)
				{
					if(children[s].name!=("uteTagID_1555"))
					{
						DestroyImmediate(children[s],true);
					}
					else
					{
						children[s].name = "Batch_"+(counterpp++).ToString();
					}
				}
			}
			#endif
		}
	}	
}