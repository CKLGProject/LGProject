using R3;
using ReactiveTouchDown;
using UnityEngine;

public class DoubleTapController : MonoBehaviour
{
    [SerializeField] private GameObject touchArea;
    
    /// <summary>
    /// 더블 터치를 인식하는 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> OnDoubleTouchObservable()
    {
        return touchArea.DoubleTouchDownAsObservable();
    }
}
