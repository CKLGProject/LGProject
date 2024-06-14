// #define LG_DEBUG
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Singleton;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TrackingState = UnityEngine.XR.ARSubsystems.TrackingState;

public class PatCaptureView : MonoBehaviour
{
    [Header("AR")] [SerializeField] private Camera arCamera;
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;

    [Header("Gesture Controller")]
    [SerializeField] private PinchController pinchController;
    [SerializeField] private SwipeController swipeController;
    [SerializeField] private DoubleTapController doubleTapController;
    
    [Header("UI")] [SerializeField] private CanvasGroup UIGroup;
    [SerializeField] private Button captureButton;
    [SerializeField] private GameObject informationMessageText;
    [SerializeField] private CanvasGroup guideMessageGroup;
    [SerializeField] private TextMeshProUGUI guideMessageText;
    [SerializeField] private CanvasGroup informationMessageGroup;
    [SerializeField] private float informationMessageFadeTime = 3f;
    [SerializeField] private Button resetButton;

    [Header("Timeline")] [SerializeField] private PlayableDirector fxDirector;
    [SerializeField] private LightController lightController;

    [SerializeField] private List<ScanData> scanDataList;
    [SerializeField] private Transform objectContent;
    [SerializeField] private ObjectRotation objectRotation;

    private RenderTexture _renderTexture;
    private ReactiveProperty<ScanData> _targetObject = new ReactiveProperty<ScanData>();

    private Subject<Unit> _onCharacterize = new Subject<Unit>();
    private static readonly int Open = Animator.StringToHash("Open");

    private void OnEnable()
    {
#if LG_DEBUG
        ScanData sampleObject = scanDataList[0];
        _targetObject.Value = sampleObject;
        captureButton.interactable = true;
        return;
#endif

        _arTrackedImageManager.trackedImagesChanged += OnTrackedImage;
    }

    private void Start()
    {
        guideMessageGroup.alpha = 0;
        informationMessageGroup.alpha = 0;
        SetActiveResetButton(false);

        swipeController.OnSwipeObservable
            .Where(_=> objectRotation.Active == false)
            .Where(_ => CompareTargetGesture(GestureType.ScrollDown))
            .Subscribe(_ => _onCharacterize.OnNext(Unit.Default))
            .AddTo(this);

        doubleTapController.OnDoubleTouchObservable()
            .Where(_=> objectRotation.Active == false)
            .Where(_ => CompareTargetGesture(GestureType.DoubleTap))
            .Subscribe(_ => _onCharacterize.OnNext(Unit.Default))
            .AddTo(this);

        pinchController.OnPinchOutObservable
            .Where(_=> objectRotation.Active == false)
            .Where(_ => CompareTargetGesture(GestureType.Pinch))
            .Subscribe(_ => _onCharacterize.OnNext(Unit.Default))
            .AddTo(this);
    }

    private void OnDisable()
    {
#if LG_DEBUG
        return;
#endif

        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImage;
    }

