using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimit = 30f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    private float timeRemaining;
    private bool timerRunning = true;

    private void Start()
    {
        timeRemaining = timeLimit;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            UpdateTimerText();
            RestartLevel();
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
    }

    private void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
