using R3;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BlipFX : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    [Tooltip("깜박이는 딜레이")] public float Delay = 1f;

    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1f;

        Observable.Interval(TimeSpan.FromSeconds(Delay)).Subscribe(_ =>
        {
            bool isShow = _canvasGroup.alpha > 0.5f;

            if (isShow)
                _canvasGroup.alpha = 0;
            else
                _canvasGroup.alpha = 1;
        }).AddTo(this);
    }
}