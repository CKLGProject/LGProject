using Cysharp.Threading.Tasks;
using R3;
using System;
using UnityEngine;

[RequireComponent(typeof(PatCaptureView))]
[RequireComponent(typeof(PatCaptureModel))]
public class PatCapturePresenter : MonoBehaviour
{
    private PatCaptureView _view;
    private PatCaptureModel _model;

    private void Start()
    {
        _view = GetComponent<PatCaptureView>();
        _model = GetComponent<PatCaptureModel>();

        Observable.Timer(TimeSpan.FromSeconds(3))
            .Subscribe(_ =>
            {
                _view.SetActiveInformationMessageText(false);
                _view.SetInteractiveCaptureStateUI(true);
            });
    }
}