// src/hooks/useProducts.ts
import { useState, useEffect, useMemo } from 'react';
import { Product } from '../features/products/component/services/types/product.types';
import { productService } from '../features/products/component/services/ProductService';

export const useProducts = () => {
  const [allProducts, setAllProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortOption, setSortOption] = useState('newest');
  const [selectedSubcategory, setSelectedSubcategory] = useState<number | null>(null);
  const ITEMS_PER_PAGE = 8;

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        setLoading(true);
        let products: Product[];
        
        if (selectedSubcategory !== null) {
          products = await productService.getProductsBySubcategory(selectedSubcategory);
        } else {
          products = await productService.getAllProducts();
          console.log('selectedSubcategory es null');
        }
        console.log('Products fetched desde useProducts:', products);
        setAllProducts(products);
        setError(null);
      } catch (err) {
        setError('Error al cargar los productos');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, [selectedSubcategory]);

  // Filtrar y ordenar productos
  const filteredAndSortedProducts = useMemo(() => {
    let result = [...allProducts];

    // Aplicar búsqueda
    if (searchQuery) {
      result = result.filter(product =>
        product.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        product.description.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    // Aplicar ordenamiento
    switch (sortOption) {
      case 'price_low':
        result.sort((a, b) => a.price - b.price);
        break;
      case 'price_high':
        result.sort((a, b) => b.price - a.price);
        break;
      case 'name_asc':
        result.sort((a, b) => a.name.localeCompare(b.name));
        break;
      case 'name_desc':
        result.sort((a, b) => b.name.localeCompare(a.name));
        break;
      case 'newest':
      default:
        // Asumiendo que tienes un campo createdAt
        result.sort((a, b) => 
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        break;
    }

    return result;
  }, [allProducts, searchQuery, sortOption]);

  // Paginar productos
  const paginatedProducts = useMemo(() => {
    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const endIndex = startIndex + ITEMS_PER_PAGE;
    return filteredAndSortedProducts.slice(startIndex, endIndex);
  }, [filteredAndSortedProducts, currentPage]);

  const totalPages = Math.ceil(filteredAndSortedProducts.length / ITEMS_PER_PAGE);

  // Reset página cuando cambian los filtros
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery, sortOption]);

  return {
    products: paginatedProducts,
    loading,
    error,
    currentPage,
    totalPages,
    setCurrentPage,
    setSearchQuery,
    setSortOption,
    totalProducts: filteredAndSortedProducts.length,
    setSelectedSubcategory,
  };
};