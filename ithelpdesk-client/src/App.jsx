 import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Navbar from "./components/Navbar";
import Login from "./pages/Login";
import Tickets from "./pages/Tickets";
import CreateTicket from "./pages/CreateTicket";
import EditTicket from "./pages/EditTicket";
import TicketDetail from "./pages/TicketDetail";
import Dashboard from "./pages/Dashboard";

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/tickets" element={<Tickets />} />
        <Route path="/tickets/create" element={<CreateTicket />} />
        <Route path="/tickets/edit/:id" element={<EditTicket />} />
        <Route path="/tickets/:id" element={<TicketDetail />} />
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/dashboard" element={<Dashboard />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;