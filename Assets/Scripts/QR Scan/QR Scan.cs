using UnityEngine;
using UnityEngine.InputSystem;
using LGProjects.Android.Utility;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using SceneReference = UnityEngine.SceneReference;
using USingleton;

public class QRScan : MonoBehaviour
{
    public QRManager QRManager;
    public ARSession Session;
    public SceneReference PreviousScene;

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
        bool isJoin = await Singleton.Instance<RelayManager>().OnJoinRelay(result);

        if (isJoin)
            NetworkManager.Singleton.StartClient();
    }

    private void OnBack(InputAction.CallbackContext obj)
    {
        // 네트워크 매니저 제거
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);

        // 세션 초기화 및 XR 초기화
        Session.Reset();
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        
        // 이전 씬으로 이동
        SceneManager.LoadScene(PreviousScene.Path);
    }
}
