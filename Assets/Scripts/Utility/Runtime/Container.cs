using UnityEngine;

namespace Utility
{
    public class Container : MonoBehaviour
    {
        /// <summary>
        /// 위치를 초기화 합니다.
        /// </summary>
        public void PositionReset()
        {
            if (TryGetComponent(out RectTransform rectTransform))
            {
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}
