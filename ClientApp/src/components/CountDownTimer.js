import React, { useState, useEffect } from 'react';

const CountdownTimer = ({ initialTime, Mode }) => {
  const [time, setTime] = useState(initialTime);

  useEffect(() => {
    let interval;

    if (Mode == 1) {
      interval = setInterval(() => {
        if (time > 0) {
          setTime(time - 1);
        } else {
          clearInterval(interval); // Stop the countdown when time reaches 0
        }
      }, 1000); // Update every 1000 milliseconds (1 second)
    }

    return () => {
      clearInterval(interval); // Cleanup the interval on component unmount
    };
  }, [time, Mode]);

  const formatTime = () => {
    const minutes = Math.floor(time / 60);
    const seconds = time % 60;
    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
  };

  return (
    <div>
      <p>{formatTime()}</p>
    </div>
  );
};

export default CountdownTimer;