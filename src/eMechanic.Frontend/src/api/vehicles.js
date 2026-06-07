import api from "./client";

export const getAll = async (params = {}) => {
  const response = await api.get("/api/v1/vehicles", { params });
  return response.data;
};

export const getById = async (id) => {
  const response = await api.get(`/api/v1/vehicles/${id}`);
  return response.data;
};

export const create = async (vehicleData) => {
  const response = await api.post("/api/v1/vehicles", vehicleData);
  return response.data;
};

export const update = async (id, vehicleData) => {
  const response = await api.put(`/api/v1/vehicles/${id}`, vehicleData);
  return response.data;
};

export const deleteVehicle = async (id) => {
  const response = await api.delete(`/api/v1/vehicles/${id}`);
  return response.data;
};

export { deleteVehicle as delete };

export const getTimeline = async (id, params = {}) => {
  const response = await api.get(`/api/v1/vehicles/${id}/timeline`, { params });
  return response.data;
};

export const getDocuments = async (vehicleId, params = {}) => {
  const response = await api.get(`/api/v1/vehicles/${vehicleId}/documents`, {
    params,
  });
  return response.data;
};

export const uploadDocument = async (vehicleId, formData) => {
  const response = await api.post(
    `/api/v1/vehicles/${vehicleId}/documents`,
    formData,
  );
  return response.data;
};

export const downloadDocument = async (vehicleId, documentId) => {
  const response = await api.get(
    `/api/v1/vehicles/${vehicleId}/documents/${documentId}/download`,
    {
      responseType: "blob",
    },
  );
  return response.data;
};

export const deleteDocument = async (vehicleId, documentId) => {
  const response = await api.delete(
    `/api/v1/vehicles/${vehicleId}/documents/${documentId}`,
  );
  return response.data;
};
