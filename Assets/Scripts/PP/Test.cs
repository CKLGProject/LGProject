using NKStudio;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
public class Test : MonoBehaviour
{
    [Tooltip("생성 시킬 오브젝트")] public GameObject SpawnObject;

    [SerializeField] private Camera arCamera;

    [RequireInterface(typeof(IARInteractor))] [SerializeField]
    private Object arInteractorObject;

    private IARInteractor _arInteractor;
    private XRBaseControllerInteractor _arInteractorAsControllerInteractor;
    private bool _everHadSelection;

    private void Start()
    {
        arCamera = Camera.main;

        // 오브젝트를 인터렉터로 변환합니다.
        _arInteractor = arInteractorObject as IARInteractor;
        
        // 오브젝트를 컨트롤러 인터렉터로 변환합니다.
        _arInteractorAsControllerInteractor = arInteractorObject as XRBaseControllerInteractor;
    }
    
    private void Update()
    {
        // 생성을 시도하는 bool 변수입니다.
        var attemptSpawn = false;

        if (GetTouchDown())
            _everHadSelection = _arInteractorAsControllerInteractor.hasSelection;
        else if (GetTouching())
            _everHadSelection |= _arInteractorAsControllerInteractor.hasSelection;
        else if (GetTouchUp())
            attemptSpawn = !_arInteractorAsControllerInteractor.hasSelection && !_everHadSelection;

        // 터치를 땟을 때 오브젝트를 생성합니다.
        if (attemptSpawn && _arInteractor.TryGetCurrentARRaycastHit(out ARRaycastHit arRaycastHit))
        {
            var arPlane = arRaycastHit.trackable as ARPlane;
            if (arPlane == null)
                return;

            TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
        }
    }

    /// <summary>
    /// 주어진 위치에서 객체를 생성하려고 시도합니다.
    /// </summary>
    /// <param name="spawnPoint">객체를 생성할 월드 공간 위치입니다.</param>
    /// <param name="spawnNormal">스폰 표면의 월드 공간 정규 벡터입니다.</param>
    /// <returns>스포너가 성공적으로 객체를 생성하면 <see langword="true"/>를 반환합니다. 그렇지 않으면, 예를 들어 스폰 포인트가 카메라의 시야에서 벗어난 경우 <see langword="false"/>를 반환합니다.</returns>
    /// <seealso cref="SpawnObject"/>
    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        // 화면을 터치한 상태에서 카메라를 회전하고 생성을 시도할 수 있기 때문에,
        // 카메라의 시야에 생성할 수 있는지 확인합니다.
        var pointInViewportSpace = arCamera.WorldToViewportPoint(spawnPoint);
        if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > 1 ||
            pointInViewportSpace.x < 0 ||
            pointInViewportSpace.y > 1 || pointInViewportSpace.y < 0)
        {
            return false;
        }

        // 오브젝트를 생성합니다.
        var newObject = Instantiate(SpawnObject);
        newObject.transform.position = spawnPoint;

        // 땅 표면에 알맞게 회전합니다.
        var facePosition = arCamera.transform.position;
        var forward = facePosition - spawnPoint;
        BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
        newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        return true;
    }

    /// <summary>
    /// 해당 프레임에 터치 입력이 있었다면,
    /// </summary>
    /// <returns>터치 입력시 true를 반환합니다.</returns>
    private bool GetTouchDown()
    {
        XRControllerState currentControllerState =
            _arInteractorAsControllerInteractor.xrController.currentControllerState;
        return currentControllerState.selectInteractionState.activatedThisFrame;
    }

    /// <summary>
    /// 지속적으로 터치 입력을 받고 있다면,
    /// </summary>
    /// <returns>지속적인 입력이 있을 시 true를 반환합니다.</returns>
    private bool GetTouching()
    {
        XRControllerState currentControllerState =
            _arInteractorAsControllerInteractor.xrController.currentControllerState;
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
}