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
            .Subscribe(_ => _view.SetActiveInformationMessageText(false));

        // Model
        _model.CanQRCodeCaptureAsObservable
            .Subscribe(canCapture => _view.SetInteractiveCaptureStateUI(canCapture));

        // View
        // 현재 트래킹되고 있는 오브젝트가 있는지 파악하는 옵저버
        _view.ExistsTargetObjectAsObservable()
            .Subscribe(exists => _model.CanQRCodeCapture = exists);

        // 캡쳐 버튼을 클릭했을 때 해당 타겟 매쉬를 활성화하는 옵저버
        _view.OnClickCaptureButtonAsObservable()
            .Where(_ => _model.CanQRCodeCapture)
            .Subscribe(_ =>
            {
                _model.CanBeCharacterized = true;
                _view.ActiveTargetObject();
            });

        // 캐릭터 라이즈가 가능할 때 더블 터치를 하는 옵저버
        _view.OnDoubleTouchObservable()
            .Where(_ => _model.CanBeCharacterized)
            .Subscribe(_ =>
            {
                Debug.Log("더블 터치");
            });
    }
}