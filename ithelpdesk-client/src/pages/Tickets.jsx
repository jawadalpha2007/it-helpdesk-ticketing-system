import { useState, useEffect } from "react";
import api from "../api/axiosConfig";
import { useNavigate } from "react-router-dom";

function Tickets() {
  const [tickets, setTickets] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
const [statusFilter, setStatusFilter] = useState("");
const [priorityFilter, setPriorityFilter] = useState("");
const [statuses, setStatuses] = useState([]);
const [priorities, setPriorities] = useState([]);
  const navigate = useNavigate();

const role = localStorage.getItem("role");
const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("fullName");
    localStorage.removeItem("role");
    navigate("/login");
};
const handleDelete = async (id) => {
    const confirmed = window.confirm("Are you sure you want to delete this ticket?");
    if (!confirmed) return;

    try {
        const token = localStorage.getItem("token");
        await api.delete(`/Tickets/${id}`, {
            headers: { Authorization: `Bearer ${token}` },
        });

        // Remove the deleted ticket from our current list, without refetching everything
        setTickets(tickets.filter((t) => t.id !== id));
    } catch (err) {
        alert("Failed to delete ticket.");
    }
};

  useEffect(() => {
  const fetchTickets = async () => {
    try {
      const token = localStorage.getItem("token");
      const headers = { Authorization: `Bearer ${token}` };

      const [ticketsRes, statusRes, priorityRes] = await Promise.all([
        api.get("/Tickets", { headers }),
        api.get("/statuses", { headers }),
        api.get("/priorities", { headers }),
      ]);

      setTickets(ticketsRes.data);
      setStatuses(statusRes.data);
      setPriorities(priorityRes.data);
    } catch (err) {
      setError("Failed to load tickets. Please log in again.");
    } finally {
      setLoading(false);
    }
  };

  fetchTickets();
}, []);

  if (loading) return <p>Loading tickets...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;
const filteredTickets = tickets.filter((ticket) => {
  const matchesSearch =
    ticket.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    ticket.ticketReference.toLowerCase().includes(searchTerm.toLowerCase());

  const matchesStatus = statusFilter === "" || ticket.statusName === statusFilter;
  const matchesPriority = priorityFilter === "" || ticket.priorityName === priorityFilter;

  return matchesSearch && matchesStatus && matchesPriority;
});
  return (
    <div className="container">
      <h2>My Tickets</h2>

      {(role === "Employee" || role === "Admin") && (
        <button onClick={() => navigate("/tickets/create")}>+ Create Ticket</button>
      )}
      
      <div style={{ display: "flex", gap: "10px", margin: "16px 0", flexWrap: "wrap" }}>
        <input
          type="text"
          placeholder="Search by title or reference..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          style={{ flex: "1", minWidth: "200px" }}
        />

        <select value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Statuses</option>
          {statuses.map((s) => (
            <option key={s.id} value={s.statusName}>{s.statusName}</option>
          ))}
        </select>

        <select value={priorityFilter} onChange={(e) => setPriorityFilter(e.target.value)}>
          <option value="">All Priorities</option>
          {priorities.map((p) => (
            <option key={p.id} value={p.priorityName}>{p.priorityName}</option>
          ))}
        </select>
      </div>

      {filteredTickets.length === 0 ? (
        <p>No tickets found.</p>
      ) : (
        <table border="1" cellPadding="8" style={{ width: "100%", borderCollapse: "collapse" }}>
          <thead>
            <tr>
              <th>Reference</th>
              <th>Title</th>
              <th>Category</th>
              <th>Priority</th>
              <th>Status</th>
              <th>Created</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredTickets.map((ticket) => (
              <tr key={ticket.id}>
                <td>
                  <a href="#" onClick={(e) => { e.preventDefault(); navigate(`/tickets/${ticket.id}`); }}>
                    {ticket.ticketReference}
                  </a>
                </td>
                <td>{ticket.title}</td>
                <td>{ticket.categoryName}</td>
                <td>{ticket.priorityName}</td>
                <td>{ticket.statusName}</td>
                <td>{new Date(ticket.createdAt).toLocaleDateString()}</td>
                <td>
                  <button onClick={() => navigate(`/tickets/edit/${ticket.id}`)}>
                    Edit
                  </button>
                   <button
  className="danger"
  onClick={() => handleDelete(ticket.id)}
  style={{ marginLeft: "5px" }}
>
  Delete
</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default Tickets;
     