using System;
using UnityEngine;

namespace Data
{
    public enum GestureType
    {
        None,
        DoubleTap,
        ScrollDown,
        Pinch
    }
    
    [Serializable]
    public class ScanData
    {
        public string ObjectName;
        public GameObject MachineObject;
        public GameObject CharacterObject;
        public string GuideMessage;
        public GameObject GestureUI;
        public GestureType GestureType;
        public EPetType RewardPet;
    }
}