using System;
using LGProjects.Android.Utility;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class SampleCode03 : MonoBehaviour
{
    private static int id;
    private const string ChannelID = "LG_Channel_id";

    public Text StepCountUI;

    private void Start()
    {
        int stepCount = AndroidUtility.GetStepCount();
        StepCountUI.text = stepCount.ToString();
    }

    public void PlayService()
    {
        AndroidUtility.TestService("LG 프로젝트", "사랑해요", "lg_small", "lg_large");
    }

    /// <summary>
    /// 노티피케이션을 3초후 울립니다.
    /// </summary>
    public void TestSend()
    {
        DateTime currentTime = DateTime.Now;
        //DateTime targetTime = currentTime.AddSeconds(3f);

        SendNotification("LG 테스트", "보상을 받아라!!!", currentTime, true);
    }

    /// <summary>
    /// 알림을 전달합니다.
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="message">내용</param>
    /// <param name="time">전달 시간</param>
    /// <param name="test">앱이 켜져있어도 울릴 것인가?</param>
    public static void SendNotification(string title, string message, DateTime time, bool test = false)
    {
#if UNITY_ANDROID
        NotificationStatus notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(id);

        Debug.Log("PPAP : " + notificationStatus);
        if (notificationStatus == NotificationStatus.Unknown)
        {
            var channel = new AndroidNotificationChannel() {
                Id = ChannelID,
                Name = "LG Project",
                Importance = Importance.Default,
                Description = "기본 알림"
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            AndroidNotification notification = new() {
                Title = title,
                Text = message,
                SmallIcon = "lg_small",
                LargeIcon = "lg_large",
                ShowInForeground = test,
                FireTime = time
            };

            AndroidNotificationCenter.SendNotification(notification, ChannelID);
        }
#endif
    }

    /// <summary>
    /// 알림을 모두 취소합니다.
    /// </summary>
    public static void CancelAllNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();
#endif
    }


}
