using Data;
using DG.Tweening;
using R3;
using ReactiveInputSystem;
using ReactiveTouchDown;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TrackingState = UnityEngine.XR.ARSubsystems.TrackingState;

public class PatCaptureView : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject informationMessageText;
    [SerializeField] private GameObject fxBackground;
    [SerializeField] private RawImage fxForeground;
    [SerializeField] private PlayableDirector fxDirector;
    [SerializeField] private GameObject TouchArea;
    [SerializeField] private CanvasGroup UIGroup;
    [SerializeField] private ObjectRotation objectRotation;

    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private Button captureButton;

    [SerializeField] private List<ScanData> ScanDataList;

    private ReactiveProperty<ScanData> _targetObject = new ReactiveProperty<ScanData>();

    private RenderTexture _renderTexture;

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
    /// 타겟을 활성화 합니다.
    /// </summary>
    public void ActiveTargetObject()
    {
        UIGroup.blocksRaycasts = false;
        DOTween.To(() => UIGroup.alpha, x => UIGroup.alpha = x, 0, 1f).SetEase(Ease.OutSine).SetDelay(0.5f);
        
        objectRotation.Active = true;
        _arTrackedImageManager.trackedImagesChanged -= OnTrackedImage;
        _targetObject.Value.MachineObject.SetActive(true);
    }

    /// <summary>
    /// 이미지 작아지는 연출 처리
    /// </summary>
    private void PlayFrontFX()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(fxForeground.rectTransform.DOScale(Vector3.zero, 2f).SetEase(Ease.InSine));
        sequence.Join(fxForeground.rectTransform.DORotate(new Vector3(0, 360, 0), 2f).SetEase(Ease.InSine)
            .SetRelative(true));
        sequence.Join(DOTween.ToAlpha(() => fxForeground.color, x => fxForeground.color = x, 0.5f, 2f)
            .SetEase(Ease.InSine));
        sequence.OnComplete(() =>
        {
            _targetObject.Value.CharacterObject.SetActive(true);
            fxDirector.Play();
        });
    }

    public void PlayScreenRotation()
    {


        _renderTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);
        Camera.main.targetTexture = _renderTexture;
        Camera.main.Render();
        RenderTexture.active = _renderTexture;

        Texture2D texture = new Texture2D(_renderTexture.width, _renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        texture.Apply();

        Camera.main.targetTexture = null;

        _targetObject.Value.MachineObject.SetActive(false);
        fxForeground.texture = texture;
        fxBackground.SetActive(true);
        fxForeground.gameObject.SetActive(true);

        PlayFrontFX();
    }

    public Observable<Unit> OnDoubleTouchObservable()
    {
        return TouchArea.DoubleTouchDownAsObservable();
    }
}