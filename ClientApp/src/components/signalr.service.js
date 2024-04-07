// signalr.service.js
import * as signalR from "@microsoft/signalr";

class SignalRService {
    constructor() {
        // Create a SignalR connection
        //This has to change when publishing to port 5000 or whatever
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7292/RoomHub")
            .build();
    }

    startConnection() {
        return this.connection.start();
    }

    getConnection() {
        return this.connection;
    }
}

const signalRService = new SignalRService();

export default signalRService;