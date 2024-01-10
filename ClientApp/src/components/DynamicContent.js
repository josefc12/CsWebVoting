import React, { Component } from 'react';

export class DynamicContent extends Component {
    static displayName = DynamicContent.name;
    

    render() {
        const { className, children } = this.props;
        const defaultClassName = 'dyno';
        const classNames = className ? `${defaultClassName} ${className}` : defaultClassName;
        return (
            <div className={classNames}>
                {children}
            </div>
        );
    }
}