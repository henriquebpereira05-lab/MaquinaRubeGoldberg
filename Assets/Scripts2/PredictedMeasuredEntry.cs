using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

[Serializable]
public class PredictedMeasuredEntry
{
    public string label = "Descida da rampa";
    [Tooltip("Mecanismo onde a medicao comeca")]
    public int fromMechanism = 0;
    [Tooltip("Mecanismo onde a medicao termina")]
    public int toMechanism = 1;
    [Tooltip("Valor calculado, em segundos")]
    public float predictedSeconds = 1.20f;
    [Tooltip("Texto TMP que mostra essa linha no HUD")]
    public TMP_Text rowText;
}


