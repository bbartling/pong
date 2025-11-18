using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public TMP_InputField serverURLInput;
    public TMP_InputField roomInput;
    public TMP_Dropdown playerDropdown;
    public Button connectButton;
    public TextMeshProUGUI statusText; // <-- NEW

    void Start()
    {
        // Add a listener to the button
        connectButton.onClick.AddListener(OnConnectClicked);
        // Set a default server URL for easy testing
        serverURLInput.text = "wss://bensunitywebsocketserver.onrender.com";
        roomInput.text = "pizza"; // <-- Set your default room
        statusText.text = "Ready to join!"; // <-- Update the status


    }

    void OnConnectClicked()
    {
        string serverURL = serverURLInput.text;
        string roomName = roomInput.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "default"; // Default room
        }

        int playerId = playerDropdown.value + 1;

        // --- NEW ---
        // Disable button and show connecting status
        connectButton.interactable = false;
        statusText.text = "Connecting...";
        // --- END NEW ---

        // Tell the NetworkManager to connect
        NetworkManager.Instance.Connect(serverURL, roomName, playerId, statusText, connectButton).Forget();
    }
}