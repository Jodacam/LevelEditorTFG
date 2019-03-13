using UnityEngine;
using UnityEditor;
using System;

public class PrefabContainer : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int cellSize;
    [HideInInspector]
    public Texture2D preview;

#if UNITY_EDITOR
    public void ShowGUI(EditorWindow window, Action<PrefabContainer> getPrefab)
    {
        if (GUILayout.Button(preview))
        {
            getPrefab.DynamicInvoke(this);
        }
    }
#endif
    internal void Init()
    {
        prefab = null;
        cellSize = Vector2Int.one;
        preview = null;
    }

    internal void showGUIEdit(EditorWindow window)
    {
        prefab = (GameObject) EditorGUILayout.ObjectField(Style.PREFAB_FIELD,prefab, typeof(GameObject), false);
        cellSize = EditorGUILayout.Vector2IntField("Cell Size", cellSize);
    }
}