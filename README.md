<h1>🕊️ PigeOn — Real-Time Friendship and Messaging System</h1>

<p>
<img src="https://i.ibb.co/WWNcb28p/Untitled-design.png" alt="PigeOn logo" width="400"/>
</p>

<p>
PigeOn is a practical back-end application demonstrating how to build <b>friendship and messaging systems</b> with <b>real-time notifications</b> using:
</p>

<ul>
<li>ASP.NET Core</li>
<li>SignalR</li>
<li>NATS (JetStream)</li>
<li>PostgreSQL</li>
</ul>

<hr/>

<h2>🧠 Architecture Overview</h2>

<ul>
<li>REST API receives requests</li>
<li>Events are published to NATS</li>
<li>Background consumers process events</li>
<li>SignalR delivers real-time notifications</li>
</ul>

<hr/>

<h2>💻 Requirements</h2>

<ul>
<li>.NET 10 SDK</li>
<li>PostgreSQL</li>
<li>NATS Server</li>
<li>Node.js (optional, for SignalR testing)</li>
<li>Git</li>
</ul>

<hr/>

<h2>🚀 Setup</h2>

<h3>1. Clone repository</h3>

<pre><code>git clone https://github.com/GustavoPoeta/pigeOn-api.git
cd pigeOn-api</code></pre>

<h3>2. Start NATS Server with JetStream</h3>

<pre><code>nats-server -js</code></pre>

<p>Expected output:</p>

<pre><code>Server is ready
JetStream is ready</code></pre>

<h3>3. Configure PostgreSQL</h3>

<p>Edit <code>appsettings.Development.json</code></p>

<pre><code>{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pigeon;Username=postgres;Password=yourpassword"
  }
}</code></pre>

<h3>4. Run database migrations</h3>

<pre><code>dotnet ef database update</code></pre>

<h3>5. Run API</h3>

<pre><code>dotnet run</code></pre>

<p>API running at:</p>

<pre><code>http://localhost:5216</code></pre>

<p>Swagger:</p>

<pre><code>http://localhost:5216/swagger</code></pre>

<hr/>

<h2>☕ REST API Quick Test</h2>

<h3>Create users</h3>

<pre><code>curl -X POST http://localhost:5216/api/users/create ^
-H "Content-Type: application/json" ^
-d "{
  \"username\": \"user1\",
  \"email\": \"user1@test.com\",
  \"password\": \"12345678\",
  \"photoPath\": null
}"</code></pre>


<pre><code>curl -X POST http://localhost:5216/api/users/create ^
-H "Content-Type: application/json" ^
-d "{
  \"username\": \"user2\",
  \"email\": \"user2@test.com\",
  \"password\": \"12345678\",
  \"photoPath\": null
}"</code></pre>


<h3>Send friendship request</h3>

<pre><code>curl -X POST http://localhost:5216/api/friendships/request ^
-H "Content-Type: application/json" ^
-d "{
  \"userId\": 1,
  \"friendId\": 2
}"</code></pre>


<h3>Accept friendship</h3>

<pre><code>curl -X POST http://localhost:5216/api/friendships/accept ^
-H "Content-Type: application/json" ^
-d "{
  \"userId\": 2,
  \"friendId\": 1
}"</code></pre>


<h3>Send message</h3>

<pre><code>curl -X POST http://localhost:5216/api/messages/create ^
-H "Content-Type: application/json" ^
-d "{
  \"senderId\": 1,
  \"receiverId\": 2,
  \"content\": \"Hello\"
}"</code></pre>

<hr/>

<h2>⚡ SignalR Real-Time Notifications</h2>

<p>SignalR Hub:</p>

<pre><code>http://localhost:5216/hubs/notifications</code></pre>

<p>Events:</p>

<ul>
<li><code>FriendshipRequested</code></li>
<li><code>FriendshipAccepted</code></li>
<li><code>MessageCreated</code></li>
</ul>

<hr/>

<h2>🧪 SignalR Test Client (Node.js)</h2>

<h3>1. Install dependency</h3>

<pre><code>npm install @microsoft/signalr</code></pre>

<h3>2. Create file client.js</h3>

<pre><code>const signalR = require("@microsoft/signalr");

const connection = new signalR.HubConnectionBuilder()
.withUrl("http://localhost:5216/hubs/notifications")
.withAutomaticReconnect()
.configureLogging(signalR.LogLevel.Information)
.build();

connection.on("FriendshipRequested", data =&gt; {

console.log("FriendshipRequested:", data);

});

connection.on("FriendshipAccepted", data =&gt; {

console.log("FriendshipAccepted:", data);

});

connection.on("MessageCreated", data =&gt; {

console.log("MessageCreated:", data);

});

async function start(){

try{

await connection.start();

console.log("Connected");

}catch(err){

console.error(err);

}

}

start();</code></pre>

<h3>3. Run client</h3>

<pre><code>node client.js</code></pre>

<hr/>

<h2>✅ Expected Result</h2>

<p>You should see:</p>

<pre><code>Connected

FriendshipRequested: {...}

MessageCreated: {...}</code></pre>

<hr/>

<h2>⚠️ Important</h2>

<p>
If your Hub uses:
</p>

<pre><code>Clients.User(userId)</code></pre>

<p>
you must configure authentication and user identity correctly,
otherwise SignalR connects but does not deliver user-targeted events.
</p>

<hr/>

<h2>📜 License</h2>

<p>
Free to use for learning and demonstration.
</p>
