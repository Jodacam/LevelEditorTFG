using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LevelEditor
{
    public class RegionTerrain : MonoBehaviour
    {


        public static class GridTerrainProperties
        {
            public const string MATERIAL_GRID_SHADER = "LevelEditor/GridShow";
            public const string SHADER_PROPERTY_GRIDSCALEX = "_GridSizeX";
            public const string SHADER_PROPERTY_GRIDSCALEY = "_GridSizeY";
        }


        #region  Variables
        public LevelRegion owner;


        public int xSize, ySize;
        public float xScale, yScale;
        private MeshFilter mesh;
        public MeshRenderer meshRenderer;
        private new MeshCollider collider;

        [SerializeField]
        public Cell[] cells;


        #endregion
        //En un principio Grid terrain tenia toda la lógica del grid. Dado que es un Monobehaviour, no se puede serializar y por lo tanto tiene que estar en Level.
        public Mesh Init(float xS, float yS, Vector2Int size, LevelRegion o)
        {

            xSize = size.x;
            ySize = size.y;
            xScale = xS;
            yScale = yS;
            Mesh m = new Mesh();
            m.name = "Procedular";
            m.SetVertices(CreateVertex().ToList());
            m.SetTriangles(CreateTris(m), 0, true);
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
            if (mesh == null)
            {
                mesh = GetComponent<MeshFilter>();
                collider = GetComponent<MeshCollider>();
            }
            Mesh m = new Mesh();
            m.name = "Procedular";
            m.SetVertices(CreateVertex().ToList());
            m.SetTriangles(CreateTris(m), 0, true);
            m.RecalculateNormals();
            mesh.sharedMesh = m;
            collider.sharedMesh = m;
            SetMaterial();

            return mesh.sharedMesh;
        }

        private void ReCalculateBound(Vector2Int size)
        {
            //Si es mas grande o igual al anterior no hay que recalcular nada, solo recoger lo antiguo. 
            Cell[] newArray = new Cell[size.x * size.y];
            for (int y = 0; y < size.y && y < ySize; y++)
            {
                for (int x = 0; x < size.x && x < xSize; x++)
                {
                    int i = getIndex(x, y);
                    int e = (x + y * size.x);
                    try
                    {
                        newArray[e] = cells[i];
                    }
                    catch (Exception exc)
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
            if (cells == null)
                cells = new Cell[xSize * ySize];

            List<Vector3> vertex = new List<Vector3>();
            mesh.GetVertices(vertex);
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

                    Cell c = cells[getIndex(x, y)];
                    if (c != null)
                    {
                        c.position = MiddlePoint;
                        c.UpdatePosition(transform);
                    }
                    else
                    {
                        c = new Cell(MiddlePoint, scale);
                    }
                    cells[getIndex(x, y)] = c;

                }
            }
            return triangles;
        }

        public void ReDoDictionary()
        {


        }
        private void SetMaterial()
        {
            if (meshRenderer.sharedMaterial == null)
                meshRenderer.material = new Material(Shader.Find(GridTerrainProperties.MATERIAL_GRID_SHADER));
            meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX, xScale);
            meshRenderer.sharedMaterial.SetFloat(GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY, yScale);
        }


        public Vector3 GetClampPosition(RaycastHit hit, Vector2Int cellSize)
        {
            //TODO
            int triangle = hit.triangleIndex;
            Vector3 point = hit.point;

            return cellSize.x * cellSize.y == 1 ? GetCellPosition(point) : GetCellPosition(point, cellSize);
        }

        #region Cells
        public void SetObjetIntoCell(SceneObjectContainer selectObject, Vector3 triangleIndex, Vector3 offset, bool instancing = false)
        {
            if (selectObject != null)
            {
                Cell c = GetCell(triangleIndex);
                //Si el el modulo es igual a 1, significa que mide 1 sola casilla.
                if (selectObject.CellCountSize == 1)
                {

                    c.AddObject(selectObject, transform, offset, instancing);
                }
                else
                {
                    //obtenemos el indice del objeto.
                    int index = cells.ToList().IndexOf(c);
                    Vector2Int indexPosition = Get2DIndex(index);
                    AddObjectToCell(selectObject.CellSize, indexPosition, c, selectObject);
                }
            }
        }

        /**
        Add a Prefab Object into the cells. Takes the size of the object in cells an puts into the middle.
        
         */
        private void AddObjectToCell(Vector2Int size, Vector2Int indexPosition, Cell mainCell, SceneObjectContainer sceneObject, bool instancing = false)
        {
            var cellToObtain = new Cell[size.x * size.y];
            if (indexPosition.x + size.x >= xSize)
            {
                indexPosition.x = xSize - size.x;
            }

            if (indexPosition.y + size.y >= ySize)
            {
                indexPosition.y = xSize - size.y;
            }
            //Calculamos las celdas adyacentes.
            Vector3 position = Vector3.zero;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int index = i + j * size.x;
                    cellToObtain[index] = cells[getIndex(indexPosition.x + i, indexPosition.y + j)];

                    position += transform.TransformPoint(cellToObtain[index].lastObjectPos);
                }
            }
            position /= sceneObject.CellCountSize;
            position -= sceneObject.Pivot;
            GameObject realObject;

            realObject = GUIAuxiliar.Instanciate(sceneObject.Prefab, transform, position, sceneObject.Rotation, sceneObject.Scale, instancing);
            var newInfo = new ObjectInfo(cellToObtain, realObject, realObject.transform.position, sceneObject.Pivot, sceneObject.Size.y, size);
            Array.ForEach(cellToObtain, element => element.SetObjectAsInfoOnly(newInfo));
        }



        //Devuelve el indice en matriz dado un indice lineal.
        private Vector2Int Get2DIndex(int index)
        {
            //Dado que se como esta formado el array lineal puede saber cuando es la posicion.
            //El Array lineal se form con x + (y*xsize), por lo que por ejemplosi xsize = 5, la posición 6 del array seria x = 1 +  y = 1 * 5 => 1 + (1*5) = 6. Sabiendo eso, es que, 6/5 = 1. 6%5 = 1.
            // Otro ejemplo con 12, seria 12/5 = 2. y = 2; 12%5 = 2; x = 2. 2 + (2*5) = 12.
            int y = index / xSize;
            int x = index % xSize;
            return new Vector2Int(x, y);
        }

        private Vector2Int Get2DIndexByPosition(Vector3 point)
        {
            int x = Mathf.FloorToInt(point.x / xScale);
            int y = Mathf.FloorToInt(point.z / yScale);
            return new Vector2Int(x, y);
        }


        //Obtiene la posicion de una celda por sus indices
        public Vector3 GetCellPosition(int x, int y)
        {
            return transform.TransformPoint(cells[getIndex(x, y)].position);
        }

        /// <summary>
        /// Obtain a Cell position Given its colision position.
        /// </summary>
        /// <param name="point">The Colide Point</param>
        /// <returns>The Position</returns>
        public Vector3 GetCellPosition(Vector3 point)
        {

            return transform.TransformPoint(cells[getIndex(point)].lastObjectPos);
        }

        public Vector3 GetCellPosition(Vector3 point, Vector2Int size)
        {

            Vector2Int indexPosition = Get2DIndexByPosition(point);
            if (indexPosition.x + size.x >= xSize)
            {
                indexPosition.x = xSize - size.x;
            }

            if (indexPosition.y + size.y >= ySize)
            {
                indexPosition.y = xSize - size.y;
            }
            var cellToObtain = new Cell[size.x * size.y];
            //Calculamos las celdas adyacentes.
            Vector3 position = Vector3.zero;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    int index = i + j * size.x;
                    cellToObtain[index] = cells[getIndex(indexPosition.x + i, indexPosition.y + j)];

                    position += transform.TransformPoint(cellToObtain[index].lastObjectPos);
                }
            }
            position /= cellToObtain.Length;
            return position;
        }

        public int GetCellInfo(int x, int y)
        {
            return cells[getIndex(x, y)].cellInfo;
        }

        public GameObject GetCellObject(int x, int y, int layer)
        {
            return GetCell(x, y).GetObject(layer);
        }

        Cell GetCell(int x, int y)
        {
            return cells[getIndex(x, y)];
        }

        public Cell GetCell(Vector3 point)
        {
            return cells[getIndex(point)];
        }
        int getIndex(int x, int y)
        {
            return x + (y * xSize);
        }

        int getIndex(Vector3 point)
        {
            int x = Mathf.FloorToInt(point.x / xScale);
            int y = Mathf.FloorToInt(point.z / yScale);
            return x + (y * xSize);
        }

        public void InitComponents()
        {
            mesh = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            collider = GetComponent<MeshCollider>();
        }

        public void Remove(Vector3 triangleIndex)
        {
            #if UNITY_EDITOR
            Undo.RegisterFullObjectHierarchyUndo(this, "Remove cell");
            #endif
            GetCell(triangleIndex).RemoveLast();


        }

        public Vector3 GetWallClampPosition(RaycastHit hit, int wallPos)
        {
            Cell c = GetCell(hit.point);
            return transform.TransformPoint(c.GetWallPosition(wallPos));
        }

        public void SetWallIntoCell(SceneObjectContainer selectObject, Vector3 triangleIndex, int wallPos, Vector3 off, bool instancing = false)
        {
            Cell c = GetCell(triangleIndex);
            c.AddWall(selectObject, transform, wallPos, instancing);
        }
        #endregion
    }
}