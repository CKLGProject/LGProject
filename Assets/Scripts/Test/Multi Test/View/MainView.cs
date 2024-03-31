using System;
using Cysharp.Threading.Tasks;
using Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using USingleton;

namespace View
{
    public class MainView : NetworkBehaviour
    {
        public UIDocument MainDocument;

        public Button ConnectButton;
        public Button ServerStartButton;

        public SceneReference QRScene;
        public SceneReference WaitingScene;

        private void Awake()
        {
            ConnectButton = MainDocument.rootVisualElement.Q<Button>("Connect-Button");
            ServerStartButton = MainDocument.rootVisualElement.Q<Button>("ServerConnect-Button");
        }
        public void ClientConnectStart()
        {
            SceneManager.LoadScene("QR Scan");
        }

        public async UniTaskVoid HostStart()
        {
            if (Singleton.Instance<RelayManager>().IsRelayEnabled)
            {
                // 로그인을 시도합니다. 
                bool isSignIn = await Singleton.Instance<RelayManager>().OnSignIn();
                if (isSignIn)
                {
                    ConnectButton.SetEnabled(false);
                    ServerStartButton.SetEnabled(false);
                    
                    // 셋업을 시전합니다.
                    bool isSetup = await Singleton.Instance<RelayManager>().OnSetupRelayServer();
                    if (isSetup)
                        await Singleton.Instance<RelayManager>().OnGetJoinCode();
                    else
                        return;
                    
                    if (!string.IsNullOrEmpty(Singleton.Instance<RelayManager>().JoinCode))
                    {
                        if (NetworkManager.Singleton.StartHost())
                            SceneManager.LoadScene(WaitingScene.Path, LoadSceneMode.Single);
                    }
                    else
                        Debug.LogWarning("Join Code가 이슈가 생겼습니다. : 001");
                }
                else
                {
                    Debug.LogWarning("로그인 실패 : 002");
                }
            }
        }
        
        /// <summary>
        /// 버튼의 활성화 상태를 관리합니다.
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveButton(bool active)
        {
            ConnectButton.SetEnabled(active);
            ServerStartButton.SetEnabled(active);
        }
    }
}
