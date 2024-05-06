using R3;
using R3.Triggers;
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

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (LGUtility.IsEditorGameView())
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Vector2 clickPosition = Mouse.current.position.ReadValue();
                    Ray ray = _camera.ScreenPointToRay(clickPosition);

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.collider.gameObject.GetInstanceID() == _instance) 
                            _onTouchDown?.OnNext(Unit.Default);
                    }
                }
            }
            else
            {
                if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                {
                    Vector2 clickPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                    Ray ray = _camera.ScreenPointToRay(clickPosition);

                    if (Physics.Raycast(ray, out RaycastHit hit)) 
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
}