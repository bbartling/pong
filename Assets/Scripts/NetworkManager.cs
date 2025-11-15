using UnityEngine;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using System;
using System.Text;
using System.Threading;
using TMPro; // <-- NEW: For the status text
using UnityEngine.UI; // <-- ADD THIS LINE

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [Header("Scene References")]
    public GameObject startMenuPanel;
    public BallController ball;
    public GameManager gameManager;
    public PaddleController leftPaddle;
    public PaddleController rightPaddle;
    public GameObject gameWorld; // <-- NEW: To hide/show all game objects

    private WebSocket client;
    private CancellationTokenSource cts;
    private bool isHost = false;
    private bool gameStarted = false; // <-- NEW: To track state
    private TextMeshProUGUI statusText; // <-- NEW: To show status

    // --- Message containers for sending ---
    private HostStateMessage hostState = new HostStateMessage();
    private ClientStateMessage clientState = new ClientStateMessage();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    // --- UPDATED Connect method signature ---
    public async UniTask Connect(string serverURL, string roomName, int playerId, TextMeshProUGUI status, Button connectButton)
    {
        this.statusText = status; // <-- NEW: Store the status text
        isHost = (playerId == 1);
        gameStarted = false;

        // --- Configure the game for Host or Client ---
        if (isHost)
        {
            Debug.Log("I am the HOST (Player 1). Enabling all physics.");
            gameManager.enabled = true;
            ball.enabled = true;
            leftPaddle.enabled = true;
            rightPaddle.enabled = false;
        }
        else
        {
            Debug.Log("I am the CLIENT (Player 2). Disabling physics.");
            gameManager.enabled = true;
            ball.enabled = false;
            leftPaddle.enabled = false;
            rightPaddle.enabled = true;

            ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            leftPaddle.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }

        // --- Connect to the server ---
        cts = new CancellationTokenSource();
        string fullUrl = serverURL + "/ws/" + roomName;
        client = new WebSocket(fullUrl);

        // --- Setup Event Handlers ---
        client.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            statusText.text = "Connected! Waiting for opponent..."; // <-- NEW
            SendLoop().Forget();
        };

        client.OnError += (e) =>
        {
            Debug.LogError("Error: " + e);
            statusText.text = $"Error: {e}. Check server/URL.";
            connectButton.interactable = true; // Let them try again
        };

        client.OnClose += (e) =>
        {
            Debug.Log("Connection closed: " + e);
            statusText.text = "Disconnected. Please restart."; // <-- NEW
            cts.Cancel();
        };

        client.OnMessage += (bytes) =>
        {
            string msg = Encoding.UTF8.GetString(bytes);
            HandleMessage(msg);
        };

        statusText.text = $"Connecting to {fullUrl}..."; // <-- NEW
        await client.Connect();
    }

    void Update()
    {
        if (client != null && client.State == WebSocketState.Open)
        {
            client.DispatchMessageQueue();
        }
    }

    void HandleMessage(string msg)
    {
        // --- NEW: Check if this is the first message to start the game ---
        if (!gameStarted)
        {
            var baseMsg = JsonUtility.FromJson<MessageBase>(msg);
            if ((isHost && baseMsg.type == "client_state") || (!isHost && baseMsg.type == "host_state"))
            {
                gameStarted = true;
                StartGame(); // Call new function
            }
        }
        // --- END NEW ---

        var messageType = JsonUtility.FromJson<MessageBase>(msg);

        if (isHost && messageType.type == "client_state")
        {
            var state = JsonUtility.FromJson<ClientStateMessage>(msg);
            rightPaddle.transform.position = new Vector3(
                rightPaddle.transform.position.x,
                state.right_paddle_y, 0);
        }
        else if (!isHost && messageType.type == "host_state")
        {
            var state = JsonUtility.FromJson<HostStateMessage>(msg);
            ball.transform.position = state.ball_pos;
            leftPaddle.transform.position = new Vector3(
                leftPaddle.transform.position.x,
                state.left_paddle_y, 0);
            gameManager.UpdateScoreText(state.left_score, state.right_score);
        }
    }

    // --- NEW FUNCTION ---
    void StartGame()
    {
        Debug.Log("Opponent connected. Starting game!");
        startMenuPanel.SetActive(false); // Hide the menu
        gameWorld.SetActive(true);      // Show the game
    }

    async UniTaskVoid SendLoop()
    {
        // ... (This function is unchanged, but make sure it matches) ...
        while (client.State == WebSocketState.Open && !cts.IsCancellationRequested)
        {
            byte[] data;
            if (isHost)
            {
                // HOST: Send the full game state
                hostState.ball_pos = ball.transform.position;
                hostState.left_paddle_y = leftPaddle.transform.position.y;
                hostState.left_score = gameManager.GetLeftScore();
                hostState.right_score = gameManager.GetRightScore();

                string json = JsonUtility.ToJson(hostState);
                data = Encoding.UTF8.GetBytes(json);
            }
            else
            {
                // CLIENT: Send only my paddle position
                clientState.right_paddle_y = rightPaddle.transform.position.y;

                string json = JsonUtility.ToJson(clientState);
                data = Encoding.UTF8.GetBytes(json);
            }

            await client.Send(data);
            await UniTask.Delay(33, cancellationToken: cts.Token);
        }
    }

    void OnDestroy()
    {
        cts?.Cancel();
        if (client != null && client.State == WebSocketState.Open)
        { // <-- FIX: Added the opening brace
            client.Close();
        } // <-- FIX: Added the closing brace
    }
}