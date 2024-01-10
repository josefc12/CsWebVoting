import React, { Component } from 'react';

export class HomeTitle extends Component {
  static displayName = HomeTitle.name;

  render() {
    return (
      <div>
        <h1>Place4me</h1>
        <h4>Map voting</h4>
        <h6>{this.props.roomName}</h6>
      </div>
    );
  }
}
