using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VocaTest : MonoBehaviour
{
    public VocaFX vocaFX;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EVocaType randomVocaType = (EVocaType)Random.Range(0, 3);
            vocaFX.PlayVoca(randomVocaType);
        }
    }
}
