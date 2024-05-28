using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PinchController : MonoBehaviour
{
    private InputAction pinchAction;
    public UnityEvent OnZoomIn;
    public UnityEvent OnZoomOut;

    private Vector2 primaryStartPos;
    private Vector2 secondaryStartPos;

    private void Awake()
    {
        pinchAction = new InputAction("Pinch", InputActionType.PassThrough, "<Touchscreen>/primaryTouch/delta,<Touchscreen>/secondaryTouch/delta");
        pinchAction.performed += OnPinch;
        pinchAction.Enable();
    }

    private void OnDestroy()
    {
        pinchAction.performed -= OnPinch;
        pinchAction.Disable();
    }
    
    private void OnPinch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            primaryStartPos = context.ReadValue<Vector2>();
            secondaryStartPos = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Performed)
        {
            Vector2 primaryCurrentPos = context.ReadValue<Vector2>();
            Vector2 secondaryCurrentPos = context.ReadValue<Vector2>();

            float startDistance = Vector2.Distance(primaryStartPos, secondaryStartPos);
            float currentDistance = Vector2.Distance(primaryCurrentPos, secondaryCurrentPos);

            float pinchAmount = currentDistance - startDistance;

            // pinchAmount가 양수이면 줌 인, 음수이면 줌 아웃
            if (pinchAmount > 0)
            {
                Debug.Log("줌인");
                OnZoomIn?.Invoke();
            }
            else if (pinchAmount < 0)
            {
                Debug.Log("줌아웃");
                OnZoomOut?.Invoke();
            }
        }
    }
}