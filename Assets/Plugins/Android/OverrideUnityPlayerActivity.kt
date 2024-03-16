package com.unity3d.player

import android.content.Intent
import android.widget.Toast


class OverrideUnityPlayerActivity : UnityPlayerActivity()
{
    private fun Toast(message: String, duration: Int) {
        UnityPlayer.currentActivity.runOnUiThread {
            Toast.makeText(UnityPlayer.currentActivity, message, duration).show()
        }
    }
}