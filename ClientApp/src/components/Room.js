import React, { Component} from 'react';
import { HomeTitle } from './HomeTitle';
import { RoomContentNominating } from './RoomContentNominating.js';
import { RoomContentVoting } from './RoomContentVoting.js';
import { RoomContentBeginning } from './RoomContentBeginning.js';
import { RoomContentResult } from './RoomContentResult.js';
import { MapTag } from './MapTag.js';
import CountdownTimer from './CountDownTimer.js';
import SignalRService from './signalr.service.js';

export class Room extends Component {
    static displayName = Room.name;

    constructor(props) {
        super(props);

        this.state = {
            stage: null,
            timerKey: Date.now(),
            timeRemaining: null,
            voters: [],
            password: "",
            isAdmin: false,
            playbackMode: true,
            timerMode: 1,
            log: [],
            myName: "",
            RoomName: "",
        };
    }

    componentDidMount() {
        const connection = SignalRService.getConnection();

        connection.on("ReceiveMessage", (user, message) => {
            this.handleReceiveMessage(user, message);
        });

        connection.on("ReceiveRoomInformation", (session, voters) => {
            this.handleReceiveInformation(session, voters);
        });

        connection.on("ReceiveForwardStage", (stage, countdown) => {
            this.handleForwardStage(stage, countdown);
        });

        connection.on("UpdateVoters", (voters,message) => {
            this.handleUpdateVoters(voters,message);
        });

        connection.on("ReceiveAdminStatus", (value) => {
            this.handleReceiveAdminStatus(value);
        });

        connection.on("UpdatePlaybackTimer", (value, message) => {
            this.handleUpdatePlaybackTimer(value, message);
        });

        connection.on("ReceiveKickAlert", () => {
            this.handleReceiveKickAlert();
        });

        this.getRoomInformation();

    }

    //Function to get initial Room/Session information from the server.
    getRoomInformation = () => {
        const connection = SignalRService.getConnection();

        if (connection.state === 'Connected') {
            connection.invoke("ServeRoomInformation")
                .catch(function (err) {
                    console.error(err.toString());
                });
        } else {
            console.error('SignalR connection is not in the "Connected" state.');
        }
    }

    handleReceiveInformation = (session, voters) => {
        const sessionJson = JSON.parse(session);
        this.setState((prevState) => ({
            
            stage: sessionJson.Stage,
            voters: voters,
            timerKey: Date.now(),
            timeRemaining: sessionJson.Timeleft,
            myName: sessionJson.MyName,
            RoomName: sessionJson.Name
        }));
        console.log(sessionJson.Timeleft);
    }

    handleForwardStage = (stage, countdown) => {
        this.setState((prevState) => ({
            timerKey: Date.now(),
            stage: stage,
            timeRemaining: countdown,
        }));
        console.log(this.state.timeRemaining +" during set")
    }

    handleStartClicked = () => {
        const connection = SignalRService.getConnection();
        connection.invoke("RequestForwardStage")

        .catch(function (err) {
            console.error(err.toString());
        });
    }

    handleUpdateVoters = (updatedVoters,message) => {
        this.setState((prevState) => ({
            voters: updatedVoters,
            log: [...prevState.log, message]
        }));
    }

    handlePasswordChange = (event) => {
        this.setState((prevState) => ({
            password: event.target.value,
        }));  
    }

    handlePasswordButton = () => {
        //Submit the password.
        const connection = SignalRService.getConnection();
        connection.invoke("SubmitAdminPassword", this.state.password)
        console.log("this.state.password.toString()")
    }

    handlePlayback = (mode) => {
        //Submit the password.  
        const connection = SignalRService.getConnection();
        connection.invoke("RequestStagePlayback", mode)
    }

    handleNextButton = () => {
        //Submit the password.
        const connection = SignalRService.getConnection();
        connection.invoke("RequestForwardStage")
    }

