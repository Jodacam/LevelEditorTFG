using UnityEngine;

public class GridTerrain : MonoBehaviour {
    public int xSize, ySize;
    public float xScale,yScale;
    private MeshFilter mesh;

    public void CreateGrid(float xS,float yS,Vector2Int size)
    {
        mesh = GetComponent<MeshFilter>();
        Mesh m = new Mesh();
        m.name = "Procedular";
        xSize = size.x;
        ySize = size.y;
        xScale = xS;
        yScale = yS;
        Vector3[] vertex = new Vector3[(xSize + 1) * (ySize + 1)];
        for (int i = 0,y=0; y<= ySize; y++) {
             for (int j = 0; j <= xSize; j++,i++) {
				vertex[i] = new Vector3(j*xScale, 0,y*yScale);
			}
		}
		m.vertices = vertex;

		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		m.triangles = triangles;
        m.RecalculateNormals();
        mesh.mesh = m;

    }
}