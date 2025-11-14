using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public bool isLeftSide;          // true for the left goal
    public GameManager gameManager;  // drag GameManager here

    private void OnTriggerEnter2D(Collider2D other)
    {
        // --- DEBUG: Check if the GameManager is assigned ---
        if (gameManager == null)
        {
            Debug.LogError("FATAL ERROR: GameManager is NOT assigned on " + gameObject.name);
            return; // Stop here, we can't do anything
        }

        // --- DEBUG: Log what entered the trigger ---
        Debug.Log(gameObject.name + " triggered by: " + other.gameObject.name + " | Tag: " + other.gameObject.tag);

        if (!other.CompareTag("Ball"))
        {
            // --- DEBUG: Log if it's NOT the ball ---
            Debug.Log("Object was not 'Ball', ignoring.");
            return;
        }

        // --- DEBUG: Log that we are about to score ---
        Debug.Log("It was the BALL! Telling GameManager to score.");

        if (isLeftSide)
        {
            // Ball went past the left side -> right player scores
            gameManager.RightPlayerScored();
        }
        else
        {
            // Ball went past the right side -> left player scores
            gameManager.LeftPlayerScored();
        }
    }
}
