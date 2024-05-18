namespace Ranking
{
    public class ItemPresenter : UnityEngine.MonoBehaviour
    {
        private ItemView _view;

        private void Start()
        {
            _view = GetComponent<ItemView>();
        }
    }
}
