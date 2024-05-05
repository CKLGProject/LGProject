using NKStudio;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TrackingState = UnityEngine.XR.ARSubsystems.TrackingState;

public class PatCaptureView : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject informationMessageText;
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private Button captureButton;
    [SerializeField] private UDictionary<string, GameObject> SpawnObjects;
    
    private ReactiveProperty<GameObject> _targetObject = new ReactiveProperty<GameObject>();
    
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
                _targetObject.Value = SpawnObjects[trackedImage.referenceImage.name];
                return;
            }
        }

        // 아무것도 트래킹 되지 않고 있다면 모두 초기화
        if (_targetObject.Value != null)
        {
            foreach (KeyValuePair<string,GameObject> spawnObject in SpawnObjects) 
                spawnObject.Value.SetActive(false);
            
            _targetObject.Value = null;
        }
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
        _targetObject.Value.SetActive(true);
    }

}