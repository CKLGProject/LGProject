using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class LogoFade : MonoBehaviour
    {
        [SerializeField] private CanvasGroup targetLogoGroup;

        [Tooltip("애니메이션 재생 시간")] public float FadeInDuration = 1;

        [Tooltip("로고를 잠시 보여주는 시간")] public float FadeOutDelay = 1;
        [Tooltip("애니메이션 재생 시간")] public float FadeOutDuration = 1;

        [Tooltip("페이드 아웃이 모두 끝나면 동작")] public UnityEvent OnFadeComplete;

        void Start()
        {
            if (targetLogoGroup == null)
            {
                Debug.LogError("CanvasGroup component is missing.");
                return;
            }

            // 초기 알파값을 0으로 설정하여 처음에는 보이지 않게 합니다.
            targetLogoGroup.alpha = 0;

            // 페이드 인 효과를 줍니다.
            PlayLogoFX();
        }

        /// <summary>
        /// 로고 페이드 효과를 동작합니다.
        /// </summary>
        public void PlayLogoFX()
        {
            targetLogoGroup.DOFade(1, FadeInDuration).SetEase(Ease.Linear).OnComplete(FadeOut); // 2초 동안 페이드 인 후 페이드 아웃 시작
        }

        /// <summary>
        /// 페이드 아웃을 처리합니다.
        /// </summary>
        private void FadeOut()
        {
            targetLogoGroup.DOFade(0, FadeOutDuration).SetDelay(FadeOutDelay).SetEase(Ease.Linear).OnComplete(() => OnFadeComplete?.Invoke()); // 2초 동안 페이드 아웃
        }
    }
}