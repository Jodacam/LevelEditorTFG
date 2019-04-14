using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectContainer
{
    private Container objectInfo;
    public GameObject preview;
    public int xSize { get { return ((PrefabContainer)objectInfo).cellSize.x; } }
    public int ySize { get { return ((PrefabContainer)objectInfo).cellSize.x; } }
    public GameObject realObject { get { return objectInfo.prefab; } }
    public bool HasObject { get { return objectInfo != null; } }

    private Vector3 internalPivot;
    public Vector3 Size {get{return((PrefabContainer)objectInfo).sizeBounds;}}
    public Vector3 Pivot { get { return internalPivot; } }
    public Vector3 WorldPivot{get{return preview.transform.TransformPoint(internalPivot);}}
    public float Heigth {get{return GetAsWall().height;}}
    public Vector3 Position { get { return preview.transform.position; } }
    public Quaternion Rotation { get { return preview.transform.rotation; } }

    public Vector2Int CellSize{get{return GetAsPrefab().cellSize;}}
    public void SetObjectInfo(Container prefab)
    {
        
        if (preview != null)
            GameObject.DestroyImmediate(preview);

        objectInfo = prefab;
        preview = GameObject.Instantiate(objectInfo.prefab);
        internalPivot = prefab.pivot;
    }

    public void SetToNull()
    {
        GameObject.DestroyImmediate(preview);
        objectInfo = null;
    }


    public void RecalculatePivot(int wall)
    {
        //Hay que mejorar mucho esto.
        

        switch (wall)
        {
            case 0:
            internalPivot = objectInfo.pivot;
            break;
            case 1:
            internalPivot = new Vector3(objectInfo.pivot.z,objectInfo.pivot.y,-objectInfo.pivot.x);
            break;
            case 2:
            internalPivot = new Vector3(-objectInfo.pivot.x,objectInfo.pivot.y,-objectInfo.pivot.z);
            break;
            case 3:
            internalPivot = new Vector3(-objectInfo.pivot.z,objectInfo.pivot.y,objectInfo.pivot.x);
            break;
        }

        
    }
    internal WallContainer GetAsWall()
    {
        return (WallContainer) objectInfo;
    }

    internal PrefabContainer GetAsPrefab()
    {
        return (PrefabContainer) objectInfo;
    }
}
