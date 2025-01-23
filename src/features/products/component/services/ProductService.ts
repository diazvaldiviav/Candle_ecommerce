import { Product } from './types/product.types';
import { api } from '../../../../services/api/axios';


const Product_URL = '/api/Products';

export const productService = {
  getAllProducts: async (): Promise<Product[]> => {
    const response = await api.get<Product[]>(Product_URL);
    return response.data;
  },

  // const response = await api.get<Category[]>(CATEGORIES_URL);

  getProductById: async (id: number): Promise<Product> => {
    const response = await api.get<Product>(`${Product_URL}/${id}`);
    return response.data;
  },

  getProductsBySubcategory: async (subcategoryId: number): Promise<Product[]> => {
    const response = await api.get<Product[]>(`${Product_URL}/subcategory/${subcategoryId}`);
    console.log('Productos por subcategoría:', response.data);
    return response.data;
  }
};