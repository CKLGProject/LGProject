using TMPro;
using UnityEngine;

public class PercentageFX : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI integerPartText;
    [SerializeField] private TextMeshProUGUI decimalPartText;
    
    /// <summary>
    /// 데미지 게이지 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="damageGage"></param>
    public void UpdateDamageGageText(float damageGage)
    {
        // Gage 상승
        int a = (int)(damageGage);
        //= 0;
        int b = (int)((damageGage - a) * 10);

        if (integerPartText)
            integerPartText.text = $"<rotate=\"0\">{a}.</rotate>";
        if (decimalPartText)
            decimalPartText.text = $"<rotate=\"0\">{b}%</rotate>";
    }
}
