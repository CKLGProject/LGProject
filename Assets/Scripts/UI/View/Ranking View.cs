using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Pool;

namespace Ranking
{
    public class RankingView : UnityEngine.MonoBehaviour
    {
        private ObjectPool<ItemView> _itemPool;
        public ItemView ItemPrefab;

        public Transform ContentView;

        private void Start()
        {
            _itemPool = new ObjectPool<ItemView>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, maxSize: 10);
        }

        public void CreateItem()
        {
            var item = _itemPool.Get();
        }

        /// <summary>
        /// UI를 다시 그립니다.
        /// </summary>
        /// <param name="rankingData"></param>
        public void RefreshUI(List<RankingData> rankingData)
        {
            _itemPool.Clear();

            for (int i = 0; i < rankingData.Count; i++)
            {
                var item = _itemPool.Get();

                int rankingIndex = i + 1;
                item.UpdateUI(rankingIndex, rankingData[i]);
            }
        }
        private ItemView CreateFunc()
        {
            return Instantiate(ItemPrefab, ContentView);
        }
        private void ActionOnGet(ItemView obj)
        {
            obj.gameObject.SetActive(true);
        }

        private void ActionOnRelease(ItemView obj)
        {
            obj.gameObject.SetActive(false);
        }

        private void ActionOnDestroy(ItemView obj)
        {
            Destroy(obj.gameObject);
        }


    }
}
