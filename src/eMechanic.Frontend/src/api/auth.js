import api from './client';

export const login = async (email, password, isWorkshop = false) => {
  const endpoint = isWorkshop ? '/api/v1/tokens/workshop' : '/api/v1/tokens/user';
  const response = await api.post(endpoint, { email, password });
  return response.data;
};

export const googleLogin = async (token) => {
  // Matches POST /api/v1/tokens/external/user/google
  // Payload: { idToken: string }
  const response = await api.post('/api/v1/tokens/external/user/google', { idToken: token });
  return response.data;
};

export const refreshToken = async (accessToken, refreshToken) => {
  // Matches POST /api/v1/tokens/refresh
  // Payload: { accessToken: string, refreshToken: string }
  const response = await api.post('/api/v1/tokens/refresh', { accessToken, refreshToken });
  return response.data;
};

export const getCurrentUser = async () => {
  const response = await api.get('/api/v1/users/me');
  return response.data;
};

export const logout = async () => {
  // No endpoint in spec, client-side only
  return Promise.resolve();
};

export const registerUser = async (userData) => {
  const response = await api.post('/api/v1/users', userData);
  return response.data;
};

export const registerWorkshop = async (workshopData) => {
  const response = await api.post('/api/v1/workshops', workshopData);
  return response.data;
};
