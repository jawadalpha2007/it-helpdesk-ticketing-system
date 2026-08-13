 import { useNavigate } from "react-router-dom";
import NotificationBell from "./NotificationBell";

function Navbar() {
  const navigate = useNavigate();
  const fullName = localStorage.getItem("fullName");
  const role = localStorage.getItem("role");

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userId");
    localStorage.removeItem("fullName");
    localStorage.removeItem("role");
    navigate("/login");
  };

  if (!fullName) return null;

  return (
    <div className="navbar">
      <h1>IT Help Desk</h1>
      <div className="user-info" style={{ display: "flex", alignItems: "center", gap: "12px" }}>
        <span>Welcome, {fullName} ({role})</span>

        <NotificationBell />

        <button className="secondary" onClick={() => navigate("/dashboard")}>
          Dashboard
        </button>

        <button className="secondary" onClick={handleLogout}>
          Logout
        </button>
      </div>
    </div>
  );
}

export default Navbar;