import { api } from '../../../../services/api/axios';
import { Category } from '../category.types';

const CATEGORIES_URL = '/api/Categories';

export const categoryService = {
  getAll: async (): Promise<Category[]> => {
    console.log('Haciendo petición a:', CATEGORIES_URL);
    try {
      const response = await api.get<Category[]>(CATEGORIES_URL);
      console.log('Respuesta de la API:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error en el servicio:', error);
      throw error;
    }
  },

   // Nuevo método para obtener categoría por ID
   getById: async (id: number): Promise<Category> => {
    const response = await api.get<Category>(`${CATEGORIES_URL}/${id}`);
    return response.data;
  },

   // Nuevo método para obtener categoría con subcategorías
   getWithSubcategories: async (id: number): Promise<Category> => {
    const response = await api.get<Category>(`${CATEGORIES_URL}/with-subcategories/${id}`);
    console.log('Respuesta de la API con subcategorías:', response.data);
    return response.data;
  }
};