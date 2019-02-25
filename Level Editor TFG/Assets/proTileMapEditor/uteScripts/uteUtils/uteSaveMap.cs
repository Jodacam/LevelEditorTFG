using UnityEngine;
using System.Collections;
using System.IO;

public class uteSaveMap : MonoBehaviour {
#if UNITY_EDITOR
	[HideInInspector]
	public bool isSaving;

	private void Start()
	{
		isSaving = false;
	}

	public IEnumerator SaveMap(string mapName, bool isItMap)
	{
		isSaving = true;
		yield return 0;

		GameObject main = (GameObject) GameObject.Find("MAP");
		uteTagObject[] allObjects = main.GetComponentsInChildren<uteTagObject>();
		string info = "";

		for(int i=0;i<allObjects.Length;i++)
		{
			if(i%2000==0) yield return 0;
			
			GameObject obj = (GameObject) ((uteTagObject)allObjects[i]).gameObject;
			string objGUID = ((uteTagObject)allObjects[i]).objGUID;
			bool objIsStatic = ((uteTagObject)allObjects[i]).isStatic;
			bool objTC = ((uteTagObject)allObjects[i]).isTC;
			string tcFamilyName = "-";

			if(obj.GetComponent<uteTcTag>())
			{
				tcFamilyName = ((uteTcTag) obj.GetComponent<uteTcTag>()).tcFamilyName;
			}

			string staticInfo = "0";
			string tcInfo = "0";

			if(objIsStatic)
				staticInfo = "1";

			if(objTC)
				tcInfo = "1";

			info += objGUID+":"+obj.transform.position.x+":"+obj.transform.position.y+":"+obj.transform.position.z+":"+((int)obj.transform.localEulerAngles.x)+":"+((int)obj.transform.localEulerAngles.y)+":"+((int)obj.transform.localEulerAngles.z)+":"+staticInfo+":"+tcInfo+":"+tcFamilyName+":$";
		}

		string path;

		if(isItMap)
		{
			path = uteGLOBAL3dMapEditor.getMapsDir();
		}
		else
		{
			path = uteGLOBAL3dMapEditor.getPatternsDir();
		}

		StreamWriter sw = new StreamWriter(path+mapName+".txt");
		sw.Write("");
		sw.Write(info);
		sw.Flush();
		sw.Close();
		isSaving = false;

		yield return 0;
	}

	private float RoundToHalf(float point)
	{
		point *= 2.0f;
		point = Mathf.Round(point);
		point /= 2.0f;

		return point;
	}
#endif
}
