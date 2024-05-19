using System.Collections.Generic;

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
        public Pat Pat;
    }
}