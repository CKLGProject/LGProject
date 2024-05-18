namespace Data
{
    public class AIData
    {
        public ECharacterType CharacterType;
        public readonly Pat Pat = new();

        /// <summary>
        /// AI 데이터를 리셋합니다.
        /// </summary>
        public void Reset()
        {
            CharacterType = ECharacterType.None;
            Pat.PatData = null;
            Pat.Level = -1;
        }

        public override string ToString()
        {
            return $"{CharacterType}, {Pat.PatData.PatType}, PatLevel:{Pat.Level}";
        }
    }
}