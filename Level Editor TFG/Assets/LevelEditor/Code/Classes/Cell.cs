

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[Serializable]
public class Cell
{
    [Serializable]
    public class ObjectInfo
    {
        public GameObject gameObject;
        public float yOffset;

        public Vector3 realPosition;

        #if UNITY_EDITOR
        public void ShowGUI(EditorWindow window)
        {
            var preview = AssetPreview.GetAssetPreview(gameObject);
            if(GUILayout.Button(preview))
            {
                Selection.activeGameObject = gameObject;
            }
            if(GUILayout.Button(EditorGUIUtility.IconContent("LookDevClose@2x")))
            {
                    Debug.Log("Delete");
            }
        }
        #endif
    }
    //Posición en cordenadas de la malla, dado que el nivel se puede instanciar en un lugar que no sea el 0,0 se tendra que acceder mediante el nivel.
    public Vector3 position;
    public List<ObjectInfo> objectList;

    //TODO Posiblemente otra clase que contenga algo de información, como el tipo de suelo etc
    public int cellInfo;

    public Vector3 lastObjectPos
    {
        get
        {
            return objectList.Count > 0 ? objectList.Last().realPosition : position;
        }
    }

    public Cell(Vector3 middlePoint)
    {
        position = middlePoint;
        objectList = new List<ObjectInfo>();
    }

    internal void UpdatePosition(Transform t)
    {
        if (objectList != null)
        {
            foreach (var e in objectList)
                e.gameObject.transform.position = t.TransformPoint(e.realPosition);
        }
    }


    //Añade un objeto a la lista de la celda.
    internal void AddObject(SceneObjectContainer obj, Transform t)
    {

        ObjectInfo newInfo = new ObjectInfo();
        if (objectList.Count >= 1)
        {

            var last = objectList.Last();
            newInfo.realPosition = new Vector3(last.realPosition.x, last.realPosition.y + last.yOffset, last.realPosition.z);

        }
        else
        {
            newInfo.realPosition = position;
        }
        newInfo.yOffset = obj.Size.y;
        newInfo.gameObject = GameObject.Instantiate(obj.preview, newInfo.realPosition, obj.preview.transform.rotation, t);
        objectList.Add(newInfo);
    }

    internal void RemoveLast()
    {
        if (objectList.Count < 1)
        {
            return;
        }
        ObjectInfo info = objectList.Last();
        if (Application.isPlaying)
            GameObject.Destroy(info.gameObject);
        else
            GameObject.DestroyImmediate(info.gameObject);

        objectList.Remove(info);

    }

    internal GameObject GetObject(int layer)
    {
        return objectList[layer].gameObject;
    }

#if UNITY_EDITOR

    class CellEditWindow : EditorWindow
    {
        public static CellEditWindow CreateWindow(Cell owner)
        {
            var window =  GetWindow(typeof(CellEditWindow)) as CellEditWindow;
            window.owner = owner;
            window.maxSize = new Vector2(300, 100);
            window.minSize = window.maxSize;
            window.ShowUtility();
            return window;
        }

        Cell owner;
        private void OnGUI()
        {
            GUILayout.Label("Cell Properties");
            owner.cellInfo = EditorGUILayout.IntField("Cell Info", owner.cellInfo);
            GUILayout.Label("Objects");
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(minSize.x), GUILayout.MinWidth(50));
            int number = 0;
            try
            {
                foreach (var prefab in owner.objectList)
                {
                    prefab.ShowGUI(this);
                    number++;
                    if (number > 3)
                    {
                        EditorGUILayout.EndHorizontal();
                        number = 0;
                        EditorGUILayout.BeginVertical();
                        Rect rect = EditorGUILayout.GetControlRect(false, 1);
                        rect.height = 1;
                        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(minSize.x), GUILayout.MinWidth(50));
                    }
                }
            }
            catch (Exception e)
            {

            }
            EditorGUILayout.EndHorizontal();

        }
    }
    public void Edit(EditorWindow window)
    {
        CellEditWindow.CreateWindow(this);
    }
#endif
}