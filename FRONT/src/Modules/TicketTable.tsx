import React, { useState, useEffect, ChangeEvent, useCallback} from 'react';
import {
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper,
  Button, TextField, Dialog, DialogActions, DialogContent, DialogTitle, Select, MenuItem,
  SelectChangeEvent, Pagination, IconButton,
  Alert,
  AlertTitle
} from '@mui/material';
import Stack from '@mui/material/Stack';
import axios from 'axios';
import AddIcon from '@mui/icons-material/Add';
import EditIcon from '@mui/icons-material/Edit';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import { PagedResult } from '../Apps/PagedResult';
import DoneIcon from '@mui/icons-material/Done';
import '../Assets/App.css'; 



interface Ticket {
  id: number;
  description: string;
  status: TicketStatus;
  date: string;
}
enum TicketStatus {
  Open = 0,   
  Closed = 1  
}

export default function TicketTable() {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [addOpen, setAddOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [newTicket, setNewTicket] = useState<Omit<Ticket, 'id'>>({ description: '', status: TicketStatus.Open, date: new Date().toISOString() });
  const [editingTicket, setEditingTicket] = useState<Ticket | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [filterOpen, setFilterOpen] = useState(false);
  const [filterCriteria, setFilterCriteria] = useState<{ description: string, status: TicketStatus | '', date: string, id: string }>({
    description: '',
    status: '',
    date: '',
    id: ''
  });
  const [successMessage, setSuccessMessage] = useState<string | null>(null); 
  const [sortField, setSortField] = useState<keyof Ticket | null>(null);
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('asc');


  const fetchTickets = useCallback(async (page = 1, pageSize = 10) => {
    const params = {
      page,
      pageSize,
      ...filterCriteria, 
      sortField, 
      sortOrder
    };
    const response = await axios.get<PagedResult<Ticket>>('https://localhost:7227/api/tickets', { params });
    setTickets(response.data.records);
    setTotalPages(response.data.totalPages);
  }, [filterCriteria, sortField, sortOrder]);
  

  useEffect(() => {
    fetchTickets(currentPage);
  }, [currentPage, filterCriteria, sortField, sortOrder, fetchTickets]);

  const handleSort = (field: keyof Ticket) => {
    const newSortOrder = (sortField === field && sortOrder === 'asc') ? 'desc' : 'asc';
    setSortField(field);
    setSortOrder(newSortOrder);
    fetchTickets(1); 
  };

  const handleOpenAddDialog = () => {
    setNewTicket({ description: '', status: TicketStatus.Open, date: new Date().toISOString() });
    setAddOpen(true);
  };

  const handleCloseAddDialog = () => {
    setAddOpen(false);
  };

  const handleOpenEditDialog = (ticket: Ticket) => {
    setEditingTicket(ticket);
    setNewTicket({ description: ticket.description, status: ticket.status, date: ticket.date });
    setEditOpen(true);
  };

  const handleCloseEditDialog = () => {
    setEditingTicket(null);
    setEditOpen(false);
  };

  const handleInputChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | React.ChangeEvent<{ value: unknown; name?: string }>) => {
    const { name, value } = e.target;
  
    if (name === 'date') {
      const dateInUTC = new Date(value as string).toISOString();
      setNewTicket((prev) => ({ ...prev, [name]: dateInUTC }));
    } else {
      setNewTicket((prev) => ({ ...prev, [name as string]: value as string }));
    }
  };
  
  const validateForm = () => {
    return !!newTicket.description;
  };

  const handleSelectChange = (e: SelectChangeEvent<string>) => {
    const { name, value } = e.target;
    const statusValue = value ? parseInt(value) as TicketStatus : TicketStatus.Open;
    setNewTicket((prev) => ({ ...prev, [name as string]: statusValue }));
  };


  const handleAddTicket = async () => {
    if (!validateForm()) return;
    try {
      await axios.post('/api/tickets', newTicket);
      setSuccessMessage('Ticket was added successfully');
      fetchTickets(currentPage);
      handleCloseAddDialog();
    } catch (error) {
      console.error('Error adding ticket:', error);
    }
  };

  const handleUpdateTicket = async () => {
    if (!validateForm()) return;
    try {
      if (editingTicket) {
        await axios.put(`/api/tickets/${editingTicket.id}`, newTicket);
        setSuccessMessage('Ticket was modified successfully');
      }
      fetchTickets(currentPage);
      handleCloseEditDialog();
    } catch (error) {
      console.error('Error updating ticket:', error);
    }
  };

  const handleDeleteTicket = async (id: number) => {
    await axios.delete(`/api/tickets/${id}`);
    setSuccessMessage('Ticket was deleted successfully');
    fetchTickets(currentPage);
  };

  const handlePageChange = (event: React.ChangeEvent<unknown>, value: number) => {
    setCurrentPage(value);
  };

  const handleOpenFilterDialog = () => {
    setFilterOpen(true);
  };
  
  const handleCloseFilterDialog = (reset = false) => {
    if (reset) {
        setFilterCriteria({
            description: '',
            status: '',
            date: '',
            id: ''
        });
    }
    setFilterOpen(false);
};

const handleFilterChange = (e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement> | SelectChangeEvent<string>) => {
  const { name, value } = e.target;
  if (name === 'status') {
    setFilterCriteria((prev) => ({ ...prev, [name]: value ? parseInt(value) as TicketStatus : TicketStatus.Open }));
  } else {
    setFilterCriteria((prev) => ({ ...prev, [name]: value }));
  }
};

  const isDescriptionFilled = newTicket.description.trim() !== '';

  const mapStatusToText = (status: TicketStatus) => {
    return status === TicketStatus.Open ? 'Open' : 'Closed';
  };


  return (

    <div>
    {/* Success message outside of the dialog-container */}
    {successMessage && ( 
      <Alert severity="success" onClose={() => setSuccessMessage(null)} style={{ marginBottom: '20px' }}>
        <AlertTitle>Success</AlertTitle>
        {successMessage}
      </Alert>
    )}

    <div className="dialog-container">
      <div className="button-container">
        <IconButton onClick={handleOpenFilterDialog} className="icon-button">
          <FilterAltIcon />
        </IconButton>
      </div>

  {/* Filter Dialog */}
  <Dialog open={filterOpen}
  onClose={() => handleCloseFilterDialog(false)}
  maxWidth="sm"
  fullWidth
  className="dialog-container">
    <DialogTitle className="dialog-title"> 
        Filter
    </DialogTitle>
    <DialogContent className="dialog-content">
        <label htmlFor="id">Ticket Id</label>
        <TextField
        margin="dense"
        name="id"
        type="number"
        fullWidth
        placeholder="Enter a ticket Id"
        value={filterCriteria.id}
        onChange={handleFilterChange}
        InputLabelProps={{ shrink: true }}
    />
    
    <label>Description</label>
    <TextField
      margin="dense"
      name="description"
      placeholder="Enter a description"
      type="text"
      fullWidth
      value={filterCriteria.description}
      onChange={handleFilterChange}
      InputLabelProps={{
        shrink: true,
      }}
    />
    
    <label>Status</label>
    <Select
      labelId="status-label"
      name="status"
      value={filterCriteria.status.toString()}
      onChange={handleFilterChange}
      fullWidth
      displayEmpty
      inputProps={{ 'aria-label': 'Without label' }}
    >
      <MenuItem value="" disabled>
        <em>Enter a status</em>
      </MenuItem>
      <MenuItem value={TicketStatus.Open}>Open</MenuItem>
      <MenuItem value={TicketStatus.Closed}>Closed</MenuItem>
    </Select>
    </DialogContent>
  
    <DialogActions className="dialog-actions" style={{ justifyContent: 'center', marginBottom: '10px' }}>  
    <Button
      variant="contained"
      onClick={() => {
          handleCloseFilterDialog(true); 
      }}
      className="dialog-button-reset"
      >
      Reset
  </Button>
    
  <Button
      variant="contained"
      onClick={() => {
          fetchTickets(1); 
          handleCloseFilterDialog(false); 
      }}
      className="dialog-button-apply"
      startIcon={<DoneIcon />}
    >
      Apply
  </Button>
    </DialogActions>
    </Dialog>

    {/* Pagination */}
    <div style={{ marginBottom: '20px' }}>
    <Stack spacing={2} justifyContent="center" alignItems="center">
        <Pagination
        count={totalPages}
        page={currentPage}
        onChange={handlePageChange}
        variant="text"
        shape="rounded"
        siblingCount={1}
        boundaryCount={1}
        showFirstButton
        showLastButton
        className="pagination" 
        />
    </Stack>
    </div>

    
    <TableContainer component={Paper} className="table-container">
    <Table>
    <TableHead>
              <TableRow className="table-header">
                <TableCell className="table-cell" onClick={() => handleSort('id')} style={{ cursor: 'pointer' }}>
                  Ticket Id {sortField === 'id' ? (sortOrder === 'asc' ? '↑' : '↓') : ''}
                </TableCell>
                <TableCell className="table-cell">
                  Description
                </TableCell>
                <TableCell className="table-cell">
                Status
              </TableCell>
                <TableCell className="table-cell" onClick={() => handleSort('date')} style={{ cursor: 'pointer' }}>
                  Date {sortField === 'date' ? (sortOrder === 'asc' ? '↑' : '↓') : ''}
                </TableCell>
                <TableCell className="table-cell">Actions</TableCell>
              </TableRow>
    </TableHead>
    <TableBody>
    {tickets.map((ticket, index) => (
    <TableRow key={ticket.id} className={index % 2 === 0 ? 'table-row-even' : 'table-row-odd'}>
        <TableCell className="table-cell">{ticket.id}</TableCell>
        <TableCell className="table-cell">{ticket.description}</TableCell>
        <TableCell className="table-cell">{mapStatusToText(ticket.status)}</TableCell>
        <TableCell className="table-cell">
        {`${new Date(ticket.date).toLocaleString('en-US', { month: 'long' }).toLowerCase()}-${new Date(ticket.date).getDate()}-${new Date(ticket.date).getFullYear()}`}
        </TableCell>
        <TableCell>
            <Button style={{ color: '#9c27b0', textTransform: 'none' }} onClick={() => handleOpenEditDialog(ticket)}>Update</Button>
            <Button style={{ color: '#9c27b0', textTransform: 'none' }} onClick={() => handleDeleteTicket(ticket.id)}>Delete</Button>
        </TableCell>
    </TableRow>
    ))}
    </TableBody>
    </Table>
    </TableContainer>


    {/* Add Button for Creating a Ticket */}
    <div className="add-button-container">
    <Button
        variant="contained"
        color="primary"
        onClick={handleOpenAddDialog}
        className="add-button" 
    >
        Add New
    </Button>
    </div>
    {/* Add Ticket Dialog */}
    <Dialog open={addOpen} onClose={handleCloseAddDialog} maxWidth="sm" fullWidth>
    <DialogTitle className="dialog-title">Add New Ticket</DialogTitle>
    <DialogContent className="dialog-content">
        <TextField
        autoFocus
        required
        margin="dense"
        name="description"
        label="Description"
        type="text"
        fullWidth
        value={newTicket.description}
        onChange={handleInputChange}
        className="text-field"
        style={{ marginBottom: '20px' }}
        />
        <Select
        margin="dense"
        name="status"
        value={newTicket.status.toString()}
        onChange={handleSelectChange}
        fullWidth
        className="select-field"
        >
        <MenuItem value={TicketStatus.Open}>Open</MenuItem>
        <MenuItem value={TicketStatus.Closed}>Closed</MenuItem>
        </Select>
    </DialogContent>
    <DialogActions className="dialog-actions" style={{ justifyContent: 'center', marginBottom: '10px' }}>
        <Button variant="contained" onClick={handleCloseAddDialog} className="cancel-button">
        Cancel
        </Button>
        <Button
        variant="contained"
        onClick={handleAddTicket}
        disabled={!isDescriptionFilled}
        className={`add-button ${!isDescriptionFilled ? 'disabled' : ''}`}
        >
        <AddIcon sx={{ marginRight: '8px' }} />
        Add
        </Button>
    </DialogActions>
    </Dialog>

      {/* Edit Ticket Dialog */}
      <Dialog open={editOpen} onClose={handleCloseEditDialog} maxWidth="sm" fullWidth>
        <DialogTitle className="dialog-title">Edit Ticket</DialogTitle>
        <DialogContent className="dialog-content">
            <TextField
            autoFocus
            margin="dense"
            name="description"
            label="Description"
            type="text"
            fullWidth
            value={newTicket.description}
            onChange={handleInputChange}
            className="text-field"
            />
            <Select
            margin="dense"
            name="status"
            value={newTicket.status.toString()}
            onChange={handleSelectChange}
            fullWidth
            className="select-field"
            >
            <MenuItem value={TicketStatus.Open}>Open</MenuItem>
            <MenuItem value={TicketStatus.Closed}>Closed</MenuItem>
            </Select>
        </DialogContent>
        <DialogActions className="dialog-actions" style={{ justifyContent: 'center', marginBottom: '10px' }}>
            <Button variant="contained" onClick={handleCloseEditDialog} className="cancel-button">
            Cancel
            </Button>
            <Button
            variant="contained"
            onClick={handleUpdateTicket}
            disabled={!isDescriptionFilled}
            className={`update-button ${!isDescriptionFilled ? 'disabled' : ''}`}

            >
            <EditIcon sx={{ marginRight: '8px' }} />
            Update
            </Button>
        </DialogActions>
    </Dialog>

    </div>
    </div>
  );
}
