using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using ZXing.QrCode;

namespace LGProjects.Android.Utility
{
    public enum EResultMode
    {
        Update,
        ChangeValue
    }
    
    /// <summary>
    /// AR Session과 ARCameraManager가 계층구조에 존재해야 합니다.
    /// </summary>
    [RequireComponent(typeof(ARCameraManager))]
    public class QRManager : MonoBehaviour
    {
        public delegate void ScanFinishResultDelegate(string result);
        
        /// <summary>
        /// QR이 성공적으로 스캔되면 호출됩니다.
        /// </summary>
        public ScanFinishResultDelegate ScanFinishResult;

        /// <summary>
        /// ChangeValue : 이전 결과와 다를 경우에만 호출합니다.
        /// </summary>
        [Tooltip("ChangeValue : 이전 결과와 다를 경우에만 호출합니다.")]
        public EResultMode resultMode = EResultMode.ChangeValue;
        
        private string _result;
        private IBarcodeReader _barcodeReader;
        private ARCameraManager _cameraManager;

        private void Start()
        {
            _barcodeReader = new BarcodeReader();
            TryGetComponent(out _cameraManager);
        }

        private void Update()
        {
            UpdateQRRecord();
        }

        private void UpdateQRRecord()
        {
            if (!_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                return;

            using (image)
            {
                var conversionParams = new XRCpuImage.ConversionParams(image, TextureFormat.R8, XRCpuImage.Transformation.MirrorY);
                var dataSize = image.GetConvertedDataSize(conversionParams);
                var grayscalePixels = new byte[dataSize];

                unsafe
                {
                    fixed (void* ptr = grayscalePixels)
                    {
                        image.Convert(conversionParams, new System.IntPtr(ptr), dataSize);
                    }
                }
                
                Result result = _barcodeReader.Decode(grayscalePixels, image.width, image.height, RGBLuminanceSource.BitmapFormat.Gray8);

                if (result != null)
                {
                    if (resultMode == EResultMode.Update)
                    {
                        ScanFinishResult?.Invoke(result.Text);
                    }
                    else if (resultMode == EResultMode.ChangeValue)
                    {
                        if (_result != result.Text)
                            ScanFinishResult?.Invoke(result.Text);
                    }
                    
                    _result = result.Text;
                }
            }
        }
        
        /// <summary>
        /// QR 코드용 Color32를 생성합니다.
        /// </summary>
        /// <param name="textForEncoding">QR에 수록할 텍스트</param>
        /// <param name="width">QR 가로 사이즈</param>
        /// <param name="height">QR 세로 사이즈</param>
        /// <returns>QR 컬러 값</returns>
        public static Color32[] ConvertQR(string textForEncoding, int width, int height)
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
    }
}
