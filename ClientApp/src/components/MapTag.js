import React, { Component } from 'react';
import { TacBack } from './TacBack';

export class MapTag extends Component {
    static displayName = MapTag.name;

    handleCheckboxChange = (event) =>{
        const {onCheckboxChange} = this.props;
        const {data} = this.props;
        const {Name} = data;
        onCheckboxChange(data,event.target.checked);
    }

    render() {
        const {data, className, myName} = this.props;
        const defaultClassName = 'tacback-maptag';
        const classNames = className ? `${defaultClassName} ${className}` : defaultClassName;
        const {Name} = data;
        console.log(myName + " This my name");
        return (
            <TacBack className={classNames}>
                <p>{Name ? Name:data}</p>
                {this.props.checkEnabled && data !== myName ? <input class="tac-check" type="checkbox" checked="true" onChange={this.handleCheckboxChange}/>:false}
            </TacBack>
        );
    }
}