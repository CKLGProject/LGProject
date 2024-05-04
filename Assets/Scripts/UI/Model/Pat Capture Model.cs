using R3;
using UnityEngine;


public class PatCaptureModel : MonoBehaviour
{
    private readonly ReactiveProperty<bool> _isScanning = new();
    
    public bool IsScanning
    {
        get => _isScanning.Value;
        set => _isScanning.Value = value;
    }
}