using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Editor/Level", order = 0)]
public class Level : ScriptableObject {
    //Clase para guardar todos los string necesarios para la serialización del nivel. Suele ser mucho más sencillo si tengo que cambiar una variable.
    public static class LevelProperties{
        public const string NAME = "name";
        
    }
    
    public GridTerrain terrainGrid;
    public GameObject terrainGameObjec;
    public Mesh terrainMesh;
    public string name;
    public Vector2Int mapSize;
    public Vector2 mapScale;



    public float xcellSize { get { return mapScale.x; } set { mapScale.x = value; } }
    public float ycellSize { get { return mapScale.y; } set { mapScale.y = value; } }

    public int xSize {get { return mapSize.x; } set { mapSize.x = value; } }
    public int ySize {get { return mapSize.y; } set { mapSize.y = value; }}

    public void LoadGrid()
    {
        terrainGrid = Instantiate(terrainGameObjec,Vector3.zero,Quaternion.identity).GetComponent<GridTerrain>();
        terrainGrid.ReDoDictionary();
    }

    public void ReCreateGrid()
    {
       terrainMesh = terrainGrid.ChangeSize(xcellSize,ycellSize,mapSize);
    }

    public void CreateGrid()
    {
            GameObject plane = new GameObject("BaseTerrain", typeof(GridTerrain));
            plane.AddComponent<MeshFilter>();
            MeshRenderer e = plane.AddComponent<MeshRenderer>();
            plane.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);      
            //plane.GetComponent<GridTerrain>().CreateGrid(xcellSize, ycellSize, mapSize);
            terrainGrid = plane.GetComponent<GridTerrain>();
            terrainMesh = terrainGrid.Init(xcellSize,ycellSize,mapSize,this);
            
            
    }

    public void InitLevel(Vector3 position, Transform levelParent)
    {
        terrainGrid = Instantiate(terrainGameObjec,position,Quaternion.identity,levelParent).GetComponent<GridTerrain>();
        terrainGrid.ReDoDictionary();
    }
}