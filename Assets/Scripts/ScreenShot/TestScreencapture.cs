using System;
using Cysharp.Threading.Tasks;
using DeadMosquito.AndroidGoodies;
using UnityEngine;

public class TestScreencapture : UnityEngine.MonoBehaviour
{
    public GameObject UI;
    
    /// <summary>
    /// 스크린샷을 실행합니다.
    /// </summary>
    public void ScreenCapture()
    {
        ScreenShotTask().Forget();
    }

    private async UniTaskVoid ScreenShotTask()
    {
        // UI를 전부 안보이게 처리
        UI.SetActive(false);
        
        //스크린샷 실행
        AGShare.ShareScreenshot();
        
        // 1초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        
        // UI를 다시 보이게 처리
        UI.SetActive(true);
    }
}
