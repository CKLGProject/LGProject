using R3;
using UnityEngine;


public class PatCaptureModel : MonoBehaviour
{
    private ReactiveProperty<bool> _canQRCodeCapture = new(false);
    public Observable<bool> CanQRCodeCaptureAsObservable => _canQRCodeCapture.AsObservable();
    
    public bool CanQRCodeCapture
    {
        get => _canQRCodeCapture.Value;
        set => _canQRCodeCapture.Value = value;
    }
}