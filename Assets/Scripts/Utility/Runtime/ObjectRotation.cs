using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectRotation : MonoBehaviour
{
    [Tooltip("오브젝트 회전을 활성화합니다.")]
    public bool Active = true;
    
    [Tooltip("회전할 오브젝트를 지정합니다.")]
    public Transform content;
    
    [Tooltip("회전 속도를 지정합니다.")]
    public float RotationSpeed = 1;
    
    private void Update()
    {
        if (Active == false)
            return;
        
        if (Pointer.current.press.isPressed)
        {
            Vector2 delta = Pointer.current.delta.ReadValue() * Time.deltaTime;
            content.Rotate(Vector3.up, -delta.x * RotationSpeed, Space.World);
            content.Rotate(Vector3.right, delta.y * RotationSpeed, Space.World);
        }       
    }

    public void Reset()
    {
        content.rotation = Quaternion.identity;
    }
}
