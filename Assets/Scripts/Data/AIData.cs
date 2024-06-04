namespace Data
{
    public class AIData
    {
        public ECharacterType CharacterType;
        public readonly Pet Pet = new();

        /// <summary>
        /// AI 데이터를 리셋합니다.
        /// </summary>
        public void Reset()
        {
            CharacterType = ECharacterType.None;
            Pet.PetData = null;
            Pet.Level = -1;
        }

        public override string ToString()
        {
            return $"{CharacterType}, {Pet.PetData.petType}, PatLevel:{Pet.Level}";
        }
    }
}