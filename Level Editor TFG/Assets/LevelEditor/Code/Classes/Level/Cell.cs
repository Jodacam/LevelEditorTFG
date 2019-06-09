

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace LevelEditor
{
    [Serializable]
    public partial class Cell
    {



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
                return objectList.Count > 0 ? position + new Vector3(0, objectList.Last().yOffset + objectList.Last().realPosition.y, 0) : position;
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
        internal void AddObject(SceneObjectContainer obj, Transform t, Vector3 offset, bool instancing = false)
        {
            Transform parent = t;
            ObjectInfo newInfo = new ObjectInfo();
            if (objectList.Count >= 1)
            {

                var last = objectList.Last();
                newInfo.realPosition = lastObjectPos + offset - obj.Pivot;


            }
            else
            {
                newInfo.realPosition = obj.Position + offset;
            }
            newInfo.yOffset = obj.Size.y;
            newInfo.pivot = obj.Pivot;

            newInfo.gameObject = GUIAuxiliar.Instanciate(obj.Prefab, parent, newInfo.realPosition, obj.Rotation, obj.Scale, instancing);
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

        public void RemoveWall(int index)
        {
            WallInfo preWall = walls[index];
            Remove(preWall);
        }
        internal GameObject GetObject(int layer)
        {
            return objectList[layer].gameObject;
        }

        internal void SetObjectAsInfoOnly(SceneObjectContainer sceneObj, GameObject realObject)
        {
            var newInfo = new ObjectInfo();
            newInfo.gameObject = realObject;
            newInfo.yOffset = sceneObj.Size.y;
            newInfo.pivot = sceneObj.Pivot;
            newInfo.realPosition = realObject.transform.position;
            objectList.Add(newInfo);
        }

        internal void SetObjectAsInfoOnly(ObjectInfo info)
        {
            objectList.Add(info);
        }

        internal void AddWall(SceneObjectContainer obj, Transform t, int wallIndex, bool instancing)
        {
            WallInfo preWall = walls[wallIndex];
            if (preWall.prefabObject != null)
            {
                GUIAuxiliar.Destroy(preWall.prefabObject);
            }
            WallInfo wall = new WallInfo(obj, t, preWall.position, instancing);
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
                

                var window = GUIAuxiliar.OpenEditorWindow<CellEditWindow>("Cell Editor");
                window.owner = owner;
                window.maxSize = new Vector2(400, 250);
                window.minSize = window.maxSize;
                window.ShowUtility();
                return window;
            }

            Cell owner;


            private void OnGUI()
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);
                GUILayout.Label("Cell Properties");
                //owner.cellInfo = EditorGUILayout.IntField("Cell Info", owner.cellInfo);
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

                WallInfo[] infos = owner.walls;
                for (int i = 0; i < 4; i++)
                {
                    var w = infos[i];
                    w.ShowGUI(this, owner, i);
                }

            }

        }
        public void Edit(EditorWindow window)
        {
            CellEditWindow.CreateWindow(this);
        }


#endif
    }
}