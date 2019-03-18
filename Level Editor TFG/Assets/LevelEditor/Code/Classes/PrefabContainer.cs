using UnityEngine;
using UnityEditor;
using System;
[Serializable]
public class PrefabContainer
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

    //Tamaño de la caja contenedora desde el centro del objeto
    public Vector3 sizeBounds;

    private bool autosize = true;

    public PrefabContainer()
    {
        prefab = null;
        cellSize = Vector2Int.one;
        preview = null;
    }




#if UNITY_EDITOR
    private GUILayoutOption maxW = GUILayout.MaxWidth(100);
    private GUILayoutOption maxH = GUILayout.MaxHeight(50);
    public void ShowGUI(EditorWindow window, Action<PrefabContainer, PrefabAction> prefabAction)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview, maxW, maxH))
        {
            prefabAction.DynamicInvoke(this, PrefabAction.Select);
        }

        EditorGUILayout.BeginHorizontal(maxW, GUILayout.MaxHeight(25));
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevClose@2x"), prefabAction, this, PrefabAction.Delete);
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevResetEnv@2x"), prefabAction, this, PrefabAction.Reload);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }



    internal void showGUIEdit(EditorWindow window)
    {
        prefab = (GameObject)EditorGUILayout.ObjectField(Style.PREFAB_FIELD, prefab, typeof(GameObject), false);
        if (prefab != null)
        {
            autosize = EditorGUILayout.Toggle(Style.LABLE_AUTOSIZE,autosize);
            if (autosize)
            {
                Renderer render = prefab.GetComponentInChildren<Renderer>();
                if (render != null)
                {
                    Bounds b = render.bounds;
                    sizeBounds = b.extents;

                }
            }
            else
            {
                sizeBounds = EditorGUILayout.Vector3Field("Bounds Size",sizeBounds);
            }
        }
        cellSize = EditorGUILayout.Vector2IntField("Cell Size", cellSize);
    }

#endif
}