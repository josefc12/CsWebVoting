import React, { Component} from 'react';
import { DynamicContent } from './DynamicContent';
import { Map } from './Map';
import { MapTag } from './MapTag';
import SignalRService from './signalr.service.js';

export class RoomContentNominating extends Component {
    static displayName = RoomContentNominating.name;

    constructor(props) {
        super(props);

        this.state = {
            jsonData: [],
            selectedNames: [],
            nominatedMaps: [],
            hasNominated: false,
        };
    }

    componentDidMount() {
        this.spawnComponent();

        const connection = SignalRService.getConnection();

        connection.on("UpdateNominations", (nominations, message) => {
            this.handleUpdateNominations(nominations, message);
        });
    }

    handleUpdateNominations = (nominations,message) => {
        const {passMessage} = this.props;
        const mapsJSON = JSON.parse(nominations);
        this.setState((prevState) => ({
            nominatedMaps: mapsJSON,
        }));
        passMessage(message);
    }

    spawnComponent = async () => {
        try {
            const response = await fetch('https://localhost:7292/image');
            const data = await response.json();
            //const imageUrl = URL.createObjectURL(blob);
            this.setState({ jsonData: data });
            console.log({data});
        } catch (error) {
            console.error('Error fetching data:', error);
        }
    };
    //Probably has to change from this to whatever the realtime handler will be later.
    //Every time someone sends nomintion it should be pushed to others.
    //Probably not through database, but through SignalR directly. It will be written to database as well,
    //for when someone reconnects to nominating in progress. So there should be a switch in room, that does different things base on the
    //stage
    //The added maps will be sent through message
    sendNomination = async () => {
        const { selectedNames } = this.state;
        const connection = SignalRService.getConnection();
        connection.invoke("SubmitNominations", selectedNames)

        this.setState((prevState) => ({
            hasNominated: true,
        }));
    }
    
    handleCheckboxChange = (name, isChecked) => {
        if(isChecked & this.state.selectedNames.length <= 2){
            this.setState((prevState) => ({
                selectedNames: [...prevState.selectedNames, name],
            }));
        }else {
            this.setState((prevState) => ({
                selectedNames: prevState.selectedNames.filter((selectedName) => selectedName !== name),
            }));
        }
    }
    handleNominationRemoval = (data) => {
        //Admin requested removal of a nomination
        const connection = SignalRService.getConnection();
        connection.invoke("RemoveNomination", data.Name)
    }
    
    render() {

    const { hasNominated,jsonData,selectedNames,nominatedMaps } = this.state;
    const {isAdmin} = this.props;

    return (
        <div class="dyno-content">
            <DynamicContent>
                <div class="dyno-map-ribbon">
                    {jsonData !== null ? jsonData.map((item, index) => (
                        <Map key={index} data={item} checkedArray={selectedNames} enable={selectedNames.length <=2 ? true:false} onCheckboxChange={this.handleCheckboxChange}/>
                    )):null}
                </div>
                <div class="dyno-choice-ribbon" style={{ display: hasNominated ?'none'  : 'flex' }}>
                    {selectedNames ? selectedNames.map((item, index) => (
                        <MapTag key={index} data={item} onCheckboxChange={this.handleCheckboxChange} checkEnabled={true}/>
                    )):null}
                </div>
                <span class="dyno-nominate-ribbon" style={{ display: hasNominated ?'none'  : 'flex' }}><p>{selectedNames.length}/3</p><button class="tacbutton" type="button" onClick={() => this.sendNomination()}>Nominate</button></span>
            </DynamicContent>
            <h6 className="dyno-title">Nominated:</h6>
            <DynamicContent className="dyno-bot">
                {nominatedMaps ? nominatedMaps.map((item, index) => (
                        <MapTag key={index} data={item} checkEnabled={isAdmin} onCheckboxChange={this.handleNominationRemoval}/>
                    )):null}
            </DynamicContent>
        </div>
    );
    }
}
