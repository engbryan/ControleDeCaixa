import axios from 'axios';
import authHandler from './authHandler';

const API_URL = 'https://yls407cds8.execute-api.us-east-1.amazonaws.com/prod/entrycontrol-reports-lambda';

export default {
  async getReports() {
    try {
      const token = authHandler.getToken();
      const response = await axios.get(API_URL, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      return response.data;
    } catch (error) {
      console.error('Erro ao buscar relatórios:', error);
      return [];
    }
  },
};
