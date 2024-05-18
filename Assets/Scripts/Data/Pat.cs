using UnityEngine;

namespace Data
{
    public class Pat
    {
        public EPatType PatType;
        public PatData PatData;
        public int Level;

        /// <summary>
        /// 해당 레벨의 프로필 이미지를 제공합니다.
        /// </summary>
        /// <returns></returns>
        public Sprite GetProfileImage()
        {
            if (Level < 0)
                return PatData.PatProfileImage[0];
            
            return PatData.PatProfileImage[Level];
        }
    }
}