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

    [SerializeField]
    Cell[] cells;

    Dictionary<int,Cell> triangleToCells;
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
    }

    public void ReCreateGrid()
    {
       
    }

    public void CreateGrid()
    {
            GameObject plane = new GameObject("BaseTerrain", typeof(GridTerrain));
            plane.AddComponent<MeshFilter>();
            MeshRenderer e = plane.AddComponent<MeshRenderer>();
            plane.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);      
            //plane.GetComponent<GridTerrain>().CreateGrid(xcellSize, ycellSize, mapSize);
            terrainGrid = plane.GetComponent<GridTerrain>();
            Mesh m = new Mesh();
            m.name = "Procedular";
            m.vertices = CreateVertex();
            m.triangles = CreateTris(m);
            m.RecalculateNormals();
            terrainMesh = m;
            terrainGrid.Init(xcellSize,ycellSize,mapSize,m,this);
            
    }


    private Vector3[] CreateVertex()
    {
        Vector3[] vertex = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int j = 0; j <= xSize; j++, i++)
            {
                vertex[i] = new Vector3(j * xcellSize, 0, y * ycellSize);
            }
        }
        return vertex;
    }

    Cell GetCell(int triangle)
    {
        return triangleToCells[triangle*3];
    }

    private int[] CreateTris(Mesh mesh)
    {
        
        int[] triangles = new int[xSize * ySize * 6];
        cells = new Cell[xSize*ySize];
        Vector3[] vertex = mesh.vertices;
        triangleToCells = new Dictionary<int, Cell>();
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {

                //Esto es cada celda, tengo que ver una forma de identificar esto.
                Vector3[] QuadPoints = new Vector3[4] {vertex[vi], vertex[vi+1], vertex[vi+xSize+1], vertex[vi+xSize+2] };
                // El centro de la celda en espacio local. Dado que es asi con Transform.TransformPoint(Vector3) puedo convetirlo a posiciones reales, por lo que el plano puede estar en cualquier parte.
                // El centro es la media de todo los puntos, por lo que es el centro encontrado en el plano que forman los 4 vertices.
                Vector3 MiddlePoint = (QuadPoints[0] + QuadPoints[1] + QuadPoints[2] + QuadPoints[3]) / 4;
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
                Cell c = new Cell(MiddlePoint);
                cells[x*y] = c; 
                triangleToCells.Add(ti,c);
                triangleToCells.Add(ti+3,c);

            }
        }
        return triangles;
    }

    public void SetIntoCell(SceneObjectContainer obj, int triangleIndex)
    {
        Cell c = GetCell(triangleIndex);
        c.cellObject = Instantiate(obj.preview,obj.preview.transform.position,Quaternion.identity,terrainGrid.transform);
    }


    public Vector3 GetCellPosition(int x, int y)
    {
        return terrainGrid.transform.TransformPoint(cells[x*y].position);
    }

    public Vector3 GetCellPosition(int x)
    {
        return terrainGrid.transform.TransformPoint(GetCell(x).position);
    }

    public int GetCellInfo(int x, int y){
        return cells[x*y].cellInfo;
    }

    public GameObject GetCellObject(int x, int y){
        return cells[x*y].cellObject;
    }

}