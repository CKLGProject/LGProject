using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ScanData
    {
        public string ObjectName;
        public GameObject MachineObject;
        public GameObject CharacterObject;
        public string GuideMessage;
        public GameObject GestureUI;
    }
}