using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class SpringLauncher : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody plate;
    public Rigidbody anchor;
    public Rigidbody ball;

    [Header("Mola")]
    public float spring = 100f;
    public float damper = 20f;
    public float restLength = 0.6f;
    public float compression = 0.4f;
    public float releaseDelay = 0.3f;

    bool released;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;

        plate.constraints = RigidbodyConstraints.FreezePositionY
                          | RigidbodyConstraints.FreezePositionZ
                          | RigidbodyConstraints.FreezeRotation;
        plate.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        Vector3 p = anchor.position;
        p.x += restLength - compression;
        plate.position = p;
        plate.transform.position = p;
        plate.isKinematic = true;

        SpringJoint j = plate.gameObject.AddComponent<SpringJoint>();
        j.connectedBody = anchor;
        j.autoConfigureConnectedAnchor = false;
        j.anchor = Vector3.zero;
        j.connectedAnchor = Vector3.zero;
        j.spring = spring;
        j.damper = damper;
        j.minDistance = restLength;
        j.maxDistance = restLength;
        j.tolerance = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (released || other.attachedRigidbody != ball) return;
        released = true;
        Invoke(nameof(Release), releaseDelay);
    }

    void Release()
    {
        plate.isKinematic = false;
    }
}
