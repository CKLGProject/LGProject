using UnityEngine;

public class LightController : MonoBehaviour
{
    [Range(0, 1)] public float LightIntensity;
    [Range(0, 1)] public float LightAlpha;
    private static readonly int Intensity = Shader.PropertyToID("_LightIntensity");
    private static readonly int Alpha = Shader.PropertyToID("_LightAlpha");

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        Shader.SetGlobalFloat(Intensity, LightIntensity);
        Shader.SetGlobalFloat(Alpha, LightAlpha);
    }

    private void OnDestroy()
    {
        Reset();
    }

    private void Reset()
    {
        Shader.SetGlobalFloat(Intensity, 0);
        Shader.SetGlobalFloat(Alpha, 0);
    }
}