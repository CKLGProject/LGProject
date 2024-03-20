package com.unity3d.player

import android.content.Intent
import android.os.Build
import android.widget.Toast

class UPlugin
{
    private fun Toast(message: String, duration: Int)
    {
        UnityPlayer.currentActivity.runOnUiThread {
            Toast.makeText(UnityPlayer.currentActivity, message, duration).show()
        }
    }

    private fun StartStepCountService(title: String, message: String, smallIcon: String, largeIcon: String)
    {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O)
        {
            val targetIntent = Intent(UnityPlayer.currentActivity, StepCountService::class.java)
            targetIntent.putExtra("Title", title)
            targetIntent.putExtra("Message", message)
            targetIntent.putExtra("SmallIcon", smallIcon)
            targetIntent.putExtra("LargeIcon", largeIcon)
            UnityPlayer.currentActivity.startService(targetIntent)
        }
    }
}