import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axiosConfig";

function CreateTicket() {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [priorityId, setPriorityId] = useState("");

  const [categories, setCategories] = useState([]);
  const [priorities, setPriorities] = useState([]);

  const [error, setError] = useState("");
  const navigate = useNavigate();

  const token = localStorage.getItem("token");

  useEffect(() => {
    const fetchLookups = async () => {
      try {
        const catResponse = await api.get("/categories", {
          headers: { Authorization: `Bearer ${token}` },
        });
        const priResponse = await api.get("/priorities", {
          headers: { Authorization: `Bearer ${token}` },
        });

        setCategories(catResponse.data);
        setPriorities(priResponse.data);
      } catch (err) {
        setError("Failed to load categories/priorities.");
      }
    };

    fetchLookups();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const userId = parseInt(localStorage.getItem("userId"));

      await api.post(
        "/Tickets",
        {
          title: title,
          description: description,
          categoryId: parseInt(categoryId),
          priorityId: parseInt(priorityId),
          createdBy: userId,
        },
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );

      navigate("/tickets");
    } catch (err) {
      setError("Failed to create ticket. Please check your input.");
    }
  };

  return (
    <div className="container">
      <h2>Create New Ticket</h2>
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
          <select
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            required
          >
            <option value="">-- Select a category --</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.categoryName}
              </option>
            ))}
          </select>
        </div>
        <br />

        <div>
          <label>Priority</label>
          <br />
          <select
            value={priorityId}
            onChange={(e) => setPriorityId(e.target.value)}
            required
          >
            <option value="">-- Select a priority --</option>
            {priorities.map((pri) => (
              <option key={pri.id} value={pri.id}>
                {pri.priorityName}
              </option>
            ))}
          </select>
        </div>
        <br />

        {error && <p style={{ color: "red" }}>{error}</p>}

        <button type="submit">Create Ticket</button>
        <button type="button" onClick={() => navigate("/tickets")} style={{ marginLeft: "10px" }}>
          Cancel
        </button>
      </form>
    </div>
  );
}

export default CreateTicket;