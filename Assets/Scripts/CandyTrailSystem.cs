using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyTrailSystem : MonoBehaviour
{
    private const int MAX_QUAD_AMOUNT = 15000;

    private Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private int quadIndex;

    private void Awake()
    {
        mesh = new Mesh();

        vertices = new Vector3[4 * MAX_QUAD_AMOUNT];
        uv = new Vector2[4 * MAX_QUAD_AMOUNT];
        triangles = new int[6 * MAX_QUAD_AMOUNT];

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.bounds = new Bounds(new Vector3(0, 0), new Vector3(9, 9));

        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void DropWrapper(Vector3 position)
    {
        Vector3 diff = position - transform.position;
        Debug.Log("Dropping wrapper at world pos: " + position + " at local pos: " + diff);
        float angle = Random.Range(0, 359);
        AddQuad(diff, angle);
        Refresh();
    }

    // public void RandomWrapper()
    // {
    //     Vector3 pos = new Vector3(Random.Range(-4, 4), Random.Range(-4, 4));
    //     float angle = Random.Range(0, 359);
    //     AddQuad(pos, angle);
    //     Refresh();
    //     Debug.Log("Added wrapper");
    // }

    private void Refresh()
    {
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void AddQuad(Vector3 position, float rotation)
    {
        if (quadIndex >= MAX_QUAD_AMOUNT) return; // mesh full

        // reload vertices
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        Vector3 quadSize = new Vector3(0.2f, 0.2f);
        // float rotation = 0f;
        vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation - 180) * quadSize;
        vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation - 270) * quadSize;
        vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation - 0) * quadSize;
        vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation - 90) * quadSize;

        // uv
        uv[vIndex0] = new Vector2(0, 0);
        uv[vIndex1] = new Vector2(0, 1);
        uv[vIndex2] = new Vector2(1, 1);
        uv[vIndex3] = new Vector2(1, 0);

        // create triangles
        int tIndex = quadIndex * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex1;
        triangles[tIndex + 2] = vIndex2;

        triangles[tIndex + 3] = vIndex0;
        triangles[tIndex + 4] = vIndex2;
        triangles[tIndex + 5] = vIndex3;

        quadIndex++;
    }
}
