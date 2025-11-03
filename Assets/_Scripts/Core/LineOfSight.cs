using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineOfSight : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField, Range(1f, 10f)] private float _viewRadius = 3f;
    [SerializeField] private int _rayCount = 360;
    [SerializeField] private LayerMask _obstaclesMask;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;

    public void SetRadius(float radius) => _viewRadius = radius;

    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void LateUpdate()
    {
        GenerateViewMesh();
    }

    private void GenerateViewMesh()
    {
        float angleIncrement = 360f / _rayCount;
        List<Vector3> viewPoints = new List<Vector3>();

        Vector3 origin = transform.position;

        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = i * angleIncrement;
            Vector3 dir = DirFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, _viewRadius, _obstaclesMask);

            if (hit)
                viewPoints.Add(hit.point);
            else
                viewPoints.Add(origin + dir * _viewRadius);
        }

        int vertexCount = viewPoints.Count + 1;
        _vertices = new Vector3[vertexCount];
        _triangles = new int[(vertexCount - 2) * 3];

        _vertices[0] = Vector3.zero;
        for (int i = 0; i < viewPoints.Count; i++)
        {
            _vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < viewPoints.Count - 1)
            {
                _triangles[i * 3] = 0;
                _triangles[i * 3 + 1] = i + 1;
                _triangles[i * 3 + 2] = i + 2;
            }
        }

        _mesh.Clear();
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.RecalculateNormals();
    }

    private Vector3 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += transform.eulerAngles.z;
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
