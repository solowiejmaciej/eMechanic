import api from './client';

export const updateProfile = async (profileData) => {
  const response = await api.put('/api/v1/users', profileData);
  return response.data;
};

export const getRepairPreferences = async () => {
  const response = await api.get('/api/v1/user/repair-preferences');
  return response.data;
};

export const updateRepairPreferences = async (preferencesData) => {
  const response = await api.put('/api/v1/user/repair-preferences', preferencesData);
  return response.data;
};

export const getRepairPreferencesForWorkshop = async (userId) => {
  const response = await api.get(`/api/v1/user/repair-preferences/${userId}`);
  return response.data;
};

