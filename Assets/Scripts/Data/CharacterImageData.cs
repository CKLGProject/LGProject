using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character Image Data", menuName = "LG Data/Character Image Data")]
    public class CharacterImageData : ScriptableObject
    {
        public ECharacterType CharacterType;
        public Sprite CharacterProfileImage;

        /// <summary>
        /// 캐릭터 프로필 이미지를 반환합니다.
        /// </summary>
        /// <returns>캐릭터 이미지</returns>
        public Sprite GetProfileImage()
        {
            return CharacterProfileImage;
        }
    }
}