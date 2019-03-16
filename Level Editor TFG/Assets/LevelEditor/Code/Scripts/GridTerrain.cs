using System;
using UnityEngine;

public class GridTerrain : MonoBehaviour {


    public static class GridTerrainProperties
    {
         public const string MATERIAL_GRID_SHADER = "LevelEditor/GridShow";
         public const string SHADER_PROPERTY_GRIDSCALEX = "_GridSizeX";
        public const string SHADER_PROPERTY_GRIDSCALEY = "_GridSizeY";
    }



    private Level owner;
    public int xSize, ySize;
    public float xScale,yScale;
    private MeshFilter mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;




    //En un principio Grid terrain tenia toda la lógica del grid. Dado que es un Monobehaviour, no se puede serializar y por lo tanto tiene que estar en Level.
    public void Init (float xS,float yS,Vector2Int size,Mesh m,Level o)
    {
       
        
        xSize = size.x;
        ySize = size.y;
        xScale = xS;
        yScale = yS;
        gameObject.layer = LayerMask.NameToLayer("Grid");
        mesh = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh.mesh = m;
        SetMaterial();
        collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = m;
        owner = o;
    }


    
    private void SetMaterial()
    {
        meshRenderer.material = new Material(Shader.Find(GridTerrainProperties.MATERIAL_GRID_SHADER));
        meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX, xScale);
        meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY, yScale);
    }

    
    public Vector3 GetClampPosition( RaycastHit hit)
    {
        //TODO
        Vector3 point = Vector3.zero;
        point.x = Mathf.Round(hit.point.x / xScale) * xScale;
        point.z = Mathf.Round(hit.point.z / yScale) * yScale;
        int triangle = hit.triangleIndex;
        point.y = hit.point.y;
        
        return owner.GetCellPosition(triangle);
    }

    public void SetObjetIntoCell(SceneObjectContainer selectObject, int triangleIndex)
    {
        owner.SetIntoCell(selectObject,triangleIndex);
    }
}