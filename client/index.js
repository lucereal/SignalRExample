const signalR = require("@microsoft/signalr");

let connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5197/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    // Handle incoming messages
    console.log("received message: " + message);
});

connection.on("UserAdded", (user, message) => {
    // Handle incoming messages
    console.log("received message: " + message);
});

connection.start().then(() => {
    // Connection to the hub is established
    console.log("connection established");
    let user = "david";
    connection.invoke("AddUser", user).
});


