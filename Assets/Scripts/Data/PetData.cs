using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    public enum EPetType
    {
        None,
        Scorchwing, // 스코치윙
        Icebound, // 아이스바운드
        Aerion, // 에리온
        Electra // 일렉트라
    }
    
    [CreateAssetMenu(fileName = "Pet Data", menuName = "LG Data/Pet Data")]
    public class PetData : ScriptableObject
    {
        [FormerlySerializedAs("PatName")] public string PetName;
        [FormerlySerializedAs("PatType")] public EPetType petType;
        [FormerlySerializedAs("PatProfileImage")] public Sprite[] PetProfileImage;

        /// <summary>
        /// 정령의 단계에 따라 프로필 이미지를 반환합니다.
        /// </summary>
        /// <param name="level">정령 단계</param>
        /// <returns>정령 이미지</returns>
        public Sprite GetProfileImage(int level)
        {
            return PetProfileImage[level];
        }
    }
}