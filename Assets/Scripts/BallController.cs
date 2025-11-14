using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    public float startSpeed = 6f;

    // --- NEW AUDIO VARIABLES ---
    public AudioClip[] paddleBounceClips; // Array for boing1, boing2, etc.
    public AudioClip wallBounceClip;      // For your future wall sound
    public AudioClip restartClip;         // For the restart/score sound

    private Rigidbody2D rb;
    private AudioSource audioSource;      // The "speaker" component
    // ----------------------------


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Get the "speaker" we added to the Ball
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Launch();
    }

    public void Launch()
    {
        // Reset to center and give it a fresh direction
        transform.position = Vector3.zero;

        float xDir = Random.value < 0.5f ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);

        Vector2 dir = new Vector2(xDir, yDir).normalized;
        rb.linearVelocity = dir * startSpeed;
    }

    public void StopBall()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // --- NEW PUBLIC METHOD FOR GAMEMANAGER ---
    // The GameManager will call this function when a point is scored
    public void PlayRestartSound()
    {
        if (restartClip != null)
        {
            audioSource.PlayOneShot(restartClip);
        }
    }

    // --- WE ARE ADDING THIS BACK (FOR SOUNDS ONLY) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit a Paddle
        if (collision.gameObject.CompareTag("Paddle"))
        {
            // Pick a random "boing" from our list
            if (paddleBounceClips != null && paddleBounceClips.Length > 0)
            {
                int index = Random.Range(0, paddleBounceClips.Length);
                AudioClip clipToPlay = paddleBounceClips[index];
                audioSource.PlayOneShot(clipToPlay);
            }
        }
        // Check if we hit a Wall (you will need to create a "Wall" tag for this)
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Play the wall sound (if we have one assigned)
            if (wallBounceClip != null)
            {
                audioSource.PlayOneShot(wallBounceClip);
            }
        }
    }
}