import React, { Component } from 'react';
import { TacBack } from './TacBack';

export class Map extends Component {
  static displayName = Map.name;
  constructor(props) {
    super(props);

    this.state = {
        checked: false
    };
  }

  handleCheckboxChange = (event) => {
    const {data, onCheckboxChange} = this.props;
    const { Name } = data;
    if(event.target.checked){
      this.setState((previousState) => ({
        checked: true
      }));
    } else {
      this.setState((previousState) => ({
        checked: false
      }));
    }
      
    onCheckboxChange(Name,event.target.checked);
  }

  render() {
    const {data} = this.props;
    const {Id, Name} = data;
    const {checkedArray} = this.props;
    console.log(this.props.enable);
    return (
      <TacBack className="tacback-map">
          <p>{Name}</p>
          {this.props.children}
          <input type="checkbox" checked={checkedArray.includes(Name)} onChange={this.handleCheckboxChange} disabled={!this.props.enable & !this.state.checked}/>
      </TacBack>
    );
  }
}