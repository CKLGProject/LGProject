using DG.Tweening;
using TMPro;
using UnityEngine;
using Utility;

[RequireComponent(typeof(CanvasGroup))]
public class Toast : MonoBehaviour
{
    private TextMeshProUGUI _toastText;
    private CanvasGroup _toastCanvasGroup;

    public string Message { get; set; }
    public LGUtility.EToast ToastType { get; set; } = LGUtility.EToast.LENGTH_SHORT;

    private Sequence _sequence;

    private void Start()
    {
        _sequence = DOTween.Sequence();
        _toastText = GetComponentInChildren<TextMeshProUGUI>();
        _toastCanvasGroup = GetComponent<CanvasGroup>();

        ToastMessage(Message, ToastType);
    }

    /// <summary>
    /// 토스트 메세지를 띄웁니다.
    /// </summary>
    /// <param name="message">메세지 내용</param>
    /// <param name="toastType">토스트 시간</param>
    private void ToastMessage(string message, LGUtility.EToast toastType = LGUtility.EToast.LENGTH_SHORT)
    {
        float duration;
        _toastCanvasGroup.alpha = 0;

        switch (toastType)
        {
            case LGUtility.EToast.LENGTH_SHORT:
                duration = 2f;
                break;
            case LGUtility.EToast.LENGTH_LONG:
                duration = 3.5f;
                break;
            default:
                duration = 2f;
                break;
        }

        const float fadeDuration = 0.25f;

        // Info Message Fade In & Out
        _sequence.Append(DOTween.To(() => _toastCanvasGroup.alpha, x => _toastCanvasGroup.alpha = x, 1, fadeDuration)
            .SetEase(Ease.Linear));
        _sequence.Append(DOTween.To(() => _toastCanvasGroup.alpha, x => _toastCanvasGroup.alpha = x, 0, fadeDuration)
            .SetEase(Ease.Linear).SetDelay(duration));
        _sequence.onComplete += () => Destroy(gameObject);
        _sequence.Play();

        _toastText.text = message;
    }
}