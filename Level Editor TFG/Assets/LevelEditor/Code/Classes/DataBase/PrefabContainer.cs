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
    private GUILayoutOption maxWButton = GUILayout.MaxWidth(100/3);
    private GUILayoutOption maxHButton = GUILayout.MaxHeight(25);
    public override void ShowGUI(EditorWindow window, Action<Container, PrefabAction> prefabAction)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview, maxW, maxH))
        {
            prefabAction.Invoke(this, PrefabAction.Select);
        }

        EditorGUILayout.BeginHorizontal(maxW,maxHButton);
        GUIStyle style = new GUIStyle(GUI.skin.button);
        
         if (GUILayout.Button(Style.ICON_EDIT, style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Edit);
        }

      
        if (GUILayout.Button(Style.ICON_RELOAD, style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Reload);
        }

       

        if (GUILayout.Button(Style.ICON_CLOSE,style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Delete);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
    }

 

    public override void ShowGUIEdit(EditorWindow window)
    {
        prefab = (GameObject)EditorGUILayout.ObjectField(Style.PREFAB_FIELD, prefab, typeof(GameObject), false);
        if (prefab != null)
        {
            Renderer render = prefab.GetComponentInChildren<Renderer>();
            AutoScaleGUI(render, cellSize);
            autosize = EditorGUILayout.Toggle(Style.LABLE_AUTOSIZE,autosize);

            if (autosize)
            {
                
                if (render != null)
                {
                    Bounds b = render.bounds;
                    sizeBounds = Vector3.Scale(b.size,scale);
                    

                }
            }
            else
            {
                sizeBounds = EditorGUILayout.Vector3Field("Bounds Size",sizeBounds);
            }

            
            AutoPivotGUI(render);
        }
        
        cellSize = EditorGUILayout.Vector2IntField("Cell Size", cellSize);
    }

#endif
}