

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
        public void ShowGUI(EditorWindow window, Cell owner)
        {
             EditorGUILayout.BeginVertical();
            var preview = AssetPreview.GetAssetPreview(gameObject);
            if (GUILayout.Button(preview, Style.maxH, Style.maxW))
            {
                Selection.activeGameObject = gameObject;
            }
            EditorGUILayout.BeginHorizontal(Style.maxWCompleteWall, Style.maxHButton);
            if (GUILayout.Button(Style.ICON_CLOSE, Style.maxHButton, Style.maxWButton))
            {
                owner.Remove(this);
            }
            if (GUILayout.Button(Style.ICON_RELOAD, Style.maxHButton, Style.maxWButton))
            {
                owner.Remove(this);
            }
            if (GUILayout.Button(Style.ICON_EDIT, Style.maxHButton, Style.maxWButton))
            {
                owner.Remove(this);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
#endif
    }


    //Posición en cordenadas de la malla, dado que el nivel se puede instanciar en un lugar que no sea el 0,0 se tendra que acceder mediante el nivel.

    [Serializable]
    public class WallInfo
    {
        public WallInfo()
        {
            height = 0;
            prefabObject = null;
            transitable = true;

        }
        public WallInfo(SceneObjectContainer c, Transform t, Vector3 position,bool instancing)
        {
            var wallInfo = c.GetAsWall();
            height = wallInfo.height;
            transitable = wallInfo.transitable;
            this.position = position;
            //TODO
            if (instancing)
            {
                prefabObject = PrefabUtility.InstantiatePrefab(wallInfo.prefab) as GameObject;
                prefabObject.transform.parent = t;
                prefabObject.transform.SetPositionAndRotation(c.Position, c.Rotation);
            }
            else
                prefabObject = GameObject.Instantiate(wallInfo.prefab, c.preview.transform.position, c.preview.transform.rotation, t);
        }
        public GameObject prefabObject;
        public Vector3 position;
        public float height;
        public bool transitable;

#if UNITY_EDITOR
        public void ShowGUI(EditorWindow window, Cell owner, int wallIndex)
        {
            EditorGUILayout.BeginVertical(Style.maxWCompleteWall, Style.maxHWalls);
            if (prefabObject != null)
            {
                var preview = AssetPreview.GetAssetPreview(prefabObject);
                if (GUILayout.Button(preview, Style.maxH, Style.maxWCompleteWall))
                {
                    Selection.activeGameObject = prefabObject;
                }
                EditorGUILayout.BeginHorizontal(Style.maxWCompleteWall, Style.maxHButton);
                if (GUILayout.Button(Style.ICON_CLOSE, Style.maxHButton, Style.maxWWalls))
                {
                    owner.RemoveWall(wallIndex);
                }
                if (GUILayout.Button(Style.ICON_RELOAD, Style.maxHButton, Style.maxWWalls))
                {
                    owner.RemoveWall(wallIndex);
                }
                if (GUILayout.Button(Style.ICON_EDIT, Style.maxHButton, Style.maxWWalls))
                {
                    owner.RemoveWall(wallIndex);
                }
                EditorGUILayout.EndHorizontal();
            }
            transitable = EditorGUILayout.Toggle("Is Transitable",transitable,Style.maxWCompleteWall);
            EditorGUILayout.EndVertical();
        }
#endif
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
            return objectList.Count > 0 ? objectList.Last().realPosition + objectList.Last().pivot : position;
        }
    }

    public Cell(Vector3 middlePoint, Vector2 Size)
    {
        position = middlePoint;
        objectList = new List<ObjectInfo>();
        walls = new WallInfo[4];
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i] = new WallInfo() { height = 0, transitable = true, prefabObject = null };
        }
        size = Size / 2;

        walls[1].position = position + new Vector3(size.x, 0, 0);
        walls[0].position = position + new Vector3(0, 0, size.y);
        walls[3].position = position + new Vector3(-size.x, 0, 0);
        walls[2].position = position + new Vector3(0, 0, -size.y);


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
    internal void AddObject(SceneObjectContainer obj, Transform t, Vector3 offset,bool instancing = false)
    {
        Transform parent = t;
        ObjectInfo newInfo = new ObjectInfo();
        if (objectList.Count >= 1)
        {

            var last = objectList.Last();
            newInfo.realPosition = (new Vector3(last.realPosition.x, last.realPosition.y + last.yOffset, last.realPosition.z)) + last.pivot + offset - obj.Pivot;
            parent = last.gameObject.transform;

        }
        else
        {
            newInfo.realPosition = obj.Position + offset;
        }
        newInfo.yOffset = obj.Size.y;
        newInfo.pivot = obj.Pivot;
        if (instancing)
        {
            newInfo.gameObject = PrefabUtility.InstantiatePrefab(obj.GetAsPrefab().prefab) as GameObject;
            newInfo.gameObject.transform.parent = parent;
            newInfo.gameObject.transform.SetPositionAndRotation(newInfo.realPosition, obj.Rotation);
        }
        else
            newInfo.gameObject = GameObject.Instantiate(obj.preview, newInfo.realPosition, obj.Rotation, parent);
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
    public void Remove(ObjectInfo objectInfo)
    {
        int index = objectList.IndexOf(objectInfo);
        if (index != -1)
        {
            objectList.Remove(objectInfo);
            
            for (int i = index; i < objectList.Count; i++)
            {

            }
            GUIAuxiliar.Destroy(objectInfo.gameObject);
        }
    }


    private void Remove(WallInfo wallInfo)
    {
        if (wallInfo.prefabObject != null)
        {
            GUIAuxiliar.Destroy(wallInfo.prefabObject);
        }

        var newWall = new WallInfo() { position = wallInfo.position };

    }

    private void RemoveWall(int index)
    {
        WallInfo preWall = walls[index];
        Remove(preWall);
    }
    internal GameObject GetObject(int layer)
    {
        return objectList[layer].gameObject;
    }


    internal void AddWall(SceneObjectContainer obj, Transform t, int wallIndex,bool instancing)
    {
        WallInfo preWall = walls[wallIndex];
        if (preWall.prefabObject != null)
        {
            GUIAuxiliar.Destroy(preWall.prefabObject);
        }
        WallInfo wall = new WallInfo(obj, t, preWall.position,instancing);
        walls[wallIndex] = wall;



    }

    internal Vector3 GetWallPosition(int wallPos)
    {
        return walls[wallPos].position;
    }
#if UNITY_EDITOR

    class CellEditWindow : EditorWindow
    {
        Vector2 scrollPosition;
        public static CellEditWindow CreateWindow(Cell owner)
        {
            var window = CellEditWindow.CreateInstance<CellEditWindow>();
            window.title = "Cell Editor";
            window.owner = owner;
            window.maxSize = new Vector2(500, 250);
            window.minSize = window.maxSize;
            window.ShowUtility();
            return window;
        }

        Cell owner;


        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
            GUILayout.Label("Cell Properties");
            owner.cellInfo = EditorGUILayout.IntField("Cell Info", owner.cellInfo);
            GUILayout.Label("Objects");
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(minSize.x), GUILayout.MinWidth(50));
            int number = 0;
            try
            {
                foreach (var prefab in owner.objectList)
                {
                    prefab.ShowGUI(this, owner);
                    number++;
                    if (number > 2)
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
            catch
            {

            }
            EditorGUILayout.EndHorizontal();


            DoWalls();



            EditorGUILayout.EndScrollView();

        }

        private void DoWalls()
        {
            GUILayout.Label("Walls");
            EditorGUILayout.BeginHorizontal();   
            WallInfo[] infos = owner.walls;
            for (int i = 0; i < 4; i++)
            {
                var w = infos[i];
                w.ShowGUI(this, owner, i);
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