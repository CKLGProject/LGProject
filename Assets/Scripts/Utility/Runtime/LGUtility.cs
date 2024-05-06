using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public static class LGUtility
    {
        public enum EToast
        {
            LENGTH_SHORT = 0,
            LENGTH_LONG = 1
        }

        private static AndroidJavaObject _androidJavaObject;

        public static AndroidJavaObject AndroidJavaObject => _androidJavaObject ??=
            new AndroidJavaObject("com.unity3d.player.OverrideUnityPlayerActivity");

        public static bool IsEditorGameView()
        {
#if UNITY_EDITOR
            return UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop;
#endif
            return false;
        }

        /// <summary>
        /// 현재 터치된 위치를 반환합니다.
        /// </summary>
        /// <returns>터치 위치</returns>
        public static Vector3 ClickPosition()
        {
#if UNITY_EDITOR
            return Mouse.current.position.ReadValue();
#else
            return Touchscreen.current.primaryTouch.position.ReadValue();
#endif
        }

        /// <summary>
        /// 현재 터치가 눌렸는지 확인합니다.
        /// </summary>
        /// <returns></returns>
        public static bool GetTouchDown()
        {
#if UNITY_EDITOR
            return Mouse.current.leftButton.wasPressedThisFrame;
#else
            return Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
#endif
        }

        /// <summary>
        /// 해당 오브젝트가 터치되었는지 체크합니니다.
        /// 콜라이더2D가 요구됩니다.
        /// </summary>
        /// <param name="gameObject">체크할 오브젝트</param>
        /// <returns>터치되었을 경우 True를 반환합니다.</returns>
        public static bool IsTouchThis(GameObject gameObject)
        {
            var position = ClickPosition();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(position), out RaycastHit rayHit))
            {
                if (rayHit.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Toast 메세지를 호출합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        public static void Toast(string message, EToast length = EToast.LENGTH_SHORT)
        {
#if UNITY_EDITOR
            Debug.Log("Toast: " + message);
#else
            AndroidJavaObject.Call("Toast", message, (int)length);
#endif
        }
    }
}