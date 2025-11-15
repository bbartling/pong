using System;
using UnityEngine;

// This defines the "letter" the Host (Player 1) sends
[Serializable]
public class HostStateMessage
{
    public string type = "host_state";
    public Vector2 ball_pos;
    public float left_paddle_y;
    public int left_score;
    public int right_score;
}

// This defines the "letter" the Client (Player 2) sends
[Serializable]
public class ClientStateMessage
{
    public string type = "client_state";
    public float right_paddle_y;
}

// A helper to peek at the message type
[Serializable]
public class MessageBase
{
    public string type;
}