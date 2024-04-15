using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ranking
{
    public class ItemView : UnityEngine.MonoBehaviour
    {
        public TMP_Text OrderText;
        public TMP_Text NicknameText;
        public TMP_Text ScoreText;
        public Image ThumbnailImage;

        public Sprite[] ThumbnailSprites;

        public void UpdateUI(int index, RankingData model)
        {
            int thumbnailIndex = model.Thumbnail;
            
            OrderText.text = index.ToString();
            NicknameText.text = model.Nickname;
            ScoreText.text = model.Score.ToString();
            ThumbnailImage.sprite = ThumbnailSprites[thumbnailIndex];
        }
    }
}
