import React, { useState, useEffect } from 'react';
import {useNavigate} from 'react-router-dom';
import { TacBack } from './TacBack';
import SignalRService from './signalr.service.js';

const LoginWindow = () => {
  const [nickname, setNickname, ] = useState('');
  const [roomname, setRoomname] = useState('');
  
  const navigate = useNavigate();

  useEffect(() => {
    const connection = SignalRService.getConnection();
    connection.on('ReceiveConnection', (message) => {
      handleReceiveConnection(message);
    });

    return () => {
      // Clean up event listeners or other resources if needed
      //connection.off('ReceiveMessage');
    };
  }, []); // Empty dependency array means this effect runs once on mount


  const handleNickChange = (event) => {
    setNickname(event.target.value);
  };
  const handleRoomChange = (event) => {
    setRoomname(event.target.value);
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    SignalRService.getConnection().invoke('JoinGroup', nickname.toString(), roomname.toString());
  };

  const handleReceiveConnection = (message) => {
    console.log(message);
    navigate('/Room');
  };

  return (
    <TacBack className="tacback-login">
      <form onSubmit={handleSubmit}>
        <div className="tacloginform">
          <input
            className="tacinput"
            type="text"
            id="nickname"
            name="nickname"
            placeholder="your nickname"
            value={nickname}
            onChange={handleNickChange}
          />
          <input
            className="tacinput"
            type="text"
            id="roomname"
            name="roomname"
            placeholder="room name"
            value={roomname}
            onChange={handleRoomChange}
          />
          <button className="tacbutton" type="submit">
            Join
          </button>
        </div>
      </form>
    </TacBack>
  );
};

export default LoginWindow;