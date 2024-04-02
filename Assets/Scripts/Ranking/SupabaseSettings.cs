using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseSettings", menuName = "SupabaseSettings")]
public class SupabaseSettings : ScriptableObject
{
    public string SupabaseURL;
    public string SupabaseAnonKey;
}
