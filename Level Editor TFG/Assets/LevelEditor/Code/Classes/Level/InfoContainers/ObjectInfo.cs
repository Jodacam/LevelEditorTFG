
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class ObjectInfo
{
    public GameObject gameObject;
    public float yOffset;
    public Vector3 pivot;
    public Vector3 realPosition;
    public Vector2Int size = Vector2Int.one;

#if UNITY_EDITOR
    public void ShowGUI(EditorWindow window, Cell owner)
    {
        EditorGUILayout.BeginVertical();
        var preview = AssetPreview.GetAssetPreview(gameObject);
        if (GUILayout.Button(preview, Style.maxH, Style.maxW))
        {
            Selection.activeGameObject = gameObject;
        }
        EditorGUILayout.BeginHorizontal(Style.maxWCompleteWall, Style.maxHButton);
        if (GUILayout.Button(Style.ICON_CLOSE, Style.maxHButton, Style.maxWButton))
        {
            owner.Remove(this);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
#endif
}


