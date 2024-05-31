using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform target; // 따라다닐 대상
    public Vector3 offset; // 대상과의 위치 오프셋
    private RectTransform uiElement; // UI 요소의 RectTransform

    void Start()
    {
        // UI 요소의 RectTransform을 가져옵니다.
        uiElement = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target != null)
        {
            // 대상의 월드 좌표를 스크린 좌표로 변환합니다.
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position + offset);

            // UI 요소의 위치를 스크린 좌표로 설정합니다.
            uiElement.position = screenPosition;
        }
    }
}