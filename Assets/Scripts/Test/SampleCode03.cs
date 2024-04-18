using System;
using LGProjects.Android.Utility;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class SampleCode03 : UnityEngine.MonoBehaviour
{
    public string SampleMessage = "This is a LG Love Message";
    public RawImage QRRawImage;

    private Texture2D QRTexture2D;

    private void Start()
    {
        QRTexture2D = new Texture2D(256, 256);
    }
    



}
