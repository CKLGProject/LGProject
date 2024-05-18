using Data;
using System;
using System.Linq;
using UnityEngine;
using USingleton.AutoSingleton;
using Random = UnityEngine.Random;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    // 유저 데이터
    public readonly UserData UserData = new();
    
    // AI 데이터
    public readonly AIData AIData = new();

    [SerializeField] private PatData[] patDataList;
    [SerializeField] private AIModel[] aiModelList;

    private void Start()
    {
        // 초기화
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
    /// 해당 액터의 현재 캐릭터를 반환합니다.
    /// </summary>
    /// <returns>캐릭터 타입</returns>
    public ECharacterType GetCharacter(ActorType actorType)
    {
        switch (actorType)
        {
            case ActorType.User:
                return UserData.CharacterType;
            case ActorType.AI:
                return AIData.CharacterType;
        }

        return ECharacterType.None;
    }
    
    /// <summary>
    /// 해당 액터의 현재 정령을 반환합니다.
    /// </summary>
    /// <returns>정령 타입</returns>
    public Pat GetPat(ActorType actorType)
    {
        switch (actorType)
        {
            case ActorType.User:
                return UserData.Pat;
            case ActorType.AI:
                return AIData.Pat;
        }

        return null;
    }

    /// <summary>
    /// AI 모델을 랜덤하게 선택합니다.
    /// </summary>
    public void RandomChoiceAI()
    {
        // 모델 선택
        AIModel model = GetRandomAIModel();

        // 캐릭터 타입 바인딩
        AIData.CharacterType = model.CharacterType;

        // 정령 데이터 바인딩
        PatData patData = FindPatDataByPatType(model.PatType);
        AIData.Pat.PatData = patData;

        // 펫 레벨 바인딩
        if (AIData.Pat.PatData)
            AIData.Pat.Level = model.PatLevel;
        else
            AIData.Pat.Level = -1;
    }
    
    /// <summary>
    /// AI 모델 중 랜덤하게 1개를 반환합니다.
    /// </summary>
    /// <returns>AI 모델</returns>
    /// <exception cref="InvalidOperationException">AI 모델이 비어있을 경우</exception>
    public AIModel GetRandomAIModel()
    {
        if (aiModelList == null || aiModelList.Length == 0)
            throw new InvalidOperationException("AIModelList가 비어 있습니다.");

        int randomIndex = Random.Range(0, aiModelList.Length);
        return aiModelList[randomIndex];
    }

    /// <summary>
    /// 정령 타입으로 정령 데이터를 반환합니다.
    /// </summary>
    /// <param name="patType">찾을 정령 타입</param>
    /// <returns>정령 데이터</returns>
    public PatData FindPatDataByPatType(EPatType patType)
    {
        return patDataList.FirstOrDefault(patData => patData.PatType == patType);
    }
}