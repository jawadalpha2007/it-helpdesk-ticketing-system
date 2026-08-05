import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api/axiosConfig";

function EditTicket() {
  const { id } = useParams();
  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [priorityId, setPriorityId] = useState("");
  const [statusId, setStatusId] = useState("");
  const [assignedTo, setAssignedTo] = useState(null);
   
  const [categories, setCategories] = useState([]);
  const [priorities, setPriorities] = useState([]);
  const [statuses, setStatuses] = useState([]);

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const headers = { Authorization: `Bearer ${token}` };

        const [ticketRes, catRes, priRes, statusRes] = await Promise.all([
          api.get(`/Tickets/${id}`, { headers }),
          api.get("/categories", { headers }),
          api.get("/priorities", { headers }),
          api.get("/statuses", { headers }),
        ]);

        const ticket = ticketRes.data;
        setTitle(ticket.title);
        setDescription(ticket.description);
        setCategories(catRes.data);
        setPriorities(priRes.data);
        setStatuses(statusRes.data);
        setAssignedTo(ticket.assignedToId ?? null);

        // Match names back to IDs for the dropdowns
        const matchedCategory = catRes.data.find(c => c.categoryName === ticket.categoryName);
        const matchedPriority = priRes.data.find(p => p.priorityName === ticket.priorityName);
        const matchedStatus = statusRes.data.find(s => s.statusName === ticket.statusName);

        setCategoryId(matchedCategory ? matchedCategory.id : "");
        setPriorityId(matchedPriority ? matchedPriority.id : "");
        setStatusId(matchedStatus ? matchedStatus.id : "");
      } catch (err) {
        setError("Failed to load ticket data.");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      await api.put(
        `/Tickets/${id}`,
        {
          title: title,
          description: description,
          categoryId: parseInt(categoryId),
          priorityId: parseInt(priorityId),
          statusId: parseInt(statusId),
          assignedTo: assignedTo,
        },
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      navigate("/tickets");
    } catch (err) {
      setError("Failed to update ticket.");
    }
  };

  if (loading) return <p>Loading ticket...</p>;

  return (
    <div className="container">
      <h2>Edit Ticket</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label>Title</label>
          <br />
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            style={{ width: "100%" }}
          />
        </div>
        <br />

        <div>
          <label>Description</label>
          <br />
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            rows={4}
            style={{ width: "100%" }}
          />
        </div>
        <br />

        <div>
          <label>Category</label>
          <br />
          <select value={categoryId} onChange={(e) => setCategoryId(e.target.value)} required>
            <option value="">-- Select a category --</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>{cat.categoryName}</option>
            ))}
          </select>
        </div>
        <br />

        <div>
          <label>Priority</label>
          <br />
          <select value={priorityId} onChange={(e) => setPriorityId(e.target.value)} required>
            <option value="">-- Select a priority --</option>
            {priorities.map((pri) => (
              <option key={pri.id} value={pri.id}>{pri.priorityName}</option>
            ))}
          </select>
        </div>
        <br />
{role !== "Employee" && (
  <>
    <div>
      <label>Status</label>
      <br />
      <select value={statusId} onChange={(e) => setStatusId(e.target.value)} required>
        <option value="">-- Select a status --</option>
        {statuses.map((s) => (
          <option key={s.id} value={s.id}>{s.statusName}</option>
        ))}
      </select>
    </div>
    <br />
  </>
)}
        {error && <p style={{ color: "red" }}>{error}</p>}

        <button type="submit">Save Changes</button>
        <button type="button" onClick={() => navigate("/tickets")} style={{ marginLeft: "10px" }}>
          Cancel
        </button>
      </form>
    </div>
  );
}

export default EditTicket;