using Data;
using UnityEngine;
using USingleton.AutoSingleton;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    public string Nickname { get; set; }

    [field: SerializeField] public PatData[] PatDataList { get; private set; }

    private void Start()
    {
        // 화면이 꺼지지 않도록 처리
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    /// <summary>
    /// 정령 개수를 반환합니다.
    /// </summary>
    /// <returns>정령 개수</returns>
    public int GetPatDataCount()
    {
        return PatDataList.Length;
    }

    /// <summary>
    /// index 번째의 펫 이름을 가져옵니다.
    /// </summary>
    /// <param name="index">찾을 펫 인덱스</param>
    /// <returns>펫 이름</returns>
    public string GetPatName(int index)
    {
        return PatDataList[index].PatName;
    }

    /// <summary>
    /// index 번째의 해당 level의 펫 이미지를 가져옵니다.
    /// </summary>
    /// <param name="index">찾을 펫 인덱스</param>
    /// <param name="level">펫 레벨</param>
    /// <returns>펫 프로필 이미지</returns>
    public Sprite GetPatProfileImage(int index, int level)
    {
        return PatDataList[index].PatProfileImage[level];
    }
}