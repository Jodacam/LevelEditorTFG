using System;
using System.Collections.Generic;
using UnityEngine;


//Clase que contiene al objeto que se esta colocando en pantalla.
//Dado que este objeto puede ser usado tanto para paredes como prefabs normales se tiene que poner hacer un cast de este.

namespace LevelEditor
{
    public class SceneObjectContainer
    {
        private Container objectInfo;
        public GameObject preview;
        public int xSize { get { return (objectInfo).cellSize.x; } }
        public int ySize { get { return (objectInfo).cellSize.x; } }
        public GameObject realObject { get { return objectInfo.prefab; } }
        public bool HasObject { get { return objectInfo != null; } }

        private Vector3 internalPivot;
        //Tamaño del objeto.
        public Vector3 Size { get { return ((PrefabContainer)objectInfo).sizeBounds; } }
        // Pivot del objeto
        public Vector3 Pivot { get { return Vector3.Scale(internalPivot, objectInfo.scale); } }

        //Pivot en coordenadas globales
        public Vector3 WorldPivot { get { return preview.transform.TransformPoint(internalPivot); } }
        //Altura de la pared.
        public float Heigth { get { return GetAsWall().height; } }

        //Posicion en coordenadas globales del objeto.
        public Vector3 Position { get { return preview.transform.position; } }
        //rotacion actual
        public Quaternion Rotation { get { return preview.transform.rotation; } }
        //Tamaño en casilla (Vector2)
        public Vector2Int CellSize { get { return objectInfo.cellSize; } }
        //Número de celdas que tiene que ocupar en total.
        public int CellCountSize { get => CellSize.x * CellSize.y; }
        public Vector3 Scale { get => objectInfo.scale; }

        public GameObject Prefab { get => objectInfo.prefab; }
        //
        public void SetObjectInfo(Container prefab)
        {

            if (preview != null)
                GameObject.DestroyImmediate(preview);

            objectInfo = prefab;
            preview = GameObject.Instantiate(objectInfo.prefab);
            preview.transform.localScale = Vector3.Scale(realObject.transform.localScale, objectInfo.scale);
            internalPivot = prefab.pivot;
        }


        //Destruye el objeto que tenemos
        public void SetToNull()
        {
            GameObject.DestroyImmediate(preview);
            objectInfo = null;
        }

        //Recalcula el pivot para las 4 rotaciones.
        public void RecalculatePivot(int wall)
        {
            //Hay que mejorar mucho esto.


            switch (wall)
            {
                case 0:
                    internalPivot = objectInfo.pivot;
                    break;
                case 1:
                    internalPivot = new Vector3(objectInfo.pivot.z, objectInfo.pivot.y, -objectInfo.pivot.x);
                    break;
                case 2:
                    internalPivot = new Vector3(-objectInfo.pivot.x, objectInfo.pivot.y, -objectInfo.pivot.z);
                    break;
                case 3:
                    internalPivot = new Vector3(-objectInfo.pivot.z, objectInfo.pivot.y, objectInfo.pivot.x);
                    break;
            }


        }

        //Obtiene el objeto como una pared. Si el objeto es una pared, excepcion.
        internal WallContainer GetAsWall()
        {
            return (WallContainer)objectInfo;
        }


        //Obtiene el objeto como un prefab normal. Si el objeto es una pared esto da una excepcion.
        internal PrefabContainer GetAsPrefab()
        {
            return (PrefabContainer)objectInfo;
        }

        internal void SetAutoScale(Vector2 cellScale)
        {
            
            if(objectInfo.autoScale){
                objectInfo.CalculateScale(Vector2.Scale(cellScale,CellSize));
                preview.transform.localScale = objectInfo.scale;
            }
        }
    }
}
