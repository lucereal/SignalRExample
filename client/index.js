const signalR = require("@microsoft/signalr");

// axios.post(`${apiBaseUrl}/api/negotiate?userid=${data.username}`, null, null)
//     .then(resp => resp.data);
const options = {
    accessTokenFactory: () => "eyJhbGciOiJIUzI1NiIsImtpZCI6Ii0xMzUzNzU3MTA4IiwidHlwIjoiSldUIn0.eyJhc3JzLnMudWlkIjoiZGYiLCJuYmYiOjE2OTU4NTgwMjIsImV4cCI6MTY5NTg2MTYyMiwiaWF0IjoxNjk1ODU4MDIyLCJhdWQiOiJodHRwczovL3JlY2VpcHRzaWduYWxyLnNlcnZpY2Uuc2lnbmFsci5uZXQvY2xpZW50Lz9odWI9aHViIn0.WwSCL3sHldiFpm5WU-6dKUzkqK2vjb5_wiE-So0trCc"
};
// connection = new signalR.HubConnectionBuilder()
//     .withUrl(info.url, options)
//     .configureLogging(signalR.LogLevel.Information)
//     .build();
let connection = new signalR.HubConnectionBuilder()
    .withUrl("https://receiptsignalr.service.signalr.net/client/?hub=hub",options)
    .build();
//Endpoint=https://receiptsignalr.service.signalr.net;AccessKey=xlM9L9GzO+nZEfPnwSOz3TGXNJfFFoR3ORrxMC1ciK0=;Version=1.0;
connection.on("ReceiveMessage", (user, message) => {
    // Handle incoming messages
    console.log("received message: " + message);
});

connection.on("newMessage", (message) => {
    // Handle incoming messages
    console.log("received message: " + message);
});

connection.on("broadcast", (user, message) => {
    // Handle incoming messages
    console.log("received message: " + message);
});

connection.start().then(() => {
    // Connection to the hub is established
    console.log("connection established");
    let user = "david";
    //connection.invoke("AddUser", user)
    
    connection.invoke("fish", "new message fool");
});


