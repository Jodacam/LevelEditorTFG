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
    public override void ShowGUI(LevelEditor.Editor.PrefabCollectionWindow window)
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(preview, maxW, maxH))
        {
           window.SelectPrefab(this);
        }

        EditorGUILayout.BeginHorizontal(maxW,maxHButton);
        GUIStyle style = new GUIStyle(GUI.skin.button);
        
         if (GUILayout.Button(Style.ICON_EDIT, style, maxWButton, maxHButton))
        {
           window.Edit(this);
        }

      
        if (GUILayout.Button(Style.ICON_RELOAD, style, maxWButton, maxHButton))
        {
            window.Reload(this);
        }

       

        if (GUILayout.Button(Style.ICON_CLOSE,style, maxWButton, maxHButton))
        {
            window.DeletePrefab(this);
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
            autosize = EditorGUILayout.Toggle(Style.LABLE_AUTOSIZE,autosize);
            if (autosize)
            {
                
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
            AutoPivotGUI(render);
        }
       
        transitable = EditorGUILayout.Toggle("Transitable", transitable);
        
    }

#endif
    }