    private void OnTrackedImage(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage trackedImage in obj.updated)
        {
            // 무언가라도 트래킹되고 있다면,
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ScanData targetObject = scanDataList
                    .FirstOrDefault(scanData => trackedImage.referenceImage.name == scanData.ObjectName);

                _targetObject.Value = targetObject;
                return;
            }
        }

        // 아무것도 트래킹 되지 않고 있다면 모두 초기화
        if (_targetObject.Value != null)
            _targetObject.Value = null;
    }

    /// <summary>
    /// Info Message 텍스트의 활성화 여부를 설정합니다.
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveInformationMessageText(bool isActive)
    {
        informationMessageText.SetActive(isActive);
    }

    /// <summary>
    /// 캡쳐 버튼의 상호작용 여부를 설정합니다.
    /// </summary>
    /// <param name="isInteractive"></param>
    public void SetInteractiveCaptureStateUI(bool isInteractive)
    {
        captureButton.interactable = isInteractive;
    }

    /// <summary>
    /// 타겟을 활성화 합니다.
    /// </summary>
    public void ActiveTargetObject()
    {
        // 캡처 버튼을 더 이상 누를 수 없겠금 방지
        UIGroup.blocksRaycasts = false;

        // AR 이미지 타겟팅 기능 비활성화
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImage;

        //UI Fade Out
        DOTween.To(() => UIGroup.alpha, x => UIGroup.alpha = x, 0, 1f).SetEase(Ease.OutSine).SetDelay(0.2f);

        // Info Message Fade In & Out
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => informationMessageGroup.alpha, x => informationMessageGroup.alpha = x, 1, 1f)
            .SetEase(Ease.InSine));
        sequence.Append(DOTween.To(() => informationMessageGroup.alpha, x => informationMessageGroup.alpha = x, 0, 1f)
            .SetEase(Ease.OutSine).SetDelay(informationMessageFadeTime));
        sequence.Play();

        // 리셋 버튼 활성화
        ShowResetButton().Forget();

        // 오브젝트 회전 활성화
        objectRotation.Active = true;

        // 펫을 추가합니다.
        Singleton.Instance<GameManager>().AddPet(_targetObject.Value.RewardPet);
        
        // 타겟팅된 가전제품 활성화
        _targetObject.Value.MachineObject.SetActive(true);
    }
    
    /// <summary>
    /// 오브젝트 회전을 비활성화 합니다.
    /// </summary>
    public void DisableObjectRotation()
    {
        objectRotation.Active = false;
    }

    /// <summary>
    /// 3초 딜레이 후 리셋 버튼을 보이게 합니다.
    /// </summary>
    private async UniTaskVoid ShowResetButton()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: destroyCancellationToken);
        SetActiveResetButton(true);
    }

    /// <summary>
    /// 캐릭터 라이즈화 하는 시퀀스를 재생합니다.
    /// </summary>
    public async UniTaskVoid PlayCharacterizeSequence()
    {
        if(_targetObject.Value.MachineObject.TryGetComponent(out Animator targetAnimator)) 
            targetAnimator.SetTrigger(Open);
        
        const float delayTime = 0.7f;
        
        // 일단 delayTime 정도.. 대기
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        
        fxDirector.Play();
    }

    /// <summary>
    /// 터치 가이드 텍스트를 보이게 합니다.
    /// </summary>
    public void ShowTouchGuideText()
    {
        SetActiveGuideMessageText(true);
        
        string message = _targetObject.Value.GuideMessage;
        guideMessageText.text = message;
        guideMessageText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 머신을 밝게 빛나게 만듭니다.
    /// </summary>
    /// <param name="duration"></param>
    public void PlayMachineIllumination(float duration)
    {
        const float targetIntensity = 7f;
        DOTween.To(x => lightController.LightIntensity = x, 0, targetIntensity, duration).SetEase(Ease.InOutSine);
        DOTween.To(x => lightController.LightAlpha = x, 0, 1, duration).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// 카메라 기능을 끕니다.
    /// </summary>
    public void OffCamera()
    {
        arSession.enabled = false;
    }

    /// <summary>
    /// 캐릭터를 보이게 합니다.
    /// </summary>
    public void ShowTargetCharacter()
    {
        _targetObject.Value.MachineObject.SetActive(false);
        _targetObject.Value.CharacterObject.SetActive(true);
    }

    /// <summary>
    /// 회전을 원래대로 되돌립니다.
    /// </summary>
    public void ResetTransformObjects()
    {
        objectContent.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 버튼을 클릭했을 때의 이벤트를 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> OnClickCaptureButtonAsObservable()
    {
        return captureButton.OnClickAsObservable();
    }

    /// <summary>
    /// 트래킹되고 있는 오브젝트가 존재하는지 확인합니다.
    /// </summary>
    /// <returns>존재한다면 true, 아닐시 false를 반환합니다.</returns>
    public Observable<bool> ExistsTargetObjectAsObservable()
    {
        return _targetObject.Select(targetObject => targetObject != null);
    }

    public Observable<Unit> OnCharacterizeAsObservable()
    {
        return _onCharacterize;
    }

    public Observable<Unit> OnResetButtonClickObservable()
    {
        return resetButton.OnClickAsObservable();
    }

    /// <summary>
    /// 타겟 오브젝트가 해당 제스처와 동일한지 체크합니다.
    /// </summary>
    /// <param name="gestureType">체크할 제스처</param>
    /// <returns>true or false</returns>
    private bool CompareTargetGesture(GestureType gestureType)
    {
        if (_targetObject.Value == null)
            return false;

        if (_targetObject.Value.GestureType == gestureType)
            return true;

        return false;
    }

    /// <summary>
    /// 제스처 UI를 보이게 합니다.
    /// </summary>
    public void SetActiveGestureUI(bool isActive)
    {
        _targetObject.Value.GestureUI.SetActive(isActive);
    }
    
    /// <summary>
    /// 리셋 버튼의 활성화 여부를 설정합니다.
    /// </summary>
    public void SetActiveResetButton(bool isActive)
    {
        resetButton.gameObject.SetActive(isActive);
    }
    
    /// <summary>
    /// 해당 가전제품을 활성화 하는 방법의 텍스트의 활성화 여부를 설정합니다.
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGuideMessageText(bool isActive)
    {
        guideMessageGroup.alpha = isActive ? 1 : 0;
        guideMessageText.gameObject.SetActive(isActive);
    }
}