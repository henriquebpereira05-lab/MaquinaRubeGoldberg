using UnityEngine;

public class SpringReleaseOnBallEnter : MonoBehaviour
{
    public Rigidbody glassBallRigidbody;
    public Rigidbody pusherBlockRigidbody;
    public SpringJoint springJoint;
    private bool released = false;

    private void Awake()
    {
        if(pusherBlockRigidbody != null)
        {
            pusherBlockRigidbody.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (released || glassBallRigidbody == null || pusherBlockRigidbody == null) return;
        if(other.attachedRigidbody == glassBallRigidbody)
        {
            released = true;
            pusherBlockRigidbody.isKinematic = false;
        }
    }
}
