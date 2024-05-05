using System;
using UnityEngine;
using USingleton.AutoSingleton;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    public string Nickname { get; set; }

    private void Start()
    {
        // 화면이 꺼지지 않도록 처리
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
