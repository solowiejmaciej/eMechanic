import api from "./client";

export const getWorkshops = async (params = {}) => {
  const response = await api.get("/api/v1/workshops", { params });
  return response.data;
};

export const getWorkshopDocuments = async (workshopId, params = {}) => {
  const response = await api.get(`/api/v1/workshops/${workshopId}/documents`, {
    params,
  });
  return response.data;
};

export const upsertReview = async (workshopId, rating, comment) => {
  const response = await api.put(`/api/v1/workshops/${workshopId}/reviews`, {
    rating,
    comment,
  });
  return response.data;
};

export const deleteReview = async (workshopId) => {
  const response = await api.delete(`/api/v1/workshops/${workshopId}/reviews`);
  return response.data;
};

export const getReviews = async (workshopId, params = {}) => {
  const response = await api.get(`/api/v1/workshops/${workshopId}/reviews`, {
    params,
  });
  return response.data;
};

export const getReviewStats = async (workshopId) => {
  const response = await api.get(
    `/api/v1/workshops/${workshopId}/reviews/stats`,
  );
  return response.data;
};

export const uploadWorkshopDocument = async (formData) => {
  const response = await api.post(
    `/api/v1/workshops/documents`,
    formData,
  );
  return response.data;
};

export const deleteWorkshopDocument = async (documentId) => {
  const response = await api.delete(
    `/api/v1/workshops/documents/${documentId}`,
  );
  return response.data;
};
