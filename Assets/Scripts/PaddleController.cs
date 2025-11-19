using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 8f;

    // --- NEW: Separate limits for Top and Bottom ---
    // Set these in the Inspector to match your specific coordinates
    public float minY = -5f; // Default value, adjust in Inspector
    public float maxY = 5f;  // Default value, adjust in Inspector

    // We will set this in the Inspector
    public bool isPlayerOne;

    private KeyCode upKey;
    private KeyCode downKey;

    // Track if we've already logged the limit to avoid spamming the console
    private bool hasLoggedMax = false;
    private bool hasLoggedMin = false;

    void Start()
    {
        // Assign keys based on player
        if (isPlayerOne)
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
        }
        else
        {
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
        }
    }

    void Update()
    {
        float move = 0f;

        if (Input.GetKey(upKey)) move = 1f;
        if (Input.GetKey(downKey)) move = -1f;

        Vector3 pos = transform.position;
        pos.y += move * speed * Time.deltaTime;

        // --- LOGGING ONLY (No Clamping Yet) ---

        // Check Max Y
        if (pos.y >= maxY)
        {
            if (!hasLoggedMax)
            {
                Debug.Log($"{gameObject.name} reached MAX Y limit: {pos.y}");
                hasLoggedMax = true; // Log once
            }
        }
        else
        {
            hasLoggedMax = false; // Reset flag when we move away
        }

        // Check Min Y
        if (pos.y <= minY)
        {
            if (!hasLoggedMin)
            {
                Debug.Log($"{gameObject.name} reached MIN Y limit: {pos.y}");
                hasLoggedMin = true; // Log once
            }
        }
        else
        {
            hasLoggedMin = false; // Reset flag when we move away
        }

        // We are NOT clamping the position here, just logging.
        // To find your limits:
        // 1. Set Min Y to -10 and Max Y to 10 in the Inspector (so you have room).
        // 2. Play the game.
        // 3. Move paddle to the top wall. Look at the Console or Transform Y.
        // 4. Move paddle to the bottom wall. Look at the Console or Transform Y.

        transform.position = pos;
    }
}