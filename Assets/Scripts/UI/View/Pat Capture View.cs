using NKStudio;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using Object = UnityEngine.Object;

public class PatCaptureView : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject informationMessageText;

    [SerializeField, RequireInterface(typeof(IARInteractor))]
    private Object arInteractorObject;

    public GameObject SpawnObject;

    private IARInteractor _arInteractor;
    private XRBaseControllerInteractor _arInteractorAsControllerInteractor;
    private bool _everHadSelection;

    /// <summary>
    /// Info Message 텍스트의 활성화 여부를 설정합니다.
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveInformationMessageText(bool isActive)
    {
        informationMessageText.SetActive(isActive);
    }

    private void Start()
    {
        arCamera = Camera.main;

        _arInteractor = arInteractorObject as IARInteractor;
        _arInteractorAsControllerInteractor = arInteractorObject as XRBaseControllerInteractor;
    }

    /// <summary>
    /// 해당 프레임에 터치 입력이 있었다면,
    /// </summary>
    /// <returns>터치 입력시 true를 반환합니다.</returns>
    private bool GetTouchDown()
    {
        XRControllerState currentControllerState = _arInteractorAsControllerInteractor.xrController.currentControllerState;
        return currentControllerState.selectInteractionState.activatedThisFrame;   
    }
    
    /// <summary>
    /// 지속적으로 터치 입력을 받고 있다면,
    /// </summary>
    /// <returns>지속적인 입력이 있을 시 true를 반환합니다.</returns>
    private bool GetTouching()
    {
        XRControllerState currentControllerState = _arInteractorAsControllerInteractor.xrController.currentControllerState;
        return currentControllerState.selectInteractionState.active;   
    }
    
    /// <summary>
    /// 해당 프레임에서 터치 입력을 떼었다면,
    /// </summary>
    /// <returns>터치 입력이 끊길시 false를 반환합니다.</returns>
    private bool GetTouchUp()
    {
        XRControllerState currentControllerState = _arInteractorAsControllerInteractor.xrController.currentControllerState;
        return currentControllerState.selectInteractionState.deactivatedThisFrame;   
    }

    private void Update()
    {
        var attemptSpawn = false;
        
        if (GetTouchDown())
            _everHadSelection = _arInteractorAsControllerInteractor.hasSelection;
        else if (GetTouching())
            _everHadSelection |= _arInteractorAsControllerInteractor.hasSelection;
        else if (GetTouchUp())
            attemptSpawn = !_arInteractorAsControllerInteractor.hasSelection && !_everHadSelection;

        if (attemptSpawn && _arInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
        {
            var arPlane = arRaycastHit.trackable as ARPlane;
            if (arPlane == null)
                return;

            TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
        }
    }

    /// <summary>
    /// 주어진 위치에서 <see cref="objectPrefabs"/>에서 객체를 생성하려고 시도합니다. 객체는 <see cref="cameraToFace"/>를 향하는 yaw 회전을 가지며, <see cref="spawnAngleRange"/> 내에서 무작위 각도를 더하거나 뺄 수 있습니다.
    /// </summary>
    /// <param name="spawnPoint">객체를 생성할 월드 공간 위치입니다.</param>
    /// <param name="spawnNormal">스폰 표면의 월드 공간 정규 벡터입니다.</param>
    /// <returns>스포너가 성공적으로 객체를 생성하면 <see langword="true"/>를 반환합니다. 그렇지 않으면, 예를 들어 스폰 포인트가 카메라의 시야에서 벗어난 경우 <see langword="false"/>를 반환합니다.</returns>
    /// <remarks>
    /// 생성할 객체를 선택하는 것은 <see cref="spawnOptionIndex"/>에 기반합니다. 인덱스가 <see cref="objectPrefabs"/>의 범위를 벗어나면, 이 메서드는 목록에서 무작위 프리팹을 선택하여 생성합니다.
    /// 그렇지 않으면, 인덱스에 있는 프리팹을 생성합니다.
    /// </remarks>
    /// <seealso cref="objectSpawned"/>
    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        var pointInViewportSpace = arCamera.WorldToViewportPoint(spawnPoint);
        if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > 1 ||
            pointInViewportSpace.x < 0 ||
            pointInViewportSpace.y > 1 || pointInViewportSpace.y < 0)
        {
            return false;
        }

        var newObject = Instantiate(SpawnObject);
        newObject.transform.position = spawnPoint;

        var facePosition = arCamera.transform.position;
        var forward = facePosition - spawnPoint;
        BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
        newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        return true;
    }
}