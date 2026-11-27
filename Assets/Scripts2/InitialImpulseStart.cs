using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PredictedMeasuredEntre
{
    public string label = "Velocidade na base da rampa";
    [Tooltip("Mecanismo cujo instante de contato sera medido")]
    public int mechanism = 1;
    [Tooltip("Valor calculado, em m/s")]
    public float predictedSpeed = 3.0f;
    [Tooltip("Texto TMP que mostra essa linha no HUD")]
    public TMP_Text rowText;
}

public class InitialImpulseStart : MonoBehaviour
{
    [Header("Bola e impulso inicial")]
    public Rigidbody ballRigidbody;
    [Tooltip("Impulso em N s ao longo do plano horizontal")]
    public Vector3 impulseForce = new Vector3(1.5f, 0f, 0f);
    public InputAction startAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
    [Tooltip("Se marcado, tambem dispara quando a bola encostar neste objeto")]
    public bool triggerOnContact = false;

    [Header("HUD")]
    public TMP_Text timerText;
    public TMP_Text activeMechanismText;

    [Header("Previsto x Medido (3 mecanismos)")]
    public PredictedMeasuredEntre[] comparisons = new PredictedMeasuredEntre[3];

    [Header("Evento final")]
    public FinalFlagBlink finalFlag;
    public int finalMechanismNumber;

    private bool hasStarted = false;
    private bool finished = false;
    private float elapsedTime = 0f;
    private readonly Dictionary<int, float> mechanismTime = new Dictionary<int, float>();
    private readonly Dictionary<int, float> mechanismSpeed = new Dictionary<int, float>();

    void OnEnable()
    {
        startAction.Enable();
    }

    private void OnDisable()
    {
        startAction.Disable();
    }

    private void Start()
    {
        if (activeMechanismText != null)
            activeMechanismText.text = "Mecanismo ativo: aguardando impulso (Espaço)";

        RefreshComparisons();
        UpdateHUD();
    }

    private void Update()
    {
        if(!hasStarted && startAction.WasPressedThisFrame())
        {
            ApplyInitialImpulse();
        }

        UpdateHUD();
    }

    private void FixedUpdate()
    {
        if (!hasStarted) return;

        elapsedTime += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnContact || hasStarted) return;
        if(other.attachedRigidbody == ballRigidbody)
        {
            ApplyInitialImpulse();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!triggerOnContact || hasStarted) return;
        if (collision.rigidbody == ballRigidbody)
        {
            ApplyInitialImpulse();
        }
    }

    public void ApplyInitialImpulse()
    {
        if (hasStarted || ballRigidbody == null) return;

        hasStarted = true;
        finished = false;
        elapsedTime = 0f;
        mechanismTime.Clear();

        ballRigidbody.AddForce(impulseForce, ForceMode.Impulse);

        RegisterMechanism(0, "Impulso inicial no plano horizontal");
    }

    public void RegisterMechanism(int mechanismNumber, string description, float speed = 0f)
    {
        if (!hasStarted && mechanismNumber > 0) return;
        if (finished) return;

        if (!mechanismTime.ContainsKey(mechanismNumber))
        {
            mechanismTime[mechanismNumber] = elapsedTime;
            mechanismSpeed[mechanismNumber] = speed;
            Debug.Log($"[MEDIDO] Mecanismo {mechanismNumber} ({description} em t = {elapsedTime:F3}s, v = {speed:F3} m/s");

            RefreshComparisons();

            if(mechanismNumber >= finalMechanismNumber)
            {
                finished = true;
                if (finalFlag != null) finalFlag.StartBlinking();
            }
        }

        if(activeMechanismText != null)
        {
            activeMechanismText.text = $"Mecanismo ativo: {mechanismNumber} - {description}";
        }
    }

    private void RefreshComparisons()
    {
        if (comparisons == null) return;

        foreach(var c in comparisons)
        {
            if (c == null || c.rowText == null) continue;

            if(mechanismSpeed.TryGetValue(c.mechanism, out float measured))
            {
                float error = c.predictedSpeed > 0f
                    ? Mathf.Abs(measured - c.predictedSpeed) / c.predictedSpeed * 100f : 0f;

                c.rowText.text = $"{c.label} (mec. {c.mechanism}\n" +
                                 $"Previsto: {c.predictedSpeed:F2} s | Medido: {measured:F2} s | Erro: {error:F1}%";

                Debug.Log($"[PREVISTO x MEDIDO] {c.label}: previsto = {c.predictedSpeed:F3}s, " +
                          $"medido = {measured:F3}s, erro = {error:F2}%");
            }
            else
            {
                c.rowText.text =
                    $"{c.label} (mec. {c.mechanism})\n" +
                    $"Previsto: {c.predictedSpeed:F2} s | Medido: aguardando...";
            }
        }
    }

    private void UpdateHUD()
    {
        if(timerText != null)
        {
            timerText.text = $"Tempo: {elapsedTime:F2} s";
        }
    }
}
