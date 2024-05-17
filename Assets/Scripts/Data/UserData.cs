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
    public class UserData
    {
        private int _currentCharacter = 0;

        public Dictionary<ECharacterType, bool> HasCharacterMap;
        public PatData PatData;
    }
}