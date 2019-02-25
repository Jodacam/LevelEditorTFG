using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class uteGLOBAL3dMapEditor : MonoBehaviour {

	public static bool isEditorRunning = false;
#if UNITY_EDITOR
	public static bool canBuild = false;
	public static int mapObjectCount = 0;
	public static bool XZsnapping = true;
	public static bool OverlapDetection = true;
	public static bool CalculateXZPivot = false;
	public static string uteCategoryInfotxt = "1f97d4f7dd2d64acf8e5c832a15ba53a";
	public static string uteMyMapstxt = "9e9936f47d5da4b88ad35169fb0d9982";
	public static string uteMyPatternstxt = "14541d53b60ba4c7cbf8e194d319d46a";
	public static string uteSettingstxt = "bf29964db71db4b81b9d25b4dc99d63a";
	public static string uteTileConnectionstxt = "c3b36fa978c5f48029aa1c4811f7ffa4";

	public static string getMapsDir()
	{
		string dir = AssetDatabase.GUIDToAssetPath("8af9fb7782c66401d9e3c5ebcc151cee");
		dir = dir.Replace("utemapsdirtagdonotdelete.txt","");
		return dir;
	}

	public static string getPatternsDir()
	{
		string dir = AssetDatabase.GUIDToAssetPath("e594b2557db7549f8aedcf80e01c62ed");
		dir = dir.Replace("utepatternsdirtagdonotdelete.txt","");
		return dir;
	}

	public static string getMyPatternsDir()
	{
		string dir = AssetDatabase.GUIDToAssetPath("14c6fcc5cd6c34faeb9053aba565446d");
		dir = dir.Replace("utemypatternsdirtagdonotdelete.txt","");
		return dir;
	}
#endif
}
