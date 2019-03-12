using System;
using UnityEngine;

public class GridTerrain : MonoBehaviour {


    public static class GridTerrainProperties
    {
         public const string MATERIAL_GRID_SHADER = "LevelEditor/GridShow";
         public const string SHADER_PROPERTY_GRIDSCALEX = "_GridSizeX";
        public const string SHADER_PROPERTY_GRIDSCALEY = "_GridSizeY";
    }
    public int xSize, ySize;
    public float xScale,yScale;
    private MeshFilter mesh;
    private MeshRenderer meshRenderer;
    private MeshCollider collider;

    public void CreateGrid(float xS,float yS,Vector2Int size)
    {
        mesh = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        Mesh m = new Mesh();
        m.name = "Procedular";
        xSize = size.x;
        ySize = size.y;
        xScale = xS;
        yScale = yS;
        m.vertices = CreateVertex();
		m.triangles = CreateTris();
        m.RecalculateNormals();
        mesh.mesh = m;
        SetMaterial();
        collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = m;
        




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

    private int[] CreateTris()
    {
        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        return triangles;
    }

    private void SetMaterial()
    {
        meshRenderer.material = new Material(Shader.Find(GridTerrain.GridTerrainProperties.MATERIAL_GRID_SHADER));
        meshRenderer.sharedMaterial.SetFloat(GridTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEX, xScale);
        meshRenderer.sharedMaterial.SetFloat(GridTerrain.GridTerrainProperties.SHADER_PROPERTY_GRIDSCALEY, yScale);
    }

    public void GetClampPosition(GameObject selectObject, RaycastHit hit)
    {
        //TODO
        selectObject.transform.position = hit.point;
    }
}