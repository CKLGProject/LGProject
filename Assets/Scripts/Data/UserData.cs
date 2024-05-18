using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public enum ECharacterType
    {
        None,
        Hit,
        Frost,
        C,
        Storm,
        E
    }

    public class UserData
    {
        public ECharacterType CharacterType;
        public string Nickname;
        public Dictionary<ECharacterType, bool> HasCharacterMap;
        public readonly Pat Pat = new();

        /// <summary>
        /// User 데이터 초기화
        /// </summary>
        public void Init()
        {
            // 캐릭터 설정
            CharacterType = (ECharacterType)PlayerPrefs.GetInt("Character", (int)ECharacterType.Hit);

            // 닉네임 설정
            string nickName = PlayerPrefs.GetString("Nickname", "Guest");
            Nickname = nickName;

            // 기본으로 히트 캐릭터 수록
            string hasCharacterMapJson = PlayerPrefs.GetString("HasCharacterMap", "{}");
            HasCharacterMap = JsonConvert.DeserializeObject<Dictionary<ECharacterType, bool>>(hasCharacterMapJson);

            if (HasCharacterMap.Count == 0) 
                HasCharacterMap.Add(ECharacterType.Hit, true);

            // 펫 설정
            Pat.PatType = (EPatType)PlayerPrefs.GetInt("Pat", (int)EPatType.None);
            Pat.PatData = singlt
            Pat.Level = PlayerPrefs.GetInt("Pat Level", -1);
        }
    }
}