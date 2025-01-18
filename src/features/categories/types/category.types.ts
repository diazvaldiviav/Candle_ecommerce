export interface Category {
    id: number;
    name: string;
    description: string;
    imageUrl: string;
    createdAt: string;
    subcategories: SubCategory[];
  }
  
  export interface SubCategory {
    id: number;
    name: string;
    description: string;
    imageUrl: string;
    createdAt: string;
    categoryId: number;
  }