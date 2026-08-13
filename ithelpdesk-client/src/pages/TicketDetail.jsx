 import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../api/axiosConfig";

function formatResolutionTime(hours) {
  if (hours < 1) {
    return `${Math.round(hours * 60)} minutes`;
  } else if (hours < 24) {
    return `${hours.toFixed(1)} hours`;
  } else {
    const days = Math.floor(hours / 24);
    const remainingHours = Math.round(hours % 24);
    return `${days} day${days !== 1 ? "s" : ""}, ${remainingHours} hour${remainingHours !== 1 ? "s" : ""}`;
  }
}

function TicketDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const token = localStorage.getItem("token");
  const role = localStorage.getItem("role");

  const [ticket, setTicket] = useState(null);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const [isInternal, setIsInternal] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);
  const [logs, setLogs] = useState([]);
  const [agents, setAgents] = useState([]);
  const [selectedAgent, setSelectedAgent] = useState("");
  const [attachments, setAttachments] = useState([]);
  const [selectedFile, setSelectedFile] = useState(null);
  const [commentFile, setCommentFile] = useState(null);

  const headers = { Authorization: `Bearer ${token}` };

  const fetchData = async () => {
    try {
      const requests = [
        api.get(`/Tickets/${id}`, { headers }),
        api.get(`/tickets/${id}/comments`, { headers }),
        api.get(`/Tickets/${id}/logs`, { headers }),
        api.get(`/Tickets/${id}/attachments`, { headers }),
      ];

      if (role === "Manager" || role === "Admin") {
        requests.push(api.get("/agents", { headers }));
      }

      const results = await Promise.all(requests);

      setTicket(results[0].data);
      setComments(results[1].data);
      setLogs(results[2].data);
      setAttachments(results[3].data);


      if (results[4]) {
        setAgents(results[4].data);
      }
    } catch (err) {
      setError("Failed to load ticket details.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [id]);

   const handleAddComment = async (e) => {
  e.preventDefault();
  if (!newComment.trim()) return;

  try {
    await api.post(
      `/tickets/${id}/comments`,
      { commentText: newComment, isInternal: isInternal },
      { headers }
    );

    if (commentFile) {
      const formData = new FormData();
      formData.append("file", commentFile);

      await api.post(`/Tickets/${id}/attachments`, formData, {
        headers: { ...headers, "Content-Type": "multipart/form-data" },
      });
    }

    setNewComment("");
    setIsInternal(false);
    setCommentFile(null);
    fetchData();
  } catch (err) {
    setError("Failed to post comment.");
  }
};

  const handleSelfAssign = async () => {
    try {
      await api.post(`/Tickets/${id}/self-assign`, {}, { headers });
      fetchData();
    } catch (err) {
      alert(err.response?.data?.message || "Failed to self-assign ticket.");
    }
  };

  const handleAssign = async (e) => {
    e.preventDefault();
    if (!selectedAgent) return;

    try {
      await api.post(
        `/Tickets/${id}/assign`,
        { agentId: parseInt(selectedAgent) },
        { headers }
      );
      setSelectedAgent("");
      fetchData();
    } catch (err) {
      alert(err.response?.data?.message || "Failed to assign ticket.");
    }
  };
  const handleFileUpload = async (e) => {
  e.preventDefault();
  if (!selectedFile) return;

  const formData = new FormData();
  formData.append("file", selectedFile);

  try {
    await api.post(`/Tickets/${id}/attachments`, formData, {
      headers: { ...headers, "Content-Type": "multipart/form-data" },
    });
    setSelectedFile(null);
    fetchData();
  } catch (err) {
    alert(err.response?.data?.message || "Failed to upload file.");
  }
};

  if (loading) return <p className="container">Loading...</p>;
  if (error) return <p className="container error-text">{error}</p>;
  if (!ticket) return <p className="container">Ticket not found.</p>;

  const canSeeInternal = role !== "Employee";

  return (
    <div className="container">
      <button className="secondary" onClick={() => navigate("/tickets")}>
        ← Back to Tickets
      </button>

      <div className="detail-header">
        <div>
          <h2 style={{ marginBottom: "4px" }}>{ticket.title}</h2>
          <span style={{ color: "#64748b", fontSize: "14px" }}>{ticket.ticketReference}</span>
        </div>
        <span className={`status-badge status-${ticket.statusName.replace(/\s/g, "").toLowerCase()}`}>
          {ticket.statusName}
        </span>
      </div>

      <p style={{ marginBottom: "20px" }}>{ticket.description}</p>

      <div className="detail-grid">
        <div className="detail-field">
          <label>Category</label>
          <span>{ticket.categoryName}</span>
        </div>
        <div className="detail-field">
          <label>Priority</label>
          <span>{ticket.priorityName}</span>
        </div>
        <div className="detail-field">
          <label>Created By</label>
          <span>{ticket.createdByName}</span>
        </div>
        <div className="detail-field">
          <label>Assigned To</label>
          <span>{ticket.assignedToName || "Unassigned"}</span>
        </div>
        <div className="detail-field">
          <label>Created</label>
          <span>{new Date(ticket.createdAt).toLocaleString()}</span>
        </div>
        {ticket.resolvedAt && (
          <div className="detail-field">
            <label>Resolved</label>
            <span>
              {new Date(ticket.resolvedAt).toLocaleString()}
              {ticket.resolutionTimeHours != null &&
                ` (took ${formatResolutionTime(ticket.resolutionTimeHours)})`}
            </span>
          </div>
        )}
      </div>

      {role === "IT Support Agent" && !ticket.assignedToName && (
        <div className="assign-box">
          <button onClick={handleSelfAssign}>Claim This Ticket</button>
        </div>
      )}

      {(role === "Manager" || role === "Admin") && (
        <div className="assign-box">
          <form onSubmit={handleAssign} style={{ display: "flex", gap: "8px", alignItems: "flex-end" }}>
            <div style={{ flex: 1 }}>
              <label>Assign to Agent</label>
              <select value={selectedAgent} onChange={(e) => setSelectedAgent(e.target.value)} required>
                <option value="">-- Select an agent --</option>
                {agents.map((a) => (
                  <option key={a.id} value={a.id}>{a.fullName}</option>
                ))}
              </select>
            </div>
            <button type="submit">Assign</button>
          </form>
        </div>
      )}
    <h3 className="section-title">Attachments</h3>
{attachments.length === 0 ? (
  <p>No attachments yet.</p>
) : (
  <ul>
    {attachments.map((a) => (
      <li key={a.id}>
        <a href={`https://localhost:7082${a.filePath}`} target="_blank" rel="noreferrer">
          {a.fileName}
        </a>{" "}
        <span className="comment-meta">
          ({(a.fileSize / 1024).toFixed(1)} KB) — uploaded by {a.uploadedByName}
        </span>
      </li>
    ))}
  </ul>
)}

<form onSubmit={handleFileUpload} style={{ marginTop: "8px" }}>
  <input type="file" onChange={(e) => setSelectedFile(e.target.files[0])} />
  <button type="submit" style={{ marginLeft: "8px" }}>Upload</button>
</form>
      <h3 className="section-title">Comments</h3>
      {comments.length === 0 ? (
        <p>No comments yet.</p>
      ) : (
        comments.map((c) => (
          <div key={c.id} className={`comment-card ${c.isInternal ? "internal" : "regular"}`}>
            <span className="comment-author">
              {c.authorName}{" "}
              {c.isInternal && <span style={{ color: "#b45309", fontSize: "12px" }}>(Internal Note)</span>}
            </span>
            <p style={{ margin: "6px 0" }}>{c.commentText}</p>
            <div className="comment-meta">{new Date(c.createdAt).toLocaleString()}</div>
          </div>
        ))
      )}
      <div style={{ margin: "8px 0" }}>
  <label>Attach a file (optional)</label>
  <br />
  <input type="file" onChange={(e) => setCommentFile(e.target.files[0])} />
</div>

      <form onSubmit={handleAddComment} style={{ marginTop: "16px" }}>
        <textarea
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          placeholder="Write a comment..."
          rows={3}
          required
        />
        <br />
        {canSeeInternal && (
          <label style={{ fontWeight: "normal", display: "block", margin: "8px 0" }}>
            <input
              type="checkbox"
              checked={isInternal}
              onChange={(e) => setIsInternal(e.target.checked)}
              style={{ width: "auto", marginRight: "6px" }}
            />
            Mark as internal note (hidden from Employee)
          </label>
        )}
        <button type="submit">Post Comment</button>
      </form>

      <h3 className="section-title">Activity Log</h3>
      {logs.length === 0 ? (
        <p>No activity yet.</p>
      ) : (
        <div className="timeline">
          {logs.map((log) => (
            <div key={log.id} className="timeline-item">
              <div className="timeline-dot" />
              <div style={{ fontSize: "14px", fontWeight: 600 }}>{log.action}</div>
              <div className="comment-meta">
                {log.performedByName} — {new Date(log.createdAt).toLocaleString()}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default TicketDetail;