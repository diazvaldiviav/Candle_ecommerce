// src/store/store.ts
import { configureStore } from '@reduxjs/toolkit';
import categorySlice from '../features/categories/types/slices/categorySlice';  // Importamos el reducer del slice
import productSlice from '../features/products/slices/productSlice';

export const store = configureStore({
  reducer: {
    products: productSlice,
    categories: categorySlice,  // Lo usamos aquí
    
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;