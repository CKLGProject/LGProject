using R3;
using R3.Triggers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace ReactiveCountdown
{
    public static class ObservableCountExtensions
    {
        public static Observable<int> CountdownAsObservable(this Component component, int count)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<int>();
            return GetOrAddComponent<ObservableCountdown>(component.gameObject)
                .OnCountDownAsObservable(count);
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

    public class ObservableCountdown : ObservableTriggerBase
    {
        private int _count;
        private readonly Subject<int> _countObject = new();

        private readonly WaitForSeconds _oneSeconds = new(1);

        public Subject<int> OnCountDownAsObservable(int targetCount)
        {
            _count = targetCount;
            StartCoroutine(Countdown());
            return _countObject;
        }

        private IEnumerator Countdown()
        {
            while (_count > 0)
            {
                _countObject.OnNext(_count);
                _count -= 1;
                yield return _oneSeconds;
            }

            _countObject.OnNext(0);
            _countObject.OnCompleted();
        }


        protected override void RaiseOnCompletedOnDestroy()
        {
            _countObject?.OnCompleted();
        }
    }
}