using System;
using USingleton;
using UnityEngine;
using LGProjects.Android.Utility;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class WaitScene : MonoBehaviour
{
    public UIDocument WaitDocument;
    public VisualElement UI;

    private Texture2D QRTexture2D;

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
        UI = WaitDocument.rootVisualElement.Q<VisualElement>("QRTexture");

        QRTexture2D = new Texture2D(256, 256);

        if (Singleton.HasInstance<RelayManager>())
        {
            string joinCode = Singleton.Instance<RelayManager>().JoinCode;
            Debug.Log(joinCode);
            GenerateJoinQR(joinCode);
        }

        Back.started += OnBack;
    }
    private void OnBack(InputAction.CallbackContext obj)
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        if (Singleton.HasInstance<RelayManager>())
            Singleton.Instance<RelayManager>().Reset();

        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// QR를 생성합니다.
    /// </summary>
    /// <param name="message">QR로 만들 내용</param>
    private void GenerateJoinQR(string message)
    {
        Color32[] convert = QRManager.ConvertQR(message, QRTexture2D.width, QRTexture2D.height);

        QRTexture2D.SetPixels32(convert);
        QRTexture2D.Apply();

        UI.style.backgroundImage = new StyleBackground(QRTexture2D);
    }
}
