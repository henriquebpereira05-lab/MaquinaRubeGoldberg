using UnityEngine;
using System.Collections.Generic;

public class ChainCameraFollow : MonoBehaviour
{
    [Header("Itens em movimento")]
    [Tooltip("Arraste aqui os itens moveis")]
    public List<Rigidbody> trackedBodies = new List<Rigidbody>();

    [Header("Enquadramento da camera")]
    [Tooltip("Distancia relativa da camera")]
    public Vector3 offset = new Vector3(1.5f, 1.5f, -12f);

    [Tooltip("Suavidade do movimento da camera")]
    public float smoothSpeed = 5f;

    [Tooltip("Velocidade minima para um item ser considerado em movimento")]
    public float minSpeedThreshold = 0.15f;

    [Header("Limites opcionais da cena")]
    public bool clampPosition = false;
    public Vector2 minXY = new Vector2(-10f, -10f);
    public Vector2 maxXY = new Vector2(30f, 15f);

    private Rigidbody currentTarget;

    private void LateUpdate()
    {
        Rigidbody activeBody = FindActiveMovingBody();
        if(activeBody != null)
        {
            currentTarget = activeBody;
        }

        if (currentTarget == null) return;

        Vector3 desiredPosition = currentTarget.position + offset;

        if (clampPosition)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minXY.x, maxXY.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minXY.y, maxXY.y);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    private Rigidbody FindActiveMovingBody()
    {
        Rigidbody bestBody = null;
        float highestSpeed = minSpeedThreshold;

        foreach(Rigidbody rb in trackedBodies)
        {
            if (rb == null || rb.isKinematic) continue;

            float speed = rb.linearVelocity.magnitude;
            if(speed > highestSpeed)
            {
                highestSpeed = speed;
                bestBody = rb;
            }
        }

        return bestBody;
    }
}
