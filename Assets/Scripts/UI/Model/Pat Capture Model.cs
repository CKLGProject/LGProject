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

    private ReactiveProperty<bool> _canBeCharacterized = new(false);
    public Observable<bool> CanBeCharacterizedAsObservable => _canBeCharacterized.AsObservable();
    
    public bool CanBeCharacterized
    {
        get => _canBeCharacterized.Value;
        set => _canBeCharacterized.Value = value;
    }
}