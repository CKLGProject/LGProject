using R3;
using R3.Triggers;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace ReactiveTouchDown
{
    public static class ObservableTriggerExtensions
    {
        public static Observable<Unit> TouchDownAsObservable(this GameObject component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableTouchDownTrigger>(component.gameObject)
                .OnTouchDownAsObservable(component);
        }

        public static Observable<Unit> DoubleTouchDownAsObservable(this GameObject component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<Unit>();
            return GetOrAddComponent<ObservableDoubleTouchDownTrigger>(component.gameObject)
                .OnTouchDownAsObservable(component);
        }

        private static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();

            if (component == null)
                component = gameObject.AddComponent<T>();

            return component;
        }
    }

    public class ObservableTouchDownTrigger : ObservableTriggerBase
    {
        private int _instance;
        private Subject<Unit> _onTouchDown;
        private Camera _camera;
        private InputAction _inputAction;

        private void OnEnable()
        {
            _inputAction.Enable();
        }

        private void OnDisable()
        {
            _inputAction.Disable();
        }

        private void Awake()
        {
            _camera = Camera.main;
            _inputAction = InputSystem.actions.FindActionMap("System").FindAction("Tap");
            _inputAction.performed += OnTouch;
        }

        private void OnTouch(InputAction.CallbackContext obj)
        {
            if(_camera != null)
            {
                Vector2 clickPosition = Pointer.current.position.ReadValue();
                Ray ray = _camera.ScreenPointToRay(clickPosition);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject.GetInstanceID() == _instance)
                        _onTouchDown?.OnNext(Unit.Default);
                }
            }
        }

        public Subject<Unit> OnTouchDownAsObservable(GameObject component)
        {
            _instance = component.gameObject.GetInstanceID();
            return _onTouchDown ??= new Subject<Unit>();
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onTouchDown?.OnCompleted();
        }
    }

    public class ObservableDoubleTouchDownTrigger : ObservableTriggerBase
    {
        private int _instance;
        private Subject<Unit> _onTouchDown;
        private Camera _camera;
        private Vector2 _touchPosition;

        // private void OnEnable()
        // {
        //     _inputAction.Enable();
        // }
        //
        // private void OnDisable()
        // {
        //     _inputAction.Disable();
        // }

        private void Awake()
        {
            _camera = Camera.main;
            InputSystem.actions.FindActionMap("System").FindAction("Double Tap").performed += OnDoubleTouch;;
            InputSystem.actions.FindActionMap("System").FindAction("Position").performed += OnPosition;
        }

        private void OnPosition(InputAction.CallbackContext obj)
        {
            _touchPosition = obj.ReadValue<Vector2>();
        }

        private void OnDoubleTouch(InputAction.CallbackContext obj)
        {
            Ray ray = _camera.ScreenPointToRay(_touchPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.GetInstanceID() == _instance)
                    _onTouchDown?.OnNext(Unit.Default);
            }
        }

        public Subject<Unit> OnTouchDownAsObservable(GameObject component)
        {
            _instance = component.gameObject.GetInstanceID();
            return _onTouchDown ??= new Subject<Unit>();
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onTouchDown?.OnCompleted();
        }
    }
}