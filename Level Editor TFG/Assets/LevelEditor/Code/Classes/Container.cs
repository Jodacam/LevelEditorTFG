
using System;
using UnityEditor;
using UnityEngine;

public abstract class Container
    {
    public enum PrefabAction
    {
        Select,
        Delete,
        Reload
    }
     public GameObject prefab;
    
    public Texture2D preview;

    //Tamaño de la caja contenedora desde el centro del objeto
    

    private bool autosize = true;
        #if UNITY_EDITOR
        public abstract void ShowGUI(EditorWindow window, Action<Container, PrefabAction> prefabAction);
        public abstract void ShowGUIEdit(EditorWindow window);
        #endif



    }
