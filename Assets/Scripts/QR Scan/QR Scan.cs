using UnityEngine;
using UnityEngine.InputSystem;
using LGProjects.Android.Utility;
using MyBox;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class QRScan : MonoBehaviour
{
    public QRManager QRManager;
    public InputAction Back;

    private void OnEnable()
    {
        Back.Enable();
    }

    private void OnDisable()
    {
        Back.Disable();
    }

    private void Start()
    {
        if (QRManager == null)
            QRManager = FindAnyObjectByType<QRManager>();

        QRManager.ScanFinishResult += ScanFinishResult;
        Back.performed += OnBack;
    }
    private async void ScanFinishResult(string result)
    {
        bool isJoin =  await Singleton<RelayManager>.Instance.OnJoinRelay(result);

        if (isJoin)
            NetworkManager.Singleton.StartClient();
    }

    private void OnBack(InputAction.CallbackContext obj)
    {
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("Main");
    }
}
