using LGProjects.Android.Utility;
using UnityEngine;

public class SampleCode02 : UnityEngine.MonoBehaviour
{
    public QRManager qrManager;

    private void Start()
    {
        if (qrManager == null)
            qrManager = FindAnyObjectByType<QRManager>();

        qrManager.ScanFinishResult += OnScanFinish;
    }

    private void OnScanFinish(string result)
    {
        AndroidUtility.Toast(result);
    }
}
