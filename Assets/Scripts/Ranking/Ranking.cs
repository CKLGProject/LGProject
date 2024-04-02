using System.Collections;
using System.Collections.Generic;
using Supabase;
using UnityEngine;

public class Ranking : MonoBehaviour
{
    public SupabaseSettings supabaseSettings;
    private Client _supabase;
    
    
    // Start is called before the first frame update
    private async void Start()
    {
        if (_supabase == null)
        {
            _supabase = new Client(supabaseSettings.SupabaseURL, supabaseSettings.SupabaseAnonKey);
            await _supabase.InitializeAsync();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
