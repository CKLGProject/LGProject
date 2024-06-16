using Data;
using R3;
using System;
using UnityEngine;
using UnityEngine.Singleton;

[RequireComponent(typeof(LobbyPopupModel))]
[RequireComponent(typeof(LobbyPopupView))]
public class LobbyPopupPresenter : MonoBehaviour
{
    private LobbyView _lobbyView;
    private LobbyPopupModel _model;
    private LobbyPopupView _view;

    private GameManager GameManager => Singleton.Instance<GameManager>();

    private void Start()
    {
        _model = GetComponent<LobbyPopupModel>();
        _view = GetComponent<LobbyPopupView>();
        _lobbyView = FindAnyObjectByType<LobbyView>();

        // 전부 비활성화
        _view.AllNoneSelected();

        // 현재 캐릭터 활성화
        ECharacterType currentCharacter = _model.SelectedCharacterType;
        _view.ActiveCurrentCharacterProfile(currentCharacter);

        _view.ActivePet(currentCharacter);

        // 현재 클릭한 캐릭터 타입을 변경
        _model.SelectedCharacterTypeObservable()
            .Subscribe(selectionCharacter =>
            {
                // 버튼 활성화 상태
                ECharacterType currentCharacterProfile = GameManager.GetCharacter(ActorType.User);
                bool buttonActive = currentCharacterProfile != selectionCharacter;
                _view.SetInteractionByCharacterSelectionButton(buttonActive);

                _view.AllNoneSelected();
                _view.ActiveCurrentCharacterProfile(selectionCharacter);
                _view.SetActiveCharacterImage(selectionCharacter);
                _view.ActivePet(selectionCharacter);

                switch (selectionCharacter)
                {
                    case ECharacterType.Hit:
                        _view.SetCharacterData(_model.HitData);
                        break;
                    case ECharacterType.Frost:
                        _view.SetCharacterData(_model.FrostData);
                        break;
                    case ECharacterType.Kane:
                        _view.SetCharacterData(_model.KaneData);
                        break;
                }
            })
            .AddTo(this);

        // 캐릭터 프로필 버튼 클릭
        _view.HitProfileButtonClicked()
            .Subscribe(_ => _model.SetSelectedCharacterType(ECharacterType.Hit))
            .AddTo(this);

        _view.FrostProfileButtonClicked()
            .Subscribe(_ => _model.SetSelectedCharacterType(ECharacterType.Frost))
            .AddTo(this);

        _view.KaneProfileButtonClicked()
            .Subscribe(_ => _model.SetSelectedCharacterType(ECharacterType.Kane))
            .AddTo(this);

        _view.StomeProfileButtonClicked()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(2))
            .AddTo(this);

        _view.brightProfileButtonClicked()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(2))
            .AddTo(this);

        _view.OnItemChoiceButtonClicked()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(3))
            .AddTo(this);

        _view.OnItemChoiceButtonClicked()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(3))
            .AddTo(this);

        // 캐릭터 선택 버튼 클릭
        _view.OnCharacterSelectionButtonClicked()
            .Subscribe(_ =>
            {
                GameManager.SetCurrentCharacter(ActorType.User, _model.SelectedCharacterType);
                GameManager.UpdateUserPetData();
                
                _model.SetChoiceCharacterType(_model.SelectedCharacterType);
                _view.SetInteractionByCharacterSelectionButton(false);
            })
            .AddTo(this);

        _view.OnCloseButtonClicked()
            .Subscribe(_ => _view.SetActivePopupView(false))
            .AddTo(this);

        _view.OnPopupViewActiveObservable()
            .Subscribe(active => _model.SetActive(active))
            .AddTo(this);

        _view.OnPetChoiceButtonClicked()
            .Subscribe(_ =>
            {
                ECharacterType selectionCharacter = _model.SelectedCharacterType;

                switch (selectionCharacter)
                {
                    case ECharacterType.Hit:

                        if (GameManager.HasPet(EPetType.Scorchwing))
                            GameManager.ActivePet(EPetType.Scorchwing);
                        else
                            _lobbyView.ShowErrorMessage(4);
                        
                        break;
                    case ECharacterType.Frost:

                        if (GameManager.HasPet(EPetType.Icebound))
                            GameManager.ActivePet(EPetType.Icebound);
                        else
                            _lobbyView.ShowErrorMessage(4);
                        
                        break;
                    case ECharacterType.Kane:

                        if (GameManager.HasPet(EPetType.Aerion))
                            GameManager.ActivePet(EPetType.Aerion);
                        else
                            _lobbyView.ShowErrorMessage(4);
                        
                        break;
                    case ECharacterType.Storm:

                        if (GameManager.HasPet(EPetType.Aerion))
                            GameManager.ActivePet(EPetType.Aerion);
                        else
                            _lobbyView.ShowErrorMessage(4);
                        
                        break;
                    case ECharacterType.E:
                    case ECharacterType.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                _view.ActivePet(selectionCharacter);
                
                if (GameManager.GetCharacter(ActorType.User) == selectionCharacter)
                {
                    GameManager.UpdateUserPetData();
                    _lobbyView.ActivePet(selectionCharacter);
                }
            })
            .AddTo(this);
    }
}