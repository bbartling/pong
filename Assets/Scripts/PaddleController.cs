using UnityEngine;

public class PaddleController : MonoBehaviour
{
    public float speed = 8f;
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public float moveLimit = 4f;

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
