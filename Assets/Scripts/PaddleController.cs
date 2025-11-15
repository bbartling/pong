using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 8f;
    public float moveLimit = 4f;

    // We will set this in the Inspector
    public bool isPlayerOne;

    private KeyCode upKey;
    private KeyCode downKey;

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
        pos.y = Mathf.Clamp(pos.y, -moveLimit, moveLimit);
        transform.position = pos;
    }
}