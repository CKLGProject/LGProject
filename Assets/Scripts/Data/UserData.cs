using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum ECharacterType
    {
        Hit,
        Frost,
        C,
        Storm,
        E
    }

    public struct PatInfo
    {
        public EPatType PatType;
        public int Level;
    }

    [System.Serializable]
    public class UserData
    {
        public ECharacterType CurrentCharacter;
        public string Nickname;
        public Dictionary<ECharacterType, bool> HasCharacterMap;
        public PatInfo CurrentPat;

        public void Init()
        {
            // 펫 설정
            CurrentPat.PatType = (EPatType)PlayerPrefs.GetInt("Pat", (int)EPatType.None);
            
            // 캐릭터 설정
            CurrentCharacter = (ECharacterType)PlayerPrefs.GetInt("Character", (int)ECharacterType.Hit);

            // 닉네임 설정
            string nickName = PlayerPrefs.GetString("Nickname", "Guest");
            Nickname = nickName;

            // 기본으로 히트 캐릭터 수록
            HasCharacterMap = new Dictionary<ECharacterType, bool>();
            HasCharacterMap.Add(ECharacterType.Hit, true);
        }
    }
}