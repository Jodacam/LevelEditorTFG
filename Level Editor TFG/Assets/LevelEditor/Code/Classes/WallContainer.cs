using UnityEngine;
using System;
using UnityEditor;
using static Container;
[Serializable]
public class WallContainer : Container
    {


        //Tamaño de la caja contenedora desde el centro del objeto
        public float  height;

        private bool autosize = true;
        public bool transitable = false;
        
       



#if UNITY_EDITOR
    private GUILayoutOption maxW = GUILayout.MaxWidth(100);
    private GUILayoutOption maxH = GUILayout.MaxHeight(50);
    private GUILayoutOption maxWButton = GUILayout.MaxWidth(100 / 3);
    private GUILayoutOption maxHButton = GUILayout.MaxHeight(25);
    public override void ShowGUI(EditorWindow window, Action<Container, PrefabAction> prefabAction)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview, maxW, maxH))
        {
            prefabAction.Invoke(this, PrefabAction.Select);
        }

        EditorGUILayout.BeginHorizontal(maxW, maxHButton);
        GUIStyle style = new GUIStyle(GUI.skin.button);

        Texture2D iconCancel = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "cross.png");

        Texture2D reload = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "return.png");


        Texture2D edit = AssetDatabase.LoadAssetAtPath<Texture2D>(GUIAuxiliar.PATH_LEVEL_EDITOR_ICON + "gear.png");

        if (GUILayout.Button(iconCancel, style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Delete);
        }
        if (GUILayout.Button(reload, style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Reload);
        }

        if (GUILayout.Button(edit, style, maxWButton, maxHButton))
        {
            prefabAction.Invoke(this, PrefabAction.Edit);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }




    public override void ShowGUIEdit(EditorWindow window)
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
                    height = b.size.y;

                }
            }
            else
            {
                height = EditorGUILayout.FloatField("Height",height);
            }
        }
        transitable = EditorGUILayout.Toggle("Transitable", transitable);
        
    }

#endif
    }
