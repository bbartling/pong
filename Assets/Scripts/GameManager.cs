using UnityEngine;
using TMPro;   // if you’re using TextMeshPro UI

public class GameManager : MonoBehaviour
{
    public BallController ball;
    public TextMeshProUGUI scoreText;   // drag the UI text here in Inspector
    public int maxScore = 5;

    private int leftScore = 0;
    private int rightScore = 0;

    private void Start()
    {
        UpdateScoreText();
    }

    private void Update()
    {
        // Full game reset with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    public void LeftPlayerScored()
    {
        leftScore++;
        ball.PlayRestartSound(); // --- PLAY RESTART SOUND ---
        CheckGameOver();
        RestartRound();
    }

    public void RightPlayerScored()
    {
        rightScore++;
        ball.PlayRestartSound(); // --- PLAY RESTART SOUND ---
        CheckGameOver();
        RestartRound();
    }

    void CheckGameOver()
    {
        if (leftScore >= maxScore || rightScore >= maxScore)
        {
            // You could pop a “Game Over” message here.
            Debug.Log("Game Over! Press R to restart.");
            // For now just stop the ball:
            ball.StopBall();
        }

        UpdateScoreText();
    }

    void RestartRound()
    {
        // Only restart if nobody has reached maxScore
        if (leftScore < maxScore && rightScore < maxScore)
        {
            ball.Launch();
        }
    }

    void ResetGame()
    {
        leftScore = 0;
        rightScore = 0;
        UpdateScoreText();
        ball.Launch();
        ball.PlayRestartSound(); // --- PLAY SOUND ON FULL RESET TOO ---
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = leftScore + " : " + rightScore;
        }
    }
}