using Data;
using DG.Tweening;
using R3;
using ReactiveTouchDown;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TrackingState = UnityEngine.XR.ARSubsystems.TrackingState;

public class PatCaptureView : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    
    [Header("Interaction")]
    [SerializeField] private GameObject TouchArea;
    [SerializeField] private ObjectRotation objectRotation;
    
    [Header("UI")]
    [SerializeField] private CanvasGroup UIGroup;
    [SerializeField] private Button captureButton;
    [SerializeField] private GameObject informationMessageText;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector fxDirector;
    [SerializeField] private LightController lightController;
    
    [SerializeField] private List<ScanData> ScanDataList;
    [SerializeField] private Transform objectContent;

    private RenderTexture _renderTexture;
    private ReactiveProperty<ScanData> _targetObject = new ReactiveProperty<ScanData>();

    private void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnTrackedImage;
    }

    private void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImage;
    }

    private void OnTrackedImage(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage trackedImage in obj.updated)
        {
            // 무언가라도 트래킹되고 있다면,
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ScanData targetObject = ScanDataList
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

        //UI Fade Out
        DOTween.To(() => UIGroup.alpha, x => UIGroup.alpha = x, 0, 1f).SetEase(Ease.OutSine).SetDelay(0.2f);

        // 오브젝트 회전 활성화
        objectRotation.Active = true;

        // AR 이미지 타겟팅 기능 비활성화
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImage;

        // 타겟팅된 가전제품 활성화
        _targetObject.Value.MachineObject.SetActive(true); // Debug
    }

    /// <summary>
    /// 캐릭터 라이즈화 하는 시퀀스를 재생합니다.
    /// </summary>
    public void PlayCharacterizeSequence()
    {
        fxDirector.Play();
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
    /// 캐릭터를 보이게 합니다.
    /// </summary>
    public void ShowTargetCharacter()
    {
        if (Application.isEditor)
            return;
        
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

    /// <summary>
    /// 더블 터치를 인식하는 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> OnDoubleTouchObservable()
    {
        return TouchArea.DoubleTouchDownAsObservable();
    }
}