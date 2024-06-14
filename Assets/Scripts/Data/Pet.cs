using UnityEngine;

namespace Data
{
    public class Pet
    {
        public EPetType PetType;
        public PetData PetData;
        public int Level;
        public bool HasPet;

        /// <summary>
        /// 해당 레벨의 프로필 이미지를 제공합니다.
        /// </summary>
        /// <returns></returns>
        public Sprite GetProfileImage()
        {
            if (Level < 0)
                return PetData.PetProfileImage[0];
            
            return PetData.PetProfileImage[Level];
        }
    }
}