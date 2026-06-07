import api from './client';

export const getRepairs = async (params = {}) => {
  const response = await api.get('/api/v1/repairs/user', { params });
  return response.data;
};

export const getRepairById = async (id) => {
  const response = await api.get(`/api/v1/repairs/user/${id}`);
  return response.data;
};

export const getWorkshopRepairs = async (params = {}) => {
  const response = await api.get('/api/v1/repairs/workshop', { params });
  return response.data;
};

export const startRepair = async (id) => {
  const response = await api.put(`/api/v1/repairs/${id}/start`);
  return response.data;
};

export const completeRepair = async (id, amount, currency = 'PLN') => {
  const response = await api.put(`/api/v1/repairs/${id}/complete`, { amount, currency });
  return response.data;
};
