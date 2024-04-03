using System;
using UnityEngine;

namespace Ranking
{
    public class ItemPresenter : MonoBehaviour
    {
        private ItemView _view;

        private void Start()
        {
            _view = GetComponent<ItemView>();
        }
    }
}
