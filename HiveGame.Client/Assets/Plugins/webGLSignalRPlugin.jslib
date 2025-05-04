mergeInto(LibraryManager.library, {
    InitializeSignalR: function (serverUrlPtr, tokenPtr) {
        const serverUrl = UTF8ToString(serverUrlPtr);
        const token = UTF8ToString(tokenPtr);

        window.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(serverUrl, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect([0, 2000, 10000, 30000])
            .build();

        window.hubConnection.on("ReceiveMessage", function (message) {
            const jsonString = JSON.stringify(message);
            SendMessage('WebGlHubService', 'ReceiveMessage', jsonString);
        });

        window.hubConnection.start().catch(err => console.error(err));
    },

    JoinQueue: function () {
        window.hubConnection.invoke("JoinQueue").catch(err => console.error(err));
    },

    LeaveQueue: function () {
        window.hubConnection.invoke("LeaveQueue").catch(err => console.error(err));
    },

    PutInsect: function (insectPtr, x, y, z) {
        const insect = UTF8ToString(insectPtr);
        window.hubConnection.invoke("PutInsect", insect, [x, y, z]).catch(err => console.error(err));
    },

    PutFirstInsect: function (insectPtr) {
        const insect = UTF8ToString(insectPtr);
        window.hubConnection.invoke("PutFirstInsect", insect).catch(err => console.error(err));
    },

    MoveInsect: function (fromX, fromY, fromZ, toX, toY, toZ) {
        window.hubConnection.invoke("MoveInsect", [fromX, fromY, fromZ], [toX, toY, toZ]).catch(err => console.error(err));
    }
});