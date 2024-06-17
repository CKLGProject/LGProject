using UnityEngine;
using Utility;

/// <summary>
/// PC에서 토스트 메세지 비스무리하게 표현하기 위해 사용
/// </summary>
public class PCToastGenerate : MonoBehaviour
{
    public Canvas ToastCanvas;
    public Toast ToastObject;

    private void Start()
    {
        bool isMobile = LGUtility.IsMobile();
        
        if (isMobile) 
            ToastCanvas.gameObject.SetActive(false);
        else
            ToastCanvas.gameObject.SetActive(true);
    }

    /// <summary>
    /// 토스트 메세지를 생성합니다.
    /// </summary>
    /// <param name="message">메세지</param>
    /// <param name="toastType">시간 간격</param>
    public void ShowToastMessage(string message, LGUtility.EToast toastType = LGUtility.EToast.LENGTH_SHORT)
    {
        Toast toast = Instantiate(ToastObject, ToastCanvas.transform);
        toast.Message = message;
        toast.ToastType = toastType;
    }
}
