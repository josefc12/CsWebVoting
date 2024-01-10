import React, { Component } from 'react';
import { HomeTitle } from './HomeTitle';
import LoginWindow from './LoginWindow';
import SignalRService from './signalr.service.js';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div class="main menu">
        <div class="cnstr-div-titles">
          <HomeTitle />
        </div>
        <LoginWindow/>
      </div>
    );
  }
}
