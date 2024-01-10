import React, { Component} from 'react';
import { DynamicContent } from './DynamicContent';

export class RoomContentBeginning extends Component {
    static displayName = RoomContentBeginning.name;

    constructor(props) {
        super(props);

        this.state = {
        };
    }
    

    //Probably has to change from this to whatever the realtime handler will be later.
    startNomination = () => {
        const {onStartClicked} = this.props;
        onStartClicked();
    }
    
    render() {
        const { isAdmin } = this.props;

    return (
        <div class="dyno-content">
            <DynamicContent>
                {/*
                    This button sends a signal to the hub
                    The hub will query the session(room) table and check what the stage of the room this Start signal came from.
                    If it's indeed 0, it will be changed to 1 and it will return 1 to the clients. The clients will set stage to 1.
                */}
                <span class="dyno-nominate-ribbon" style={{ display: isAdmin ?'flex'  : 'none' }}><button class="tacbutton" type="button" onClick={() => this.startNomination()}  >Start</button></span>
            </DynamicContent>
        </div>
    );
    }
}
