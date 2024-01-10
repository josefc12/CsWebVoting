import React, { Component } from 'react';

export class TacBack extends Component {
  static displayName = TacBack.name;

  render() {
    const { className, children } = this.props;
    const defaultClassName = 'tacback';
    const classNames = className ? `${defaultClassName} ${className}` : defaultClassName;

    return (
      <div className={classNames}>
        {children}
      </div>
    );
  }
}
