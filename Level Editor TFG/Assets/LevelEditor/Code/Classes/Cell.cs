

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[Serializable]
public class Cell
{
    [Serializable]
    public class ObjectInfo
    {
        public GameObject gameObject;
        public float yOffset;

        public Vector3 realPosition;
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
            return objectList.Count>0 ? objectList.Last().realPosition : position;
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
}