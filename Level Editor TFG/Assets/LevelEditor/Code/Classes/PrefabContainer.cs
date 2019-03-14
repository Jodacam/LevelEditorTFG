using UnityEngine;
using UnityEditor;
using System;

public class PrefabContainer : ScriptableObject
{

    public enum PrefabAction
    {
        Select,
        Delete,
        Reload
    }
    public GameObject prefab;
    public Vector2Int cellSize;
    [HideInInspector]
    public Texture2D preview;


    internal void Init()
    {
        prefab = null;
        cellSize = Vector2Int.one;
        preview = null;
    }




#if UNITY_EDITOR
    private GUILayoutOption maxW = GUILayout.MaxWidth(100);
    private GUILayoutOption maxH = GUILayout.MaxHeight(50);
    public void ShowGUI(EditorWindow window, Action<PrefabContainer,PrefabAction> prefabAction)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview,maxW,maxH))
        {
            prefabAction.DynamicInvoke(this,PrefabAction.Select);
        }
        
        EditorGUILayout.BeginHorizontal(maxW,GUILayout.MaxHeight(25));
        GUIAuxiliar.Button( EditorGUIUtility.IconContent("LookDevClose@2x"), prefabAction, this, PrefabAction.Delete);
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevResetEnv@2x"), prefabAction, this, PrefabAction.Reload);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    

    internal void showGUIEdit(EditorWindow window)
    {
        prefab = (GameObject) EditorGUILayout.ObjectField(Style.PREFAB_FIELD,prefab, typeof(GameObject), false);
        cellSize = EditorGUILayout.Vector2IntField("Cell Size", cellSize);
    }

#endif
}