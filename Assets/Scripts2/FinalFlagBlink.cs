using UnityEngine;
using System.Collections;

public class FinalFlagBlink : MonoBehaviour
{
    public Renderer flagRenderer;
    public Rigidbody ballRigidbody;

    public Color blinkColor = Color.yellow;
    public float blinkInterval = 0.25f;
    public int blinkCount = 10;

    private Coroutine blinkRoutine;
    private bool hasBlinked = false;
    private Color originalColor;

    private void Awake()
    {
        if(flagRenderer != null)
        {
            originalColor = flagRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasBlinked) return;

        if(other.attachedRigidbody == ballRigidbody)
        {
            hasBlinked = true;
            StartBlinking();
        }
    }

    public void StartBlinking()
    {
        if (flagRenderer == null) return;

        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }

        blinkRoutine = StartCoroutine(BlinkRoutine());
    }

    public void StopBlinking(bool visibleAtEnd = true)
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (flagRenderer != null)
        {
            flagRenderer.material.color = originalColor;
        }
    }

    private IEnumerator BlinkRoutine()
    {
        int count = 0;
        bool useBlinkColor = true;

        while (blinkCount < 0 || count < blinkCount)
        {
            flagRenderer.material.color = useBlinkColor ? blinkColor : originalColor;
            useBlinkColor = !useBlinkColor;
            count++;
            yield return new WaitForSeconds(blinkInterval);
        }

        flagRenderer.material.color = originalColor;
        blinkRoutine = null;
    }
}