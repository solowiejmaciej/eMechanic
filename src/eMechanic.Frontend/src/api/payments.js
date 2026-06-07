import api from './client';

export const initializePayment = async (paymentData) => {
  const response = await api.post('/api/v1/payments/initialize', paymentData);
  return response.data;
};
