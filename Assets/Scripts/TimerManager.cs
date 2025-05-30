using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject losePanel; 
    private WinChecker winChecker; 
    public float totalTime = 45f;
    private float timeRemaining;
    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    void Start()
    {
        winChecker = FindObjectOfType<WinChecker>();

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        ResetTimer();
    }

    void Update()
    {
        if (!isGameOver)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameOver = true;
                UpdateTimerDisplay();

                ShowLosePanel();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int totalSeconds = Mathf.CeilToInt(timeRemaining); 
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ResetTimer()
    {
        timeRemaining = totalTime;
        isGameOver = false;
        if (losePanel != null) losePanel.SetActive(false);
        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        isGameOver = true;
    }

    private void ShowLosePanel()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            if (winChecker != null && winChecker.gameObjectToDisable != null)
            {
                winChecker.gameObjectToDisable.SetActive(false); 
            }
        }

    }
}