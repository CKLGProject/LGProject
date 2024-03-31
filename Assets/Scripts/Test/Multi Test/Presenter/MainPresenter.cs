using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using USingleton;
using View;

namespace Presenter
{
    [RequireComponent(typeof(MainView))]
    public class MainPresenter : NetworkBehaviour
    {
        private MainView _mainView;
        private async void Start()
        {
            _mainView = GetComponent<MainView>();

            _mainView.ConnectButton.clicked += () => {
                _mainView.ClientConnectStart();
            };

            _mainView.ServerStartButton.clicked += () => {
                _mainView.HostStart().Forget();
            };

            // 초기에 안보이도록 처리합니다. 
            _mainView.SetActiveButton(false);

            await UniTask.WaitUntil(() => AuthenticationService.Instance.IsSignedIn);
            
            // 로그인이 완료되면 보이도록 처리 
            _mainView.SetActiveButton(true);
        }


    }
}
