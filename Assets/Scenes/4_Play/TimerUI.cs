using UnityEngine;
using TMPro;
using Fusion;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        if (gameTimer == null || gameTimer.Runner == null || !gameTimer.Object.IsValid)
            return; // Spawned されるまで何もしない

        float time = gameTimer.RemainingTime;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}





