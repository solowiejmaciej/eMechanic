import api from './client';

export const getWorkshops = async (params = {}) => {
  const response = await api.get('/api/v1/workshops', { params });
  return response.data;
};

export const getWorkshopDocuments = async (workshopId, params = {}) => {
  const response = await api.get(`/api/v1/workshops/${workshopId}/documents`, { params });
  return response.data;
};
