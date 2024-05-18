using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AI Model", menuName = "LG Data/AI Model")]
    public class AIModel : ScriptableObject
    {
        public ECharacterType CharacterType;
        public EPatType PatType;
        public int PatLevel;

        public override string ToString()
        {
            return $"{CharacterType}, {PatType}, Pat Level : {PatLevel}";
        }
    }
}