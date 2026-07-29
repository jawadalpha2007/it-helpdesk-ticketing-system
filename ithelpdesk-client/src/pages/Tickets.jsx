import { useState, useEffect } from "react";
import api from "../api/axiosConfig";
import { useNavigate } from "react-router-dom";

function Tickets() {
  const [tickets, setTickets] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

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

        const response = await api.get("/Tickets", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        setTickets(response.data);
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

  return (
     <div className="container">
      <h2>My Tickets</h2>

      
      <button onClick={() => navigate("/tickets/create")}>+ Create Ticket</button>
<button onClick={handleLogout} style={{ marginLeft: "10px" }}>Logout</button>


      {tickets.length === 0 ? (
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
    {tickets.map((ticket) => (
      <tr key={ticket.id}>
        <td>{ticket.ticketReference}</td>
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