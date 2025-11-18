using UnityEngine;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using System;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [Header("Scene References")]
    public GameObject startMenuPanel;
    public BallController ball;
    public GameManager gameManager;
    public PaddleController leftPaddle;
    public PaddleController rightPaddle;
    public GameObject gameWorld;

    [Header("Networking")]
    [Tooltip("Higher = faster, snappier. Lower = slower, smoother.")]
    public float smoothingSpeed = 15f;

    private WebSocket client;
    private CancellationTokenSource cts;
    private bool isHost = false;
    private bool gameStarted = false;
    private TextMeshProUGUI statusText;

    private HostStateMessage hostState = new HostStateMessage();
    private ClientStateMessage clientState = new ClientStateMessage();

    private Vector2 targetBallPos;
    private float targetLeftPaddleY;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public bool GetIsHost()
    {
        return isHost;
    }

    public async UniTask Connect(string serverURL, string roomName, int playerId, TextMeshProUGUI status, Button connectButton)
    {
        this.statusText = status;
        isHost = (playerId == 1);
        gameStarted = false;

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

            targetBallPos = ball.transform.position;
            targetLeftPaddleY = leftPaddle.transform.position.y;
        }

        cts = new CancellationTokenSource();
        string fullUrl = serverURL + "/ws/" + roomName;
        client = new WebSocket(fullUrl);

        client.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            statusText.text = "Connected! Waiting for opponent...";
            SendLoop().Forget();
        };

        client.OnError += (e) =>
        {
            Debug.LogError("Error: " + e);
            statusText.text = $"Error: {e}. Check server/URL.";
            connectButton.interactable = true;
        };

        client.OnClose += (e) =>
        {
            Debug.Log("Connection closed: " + e);
            statusText.text = "Disconnected. Please restart.";
            cts.Cancel();
        };

        client.OnMessage += (bytes) =>
        {
            string msg = Encoding.UTF8.GetString(bytes);
            HandleMessage(msg);
        };

        statusText.text = $"Connecting to {fullUrl}...";
        await client.Connect();
    }

    void Update()
    {
        if (client != null && client.State == WebSocketState.Open)
        {
            client.DispatchMessageQueue();
        }

        if (!isHost && gameStarted)
        {
            InterpolateClientState();
        }
    }

    void InterpolateClientState()
    {
        ball.transform.position = Vector3.Lerp(
            ball.transform.position,
            targetBallPos,
            Time.deltaTime * smoothingSpeed
        );

        Vector3 paddleTarget = new Vector3(
            leftPaddle.transform.position.x,
            targetLeftPaddleY,
            0
        );

        leftPaddle.transform.position = Vector3.Lerp(
            leftPaddle.transform.position,
            paddleTarget,
            Time.deltaTime * smoothingSpeed
        );
    }

    void HandleMessage(string msg)
    {
        if (!gameStarted)
        {
            var baseMsg = JsonUtility.FromJson<MessageBase>(msg);
            if ((isHost && baseMsg.type == "client_state") || (!isHost && baseMsg.type == "host_state"))
            {
                gameStarted = true;
                StartGame();
            }
        }

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
            targetBallPos = state.ball_pos;
            targetLeftPaddleY = state.left_paddle_y;
            gameManager.UpdateScoreText(state.left_score, state.right_score);
        }
    }

    void StartGame()
    {
        Debug.Log("Opponent connected. Starting game!");
        startMenuPanel.SetActive(false);
        gameWorld.SetActive(true);
        gameManager.scoreText.gameObject.SetActive(true);

        // --- NEW: Play the start sound on connection ---
        gameManager.PlayStartSound();
    }

    async UniTaskVoid SendLoop()
    {
        while (client.State == WebSocketState.Open && !cts.IsCancellationRequested)
        {
            byte[] data;
            if (isHost)
            {
                hostState.ball_pos = ball.transform.position;
                hostState.left_paddle_y = leftPaddle.transform.position.y;
                hostState.left_score = gameManager.GetLeftScore();
                hostState.right_score = gameManager.GetRightScore();
                string json = JsonUtility.ToJson(hostState);
                data = Encoding.UTF8.GetBytes(json);
            }
            else
            {
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
        {
            client.Close();
        }
    }
}

