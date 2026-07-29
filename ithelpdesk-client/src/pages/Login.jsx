import { useState } from "react";
import api from "../api/axiosConfig";
import { useNavigate } from "react-router-dom";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const response = await api.post("/Auth/login", {
        email: email,
        password: password,
      });

      const { id,token, fullName, role } = response.data;

      // Temporarily store the token so we can use it later
      localStorage.setItem("userId", id);
      localStorage.setItem("token", token);
      localStorage.setItem("fullName", fullName);
      localStorage.setItem("role", role);

      navigate("/tickets");
    } catch (err) {
      setError("Invalid email or password.");
    }
  };

  return (
   <div className="container">
      <h2>IT Help Desk - Login</h2>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email</label>
          <br />
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <br />
        <div>
          <label>Password</label>
          <br />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <br />
        {error && <p style={{ color: "red" }}>{error}</p>}
        <button type="submit">Login</button>
      </form>
    </div>
  );
}

export default Login;