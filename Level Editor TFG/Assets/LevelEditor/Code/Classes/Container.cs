
using System;
using UnityEditor;
using UnityEngine;

public abstract class Container
{
    public enum PrefabAction
    {
        Select,
        Delete,
        Reload,
        Edit
    }
    public GameObject prefab;

    public Texture2D preview;


    //Pivot del objeto. El objeto se va a colocar aqui. lo que tengo que saber es ver donde está el pivot y Render.Bound.Center.
    public Vector3 pivot;
    public bool autoPivot;

    private bool autosize = true;
#if UNITY_EDITOR
    public abstract void ShowGUI(EditorWindow window, Action<Container, PrefabAction> prefabAction);
    public abstract void ShowGUIEdit(EditorWindow window);
    public virtual void Reload(EditorWindow window)
    {

        while (AssetPreview.IsLoadingAssetPreview(prefab.GetInstanceID()))
        {
            preview = AssetPreview.GetAssetPreview(prefab);
        }
    }
#endif



}
