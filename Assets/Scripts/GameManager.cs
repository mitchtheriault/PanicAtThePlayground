using UnityEngine;
using TMPro; // For TextMeshPro
using UnityEngine.SceneManagement; // For restarting or changing scenes

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public float gameDuration = 60f; // Game duration in seconds
    private float timer;
    private bool isGameOver = false;

    public TMP_Text scoreText; // Assign in Inspector
    public TMP_Text timerText; // Assign in Inspector
    public GameObject gameOverScreen; // Assign in Inspector

    public AudioSource backgroundMusic; // Assign in Inspector
    public AudioSource gameOverMusic; // Assign in Inspector

    public static GameManager instance; // Singleton reference

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Assign the singleton
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Prevent duplicate GameManagers
        }

        //DontDestroyOnLoad(gameObject); // Optional: Keep GameManager across scenes
    }

    private void Start()
    {
        timer = gameDuration;
        UpdateScoreText();
        UpdateTimerText();
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false); // Ensure game over screen is hidden at start

        if (backgroundMusic != null)
            backgroundMusic.Play(); // Play background music at start
    }

    private void Update()
    {
        if (isGameOver) return;

        // Countdown timer
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            GameOver();
        }

        UpdateTimerText();
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timer);
    }

    private void GameOver()
    {
        isGameOver = true;
        if (backgroundMusic != null)
            backgroundMusic.Stop(); // Stop background music

        if (gameOverMusic != null)
            gameOverMusic.Play(); // Play game over music

        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        Debug.Log("Game Over! Final Score: " + score);

        GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().enabled = false;

    }

    public void RestartGame()
    {
        if (gameOverMusic != null)
            gameOverMusic.Stop(); // Stop game over music

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        timer = gameDuration;
        score = 0;
    }
}