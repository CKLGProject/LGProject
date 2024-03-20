package com.unity3d.player

import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.Service
import android.content.Intent
import android.os.Build
import android.os.IBinder
import com.unity.androidnotifications.UnityNotificationManager

class StepCountService : Service()
{
    companion object
    {
        const val CHANNEL_ID: String = "LG_Channel_id";
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int
    {
        if (intent == null) return START_STICKY
        else
        {
            val title = intent.getStringExtra("Title")
            val message = intent.getStringExtra("Message")
            val smallIcon = intent.getStringExtra("SmallIcon")
            val largeIcon = intent.getStringExtra("LargeIcon")

            ProcessCommand(title, message, smallIcon, largeIcon)

            return super.onStartCommand(intent, flags, startId)
        }
    }

    override fun onBind(intent: Intent): IBinder
    {
        TODO("Return the communication channel to the service.")
    }

    private fun ProcessCommand(title:String?, message:String?, smallIcon: String?, largeIcon: String?)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            val notificationChannel = NotificationChannel(
                CHANNEL_ID,
                "LG Project",
                NotificationManager.IMPORTANCE_DEFAULT,
            )

            notificationChannel.description = "보상형 광고를 LG가 준데요"

            val unityNotificationManager = UnityNotificationManager.getNotificationManagerImpl(
                UnityPlayer.currentActivity,
                null
            )

            unityNotificationManager.registerNotificationChannel(
                notificationChannel.id,
                notificationChannel.name.toString(),
                notificationChannel.importance,
                notificationChannel.description,
                false,
                false,
                false,
                false,
                notificationChannel.vibrationPattern,
                1,
                null
            )

            val builder = unityNotificationManager.createNotificationBuilder(CHANNEL_ID)
                .setContentTitle(title)
                .setContentText(message)
            
            if (!smallIcon.isNullOrBlank())
            {
                UnityNotificationManager.setNotificationIcon(
                    builder,
                    UnityNotificationManager.KEY_SMALL_ICON,
                    smallIcon.toString()
                )
            }

            if (!largeIcon.isNullOrBlank())
            {
                UnityNotificationManager.setNotificationIcon(
                    builder,
                    UnityNotificationManager.KEY_LARGE_ICON,
                    largeIcon.toString()
                )
            }

            unityNotificationManager.scheduleNotification(builder, false)
        }

        val stopIntent = Intent(UnityPlayer.currentActivity, StepCountService::class.java)
        UnityPlayer.currentActivity.stopService(stopIntent)
    }
}