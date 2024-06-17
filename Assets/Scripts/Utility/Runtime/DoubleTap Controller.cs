using R3;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleTapController : MonoBehaviour
{
    private Subject<Unit> _onTouchDown;
    
    private InputAction _touchPrimaryDoubleTap;

    private void Awake()
    {
        _onTouchDown = new Subject<Unit>();
        _touchPrimaryDoubleTap = InputSystem.actions.FindActionMap("System").FindAction("Double Tap");
    }

    private void Start()
    {
        _touchPrimaryDoubleTap.performed += OnDoubleTouch;
    }

    private void OnEnable()
    {
        _touchPrimaryDoubleTap.Enable();
    }

    private void OnDisable()
    {
        _touchPrimaryDoubleTap.Disable();
    }
    
    private void OnDoubleTouch(InputAction.CallbackContext obj)
    {
        _onTouchDown.OnNext(Unit.Default);
    }

    /// <summary>
    /// 더블 터치를 인식하는 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> OnDoubleTouchObservable()
    {
        return _onTouchDown.AsObservable();
    }
}