# pong
A Unity concept for learning 2-player game development: build a custom server on the internet that allows two Unity clients to play a game of Pong, using WebSockets to communicate data back and forth.

---


### Install Websockets support
https://github.com/endel/NativeWebSocket

In package manager from git URL:
* https://github.com/endel/NativeWebSocket.git#upm


### Install UniTask
https://github.com/Cysharp/UniTask?tab=readme-ov-file#upm-package

In package manager from git URL:
* https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask


### Deployed on Render web app service
* Wake up web service if it is sleeping by going to URL: https://bensunitywebsocketserver.onrender.com/

### In Unity Windows build enter in the URL for the Websocket server
* wss://bensunitywebsocketserver.onrender.com

![Setup Snippet](https://raw.githubusercontent.com/bbartling/pong/develop/setup_snip.png)

### Game Play
![Game Snippet](https://github.com/bbartling/pong/blob/develop/game_snip.png)


---

# 🚀 How the Game Works

## 🎱 1. Unity Handles 100% of the Physics

The ball physics, paddle movement, collision, scoring — all of that stays inside Unity.

* `BallController.cs` moves the ball, applies velocity, collisions, plays sounds. 
* `PaddleController.cs` handles W/S and ↑/↓ input. 
* `GameManager.cs` keeps score and restarts rounds. 
* `GoalTrigger.cs` detects when the ball passes a goal. 

Only **one player acts as the “Host”**, meaning:

* Host simulates ball physics
* Host simulates left paddle
* Client only moves the right paddle
* Host publishes the *entire* game state to the server

The client simply renders whatever game state the host sends.

---

## 🌐 2. WebSockets Transfer State 30 Times Per Second

Instead of sending physics calculations, the game only sends:

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

---


## 📜 License

Everything here is **MIT Licensed** — free, open source, and made for the BAS community.  
Use it, remix it, or improve it — just share it forward so others can benefit too. 🥰🌍


【MIT License】

Copyright 2025 Ben Bartling

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


