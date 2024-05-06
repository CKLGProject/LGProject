using UnityEngine;

namespace Utility
{
    public class ScreenOrientation : MonoBehaviour
    {
        [SerializeField] private UnityEngine.ScreenOrientation screenOrientation = UnityEngine.ScreenOrientation.LandscapeLeft;

        private void Start()
        {
            Screen.orientation = screenOrientation;
        }

        /// <summary>
        /// 모바일 디바이스 화면 방향 설정
        /// </summary>
        /// <param name="orientation"></param>
        public void SetScreenOrientation(UnityEngine.ScreenOrientation orientation)
        {
            screenOrientation = orientation;
            Screen.orientation = screenOrientation;
        }
    }
}