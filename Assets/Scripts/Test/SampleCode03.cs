using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class SampleCode03 : MonoBehaviour
{
    public string SampleMessage = "This is a LG Love Message";
    public RawImage QRRawImage;

    private Texture2D QRTexture2D;

    private void Start()
    {
        QRTexture2D = new Texture2D(256, 256);
    }

    /// <summary>
    /// QR 코드용 Color32를 생성합니다.
    /// </summary>
    /// <param name="textForEncoding">QR에 수록할 텍스트</param>
    /// <param name="width">QR 가로 사이즈</param>
    /// <param name="height">QR 세로 사이즈</param>
    /// <returns>QR 컬러 값</returns>
    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        BarcodeWriter writer = new() {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width
            }
        };

        return writer.Write(textForEncoding);
    }

    /// <summary>
    /// 테스트를 해봅니다.
    /// </summary>
    public void TestGenerateQR()
    {
        Color32[] convert = Encode(SampleMessage, QRTexture2D.width, QRTexture2D.height);

        QRTexture2D.SetPixels32(convert);
        QRTexture2D.Apply();

        QRRawImage.texture = QRTexture2D;
    }
}
