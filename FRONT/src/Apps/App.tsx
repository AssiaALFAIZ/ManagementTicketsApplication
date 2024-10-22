import React from 'react';
import '../Assets/App.css';  
import TicketTable from '../Modules/TicketTable';

const App: React.FC = () => {
  return (
    <div className="app-container">
      <h1 className="app-title">Ticket Management</h1>
      <TicketTable />
    </div>
  );
}

export default App;
