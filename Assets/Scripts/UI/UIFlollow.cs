using UnityEngine;
using UnityEngine.Serialization;

public class UIFollow : MonoBehaviour
{
    [FormerlySerializedAs("target")] public Transform Target; // 따라다닐 대상
    [FormerlySerializedAs("offset")] public Vector3 Offset; // 대상과의 위치 오프셋
    private RectTransform _rectTransform; // UI 요소의 RectTransform

    private Camera _mainCamera;
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        // UI 요소의 RectTransform을 가져옵니다.
        _rectTransform = GetComponent<RectTransform>();
        Target = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        if (Target)
        {
            // 대상의 월드 좌표를 스크린 좌표로 변환합니다.
            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(Target.position + Offset);

            // UI 요소의 위치를 스크린 좌표로 설정합니다.
            _rectTransform.position = screenPosition;
        }
    }
}