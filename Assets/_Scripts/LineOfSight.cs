using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineOfSight : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    [Header("Vision Settings")]
    [Range(1f, 10f)] public float viewRadius = 10f;
    public int rayCount = 360;
    public LayerMask obstacleMask;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -1;
        transform.position = _tilemap.GetCellCenterLocal(_tilemap.WorldToCell(mouseWorldPos)) + _tilemap.transform.position;
    }

    private void LateUpdate()
    {
        GenerateViewMesh();
    }

    private void GenerateViewMesh()
    {
        float angleIncrement = 360f / rayCount;
        List<Vector3> viewPoints = new List<Vector3>();

        Vector3 origin = transform.position;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = i * angleIncrement;
            Vector3 dir = DirFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewRadius, obstacleMask);

            if (hit)
            {
                viewPoints.Add(hit.point);
            }
            else
            {
                viewPoints.Add(origin + dir * viewRadius);
            }
        }

        int vertexCount = viewPoints.Count + 1;
        vertices = new Vector3[vertexCount];
        triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < viewPoints.Count; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < viewPoints.Count - 1)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
