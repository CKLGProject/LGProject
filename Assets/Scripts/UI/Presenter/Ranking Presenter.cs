using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Postgrest;
using Postgrest.Responses;
using UnityEngine;
using Client = Supabase.Client;

namespace Ranking
{
    public class RankingPresenter : MonoBehaviour
    {
        private RankingModel _model;
        private RankingView _view;
        private Client _supabase;

        private async void Start()
        {
            _model = GetComponent<RankingModel>();
            _view = GetComponent<RankingView>();

            var supabaseSettings = _model.SupabaseSettings;

            _supabase = new Client(supabaseSettings.SupabaseURL, supabaseSettings.SupabaseAnonKey);
            await _supabase.InitializeAsync();

            Init();
        }

        private async void Init()
        {
            // 값 가져오기
            ModeledResponse<RankingData> result = await _supabase.From<RankingData>()
                .Order( "score", Constants.Ordering.Descending).Get();
     
            List<RankingData> rankingData = result.Models;
            
            // View에 반영
            _view.RefreshUI(rankingData);
        }
        
        
    }
}
// 메모 코드
//     private async UniTaskVoid InsertData()
//     {
//         RankingModel newModel = new();
//         ModeledResponse<RankingModel> result = await _supabase.From<RankingModel>().Insert(newModel);
//
//         if (result.ResponseMessage is { StatusCode: HttpStatusCode.Created })
//             Debug.Log("생성됨");
//     }
//
//     private async UniTaskVoid UpdateData()
//     {
//         ModeledResponse<RankingModel> update = await _supabase.From<RankingModel>()
//             .Where(x => x.Id == 1)
//             .Set(x => x.CreatedAt, DateTime.Now)
//             .Update();
//     }