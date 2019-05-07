
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


    private Cell[] owners;
    public ObjectInfo(){}
    public ObjectInfo(Cell[] owners, GameObject gameObject,Vector3 realPosition,Vector3 pivot,float yOffset,Vector2Int size)
    {
        this.owners = owners;
        this.realPosition = realPosition;
        this.pivot = pivot;
        this.size = size;
        this.yOffset = yOffset;
        this.gameObject = gameObject;
    }

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
            Array.ForEach(owners, cell => cell.Remove(this));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
#endif
}


