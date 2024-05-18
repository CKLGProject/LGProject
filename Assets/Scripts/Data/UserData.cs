using System.Collections.Generic;

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
    
    [System.Serializable]
    public class UserData
    {
        private ECharacterType _currentCharacter = 0;

        public string Nickname { get; set; }
        public Dictionary<ECharacterType, bool> HasCharacterMap;
        public PatData[] PatDataList;
    }
}