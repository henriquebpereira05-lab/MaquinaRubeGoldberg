using UnityEngine;
using System.Collections.Generic;

public class SequentialChainCamera : MonoBehaviour
{
    [Header("Sequencia da maquina")]
    [Tooltip("Colocar na ordem exata")]
    public List<Rigidbody> orderedBodies = new List<Rigidbody>();

    [Header("Enquadramento de camera")]
    public Vector3 offset = new Vector3(1.5f, 1.5f, -12f);
    public float smoothSpeed = 6f;

    [Header("Deteccao de inicio de movimento")]
    [Tooltip("Velocidade minima para considerar que o proximo item comecou a se mexer")]
    public float minStartSpeed = 0.25f;

    [Tooltip("Tempo minimo antes de permitir avancar para o proximo item")]
    public float minSwitchDelay = 0.35f;

    [Header("Limites opcionais de cena")]
    public bool clampPosition = false;
    public Vector2 minXY = new Vector2(-10f, -10f);
    public Vector2 maxXY = new Vector2(40f, 20f);

    private int currentIndex = 0;
    private float timeSinceLastSwitch = 0f;

    private void LateUpdate()
    {
        if (orderedBodies == null || orderedBodies.Count == 0) return;

        timeSinceLastSwitch += Time.deltaTime;

        if(timeSinceLastSwitch >= minSwitchDelay)
        {
            AdvanceToNextMovingBody();
        }

        Rigidbody target = orderedBodies[Mathf.Clamp(currentIndex, 0, orderedBodies.Count - 1)];
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (clampPosition)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minXY.x, maxXY.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minXY.y, maxXY.y);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    private void AdvanceToNextMovingBody()
    {
        for(int i = currentIndex + 1; i < orderedBodies.Count; i++)
        {
            Rigidbody candidate = orderedBodies[i];
            if (candidate == null || candidate.isKinematic) continue;

            if(candidate.linearVelocity.magnitude >= minStartSpeed)
            {
                currentIndex = i;
                timeSinceLastSwitch = 0f;
                break;
            }
        }
    }

    public void FocusMechanismIndex(int index)
    {
        if(index >= currentIndex && index < orderedBodies.Count)
        {
            currentIndex = index;
            timeSinceLastSwitch = 0f;
        }
    }
}
