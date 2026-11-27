using UnityEngine;

#if UNITY_6000_0_OR_NEWER
using PMat = UnityEngine.PhysicsMaterial;
#else
using PMat = UnityEngine.PhysicsMaterial;
#endif

public class CurvedRamp : MonoBehaviour
{
    public float width = 1.5f;
    public float height = 3f;
    public float depth = 1f;
    public float thickness = 0.1f;
    [Range(6, 48)] public int segments = 24;
    public PMat material;

    [ContextMenu("Construir")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject c = transform.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(c); else DestroyImmediate(c);
        }

        Vector2 prev = Point(0f);
        for(int i = 1; i <= segments; i++)
        {
            Vector2 cur = Point(i / (float)segments);
            Vector2 d = cur - prev;
            float len = d.magnitude;
            Vector2 t = d / len;
            Vector2 n = new Vector2(-t.y, t.x);
            Vector2 mid = (prev + cur) * 0.5f - n * (thickness * 0.5f);

            GameObject seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = "Seg" + i;
            seg.transform.SetParent(transform, false);
            seg.transform.localPosition = new Vector3(mid.x, mid.y, 0f);
            seg.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(t.y, t.x) * Mathf.Deg2Rad);
            seg.transform.localScale = new Vector3(len * 1.05f, thickness, depth);
            if (material != null) seg.GetComponent<Collider>().sharedMaterial = material;

            prev = cur;
        }
    }

    Vector2 Point(float u)
    {
        float ang = Mathf.Lerp(180f, 270f, u) * Mathf.Deg2Rad;
        return new Vector2(width + width * Mathf.Cos(ang), height + height * Mathf.Sin(ang));
    }
}
