using R3;
using ReactiveInputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

[RequireComponent(typeof(InitStartView))]
public class InitStartPresenter : MonoBehaviour
{
    private InitStartView _view;
    private InputAction _inputAction;
    
    private void Start()
    {
        _view = GetComponent<InitStartView>();

        bool isMobile = LGUtility.IsMobile();
        _inputAction = InputSystem.actions.FindActionMap("System").FindAction(isMobile ? "Tap" : "AnyKey");
        
        // 텍스트 변경
        _view.ChangeTextByOS();
        
        // View
        _inputAction.PerformedAsObservable()
            .Subscribe(_ => _view.Connect());
    }
}