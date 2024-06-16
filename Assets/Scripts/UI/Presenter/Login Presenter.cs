using R3;
using ReactiveInputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InitStartView))]
public class InitStartPresenter : MonoBehaviour
{
    private InitStartView _view;
    private InputAction _inputAction;
    
    private void Start()
    {
        _view = GetComponent<InitStartView>();
        _inputAction = InputSystem.actions.FindActionMap("System").FindAction("Tap");
        
        // View
        _inputAction.PerformedAsObservable()
            .Subscribe(_ => _view.Connect());
    }
}