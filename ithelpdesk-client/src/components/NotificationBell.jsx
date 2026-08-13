import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import api from "../api/axiosConfig";

function NotificationBell() {
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [showDropdown, setShowDropdown] = useState(false);
  const connectionRef = useRef(null);

  const token = localStorage.getItem("token");
  const headers = { Authorization: `Bearer ${token}` };

  const fetchNotifications = async () => {
    try {
      const [notifRes, countRes] = await Promise.all([
        api.get("/notifications", { headers }),
        api.get("/notifications/unread-count", { headers }),
      ]);
      setNotifications(notifRes.data);
      setUnreadCount(countRes.data.count);
    } catch (err) {
      console.error("Failed to fetch notifications", err);
    }
  };

  useEffect(() => {
    if (!token) return;

    fetchNotifications();

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7082/hubs/notifications", {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveNotification", (payload) => {
      setNotifications((prev) => [payload, ...prev]);
      setUnreadCount((prev) => prev + 1);
    });

    connection.start().catch((err) => console.error("SignalR connection failed:", err));
    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  const handleMarkAsRead = async (id) => {
    try {
      await api.post(`/notifications/${id}/read`, {}, { headers });
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch (err) {
      console.error("Failed to mark as read", err);
    }
  };

  if (!token) return null;

  return (
    <div style={{ position: "relative" }}>
      <button
        className="secondary"
        onClick={() => setShowDropdown(!showDropdown)}
        style={{ position: "relative" }}
      >
        🔔 Notifications
        {unreadCount > 0 && (
          <span
            style={{
              position: "absolute",
              top: "-6px",
              right: "-6px",
              backgroundColor: "#dc2626",
              color: "white",
              borderRadius: "50%",
              fontSize: "11px",
              width: "18px",
              height: "18px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            {unreadCount}
          </span>
        )}
      </button>

      {showDropdown && (
        <div
          style={{
            position: "absolute",
            right: 0,
            top: "40px",
            width: "320px",
            maxHeight: "400px",
            overflowY: "auto",
            backgroundColor: "white",
            border: "1px solid #e2e8f0",
            borderRadius: "8px",
            boxShadow: "0 4px 12px rgba(0,0,0,0.15)",
            zIndex: 100,
          }}
        >
          {notifications.length === 0 ? (
            <p style={{ padding: "16px", color: "#64748b" }}>No notifications yet.</p>
          ) : (
            notifications.map((n) => (
              <div
                key={n.id}
                onClick={() => !n.isRead && handleMarkAsRead(n.id)}
                style={{
                  padding: "12px 16px",
                  borderBottom: "1px solid #f1f5f9",
                  backgroundColor: n.isRead ? "white" : "#eff6ff",
                  cursor: n.isRead ? "default" : "pointer",
                  color: "#1e293b",
                }}
              >
                <div style={{ fontSize: "14px" }}>{n.message}</div>
                <div style={{ fontSize: "12px", color: "#64748b", marginTop: "4px" }}>
                  {new Date(n.createdAt).toLocaleString()}
                </div>
              </div>
            ))
          )}
        </div>
      )}
    </div>
  );
}

export default NotificationBell;