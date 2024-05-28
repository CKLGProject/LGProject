using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PinchController : MonoBehaviour
{
    [Tooltip("핀치 펼칠 때 감도")]
    public float PinchOutSensitivity = 40f;
    
    [Tooltip("핀치 모았을 때 감도")]
    public float PinchInSensitivity = 40f;
    
    /// <summary>
    /// 핀치 펼쳤을 때 호출되는 옵저버
    /// </summary>
    public Observable<float> OnPinchOutObservable => _pinchOutObject.AsObservable();
    
    /// <summary>
    /// 핀치 모았을 때 호출되는 옵저버
    /// </summary>
    public Observable<float> OnPinchInObservable => _pinchInObject.AsObservable();
    
    private Subject<float> _pinchOutObject;
    private Subject<float> _pinchInObject;
    
    private InputAction _touch0Contact;
    private InputAction _touch1Contact;
    private InputAction _touch0Position;
    private InputAction _touch1Position;
    
    private float _prevMagnitude;
    
    private void Awake()
    {
        // Init 
        _pinchOutObject = new Subject<float>();
        _pinchInObject = new Subject<float>();
        
        // Pinch gesture
        _touch0Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch0/press");
        _touch1Contact = new InputAction(type: InputActionType.Button, binding: "<Touchscreen>/touch1/press");
    
        _touch0Position = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch0/position");
        _touch1Position = new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/touch1/position");
    }
    
    private void OnEnable()
    {
        _touch0Contact.Enable();
        _touch1Contact.Enable();
        _touch0Position.Enable();
        _touch1Position.Enable();
    }
    
    private void OnDisable()
    {
        _touch0Contact.Disable();
        _touch1Contact.Disable();
        _touch0Position.Disable();
        _touch1Position.Disable();
    }
    
    private void Start()
    {
        _touch0Contact.canceled += ResetMagnitude;
        _touch1Contact.canceled += ResetMagnitude;
        _touch1Position.performed += OnTouchToPinch;
    }
    
    private void OnTouchToPinch(InputAction.CallbackContext obj)
    {
        // 핀치 제스처가 아닌 경우
        if (Touchscreen.current.touches.Count < 2)
            return;
    
        float magnitude = (_touch0Position.ReadValue<Vector2>() - _touch1Position.ReadValue<Vector2>()).magnitude;
    
        if (_prevMagnitude == 0)
            _prevMagnitude = magnitude;
    
        float difference = magnitude - _prevMagnitude;
        _prevMagnitude = magnitude;
    
        OnPinch(difference);
    }
    
    private void ResetMagnitude(InputAction.CallbackContext obj)
    {
        _prevMagnitude = 0;
    }
    
    private void OnPinch(float increment)
    {
        if (increment > PinchOutSensitivity)
            _pinchOutObject.OnNext(increment);
    
        if (increment < PinchInSensitivity)
            _pinchInObject.OnNext(increment);
    }
}