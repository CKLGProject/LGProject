using Data;
using UnityEngine;
using USingleton.AutoSingleton;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    public UserData UserData;

    private void Start()
    {
        UserData.Init();

        // 화면이 꺼지지 않도록 처리
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    
    /// <summary>
    /// 닉네임을 설정합니다.
    /// </summary>
    /// <param name="value">설정할 닉네임</param>
    public void SetNickname(string value)
    {
        UserData.Nickname = value;
    }

    /// <summary>
    /// 유저 닉네임을 반환합니다.
    /// </summary>
    /// <returns>유저 닉네임</returns>
    public string GetNickname()
    {
        return UserData.Nickname;
    }

    /// <summary>
    /// 유저의 현재 캐릭터를 반환합니다.
    /// </summary>
    /// <returns>캐릭터 타입</returns>
    public ECharacterType CurrentCharacter()
    {
        return UserData.CurrentCharacter;
    }
    
    /// <summary>
    /// 유저의 현재 펫을 반환합니다.
    /// </summary>
    /// <returns>펫 타입</returns>
    public EPatType CurrentPat()
    {
        return UserData.CurrentPat;
    }
}