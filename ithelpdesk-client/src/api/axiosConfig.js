import axios from "axios";

const api = axios.create({
  baseURL: "https://localhost:7082/api",
});

export default api;