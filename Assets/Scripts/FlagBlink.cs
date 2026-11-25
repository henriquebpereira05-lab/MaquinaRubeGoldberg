using UnityEngine;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;

[RequireComponent(typeof(Collider))]
public class FlagBlink : MonoBehaviour
{
    public Renderer flagRenderer;
    public Rigidbody ball;
    public Color colorA = new Color(0.55f, 0f, 0.1f);
    public Color colorB = Color.white;
    public float interval = 0.25f;
    public float duration = 5f;

    bool triggered;

    void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        flagRenderer.material.color = colorA;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered || other.attachedRigidbody != ball) return;
        triggered = true;
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        float t = 0f;
        bool a = false;
        while(t < duration)
        {
            flagRenderer.material.color = a ? colorA : colorB;
            a = !a;
            yield return new WaitForSeconds(interval);
            t += interval;
        }
        flagRenderer.material.color = colorA;
    }
}
