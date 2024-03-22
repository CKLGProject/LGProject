package com.unity3d.player

import android.content.ComponentName
import android.content.Intent
import android.content.ServiceConnection
import android.os.Build
import android.os.IBinder
import android.widget.Toast

class LGUnityPlayerActivity : UnityPlayerActivity()
{
    var stepCountService: StepCountService? = null

    private fun Toast(message: String, duration: Int)
    {
        UnityPlayer.currentActivity.runOnUiThread {
            Toast.makeText(UnityPlayer.currentActivity, message, duration).show()
        }
    }

    private fun StartStepCountService(
        title: String,
        message: String,
        smallIcon: String,
        largeIcon: String
    )
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            val targetIntent = Intent(UnityPlayer.currentActivity, StepCountService::class.java)
            targetIntent.putExtra("Title", title)
            targetIntent.putExtra("Message", message)
            targetIntent.putExtra("SmallIcon", smallIcon)
            targetIntent.putExtra("LargeIcon", largeIcon)

            UnityPlayer.currentActivity.bindService(
                targetIntent,
                connection,
                UnityPlayerActivity.BIND_AUTO_CREATE
            )

        }
    }


    val connection: ServiceConnection = object : ServiceConnection
    {
        override fun onServiceConnected(name: ComponentName?, service: IBinder?)
        {
            val binder = service as StepCountService.StepCountBinder
            stepCountService = binder.service
        }

        override fun onServiceDisconnected(name: ComponentName?)
        {
            stepCountService = null
        }

    }

    private fun GetStepCount(): Int
    {
        if (stepCountService != null)
            return stepCountService!!.GetNumber()
        else
            return 0
    }

    override fun onStop()
    {
        super.onStop()
        UnityPlayer.currentActivity.unbindService(connection)
    }
}