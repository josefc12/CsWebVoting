import React, { Component} from 'react';
import { DynamicContent } from './DynamicContent';
import { MapScored } from './MapScored';
import SignalRService from './signalr.service.js';

export class RoomContentResult extends Component {
    static displayName = RoomContentResult.name;

    constructor(props) {
        super(props);

        this.state = {
            winner: [],
        };
    }

    componentDidMount() {
        this.fetchWinner();

        const connection = SignalRService.getConnection();

        connection.on("UpdateWinner", (winner) => {
            this.handleUpdateWinner(winner);
        });
    }
    //Probably has to change from this to whatever the realtime handler will be later.
    fetchWinner = async () => {
        const connection = SignalRService.getConnection();
        connection.invoke("RequestWinner");
    }

    handleUpdateWinner = (winner) => {
        const winnerJSON = JSON.parse(winner);
        this.setState((prevState) => ({
            winner: winnerJSON,
        }));
    }
    
    render() {
        const { isAdmin } = this.props;
        const { winner } = this.state;
    return (
        <div class="dyno-content">
            <DynamicContent>
                {winner !== null ? winner.map((item, index) => (
                    <MapScored key={index} data={item} checkEnabled={false}/>
                )):null}
                <span class="dyno-nominate-ribbon" style={{ display: isAdmin ?'flex'  : 'none' }}><button class="tacbutton" type="button" onClick={() => this.copy() }>Copy</button></span>
            </DynamicContent>
        </div>
    );
    }
}
