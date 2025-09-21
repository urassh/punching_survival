using UnityEngine;

public class SpaceSkyboxAnimator : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Skyboxの水平回転速度（度/秒）")]
    public float rotationSpeed = 1.2f; // 正: 東→西に流れる印象

    [Header("Exposure Pulse (任意)")]
    [Range(0f, 5f)] public float baseExposure = 0.4f;
    [Range(0f, 2f)] public float pulseAmplitude = 0.1f;
    [Range(0f, 5f)] public float pulseSpeed = 0.2f;

    [Header("Tint Shift (任意)")]
    public Color baseTint = Color.white;
    public Color targetTint = new Color(0.8f, 0.9f, 1f);
    [Range(0f, 1f)] public float tintLerp = 0.2f;

    private Material skyboxMat;
    private float rotation;

    void Start()
    {
        // 現在のSkyboxマテリアルのインスタンス化（他シーンと共有しないため）
        if (RenderSettings.skybox != null)
            skyboxMat = new Material(RenderSettings.skybox);
        else
            Debug.LogWarning("Skybox material is not assigned in RenderSettings.");

        RenderSettings.skybox = skyboxMat;
    }

    void Update()
    {
        if (skyboxMat == null) return;

        // 1) 回転（Skybox/Panoramic, Skybox/Cubemap は _Rotation を持つ）
        rotation += rotationSpeed * Time.deltaTime;
        skyboxMat.SetFloat("_Rotation", rotation % 360f);

        // 2) 露光の微小パルス（_Exposure）
        float exposure = baseExposure + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        skyboxMat.SetFloat("_Exposure", Mathf.Max(0f, exposure));

        // 3) 色味のなめらかな遷移（_Tint）
        Color oscillated = Color.Lerp(baseTint, targetTint, (Mathf.Sin(Time.time * 0.1f) * 0.5f + 0.5f) * tintLerp);
        skyboxMat.SetColor("_Tint", oscillated);
    }
}
