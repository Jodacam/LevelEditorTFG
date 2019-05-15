using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class that contains the MonoBehaviour of the level. It Contains its Terrain and Objects.
/// </summary>
public class LevelScript : MonoBehaviour
{

    [SerializeField]
    MeshFilter terrainMesh;
    [SerializeField]
    MeshCollider terrainMeshCollider;

    [SerializeField]
    public Level owner;

    [SerializeField]
    List<LevelObjectData> listOfObjects;
    
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
        newMesh.name = "Procedural";
        Vector3 startPoint = Vector3.zero;
        Vector3 endPoint = new Vector3(cellSize.x * cellCount.x, 0, cellSize.y * cellCount.y);
        Vector3 zPoint = new Vector3(0, 0, cellSize.y * cellCount.y);
        Vector3 xPoint = new Vector3(cellSize.x * cellCount.x, 0, 0);
        List<Vector3> vertex = new List<Vector3>() { startPoint, zPoint, endPoint, xPoint };
        int[] triangles = { 0, 1, 2, 0, 2, 3 };
        newMesh.SetVertices(vertex);
        newMesh.SetTriangles(triangles, 0);
        var renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial = new Material(Shader.Find(RegionTerrain.GridTerrainProperties.MATERIAL_GRID_SHADER));
        renderer.sharedMaterial.SetFloat(RegionTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX, cellSize.x);
        renderer.sharedMaterial.SetFloat(RegionTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY, cellSize.y);
        terrainMesh.sharedMesh = newMesh;
        terrainMeshCollider.sharedMesh = terrainMesh.sharedMesh;
        centerCellOffset = new Vector3(cellSize.x * 0.5f, 0, cellSize.y * 0.5f);
    }

    public void InitTerrain(Vector2 cellSize, Vector2Int cellCount, Level owner)
    {

        if (terrainMesh == null)
        {
            terrainMesh = GetComponent<MeshFilter>();
        }
        if (terrainMeshCollider == null)
        {
            terrainMeshCollider = GetComponent<MeshCollider>();
        }
        this.owner = owner;
        CreateMesh(cellSize, cellCount);
    }

    public void ScaleMesh(Vector2 cellSize, Vector2Int cellCount)
    {
        CreateMesh(cellSize, cellCount);
    }


    /// <summary>
    /// Gets the position of the cell.
    /// </summary>
    /// <param name="point">Point where the mouse touch</param>
    /// <param name="cellSize">Size of the object</param>
    /// <returns>Position on the field</returns>
    public Vector3 GetClampPositon(Vector3 point, Ray ray, Vector2Int cellSize)
    {
        return cellSize.x * cellSize.y == 1 ? GetCellPosition(point,ray) : GetCellPosition(point, cellSize,ray);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="cellSize"></param>
    /// <returns></returns>
    private Vector3 GetCellPosition(Vector3 point, Vector2Int cellSize, Ray ray)
    {
        Vector3 floorPos = new Vector3(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), Mathf.FloorToInt(point.z));
        Vector3[] positions = new Vector3[cellSize.x * cellSize.y];
        Vector3 totalPosition = Vector3.zero;
        for (int i = 0; i < cellSize.x; i++)
        {
            for (int j = 0; j < cellSize.y; j++)
            {
                Vector3 centerPosition = floorPos + centerCellOffset;
                centerPosition += new Vector3(i * owner.cellSize.x, 0, j * owner.cellSize.y);
                positions[i + j * cellSize.x] = centerPosition;
                totalPosition += centerPosition;
            }
        }
        float n = 1.0f / positions.Length;
        totalPosition *= n;


        float minDistance = float.MaxValue;
        float actualValue = 0;
        int selected = -1;
        for (int i = 0; i<listOfObjects.Count; i++)
        {
            if(listOfObjects[i].RayCast(ray, out actualValue))
            {
                if(actualValue < minDistance)
                {
                    selected = i;
                    minDistance = actualValue;
                }
            }
        }
        float h = listOfObjects[selected].height;
        return totalPosition + new Vector3(0,h,0);
    }

    private Vector3 GetCellPosition(Vector3 point,Ray ray)
    {
        

        float minDistance = float.MaxValue;
        float actualValue = 0;
        int selected = -1;
        for (int i = 0; i<listOfObjects.Count; i++)
        {
            if(listOfObjects[i].RayCast(ray, out actualValue))
            {
                if(actualValue < minDistance)
                {
                    selected = i;
                    minDistance = actualValue;
                }
            }
        }
        float h = listOfObjects[selected].height;
        Vector3 mousePositionClamp = new Vector3(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y) + h, Mathf.FloorToInt(point.z));
        mousePositionClamp += centerCellOffset;
        return mousePositionClamp;
    }

#if UNITY_EDITOR
    public void SaveItself(string path)
    {
        string exist = AssetDatabase.GetAssetPath(terrainMesh.sharedMesh);
        if (string.IsNullOrEmpty(exist))
        {
            AssetDatabase.CreateAsset(terrainMesh.sharedMesh, path + terrainMesh.name + ".mesh");
            AssetDatabase.CreateAsset(GetComponent<MeshRenderer>().sharedMaterial, path + "Material.mat");
        }
        
    }

    public void SetObject(SceneObjectContainer selectObject, Vector3 position,bool instancing = false)
    {
        var sceneObject = GUIAuxiliar.Instanciate(selectObject.Prefab, transform, position, selectObject.Rotation, selectObject.Scale, instancing);
        var dataObject = new LevelObjectData(sceneObject,position);
        listOfObjects.Add(dataObject);
    }
#endif
}