<h2>⚡ Real-Time Notifications (SignalR)</h2>

<p><b>SignalR Hub:</b></p>
<pre><code>http://localhost:5216/hubs/notifications</code></pre>

<p><b>Events emitted:</b></p>
<table>
  <thead>
    <tr>
      <th>Event</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>FriendshipRequested</code></td>
      <td>Sent when a friendship request is created</td>
    </tr>
    <tr>
      <td><code>FriendshipAccepted</code></td>
      <td>Sent when a friendship request is accepted</td>
    </tr>
    <tr>
      <td><code>MessageCreated</code></td>
      <td>Sent when a message is created</td>
    </tr>
  </tbody>
</table>

<hr/>

<h2>🧪 Minimal SignalR Test Client (Node.js)</h2>

<p>Create a file named <code>client.js</code> and paste this code.</p>

<h3>1) Install dependency</h3>
<pre><code>npm install @microsoft/signalr</code></pre>

<h3>2) client.js</h3>
<pre><code>const signalR = require("@microsoft/signalr");

const HUB_URL = process.env.HUB_URL ?? "http://localhost:5216/hubs/notifications";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(HUB_URL)
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Information)
  .build();

connection.on("FriendshipRequested", (payload) =&gt; {
  console.log("FriendshipRequested:", payload);
});

connection.on("FriendshipAccepted", (payload) =&gt; {
  console.log("FriendshipAccepted:", payload);
});

connection.on("MessageCreated", (payload) =&gt; {
  console.log("MessageCreated:", payload);
});

async function start() {
  try {
    await connection.start();
    console.log("✅ Connected to SignalR:", HUB_URL);
  } catch (err) {
    console.error("❌ Failed to connect:", err);
    setTimeout(start, 3000);
  }
}

start();</code></pre>

<h3>3) Run</h3>
<pre><code># optional
export HUB_URL="http://localhost:5216/hubs/notifications"

node client.js</code></pre>

<p>
  Now perform REST actions (friendship request/accept, message create) and watch the events appear in your terminal.
</p>

<blockquote>
  <b>Note:</b> If your Hub routes messages using <code>Clients.User(...)</code>, make sure your app is configuring the user identity correctly; otherwise the connection works but user-targeted events may not be delivered.
</blockquote>
