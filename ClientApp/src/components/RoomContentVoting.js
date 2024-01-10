import React, { Component} from 'react';
import { DynamicContent } from './DynamicContent';
import { MapVot } from './MapVot';
import { MapScored } from './MapScored';
import SignalRService from './signalr.service.js';

export class RoomContentVoting extends Component {
    static displayName = RoomContentVoting.name;

    constructor(props) {
        super(props);

        this.state = {
            mapData: [], //Object array of maps to be voted on
            selectedMap: String, //The map that's currently selected to vote for
            votedMaps: [], //The top 3 display of maps
            hasVoted: false, //The top 3 display of maps
        };
    }

    componentDidMount() {
        this.spawnComponent();

        const connection = SignalRService.getConnection();

        connection.on("UpdateVotingPool", (pool, message) => {
            this.handleUpdateVotingPool(pool, message);
        });

        connection.on("UpdateTopVotes", (pool) => {
            this.handleUpdateTopVotes(pool);
        });
    }

    //Initially get the map data. Single time.
    spawnComponent = async () => {
        //UpdateVotingPool
        //Ask for initial voting pool from the server.
        const connection = SignalRService.getConnection();
        connection.invoke("RequestVotingPool");
        connection.invoke("RequestTop3");

    
        /*
        try {
            const response = await fetch('https://localhost:7292/nominatedmaps');
            const data = await response.json();
            //const imageUrl = URL.createObjectURL(blob);
            this.setState({ mapData: data });
            console.log({data});
        } catch (error) {
            console.error('Error fetching data:', error);
        }
        */
    };
    //Probably has to change from this to whatever the realtime handler will be later.
    sendVote = async () => {
        const { selectedMap } = this.state;
        const connection = SignalRService.getConnection();
        connection.invoke("SubmitVote", selectedMap)

        this.setState((prevState) => ({
            hasVoted: true,
        }));
    }
    
    handleUpdateVotingPool = (pool,message) => {
        const {passMessage} = this.props;
        const mapsJSON = JSON.parse(pool);
        this.setState((prevState) => ({
            mapData: mapsJSON,
        }));
        passMessage(message);
    }

    handleUpdateTopVotes = (pool) => {
        const votedJSON = JSON.parse(pool);
        this.setState((prevState) => ({
            votedMaps: votedJSON,
        }));
    }

    handleCheckboxChange = (name, isChecked) => {
        if(isChecked){
            this.setState((prevState) => ({
                selectedMap: name,
            }));
        }else {
            this.setState((prevState) => ({
                selectedMap: null,
            }));
        }
    }

    render() {

    const { mapData, hasVoted, selectedMap, votedMaps } = this.state;

    return (
        <div class="dyno-content">
            <DynamicContent>
                <div class="dyno-map-ribbon">
                    {/*
                        Row of MapVoted with real-time score
                        Singleton will make Object out of the nominated array.
                        Users will post votes through realtime socket
                        For testing, use Controller that will take a vode and send an update:
                            1. Client side array of MapVoted
                            2. Updated Top3 MapScored
                    */}
                    {mapData !== null ? mapData.map((item, index) => (
                        <MapVot key={index} data={item} checkedArray={[selectedMap]} enable={true} onCheckboxChange={this.handleCheckboxChange}/>
                    )):null}
                </div>
                {/*/button*/}
                <span class="dyno-nominate-ribbon" style={{ display: hasVoted ?'none'  : 'flex' }}><button class="tacbutton" type="button" onClick={() => this.sendVote()}>Vote</button></span>
            </DynamicContent>
            <h6 className="dyno-title">Top 3:</h6>
            <DynamicContent className="dyno-bot">
                {/*Row of Top3 MapScored with real-time score and*/}
                {votedMaps ? votedMaps.map((item, index) => (
                    <MapScored key={index} data={item} checkEnabled={false}/>
                )):null}
                
            </DynamicContent>
        </div>
    );
    }
}
