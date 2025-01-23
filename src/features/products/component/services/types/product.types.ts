// Interfaces para los subtipos
export interface ProductColor {
    colorId: number;
    colorName: string;
    hexCode: string;
    stock: number;
  }
  
  export interface ProductSize {
    sizeId: number;
    sizeName: string;
    stock: number;
  }
  
  // Interfaz principal del producto
  export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    stock: number;
    subcategoryId: number;
    subcategoryName: string;
    imageUrl: string;
    createdAt: string;
    productColors: ProductColor[];
    productSizes: ProductSize[];
  }
  
  // Interface para el estado en Redux
  export interface ProductState {
    products: Product[];
    selectedProduct: Product | null;
    loading: boolean;
    error: string | null;
  }