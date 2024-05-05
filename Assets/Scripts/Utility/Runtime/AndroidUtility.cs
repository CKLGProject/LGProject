using System;
using UnityEngine;


namespace LGProjects.Android.Utility
{
    public enum EToast
    {
        LENGTH_SHORT = 0,
        LENGTH_LONG = 1
    }

    public class AndroidUtility
    {
        private static readonly AndroidJavaObject AndroidJavaObject = new AndroidJavaObject("com.unity3d.player.OverrideUnityPlayerActivity");

        /// <summary>
        /// Toast 메세지를 호출합니다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        public static void Toast(string message, EToast length = EToast.LENGTH_SHORT)
        {
            AndroidJavaObject.Call("Toast", message, (int)length);
        }
    }
}
