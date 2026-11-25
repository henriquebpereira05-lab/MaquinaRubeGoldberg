using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionLogger : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision c)
    {
        float v = Speed(rb);
        float ec = 0.5f * rb.mass * v * v;
        Debug.Log($"[{name}] bateu em {c.gameObject.name} | v relativa (impacto): {c.relativeVelocity.magnitude:F2} m/s | v depois: {v:F2} m/s | Ec depois: {ec:F2} J");
    }

    static float Speed(Rigidbody r)
    {
#if UNITY_6000_0_OR_NEWER
        return r.linearVelocity.magnitude;
#else
        return r.velocity.magnitude;
#endif
    }
}
