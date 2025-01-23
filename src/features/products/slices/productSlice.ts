import { createAsyncThunk, createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Product, ProductState } from '../component/services/types/product.types';
import { productService } from '../component/services/ProductService';

const initialState: ProductState = {
  products: [],
  selectedProduct: null,
  loading: false,
  error: null,
};

// 1. Primero creamos el slice y exportamos las acciones
export const productSlice = createSlice({
  name: 'products',
  initialState,
  reducers: {
    startLoading: (state) => {
      state.loading = true;
      state.error = null;
    },
    loadProductsSuccess: (state, action: PayloadAction<Product[]>) => {
      state.loading = false;
      state.products = action.payload;
      state.error = null;
    },
    loadProductSuccess: (state, action: PayloadAction<Product>) => {
      state.loading = false;
      state.selectedProduct = action.payload;
      state.error = null;
    },
    setError: (state, action: PayloadAction<string>) => {
      state.loading = false;
      state.error = action.payload;
    },
    filterProductsBySubcategory: (state, action) => {
      state.loading = false;
      state.products = action.payload;
      state.error = null;
    },
  },
});

// 2. Exportamos las acciones
export const {
  startLoading,
  loadProductsSuccess,
  loadProductSuccess,
  setError,
  filterProductsBySubcategory,
} = productSlice.actions;

//tunk para cargar todos los productos
export const fetchProducts = createAsyncThunk(
  'product/fetchProducts',
  async (_, { dispatch }) => {
    try {
      dispatch(startLoading());
      const products = await productService.getAllProducts();
      dispatch(loadProductsSuccess(products));
    } catch (error) {
      dispatch(setError(error instanceof Error ? error.message : 'Error al cargar los productos'));
      throw error;
    }
  }
);

//tunk para filtrar productos por subcategoría
export const fetchProductsBySubcategory = createAsyncThunk(
  'product/fetchProductsBySubcategory',
  async (subcategoryId: number, { dispatch }) => {
    try {
      dispatch(startLoading());
      const products = await productService.getProductsBySubcategory(subcategoryId);
      dispatch(filterProductsBySubcategory(products));
    } catch (error) {
      dispatch(setError(error instanceof Error ? error.message : 'Error al filtrar los productos'));
      throw error;
    }
  }
);



export const fetchProductById = createAsyncThunk(
  'product/fetchProductById',
  async (id: number, { dispatch }) => {
    try {
      dispatch(startLoading());
      const product = await productService.getProductById(id);
      dispatch(loadProductSuccess(product));
    } catch (error) {
      dispatch(setError(error instanceof Error ? error.message : 'Error al cargar el producto'));
      throw error;
    }
  }
);

// 4. Finalmente exportamos el reducer
export default productSlice.reducer;