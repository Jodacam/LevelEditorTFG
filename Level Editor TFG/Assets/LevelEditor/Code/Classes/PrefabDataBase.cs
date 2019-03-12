using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class PrefabDataBase : ScriptableObject
{
    [MenuItem("Tools/MyTool/Do It in C#")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }

    public string dataBaseName;
    public List<PrefabContainer> prefabList;
}