using System.Collections.Generic;

namespace Data
{
    public class UserData
    {
        public ECharacterType CharacterType;
        public string Nickname;
        public Dictionary<ECharacterType, bool> HasCharacterMap;
        public Dictionary<EPetType, bool> HasPetMap;
        public Dictionary<EPetType, bool> IsEnablePetMap;
        public Pet Pet;
    }
}