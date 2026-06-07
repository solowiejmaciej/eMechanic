import api from './client';

export const create = async (vehicleId, workshopId, description) => {
  const response = await api.post('/api/v1/repair-requests', { vehicleId, workshopId, description });
  return response.data;
};

export const getByVehicleId = async (vehicleId, params = {}) => {
  const response = await api.get(`/api/v1/repair-requests/vehicle/${vehicleId}`, { params });
  return response.data;
};

export const acceptEstimation = async (id) => {
  const response = await api.put(`/api/v1/repair-requests/${id}/accept`);
  return response.data;
};

export const rejectEstimation = async (id, reason) => {
  const response = await api.put(`/api/v1/repair-requests/${id}/reject`, { reason });
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/api/v1/repair-requests/${id}`);
  return response.data;
};

export const getSummary = async (id) => {
  const response = await api.get(`/api/v1/repair-requests/${id}/summarize`);
  return response.data;
};

export const getWorkshopRequests = async (params = {}) => {
  const response = await api.get('/api/v1/repair-requests', { params });
  return response.data;
};

export const provideEstimation = async (id, diagnosis, cost, currency = 'PLN') => {
  const response = await api.put(`/api/v1/repair-requests/${id}/estimation`, { diagnosis, cost, currency });
  return response.data;
};
