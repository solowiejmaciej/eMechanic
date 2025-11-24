import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 300000, // 5 minutes
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;
