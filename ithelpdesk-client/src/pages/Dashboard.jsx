import { useState, useEffect } from "react";
import api from "../api/axiosConfig";
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip,
  PieChart, Pie, Cell, ResponsiveContainer, Legend
} from "recharts";

const COLORS = ["#3b82f6", "#f59e0b", "#a855f7", "#22c55e", "#64748b"];

function Dashboard() {
  const [stats, setStats] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(true);

  const token = localStorage.getItem("token");
  const headers = { Authorization: `Bearer ${token}` };

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const res = await api.get("/dashboard/stats", { headers });
        setStats(res.data);
      } catch (err) {
        setError("Failed to load dashboard stats.");
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading) return <p className="container">Loading dashboard...</p>;
  if (error) return <p className="container error-text">{error}</p>;
  if (!stats) return null;

  return (
    <div className="container">
      <h2>Dashboard</h2>

      {/* KPI Cards */}
      <div style={{ display: "flex", gap: "16px", flexWrap: "wrap", marginBottom: "28px" }}>
        <KpiCard label="Total Tickets" value={stats.totalTickets} color="#2563eb" />
        <KpiCard label="Open" value={stats.openTickets} color="#3b82f6" />
        <KpiCard label="In Progress" value={stats.inProgressTickets} color="#f59e0b" />
        <KpiCard label="Resolved" value={stats.resolvedTickets} color="#22c55e" />
        <KpiCard
          label="Avg Resolution"
          value={stats.averageResolutionHours != null ? `${stats.averageResolutionHours.toFixed(1)}h` : "N/A"}
          color="#a855f7"
        />
      </div>

      {/* Charts */}
      <div style={{ display: "flex", gap: "24px", flexWrap: "wrap" }}>
        <div style={{ flex: "1", minWidth: "300px", height: "300px" }}>
          <h3 className="section-title" style={{ marginTop: 0 }}>Tickets by Status</h3>
          <ResponsiveContainer width="100%" height="100%">
            <PieChart>
              <Pie
                data={stats.ticketsByStatus}
                dataKey="count"
                nameKey="statusName"
                cx="50%"
                cy="50%"
                outerRadius={90}
                label
              >
                {stats.ticketsByStatus.map((entry, index) => (
                  <Cell key={index} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div style={{ flex: "1", minWidth: "300px", height: "300px" }}>
          <h3 className="section-title" style={{ marginTop: 0 }}>Tickets by Priority</h3>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={stats.ticketsByPriority}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="priorityName" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="count" fill="#2563eb" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        <div style={{ flex: "1", minWidth: "300px", height: "300px" }}>
          <h3 className="section-title" style={{ marginTop: 0 }}>Tickets by Category</h3>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={stats.ticketsByCategory}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="categoryName" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="count" fill="#22c55e" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}

function KpiCard({ label, value, color }) {
  return (
    <div style={{
      flex: "1",
      minWidth: "140px",
      padding: "16px",
      borderRadius: "8px",
      backgroundColor: "white",
      border: `1px solid #e2e8f0`,
      borderTop: `4px solid ${color}`,
    }}>
      <div style={{ fontSize: "12px", color: "#64748b", textTransform: "uppercase" }}>{label}</div>
      <div style={{ fontSize: "26px", fontWeight: 700, color: "#1e293b" }}>{value}</div>
    </div>
  );
}

export default Dashboard;