import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { Category } from "../category.types";
import { categoryService } from "../services/category.services";

interface CategoryState {
  categories: Category[];
  loading: boolean;
  error: string | null;
  selectedCategory: Category | null;
}

const initialState: CategoryState = {
  categories: [],
  selectedCategory: null,
  loading: false,
  error: null,
};

// Thunk para obtener todas las categorías
export const fetchCategories = createAsyncThunk(
  "categories/fetchAll",
  async (_, { rejectWithValue }) => {
    try {
      console.log("Iniciando fetchCategories...");
      const categories = await categoryService.getAll();
      console.log("Categorías obtenidas:", categories);
      return categories;
    } catch (error) {
      console.error("Error en fetchCategories:", error);
      return rejectWithValue("Error al cargar las categorías");
    }
  }
);

// Nuevo thunk para obtener categoría por ID
export const fetchCategoryById = createAsyncThunk(
  "categories/fetchById",
  async (id: number, { rejectWithValue }) => {
    try {
      return await categoryService.getById(id);
    } catch (error) {
      console.error("Error en fetchCategoryById:", error);
      return rejectWithValue("Error al cargar la categoría: " + error);
    }
  }
);

// Nuevo thunk para obtener categoría con subcategorías
export const fetchCategoryWithSubcategories = createAsyncThunk(
  "categories/fetchWithSubcategories",
  async (id: number, { rejectWithValue }) => {
    try {
      return await categoryService.getWithSubcategories(id);
    } catch (error) {
      console.error("Error en fetchCategoryWithSubcategories:", error);
      return rejectWithValue("Error al cargar la categoría con subcategorías");
    }
  }
);

const categorySlice = createSlice({
  name: "categories",
  initialState,
  reducers: {
    clearSelectedCategory: (state) => {
      state.selectedCategory = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchCategories.pending, (state) => {
        console.log("Estado: Loading...");
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCategories.fulfilled, (state, action) => {
        console.log("Estado: Success!", action.payload);
        state.loading = false;
        state.categories = action.payload;
      })
      .addCase(fetchCategories.rejected, (state, action) => {
        console.log("Estado: Error!", action.payload);
        state.loading = false;
        state.error = action.payload as string;
      })
      // Nuevos casos para fetchCategoryById
      .addCase(fetchCategoryById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCategoryById.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedCategory = action.payload;
      })
      .addCase(fetchCategoryById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // Nuevos casos para fetchCategoryWithSubcategories
      .addCase(fetchCategoryWithSubcategories.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchCategoryWithSubcategories.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedCategory = action.payload;
      })
      .addCase(fetchCategoryWithSubcategories.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearSelectedCategory } = categorySlice.actions;
export default categorySlice.reducer;
