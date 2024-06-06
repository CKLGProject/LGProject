using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using USingleton.AutoSingleton;
using Random = UnityEngine.Random;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    // 유저 데이터
    private readonly UserData _userData = new();
    
    // AI 데이터
    private readonly AIData _aiData = new();

    [FormerlySerializedAs("patDataList")] [SerializeField] private PetData[] petDataList;
    [SerializeField] private AIModel[] aiModelList;

    private void Start()
    {
        // 초기화
        InitUserData();

        // 화면이 꺼지지 않도록 처리
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    
    /// <summary>
    /// 닉네임을 설정합니다.
    /// </summary>
    /// <param name="value">설정할 닉네임</param>
    public void SetNickname(string value)
    {
        _userData.Nickname = value;
    }

    /// <summary>
    /// 유저 닉네임을 반환합니다.
    /// </summary>
    /// <returns>유저 닉네임</returns>
    public string GetNickname()
    {
        return _userData.Nickname;
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
                return _userData.CharacterType;
            case ActorType.AI:
                return _aiData.CharacterType;
        }

        return ECharacterType.None;
    }

    /// <summary>
    /// 현재 캐릭터 상태를 변경합니다.
    /// </summary>
    /// <param name="characterType">캐릭터 타입</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetCurrentCharacter(ActorType actorType, ECharacterType characterType)
    {
        switch (actorType)
        {
            case ActorType.User:
                PlayerPrefs.SetInt("Character", (int)characterType);
                
                // 캐릭터 설정
                _userData.CharacterType = characterType;
                break;
            case ActorType.AI:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actorType), actorType, null);
        }
    }
    
    /// <summary>
    /// 해당 액터의 현재 정령을 반환합니다.
    /// </summary>
    /// <returns>정령 타입</returns>
    public Pet GetPat(ActorType actorType)
    {
        switch (actorType)
        {
            case ActorType.User:
                return _userData.Pet;
            case ActorType.AI:
                return _aiData.Pet;
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
        _aiData.CharacterType = model.CharacterType;

        // 정령 데이터 바인딩
        PetData petData = FindPatDataByPatType(model.petType);
        _aiData.Pet.PetData = petData;

        // 펫 레벨 바인딩
        if (_aiData.Pet.PetData)
            _aiData.Pet.Level = model.PatLevel;
        else
            _aiData.Pet.Level = -1;
    }

    /// <summary>
    /// User Data를 초기화합니다.
    /// </summary>
    private void InitUserData()
    {
        // 캐릭터 설정
        _userData.CharacterType = (ECharacterType)PlayerPrefs.GetInt("Character", (int)ECharacterType.Hit);

        // 닉네임 설정
        string nickName = PlayerPrefs.GetString("Nickname", "Guest");
        _userData.Nickname = nickName;

        // 기본으로 히트 캐릭터 수록
        string hasCharacterMapJson = PlayerPrefs.GetString("HasCharacterMap", "{}");
        _userData.HasCharacterMap = JsonConvert.DeserializeObject<Dictionary<ECharacterType, bool>>(hasCharacterMapJson);

        if (_userData.HasCharacterMap.Count == 0) 
            _userData.HasCharacterMap.Add(ECharacterType.Hit, true);
        
        // 펫 설정
        _userData.Pet = new();
        _userData.Pet.PetType = (EPetType)PlayerPrefs.GetInt("Pat", (int)EPetType.None);
        _userData.Pet.PetData = FindPatDataByPatType(_userData.Pet.PetType);
        _userData.Pet.Level = PlayerPrefs.GetInt("Pat Level", 0);
    }
    
    /// <summary>
    /// AI 모델 중 랜덤하게 1개를 반환합니다.
    /// </summary>
    /// <returns>AI 모델</returns>
    /// <exception cref="InvalidOperationException">AI 모델이 비어있을 경우</exception>
    private AIModel GetRandomAIModel()
    {
        if (aiModelList == null || aiModelList.Length == 0)
            throw new InvalidOperationException("AIModelList가 비어 있습니다.");

        int randomIndex = Random.Range(0, aiModelList.Length);
        return aiModelList[randomIndex];
    }

    /// <summary>
    /// 정령 타입으로 정령 데이터를 반환합니다.
    /// </summary>
    /// <param name="petType">찾을 정령 타입</param>
    /// <returns>정령 데이터</returns>
    private PetData FindPatDataByPatType(EPetType petType)
    {
        return petDataList.FirstOrDefault(patData => patData.petType == petType);
    }
}