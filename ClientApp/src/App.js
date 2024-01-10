import React, { Component } from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './AppRoutes';
import { Layout } from './components/Layout';
import "./tac-custom.css";
import SignalRService from './components/signalr.service.js';
export default class App extends Component {
  static displayName = App.name;

  componentDidMount() {
      
    SignalRService.startConnection()
    .then(() => {
        console.log("Connection started successfully in Home");
    })
      
  }
  render() {
    return (
      <div class='cnstr-div'>
      <Layout>
        <Routes>
          {AppRoutes.map((route, index) => {
            const { element, ...rest } = route;
            return <Route key={index} {...rest} element={element} />;
          })}
        </Routes>
      </Layout>
      </div>
    );
  }
}
