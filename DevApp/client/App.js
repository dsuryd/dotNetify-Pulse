import React from 'react';
import { VMContext } from 'dotnetify-elements/components';
import { DataGrid } from 'dotnetify-elements/components/DataGrid';
import { Frame } from 'dotnetify-elements/components/Panel';

class App extends React.Component {
  render() {
    return (
      <VMContext vm="PulseVM">
        <Frame>
          <h1>Pulse</h1>
          <DataGrid id="Logs" />
        </Frame>
      </VMContext>
    );
  }
}

export default App;
