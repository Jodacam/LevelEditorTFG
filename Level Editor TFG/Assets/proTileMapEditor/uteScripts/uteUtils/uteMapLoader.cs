using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

[AddComponentMenu("proTileMapEditor/uteMapLoader")]
[ExecuteInEditMode]
#endif
public class uteMapLoader : MonoBehaviour {

	[SerializeField]
	public bool LoadAuto=true;
	[SerializeField]
	public bool StaticBatching=true;
	[SerializeField]
	public bool AddMeshColliders=false;
	[SerializeField]
	public bool RemoveLeftovers=true;
	[SerializeField]
	public Vector3 MapOffset = new Vector3(0,0,0);
	[HideInInspector]
	public GameObject[] refTiles;
	[HideInInspector]
	public string mapName;
	[HideInInspector]
	public bool isMapLoaded;

	public Vector3 loadMapOffset
	{
		get { return MapOffset; }
		set
		{
			if(MapOffset == value) return;

			MapOffset = value;
		}
	}

	public bool loadAutoVal
    {
        get { return LoadAuto; }
        set
        {
            if (LoadAuto == value) return;
 
            LoadAuto = value;
        }
    }

    public bool loadStaticBatching
    {
    	get { return StaticBatching; }
    	set
    	{
    		if(StaticBatching==value) return;

    		StaticBatching = value;
    	}
    }

    public bool loadAddMeshColliders
    {
    	get { return AddMeshColliders; }
    	set
    	{
    		if(AddMeshColliders==value) return;

    		AddMeshColliders = value;
    	}
    }

    public bool loadRemoveLeftovers
    {
    	get { return RemoveLeftovers; }
    	set
    	{
    		if(RemoveLeftovers==value) return;

    		RemoveLeftovers = value;
    	}
    }

    [HideInInspector]
    public string myLatestMap = "";

	#if UNITY_EDITOR
	[SerializeField]
	[HideInInspector]
	public int currentMapIndex;
	[HideInInspector]
	public string currentMapName;

	public int myMapIndexVal
    {
        get { return currentMapIndex; }
        set
        {
            if (currentMapIndex == value) return;
 
            currentMapIndex = value;
        }
    }

    public void SetMap(string name)
    {
    	isMapLoaded = false;
    	currentMapName = name;
    	mapName = name;
    	StreamReader sr = new StreamReader(uteGLOBAL3dMapEditor.getMapsDir()+name+".txt");
    	string allmapinfo = sr.ReadToEnd();
    	sr.Close();

    	string[] allMapBaseItems = allmapinfo.Split("$"[0]);
    	List<string> allGuids = new List<string>();

    	for(int i=0;i<allMapBaseItems.Length-1;i++)
    	{
    		string[] allInfoSplit = allMapBaseItems[i].Split(":"[0]);
    		allGuids.Add(allInfoSplit[0].ToString());
    	}

    	allGuids = RemoveDuplicates(allGuids);

    	refTiles = new GameObject[allGuids.Count];

    	for(int i=0;i<allGuids.Count;i++)
    	{
    		string guid = allGuids[i].ToString();
    		string opath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
			GameObject tGO = (GameObject) UnityEditor.AssetDatabase.LoadMainAssetAtPath(opath);
			refTiles[i] = tGO;
			allmapinfo = allmapinfo.Replace(guid,i.ToString());
		}

		myLatestMap = allmapinfo;
    }

    private List<string> RemoveDuplicates(List<string> myList)
    {
        List<string> newList = new List<string>();

        for(int i=0;i<myList.Count;i++)
            if (!newList.Contains(myList[i].ToString()))
                newList.Add(myList[i].ToString());

        return newList;
    }

	#endif

	private void Awake()
	{
		#if UNITY_EDITOR
		if(EditorApplication.isPlaying)
		{
		#endif
			if(LoadAuto)
			{
				LoadMap();
			}
		#if UNITY_EDITOR
		}
		#endif
	}

	public void LoadMap()
	{
		StartCoroutine(_LoadMap());
	}

