using UnityEngine;

public class SmallingStage : MonoBehaviour
{
    private Vector3 startScale;
    public float targetPercent = 25f;
    public float gameTime = 60f;

    private Vector3 targetScale;
    private float elapsedTime = 0f;
    private bool isFinished = false; 

    void Start()
    {
        startScale = transform.localScale;

        float scaleMultiplier = targetPercent / 100f;
        targetScale = new Vector3(
            startScale.x * scaleMultiplier,
            startScale.y,
            startScale.z * scaleMultiplier
        );
    }

    void Update()
    {
        if (isFinished)
            return;

        // 制限時間の何%の時間がたったかを計算する（終了判定用）
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / gameTime;

        // 経過時間が100%以上で終了
        if (progress >= 1f)
        {
            progress = 1f;
            isFinished = true; 
        }

        float newX = Mathf.Lerp(startScale.x, targetScale.x, progress);
        float newZ = Mathf.Lerp(startScale.z, targetScale.z, progress);

        transform.localScale = new Vector3(newX, startScale.y, newZ);

    }
}



   




