

using System;
using UnityEngine;
using UnityEditor;


//Posición en cordenadas de la malla, dado que el nivel se puede instanciar en un lugar que no sea el 0,0 se tendra que acceder mediante el nivel.

[Serializable]
public class WallInfo
{
    public WallInfo()
    {
        height = 0;
        prefabObject = null;
        transitable = true;

    }
    public WallInfo(SceneObjectContainer c, Transform t, Vector3 position, bool instancing)
    {
        var wallInfo = c.GetAsWall();
        height = wallInfo.height;
        transitable = wallInfo.transitable;
        this.position = position;
        //TODO
        prefabObject = GUIAuxiliar.Instanciate(wallInfo.prefab,t,c.preview.transform.position,c.preview.transform.rotation,Vector3.one,instancing);
        
    }
    public GameObject prefabObject;
    public Vector3 position;
    public float height;
    public bool transitable;

#if UNITY_EDITOR
    public void ShowGUI(EditorWindow window, Cell owner, int wallIndex)
    {

        EditorGUILayout.BeginVertical(Style.maxWCompleteWall, Style.maxHWalls);
        if (prefabObject != null)
        {
            var preview = AssetPreview.GetAssetPreview(prefabObject);
            if (GUILayout.Button(preview, Style.maxH, Style.maxWCompleteWall))
            {
                Selection.activeGameObject = prefabObject;
            }
            EditorGUILayout.BeginHorizontal(Style.maxWCompleteWall, Style.maxHButton);
            if (GUILayout.Button(Style.ICON_CLOSE, Style.maxHButton, Style.maxWWalls))
            {
                owner.RemoveWall(wallIndex);
            }
            EditorGUILayout.EndHorizontal();
        }
        transitable = EditorGUILayout.Toggle("Transitable?", transitable,Style.maxWCompleteWall);
        EditorGUILayout.EndVertical();
    }
#endif
}


