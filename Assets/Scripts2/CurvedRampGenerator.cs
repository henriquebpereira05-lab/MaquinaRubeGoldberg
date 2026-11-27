using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class CurvedRampGenerator : MonoBehaviour
{
    [Header("Dimensoes da curva")]
    [Tooltip("Raio interno da curva")]
    public float radius = 2f;

    [Tooltip("Largura da psita no eixo z")]
    public float width = 1.5f;

    [Tooltip("Espessura da parede da rampa")]
    public float thickness = 0.2f;

    [Tooltip("Angulo total da curva em graus")]
    [Range(10f, 180f)]
    public float arcAngle = 90f;

    [Tooltip("Quantidade de subdivisoes")]
    [Range(6, 64)]
    public int segments = 24;

    [Header("Orientacao")]
    [Tooltip("Marque para a curva descer da esquerda para a direita")]
    public bool descendLeftToRight = true;
    public bool flipInnerSurfaceWinding = false;

    [Header("Fisica")]
    public PhysicsMaterial rampPhysicsMaterial;

    private void OnValidate()
    {
        GenerateRamp();
    }

    [ContextMenu("Gerar rampa curva")]
    public void GenerateRamp()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        mesh.name = "CurvedRampMesh";

        int ringCount = segments + 1;
        Vector3[] vertices = new Vector3[ringCount * 4];
        int[] triangles = new int[segments * 24];

        float halfWidth = width * 0.5f;
        float outerRadius = radius + thickness;
        float totalRad = arcAngle * Mathf.Deg2Rad;

        for(int i = 0; i < ringCount; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.PI + (t * totalRad);

            float xDir = Mathf.Cos(angle);
            float yDir = Mathf.Sin(angle);

            if (!descendLeftToRight)
            {
                xDir = -xDir;
            }

            Vector3 innerPos = new Vector3(xDir * radius, yDir * radius, 0f);
            Vector3 outerPos = new Vector3(xDir * outerRadius, yDir * outerRadius, 0f);

            int vIndex = i * 4;
            vertices[vIndex + 0] = innerPos + new Vector3(0f, 0f, -halfWidth);
            vertices[vIndex + 1] = innerPos + new Vector3(0f, 0f, halfWidth);
            vertices[vIndex + 2] = outerPos + new Vector3(0f, 0f, -halfWidth);
            vertices[vIndex + 3] = outerPos + new Vector3(0f, 0f, halfWidth);
        }

        int triIndex = 0;
        for (int i = 0; i < segments; i++)
        {
            int cur = i * 4;
            int next = (i + 1) * 4;

            bool useLeftToRifhtOrder = descendLeftToRight ^ flipInnerSurfaceWinding;

            if (descendLeftToRight)
            {
                AddQuad(triangles, ref triIndex, cur + 0, next + 0, next + 1, cur + 1);
                AddQuad(triangles, ref triIndex, cur + 2, cur + 3, next + 3, next + 2);
                AddQuad(triangles, ref triIndex, cur + 0, cur + 2, next + 2, next + 0);
                AddQuad(triangles, ref triIndex, cur + 1, next + 1, next + 3, cur + 3);
            }
            else
            {
                AddQuad(triangles, ref triIndex, cur + 0, cur + 1, next + 1, next + 0);
                AddQuad(triangles, ref triIndex, cur + 2, next + 2, next + 3, cur + 3);
                AddQuad(triangles, ref triIndex, cur + 0, next + 0, next + 2, cur + 2);
                AddQuad(triangles, ref triIndex, cur + 1, cur + 3, next + 3, next + 1);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false;

        if(rampPhysicsMaterial != null)
        {
            meshCollider.sharedMaterial = rampPhysicsMaterial;
        }
    }

    private void AddQuad(int[] tris, ref int idx, int a, int b, int c, int d)
    {
        tris[idx++] = a;
        tris[idx++] = b;
        tris[idx++] = c;

        tris[idx++] = a;
        tris[idx++] = c;
        tris[idx++] = d;
    }
}
