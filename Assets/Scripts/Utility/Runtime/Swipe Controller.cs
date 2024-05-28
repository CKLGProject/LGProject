using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeController : MonoBehaviour
{
    public enum EHorizontalFilterType
    {
        None,
        Left,
        Right
    }

    public enum EVerticalFilterType
    {
        None,
        Up,
        Down
    }

    // Enum 변수를 선언
    public EHorizontalFilterType HorizontalFilterType;
    public EVerticalFilterType VerticalFilterType;

    public float SwipeHorizontalSensitivity = 150;
    public float SwipeVerticalSensitivity = 150;

    /// <summary>
    /// 스와이프 했을 때 호출되는 옵저버
    /// </summary>
    public Observable<Vector2> OnSwipeObservable => _swipeObject.AsObservable();

    private Subject<Vector2> _swipeObject;

    private InputAction _touchPrimaryTouchDelta;

    private void Awake()
    {
        // Init 
        _swipeObject = new Subject<Vector2>();

        // Pinch gesture
        _touchPrimaryTouchDelta = 
            new InputAction(type: InputActionType.Value, binding: "<Touchscreen>/primaryTouch/delta");
    }

    private void OnEnable()
    {
        _touchPrimaryTouchDelta.Enable();
    }

    private void OnDisable()
    {
        _touchPrimaryTouchDelta.Disable();
    }

    private void Start()
    {
        _touchPrimaryTouchDelta.performed += OnTouchToSwipe;
    }

    private void OnTouchToSwipe(InputAction.CallbackContext obj)
    {
        // 핀치 제스처가 아닌 경우
        Vector2 delta = _touchPrimaryTouchDelta.ReadValue<Vector2>();
        OnSwipe(delta);
    }

    private void OnSwipe(Vector2 swipeDelta)
    {
        if (swipeDelta == Vector2.zero)
            return;

        bool horizontalPass = false;
        bool verticalPass = false;

        // 수평 필터 검사
        switch (HorizontalFilterType)
        {
            case EHorizontalFilterType.None:
                horizontalPass = true;
                break;
            case EHorizontalFilterType.Left:
                horizontalPass = swipeDelta.x < -SwipeHorizontalSensitivity;
                break;
            case EHorizontalFilterType.Right:
                horizontalPass = swipeDelta.x > SwipeHorizontalSensitivity;
                break;
        }

        // 수직 필터 검사
        switch (VerticalFilterType)
        {
            case EVerticalFilterType.None:
                verticalPass = true;
                break;
            case EVerticalFilterType.Up:
                verticalPass = swipeDelta.y > SwipeVerticalSensitivity;
                break;
            case EVerticalFilterType.Down:
                verticalPass = swipeDelta.y < -SwipeVerticalSensitivity;
                break;
        }

        // 두 필터 조건이 모두 만족되면 _swipeObject에 값 전달
        if (horizontalPass && verticalPass) 
            _swipeObject.OnNext(swipeDelta);
    }
}
