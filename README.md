# pong
A Unity concept for learning 2-player game development: build a custom server on the internet that allows two Unity clients to play a game of Pong, using WebSockets to communicate data back and forth.

Unity handles all gameplay simulation locally, including ball physics, paddle movement, collisions, scoring, and round resets. Scripts like `BallController.cs`, `PaddleController.cs`, `GameManager.cs`, and `GoalTrigger.cs` run entirely inside the Unity engine, meaning the game does not rely on the server for any physics or logic. One player is designated as the **Host**, and that machine becomes the authoritative source of truth for the match—it controls ball movement, simulates the left paddle, and tracks the current score. The other player, the **Client**, controls only the right paddle. The Host sends its full game state (ball position, paddle position, and score) over WebSockets, and the Client simply renders whatever the Host transmits, keeping the two players visually synchronized while ensuring that all physics remain consistent and authoritative.


---

<details>
<summary>📊 Unity Package Manager Notes</summary>

### Install Websockets support
https://github.com/endel/NativeWebSocket

In package manager from git URL:
* https://github.com/endel/NativeWebSocket.git#upm


### Install UniTask
https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package

In package manager from git URL:
* https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask


</details>

---


<details>
<summary>🐍 Python Websocket Server Notes</summary>

* https://github.com/bbartling/bensUnityWebSocketServer

### Deployed on Render web app service
* Wake up web service if it is sleeping by going to URL: https://bensunitywebsocketserver.onrender.com/

### In Unity Windows build enter in the URL for the Websocket server
* wss://bensunitywebsocketserver.onrender.com

![Setup Snippet](https://raw.githubusercontent.com/bbartling/pong/develop/setup_snip.png)

Instead of sending physics calculations to crunch in Python, the game only sends this data below WebSockets Transfer State 30 Times Per Second:

### **What Host sends → Client**

(Every ~33 ms)

```json
{
  "type": "host_state",
  "ball_pos": [x, y],
  "left_paddle_y": y,
  "left_score": 0,
  "right_score": 0
}
```

(Defined in `HostStateMessage` )

### **What Client sends → Host**

```json
{
  "type": "client_state",
  "right_paddle_y": y
}
```

</details>

---

![Game Snippet](https://github.com/bbartling/pong/blob/develop/game_snip.png)

---

## 📜 License

Everything here is **MIT Licensed** — free, open source, and made for the BAS community.  
Use it, remix it, or improve it — just share it forward so others can benefit too. 🥰🌍


【MIT License】

Copyright 2025 Ben Bartling

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


