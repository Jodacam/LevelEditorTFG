using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectContainer
{
    private PrefabContainer objectInfo;
    public GameObject preview;
    public int xSize { get { return objectInfo.cellSize.x; } }
    public int ySize { get { return objectInfo.cellSize.x; } }
    public GameObject realObject { get { return objectInfo.prefab; } }
    public bool HasObject { get { return objectInfo != null; } }

    public Vector3 Size {get{return objectInfo.sizeBounds;}}
    public void SetObjectInfo(PrefabContainer prefab)
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
}
