using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [CreateAssetMenu(fileName = "AI Model", menuName = "LG Data/AI Model")]
    public class AIModel : ScriptableObject
    {
        public ECharacterType CharacterType;
        [FormerlySerializedAs("PatType")] public EPetType petType;
        public int PatLevel;

        public override string ToString()
        {
            return $"{CharacterType}, {petType}, Pat Level : {PatLevel}";
        }
    }
}