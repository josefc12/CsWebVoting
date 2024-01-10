import React, { Component } from 'react';
import { TacBack } from './TacBack';

export class MapScored extends Component {
  static displayName = MapScored.name;
  constructor(props) {
    super(props);

    this.state = {
        
    };
  }

  render() {

    const {data} = this.props;
    const {Name, VoteAmount} = data;

    return (
      <TacBack className="tacback-map">
          <p>{Name}</p>
          <p>{VoteAmount}</p>
      </TacBack>
    );
  }
}