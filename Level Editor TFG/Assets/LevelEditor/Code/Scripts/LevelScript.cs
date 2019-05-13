using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains the MonoBehaviour of the level. It Contains its Terrain and Objects.
/// </summary>
public class LevelScript : MonoBehaviour {
    
    [SerializeField]
    MeshFilter terrainMesh;
    [SerializeField]
    MeshCollider terrainMeshCollider;

    [SerializeField]
    public Level owner;


    private Vector3 centerCellOffset;
    /// <summary>
    /// Creates the map. In regions we have a full Cell System beacuse we only put objects. Here we have a QuadMesh to RayCast, but there is no real Cells.
    /// The Object will be positiones with a Clamp, but no into a specific Cell.
    /// </summary>
    /// <param name="cellSize"> The size of each cell</param>
    /// <param name="cellCount">The number of cells, cellCount.x = x, cellCount.y = z</param>
    public void CreateMesh(Vector2 cellSize, Vector2Int cellCount)
    {

        Mesh newMesh = new Mesh();
        Vector3 startPoint = Vector3.zero;
        Vector3 endPoint = new Vector3(cellSize.x*cellCount.x,0,cellSize.y*cellCount.y);
        Vector3 zPoint = new Vector3(0,0,cellSize.y*cellCount.y);
        Vector3 xPoint = new Vector3(cellSize.x*cellCount.x,0,0);
        List<Vector3> vertex = new List<Vector3>(){startPoint,zPoint,endPoint,xPoint};
        int[] triangles = {0,1,2,0,2,3};
        newMesh.SetVertices(vertex);
        newMesh.SetTriangles(triangles,0);
        var renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find(RegionTerrain.GridTerrainProperties.MATERIAL_GRID_SHADER));
        renderer.sharedMaterial.SetFloat(RegionTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX,cellSize.x);
        renderer.sharedMaterial.SetFloat(RegionTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY,cellSize.y);
        terrainMesh.sharedMesh = newMesh;
        terrainMeshCollider.sharedMesh = newMesh;
        centerCellOffset = new Vector3(cellSize.x*0.5f,0,cellSize.y*0.5f);
    }

    public void InitTerrain(Vector2 cellSize, Vector2Int cellCount)
    {

        if(terrainMesh == null){
            terrainMesh = GetComponent<MeshFilter>(); 
        }
        if (terrainMeshCollider== null){
            terrainMeshCollider = GetComponent<MeshCollider>();
        }
        CreateMesh(cellSize,cellCount);
    }

    public void ScaleMesh(Vector2 cellSize, Vector2Int cellCount){
        CreateMesh(cellSize,cellCount);
    }

    public Vector3 GetClampPositon(Vector3 point, Vector2Int cellSize)
    {
        Vector3 mousePositionClamp = new Vector3(Mathf.FloorToInt(point.x),Mathf.FloorToInt(point.y),Mathf.FloorToInt(point.z));
        mousePositionClamp += centerCellOffset;
        return mousePositionClamp;
    }
}