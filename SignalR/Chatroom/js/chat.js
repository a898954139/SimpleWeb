"use strict";

const connection = new signalR
    .HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", (user, message) => {
   let li = document.createElement("li");
   li.textContent = `${user} says ${message}`;
   document.getElementById("messageList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document