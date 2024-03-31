using LGProjects.Android.Utility;
using UnityEngine;

public class SampleCode02 : MonoBehaviour
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