	private IEnumerator _LoadMap()
	{
		#if UNITY_EDITOR
		Debug.Log("Loading Map... (This message appears only in the Editor)");
		#endif

		GameObject MAP = new GameObject(mapName);
		GameObject MAP_S = new GameObject("STATIC");
		GameObject MAP_D = new GameObject("DYNAMIC");
		MAP_S.transform.parent = MAP.transform;
		MAP_D.transform.parent = MAP.transform;

		string[] myMapInfoAll = myLatestMap.Split("$"[0]);

		for(int i=0;i<myMapInfoAll.Length-1;i++)
		{
			if(i%6000==0) yield return 0;

			string[] myMapParts = myMapInfoAll[i].Split(":"[0]);
			int objID = System.Convert.ToInt32(myMapParts[0]);
			GameObject obj = (GameObject) refTiles[objID];
			float pX = System.Convert.ToSingle(myMapParts[1]);
			float pY = System.Convert.ToSingle(myMapParts[2]);
			float pZ = System.Convert.ToSingle(myMapParts[3]);
			int rX = System.Convert.ToInt32(myMapParts[4]);
			int rY = System.Convert.ToInt32(myMapParts[5]);
			int rZ = System.Convert.ToInt32(myMapParts[6]);
			string staticInfo = myMapParts[7];
			bool isStatic = false;

			if(staticInfo.Equals("1"))
			{
				isStatic = true;
			}

			GameObject newObj = (GameObject) Instantiate(obj,new Vector3(pX,pY,pZ)+MapOffset+new Vector3(-500,0,-500),Quaternion.identity);
			newObj.name = objID.ToString();
			newObj.transform.localEulerAngles = new Vector3(rX,rY,rZ);

			if(isStatic)
			{
				newObj.isStatic = true;
				newObj.transform.parent = MAP_S.transform;
			}
			else
			{
				newObj.isStatic = false;
				newObj.transform.parent = MAP_D.transform;
			}
		}

		if(StaticBatching)
		{
			uteCombineChildren batching = (uteCombineChildren) MAP_S.AddComponent<uteCombineChildren>();
			batching.Batch(AddMeshColliders,RemoveLeftovers);
		}

		isMapLoaded = true;

		#if UNITY_EDITOR
		Debug.Log("Map LOADED! (This message appears only in the Editor)");
		#endif

		yield return 0;
	}

	public void LoadMapAsync(int frameSkip = 5)
	{
		StartCoroutine(_LoadMapAsync(frameSkip));
	}

	private IEnumerator _LoadMapAsync(int frameSkip)
	{
		#if UNITY_EDITOR
		Debug.Log("Loading Map... (This message appears only in the Editor)");
		#endif

		GameObject MAP = new GameObject(mapName);
		GameObject MAP_S = new GameObject("STATIC");
		GameObject MAP_D = new GameObject("DYNAMIC");
		MAP_S.transform.parent = MAP.transform;
		MAP_D.transform.parent = MAP.transform;

		string[] myMapInfoAll = myLatestMap.Split("$"[0]);

		for(int i=0;i<myMapInfoAll.Length-1;i++)
		{
			string[] myMapParts = myMapInfoAll[i].Split(":"[0]);
			int objID = System.Convert.ToInt32(myMapParts[0]);
			GameObject obj = (GameObject) refTiles[objID];
			float pX = System.Convert.ToSingle(myMapParts[1]);
			float pY = System.Convert.ToSingle(myMapParts[2]);
			float pZ = System.Convert.ToSingle(myMapParts[3]);
			int rX = System.Convert.ToInt32(myMapParts[4]);
			int rY = System.Convert.ToInt32(myMapParts[5]);
			int rZ = System.Convert.ToInt32(myMapParts[6]);
			string staticInfo = myMapParts[7];
			bool isStatic = false;

			if(staticInfo.Equals("1"))
			{
				isStatic = true;
			}

			GameObject newObj = (GameObject) Instantiate(obj,new Vector3(pX,pY,pZ)+MapOffset+new Vector3(-500,0,-500),Quaternion.identity);
			newObj.name = objID.ToString();
			newObj.transform.localEulerAngles = new Vector3(rX,rY,rZ);

			if(i%frameSkip==0)
			{
				yield return 0;
			}

			if(isStatic)
			{
				newObj.isStatic = true;
				newObj.transform.parent = MAP_S.transform;
			}
			else
			{
				newObj.isStatic = false;
				newObj.transform.parent = MAP_D.transform;
			}
		}

		if(StaticBatching)
		{
			uteCombineChildren batching = (uteCombineChildren) MAP_S.AddComponent<uteCombineChildren>();
			batching.Batch(AddMeshColliders,RemoveLeftovers);
		}

		isMapLoaded = true;

		#if UNITY_EDITOR
		Debug.Log("Map LOADED! (This message appears only in the Editor)");
		#endif

		yield return 0;
	}
}
