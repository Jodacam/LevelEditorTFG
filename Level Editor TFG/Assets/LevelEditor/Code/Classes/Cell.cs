

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
        public Vector3 pivot;
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

    [Serializable]
    public class WallInfo
    {
        public WallInfo(){}
        public WallInfo(WallContainer c,Transform t,Vector3 position)
        {
            height = c.height;
            transitable = true;
            this.position = position;
            //TODO
            prefabObject = GameObject.Instantiate(c.prefab, c.prefab.transform.position, c.prefab.transform.rotation, t);
        }
        public GameObject prefabObject;
        public Vector3 position;
        public float height;
        public bool transitable;
    }

    public Vector3 position;
    public List<ObjectInfo> objectList;
    public WallInfo[] walls;
    public Vector2 size;

    //TODO Posiblemente otra clase que contenga algo de información, como el tipo de suelo etc
    public int cellInfo;

    public Vector3 lastObjectPos
    {
        get
        {
            return objectList.Count > 0 ? objectList.Last().realPosition + objectList.Last().pivot: position;
        }
    }

    public Cell(Vector3 middlePoint,Vector2 Size)
    {
        position = middlePoint;
        objectList = new List<ObjectInfo>();
        walls = new WallInfo[4];
        for(int i = 0; i<walls.Length; i++){
            walls[i] = new WallInfo(){height = 0,transitable = true,prefabObject = null};
        }
        size = Size;

        walls[0].position = position + new Vector3(0,0,size.y);
        walls[1].position = position + new Vector3(size.x,0,0);
        walls[2].position = position + new Vector3(0,0,-size.y);
        walls[3].position = position + new Vector3(-size.x, 0, 0);

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
    internal void AddObject(SceneObjectContainer obj, Transform t,Vector3 offset)
    {
        Transform parent = t;
        ObjectInfo newInfo = new ObjectInfo();
        if (objectList.Count >= 1)
        {

            var last = objectList.Last();
            newInfo.realPosition = (new Vector3(last.realPosition.x, last.realPosition.y + last.yOffset, last.realPosition.z))+last.pivot +offset;
            parent= last.gameObject.transform;

        }
        else
        {
            newInfo.realPosition = obj.Position+offset;
        }
        newInfo.yOffset = obj.Size.y;
        newInfo.pivot = obj.Pivot;
        newInfo.gameObject = GameObject.Instantiate(obj.preview, newInfo.realPosition, obj.preview.transform.rotation, parent);
        objectList.Add(newInfo);
    }

    internal void RemoveLast()
    {
        if (objectList.Count < 1)
        {
            return;
        }
        ObjectInfo info = objectList.Last();
        GUIAuxiliar.Destroy(info.gameObject);   
        objectList.Remove(info);

    }

    internal GameObject GetObject(int layer)
    {
        return objectList[layer].gameObject;
    }


    internal void AddWall(SceneObjectContainer obj,Transform t,int wallIndex)
    {
        WallInfo preWall = walls[wallIndex];
        if(preWall.prefabObject != null)
        {
            GUIAuxiliar.Destroy(preWall.prefabObject);
        }
        WallInfo wall = new WallInfo(obj.GetAsWall(),t,preWall.position);      
        walls[wallIndex] = wall;
        

     
    }
#if UNITY_EDITOR

    class CellEditWindow : EditorWindow
    {
        Vector2 scrollPosition;
        public static CellEditWindow CreateWindow(Cell owner)
        {
            var window =  CellEditWindow.CreateInstance<CellEditWindow>();
            window.owner = owner;
            window.maxSize = new Vector2(500, 250);
            window.minSize = window.maxSize;
            window.ShowUtility();
            return window;
        }

        Cell owner;
        private void OnGUI()
        {
            EditorGUILayout.BeginScrollView(scrollPosition, false, false);
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

    internal Vector3 GetWallPosition(int wallPos)
    {
        return walls[wallPos].position;
    }
#endif
}