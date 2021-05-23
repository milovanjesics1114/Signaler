const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(start);
connection._signalrHandler = (a, b) => { };

var groupId = null;

// Start the connection.
start();

function messageHandler(gid, message, senderUsername) {

    if (gid == groupId) {
        document.getElementById("message-box").innerText += '\n' + senderUsername + ':\n' + message;
    } else {
        getGroups();
    }
}

connection.on("ReceiveMessage", messageHandler);


function getChatMessages(gid) {
    document.getElementById('message-box').innerText = '';

    groupId = gid;
    fetch('/Chats/GroupMessages?groupId=' + gid)
        .then(response => response.text())
        .then(text => {
            let parser = new DOMParser();
            let doc = parser.parseFromString(text, "text/html")

            document.getElementById("message-box").innerText = doc.getElementById("messages").innerText;
        });
}

function getGroups() {
    fetch('/Chats/Groups')
        .then(response => response.text())
        .then(text => {
            let parser = new DOMParser();
            let doc = parser.parseFromString(text, "text/html")

            document.getElementById("groups").innerHTML = doc.getElementById("groups").innerHTML;

        });
}

async function createChat(username) {
    try {
        await connection.invoke("CreateChat", username);

        getGroups();

    } catch (err) {
        alert("Couldn't create chat with : " + username);
        console.error(err);
    }
}

async function createGroup(groupName, users) {
    try {
        await connection.invoke("CreateGroup", groupName, users)
        getGroups();
    } catch (err) {
        console.error(err);
        alert("Couldn't create group: " + groupName);
    }
}

async function sendMessage(gid, msg) {
    try {
        if (gid !== null) {
            await connection.invoke("SendMessage", gid, msg);
            document.getElementById("message-box").innerText += '\nMe:\n' + msg;
        }
    } catch (err) {
        alert("Couldn't send message: " + msg);
        console.error(err);
    }
}

