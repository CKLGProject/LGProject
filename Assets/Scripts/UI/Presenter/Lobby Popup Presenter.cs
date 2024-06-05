using R3;
using UnityEngine;

[RequireComponent(typeof(LobbyPopupModel))]
[RequireComponent(typeof(LobbyPopupView))]
public class LobbyPopupPresenter : MonoBehaviour
{
    private LobbyPopupModel _model;
    private LobbyPopupView _view;


    private void Start()
    {
        _model = GetComponent<LobbyPopupModel>();
        _view = GetComponent<LobbyPopupView>();

        _view.OnCharacterSelectionButtonClicked()
            .Subscribe(_ =>
            {
                
            })
            .AddTo(this);
    }
}