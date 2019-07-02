using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class that contains the object data of each cell.
/// </summary>
[Serializable]
public class LevelObjectData
{


    [SerializeField]
    public GameObject gameObject;

    [SerializeField]
    Renderer[] renderComponents;

    [SerializeField]
    Bounds renderBounds;

    [SerializeField]
    Vector3 position;

    [SerializeField]
    Vector3 objectPivot;
   

    public LevelObjectData(GameObject sceneObject, Vector3 position)
    {
        gameObject = sceneObject;
        this.position = position;
        renderComponents = sceneObject.GetComponentsInChildren<Renderer>();
        CalculateBounds();
        
    }

    public void CalculateBounds()
    {
        if(renderComponents.Length > 0)
        {
            Bounds b = renderComponents[0].bounds;
            for (int i = 1; i < renderComponents.Length; i++)
            {
                b.Encapsulate(renderComponents[i].bounds);
            }
            renderBounds = b;
            
        }
    }

    public float height { get { return renderBounds.size.y + position.y;} }



    /// <summary>
    /// Check if a ray have intersect with the render bound of the mesh use
    /// </summary>
    /// <param name="ray">The ray</param>
    /// <param name="distance">The distance between the origin of the ray and the object</param>
    /// <returns></returns>
    public bool RayCast(Ray ray, out float distance)
    {
        
        CalculateBounds();
        bool intersect = renderBounds.IntersectRay(ray, out distance);
        return intersect;
    }

}
