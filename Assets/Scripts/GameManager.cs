using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public BallController ball;
    public TextMeshProUGUI scoreText;
    public int maxScore = 5;

    [Header("Audio Clips")]
    [Tooltip("The sound to play when a player wins the match")]
    public AudioClip winnerClip;

    [Tooltip("The sound to play when the game starts (First time and Reset)")]
    public AudioClip startClip; // --- NEW START SOUND SLOT ---

    private AudioSource audioSource;
    private int leftScore = 0;
    private int rightScore = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public int GetLeftScore() => leftScore;
    public int GetRightScore() => rightScore;

    public void UpdateScoreText(int left, int right)
    {
        if (isGameOver) return;
        leftScore = left;
        rightScore = right;
        UpdateScoreText();
    }

    private void Start()
    {
        UpdateScoreText();
    }

    private void Update()
    {
        if (NetworkManager.Instance.GetIsHost() && Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    // --- NEW PUBLIC METHOD TO PLAY START SOUND ---
    public void PlayStartSound()
    {
        if (startClip != null)
        {
            audioSource.PlayOneShot(startClip);
        }
    }

    public void LeftPlayerScored()
    {
        if (isGameOver) return;
        leftScore++;
        ball.PlayRestartSound(); // Plays the "Goal Cheer"
        CheckGameOver();
        if (!isGameOver) RestartRound();
    }

    public void RightPlayerScored()
    {
        if (isGameOver) return;
        rightScore++;
        ball.PlayRestartSound(); // Plays the "Goal Cheer"
        CheckGameOver();
        if (!isGameOver) RestartRound();
    }

    void CheckGameOver()
    {
        if (isGameOver) return;

        if (leftScore >= maxScore)
        {
            EndGame("Player 1 Won!");
        }
        else if (rightScore >= maxScore)
        {
            EndGame("Player 2 Won!");
        }
        else
        {
            UpdateScoreText();
        }
    }

    void EndGame(string message)
    {
        isGameOver = true;
        scoreText.text = message;
        ball.StopBall();
        if (winnerClip != null) audioSource.PlayOneShot(winnerClip);
    }

    void RestartRound()
    {
        ball.Launch();
    }

    void ResetGame()
    {
        isGameOver = false;
        leftScore = 0;
        rightScore = 0;
        UpdateScoreText();
        ball.Launch();

        // --- MODIFIED: Play Start Sound instead of Goal Cheer ---
        PlayStartSound();
    }

    void UpdateScoreText()
    {
        if (scoreText != null && !isGameOver)
        {
            scoreText.text = leftScore + " : " + rightScore;
        }
    }
}