    handleReceiveAdminStatus = (value) => {
        if (value === true) {
            //set isAdmin to true, which should enable different buttons
            this.setState((prevState) => ({
                isAdmin: value,
            }));  
        } else if (value === false ){
            this.setState((prevState) => ({
                isAdmin: value,
            })); 
        } else {
            //Inform that value received wasn't a bool.
            console.log("Received value wasn't a bool.")
        }
    }

    handleUpdatePlaybackTimer = (value, message) => {
        this.setState((prevState) => ({
            timerMode: value,
            playbackMode: value === 1 ? true : false,
            log: [...prevState.log, message],
        })); 
        
    }

    handlePassMessage = (message) => {
        this.setState((prevState) => ({
            log: [...prevState.log, message],
        })); 
    }

    handleKickRequest = (data) => {
        console.log(data.toString())
        //Admin requested removal of a nomination
        const connection = SignalRService.getConnection();
        connection.invoke("RemoveVoter", data)
    }

    handleReceiveKickAlert = () => {
        alert("YOU HAVE BEEN KICKED!");
        window.location = '/';
    }

    render() {
    
        const { stage, voters, timeRemaining, password, isAdmin, playbackMode,log,myName,RoomName } = this.state;
        console.log(timeRemaining +" during re-render")
        let stageComponent;
        let stageMessage;
        let timer;
        switch (stage){
            case 0: 
                stageComponent = <RoomContentBeginning onStartClicked={this.handleStartClicked} isAdmin={isAdmin}/>;
                stageMessage = "Waiting for Admin";
            break;
            case 1: 
                stageComponent = <RoomContentNominating isAdmin={isAdmin} passMessage={this.handlePassMessage}/>;
                stageMessage = "Nominate";
            break;
            case 2: 
                stageComponent = <RoomContentVoting isAdmin={isAdmin} passMessage={this.handlePassMessage}/>;
                stageMessage = "Vote";
            break;
            case 3: 
                stageComponent = <RoomContentResult isAdmin={isAdmin} />;
                stageMessage = "Session will shutdown in:";
            break;
        }

    return (
        <div class="main room">
            <header class="testheader">
                <div class="cnstr-div-titles header">
                    <HomeTitle roomName={RoomName}/>
                </div>
            </header>
            <div class="admin">
                <input
                    className="tacinput"
                    type="text"
                    id="password"
                    name="password"
                    placeholder="admin password"
                    value={password}
                    onChange={this.handlePasswordChange}
                    style={{ display: isAdmin ? 'none' : 'flex' }}
                />
                <button className="tacbutton" type="submit" onClick={() => this.handlePasswordButton()} style={{ display: isAdmin ? 'none' : 'block' }}>
                    Submit
                </button>
                <button className="tacbutton" type="submit" onClick={() => this.handlePlayback(playbackMode)} style={{ display: isAdmin ? 'block' : 'none' }}>
                    {playbackMode ? "PAUSE":"RESUME"}
                </button>
                <button className="tacbutton" type="submit" onClick={() => this.handleNextButton()} style={{ display: isAdmin ? 'block' : 'none' }}>
                    NEXT
                </button>
            </div>
            
            <div class="content">
                <div class="pane left">
                    <div class="cont player">
                        <h6><b>Players:</b></h6>
                        {voters !== null ? voters.map((item, index) => (
                            <MapTag  className="tacback-maptag player" key={index} myName={myName} data={item} checkEnabled={isAdmin} onCheckboxChange={this.handleKickRequest}/>
                        )):null}
                    </div>
                    <div class="cont log">
                        <h6><b>Log:</b></h6>
                        {log !== null ? log.map((item, index) => (
                            <p key={index}>{item}</p>
                        )):null}
                    </div>
                </div>
                <div class="pane right">
                    <div class="cont tacheading">
                        <p>{stageMessage}</p>
                        <CountdownTimer  key={this.state.timerKey} initialTime={timeRemaining} Mode={this.state.timerMode}/>
                    </div>
                    {stageComponent}
                </div>
            </div>
        </div>
    );
    }
}
