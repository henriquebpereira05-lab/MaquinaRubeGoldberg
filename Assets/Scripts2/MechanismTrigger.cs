using UnityEngine;

public class MechanismTrigger : MonoBehaviour
{
    public InitialImpulseStart controller;
    public int mechanismNumber = 1;
    public string description = "Bloco chega à base da rampa";
    public Rigidbody activator;

    private bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        Try(other.attachedRigidbody);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Try(collision.rigidbody);
    }

    private void Try(Rigidbody rb)
    {
        if (done || controller == null || rb == null) return;
        if (activator != null && rb != activator) return;

        done = true;
        controller.RegisterMechanism(mechanismNumber, description, rb.linearVelocity.magnitude);
    }
}
