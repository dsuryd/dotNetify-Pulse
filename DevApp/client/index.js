import React from 'react';
import dotnetify from 'dotnetify';
import ReactDOM from 'react-dom';
import App from './App';

dotnetify.debug = true;
ReactDOM.render(<App />, document.getElementById('App'));
