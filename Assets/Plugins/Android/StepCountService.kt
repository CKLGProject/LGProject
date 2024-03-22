package com.unity3d.player

import android.app.NotificationChannel
import android.app.NotificationManager
import android.app.Service
import android.content.Intent
import android.hardware.Sensor
import android.hardware.SensorEvent
import android.hardware.SensorEventListener
import android.hardware.SensorManager
import android.os.Binder
import android.os.Build
import android.os.IBinder
import com.unity.androidnotifications.UnityNotificationManager

class StepCountService : Service(), SensorEventListener
{
    companion object
    {
        const val CHANNEL_ID: String = "LG_Channel_id"
    }

    private var number: Int = 0

    lateinit var sensorManager: SensorManager
    var stepCountSensor: Sensor? = null

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

    override fun onCreate()
    {
        super.onCreate()
        number = 0

        sensorManager = UnityPlayer.currentActivity.getSystemService(UnityPlayerActivity.SENSOR_SERVICE) as SensorManager
        stepCountSensor = sensorManager.getDefaultSensor(Sensor.TYPE_STEP_DETECTOR)

        sensorManager.registerListener(this, stepCountSensor, SensorManager.SENSOR_DELAY_FASTEST)
    }


    override fun onBind(intent: Intent): IBinder
    {
        return StepCountBinder()
    }

    inner class StepCountBinder : Binder()
    {
        val service: StepCountService
            get() = this@StepCountService

    }


    fun GetNumber(): Int
    {
        return number
    }

    private fun ProcessCommand(
        title: String?,
        message: String?,
        smallIcon: String?,
        largeIcon: String?
    )
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

    override fun onSensorChanged(event: SensorEvent?)
    {
        if (event != null)
            if (event.sensor.type == Sensor.TYPE_STEP_DETECTOR)
                if (event.values[0]== 1.0f) number +=1
    }

    override fun onAccuracyChanged(p0: Sensor?, p1: Int)
    {
        TODO("Not yet implemented")
    }
}