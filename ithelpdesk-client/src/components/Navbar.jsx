import { useNavigate } from "react-router-dom";

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

  if (!fullName) return null; // don't show navbar if not logged in

  return (
    <div className="navbar">
      <h1>IT Help Desk</h1>
      <div className="user-info">
        Welcome, {fullName} ({role})
        <button className="secondary" onClick={handleLogout} style={{ marginLeft: "16px" }}>
          Logout
        </button>
      </div>
    </div>
  );
}

export default Navbar;