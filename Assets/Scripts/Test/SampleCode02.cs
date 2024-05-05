using LGProjects.Android.Utility;
using UnityEngine;
using Utility;

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
        LGUtility.Toast(result);
    }
}
