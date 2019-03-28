using UnityEngine;
using UnityEditor;
using System;
using static Container;

[Serializable]
public class PrefabContainer : Container
{


    public Vector2Int cellSize;
    [HideInInspector]

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
    public override void ShowGUI(EditorWindow window, Action<Container, PrefabAction> prefabAction)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview, maxW, maxH))
        {
            prefabAction.DynamicInvoke(this, PrefabAction.Select);
        }

        EditorGUILayout.BeginHorizontal(maxW, GUILayout.MaxHeight(25));
        Texture2D iconCancel = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "cross.png");
        
        GUIAuxiliar.Button(EditorGUIUtility.TrIconContent(iconCancel), prefabAction, this, PrefabAction.Delete);
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevResetEnv@2x"), prefabAction, this, PrefabAction.Reload);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
    }



    public override void ShowGUIEdit(EditorWindow window)
    {
        prefab = (GameObject)EditorGUILayout.ObjectField(Style.PREFAB_FIELD, prefab, typeof(GameObject), false);
        if (prefab != null)
        {
            Renderer render = prefab.GetComponentInChildren<Renderer>();
            autosize = EditorGUILayout.Toggle(Style.LABLE_AUTOSIZE,autosize);
            if (autosize)
            {
                
                if (render != null)
                {
                    Bounds b = render.bounds;
                    sizeBounds = b.size;
                    

                }
            }
            else
            {
                sizeBounds = EditorGUILayout.Vector3Field("Bounds Size",sizeBounds);
            }

            autoPivot = EditorGUILayout.Toggle(Style.LABLE_AUTOPIVOT,autoPivot);
            if (autoPivot)
            {

                if (render != null)
                {
                    Bounds b = render.bounds;
                    float y = render.bounds.center.y;
                    pivot = render.transform.InverseTransformPoint(b.center);
                    pivot.y -= y;

                }
            }
            else
            {
                pivot = EditorGUILayout.Vector3Field("Pivot", pivot);
            }
        }
        
        cellSize = EditorGUILayout.Vector2IntField("Cell Size", cellSize);
    }

#endif
}