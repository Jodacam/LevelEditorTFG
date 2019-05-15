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
    GameObject gameObject;

    [SerializeField]
    Renderer renderComponent;

    Bounds renderBounds;

    [SerializeField]
    Vector3 position;

    [SerializeField]
    Vector3 objectPivot;
   

    public LevelObjectData(GameObject sceneObject, Vector3 position)
    {
         gameObject = sceneObject;
        this.position = position;
        renderComponent = sceneObject.GetComponent<Renderer>();
        
    }

    public float height { get { if (renderBounds == null){renderBounds = renderComponent.bounds;} return renderBounds.size.y + position.y;} }



    /// <summary>
    /// Check if a ray have intersect with the render bound of the mesh use
    /// </summary>
    /// <param name="ray">The ray</param>
    /// <param name="distance">The distance between the origin of the ray and the object</param>
    /// <returns></returns>
    public bool RayCast(Ray ray, out float distance)
    {
        if (renderBounds == null)
        {
            renderBounds = renderComponent.bounds;
        }

        bool intersect = renderBounds.IntersectRay(ray, out distance);
        return intersect;
    }

}
