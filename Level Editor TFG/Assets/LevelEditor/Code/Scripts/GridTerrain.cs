using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridTerrain : MonoBehaviour
{


    public static class GridTerrainProperties
    {
        public const string MATERIAL_GRID_SHADER = "LevelEditor/GridShow";
        public const string SHADER_PROPERTY_GRIDSCALEX = "_GridSizeX";
        public const string SHADER_PROPERTY_GRIDSCALEY = "_GridSizeY";
    }


#region  Variables
    private Level owner;
    public int xSize, ySize;
    public float xScale, yScale;
    private MeshFilter mesh;
    public MeshRenderer meshRenderer;
    private new MeshCollider collider;
    
    [SerializeField]
    public Cell[] cells;
    Dictionary<int, Cell> triangleToCells;

#endregion
    //En un principio Grid terrain tenia toda la lógica del grid. Dado que es un Monobehaviour, no se puede serializar y por lo tanto tiene que estar en Level.
    public Mesh Init(float xS, float yS, Vector2Int size, Level o)
    {

        xSize = size.x;
        ySize = size.y;
        xScale = xS;
        yScale = yS;
        Mesh m = new Mesh();
        m.name = "Procedular";
        m.vertices = CreateVertex();
        m.triangles = CreateTris(m);
        m.RecalculateNormals();
        gameObject.layer = LayerMask.NameToLayer("Grid");
        mesh = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh.mesh = m;
        SetMaterial();
        collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = m;
        owner = o;
        return m;
    }

    public Mesh ChangeSize(float xS, float yS, Vector2Int size)
    {
        ReCalculateBound(size);
        xSize = size.x;
        ySize = size.y;
        xScale = xS;
        yScale = yS;
        Mesh m = new Mesh();
        m.name = "Procedular";
        m.SetVertices(CreateVertex().ToList());   
        m.SetTriangles(CreateTris(m),0,true);
        m.RecalculateNormals();
        mesh.sharedMesh = m;
        collider.sharedMesh =m;
        SetMaterial();

        return mesh.sharedMesh;
    }

    private void ReCalculateBound(Vector2Int size)
    {
        //Si es mas grande o igual al anterior no hay que recalcular nada, solo recoger lo antiguo. 
        Cell[] newArray = new Cell[size.x*size.y];
        for(int y = 0; y <size.y && y<ySize; y++)
        {
            for(int x = 0; x <size.x && x<xSize; x++)
            {
                int i = getIndex(x,y);
                int e = (x+y*size.x);
                try
                {
                newArray[e] = cells[i];
                }
                catch(Exception exc)
                {
                    Debug.Log(exc.Message);
                }
            }
        }

        cells = newArray;
    }

    private Vector3[] CreateVertex()
    {
        Vector3[] vertex = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int j = 0; j <= xSize; j++, i++)
            {
                vertex[i] = new Vector3(j * xScale, 0, y * yScale);
            }
        }
        return vertex;
    }

    private int[] CreateTris(Mesh mesh)
    {

        int[] triangles = new int[xSize * ySize * 6];
        if(cells == null)
            cells = new Cell[xSize * ySize];

        List<Vector3> vertex = new List<Vector3>();
        mesh.GetVertices(vertex);
        triangleToCells = new Dictionary<int, Cell>();
        Vector2 scale = new Vector2(xScale, xScale);
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {

                
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;              


                //Esto es cada celda, tengo que ver una forma de identificar esto.
                Vector3[] QuadPoints = new Vector3[4] { vertex[vi], vertex[vi + 1], vertex[vi + xSize + 1], vertex[vi + xSize + 2] };
                // El centro de la celda en espacio local. Dado que es asi con Transform.TransformPoint(Vector3) puedo convetirlo a posiciones reales, por lo que el plano puede estar en cualquier parte.
                // El centro es la media de todo los puntos, por lo que es el centro encontrado en el plano que forman los 4 vertices.
                Vector3 MiddlePoint = (QuadPoints[0] + QuadPoints[1] + QuadPoints[2] + QuadPoints[3]) / 4;
               
                Cell c =  cells[getIndex(x, y)];
                if(c != null)
                {
                    c.position = MiddlePoint;
                    c.UpdatePosition(transform);
                }
                else
                {
                    c = new Cell(MiddlePoint,scale);
                }
                cells[getIndex(x, y)] = c;
                triangleToCells.Add(ti, c);
                triangleToCells.Add(ti + 3, c);

            }
        }
        return triangles;
    }

    public void ReDoDictionary()
    {
        triangleToCells = new Dictionary<int, Cell>();
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6)
            {
                triangleToCells.Add(ti, cells[getIndex(x, y)]);
                triangleToCells.Add(ti + 3, cells[getIndex(x, y)]);
            }
        }
    }
    private void SetMaterial()
    {
        if(meshRenderer.sharedMaterial == null)
            meshRenderer.material = new Material(Shader.Find(GridTerrainProperties.MATERIAL_GRID_SHADER));
        meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX, xScale);
        meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY, yScale);
    }


    public Vector3 GetClampPosition(RaycastHit hit)
    {
        //TODO
        int triangle = hit.triangleIndex;

        return GetCellPosition(triangle);
    }

    #region Cells
    public void SetObjetIntoCell(SceneObjectContainer selectObject, int triangleIndex,Vector3 offset)
    {
        Cell c = GetCell(triangleIndex);
        if (selectObject != null)
            c.AddObject(selectObject, transform,offset);
    }




    public Vector3 GetCellPosition(int x, int y)
    {
        return transform.TransformPoint(cells[getIndex(x, y)].position);
    }

    public Vector3 GetCellPosition(int x)
    {
        return transform.TransformPoint(GetCell(x).lastObjectPos);
    }

    public int GetCellInfo(int x, int y)
    {
        return cells[getIndex(x, y)].cellInfo;
    }

    public GameObject GetCellObject(int x, int y,int layer)
    {
        return GetCell(x,y).GetObject(layer);
    }

    public Cell GetCell(int triangle)
    {
        return triangleToCells[triangle * 3];
    }

    Cell GetCell(int x, int y)
    {
        return cells[getIndex(x,y)];
    }

    int getIndex(int x, int y)
    {
        return x + (y * xSize);
    }

    public void InitComponents()
    {
        mesh = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<MeshCollider>();
    }

    public void Remove(int triangleIndex) => GetCell(triangleIndex).RemoveLast();

    public Vector3 GetWallClampPosition(RaycastHit hit, int wallPos)
    {
        Cell c = GetCell(hit.triangleIndex);
        return transform.TransformPoint(c.GetWallPosition(wallPos));
    }

    public void SetWallIntoCell(SceneObjectContainer selectObject, int triangleIndex, int wallPos, Vector3 off)
    {
        Cell c = GetCell(triangleIndex);
        c.AddWall(selectObject, transform, wallPos);
    }
    #endregion
}