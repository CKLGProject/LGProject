using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Pat Data", menuName = "LG Data/Pat Data")]
    public class PatData : ScriptableObject
    {
        public string PatName;
        public Sprite[] PatProfileImage;

        /// <summary>
        /// 정령의 단계에 따라 프로필 이미지를 반환합니다.
        /// </summary>
        /// <param name="level">정령 단계</param>
        /// <returns>정령 이미지</returns>
        public Sprite GetProfileImage(int level)
        {
            return PatProfileImage[level];
        }
    }
}