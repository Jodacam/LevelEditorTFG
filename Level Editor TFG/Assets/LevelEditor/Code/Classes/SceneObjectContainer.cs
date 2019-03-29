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

    public Vector3 Size {get{return((PrefabContainer)objectInfo).sizeBounds;}}
    public Vector3 Pivot { get { return objectInfo.pivot; } }
    public float Heigth {get{return GetAsWall().height;}}
    public Vector3 Position { get { return preview.transform.position; } }
    public void SetObjectInfo(Container prefab)
    {
        
        if (preview != null)
            GameObject.DestroyImmediate(preview);

        objectInfo = prefab;
        preview = GameObject.Instantiate(objectInfo.prefab);
    }

    public void SetToNull()
    {
        GameObject.DestroyImmediate(preview);
        objectInfo = null;
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
