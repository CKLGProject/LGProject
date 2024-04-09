using UnityEngine;
namespace Data
{
    [CreateAssetMenu(fileName = "Location", menuName = "Data/Location")]
    public class Location : ScriptableObject
    {
        public string Name;
        public double Latitude;
        public double Longitude;
    }
}
