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
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevClose@2x"), prefabAction, this, PrefabAction.Delete);
        GUIAuxiliar.Button(EditorGUIUtility.IconContent("LookDevResetEnv@2x"), prefabAction, this, PrefabAction.Reload);
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
                    height = b.extents.y;

                }
            }
            else
            {
                height = EditorGUILayout.FloatField("Height",height);
            }
        }
        
    }

#endif
    }
