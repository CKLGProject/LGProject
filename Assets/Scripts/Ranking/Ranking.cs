using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Data;
using Postgrest.Responses;
using UnityEngine;
using UnityEngine.InputSystem;
using Client = Supabase.Client;


public class Ranking : MonoBehaviour
{
    public SupabaseSettings supabaseSettings;
    private Client _supabase;

    private async void Start()
    {
        if (_supabase == null)
        {
            _supabase = new Client(supabaseSettings.SupabaseURL, supabaseSettings.SupabaseAnonKey);
            await _supabase.InitializeAsync();
        }
    }

    private void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
            InsertData().Forget();

        if (Keyboard.current.sKey.wasPressedThisFrame)
            ShowData().Forget();

        if (Keyboard.current.dKey.wasPressedThisFrame)
            UpdateData().Forget();
    }

    private async UniTaskVoid ShowData()
    {
        ModeledResponse<RankingModel> result = await _supabase.From<RankingModel>().Get();
        List<RankingModel> rankingData = result.Models;

        foreach (RankingModel rankingModel in rankingData)
            Debug.Log(rankingModel.Nickname);
    }

    private async UniTaskVoid InsertData()
    {
        RankingModel newModel = new();
        ModeledResponse<RankingModel> result = await _supabase.From<RankingModel>().Insert(newModel);

        if (result.ResponseMessage is { StatusCode: HttpStatusCode.Created })
            Debug.Log("생성됨");
    }

    private async UniTaskVoid UpdateData()
    {
        ModeledResponse<RankingModel> update = await _supabase.From<RankingModel>()
            .Where(x => x.Id == 1)
            .Set(x => x.CreatedAt, DateTime.Now)
            .Update();
    }
}
