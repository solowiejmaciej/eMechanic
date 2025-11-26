import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 300000, // 5 minutes
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Inject Token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle 401 & Refresh
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // If 401 and not already retrying
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        const accessToken = localStorage.getItem('accessToken');

        if (refreshToken && accessToken) {
          // Matches POST /api/v1/tokens/refresh
          // Payload: { accessToken: string, refreshToken: string }
          const response = await axios.post(`${import.meta.env.VITE_API_URL}/api/v1/tokens/refresh`, {
            accessToken: accessToken,
            refreshToken: refreshToken
          });

          // Response: { token, refreshToken, expiresAtUtc }
          const { token: newAccessToken, refreshToken: newRefreshToken } = response.data;

          localStorage.setItem('accessToken', newAccessToken);
          localStorage.setItem('refreshToken', newRefreshToken);

          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
          return api(originalRequest);
        }
      } catch (refreshError) {
        console.error("Token refresh failed:", refreshError);
        // Clear tokens and redirect to login
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        window.location.href = '/login';
      }
    }

    return Promise.reject(error);
  }
);

export default api;
